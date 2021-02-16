using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "ScriptableObjects/PlayerObject", order = 2)]
public class PlayerObject : ScriptableObject
{
    [System.Serializable]
    public struct creatureAmount {
        public CreatureObject creatureType;
        public int quantity;
    }

    public List<creatureAmount> creatureDeck;
}
