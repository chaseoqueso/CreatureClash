using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Player playerOne;
    public Player playerTwo;

    public float almostDeadValue;

    private Dialogue dialogue;

    void Start()
    {
        //Jen broke this
        //playerOne = GameManager.Instance.player1;
        //playerTwo = GameManager.Instance.player2;
    }

    public void triggerDialogue()
    {
        // Reset dialogue
        dialogue.characterLines.Clear();

        // Select next lines based on game state / triggers / special stuff
        selectNextDialogue();

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void selectNextDialogue()
    {
        // If necessary, add narrator dialogue

        
        // Check game state & add dialogue
        // Check for special dialogue conditions
        if( checkForSpecialDialogue() ){
            // If added special dialogue to dialogue, end here
            return;
        }
        // Check for start dialogue
        else if(GameManager.Instance.turnCount == 1){
            // add Player One dialogue
            int randomIndex = Random.Range(0, playerOne.playerObject.challengeDialogue.Count - 1);
            AppendDialogueItems(playerOne.playerObject.challengeDialogue[randomIndex].characterLines);
            // add Player Two dialogue
            randomIndex = Random.Range(0, playerTwo.playerObject.challengeDialogue.Count - 1);
            AppendDialogueItems(playerTwo.playerObject.challengeDialogue[randomIndex].characterLines);
        }
        // Check for end dialogue
        // else if( ){

        // }
        // Check for players almost dead              UPDATE THESE TO BE CURRENT / MAX HEALTH!!!!!!!!!!!!!
        // if both almost dead now (maybe make this it's own thing rather than using both selfAlmostDead??? or use special dialogue)
        else if( (playerOne.currentHealth / playerOne.maxHealth) <= almostDeadValue && (playerTwo.currentHealth / playerTwo.maxHealth) <= almostDeadValue ){
            // add Player One dialogue
            int randomIndex = Random.Range(0, playerOne.playerObject.selfAlmostDeadDialogue.Count - 1);
            AppendDialogueItems(playerOne.playerObject.selfAlmostDeadDialogue[randomIndex].characterLines);
            // add Player Two dialogue
            randomIndex = Random.Range(0, playerTwo.playerObject.selfAlmostDeadDialogue.Count - 1);
            AppendDialogueItems(playerTwo.playerObject.selfAlmostDeadDialogue[randomIndex].characterLines);
        }
        // if player 1 is almost dead
        else if( (playerOne.currentHealth / playerOne.maxHealth) <= almostDeadValue ){
            // add Player One dialogue
            int randomIndex = Random.Range(0, playerOne.playerObject.selfAlmostDeadDialogue.Count - 1);
            AppendDialogueItems(playerOne.playerObject.selfAlmostDeadDialogue[randomIndex].characterLines);
            // add Player Two dialogue
            randomIndex = Random.Range(0, playerTwo.playerObject.opponentAlmostDeadDialogue.Count - 1);
            AppendDialogueItems(playerTwo.playerObject.opponentAlmostDeadDialogue[randomIndex].characterLines);
        }
        // if player 2 is almost dead
        else if( (playerTwo.currentHealth / playerTwo.maxHealth) <= almostDeadValue ){
            // add Player One dialogue
            int randomIndex = Random.Range(0, playerOne.playerObject.opponentAlmostDeadDialogue.Count - 1);
            AppendDialogueItems(playerOne.playerObject.opponentAlmostDeadDialogue[randomIndex].characterLines);
            // add Player Two dialogue
            randomIndex = Random.Range(0, playerTwo.playerObject.selfAlmostDeadDialogue.Count - 1);
            AppendDialogueItems(playerTwo.playerObject.selfAlmostDeadDialogue[randomIndex].characterLines);
        }
        // Check for creature death
        // else if( creaturedeath ){

        // }
        // Check for damage dealt/taken
        // else if( damage dealt/taken ){

        // }
    }

    private bool checkForSpecialDialogue()
    {
        // Check if using special dialogue
        int randomIndex = Random.Range(0, 2);
        if( randomIndex == 0 ){
            return false;
        }

        List<SpecialDialogue> specialDialogue;
        
        if( randomIndex == 1 ){
            specialDialogue = playerOne.playerObject.specialDialogue;
            // If special dialogue found and added, return true
            if(specialDialogueLoop(specialDialogue)){       // , playerOne.roundTriggers)){
                return true;
            }
        }
        else if( randomIndex == 2 ){
            // If no special dialogue that fits these conditions, return false
            specialDialogue = playerTwo.playerObject.specialDialogue;
            return specialDialogueLoop(specialDialogue);        // , playerTwo.roundTriggers);
        }

        return false;
    }

    private bool specialDialogueLoop(List<SpecialDialogue> specialDialogue) //, List<SpecialDialogue.dialogueTrigger> roundTriggers)
    {
        return false;

        /*
        // roundTriggers must always contain the trigger for who the opponent is
        // OR
        // just check that here (and it's never included there)

        List<SpecialDialogue> dialogueOptions = new List<SpecialDialogue>();

        foreach( SpecialDialogue dialogue in specialDialogue ){
            foreach( SpecialDialogue.dialogueTrigger trigger in dialogue.triggers ){
                if( !roundTriggers.Contains(trigger) ){
                    // Go to next dialogue
                }
            }
            // If it makes it this far, all trigger conditions have been met - add to potential dialogue options
            dialogueOptions.Add(dialogue);
        }

        // If there are no valid special dialogue options, return false
        if(dialogueOptions.Count == 0){
            return false;
        }

        int randomIndex = Random.Range(0, dialogueOptions.Count - 1);

        // Append dialogue items
        foreach( Dialogue.characterLine line in dialogueOptions[randomIndex].characterLines ){
            dialogue.characterLines.Add(line);
        }

        return true;
        */
    }

    private void AppendDialogueItems( List<Dialogue.characterLine> listToAdd )
    {
        foreach( Dialogue.characterLine line in listToAdd ){
            dialogue.characterLines.Add(line);
        }
    }
}