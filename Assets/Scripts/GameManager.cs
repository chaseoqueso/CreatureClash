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

    public delegate void targetCallback(ITargetable target);

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
    
    private ITargetable currentTarget;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        turnCount = 1;
        player1.actionPoints = player2.actionPoints = turnCount + 2;
        currentTurn = Turn.player1;
        player1.enableTargeting = true;
        player2.enableTargeting = false;
        updateUI();
    }

    public void progressTurn()
    {
        switch(currentTurn)
        {
            case Turn.player1:
                currentTurn = Turn.player2; 
                player1.enableTargeting = false;
                player2.enableTargeting = true;
                break;

            case Turn.player2:
                currentTurn = Turn.resolveAttacks; 
                player1.enableTargeting = false;
                player2.enableTargeting = false;
                StartCoroutine(resolveAttacks());
                break;

            case Turn.resolveAttacks:
                ++turnCount;
                player1.actionPoints = player2.actionPoints = turnCount + 2;

                player1.enableTargeting = true;
                player2.enableTargeting = false;
                currentTurn = Turn.player1; 
                break;
        }
        updateUI();
    }

    public void performAfterTargetSelect(Player player, Action.targets targetType, Action.targetRestrictions restrictions, targetCallback callback)
    {
        if(player == null)
        {
            Debug.LogError("Player cannot be null.");
            return;
        }

        int playerNumber = player == player1 ? 1 : 2;
        disableTargeting();
        enableTargetingOnTargets(playerNumber, targetType, restrictions);

        StartCoroutine(waitForTargetSelect(callback));
    }

    private IEnumerator waitForTargetSelect(targetCallback callback)
    {
        currentTarget = null;
        while(currentTarget == null)
        {
            yield return null;
        }
        
        callback(currentTarget);

        resetTargeting();
    }

    public void targetWasClicked(ITargetable target)
    {
        currentTarget = target;
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

    private void enableTargetingOnTargets(int playerNumber, Action.targets targetType, Action.targetRestrictions restrictions)
    {
        switch (targetType) {
            case Action.targets.anySingle:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        player1.enableTargeting = true;
                        p1Front.enableCreatureTargeting(true);
                        p1Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        player2.enableTargeting = true;
                        p2Front.enableCreatureTargeting(true);
                        p2Back.enableCreatureTargeting(true);
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        player2.enableTargeting = true;
                        p2Front.enableCreatureTargeting(true);
                        p2Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        player1.enableTargeting = true;
                        p1Front.enableCreatureTargeting(true);
                        p1Back.enableCreatureTargeting(true);
                    }
                }
                break;

            case Action.targets.backRow:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Back.enableTargeting = true;
                    }
                    else
                    {
                        p2Back.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Back.enableTargeting = true;
                    }
                    else
                    {
                        p1Back.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.backSingle:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        p2Back.enableCreatureTargeting(true);
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        p1Back.enableCreatureTargeting(true);
                    }
                }
                break;
                
            case Action.targets.bothRows:
                //TODO idk
                break;
                
            case Action.targets.eitherRow:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Front.enableTargeting = true;
                        p1Back.enableTargeting = true;
                    }
                    else
                    {
                        p2Front.enableTargeting = true;
                        p2Back.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Front.enableTargeting = true;
                        p2Back.enableTargeting = true;
                    }
                    else
                    {
                        p1Front.enableTargeting = true;
                        p1Back.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.frontRow:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Front.enableTargeting = true;
                    }
                    else
                    {
                        p2Front.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Front.enableTargeting = true;
                    }
                    else
                    {
                        p1Front.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.frontSingle:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Front.enableCreatureTargeting(true);
                    }
                    else
                    {
                        p2Front.enableCreatureTargeting(true);
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Front.enableCreatureTargeting(true);
                    }
                    else
                    {
                        p1Front.enableCreatureTargeting(true);
                    }
                }
                break;
                
            case Action.targets.player:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        player1.enableTargeting = true;
                    }
                    else
                    {
                        player2.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        player2.enableTargeting = true;
                    }
                    else
                    {
                        player1.enableTargeting = true;
                    }
                }
                break;
        }
    }

    public void resetTargeting()
    {
        p1Front.enableTargeting = false;
        p1Front.resetCreatureTargeting();
        p1Back.enableTargeting = false;
        p1Back.resetCreatureTargeting();
        p2Front.enableTargeting = false;
        p2Front.resetCreatureTargeting();
        p2Back.enableTargeting = false;
        p2Back.resetCreatureTargeting();
    }

    public void disableTargeting()
    {
        player1.enableTargeting = false;
        player2.enableTargeting = false;
        p1Front.enableTargeting = false;
        p1Front.enableCreatureTargeting(false);
        p1Back.enableTargeting = false;
        p1Back.enableCreatureTargeting(false);
        p2Front.enableTargeting = false;
        p2Front.enableCreatureTargeting(false);
        p2Back.enableTargeting = false;
        p2Back.enableCreatureTargeting(false);
    }
}
