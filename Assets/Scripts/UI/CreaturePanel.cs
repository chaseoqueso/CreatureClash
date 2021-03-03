using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreaturePanel : MonoBehaviour
{
    public Player player;
    public int index;
    public int currentMana;

    public int creatureCostValue;

    // UI data
    public TMP_Text creatureName;
    public TMP_Text creatureDescription;

    public TMP_Text creatureHP;
    public TMP_Text creatureDMG;
    public TMP_Text creatureSPD;
    public TMP_Text creatureDEF;
    public TMP_Text creatureCOST;

    public Image creatureIMG;

    public Button panelButton;

    public void setUIValues(CreatureObject creature)
    {
        creatureName.text = creature.CreatureName();
        creatureDescription.text = creature.FlavorText();

        //creatureIMG.mainTexture = creature.Texture();

        creatureCostValue = creature.ManaCost();        
        creatureCOST.text = creatureCostValue + "";

        // Set stats
        creatureHP.text = creature.MaxHealth() + "";
        creatureDMG.text = creature.BaseDamage() + "";
        creatureDEF.text = creature.BaseDefense() + "";
        creatureSPD.text = creature.BaseSpeed() + "";

        // if( creatureCostValue > currentMana ){
        //     setInteractable(false);
        // }
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

    public void summonCreature()
    {
        // Summons the given creature and disables the button
        player.summonCreature(index);
        setInteractable(false);

        GetComponentInParent<SpellbookMenu>().closeSpellbook();
    }
}
