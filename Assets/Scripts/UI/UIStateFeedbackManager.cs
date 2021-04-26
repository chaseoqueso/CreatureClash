using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStateFeedbackManager : MonoBehaviour
{
    public GameObject statusFeedbackUI;
    public TMP_Text statusText;

    public Creature creature;

    void Start()
    {
        setStatusFeedbackUIActive(false);
    }

    public void setStatusUI()
    {
        string s = "<b>" + creature.name + "</b>";
        s += "\nHP: " + creature.currentHealth + "/" + creature.currentMaxHP;
        
        if( creature.activeEffects.Count != 0 ){
            s += "\nStatus: ";

            foreach( Action.statusEffect effect in creature.activeEffects.Keys ){
                s += effect.statusType; // Should probably be an effect NAME which we have to add
                s += " (" + effect.effectDuration + " rounds)";
            }
        }

        statusText.text = s;
    }

    public void setStatusFeedbackUIActive(bool value)
    {
        statusFeedbackUI.SetActive(value);
    }
}


/*

Add health bars (non-interactable sliders, if possible have it change color as it decreases
value -> see the tanks tutorial or others)

*/