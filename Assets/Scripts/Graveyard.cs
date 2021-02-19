using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour
{
    public List<Creature> deadCreatures;

    // Start is called before the first frame update
    void Start()
    {
        deadCreatures = new List<Creature>();
    }

    public Creature resurrectLastCreature()
    {
        // have this summon the creature onto the field, once that can happen (rather than just returning it)
        // probably plays special animations and stuff, if that's a thing
        if( deadCreatures.Count < 1 ){
            Debug.Log("No creature to resurrect.");
            return null;
        }
        Creature resCreature = deadCreatures[deadCreatures.Count - 1];
        deadCreatures.Remove(resCreature);
        return resCreature;
    }

    public Creature resurrectTargetCreature(Creature target)
    {
        // have this summon the creature onto the field, once that can happen
        if( !deadCreatures.Contains(target) ){
            Debug.Log("No creature to resurrect.");
            return null;
        }
        deadCreatures.Remove(target);
        return target;
    }

    public void emptyGraveyard()
    {
        deadCreatures.Clear();
    }
}
