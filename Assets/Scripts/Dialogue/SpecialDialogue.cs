using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialDialogue
{
    public enum dialogueTrigger{
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
        criticalHit,

        // Almost dead
        opponentAlmostDead,
        selfAlmostDead,
        bothAlmostDead
    }

    // List of triggers
    public List<dialogueTrigger> triggers;

    // List of lines
    public List<Dialogue.characterLine> characterLines;
}
