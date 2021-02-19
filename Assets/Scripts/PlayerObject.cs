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

    // Dialogue
    // ========
    // Start
    public List<Dialogue> challengeDialogue;
    // End
    public List<Dialogue> winDialogue;
    public List<Dialogue> lossDialogue;
    // Damage dealt/taken
    public List<Dialogue> damageDealtDialogue;
    public List<Dialogue> damageTakenDialogue;
    // Own creature dies
    public List<Dialogue> creatureDeathDialogue;
    // Almost dead
    public List<Dialogue> opponentAlmostDeadDialogue;
    public List<Dialogue> selfAlmostDeadDialogue;

    // Special dialogue
    public List<SpecialDialogue> specialDialogue;
}