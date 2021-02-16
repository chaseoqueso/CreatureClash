using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Turn {
        player1,
        player2,
        resolveAttacks
    }

    public Turn currentTurn {get; private set;}
    public int turnCount;
    public Player player1;
    public Player player2;
    public GameObject player1UI;
    public GameObject player2UI;
    public Text turnText;

    void Start()
    {
        turnCount = 1;
        player1.actionPoints = player2.actionPoints = turnCount + 2;
        currentTurn = Turn.player1;
        updateUI();
    }

    public void progressTurn()
    {
        switch(currentTurn)
        {
            case Turn.player1:
                currentTurn = Turn.player2; 
                break;
            case Turn.player2:
                currentTurn = Turn.resolveAttacks; 
                StartCoroutine(resolveAttacks());
                break;
            case Turn.resolveAttacks:
                ++turnCount;
                player1.actionPoints = player2.actionPoints = turnCount + 2;
                currentTurn = Turn.player1; 
                break;
            default:
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
