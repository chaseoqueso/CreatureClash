using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public PlayerObject playerOne;
    public PlayerObject playerTwo;

    private Dialogue dialogue;

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
        // Check game state
        
        // Pick dialogue (add lines to dialogue.sentences)
        // Check for special dialogue conditions

        // Check for start/end dialogue

        // Check for creature death

        // Check for self almost dead

        // Check for opponent almost dead

        // Check for damage dealt/taken

    }
}