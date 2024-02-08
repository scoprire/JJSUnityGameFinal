using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BoardManagerTest : MonoBehaviour
{
    public static BoardManagerTest instance;
    public List<Sprite> resources = new List<Sprite>(); //sprites of resources
    public GameObject tile;  //prefab of tile 
    public int xSize, ySize; //size of board (set in Unity)

    public GameObject[,] tiles; //tiles in board as an 2D array

    public bool IsShifting { get; set; } //checks if it is swapping

    private float border = 0.1f; //border between sprites

    void Start()
    {
        instance = GetComponent<BoardManagerTest>();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x + border, offset.y + border);
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

        Sprite[] previousLeft = new Sprite[ySize]; //set previous pieces
        Sprite previousBelow = null;


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
}
