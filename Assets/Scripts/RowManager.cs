using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    private const float creatureLerpTime = 0.5f;

    public Vector3 topEndpoint;
    public Vector3 bottomEndpoint;
    public GameObject creaturePrefab;

    private List<Creature> creatures;
    private List<Vector3> creaturePositions;

    void Awake()
    {
        creatures = new List<Creature>();
        creaturePositions = new List<Vector3>();
    }

    public Creature summonCreature(CreatureObject creatureType)
    {
        Vector3 spawnPos = Vector3.Lerp(bottomEndpoint, topEndpoint, 1/(creaturePositions.Count + 2));

        creaturePositions.Add(spawnPos);
        for(int i = 1; i < creaturePositions.Count; ++i)
        {
            creaturePositions[i] = Vector3.Lerp(bottomEndpoint, topEndpoint, (i+1)/(creaturePositions.Count + 2));
        }

        Creature c = Instantiate(creaturePrefab, spawnPos, Quaternion.identity, transform).GetComponent<Creature>();
        creatures.Add(c);

        StartCoroutine(lerpCreaturePositions(creatureLerpTime));
        return c;
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
}
