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
    public Image fill;

    public Creature creature;

    void Start()
    {
        // Set starting health values
        slider.maxValue = creature.currentMaxHP;
        slider.value = creature.currentHealth;
        fill.color = healthGradient.Evaluate(1f);

        // Status UI (on hover)
        if(creature.player.playerNumber == 2){
            statusText.transform.localScale = new Vector3(-1, 1, 1);
        }
        setStatusFeedbackUIActive(false);
    }

    public void updateMaxHealthBar()
    {
        // set new max value
        slider.maxValue = creature.currentMaxHP;
        // update hp bar
        fill.color = healthGradient.Evaluate(slider.normalizedValue);
    }

    public void updateCurrentHealthBar()
    {
        // set new current value
        slider.value = creature.currentHealth;
        // update hp bar
        fill.color = healthGradient.Evaluate(slider.normalizedValue);
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