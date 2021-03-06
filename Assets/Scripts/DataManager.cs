using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance
    {
        get {
            return instance;
        }

        set {
            if(instance == null || value == null)
            {
                instance = value;
            }
            else
            {
                Destroy(value);
            }
        }
    }

    public Player player1;
    public Player player2;
    public RowManager p1Front;
    public RowManager p1Back;
    public RowManager p2Front;
    public RowManager p2Back;
    public GameObject player1UI;
    public GameObject player2UI;
    public SpellbookMenu spellBook;
    public AbilityUIManager abilityUI;
    public Canvas UICanvas;

    void Awake()
    {
        DataManager.Instance = this;
    }

    void OnDestroy()
    {
        DataManager.Instance = null;
    }
}
