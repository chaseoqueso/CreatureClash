using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellbookMenu : MonoBehaviour
{
    public static bool spellbookIsActive = false;
    public GameObject spellbookUI;

    public GameObject creaturePanelPrefab;

    private Player currentPlayer;



    private void assignCreatureValues()
    {
        // Loop through up to four creature panels and assign values
        if( currentPlayer.cardsInHand.Count != 0 ){
            float yPos = 305;
            foreach(CreatureObject creature in currentPlayer.cardsInHand){
                GameObject creaturePanel = Instantiate(creaturePanelPrefab, new Vector3(-447, yPos, 0), Quaternion.identity);
                // creaturePanel.

                // Decrease yPos for next creature panel
                yPos -= 210;
            }
            
            return;
        }

        // If no creatures left to summon, display something like an image or something idk
    }

    private void assignSpellValues()
    {

    }

    public void clearSpellbook()
    {

    }

    public void closeSpellbook()
    {
        spellbookUI.SetActive(false);
        spellbookIsActive = false;
        // Play animation

        // Set values
        clearSpellbook();
    }

    public void openSpellbook(Player player)
    {
        // Set values
        currentPlayer = player;
        assignCreatureValues();
        assignSpellValues();

        // Play animation
        spellbookUI.SetActive(true);
        spellbookIsActive = true;
    }
}
