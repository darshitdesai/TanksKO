using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode gunUp;
    public KeyCode gunDown;
    public KeyCode fire;
    public KeyCode exit;

    [Header("Objects")]
    public Map map;
    public GameObject tankPrefab;
    public Text announcerText;
    public Text healthText;
    public Text fuelText;

    private GameObject[] tanks;
    private TankBehaviour[] tankScripts;
    private int activeTankID = -1;
    private GameObject activeTank;
    private TankBehaviour activeTankScript;
    private string[] tankNames = { "Red", "Green", "Blue", "Purple"};
    //0: Red
    //45: Orange
    //90: Lime
    //135: Green
    //180: Cyan
    //225: Blue
    //270: Mauve
    //315: Fuchsia

    private string[] factionNames = { "Americans", "Soviets", "Nazis", "Martians", "Skeleton Army", "Fidget Spinner Convention", "Orks", "entire human race" };
    private string[] eventNames = { "It's World War 3", "Everyone's jimmies are rustled", "bloggers are triggered", "it's the war on drugs all over again", "it's the end of the world as we know it" };

    IEnumerator Start ()
    {
        announcerText.text = "Oh no the " + factionNames[(int)(Random.value * factionNames.Length)] + " attacked the " + factionNames[(int)(Random.value * factionNames.Length)] + " and now " + eventNames[(int)(Random.value * eventNames.Length)];

        yield return new WaitUntil(() => map.ready);
        int playerCount = GameStarter.playerCount;
        if (playerCount == 0)
            playerCount = 2;

        tanks = new GameObject[playerCount];
        tankScripts = new TankBehaviour[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            int spawnX = (i + 1) * map.width / (playerCount + 1);
            Vector2 spawnPosition2D = map.positionToScreenSpace(new Vector2(spawnX, map.getHeight(spawnX)));
            Vector3 spawnPosition3D = new Vector3(spawnPosition2D.x, spawnPosition2D.y, tankPrefab.GetComponent<Transform>().position.z);
            tanks[i] = Instantiate(tankPrefab, spawnPosition3D, Quaternion.identity) as GameObject;
            tankScripts[i] = tanks[i].GetComponent<TankBehaviour>();
            tankScripts[i].map = map;
            float hue = (float)i / playerCount;
            tanks[i].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(hue, 1, 1);
            tanks[i].name = tankNames[(int)(hue * tankNames.Length)];
        }
        changeTurn();
    }

    void Update ()
    {

        if (Input.GetKey(exit))
        {
            SceneManager.LoadScene("main");
        }

        if (activeTankScript == null)
            return;

        healthText.text = "Health: " + (int)activeTankScript.health;
        fuelText.text = "Fuel: " + (int)activeTankScript.fuel;

        if (activeTank == null)
            return;

        if (Input.GetKey(moveLeft))
        {
            activeTankScript.Move(0);
        }
        if (Input.GetKey(moveRight))
        {
            activeTankScript.Move(1);
        }
        if (Input.GetKey(gunDown))
        {
            activeTankScript.TiltBarrel(0);
        }
        if (Input.GetKey(gunUp))
        {
            activeTankScript.TiltBarrel(1);
        }
        if (Input.GetKey(fire))
        {
            activeTankScript.Fire();
            changeTurn();
        }
    }

    void changeTurn()
    {
        activeTank = null;
        if (activeTankID == -1)
            activeTankID = (int)(Random.value * tanks.Length);
        Invoke("startTurn", 4);
    }
    void startTurn()
    {
        int nextTankID = getValidTank((activeTankID + 1) % tanks.Length);
        if (nextTankID == activeTankID)
        {
            announcerText.text = "The winner is " + tanks[activeTankID].name + ", thanks for playing!!! Press esc to exit";
            return;
        }
        if (nextTankID == -1)
        {
            announcerText.text = "Everyone is dead, good job";
            return;
        }

        activeTankID = nextTankID;
        activeTank = tanks[activeTankID];
        activeTankScript = tankScripts[activeTankID];
        
        activeTankScript.fuel = activeTankScript.maxFuel;

        announcerText.text = "It is " + activeTank.name + "'s turn";
        announcerText.color = activeTank.GetComponent<SpriteRenderer>().color;
    }
    int getValidTank(int startID)
    {
        for (int i = 0; i < tanks.Length; i++)
        {
            int ID = (i + startID) % tanks.Length;
            if (tanks[ID] != null)
                return ID;
        }
        return -1;
    }
}
