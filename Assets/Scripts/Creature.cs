using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // Creature class takes in a CreatureObject creature that feeds all the creature info to this generic class
    public CreatureObject creature;

    // Saves the action list from the CreatureObject for more convenient access
    private List<Action> actions;

    // Current values
    public float currentHealth;

    public Action currentAction;
    public List<List<Creature>> currentTargets;

    // Current status effects
    public Dictionary<Action.statusEffect, int> activeEffects;


    // Start is called before the first frame update
    void Start()
    {
        activeEffects = new Dictionary<Action.statusEffect, int>();

        actions = creature.Actions();

        // on start, set current hp to max hp
        currentHealth = creature.MaxHealth();

        // set action stuff to null
        setIdle();
    }

    // Returns a list of all Actions this creature can do
    public List<Action> getActionList()
    {
        return actions;
    }

    public void updateCurrentHealth(float num)
    {
        // num + if healing, - if damage
        currentHealth += num;
        // If creature's health goes above max health, drop it back to max
        if(currentHealth > creature.MaxHealth()){
            currentHealth = creature.MaxHealth();
        }
        // If HP drops to 0, this creature is killed
        if(currentHealth <= 0){
            currentHealth = 0;
            killCreature();
        }
    }

    public void setNextAction(int actionIndex, List<List<Creature>> targets)
    {
        if( actionIndex >= actions.Count ){
            Debug.Log("Invalid Action Index");
            setIdle();
            return;
        }

        currentTargets = targets;
        currentAction = actions[actionIndex];
    }

    public void performNextAction()
    {
        // Loop through targets & effect groups
        // (List of target lists will always been in order of effect groups, & always == length)
        for( int i = 0; i < currentTargets.Count; i++ ){
            // For a single effect group, perform the action effects on those targets
            List<Creature> targets = currentTargets[i];
            List<Action.Effect> effectsOnTargets = currentAction.actionEffects[i].effectGroupList;

            foreach( Action.Effect effect in effectsOnTargets ){
                // If status effect, assign status to the effected targets
                if( effect.type == Action.effectType.status ){
                    foreach( Creature target in targets ){
                        target.setStatusEffect(effect.status, effect.effectDuration);
                    }
                }
                // If damage/heal, adjust hp of effected targets
                if( effect.type == Action.effectType.damage || effect.type == Action.effectType.heal ){
                    foreach( Creature target in targets ){
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

    public void setStatusEffect(Action.statusEffect effect, int duration)
    {
        // If the effect is not yet on this creature, add it and save the duration
        // Otherwise, update the duration
        activeEffects[effect] = duration;
    }


    // Called each round to update the duration values for active effects
    public void updateEffectDurations()
    {
        // Loop through all active effects and decrease duration by 1
        foreach( Action.statusEffect effect in activeEffects.Keys ){
            activeEffects[effect] = activeEffects[effect] - 1;
            // If the duration is 0, remove the effect
            if(activeEffects[effect] <= 0){
                activeEffects.Remove(effect);
            }
        }
    }

    public void killCreature()
    {
        // setIdle();

        // Play death animation / sound effects

        // Destroy this object
        // -> action resolution in other script must check that a creature still exists before performing the action
        Destroy(gameObject);

        // Or put it in the graveyard or whatever we're gonna do with dead creatures
    }
}
