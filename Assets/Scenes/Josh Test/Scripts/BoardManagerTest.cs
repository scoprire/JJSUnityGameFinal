using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BoardManagerTest : MonoBehaviour
{
    public static BoardManagerTest instance;
    private Transform BoardTransform;
    public List<Sprite> resources = new List<Sprite>(); //sprites of resources
    public GameObject tile;  //prefab of tile 
    public int xSize, ySize; //size of board (set in Unity)

    public GameObject[,] tiles; //tiles in board as an 2D array

    public bool IsShifting { get; set; } //checks if it is shifting
    public bool IsSwapping { get; set; } //checks if it is swapping

    private float border = 0.1f; //border between sprites

    void Start()
    {
        instance = GetComponent<BoardManagerTest>();
        BoardTransform = GetComponent<Transform>();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.extents; //could be bounds.size

        BoardTransform.position = new Vector3(-((xSize - 1f) * (offset.x + border) * 0.5f), BoardTransform.position.y, 0f); //position board so it is centered based on offset

        CreateBoard(offset.x + border, offset.y + border);
        IsSwapping = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];  //creates board with given size   

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize]; //list of tiles to the left
        Sprite previousBelow = null; //tile right below


        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation); //sets newTile to make Tile prefabs

                tiles[x, y] = newTile; //sets Tile to it's x and y location

                newTile.transform.parent = transform; //parents new tile to board manager

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
    }
    public IEnumerator FindNullTiles()
    {
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
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = 0.2f)
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
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite; //replaces current tile with one above
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1); //creates new tile at the very top
            }
        }

        IsShifting = false; //allows swawpping
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


}
