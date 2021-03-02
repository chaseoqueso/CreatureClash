using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUIManager : MonoBehaviour
{
    public static bool actionSelectIsActive = false;
    public GameObject actionSelectUI;

    public GameObject abilityUIPanelPrefab;
    public List<GameObject> abilityUIPanels;

    public Creature creature;

    void Start()
    {
        actionSelectUI.SetActive(false);
    }
    
    public void openActionSelectUI(Creature c)
    {
        actionSelectUI.SetActive(true);
        actionSelectIsActive = true;
        creature = c;
        setActionUIValues();
    }

    private void setActionUIValues()
    {
        List<Action> actionList = creature.getActionList();

        float xPos = creature.transform.position.x;
        float yPos = creature.transform.position.y;

        foreach(Action a in actionList){
            GameObject actionPanel = Instantiate(abilityUIPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            actionPanel.transform.position = new Vector3(xPos, yPos, 0);
            abilityUIPanels.Add(actionPanel);

            actionPanel.GetComponent<AbilityUIPanel>().setUIValues(a);

            yPos += 100;
        }

        // Could add stuff to check for cooldowns here and keep things disabled if on cooldown
        // and add a number visual for how many rounds left
    }
}
