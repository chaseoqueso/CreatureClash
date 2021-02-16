using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialDialogue
{
    public enum dialogueTriggers{
        // If opponent is specific characters
        opponentIsNecromancer,
        opponentIsDruid,
        opponentIsFire,

        // Creature died/resurrected
        ownCreatureDeath,
        enemyCreatureDeath,
        ownCreatureResurrected,
        enemyCreatureResurrected,

        // Damage dealt/taken
        damageDealt,
        damageTaken,

        // Almost dead
        opponentAlmostDead,
        selfAlmostDead
    }

    // List of triggers
    public List<dialogueTriggers> triggers;

    // List of lines
    public List<Dialogue.characterLine> characterLines;
}
