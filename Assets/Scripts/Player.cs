using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITargetable
{
    public class SummonAction : PlayerAction {
        public int creatureIndex;
        public RowManager summonRow;

        public SummonAction(int index, RowManager rm)
        {
            creatureIndex = index;
            summonRow = rm;
        }
    }

    public PlayerObject playerObject;
    public List<CreatureObject> deck;
    public int playerNumber;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float currentDef;
    [HideInInspector] public float currentDamage;

    [HideInInspector] public int actionPoints;
    [HideInInspector] public bool enableTargeting;
    [HideInInspector] public List<CreatureObject> cardsInHand;
    [HideInInspector] public List<PlayerAction> queuedActions;
    [HideInInspector] public List<List<List<ITargetable>>> queuedTargets;
    [HideInInspector] public Dictionary<Action.statusEffect, int> activeEffects;

    private Material material;
    private Collider2D col;
    private Animation anim;

    void Awake()
    {
        enableTargeting = false;
        material = GetComponent<Renderer>().material;
        col = GetComponent<Collider2D>();
        queuedActions = new List<PlayerAction>();
        queuedTargets = new List<List<List<ITargetable>>>();
        activeEffects = new Dictionary<Action.statusEffect, int>();
        anim = GetComponentInChildren<Animation>();
    }

    void Start()
    {
        Debug.Log(playerObject);
        currentHealth = maxHealth = playerObject.baseHealth;
        material.mainTexture = playerObject.characterSprite;
        transform.localScale = new Vector3(playerObject.characterSprite.width/material.GetFloat("pixelsPerUnit"), playerObject.characterSprite.height/material.GetFloat("pixelsPerUnit"), 1);

        loadDeck();

        cardsInHand = new List<CreatureObject>();
        for(int i = 0; i < 4; ++i)
        {
            cardsInHand.Add(drawCard());
        }
    }

    void Update()
    {
        if(enableTargeting)
        {
            material.SetFloat("outlineIntensity", Mathf.Abs(Mathf.Sin(Time.time * 2f)));
        }
        else
        {
            material.SetFloat("outlineIntensity", 0);
        }
    }

    void OnMouseOver()
    {
        if(enableTargeting && Input.GetMouseButtonDown(0)) 
        {
            GameManager.Instance.targetWasClicked(this);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            clearActions();
        }
    }

    public void clearActions()
    {
        queuedActions.Clear();
        queuedTargets.Clear();
        actionPoints = GameManager.Instance.turnCount + 2;
        GameManager.Instance.resetTargeting();
    }

    public void drawCardsUntilFull()
    {
        for(int i = 0; i < cardsInHand.Count; ++i)
        {
            if(cardsInHand[i] == null)
            {
                cardsInHand.RemoveAt(i);
                --i;
            }
        }

        while(cardsInHand.Count < 4)
        {
            if(deck.Count == 0)
                break;

            cardsInHand.Add(drawCard());
        }
    }

    public List<int> getCurrentSummonIndices()
    {
        List<int> ret = new List<int>();
        foreach(Action a in queuedActions)
        {
            if(a is SummonAction)
            {
                ret.Add(((SummonAction)a).creatureIndex);
            }
        }
        return ret;
    }

    public void queueAction(PlayerAction action, List<List<ITargetable>> targets)
    {
        if(action.manaCost > actionPoints)
        {
            Debug.LogError("Jen didn't do her job.");
            return;
        }

        queuedActions.Add(action);
        queuedTargets.Add(targets);
        actionPoints -= action.manaCost;
    }

    public void queueSpell(int index)
    {
        List<List<ITargetable>> targetGroups = new List<List<ITargetable>>();

        void setActionCallback(ITargetable target)
        {
            if(target == null)
            {
                targetGroups.Add(this.getTargets());
            }
            else
            {
                targetGroups.Add(target.getTargets());
            }
        }

        PlayerAction action = playerObject.actions[index];

        foreach(Action.EffectGroup group in action.actionEffectGroups) {
            GameManager.Instance.performAfterTargetSelect(this, group.targetType, group.targetRestriction, group.blockedByFrontline, setActionCallback);
        }
            
        queueAction(action, targetGroups);
    }

    public void summonCreature(int index)
    {
        void summonInRow(ITargetable target) {
            if(target.getTargetType() != ITargetable.TargetType.row)
            {
                Debug.LogError("Target other than row was selected to summon creature.");
                return;
            }
            
            SummonAction summon = new SummonAction(index, (RowManager) target);
            summon.manaCost = cardsInHand[index].ManaCost();
            queueAction(summon, null);
        }

        GameManager.Instance.performAfterTargetSelect(this, Action.targets.eitherRow, Action.targetRestrictions.allies, false, summonInRow);
    }

    public void performQueuedAction(int actionIndex)
    {
        Action currentAction = queuedActions[actionIndex];

        // Check for null values
        if( currentAction == null ){
            Debug.LogError("Player is Idle and has no Action to perform.");
            return;
        }

        if(currentAction is SummonAction)
        {
            SummonAction s = (SummonAction)currentAction;
            s.summonRow.summonCreature(cardsInHand[s.creatureIndex], this);
            cardsInHand[s.creatureIndex] = null;
        }
        else
        {
            List<List<ITargetable>> currentTargets = queuedTargets[actionIndex];
            if( currentTargets == null ){
                Debug.LogError("Player has no Targets for assigned Action.");
                return;
            }

            // Loop through targets & effect groups
            // (List of target lists will always been in order of effect groups, & always == length)
            for( int i = 0; i < currentTargets.Count; i++ ){
                // For a single effect group, perform the action effects on those targets
                List<ITargetable> targets = currentTargets[i];
                List<Action.Effect> effectsOnTargets = currentAction.actionEffectGroups[i].groupEffects;

                foreach( Action.Effect effect in effectsOnTargets ){
                    // If status effect, assign status to the affected targets
                    if( effect.type == Action.effectType.status ) {
                        foreach( ITargetable target in targets ) {
                            if(target != null)
                                target.setStatusEffect(effect.status);
                        }
                    }
                    // If damage, adjust hp of affected targets
                    else if(effect.type == Action.effectType.damage){
                        foreach( ITargetable target in targets ){
                            if(target != null)
                                target.updateCurrentHealth(currentDamage * effect.hpMulti + effect.hpValue);
                        }
                    }
                    // If heal, adjust hp of affected targets
                    else if(effect.type == Action.effectType.heal){       // ( effect.type == Action.effectType.damage || effect.type == Action.effectType.heal ){
                        foreach( ITargetable target in targets ){
                            if(target != null)
                                target.updateCurrentHealth(maxHealth * effect.hpMulti + effect.hpValue);
                        }
                    }
                    // If special, call the special effect with the correct parameters
                    else if(effect.type == Action.effectType.special){       // ( effect.type == Action.effectType.damage || effect.type == Action.effectType.heal ){
                        effect.optionalSpecialEffect.performStatusEffect(this, targets);
                    }
                }
            }
        }
    }

    public void loadDeck()
    {
        deck = new List<CreatureObject>();
        foreach(PlayerObject.creatureAmount c in playerObject.creatureDeck)
        {
            for(int i = 0; i < c.quantity; ++i)
            {
                deck.Insert(Random.Range(0, deck.Count), c.creatureType);
            }
        }
    }

    public CreatureObject drawCard()
    {
        CreatureObject ret = deck[0];
        deck.RemoveAt(0);
        return ret;
    }

    public void shuffleDeck()
    {
        for(int i = 0; i < deck.Count; ++i)
        {
            int index = Random.Range(0, deck.Count);
            CreatureObject temp = deck[index];
            deck[index] = deck[i];
            deck[i] = temp;
        }
    }

    public ITargetable.TargetType getTargetType()
    {
        return ITargetable.TargetType.player;
    }

    public List<ITargetable> getTargets()
    {
        List<ITargetable> temp = new List<ITargetable>();
        temp.Add(this);
        return temp;
    }

    public float updateCurrentHealth(float num)
    {
        if(num < 0) //If taking damage, reduce by the creature's defense
        {
            num *= Mathf.Clamp01(1 - (currentDef/100f));
        }

        // num + if healing, - if damage
        currentHealth += num;

        float excess = 0;

        // If creature's health goes above max health, drop it back to max
        if(currentHealth > maxHealth){
            excess = currentHealth - maxHealth;
            currentHealth = maxHealth;
        }

        // If HP drops to 0, this creature is killed
        if(currentHealth <= 0){
            excess = currentHealth;
            currentHealth = 0;
            
            GameManager.Instance.StopAllCoroutines();
            Canvas canvas = DataManager.Instance.UICanvas;
            GameObject winScreenPanel = Instantiate(GameManager.Instance.winScreenPrefab, new Vector3(canvas.GetComponent<RectTransform>().rect.width/2, canvas.GetComponent<RectTransform>().rect.height/2, 0), Quaternion.identity, canvas.transform);
            winScreenPanel.GetComponent<WInScreenPanel>().setWinText(playerNumber);
        }
        
        //return the actual amount of damage/healing applied to the creature
        return num - excess;
    }

    public void setStatusEffect(Action.statusEffect status)
    {
        // If the effect is not yet on this creature, add it
        toggleStatusEffect(status, true);
        // Update the duration
        activeEffects[status] = status.effectDuration;
    }

    public void toggleStatusEffect(Action.statusEffect status, bool setActive)
    {
        // Enable the effect
        if( setActive ){
            // If this effect is already active on this creature, return
            if(activeEffects.ContainsKey(status)){
                return;
            }

            // Set affected status value
            else if( status.statusType == Action.statusEffectType.defense ){
                currentDef = currentDef * status.modifierMult + status.modifierValue;
            }
            else if( status.statusType == Action.statusEffectType.damage ){
                currentDamage = currentDamage * status.modifierMult + status.modifierValue;
            }
            else if( status.statusType == Action.statusEffectType.maxHealth ){
                maxHealth = maxHealth * status.modifierMult + status.modifierValue;
                if( currentHealth > maxHealth ){
                    currentHealth = maxHealth;
                }
            }
            return;
        }

        // Disable the effect (if !setActive)
        // If this effect is not on the creature, you don't need to remove it
        if( !activeEffects.ContainsKey(status) ){
            return;
        }
        // Return affected status value to base
        if( status.statusType == Action.statusEffectType.defense ){
            currentDef = playerObject.baseDef;
        }
        else if( status.statusType == Action.statusEffectType.damage ){
            currentDamage = playerObject.baseDamage;
        }
        else if( status.statusType == Action.statusEffectType.maxHealth ){
            maxHealth = playerObject.baseHealth;
            if( currentHealth > maxHealth ){
                currentHealth = maxHealth;
            }
        }
    }

    // Called at the start of each round (BEFORE CHOOSING ACTIONS)
    // Updates the duration values for active effects & performs health over time effects
    public void updateStatusEffects()
    {
        List<Action.statusEffect> effects = new List<Action.statusEffect>();
        foreach( Action.statusEffect effect in activeEffects.Keys ){
            effects.Add(effect);
        }
        
        // Loop through all active effects and decrease duration by 1
        foreach( Action.statusEffect effect in effects ){
            activeEffects[effect] = activeEffects[effect] - 1;

            // If health over time, perform the effect
            if( effect.statusType == Action.statusEffectType.healthOverTime ){
                performHealthOverTimeEffect(effect);
                
                if(activeEffects[effect] <= 0) {
                    activeEffects.Remove(effect);
                }
            }
            else
            {
                // If the duration is 0, the status will be removed at the end of the current turn
                if(activeEffects[effect] < 0){
                    toggleStatusEffect(effect, false);
                    activeEffects.Remove(effect);
                }
            }
        }
    }

    public void performHealthOverTimeEffect(Action.statusEffect status)
    {
        if( status.statusType != Action.statusEffectType.healthOverTime ){
            return;
        }
        updateCurrentHealth(status.modifierValue);
    }

    public void playAnimationClip(AnimationClip clip)
    {
        
    }
}
