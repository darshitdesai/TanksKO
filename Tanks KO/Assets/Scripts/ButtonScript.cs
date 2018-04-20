using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
  
    public void HelpBtn(string help)
    {
        SceneManager.LoadScene(help);
    }

    public void BackBtn(string main)
    {
        SceneManager.LoadScene(main);
    }

    public void SettingBtn(string setting)
    {
        SceneManager.LoadScene(setting);
    }
}
