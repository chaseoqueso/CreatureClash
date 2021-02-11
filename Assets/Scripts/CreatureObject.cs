using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureObject", menuName = "ScriptableObjects/CreatureObject", order = 1)]
public class CreatureObject : ScriptableObject
{
    [SerializeField] private string creatureName;
    [SerializeField] private int manaCost;

    // Stats
    [SerializeField] private float maxHealth;
    [SerializeField] private float baseDamage;
    [SerializeField] private float defense;
    [SerializeField] private float speed;

    // Art
    [SerializeField] private Sprite sprite;

    // Sound Effects
    // ??? onSummon;
    // ??? onAction;
    // ??? onDamageTaken;
    // ??? onDeath;

    // Actions
    [SerializeField] private List<Action> actions;
    // [SerializeField] private Action action;

    // Getter functions
    public string CreatureName(){return creatureName;}

    public int ManaCost(){return manaCost;}

    public float MaxHealth(){return maxHealth;}

    public float BaseDamage(){return baseDamage;}

    public float Defense(){return defense;}

    public float Speed(){return speed;}

    public Sprite Sprite(){return sprite;}

    public List<Action> Actions(){return actions;}
}
