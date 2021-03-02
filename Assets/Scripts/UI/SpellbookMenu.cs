using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellbookMenu : MonoBehaviour
{
    public static bool spellbookIsActive = false;
    public GameObject spellbookUI;

    public GameObject creaturePanelPrefab;


    private Player currentPlayer;
    private List<GameObject> creaturePanels;



    void Start()
    {
        creaturePanels = new List<GameObject>();

        spellbookUI.SetActive(false);
        spellbookIsActive = false;
    }

    private void assignCreatureValues(List<int> indexes)
    {
        // Loop through up to four creature panels and assign values
        if( currentPlayer.cardsInHand.Count != 0 ){
            float yPos = 800;
            int i = 0;
            foreach(CreatureObject creature in currentPlayer.cardsInHand){
                // Create the creature panel
                GameObject creaturePanel = Instantiate(creaturePanelPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
                creaturePanel.transform.position = new Vector3(500, yPos, 0);
                creaturePanels.Add(creaturePanel);

                CreaturePanel panel = creaturePanel.GetComponent<CreaturePanel>();

                // Set the UI values in the creature panel
                panel.setUIValues(creature);
                panel.player = currentPlayer;
                panel.index = i;

                // If already used, set the button to not interactable
                if( indexes.Contains(i) ){
                    panel.setInteractable(false);
                }
            
                // Decrease yPos for next creature panel
                yPos -= 200;
                i++;
            }
            return;
        }
        // If no creatures left to summon, display something like an image or something idk
    }

    private void assignSpellValues()
    {

    }

    private void clearSpellbook()
    {
        // Delete all creature panels
        foreach(GameObject panel in creaturePanels){
            Destroy(panel);
        }
        creaturePanels.Clear();
    }

    public void closeSpellbook()
    {
        spellbookUI.SetActive(false);
        spellbookIsActive = false;
        // Play animation

        // Set values -> make sure this doesn't have to come before SetActive(false)
        clearSpellbook();
    }

    public void openSpellbook(Player player, List<int> creatureUsedIndexes)
    {
        // Set values
        currentPlayer = player;
        assignCreatureValues(creatureUsedIndexes);
        assignSpellValues();

        // Play animation
        spellbookUI.SetActive(true);
        spellbookIsActive = true;
    }
}
