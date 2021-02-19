using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITargetable
{
    public PlayerObject playerObject;
    public List<CreatureObject> deck;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public int actionPoints;
    [HideInInspector] public bool enableTargeting;

    void Start()
    {
        loadDeck();
        enableTargeting = false;
        currentHealth = maxHealth = 100;
    }

    void OnMouseDown()
    {

        void summonInRow(ITargetable target) {
            if(target.getTargetType() != ITargetable.TargetType.row)
            {
                Debug.LogError("Target other than row was selected to summon creature.");
                return;
            }
            RowManager row = (RowManager) target;
            row.summonCreature(drawCard());
        }

        GameManager.Instance.performAfterTargetSelect(this, Action.targets.eitherRow, Action.targetRestrictions.allies, summonInRow);
        
        if(!enableTargeting)
            return;
        
        GameManager.Instance.targetWasClicked(this);
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

    public ITargetable.TargetType getTargetType()
    {
        return ITargetable.TargetType.player;
    }

    public List<ITargetable> getTargets()
    {
        List<ITargetable> temp = new List<ITargetable>();
        temp.Add(this);
        return temp;
    }

    public void updateCurrentHealth(float num)
    {
        currentHealth += num;
        // If player's health goes above max health, drop it back to max
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
        // If HP drops to 0, this player is killed
        if(currentHealth <= 0){
            //end game
        }
    }
}
