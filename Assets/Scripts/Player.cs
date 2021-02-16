using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerObject playerObject;
    public List<CreatureObject> deck;

    [HideInInspector] public float health;
    [HideInInspector] public int actionPoints;
    [HideInInspector] public bool playerEnabled;

    void Start()
    {
        loadDeck();
    }

    void OnMouseDown()
    {
        if(!playerEnabled)
            return;
        
        Debug.Log(GameManager.Instance);
        GameManager.Instance.p1Front.summonCreature(drawCard());
    }

    public void loadDeck()
    {
        deck = new List<CreatureObject>();
        foreach(PlayerObject.creatureAmount c in playerObject.creatureDeck)
        {
            for(int i = 0; i < c.quantity; ++i)
            {
                deck.Insert(Random.Range(0, deck.Count), c.creatureType);
            }
        }
    }

    public CreatureObject drawCard()
    {
        CreatureObject ret = deck[0];
        deck.RemoveAt(0);
        return ret;
    }

    public void shuffleDeck()
    {
        for(int i = 0; i < deck.Count; ++i)
        {
            int index = Random.Range(0, deck.Count);
            CreatureObject temp = deck[index];
            deck[index] = deck[i];
            deck[i] = temp;
        }
    }
}
