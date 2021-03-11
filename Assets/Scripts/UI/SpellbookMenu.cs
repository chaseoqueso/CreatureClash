using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellbookMenu : MonoBehaviour
{
    public GameObject spellbookUI;

    public GameObject creaturePanelPrefab;
    public GameObject spellPanelPrefab;

    public TMP_Text currentMana;
    [HideInInspector] public int manaValue;

    private Player currentPlayer;
    private List<GameObject> creaturePanels;
    private List<GameObject> spellPanels;

    public float yPosValue = 800;
    public float yPosValueDecrease = 200;


    void Start()
    {
        creaturePanels = new List<GameObject>();
        spellPanels = new List<GameObject>();

        spellbookUI.SetActive(false);
    }

    private void assignCreatureValues(List<int> indexes)
    {
        // Loop through up to four creature panels and assign values
        if( currentPlayer.cardsInHand.Count != 0 ){
            float yPos = yPosValue;
            int i = 0;
            foreach(CreatureObject creature in currentPlayer.cardsInHand){
                // Create the creature panel
                GameObject creaturePanel = Instantiate(creaturePanelPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
                creaturePanel.transform.position = new Vector3(500, yPos, 0);
                creaturePanels.Add(creaturePanel);

                CreaturePanel panel = creaturePanel.GetComponent<CreaturePanel>();

                // Set the UI values in the creature panel
                panel.player = currentPlayer;
                panel.index = i;
                panel.setUIValues(creature);

                // If already used, set the button to not interactable
                if( indexes.Contains(i) ){
                    panel.setInteractable(false);
                }
            
                // Decrease yPos for next creature panel
                yPos -= yPosValueDecrease;
                i++;
            }
            return;
        }
        // If no creatures left to summon, display something like an image or something idk
    }

    private void assignSpellValues()
    {
        List<PlayerAction> actions = currentPlayer.playerObject.actions;
        float yPos = yPosValue;

        // Loop through all of this player's spells and display the spell data
        for(int i = 0; i < actions.Count; i++){
            // Get current spell
            PlayerAction a = actions[i];

            // Create the panel
            GameObject spellPanel = Instantiate(spellPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            spellPanel.transform.position = new Vector3(1410, yPos, 0);
            spellPanels.Add(spellPanel);

            SpellPanel panel = spellPanel.GetComponent<SpellPanel>();

            // Set the UI values in the panel
            panel.player = currentPlayer;
            panel.index = i;
            panel.setUIValues(a);
        
            // Decrease yPos for next panel
            yPos -= yPosValueDecrease;
        }
    }

    private void clearSpellbook()
    {
        // Delete all creature panels
        foreach(GameObject panel in creaturePanels){
            Destroy(panel);
        }
        creaturePanels.Clear();

        // Delete all spell panels
        foreach(GameObject panel in spellPanels){
            Destroy(panel);
        }
        spellPanels.Clear();
    }

    public void closeSpellbook()
    {
        spellbookUI.SetActive(false);
        // Play animation

        // Set values -> make sure this doesn't have to come before SetActive(false)
        clearSpellbook();
    }

    public void backButton()
    {
        spellbookUI.SetActive(false);
        //Play animation

        clearSpellbook();
        GameManager.Instance.resetTargeting();
    }

    public void openSpellbook(Player player, List<int> creatureUsedIndexes)
    {
        // Set values
        currentPlayer = player;
        manaValue = currentPlayer.actionPoints;
        currentMana.text = manaValue + "";
        assignCreatureValues(creatureUsedIndexes);
        assignSpellValues();

        // Play animation
        spellbookUI.SetActive(true);
    }
}
