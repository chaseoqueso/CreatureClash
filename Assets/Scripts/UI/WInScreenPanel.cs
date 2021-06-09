using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WInScreenPanel : MonoBehaviour
{
    public TMP_Text winText;

    public void setWinText(int playerIndex)
    {
        if(playerIndex == 2){
            winText.text = "Player 1 wins!";
            return;
        }
        winText.text = "Player 2 wins!";
    }

    public void loadMenu()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(MusicManager.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
