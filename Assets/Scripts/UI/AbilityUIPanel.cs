using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUIPanel : MonoBehaviour
{
    public Action ability;
    
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
        setInteractable(false);

        // Close the menu?
    }
}
