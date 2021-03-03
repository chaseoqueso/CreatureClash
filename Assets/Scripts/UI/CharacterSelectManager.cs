using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CharacterSelectManager : MonoBehaviour
{
    public List<PlayerObject> characters;

    [HideInInspector] public bool readyP1, readyP2;

    // Start is called before the first frame update
    void Start()
    {
        readyP1 = false;
        readyP2 = false;
    }

    public void PlayGame()
    {
        if( readyP1 && readyP2 ){
            // Start game
            SceneManager.LoadScene("ChaseScene");
            Debug.Log("Start Game");
        }
    }

    public void LoadMenu()
    {
        //SceneManager.LoadScene("MainMenu");
        Debug.Log("Loading menu...");
    }
}
