using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour, ITargetable
{
    private const float creatureLerpTime = 0.5f;

    public Vector3 topEndpoint;
    public Vector3 bottomEndpoint;
    public GameObject creaturePrefab;
    public bool enableTargeting;

    private List<Creature> creatures;
    private List<Vector3> creaturePositions;

    void Awake()
    {
        creatures = new List<Creature>();
        creaturePositions = new List<Vector3>();
        enableTargeting = false;
    }

    void OnMouseDown()
    {
        if(!enableTargeting)
            return;
        
        GameManager.Instance.targetWasClicked(this);
    }

    public void updateCurrentHealth(float num)
    {
        foreach(Creature c in creatures)
        {
            c.updateCurrentHealth(num);
        }
    }

    public void enableCreatureTargeting(bool enable)
    {
        foreach(Creature c in creatures)
        {
            c.enableTargeting = enable;
        }
    }

    public void resetCreatureTargeting()
    {
        foreach(Creature c in creatures)
        {
            c.enableTargeting = c.currentAction == null;
        }
    }

    public Creature summonCreature(CreatureObject creatureType)
    {
        Vector3 spawnPos = Vector3.Lerp(bottomEndpoint, topEndpoint, 1f/((creaturePositions.Count + 1) * 2f));

        creaturePositions.Add(spawnPos);
        for(int i = 1; i < creaturePositions.Count; ++i)
        {
            creaturePositions[i-1] = Vector3.Lerp(bottomEndpoint, topEndpoint, (2f*i + 1)/((creaturePositions.Count) * 2f));
        }

        GameObject creature = Instantiate(creaturePrefab, spawnPos, Quaternion.identity, transform);
        Creature creatureScript = creature.GetComponent<Creature>();
        creatures.Add(creatureScript);
        creatureScript.creature = creatureType;


        StartCoroutine(lerpCreaturePositions(creatureLerpTime));
        return creatureScript;
    }

    private IEnumerator lerpCreaturePositions(float duration)
    {
        float startTime = Time.time;
        List<Vector3> startingPositions = new List<Vector3>();
        for(int i = 0; i < creatures.Count; ++i)
        {
            startingPositions.Add(creatures[i].transform.position);
        }

        do {
            for(int i = 0; i < creatures.Count; ++i)
            {
                creatures[i].transform.position = Vector3.Lerp(startingPositions[i], creaturePositions[i], (Time.time - startTime)/duration);   //Mathf.Sin((Time.time - startTime)/duration) * (Mathf.PI/2f));
            }
            yield return null;
        }
        while(Time.time < startTime + duration);
    }

    public ITargetable.TargetType getTargetType()
    {
        return ITargetable.TargetType.row;
    }

    public List<ITargetable> getTargets()
    {
        List<ITargetable> temp = new List<ITargetable>();
        foreach(Creature c in creatures)
            temp.Add(c);
        return temp;
    }
}