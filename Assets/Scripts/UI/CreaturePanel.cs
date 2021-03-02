using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreaturePanel : MonoBehaviour
{
    // UI data
    public TMP_Text creatureName;
    public TMP_Text creatureDescription;

    public TMP_Text creatureHP;
    public TMP_Text creatureDMG;
    public TMP_Text creatureSPD;
    public TMP_Text creatureDEF;
    public TMP_Text creatureCOST;

    public Image creatureIMG;

    public void setUIValues(CreatureObject creature)
    {
        creatureName.text = creature.CreatureName();
        creatureDescription.text = creature.FlavorText();

        //creatureIMG.mainTexture = creature.Texture();
        
        creatureCOST.text = creature.ManaCost() + "";

        // Set stats
        creatureHP.text = creature.MaxHealth() + "";
        creatureDMG.text = creature.BaseDamage() + "";
        creatureDEF.text = creature.BaseDefense() + "";
        creatureSPD.text = creature.BaseSpeed() + "";
    }
}
