using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectMenu : MonoBehaviour
{
    public CharacterSelectManager charManager;

    // Player UI data
    public TMP_Text playerName;
    public TMP_Text playerClass;
    public TMP_Text playerDescription;
    public Image playerIMG;

    // Player data
    public int playerNum;
    private int charIndex;

    // Start is called before the first frame update
    void Start()
    {
        charIndex = 0;

        // Display first character option
        setUIValues(playerName, playerClass, playerDescription, playerIMG, charManager.characters[charIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nextCharacter()
    {
        incrementIndex(1);
        setUIValues(playerName, playerClass, playerDescription, playerIMG, charManager.characters[charIndex]);
    }

    public void prevCharacter()
    {
        incrementIndex(-1);
        setUIValues(playerName, playerClass, playerDescription, playerIMG, charManager.characters[charIndex]);
    }

    private void incrementIndex(int num)
    {
        charIndex += num;
        if(charIndex == charManager.characters.Count){
            charIndex = 0;
            return;
        }
        if(charIndex < 0){
            charIndex = charManager.characters.Count - 1;
        }
    }

    // Called when players select "ready"
    public void readyPlayer()
    {
        GameManager.Instance.player1.playerObject = charManager.characters[charIndex];
        // could do some effects to gray out the button and the image

        // Set readyP bool to true
        if( playerNum == 1 ){
            charManager.readyP1 = true;
            return;
        }
        charManager.readyP2 = true;
    }

    public void setUIValues(TMP_Text characterName, TMP_Text charClass, TMP_Text description, Image img, PlayerObject character)
    {
        characterName.text = character.characterName;
        charClass.text = character.characterClass;
        description.text = character.flavorText;
        img.sprite = character.characterPortrait;
    }
}
