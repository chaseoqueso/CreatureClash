using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Action
{
    public static readonly Action swapRows = new SwapAction();
    
    // Specific targets
    public enum targets{
        none,                // No targets for this ability type

        frontSingle,         // Exactly one target (front)
        backSingle,          // Exactly one target (back, including player)
        backCreature,        // Exactly one target (back, excluding player)
        anySingle,           // Exactly one target (any, including player)
        anyCreature,         // Exactly one target (any, excluding player)
        self,                // Can only target self

        frontRow,            // Can only target front row
        backRow,             // Can only target back row
        eitherRow,           // Front OR back row can be targeted
        bothRows,            // AOE effecting both rows
        
        player               // Can only target player
    }
    // Enemies vs allies target restriction
    public enum targetRestrictions{
        none,
        enemies,
        allies,
        self
    }
    // Types of effects
    public enum effectType{
        damage,
        heal,
        status,
        swapRow,
        special
    }
    // All possible status effects
    public enum statusEffectType{
        // If no status effect
        none,
        // What stat this effect adjusts
        speed,
        damage,
        maxHealth,
        healthOverTime,
        defense
    }

    [System.Serializable]
    public class statusEffect{
        public statusEffectType statusType;

        public float modifierValue = 0;         // flat mod; 0 if no mod
        public float modifierMult = 1;          // % mod; 1 if no mult mod

        // Status effect duration (in # of turns)
        public int effectDuration;
    }

    
    // Effect class
    [System.Serializable]
    public class Effect
    {
        public string effectName;

        // Effect values
        public effectType type;

        // Status Effect struct w/ default values
        public statusEffect status = new statusEffect{statusType = statusEffectType.none,
                                                    modifierValue = 0, modifierMult = 1, effectDuration = 1};            // only used if type == status

        // Health or damage number
        public float hpValue = 0;                   // 0 if type == status
        public float hpMulti = 1;                   // 0 for no heal, 1 for no damage

        public SpecialEffectObject optionalSpecialEffect; //Use if effectType == special
    }
    // EffectGroup class
    [System.Serializable]
    public class EffectGroup
    {
        public string groupName;
        
        // Effect Group target
        public targets targetType;
        public targetRestrictions targetRestriction;
        public bool blockedByFrontline;

        // Effects
        public List<Effect> groupEffects;
    }


    // Action name
    public string actionName;
    // Action description
    [TextArea(3, 10)]
    public string description;
    // Action icon
    public Sprite actionIcon;
    //Animation for when a creature performs this action
    public AnimationClip attackAnim;
    //The normalized [0.0-1.0] time during the attack animation that the attack hits the target
    public float attackContactTime;
    //Animation for when a creature gets hit by this action
    public AnimationClip hitAnim;
    
    // All effects of the action
    public List<EffectGroup> actionEffectGroups;
    
    private class SwapAction : Action 
    {
        public SwapAction()
        {
            actionName = "Swap Rows";
            description = "The creature moves from its current row to the other row.";

            Effect swap = new Effect();
            swap.type = effectType.swapRow;

            EffectGroup group = new EffectGroup();
            group.groupEffects = new List<Effect>();
            group.groupEffects.Add(swap);
            group.targetRestriction = targetRestrictions.self;
            group.targetType = targets.self;

            actionEffectGroups = new List<EffectGroup>();
            actionEffectGroups.Add(group);
        }
    }

}