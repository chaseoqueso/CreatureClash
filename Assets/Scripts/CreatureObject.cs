using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureObject", menuName = "ScriptableObjects/CreatureObject", order = 1)]
public class CreatureObject : ScriptableObject
{
    [SerializeField] private string creatureName;
    [SerializeField] private int manaCost;

    [TextArea(3, 10)]
    [SerializeField] private string flavorText;

    // Stats
    [SerializeField] private float maxHealth;
    [SerializeField] private float baseDamage;
    [SerializeField] private float baseDefense;
    [SerializeField] private float baseSpeed;

    // Art
    [SerializeField] private Texture2D texture;
    [SerializeField] private Texture2D heightMap;

    // Sound Effects
    // ??? onSummon;
    // ??? onAction;
    // ??? onDamageTaken;
    // ??? onDeath;

    // Actions
    [SerializeField] private List<Action> actions;

    // Getter functions
    public string CreatureName(){return creatureName;}

    public int ManaCost(){return manaCost;}

    public string FlavorText(){return flavorText;}

    public float MaxHealth(){return maxHealth;}

    public float BaseDamage(){return baseDamage;}

    public float BaseDefense(){return baseDefense;}

    public float BaseSpeed(){return baseSpeed;}

    public Texture2D Texture(){return texture;}

    public List<Action> Actions(){return actions;}
}
