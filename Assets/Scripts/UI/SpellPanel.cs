using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellPanel : MonoBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public int index;

    [HideInInspector] public int spellCostValue;

    // UI data
    public TMP_Text spellName;
    public TMP_Text spellDescription;
    public TMP_Text spellCOST;

    public Button panelButton;

    public void setUIValues(PlayerAction a)
    {
        spellName.text = a.actionName;
        spellDescription.text = a.description;

        spellCostValue = a.manaCost;      
        spellCOST.text = spellCostValue + "";

        if( spellCostValue > player.actionPoints ){
            setInteractable(false);
        }

        // if no valid targets, set not interactable
        List<Action.EffectGroup> actionEffectGroups = a.actionEffectGroups;
        foreach( Action.EffectGroup effectGroup in actionEffectGroups ){
            if(!GameManager.Instance.enableTargetingOnTargets(player.playerNumber, effectGroup.targetType, effectGroup.targetRestriction, effectGroup.blockedByFrontline, false)){
                setInteractable(false);
                GameManager.Instance.resetTargeting();
                return;
            }
        }
        GameManager.Instance.resetTargeting();
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

    public void castSpell()
    {
        // Select targets and then queue the action
        player.queueSpell(index);

        GetComponentInParent<SpellbookMenu>().closeSpellbook();
    }
}
