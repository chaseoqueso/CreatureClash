using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour, ITargetable
{
    private const float creatureLerpTime = 0.4f;

    public Vector3 topEndpoint;
    public Vector3 bottomEndpoint;
    public GameObject creaturePrefab;
    public bool enableTargeting;

    public List<Creature> creatures;
    private List<Vector3> creaturePositions;
    private SpriteRenderer[] renderers;

    void Awake()
    {
        creatures = new List<Creature>();
        creaturePositions = new List<Vector3>();
        enableTargeting = false;
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if(enableTargeting) {
            foreach(SpriteRenderer r in renderers)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, Mathf.Abs(Mathf.Sin(Time.time * 2f)));
            }
        }
        else
        {
            foreach(SpriteRenderer r in renderers)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, 0);
            }
        }
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

    //returns true if the row has at least 1 creature
    public bool enableCreatureTargeting(bool enable)
    {
        foreach(Creature c in creatures)
        {
            c.enableTargeting = enable;
        }
        return creatures.Count > 0;
    }

    //returns true if the row has at least 1 creature
    public bool resetCreatureTargeting()
    {
        foreach(Creature c in creatures)
        {
            c.enableTargeting = c.currentAction == null && !c.justSummoned;
        }
        return creatures.Count > 0;
    }

    //returns true if the row has at least 1 creature
    public bool removeSummoningSickness()
    {
        foreach(Creature c in creatures)
        {
            c.justSummoned = false;
        }
        return creatures.Count > 0;
    }

    public Creature getRandomCreature()
    {
        if(creatures.Count == 0)
            return null;

        return creatures[Random.Range(0, creatures.Count)];
    }

    public Creature summonCreature(CreatureObject creatureType, Player player)
    {
        Vector3 spawnPos = Vector3.Lerp(bottomEndpoint, topEndpoint, 1f/((creaturePositions.Count + 1) * 2f));

        creaturePositions.Add(spawnPos);
        for(int i = 1; i < creaturePositions.Count; ++i)
        {
            creaturePositions[creaturePositions.Count-i-1] = Vector3.Lerp(bottomEndpoint, topEndpoint, (2f*i + 1)/((creaturePositions.Count) * 2f));
        }

        GameObject creature = Instantiate(creaturePrefab, spawnPos, Quaternion.identity, transform);
        Creature creatureScript = creature.GetComponent<Creature>();
        creatures.Add(creatureScript);
        creatureScript.creature = creatureType;
        creatureScript.player = player;
        creatureScript.row = this;


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
                Vector3 targetPos = Vector3.Lerp(startingPositions[i], creaturePositions[i], Mathf.Sin((Time.time - startTime)/duration) * (Mathf.PI/2f));
                creatures[i].transform.position = new Vector3(targetPos.x, creatures[i].transform.position.y, targetPos.z);
            }
            yield return null;
        }
        while(Time.time < startTime + duration);
    }

    public void removeCreature(Creature c)
    {
        int index = creatures.IndexOf(c);
        creatures.RemoveAt(index);
        creaturePositions.RemoveAt(index);

        for(int i = 0; i < creaturePositions.Count; ++i)
        {
            creaturePositions[creaturePositions.Count-i-1] = Vector3.Lerp(bottomEndpoint, topEndpoint, (2f*i + 1)/((creaturePositions.Count) * 2f));
        }

        Debug.Log("Number of Creatures Left: " + creatures.Count);
        
        StartCoroutine(lerpCreaturePositions(creatureLerpTime));
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
    
    public void setStatusEffect(Action.statusEffect status)
    {
        foreach(Creature c in creatures)
        {
            c.setStatusEffect(status);
        }
    }

    public void updateStatusEffects()
    {
        foreach(Creature c in creatures)
        {
            c.updateStatusEffects();
        }
    }
}
