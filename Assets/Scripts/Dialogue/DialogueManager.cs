using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image characterImage;

    private Queue<string> sentences;
    private Queue<string> names;
    private Queue<Texture2D> portraits;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        names = new Queue<string>();
        portraits = new Queue<Texture2D>();
    }

    void Update()
    {
        if( Input.GetAxis("Submit") == 1 ){
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();
        names.Clear();
        portraits.Clear();

        if( dialogue.characterLines.Count != 0 ){
            // Add each line to the queue
            foreach(Dialogue.characterLine line in dialogue.characterLines){
                sentences.Enqueue(line.sentence);
                names.Enqueue(line.speaker.characterName);
                portraits.Enqueue(line.speaker.characterPortrait);
            }
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0){
            EndDialogue();
            return;
        }

        nameText.text = names.Dequeue();
        
        Texture2D t = portraits.Dequeue();
        characterImage.sprite = Sprite.Create(t, new Rect(0,0,t.width, t.height), new Vector2(0.5f, 0.5f));
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray() ){
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        Debug.Log("End of Conversation");
        // Remove dialogue box
    }

    
}
