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

        // if the icons are textures instead of sprites for some reason (but they can prob just be sprites)
        // this was how we did it for creatures
        // Texture2D t = creature.Texture();
        // creatureIMG.sprite = Sprite.Create(t, new Rect(0,0,t.width, t.height), new Vector2(0.5f, 0.5f));

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
