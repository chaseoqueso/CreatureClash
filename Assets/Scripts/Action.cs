using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        self
    }
    // Types of effects
    public enum effectType{
        damage,
        heal,
        status
    }
    // All possible status effects
    public enum statusEffect{
        // If no status effect
        none,
        // Debuffs
        slow,
        // Buffs
        haste
    }

    
    // Effect class
    [System.Serializable]
    public class Effect
    {
        public string effectName;

        // Effect values
        public effectType type;
        public statusEffect status;     // none if type != status

        // Health or damage number (+ for heal, - for damage)
        public float hpValue;           // 0 if type == status

        // Status effect duration (in # of turns)
        public int effectDuration;      // 0 if type != status
    }
    // EffectGroup class
    [System.Serializable]
    public class EffectGroup
    {
        public string groupName;
        
        // Effect Group target
        public targets targetType;
        public targetRestrictions targetRestriction;

        // Effects
        public List<Effect> effectGroupList;
    }


    // Action name
    public string actionName;
    // Action description
    public string description;
    
    // All effects of the action
    public List<EffectGroup> actionEffects;
    
}


    // [CustomEditor(typeof(Effect))]
    // public class ScriptEditor : Editor
    // {
    //     Effect effect;

    //     void OnEnable()
    //     {
    //         effect = (Effect)target;
    //     }

    //     public override void OnInspectorGUI()
    //     {
    //         effect.type = (effectType)EditorGUILayout.EnumPopup(effect.type); // "Effect Type", 

    //         switch(effect.type)
    //         {
    //             case effectType.status:
    //             {
    //                 // If this effect is a status effect
    //                 effect.status = (statusEffect)EditorGUILayout.EnumPopup(effect.status);
    //                 effect.effectDuration = EditorGUILayout.IntField(effect.effectDuration);
    //                 break;
    //             }
    //             case effectType.damage:
    //             case effectType.heal:
    //             {
    //                 // If effect is damage or heal
    //                 effect.hpValue = EditorGUILayout.FloatField(effect.hpValue); //"HP Value", 
    //                 break;
    //             }
    //         }
    //     }
    // }