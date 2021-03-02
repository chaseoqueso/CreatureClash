using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour, ITargetable
{
    // Creature class takes in a CreatureObject creature that feeds all the creature info to this generic class
    public CreatureObject creature;
    public Player player;
    public RowManager row;

    // Saves the action list from the CreatureObject for more convenient access
    private List<Action> actions;

    // Current values
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentMaxHP;
    [HideInInspector] public float currentDef;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentDamage;

    [HideInInspector] public Action currentAction;
    [HideInInspector] public List<List<ITargetable>> currentTargets;
    [HideInInspector] public bool enableTargeting;
    [HideInInspector] public bool justSummoned;
    [HideInInspector] public bool isDead;

    // Current status effects
    [HideInInspector] public Dictionary<Action.statusEffect, int> activeEffects;

    private Material material;
    private Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        activeEffects = new Dictionary<Action.statusEffect, int>();

        actions = creature.Actions();

        // on start, set current values to max values
        currentHealth = creature.MaxHealth();
        currentMaxHP = creature.MaxHealth();
        currentDef = creature.BaseDefense();
        currentSpeed = creature.BaseSpeed();
        currentDamage = creature.BaseDamage();

        // set action stuff to null
        setIdle();

        col = GetComponent<Collider2D>();
        material = GetComponent<Renderer>().material;
        material.mainTexture = creature.Texture();
        transform.localScale = new Vector3(creature.Texture().width/material.GetFloat("pixelsPerUnit"), creature.Texture().height/material.GetFloat("pixelsPerUnit"), 1);
        justSummoned = true;
    }

    void Update()
    {
        col.enabled = enableTargeting;
        transform.position = new Vector3(transform.position.x, transform.localScale.y/2 * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.x), transform.position.z);
        if(enableTargeting)
        {
            material.SetFloat("outlineIntensity", Mathf.Abs(Mathf.Sin(Time.time * 2f)));
        }
        else
        {
            material.SetFloat("outlineIntensity", 0);
        }
    }

    // Returns a list of all Actions this creature can do
    public List<Action> getActionList()
    {
        return actions;
    }

    public void updateCurrentHealth(float num)
    {
        if(isDead)
            return;
        // num + if healing, - if damage
        currentHealth += num * Mathf.Clamp01(1 - (currentDef/100f));
        Debug.Log(currentHealth);
        // If creature's health goes above max health, drop it back to max
        if(currentHealth > currentMaxHP){
            currentHealth = currentMaxHP;
        }
        // If HP drops to 0, this creature is killed
        if(currentHealth <= 0){
            currentHealth = 0;
            killCreature();
        }
    }

    public void selectTargetsForAction(int actionIndex)
    {
        List<List<ITargetable>> targetGroups = new List<List<ITargetable>>();

        void setActionCallback(ITargetable target)
        {
            targetGroups.Add(target.getTargets());
        }

        Action action = actions[actionIndex];

        foreach(Action.EffectGroup group in action.actionEffectGroups) {
            GameManager.Instance.performAfterTargetSelect(player, group.targetType, group.targetRestriction, group.blockedByFrontline, setActionCallback);
        }
            
        setNextAction(actionIndex, targetGroups);
    }

    public void setNextAction(int actionIndex, List<List<ITargetable>> targets)
    {
        if( actionIndex >= actions.Count || actionIndex < 0 ){
            Debug.Log("Invalid Action Index");
            setIdle();
            return;
        }

        currentTargets = targets;
        currentAction = actions[actionIndex];
    }

    public void performNextAction()
    {
        // Check for null values
        if( currentAction == null ){
            Debug.Log("Creature is Idle and has no Action to perform.");
            return;
        }
        if( currentTargets == null ){
            Debug.Log("Creature has no Targets for assigned Action.");
            return;
        }

        // Loop through targets & effect groups
        // (List of target lists will always been in order of effect groups, & always == length)
        for( int i = 0; i < currentTargets.Count; i++ ){
            // For a single effect group, perform the action effects on those targets
            List<ITargetable> targets = new List<ITargetable>();
            Action.EffectGroup currentEffectGroup = currentAction.actionEffectGroups[i];
            List<Action.Effect> effectsOnTargets = currentEffectGroup.groupEffects;

            if(currentEffectGroup.blockedByFrontline) //Process for selecting a new target if the attack can be blocked by the frontline
            {
                foreach(ITargetable target in currentTargets[i])
                {
                    ITargetable newTarget;
                    switch(target.getTargetType())
                    {
                        case ITargetable.TargetType.player:

                            newTarget = GameManager.Instance.selectRandomTargetInFrontline((Player)target);

                            if(newTarget != null) //if there's a creature in the frontline
                            {
                                if(currentEffectGroup.targetType != Action.targets.anySingle) //but the attack can't target the frontline
                                {
                                    targets.Add(null); //the attack misses
                                }
                                else //if the attack can target the frontline, we change from the intended target to the frontline target
                                {
                                    targets.Add(newTarget);
                                }
                            }
                            else
                            {
                                targets.Add(target);
                            }
                            break;

                        case ITargetable.TargetType.creature:

                            if(GameManager.Instance.creatureIsInFrontRow((Creature)target)) { //If the target is already in the frontline we're chillin
                                targets.Add(target);
                                break;
                            }

                            newTarget = GameManager.Instance.selectRandomTargetInFrontline(((Creature)target).player);

                            if(newTarget != null) //if there's a creature in the frontline
                            {
                                if(currentEffectGroup.targetType == Action.targets.backCreature || currentEffectGroup.targetType == Action.targets.backSingle) //but the attack can't target the frontline
                                {
                                    targets.Add(null); //the attack misses
                                }
                                else //if the attack can target the frontline, we change from the intended target to the frontline target
                                {
                                    targets.Add(newTarget);
                                }
                            }
                            else
                            {
                                targets.Add(target);
                            }
                            break;

                        case ITargetable.TargetType.row:

                            if(GameManager.Instance.rowIsFrontRow((RowManager)target)) { //If the target is already in the frontline we're chillin
                                targets.Add(target);
                                break;
                            }

                            newTarget = GameManager.Instance.selectFrontRowFromBackRow(((RowManager)target));

                            if(newTarget != null) //if our row is in fact the back row (which it should be)
                            {
                                if(currentEffectGroup.targetType == Action.targets.backRow) //but the attack can't target the frontline
                                {
                                    targets.Add(null); //the attack misses
                                }
                                else //if the attack can target the frontline, we change from the intended target to the frontline target
                                {
                                    targets.Add(newTarget);
                                }
                            }
                            else
                            {
                                targets.Add(target);
                            }
                            break;

                        default:
                            targets.Add(target);
                            break;
                    }
                }
            }

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
                            target.updateCurrentHealth(currentDamage * effect.damageMulti - effect.hpValue);
                    }
                }
                // If heal, adjust hp of affected targets
                else{       // ( effect.type == Action.effectType.damage || effect.type == Action.effectType.heal ){
                    foreach( ITargetable target in targets ){
                        if(target != null)
                            target.updateCurrentHealth(effect.hpValue);
                    }
                }
            }
        }

        // Set idle after performing the current action
        setIdle();
    }

    public void setIdle()
    {
        currentTargets = null;
        currentAction = null;
    }

    public void setStatusEffect(Action.statusEffect status)
    {
        // If the effect is not yet on this creature, add it
        toggleStatusEffect(status, true);
        // Update the duration
        activeEffects[status] = status.effectDuration;
    }

    public void performHealthOverTimeEffect(Action.statusEffect status)
    {
        if( status.statusType != Action.statusEffectType.healthOverTime ){
            return;
        }
        updateCurrentHealth(status.modifierValue + (currentMaxHP * status.modifierMult));
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
            if( status.statusType == Action.statusEffectType.speed ){
                currentSpeed = currentSpeed * status.modifierMult + status.modifierValue;
            }
            else if( status.statusType == Action.statusEffectType.defense ){
                currentDef = currentDef * status.modifierMult + status.modifierValue;
            }
            else if( status.statusType == Action.statusEffectType.damage ){
                currentDamage = currentDamage * status.modifierMult + status.modifierValue;
            }
            else if( status.statusType == Action.statusEffectType.maxHealth ){
                currentMaxHP = currentMaxHP * status.modifierMult + status.modifierValue;
                if( currentHealth > currentMaxHP ){
                    currentHealth = currentMaxHP;
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
        if( status.statusType == Action.statusEffectType.speed ){
            currentSpeed = creature.BaseSpeed();
        }
        else if( status.statusType == Action.statusEffectType.defense ){
            currentDef = creature.BaseDefense();
        }
        else if( status.statusType == Action.statusEffectType.damage ){
            currentDamage = creature.BaseDamage();
        }
        else if( status.statusType == Action.statusEffectType.maxHealth ){
            currentMaxHP = creature.MaxHealth();
            if( currentHealth > currentMaxHP ){
                currentHealth = currentMaxHP;
            }
        }
    }

    // Called at the start of each round (BEFORE CHOOSING ACTIONS)
    // Updates the duration values for active effects & performs health over time effects
    public void updateStatusEffects()
    {
        // Loop through all active effects and decrease duration by 1
        foreach( Action.statusEffect effect in activeEffects.Keys ){
            activeEffects[effect] = activeEffects[effect] - 1;

            // If health over time, perform the effect
            if( effect.statusType == Action.statusEffectType.healthOverTime ){
                performHealthOverTimeEffect(effect);
            }

            // If the duration is 0, remove the effect
            if(activeEffects[effect] <= 0){
                toggleStatusEffect(effect, false);
                activeEffects.Remove(effect);
            }
        }
    }

    public void killCreature()
    {
        setIdle();
        isDead = true;

        // Play death animation / sound effects

        // Destroy this object
        // -> action resolution in other script must check that a creature still exists or not in graveyard or whatever
        // before performing the action
        Destroy(gameObject);

        // Or put it in the graveyard or whatever we're gonna do with dead creatures
        // gameObject.GetComponentInParent<Graveyard>().deadCreatures.Add(this);
        // remove it from the playing field or whatever idk
    }
    
    public ITargetable.TargetType getTargetType()
    {
        return ITargetable.TargetType.creature;
    }

    public List<ITargetable> getTargets()
    {
        List<ITargetable> temp = new List<ITargetable>();
        temp.Add(this);
        return temp;
    }

    void OnMouseDown()
    {
        Debug.Log(enableTargeting);
        if(!enableTargeting)
            return;
        
        GameManager.Instance.targetWasClicked(this);
    }
}
