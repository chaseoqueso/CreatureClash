using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStateFeedbackManager : MonoBehaviour
{
    public GameObject statusFeedbackUI;
    public TMP_Text statusText;

    void Start()
    {
        setStatusFeedbackUIActive(false);
    }

    public void setStatusUI(Creature hoverCreature)
    {
        string s = "<b>" + hoverCreature.name + "</b>";
        s += "\nHP: " + hoverCreature.currentHealth + "/" + hoverCreature.currentMaxHP;
        
        if( hoverCreature.activeEffects.Count != 0 ){
            s += "\nStatus: ";

            foreach( Action.statusEffect effect in hoverCreature.activeEffects.Keys ){
                s += effect.statusType; // Should probably be an effect NAME which we have to add
                s += " (" + effect.effectDuration + " rounds)";
            }
        }

        statusText.text = s;
    }

    void setStatusFeedbackUIActive(bool value)
    {
        statusFeedbackUI.SetActive(value);
    }
}


/*

Add health bars (non-interactable sliders, if possible have it change color as it decreases
value -> see the tanks tutorial or others)


Add to Creature.cs:

- call setStatusUI(this) in OnMouseOver()
- add onMouseExit() and have it call setStatusFeedbackUIActive(false);

*/