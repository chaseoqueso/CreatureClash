using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // Creature class takes in a CreatureObject creature that feeds all the creature info to this generic class
    public CreatureObject creature;
    public List<Action> actions;

    // Current values
    public float currentHealth;

    public Action currentAction;
    public List<Creature> currentTargets;

    // Current status effects
    public Dictionary<Action.statusEffect, int> activeEffects;


    // Start is called before the first frame update
    void Start()
    {
        activeEffects = new Dictionary<Action.statusEffect, int>();

        // on start, set current hp to max hp
        currentHealth = creature.MaxHealth();

        // set action stuff to null
        currentAction = null;
        currentTargets = null;
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
            killCreature();
        }
    }

    public void setNextAction(int actionIndex, List<Creature> targets)
    {
        if( actionIndex >= actions.Count ){
            Debug.Log("Invalid Action Index");
            currentTargets = null;
            currentAction = null;
            return;
        }

        currentTargets = targets;
        currentAction = actions[actionIndex];
    }

    public void setStatusEffect(Action.statusEffect newEffect, int duration)
    {
        // If the effect is not yet on this creature, add it and save the duration
        // if( !activeEffects.ContainsKey(newEffect) ){
        //     activeEffects.Add(newEffect, duration);
        //     return;
        // }
        // Otherwise, update the duration
        activeEffects[newEffect] = duration;
    }


    // Called each round to update the duration values for active effects
    public void updateEffectDurations()
    {
        // Loop through all active effects and decrease duration by 1
        // foreach( int effect in activeEffects.Keys ){
        //     activeEffects[effect] = activeEffects[effect] - 1;
        //     // If the duration is 0, remove the effect
        //     if(activeEffects[effect] <= 0){
        //         activeEffects.Remove(effect);
        //     }
        // }
    }

    public void killCreature()
    {
        // Play death animation / sound effects

        // Destroy this object
        // -> action resolution in other script must check that a creature still exists before performing the action
        Destroy(gameObject);
    }
}
