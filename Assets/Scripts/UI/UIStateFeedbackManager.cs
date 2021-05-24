using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStateFeedbackManager : MonoBehaviour
{
    public GameObject statusFeedbackUI;
    public TMP_Text statusText;

    // public GameObject healthBar;
    public Slider slider;
    public Gradient healthGradient;

    public Creature creature;

    void Start()
    {
        // Set starting health values
        setMaxHealthBar();
        setCurrentHealthBar();

        // Status UI (on hover)
        if(creature.player.playerNumber == 2){
            statusText.transform.localScale = new Vector3(-1, 1, 1);
        }
        setStatusFeedbackUIActive(false);
    }

    public void setCurrentHealthBar()
    {
        slider.value = creature.currentHealth;
    }

    public void setMaxHealthBar()
    {
        slider.maxValue = creature.currentMaxHP;
    }

    public void setStatusUI()
    {
        string s = "<b>" + creature.creature.CreatureName() + "</b>";
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