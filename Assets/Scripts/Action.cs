using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    // Specific targets
    public enum targets{
        none,                // No targets for this ability type

        frontSingle,         // Exactly one target (front)
        backSingle,          // Exactly one target (back)
        anySingle,           // Exactly one target (any)

        frontRow,            // Can only target front row
        backRow,             // Can only target back row
        eitherRow,           // Front OR back row can be targeted
        bothRows,            // AOE effecting both rows
        
        player,              // Can only target player
        self                 // Can only target self
    }
    // Enemies vs allies target restriction
    public enum targetRestrictions{
        none,
        enemies,
        allies,
        both,
        self
    }
    // All possible status effects
    public enum statusEffect{
        // Debuffs
        slow,
        // Buffs
        haste
    }


    // Action name
    public string name;


    // Heal action values
    public bool isHealthEffect;
    public float healValue;
    // Heal restrictions
    public targets healTargetType;
    public targetRestrictions healTargetRestriction;


    // Damage action values
    public bool isDamageEffect;
    public float damageValue;
    // Damage restrictions
    public targets damageTargetType;
    public targetRestrictions damageTargetRestriction;


    // Status action values
    public bool isStatusEffect;
    public List<statusEffect> effectValues;    // Status effects
    public List<int> effectDurations;          // Associated int DURATION value (# of turns) for each effect
    // Status restrictions
    public targets statusTargetType;
    public targetRestrictions statusTargetRestriction;


}
