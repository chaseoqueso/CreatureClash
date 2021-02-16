using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get {
            return instance;
        }

        set {
            if(instance == null)
            {
                instance = value;
            }
        }
    }

    public enum Turn {
        player1,
        player2,
        resolveAttacks
    }

    public Player player1;
    public Player player2;
    public RowManager p1Front;
    public RowManager p1Back;
    public RowManager p2Front;
    public RowManager p2Back;
    public GameObject player1UI;
    public GameObject player2UI;
    public Text turnText;
    public Turn currentTurn {get; private set;}
    public int turnCount;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        turnCount = 1;
        player1.actionPoints = player2.actionPoints = turnCount + 2;
        currentTurn = Turn.player1;
        player1.playerEnabled = true;
        player2.playerEnabled = false;
        updateUI();
    }

    public void progressTurn()
    {
        switch(currentTurn)
        {
            case Turn.player1:
                currentTurn = Turn.player2; 
                player1.playerEnabled = false;
                player2.playerEnabled = true;
                break;

            case Turn.player2:
                currentTurn = Turn.resolveAttacks; 
                player1.playerEnabled = false;
                player2.playerEnabled = false;
                StartCoroutine(resolveAttacks());
                break;

            case Turn.resolveAttacks:
                ++turnCount;
                player1.actionPoints = player2.actionPoints = turnCount + 2;

                player1.playerEnabled = true;
                player2.playerEnabled = false;
                currentTurn = Turn.player1; 
                break;
        }
        updateUI();
    }

    public void updateUI()
    {
        switch(currentTurn)
        {
            case Turn.player1:
                foreach(Button b in player1UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = true;
                }
                foreach(Button b in player2UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                turnText.text = "Current Turn: Player 1";
                break;
            case Turn.player2:
                foreach(Button b in player1UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                foreach(Button b in player2UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = true;
                }
                turnText.text = "Current Turn: Player 2";
                break;
            case Turn.resolveAttacks:
                foreach(Button b in player1UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                foreach(Button b in player2UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                turnText.text = "Resolving Attacks";
                break;
        }
    }

    private IEnumerator resolveAttacks()
    {
        yield return new WaitForSeconds(1f);
        progressTurn();
    }
}
