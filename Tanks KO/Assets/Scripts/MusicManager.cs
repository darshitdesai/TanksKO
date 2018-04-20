using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour {

    public Slider Volume;
    public AudioSource myMusic;

    public void Update()
    {
        myMusic.volume = Volume.value;
    }
}
