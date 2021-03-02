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

    public class CreatureComparer : Comparer<Creature> {
        public override int Compare(Creature c1, Creature c2)
        {
            return (int)Mathf.Sign(c1.currentSpeed - c2.currentSpeed);
        }
    }

    public enum Turn {
        player1,
        player2,
        resolveAttacks
    }

    public class BothRowsTargetable : ITargetable
    {
        public bool isPlayer1;

        public ITargetable.TargetType getTargetType()
        {
            return ITargetable.TargetType.bothRows;
        }

        public List<ITargetable> getTargets()
        {
            List<ITargetable> returnList = new List<ITargetable>();
            if(isPlayer1)
            {
                returnList.AddRange(GameManager.Instance.p1Front.getTargets());
                returnList.AddRange(GameManager.Instance.p1Back.getTargets());
                return returnList;
            }
            else
            {
                returnList.AddRange(GameManager.Instance.p2Front.getTargets());
                returnList.AddRange(GameManager.Instance.p2Back.getTargets());
                return returnList;
            }
        }

        public void updateCurrentHealth(float num)
        {
            if(isPlayer1)
            {
                GameManager.Instance.p1Front.updateCurrentHealth(num);
                GameManager.Instance.p1Back.updateCurrentHealth(num);
            }
            else
            {
                GameManager.Instance.p2Front.updateCurrentHealth(num);
                GameManager.Instance.p2Back.updateCurrentHealth(num);
            }
        }

        public void setStatusEffect(Action.statusEffect effect)
        {
            if(isPlayer1)
            {
                GameManager.Instance.p1Front.setStatusEffect(effect);
                GameManager.Instance.p1Back.setStatusEffect(effect);
            }
            else
            {
                GameManager.Instance.p2Front.setStatusEffect(effect);
                GameManager.Instance.p2Back.setStatusEffect(effect);
            }
        }
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
    public SpellbookMenu spellBook;
    public Text turnText;
    public Turn currentTurn {get; private set;}
    public int turnCount;
    
    private ITargetable currentTarget;
    private bool targeting;
    private bool bothRowsTargeting;

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
        player1.playerNumber = 1;
        player2.playerNumber = 2;
        updateUI();
    }

    public void progressTurn()
    {
        switch(currentTurn)
        {
            case Turn.player1:
                currentTurn = Turn.player2; 
                resetTargeting();
                
                Debug.Log("Player 1 turn end, player 2 turn start");
                break;

            case Turn.player2:
                currentTurn = Turn.resolveAttacks; 
                resetTargeting();

                Debug.Log("Player 2 turn end, resolve attacks start");
                StartCoroutine(resolveAttacks());
                break;

            case Turn.resolveAttacks:
                ++turnCount;
                player1.actionPoints = player2.actionPoints = turnCount + 2;

                currentTurn = Turn.player1; 
                p1Front.removeSummoningSickness();
                p1Back.removeSummoningSickness();
                p2Front.removeSummoningSickness();
                p2Back.removeSummoningSickness();
                player1.drawCardsUntilFull();
                player2.drawCardsUntilFull();
                resetTargeting();

                Debug.Log("Resolve attacks end, player 1 turn start");
                break;
        }
        updateUI();
    }

    public void performAfterTargetSelect(Player player, Action.targets targetType, Action.targetRestrictions restrictions, bool blockedByFrontline, targetCallback callback)
    {
        if(player == null)
        {
            Debug.LogError("Player cannot be null.");
            return;
        }

        int playerNumber = player == player1 ? 1 : 2;
        disableTargeting();
        enableTargetingOnTargets(playerNumber, targetType, restrictions, blockedByFrontline);

        StartCoroutine(waitForTargetSelect(callback));
    }

    private IEnumerator waitForTargetSelect(targetCallback callback)
    {
        targeting = true;
        Debug.Log("Waiting for target select");

        currentTarget = null;
        while(currentTarget == null)
        {
            yield return null;
        }
        Debug.Log("Target selected: " + currentTarget);
        
        callback(currentTarget);

        resetTargeting();
        targeting = false;
    }

    public void targetWasClicked(ITargetable target)
    {
        if(targeting) {
            if(bothRowsTargeting)
            {
                BothRowsTargetable bothrows = new BothRowsTargetable();
                if((Object)target == p1Back || (Object)target == p1Front)
                {
                    bothrows.isPlayer1 = true;
                }
                else
                {
                    bothrows.isPlayer1 = false;
                }
                currentTarget = bothrows;
            }
            else
            {
                currentTarget = target;
            }
            Debug.Log("Target Clicked: " + currentTarget);
        } else {
            switch(target.getTargetType()) {
                case ITargetable.TargetType.player:
                    Player p = (Player)target;
                    spellBook.openSpellbook(p, p.getCurrentSummonIndices());
                    break;

                case ITargetable.TargetType.creature:
                    ((Creature)target).selectTargetsForAction(0);
                    break;
            }
            Debug.Log("Object Clicked: " + target);
        }
    }

    public bool creatureIsInFrontRow(Creature creature)
    {
        return (creature.row == p1Front || creature.row == p2Front);
    }

    public bool rowIsFrontRow(RowManager row)
    {
        return (row == p1Front || row == p2Front);
    }

    public Player getPlayerFromRow(RowManager row)
    {
        if(row == null)
            return null;

        if(row == p1Front || row == p1Back)
            return player1;
        
        return player2;
    }

    public ITargetable selectRandomTargetInFrontline(Player player)
    {
        if(player == player1)
        {
            return p1Front.getRandomCreature();
        }
        else
        {
            return p2Front.getRandomCreature();
        }
    }

    public RowManager selectFrontRowFromBackRow(RowManager row)
    {
        if(row == p1Back)
            return p1Front;
        
        if(row == p2Back)
            return p2Front;
        
        return null;
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
        List<Creature> actionOrder = new List<Creature>();
        actionOrder.AddRange(p1Front.creatures);
        actionOrder.AddRange(p1Back.creatures);
        actionOrder.AddRange(p2Front.creatures);
        actionOrder.AddRange(p2Back.creatures);

        Comparer<Creature> comparer = new CreatureComparer();
        actionOrder.Sort(comparer);

        for(int i = 0; i < actionOrder.Count; ++i)
        {
            if(actionOrder[i] != null && actionOrder[i].currentAction != null)
            {
                actionOrder[i].performNextAction();
                yield return new WaitForSeconds(0.5f);
            }
        }

        for(int i = 0; i < Mathf.Max(player1.queuedActions.Count, player2.queuedActions.Count); ++i)
        {
            if(i < player1.queuedActions.Count) {
                player1.performQueuedAction(i);
                yield return new WaitForSeconds(0.5f);
            }

            if(i < player2.queuedActions.Count) {
                player2.performQueuedAction(i);
                yield return new WaitForSeconds(0.5f);
            }
        }

        player1.queuedActions.Clear();
        player1.queuedTargets.Clear();
        player2.queuedActions.Clear();
        player2.queuedTargets.Clear();

        progressTurn();
    }

    private void enableTargetingOnTargets(int playerNumber, Action.targets targetType, Action.targetRestrictions restrictions, bool blockedByFrontline)
    {
        switch (targetType) {
            case Action.targets.anySingle:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Front.enableCreatureTargeting(true);
                        p1Back.enableCreatureTargeting(true);
                        player1.enableTargeting = true;
                    }
                    else
                    {
                        p2Front.enableCreatureTargeting(true);
                        p2Back.enableCreatureTargeting(true);
                        player2.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Front.enableCreatureTargeting(true);
                        p2Back.enableCreatureTargeting(true);
                        player2.enableTargeting = true;
                    }
                    else
                    {
                        p1Front.enableCreatureTargeting(true);
                        p1Back.enableCreatureTargeting(true);
                        player1.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.anyCreature:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Front.enableCreatureTargeting(true);
                        p1Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        p2Front.enableCreatureTargeting(true);
                        p2Back.enableCreatureTargeting(true);
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Front.enableCreatureTargeting(true);
                        p2Back.enableCreatureTargeting(true);
                    }
                    else
                    {
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
                
            case Action.targets.backCreature:

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
                
            case Action.targets.backSingle:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p1Back.enableCreatureTargeting(true);
                        player1.enableTargeting = true;
                    }
                    else
                    {
                        p2Back.enableCreatureTargeting(true);
                        player2.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        p2Back.enableCreatureTargeting(true);
                        player2.enableTargeting = true;
                    }
                    else
                    {
                        p1Back.enableCreatureTargeting(true);
                        player1.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.bothRows:

                bothRowsTargeting = true;
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

    public ITargetable getParentTarget(ITargetable target)
    {
        if(target.getTargetType() != ITargetable.TargetType.creature)
            return target;

        Creature creature = (Creature)target;
        
        if(p1Front.creatures.Contains(creature))
            return p1Front;
        
        if(p1Back.creatures.Contains(creature))
            return p1Back;
        
        if(p2Front.creatures.Contains(creature))
            return p2Front;
        
        if(p2Back.creatures.Contains(creature))
            return p2Back;

        return null;
    }

    public void resetTargeting()
    {
        p1Front.enableTargeting = false;
        p1Back.enableTargeting = false;
        p2Front.enableTargeting = false;
        p2Back.enableTargeting = false;

        if(currentTurn == Turn.player1) {
            player1.enableTargeting = player1.actionPoints > 0;
            player2.enableTargeting = false;
            p1Front.resetCreatureTargeting();
            p1Back.resetCreatureTargeting();
            p2Front.enableCreatureTargeting(false);
            p2Back.enableCreatureTargeting(false);
        } else if(currentTurn == Turn.player2) {
            player2.enableTargeting = player2.actionPoints > 0;
            player1.enableTargeting = false;
            p2Front.resetCreatureTargeting();
            p2Back.resetCreatureTargeting();
            p1Front.enableCreatureTargeting(false);
            p1Back.enableCreatureTargeting(false);
        } else {
            disableTargeting();
        }
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
