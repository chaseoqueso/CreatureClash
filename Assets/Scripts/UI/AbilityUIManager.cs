using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : MonoBehaviour
{
    public GameObject actionSelectUI;

    public GameObject abilityUIPanelPrefab;
    [HideInInspector] public List<GameObject> abilityUIPanels;

    [HideInInspector] public Creature creature;

    public float abilityUIPanelHeight = 110;
    public float abilityUIPanelWidth = 370;

    void Start()
    {
        actionSelectUI.SetActive(false);
    }
    
    public void openActionSelectUI(Creature c)
    {
        actionSelectUI.SetActive(true);
        creature = c;
        setActionUIValues();
    }

    public void closeActionSelectUI()
    {
        actionSelectUI.SetActive(false);

        // Delete all panels
        foreach(GameObject panel in abilityUIPanels){
            Destroy(panel);
        }
        abilityUIPanels.Clear();
    }

    private void setActionUIValues()
    {
        List<Action> actionList = creature.getActionList();

        float xPos = 500; // creature.transform.position.x;
        float yPos = 800; // creature.transform.position.y;
        // float panelSize = abilityUIPanelHeight;

        for(int i = 0; i < actionList.Count; i++){
            GameObject actionPanel = Instantiate(abilityUIPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            actionPanel.transform.position = new Vector3(xPos, yPos, 0);
            abilityUIPanels.Add(actionPanel);

            AbilityUIPanel a = actionPanel.GetComponent<AbilityUIPanel>();
            a.setUIValues(actionList[i]);
            a.abilityIndex = i;

            if(actionList[i] == Action.swapRows && GameManager.Instance.getOtherRowForSamePlayer(creature.row).isAtMaxCreatures())
            {
                a.setInteractable(false);
            }

            yPos -= 100;

            // actionSelectUI.GetComponent<RectTransform>().sizeDelta = new Vector2(abilityUIPanelWidth, panelSize);
            // panelSize += abilityUIPanelHeight;
        }
        // Could add stuff to check for cooldowns here and keep things disabled if on cooldown
        // and add a number visual for how many rounds left
    }
}
