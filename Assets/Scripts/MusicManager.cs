using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource source;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
    }

    public void playMusic(AudioClip loop, AudioClip intro = null)
    {
        if(intro)
        {
            source.clip = intro;
            source.loop = false;
            source.Play();
            StartCoroutine(playLoopAfterIntroFinish(loop));
        }
        else
        {
            if(source.clip = loop)
                return;
                
            source.clip = loop;
            source.loop = true;
            source.Play();
        }
    }

    private IEnumerator playLoopAfterIntroFinish(AudioClip loop)
    {
        while(source.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        source.clip = loop;
        source.loop = true;
        source.Play();
    }
}
