using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    private Texture2D texture;
    int[] heightNodes;
    [Header("Properties")]
    public int width = 1920;
    public int height = 1080;
    public int pixelsPerUnit = 100;
    public float gravity;
    public int powerupCount = 3;

    [Header("Objects")]
    public GameObject[] powerups;

    [HideInInspector]
    public bool ready;

    public void Start()
    {
        texture = new Texture2D(width, height);
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        Generate(GameStarter.mapName);
        ready = true;
    }

    public void Generate(string biome)
    {
        int offset = 200;
        int heightVariance;
        int heightLimit;
        Color colourA;
        Color colourB;
        Color colourSky;
        float[] colourRules = new float[8];
        float colourSkew = 0.5f;

        if (biome == "desert")
        {
            heightNodes = new int[100];
            heightVariance = 30;
            heightLimit = 400;
            colourA = new Color(0.9f, 0.9f, 0.8f);
            colourB = new Color(0.8f, 0.6f, 0.4f);
            colourSky = new Color(0.175f, 0.6805f, 0.6627f);
            colourRules = new float[] { 0.2f, 0.2f, 0f, 0.2f, 0f, 0f, 0f, -0.2f };
        }
        else if (biome == "jungle")
        {
            heightNodes = new int[60];
            heightVariance = 100;
            heightLimit = 600;
            colourA = new Color(0.2f, 0.5f, 0.1f);
            colourB = new Color(0.2f, 0.6f, 0.1f);
            colourSky = new Color(0.5f, 0.7f, 0.5f);
            colourRules = new float[] { 1f, -0.1f, 0f, -0.1f, -0.1f, -0.1f, -0.1f, -0.1f };
        }
        else if (biome == "mountain")
        {
            heightNodes = new int[40];
            heightVariance = 200;
            heightLimit = 600;
            colourA = Color.gray;
            colourB = Color.white;
            colourSky = new Color(0.5f, 0.5f, 0.8f);
            colourSkew = 1f;
            colourRules = new float[] { -0.2f, 0.1f, 0.0f, 0f, 0.1f, 0f, 0f, -0.01f };
        }
        else if (biome == "moon")
        {
            gravity *= 0.5f;
            heightNodes = new int[20];
            heightVariance = 200;
            heightLimit = 400;
            colourA = Color.gray;
            colourB = new Color(0.2f, 0.2f, 0.2f);
            colourSky = Color.black;
            colourSkew = 0.9f;
            colourRules = new float[] { -0.1f, -0.1f, -0.1f, -0.1f, -0.1f, -0.1f, -0.1f, 0.2f };
        }
        else
        {
            heightNodes = new int[50];
            heightVariance = 50;
            heightLimit = 500;
            colourA = Color.magenta;
            colourB = Color.cyan;
            colourSky = Color.green;
            colourRules = new float[] { -1f, 1f, 1f, -1f, 1f, -1f, -1f, -1f };
            colourSkew = 0.3f;
        }
        int terrainHeight = heightLimit / 2;
        int maxDistance;
        for (int i = 0; i < heightNodes.Length; i++)
        {
            float change = (2*Random.value-1) * heightVariance;
            if (change > 0)
                maxDistance = heightLimit - terrainHeight;
            else
                maxDistance = terrainHeight;

            terrainHeight = terrainHeight + (int)( maxDistance / (Mathf.PI / 2) * Mathf.Atan(Mathf.PI / 2 * change / maxDistance));
            heightNodes[i] = terrainHeight + offset;
        }

        float[][] proportionMap = new float[width][];
        for (int x = 0; x < width; x++)
        {
            proportionMap[x] = new float[height];
        }

        for (int y = height-1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                float nodePosition = (float)(x * (heightNodes.Length-1)) / texture.width;
                float nodeProportion = nodePosition % 1;
                int floor = (int)(
                    heightNodes[(int)nodePosition]*(1-nodeProportion)
                    + heightNodes[(int)nodePosition+1]*nodeProportion );

                Color colour;
                if (y > floor)
                {
                    colour = colourSky;
                }
                else if (y == floor)
                {
                    proportionMap[x][y] = Mathf.Clamp(Random.value - 1f + 2 * colourSkew, 0, 1);
                    colour = colourA * (1 - proportionMap[x][y]) + colourB * proportionMap[x][y];
                }
                else
                {
                    int index = 0;
                    int count = 0;
                    float talliedProportion = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        int scanX = Mathf.Clamp(x + i, 0, width - 1);
                        int scanY = Mathf.Clamp(y + 1, 0, height - 1);
                        float proportion = proportionMap[scanX][scanY];
                        if (proportion > 0.5)
                            index += (int)Mathf.Pow(2, count);
                        talliedProportion += proportion;
                        count++;
                    }
                    float averageProportion = talliedProportion / count;
                    averageProportion = Mathf.Clamp(averageProportion + colourRules[index], 0, 1);
                    proportionMap[x][y] = averageProportion;
                    colour = colourA * (1 - proportionMap[x][y]) + colourB * proportionMap[x][y];
                }
                texture.SetPixel(x, y, colour);
            }
        }
        texture.Apply();

        for (int i = 0; i < powerupCount; i++)
        {
            GameObject powerup = Instantiate(powerups[(int)(Random.value * powerups.Length)]);
            int mapX = (int)(Random.value * width);
            int mapY = getHeight(mapX);
            powerup.GetComponent<Transform>().position = positionToScreenSpace(new Vector2(mapX, mapY));
        }

    }

    public int getHeight(int x)
    {
        if (x < 0) x = 0;
        if (x > width-1) x = width-1;
        float nodePosition = (float)(x * (heightNodes.Length - 1)) / texture.width;
        float nodeProportion = nodePosition % 1;
        int floor = (int)(heightNodes[(int)nodePosition] * (1 - nodeProportion));
        if (nodeProportion > 0)
            floor += (int)(heightNodes[(int)nodePosition + 1] * nodeProportion);
        return floor;
    }

    public float getSlope(int x)
    {
        if (x < 0) x = 0;
        if (x > width-1) x = width-1;
        float nodePosition = (float)(x * (heightNodes.Length - 1)) / texture.width;
        float heightDifference = heightNodes[(int)nodePosition + 1] - heightNodes[(int)nodePosition];
        float nodeWidth = (float)texture.width / (heightNodes.Length - 1);
        return heightDifference / nodeWidth;
    }

    public Vector2 positionToScreenSpace(Vector2 position)
    {
        position -= new Vector2(width / 2, height / 2);
        return new Vector2(position.x, position.y) / pixelsPerUnit;
    }
    public Vector2 screenSpaceToPosition(Vector2 screenSpace)
    {
        Vector2 position = new Vector2(screenSpace.x, screenSpace.y) * pixelsPerUnit;
        return position + new Vector2(width / 2, height / 2);
    }
}
