using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class BoardManagerTest : MonoBehaviour
{
    public static BoardManagerTest instance;
    private Transform BoardTransform;
    public List<Sprite> resources = new List<Sprite>(); //sprites of resources
    public GameObject tile, node;  //prefab of tile 
 
    public int xSize, ySize; //size of board (set in Unity)

    public GameObject[,] tiles; //tiles in board as an 2D array
    private GameObject[,] brickBoard;
    
    public bool IsShifting { get; set; } //checks if it is shifting
    public bool IsSwapping { get; set; } //checks if it is swapping

    private float border = 0.3f; //border between sprites
    private float shiftDelay = 0.15f;

    public bool bricked = false;
    public GameObject tileCheck;

    bool fRunning = false;
    bool justChecked = false;
    public bool IsResetting { get; set; }

    public TextMeshProUGUI resettingText;

    int enemyMaxHealth = 1500;
    int enemyHealth;
    public Slider enemyHealthBar;
    int enemyDamage = 35;

    public int currentPlayerHealth = 100;
    int playerDamage = 100;

    float timer = 0;
    int gameTimer = 0;
    public TextMeshProUGUI gameTimerText;

    int missionGoalCount;
    string mission;
    public bool runningMission;

    void Start()
    {

        instance = GetComponent<BoardManagerTest>();
        BoardTransform = GetComponent<Transform>();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.extents; //could be bounds.size

        //BoardTransform.position = new Vector3(-((xSize - 1f) * (offset.x + border) * 0.5f), BoardTransform.position.y, 0f); //position board so it is centered based on offset

        enemyHealthBar.maxValue = enemyMaxHealth;
        enemyHealth = enemyMaxHealth;

        IsSwapping = false; //set to false initially
        IsShifting = false;
        IsResetting = false;
        runningMission = false;
        
        resettingText.faceColor = new Color32(resettingText.faceColor.r, resettingText.faceColor.g, resettingText.faceColor.b, 0);

        CreateBoard(offset.x + border, offset.y + border);  
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >=  1f) 
        {
            timer -= 1f;
            gameTimer++;
            if (gameTimer % 10 == 0 && gameTimer != 0)
            {
                MissionMake();
            }
        }
        gameTimerText.text = "Timer: " + gameTimer;
        

        enemyHealthBar.value = enemyHealth;

        if (!IsShifting && !fRunning && !IsSwapping)
        {
            if (!justChecked)
            {
                CheckForBrick();
                
                if (bricked)
                {
                    Debug.Log(bricked);
                }
            }

            if (IsResetting)
            {
                Debug.Log("Done Resetting");
                shiftDelay = 0.15f;
                IsResetting = false;
            }
        }

        
    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];  //creates board with given size   
        brickBoard = new GameObject[xSize, ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize]; //list of tiles to the left
        Sprite previousBelow = null; //tile right below


        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x) - 0.25f, startY + (yOffset * y), 0), tile.transform.rotation); //creates a new tile at a location in game

                GameObject brickTile = Instantiate(tileCheck, new Vector3(startX + (xOffset * x) - 20f, startY + (yOffset * y) - 20f, 0), tile.transform.rotation);

                tiles[x, y] = newTile; //sets Tile to it's respective array location
                brickBoard[x, y] = brickTile;

                newTile.transform.parent = transform; //parents new tile to board manager
                brickTile.transform.parent = transform;

                List<Sprite> possibleResources = new List<Sprite>();
                possibleResources.AddRange(resources); //adds all resources to list

                possibleResources.Remove(previousLeft[y]); //removes any resource to the left
                possibleResources.Remove(previousBelow); //removes resource right below

                Sprite newSprite = possibleResources[UnityEngine.Random.Range(0, possibleResources.Count)]; //creates a new sprite from possible resources

                newTile.GetComponent<SpriteRenderer>().sprite = newSprite; //changes tile to chosen sprite

                previousLeft[y] = newSprite; //sets current sprite as left
                previousBelow = newSprite; //sets current sprite as bottom

            }
        }

        Debug.Log("checking");
        CheckForBrick();
    }

    public IEnumerator FindNullTiles()
    {
        fRunning = true;
        justChecked = false;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) //looks for null (invisible) tiles
                {
                    
                    yield return StartCoroutine(ShiftTilesDown(x, y)); //starts shift in current coloumn
                    break; //moves to next coloumn
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<TileTest>().ClearAllMatches(); //re-checks board
            }
        }
        fRunning = false;
    }

    private IEnumerator ShiftTilesDown(int x, int yStart)
    {
        IsShifting = true; //makes it so you can't swap
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {  
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            {
                nullCount++; //counts how many missing tiles
            }
            renders.Add(render); //adds tiles above selected tile inclusive into list
        }

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            if (renders.Count == 1)
            {
                renders[0].sprite = GetNewSprite(x, ySize - 1);
            }
            else
            {
                for (int k = 0; k < renders.Count - 1; k++)
                {
                    renders[k].sprite = renders[k + 1].sprite; //replaces current tile with one above
                    renders[k + 1].sprite = GetNewSprite(x, ySize - 1); //creates new tile at the very top
                }
            }
        }

        IsShifting = false; //allows swapping
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(resources); //add in all resources

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);//remove tilePossibility to left
        }
        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);//remove tilePossibility to right
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);//remove tilePossibility under
        }

        return possibleCharacters[UnityEngine.Random.Range(0, possibleCharacters.Count)]; //random possible sprite
    }

    private void CheckForBrick()
    {
        justChecked = true;
        resettingText.faceColor = new Color32(resettingText.faceColor.r, resettingText.faceColor.g, resettingText.faceColor.b, 0);
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                brickBoard[x, y].GetComponent<TileCheck>().GetComponent<SpriteRenderer>().sprite = tiles[x, y].GetComponent<SpriteRenderer>().sprite;
            }
        }
        
        for (int x = 0; x < xSize - 1; x++)
        {
            for (int y = 0; y < ySize - 1; y++)
            {
                if (brickBoard[x, y].GetComponent<TileCheck>().TestBoard(brickBoard[x, y + 1].GetComponent<TileCheck>())) //Checks above swap
                {
                    bricked = false;
                    return;
                }
                if (brickBoard[x, y].GetComponent<TileCheck>().TestBoard(brickBoard[x + 1, y].GetComponent<TileCheck>())) //Checks right swap
                {
                    bricked = false;
                    return;
                }
            }
            if (brickBoard[x, ySize - 1].GetComponent<TileCheck>().TestBoard(brickBoard[x + 1, ySize - 1].GetComponent<TileCheck>())) //checks horizontal swap whole top row
            {
                bricked = false;
                return;
            }
        }

        for (int y = 0; y < ySize - 1; y++)
        {
            if (brickBoard[xSize - 1, y].GetComponent<TileCheck>().TestBoard(brickBoard[xSize - 1, y + 1].GetComponent<TileCheck>())) //Checks above swap in last coloumn
            {
                bricked = false;
                return;
            }
        }
        StopCoroutine(ResetText());
        StartCoroutine(ResetText());
        bricked = true;
    }

    public void ResetBoard()
    {
        shiftDelay = 0.05f;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<SpriteRenderer>().sprite = null;
                tiles[x, y].transform.GetChild(0).gameObject.SetActive(false);
                tiles[x, y].transform.GetChild(1).gameObject.SetActive(false);
                tiles[x, y].transform.GetChild(2).gameObject.SetActive(false);
                tiles[x, y].transform.GetChild(3).gameObject.SetActive(false);
                tiles[x, y].transform.GetChild(4).gameObject.SetActive(false);
            }
        }
        StartCoroutine(BoardManagerTest.instance.FindNullTiles());
    }

    private IEnumerator ResetText()
    {
        IsResetting = true;
        Color start = resettingText.faceColor;
        Color end = new Color32(resettingText.faceColor.r, resettingText.faceColor.g, resettingText.faceColor.b, 255);

        for (float t = 0f; t < 1; t += Time.deltaTime)
        {
            resettingText.faceColor = Color.Lerp(start, end, t);
            yield return null;
        }

        resettingText.faceColor = end;

        yield return new WaitForSeconds(0.5f);
        ResetBoard();

        while (IsResetting)
        {
            for (float t = 0f; t < 1; t += Time.deltaTime)
            {
                if (!IsResetting) { break; }
                resettingText.faceColor = Color.Lerp(end, start, t);
                yield return null;
            }

            resettingText.faceColor = start;

            for (float t = 0f; t < 1; t += Time.deltaTime)
            {
                if (!IsResetting) { break; }
                resettingText.faceColor = Color.Lerp(start, end, t);
                yield return null;
            }

            resettingText.faceColor = end;

            
        }

        resettingText.faceColor = end;
        for (float t = 0f; t < 1; t += Time.deltaTime)
        {
            resettingText.faceColor = Color.Lerp(end, start, t);
            yield return null;
        }
        resettingText.faceColor = start;
        
    }


    public void NodeMake(Vector2 start, string spriteName)
    {
        if (!IsResetting)
        {
            GameObject newNode = Instantiate(node, start, node.transform.rotation, transform);
            Vector2 end;
            float seconds = UnityEngine.Random.Range(0.5f, 0.8f);

            switch (spriteName)
            {
                case "Circle": //Green Cube: Health Up
                    end = new Vector2(5, -1.5f);
                    break;

                case "Triangle": //Donut: Time Stop
                    int choose = UnityEngine.Random.Range(0, 10);
                    if (choose > 5)
                    {
                        end = new Vector2(3.25f, -2.05f);
                    }
                    else 
                    {
                        end = new Vector2(-3.25f, -2.05f);
                    }  
                    break;

                case "9-Sliced": //Silver Sphere: MinionAttack
                    end = new Vector2(5, 1);
                    break;

                case "Hexagon Pointed-Top": //BlueDode: AttackStall
                    end = new Vector2(-5, -1.5f);
                    break;

                case "Hexagon Flat-Top": //Brown Dode: RobotAttack
                    end = new Vector2(-5, 1);
                    break;

                default:
                    end = new Vector2(10, 10);
                    break;
            }

            if (runningMission && mission == spriteName)
            {
                end = new Vector2(8.4f, 7.05f);
            }
            newNode.GetComponent<Node>().moveHere(end, seconds, spriteName);
        }
    }

    public void EnemyTakeDmg()
    {
        enemyHealth -= playerDamage;
    }

    public void PlayerTakeDmg()
    {
        StartCoroutine(transform.GetChild(2).gameObject.GetComponent<PlayerHealthBar>().TakeDamage(enemyDamage));
    }

    void MissionMake()
    {
        if (!transform.GetChild(3).gameObject.GetComponent<Mission>().onMission)
        {
            missionGoalCount = 10 + (gameTimer / 30) * 5;
            int choose = UnityEngine.Random.Range(1, 4);
            switch (choose)
            {
                case 1: //Green Cube: Health Up
                    mission = "Circle";
                    break;

                case 2: //Donut: Time Stop
                    mission = "Triangle";
                    break;

                case 3: //Brown Dode
                    mission = "Hexagon Flat-Top";
                    break;

                case 4: //BlueDode: AttackStall
                    mission = "Hexagon Pointed-Top";
                    break;

                default:
                    return;
            }

            transform.GetChild(3).gameObject.GetComponent<Mission>().MissionSet(mission, missionGoalCount);
            runningMission = true;
        }
    }

    public void MissionSucceeded()
    {
        for (int i = 0; i < missionGoalCount * 2; i++)
        {
            GameObject newNode = Instantiate(node, new Vector2(8.4f, 7.05f), node.transform.rotation, transform);
            Vector2 end;
            float seconds = UnityEngine.Random.Range(0.1f, 3f);
            switch (mission)
            {
                case "Circle": //Green Cube: Health Up
                    end = new Vector2(5, -1.5f);
                    break;

                case "Triangle": //Donut: Time Stop
                    int choose = UnityEngine.Random.Range(0, 10);
                    if (choose > 5)
                    {
                        end = new Vector2(3.25f, -2.05f);
                    }
                    else
                    {
                        end = new Vector2(-3.25f, -2.05f);
                    }
                    break;

                case "9-Sliced": //Silver Sphere: MinionAttack
                    end = new Vector2(5, 1);
                    break;

                case "Hexagon Pointed-Top": //BlueDode: AttackStall
                    end = new Vector2(-5, -1.5f);
                    break;

                case "Hexagon Flat-Top": //Brown Dode: RobotAttack
                    end = new Vector2(-5, 1);
                    break;

                default:
                    end = new Vector2(10, 10);
                    break;
            }

            newNode.GetComponent<Node>().moveHere(end, seconds, mission);
        }
        runningMission = false;
        mission = "";
    }
}
