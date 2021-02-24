using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITargetable
{
    public PlayerObject playerObject;
    public List<CreatureObject> deck;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float currentDef;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentDamage;

    [HideInInspector] public int actionPoints;
    [HideInInspector] public bool enableTargeting;
    [HideInInspector] public Dictionary<Action.statusEffect, int> activeEffects;

    void Start()
    {
        loadDeck();
        enableTargeting = false;
        currentHealth = maxHealth = 100;
    }

    void OnMouseDown()
    {
        if(!enableTargeting)
            return;

        GameManager.Instance.targetWasClicked(this);
    }

    public void summonCreature()
    {
        void summonInRow(ITargetable target) {
            if(target.getTargetType() != ITargetable.TargetType.row)
            {
                Debug.LogError("Target other than row was selected to summon creature.");
                return;
            }
            RowManager row = (RowManager) target;
            row.summonCreature(drawCard(), this);
        }

        GameManager.Instance.performAfterTargetSelect(this, Action.targets.eitherRow, Action.targetRestrictions.allies, summonInRow);
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

    public void updateCurrentHealth(float num)
    {
        currentHealth += num;
        // If player's health goes above max health, drop it back to max
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
        // If HP drops to 0, this player is killed
        if(currentHealth <= 0){
            //end game
        }
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
        if( status.statusType == Action.statusEffectType.speed ){
            currentSpeed = playerObject.baseSpeed;
        }
        else if( status.statusType == Action.statusEffectType.defense ){
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

    public void performHealthOverTimeEffect(Action.statusEffect status)
    {
        if( status.statusType != Action.statusEffectType.healthOverTime ){
            return;
        }
        updateCurrentHealth(status.modifierValue);
    }
}
