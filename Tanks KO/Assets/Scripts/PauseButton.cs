using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour {

    public bool pause;

    // Use this for initialization
    void Start()
    {
        pause = false;
    }

    public void Pause()
    {
        pause = !pause;

        if (pause)
        {
            Time.timeScale = 0;
        }
        else if (!pause)
        {
            Time.timeScale = 1;
        }
    }

}
