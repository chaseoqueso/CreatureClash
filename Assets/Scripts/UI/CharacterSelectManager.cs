using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CharacterSelectManager : MonoBehaviour
{
    public List<PlayerObject> characters;
    public List<Sprite> bannerSprites;

    [HideInInspector] public bool readyP1, readyP2;

    // Start is called before the first frame update
    void Start()
    {
        readyP1 = false;
        readyP2 = false;
    }

    public void PlayGame()
    {
        //Destroy(gameObject);
        if( readyP1 && readyP2 ){
            // Start game
            string level = Random.Range(0, 2) == 1 ? "ForestArena" : "CrossroadsArena";
            SceneManager.LoadScene(level);
            Debug.Log("Start Game");
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Loading menu...");
        Destroy(GameManager.Instance.gameObject);
    }
}
