using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUIPanel : MonoBehaviour
{
    public Action ability;
    public int abilityIndex;
    
    // UI data
    public TMP_Text abilityName;
    public TMP_Text abilityDescription;
    public Image abilityIcon;

    public Button panelButton;


    public void setUIValues( Action a )
    {
        ability = a;

        abilityName.text = ability.actionName;
        abilityDescription.text = ability.description;
        abilityIcon.sprite = ability.actionIcon;
    }

    public void setInteractable(bool setActive)
    {
        panelButton.interactable = setActive;

        if(!setActive){
            // Set visual if false
            return;
        }
        // Set visual if true
    }

    public void selectAbility()
    {
        // Select targets and then queue the action for the creature
        GetComponentInParent<AbilityUIManager>().creature.selectTargetsForAction(abilityIndex);

        // Close the menu
        setInteractable(false);
        GetComponentInParent<AbilityUIManager>().closeActionSelectUI();
    }
}
