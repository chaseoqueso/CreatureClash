using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
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
                returnList.AddRange(GameManager.Instance.data.p1Front.getTargets());
                returnList.AddRange(GameManager.Instance.data.p1Back.getTargets());
                return returnList;
            }
            else
            {
                returnList.AddRange(GameManager.Instance.data.p2Front.getTargets());
                returnList.AddRange(GameManager.Instance.data.p2Back.getTargets());
                return returnList;
            }
        }

        public void updateCurrentHealth(float num)
        {
            if(isPlayer1)
            {
                GameManager.Instance.data.p1Front.updateCurrentHealth(num);
                GameManager.Instance.data.p1Back.updateCurrentHealth(num);
            }
            else
            {
                GameManager.Instance.data.p2Front.updateCurrentHealth(num);
                GameManager.Instance.data.p2Back.updateCurrentHealth(num);
            }
        }

        public void setStatusEffect(Action.statusEffect effect)
        {
            if(isPlayer1)
            {
                GameManager.Instance.data.p1Front.setStatusEffect(effect);
                GameManager.Instance.data.p1Back.setStatusEffect(effect);
            }
            else
            {
                GameManager.Instance.data.p2Front.setStatusEffect(effect);
                GameManager.Instance.data.p2Back.setStatusEffect(effect);
            }
        }

        public void updateStatusEffects()
        {
            GameManager.instance.data.p1Front.updateStatusEffects();
            GameManager.instance.data.p1Back.updateStatusEffects();
            GameManager.instance.data.p2Front.updateStatusEffects();
            GameManager.instance.data.p2Back.updateStatusEffects();
        }
    }

    public delegate void targetCallback(ITargetable target);

    public DataManager data;
    public PlayerObject player1Object;
    public PlayerObject player2Object;
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
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += startGame;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    public void startGame(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "ForestArena") {
            data = DataManager.Instance;

            turnCount = 1;
            data.player1.actionPoints = data.player2.actionPoints = turnCount + 2;
            currentTurn = Turn.player1;
            data.player1.playerObject = player1Object;
            data.player2.playerObject = player2Object;
            data.player1.playerNumber = 1;
            data.player2.playerNumber = 2;
            resetTargeting();

            data.player1UI.GetComponentInChildren<Button>().onClick.AddListener(progressTurn);
            data.player2UI.GetComponentInChildren<Button>().onClick.AddListener(progressTurn);
            updateUI();
        }
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
                data.player1.actionPoints = data.player2.actionPoints = turnCount + 2;

                currentTurn = Turn.player1; 
                data.p1Front.removeSummoningSickness();
                data.p1Back.removeSummoningSickness();
                data.p2Front.removeSummoningSickness();
                data.p2Back.removeSummoningSickness();
                data.player1.drawCardsUntilFull();
                data.player2.drawCardsUntilFull();
                data.player1.updateStatusEffects();
                data.player2.updateStatusEffects();
                data.p1Front.updateStatusEffects();
                data.p1Back.updateStatusEffects();
                data.p2Front.updateStatusEffects();
                data.p2Back.updateStatusEffects();
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

        if(targetType == Action.targets.self || restrictions == Action.targetRestrictions.self) {
            callback(null);
            return;
        }

        int playerNumber = player == data.player1 ? 1 : 2;
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
                if((Object)target == data.p1Back || (Object)target == data.p1Front)
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
                    data.spellBook.openSpellbook(p, p.getCurrentSummonIndices());
                    break;

                case ITargetable.TargetType.creature:
                    data.abilityUI.openActionSelectUI((Creature)target);
                    break;
            }
            disableTargeting();
            Debug.Log("Object Clicked: " + target);
        }
    }

    public bool creatureIsInFrontRow(Creature creature)
    {
        return (creature.row == data.p1Front || creature.row == data.p2Front);
    }

    public bool rowIsFrontRow(RowManager row)
    {
        return (row == data.p1Front || row == data.p2Front);
    }

    public Player getPlayerFromRow(RowManager row)
    {
        if(row == null)
            return null;

        if(row == data.p1Front || row == data.p1Back)
            return data.player1;
        
        return data.player2;
    }

    public ITargetable selectRandomTargetInFrontline(Player player)
    {
        if(player == data.player1)
        {
            return data.p1Front.getRandomCreature();
        }
        else
        {
            return data.p2Front.getRandomCreature();
        }
    }

    public RowManager selectFrontRowFromBackRow(RowManager row)
    {
        if(row == data.p1Back)
            return data.p1Front;
        
        if(row == data.p2Back)
            return data.p2Front;
        
        return null;
    }

    public bool hasCreatureInFrontRow(ITargetable target)
    {
        switch(target.getTargetType())
        {
            case ITargetable.TargetType.player:
                Player p = (Player)target;
                if(p == data.player1 && data.p1Front.creatures.Count > 0)
                    return true;
                if(p == data.player2 && data.p2Front.creatures.Count > 0)
                    return true;
                return false;
                
            case ITargetable.TargetType.row:
                RowManager r = (RowManager)target;
                if((r == data.p1Back || r == data.p1Front) && data.p1Front.creatures.Count > 0)
                    return true;
                if((r == data.p2Back || r == data.p2Front) && data.p2Front.creatures.Count > 0)
                    return true;
                return false;
                
            case ITargetable.TargetType.creature:
                Creature c = (Creature)target;
                if(c.player == data.player1 && data.p1Front.creatures.Count > 0)
                    return true;
                if(c.player == data.player2 && data.p2Front.creatures.Count > 0)
                    return true;
                return false;
                
            default: return false;
        }
    }

    public void updateUI()
    {
        switch(currentTurn)
        {
            case Turn.player1:
                foreach(Button b in data.player1UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = true;
                }
                foreach(Button b in data.player2UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }

                data.turnText.text = "Current Turn: Player 1";
                break;
            case Turn.player2:
                foreach(Button b in data.player1UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                foreach(Button b in data.player2UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = true;
                }

                data.turnText.text = "Current Turn: Player 2";
                break;
            case Turn.resolveAttacks:
                foreach(Button b in data.player1UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                foreach(Button b in data.player2UI.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                data.turnText.text = "Resolving Attacks";
                break;
        }
    }

    private IEnumerator resolveAttacks()
    {
        List<Creature> actionOrder = new List<Creature>();
        actionOrder.AddRange(data.p1Front.creatures);
        actionOrder.AddRange(data.p1Back.creatures);
        actionOrder.AddRange(data.p2Front.creatures);
        actionOrder.AddRange(data.p2Back.creatures);

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

        for(int i = 0; i < Mathf.Max(data.player1.queuedActions.Count, data.player2.queuedActions.Count); ++i)
        {
            if(i < data.player1.queuedActions.Count) {
                data.player1.performQueuedAction(i);
                yield return new WaitForSeconds(0.5f);
            }

            if(i < data.player2.queuedActions.Count) {
                data.player2.performQueuedAction(i);
                yield return new WaitForSeconds(0.5f);
            }
        }

        data.player1.queuedActions.Clear();
        data.player1.queuedTargets.Clear();
        data.player2.queuedActions.Clear();
        data.player2.queuedTargets.Clear();

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
                        data.p1Front.enableCreatureTargeting(true);
                        data.p1Back.enableCreatureTargeting(true);
                        data.player1.enableTargeting = true;
                    }
                    else
                    {
                        data.p2Front.enableCreatureTargeting(true);
                        data.p2Back.enableCreatureTargeting(true);
                        data.player2.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Front.enableCreatureTargeting(true);
                        data.p2Back.enableCreatureTargeting(true);
                        data.player2.enableTargeting = true;
                    }
                    else
                    {
                        data.p1Front.enableCreatureTargeting(true);
                        data.p1Back.enableCreatureTargeting(true);
                        data.player1.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.anyCreature:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Front.enableCreatureTargeting(true);
                        data.p1Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        data.p2Front.enableCreatureTargeting(true);
                        data.p2Back.enableCreatureTargeting(true);
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Front.enableCreatureTargeting(true);
                        data.p2Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        data.p1Front.enableCreatureTargeting(true);
                        data.p1Back.enableCreatureTargeting(true);
                    }
                }
                break;

            case Action.targets.backRow:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Back.enableTargeting = true;
                    }
                    else
                    {
                        data.p2Back.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Back.enableTargeting = true;
                    }
                    else
                    {
                        data.p1Back.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.backCreature:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        data.p2Back.enableCreatureTargeting(true);
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Back.enableCreatureTargeting(true);
                    }
                    else
                    {
                        data.p1Back.enableCreatureTargeting(true);
                    }
                }
                break;
                
            case Action.targets.backSingle:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Back.enableCreatureTargeting(true);
                        data.player1.enableTargeting = true;
                    }
                    else
                    {
                        data.p2Back.enableCreatureTargeting(true);
                        data.player2.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Back.enableCreatureTargeting(true);
                        data.player2.enableTargeting = true;
                    }
                    else
                    {
                        data.p1Back.enableCreatureTargeting(true);
                        data.player1.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.bothRows:

                bothRowsTargeting = true;
                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Front.enableTargeting = true;
                        data.p1Back.enableTargeting = true;
                    }
                    else
                    {
                        data.p2Front.enableTargeting = true;
                        data.p2Back.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Front.enableTargeting = true;
                        data.p2Back.enableTargeting = true;
                    }
                    else
                    {
                        data.p1Front.enableTargeting = true;
                        data.p1Back.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.eitherRow:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Front.enableTargeting = true;
                        data.p1Back.enableTargeting = true;
                    }
                    else
                    {
                        data.p2Front.enableTargeting = true;
                        data.p2Back.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Front.enableTargeting = true;
                        data.p2Back.enableTargeting = true;
                    }
                    else
                    {
                        data.p1Front.enableTargeting = true;
                        data.p1Back.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.frontRow:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Front.enableTargeting = true;
                    }
                    else
                    {
                        data.p2Front.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Front.enableTargeting = true;
                    }
                    else
                    {
                        data.p1Front.enableTargeting = true;
                    }
                }
                break;
                
            case Action.targets.frontSingle:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p1Front.enableCreatureTargeting(true);
                    }
                    else
                    {
                        data.p2Front.enableCreatureTargeting(true);
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.p2Front.enableCreatureTargeting(true);
                    }
                    else
                    {
                        data.p1Front.enableCreatureTargeting(true);
                    }
                }
                break;
                
            case Action.targets.player:

                if(restrictions == Action.targetRestrictions.allies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.player1.enableTargeting = true;
                    }
                    else
                    {
                        data.player2.enableTargeting = true;
                    }
                }

                if(restrictions == Action.targetRestrictions.enemies || restrictions == Action.targetRestrictions.none)
                {
                    if(playerNumber == 1)
                    {
                        data.player2.enableTargeting = true;
                    }
                    else
                    {
                        data.player1.enableTargeting = true;
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
        
        if(data.p1Front.creatures.Contains(creature))
            return data.p1Front;
        
        if(data.p1Back.creatures.Contains(creature))
            return data.p1Back;
        
        if(data.p2Front.creatures.Contains(creature))
            return data.p2Front;
        
        if(data.p2Back.creatures.Contains(creature))
            return data.p2Back;

        return null;
    }

    public void resetTargeting()
    {
        data.p1Front.enableTargeting = false;
        data.p1Back.enableTargeting = false;
        data.p2Front.enableTargeting = false;
        data.p2Back.enableTargeting = false;

        if(currentTurn == Turn.player1) {
            data.player1.enableTargeting = data.player1.actionPoints > 0;
            data.player2.enableTargeting = false;
            data.p1Front.resetCreatureTargeting();
            data.p1Back.resetCreatureTargeting();
            data.p2Front.enableCreatureTargeting(false);
            data.p2Back.enableCreatureTargeting(false);
        } else if(currentTurn == Turn.player2) {
            data.player2.enableTargeting = data.player2.actionPoints > 0;
            data.player1.enableTargeting = false;
            data.p2Front.resetCreatureTargeting();
            data.p2Back.resetCreatureTargeting();
            data.p1Front.enableCreatureTargeting(false);
            data.p1Back.enableCreatureTargeting(false);
        } else {
            disableTargeting();
        }
    }

    public void disableTargeting()
    {
        data.player1.enableTargeting = false;
        data.player2.enableTargeting = false;
        data.p1Front.enableTargeting = false;
        data.p1Front.enableCreatureTargeting(false);
        data.p1Back.enableTargeting = false;
        data.p1Back.enableCreatureTargeting(false);
        data.p2Front.enableTargeting = false;
        data.p2Front.enableCreatureTargeting(false);
        data.p2Back.enableTargeting = false;
        data.p2Back.enableCreatureTargeting(false);
    }
}
