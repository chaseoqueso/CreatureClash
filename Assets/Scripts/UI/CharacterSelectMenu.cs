using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectMenu : MonoBehaviour
{
    public CharacterSelectManager charManager;

    public Button readyButton;

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
        setReadyButtonActive(false);

        // Set readyP bool to true
        if( playerNum == 1 ){
            GameManager.Instance.player1Object = charManager.characters[charIndex];
            charManager.readyP1 = true;
            return;
        }
        GameManager.Instance.player2Object = charManager.characters[charIndex];
        charManager.readyP2 = true;
    }

    public void setReadyButtonActive(bool setActive)
    {
        readyButton.interactable = setActive;
    }

    public void setUIValues(TMP_Text characterName, TMP_Text charClass, TMP_Text description, Image img, PlayerObject character)
    {
        characterName.text = character.characterName;
        charClass.text = character.characterClass;
        description.text = character.flavorText;
        Texture2D t = character.characterSprite;
        img.sprite = Sprite.Create(t, new Rect(0,0,t.width, t.height), new Vector2(0.5f, 0.5f));
    }

    // if the portraits are textures instead of sprites
    // this was how we did it for creatures (would also do this for dialogue)
    // Texture2D t = creature.Texture();
    // creatureIMG.sprite = Sprite.Create(t, new Rect(0,0,t.width, t.height), new Vector2(0.5f, 0.5f));
}
