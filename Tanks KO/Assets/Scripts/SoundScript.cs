using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundScript : MonoBehaviour {

    public GameObject soundControl;
    public Sprite soundOffSprite;
    public Sprite soundOnSprite;

    // Use this for initialization
    void Start () {
        if (AudioListener.pause == true)
        {
            soundControl.GetComponent<Image>().sprite = soundOffSprite;
        }
        else
        {
            soundControl.GetComponent<Image>().sprite = soundOnSprite;
        }
    }

    public void SoundController()
    {
        if (AudioListener.pause == true)
        {
            AudioListener.pause = false;
            soundControl.GetComponent<Image>().sprite = soundOnSprite;
        }
        else
        {
            AudioListener.pause = true;
            soundControl.GetComponent<Image>().sprite = soundOffSprite;
        }
    }

}
