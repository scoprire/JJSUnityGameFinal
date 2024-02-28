using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

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

    private float border = 0.1f; //border between sprites
    private float shiftDelay = 0.15f;
    public bool bricked = false;

    bool fRunning = false; //checks if coRoutine is running
    bool justChecked = false; //checks if board checks
    public bool IsResetting { get; set; } //if board is resetting

    public Button resetBoard; //button to reset

    private int[] rBullet; //
    private int cBullet;
    [SerializeField] private GameObject[] bullets; //to access bui
    void Start()
    {

        instance = GetComponent<BoardManagerTest>();
        BoardTransform = GetComponent<Transform>();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.extents; //could be bounds.size

        BoardTransform.position = new Vector3(-((xSize - 1f) * (offset.x + border) * 0.5f), BoardTransform.position.y, 0f); //position board so it is centered based on offset

        resetBoard.gameObject.SetActive(false);
        resetBoard.onClick.AddListener(ResetBoard);
        
        IsSwapping = false; //set to false initially
        IsShifting = false;
        IsResetting = false;

        rBullet = new int[5];
        cBullet = 0;
        
        CreateBoard(offset.x + border, offset.y + border);  
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsShifting && !fRunning && !IsSwapping)
        {
            if (!justChecked)
            {
                CheckForBrick();
                Debug.Log("checking");
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
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation); //creates a new tile at a location in game
                GameObject brickTile = Instantiate(tile, new Vector3(startX + (xOffset * x) - 10f, startY + (yOffset * y) - 10f, 0), tile.transform.rotation);
                tiles[x, y] = newTile; //sets Tile to it's respective array location
                brickBoard[x, y] = brickTile;

                newTile.transform.parent = transform; //parents new tile to board manager
                brickTile.transform.parent = transform;

                List<Sprite> possibleResources = new List<Sprite>();
                possibleResources.AddRange(resources); //adds all resources to list

                possibleResources.Remove(previousLeft[y]); //removes any resource to the left
                possibleResources.Remove(previousBelow); //removes resource right below

                Sprite newSprite = possibleResources[Random.Range(0, possibleResources.Count)]; //creates a new sprite from possible resources

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
                //NodeMake(new Vector2(tiles[x, y].GetComponent<Transform>().position.x, tiles[x, y].GetComponent<Transform>().position.y));
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

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //random possible sprite
    }



    private void CheckForBrick()
    {
        justChecked = true;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                brickBoard[x, y].GetComponent<TileTest>().GetComponent<SpriteRenderer>().sprite = tiles[x, y].GetComponent<SpriteRenderer>().sprite;
            }
        }
        
        for (int x = 0; x < xSize - 1; x++)
        {
            for (int y = 0; y < ySize - 1; y++)
            {
                if (brickBoard[x, y].GetComponent<TileTest>().TestBoard(brickBoard[x, y + 1].GetComponent<TileTest>())) //Checks above swap
                {
                    bricked = false;
                    return;
                }
                if (brickBoard[x, y].GetComponent<TileTest>().TestBoard(brickBoard[x + 1, y].GetComponent<TileTest>())) //Checks right swap
                {
                    bricked = false;
                    return;
                }
            }
            if (brickBoard[x, ySize - 1].GetComponent<TileTest>().TestBoard(brickBoard[x + 1, ySize - 1].GetComponent<TileTest>())) //checks horizontal swap whole top row
            {
                bricked = false;
                return;
            }
        }

        for (int y = 0; y < ySize - 1; y++)
        {
            if (brickBoard[xSize - 1, y].GetComponent<TileTest>().TestBoard(brickBoard[xSize - 1, y + 1].GetComponent<TileTest>())) //Checks above swap in last coloumn
            {
                bricked = false;
                return;
            }
        }

        resetBoard.gameObject.SetActive(true);
        bricked = true;
    }

    public void ResetBoard()
    {
        shiftDelay = 0.05f;
        IsResetting = true;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        resetBoard.gameObject.SetActive(false);
        StartCoroutine(BoardManagerTest.instance.FindNullTiles());
    }


    public void NodeMake(Vector2 start, string spriteName)
    {
        if (!IsResetting)
        {
            GameObject newNode = Instantiate(node, start, node.transform.rotation, transform);
            Vector2 end;
            float speed;

            switch (spriteName)
            {
                case "Circle":
                    end = ToBullet();
                    speed = Random.Range(0.5f, 0.8f);
                    break;

                case "Triangle":
                    end = new Vector2(0, 5);
                    speed = Random.Range(0.1f, 0.15f);
                    break;

                case "9-Sliced":
                    end = new Vector2(-5, 5);
                    speed = Random.Range(0.1f, 0.15f);
                    break;

                case "Hexagon Pointed-Top":
                    end = new Vector2(5, 0);
                    speed = Random.Range(0.1f, 0.15f);
                    break;

                case "Hexagon Flat-Top":
                    end = new Vector2(-5, 0);
                    speed = Random.Range(0.1f, 0.15f);
                    break;

                default:
                    end = new Vector2(10, 10);
                    speed = Random.Range(0.1f, 0.15f);
                    break;
            }

            newNode.GetComponent<Node>().moveHere(end, speed, "" + (cBullet + 1));
        }
    }

    private Vector2 ToBullet()
    {
        for (int i = cBullet; i < 5; i++) 
        {
            Debug.Log(bullets[cBullet].GetComponent<Bullet>().countToShoot);
            if (cBullet == 4 && rBullet[cBullet] >= bullets[cBullet].GetComponent<Bullet>().countToShoot)
            {
                cBullet = 0;
                rBullet = new int[5];
                i = 0;
            }
            if (rBullet[i] < bullets[cBullet].GetComponent<Bullet>().countToShoot)
            {
                rBullet[i]++;
                cBullet = i;
                break;
            }
        }

        return bullets[cBullet].GetComponent<Transform>().position;
    }



    public void BulletShoot()
    {
        
    }
}
