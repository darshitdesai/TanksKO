using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public static int playerCount;
    public static string mapName;

    private void Start()
    {
        playerCount = 2;
        mapName = "desert";
        UpdateText();
    }

    public void SetPlayerCount (int playerCount)
    {
        GameStarter.playerCount = playerCount;
        UpdateText();
    }

    public void SetMap(string mapName)
    {
        GameStarter.mapName = mapName;
        UpdateText();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }

    private void UpdateText()
    {
        GetComponent<UnityEngine.UI.Text>().text = "Start Game: (" + playerCount + " players, " + mapName + ")";
    }
}
