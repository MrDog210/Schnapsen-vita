using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    Object[] Glasba;
    public AudioSource audioPlayer;
    public static MusicManager instance;
    //KeyCode[] konamicode;
    //public AudioClip poljka;

    //int currentKeyIndex = 0;

    //void Start()
    //{
    //    konamicode = new KeyCode[]{KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow,
    //KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A};
    //}

    //OnInput(KeyCode KeyCodeValue)
    //{
    //    if (KeyCodeValue == konamicode[currentKeyIndex])
    //   {
    //        currentKeyIndex++;
    //        if (currentKeyIndex >= konamicode.Length)
    //
    //    }
    //    else
    //    {
    //        currentKeyIndex = 0;
    //    }
    //}

    private void Awake()
    {
        if (instance == null) //Preveri, èe musicmanager že obstaja, èe obstaja se zbriše
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
            
        DontDestroyOnLoad(transform.gameObject);
        Glasba = Resources.LoadAll("Music", typeof(AudioClip));
        playRandomMusic();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (!audioPlayer.isPlaying)
            playRandomMusic();
    }

    void playRandomMusic()
    {
        audioPlayer.clip = Glasba[Random.Range(0, Glasba.Length)] as AudioClip;
        audioPlayer.Play();
    }
}
