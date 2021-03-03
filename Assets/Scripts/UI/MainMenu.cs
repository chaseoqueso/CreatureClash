using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Go to character select screen
        SceneManager.LoadScene("CharacterSelectMenu");
    }

    public void QuitGame()
    {
        // Debug.Log("QUIT!");
        Application.Quit();
    }


}
