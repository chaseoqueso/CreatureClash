using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "ScriptableObjects/PlayerObject", order = 2)]
public class PlayerObject : ScriptableObject
{
    // Character class info
    public string characterName;
    public string characterClass;
    public Sprite characterSprite;

    [TextArea(3, 10)]
    public string flavorText;       // For character select menu


    // Creature deck info
    [System.Serializable]
    public struct creatureAmount {
        public CreatureObject creatureType;
        public int quantity;
    }

    public List<creatureAmount> creatureDeck;


    // Character portrait image (for dialogue)
    public Sprite characterPortrait;

    // Dialogue                                         Priority
    // ========                                         ========
    // Start
    public List<Dialogue> challengeDialogue;            // 2
    // End
    public List<Dialogue> winDialogue;                  // 2
    public List<Dialogue> lossDialogue;                 // 2
    // Damage dealt/taken
    public List<Dialogue> damageDealtDialogue;          // 6
    public List<Dialogue> damageTakenDialogue;          // 6
    // Own creature dies
    public List<Dialogue> creatureDeathDialogue;        // 3
    // Almost dead
    public List<Dialogue> opponentAlmostDeadDialogue;   // 5
    public List<Dialogue> selfAlmostDeadDialogue;       // 4

    // Special dialogue
    public List<Dialogue> specialDialogue;              // 1
}
