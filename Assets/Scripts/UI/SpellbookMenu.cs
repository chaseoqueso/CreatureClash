using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellbookMenu : MonoBehaviour
{
    public static bool spellbookIsActive = false;
    public GameObject spellbookUI;

    private Player currentPlayer;



    private void assignCreatureValues()
    {
        // Loop through up to four creature panels and assign values
        
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
