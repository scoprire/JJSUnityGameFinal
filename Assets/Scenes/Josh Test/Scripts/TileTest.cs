using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.VFX;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class TileTest : MonoBehaviour
{
    private new SpriteRenderer renderer;
    public new Transform transform;
    public new Rigidbody2D rb;

    public Color startColor; //color when starting
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f); //color when selected
    private static TileTest previousSelected = null;

    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool matchFound = false;

    [SerializeField] private float tileScale = 1f;
    private float swapTime = 0.25f;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>(); //set renderer 
        transform = GetComponent<Transform>(); //set transform
        rb = GetComponent<Rigidbody2D>(); //set rigidbody
        startColor = renderer.color; //sets startColor to starting color
        transform.localScale = new Vector3(tileScale, tileScale, tileScale);
        renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (renderer.sprite != null)
        {
            switch (renderer.sprite.name)
            {
                case "Circle":
                    transform.GetChild(0).gameObject.SetActive(true);
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(2).gameObject.SetActive(false);
                    transform.GetChild(3).gameObject.SetActive(false);
                    transform.GetChild(4).gameObject.SetActive(false);
                    break;

                case "Triangle":
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(true);
                    transform.GetChild(2).gameObject.SetActive(false);
                    transform.GetChild(3).gameObject.SetActive(false);
                    transform.GetChild(4).gameObject.SetActive(false);
                    break;

                case "9-Sliced":
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(2).gameObject.SetActive(true);
                    transform.GetChild(3).gameObject.SetActive(false);
                    transform.GetChild(4).gameObject.SetActive(false);
                    break;

                case "Hexagon Pointed-Top":
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(2).gameObject.SetActive(false);
                    transform.GetChild(3).gameObject.SetActive(true);
                    transform.GetChild(4).gameObject.SetActive(false);
                    break;

                case "Hexagon Flat-Top":
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(2).gameObject.SetActive(false);
                    transform.GetChild(3).gameObject.SetActive(false);
                    transform.GetChild(4).gameObject.SetActive(true);
                    break;

                default:
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(2).gameObject.SetActive(false);
                    transform.GetChild(3).gameObject.SetActive(false);
                    transform.GetChild(4).gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void Select()
    {
        isSelected = true;
        renderer.color = selectedColor;
        transform.GetChild(5).gameObject.SetActive(true);
        previousSelected = gameObject.GetComponent<TileTest>();
    }

    private void Deselect()
    {
        isSelected = false;
        renderer.color = startColor;
        transform.GetChild(5).gameObject.SetActive(false);
        previousSelected = null;
    }

    void OnMouseDown()
    {
        if (!(renderer.sprite == null || BoardManagerTest.instance.IsShifting || BoardManagerTest.instance.IsSwapping || BoardManagerTest.instance.IsResetting)) //makes sure not currently swapping, shifting or touching a null element
        {
            StartCoroutine(Choose());
        }
    }

    IEnumerator Choose()
    {
        if (isSelected) //is sprite selected
        {
            Deselect();
        }
        else
        {
            if (previousSelected == null) //is there a tile that is already selected
            {
                Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                {
                    BoardManagerTest.instance.IsSwapping = true;
                    yield return (SwapSprite(previousSelected));
                    if (previousSelected.CheckMatches() || CheckMatches())
                    {
                        previousSelected.ClearAllMatches(); //looks for matches of previousSelected
                        previousSelected.Deselect();
                        ClearAllMatches(); //looks for matches of current
                        BoardManagerTest.instance.IsSwapping = false;
                    }
                    else 
                    {
                        yield return (SwapSprite(previousSelected));
                        previousSelected.Deselect();
                        BoardManagerTest.instance.IsSwapping = false;
                    }

                }
                else
                {
                    previousSelected.GetComponent<TileTest>().Deselect();
                    Select();
                }


            }

        }
        yield return null;
    }

    IEnumerator SwapSprite(TileTest pTile)
    {
        pTile.transform.GetChild(5).gameObject.SetActive(false); //clears select() color without clearing select

        Vector2 start = transform.position;
        Vector2 end = pTile.transform.position;
        Vector2 mid = Vector2.zero;
        float xDif = start.x - end.x;
        float yDif = start.y - end.y;
        if (xDif != 0)
        {
            if (xDif < 0)
            {
                mid = new Vector2(end.x + (xDif / 2), end.y);
            }
            else
            {
                mid = new Vector2(start.x - (xDif / 2), start.y);
            }
        }
        else if (yDif != 0)
        {
            if (yDif < 0)
            {
                mid = new Vector2(end.x, end.y + (yDif / 2));
            }
            else
            {
                mid = new Vector2(start.x, start.y - (yDif / 2));
            }
        }

        for (float t = 0f; t < 0.5f; t += Time.deltaTime / (swapTime ) ) //moves sprites to the midpoint of both
        {
            pTile.transform.position = Vector2.MoveTowards(end, start, t);
            transform.position = Vector2.MoveTowards(start, end, t);
            yield return null;
        }

        
        Sprite temp = pTile.renderer.sprite;
        pTile.renderer.sprite = renderer.sprite; //swaps sprites with each other
        renderer.sprite = temp;


        for (float t = 0f; t < 1f; t += Time.deltaTime / (swapTime )) //moves sprites back to their original position with new sprite render
        {
            pTile.transform.position = Vector2.MoveTowards(mid, end, t);
            transform.position = Vector2.MoveTowards(mid, start, t);
            yield return null;
        }
    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); //raycasts in direction
        if (hit.collider != null)
        {
            return hit.collider.gameObject; //return first hit object
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>(); 
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i])); //adds all adjacent (up, down, left, right) tiles to list
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    { 
        List<GameObject> matchingTiles = new List<GameObject>(); 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); 
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == renderer.sprite) //repeats sending a raycast if sprite is the same
        { 
            matchingTiles.Add(hit.collider.gameObject); //adds matching tile to list
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir); //raycasts again
        }
        return matchingTiles; //returns list of matching tiles
    }

    private void ClearMatch(Vector2[] paths) 
    {
        List<GameObject> matchingTiles = new List<GameObject>(); 
        for (int i = 0; i < paths.Length; i++) 
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }


        if (matchingTiles.Count >= 2) //checks how many matching tiles
        {
            int j = 1;
            if (matchingTiles.Count == 3)
            {
                j = 2;
            }
            if (matchingTiles.Count >= 4)
            {
                j = 3;
            }

            for (int i = 0; i < matchingTiles.Count; i++) 
            {
                for (int t = 0; t < j; t++)
                {
                    BoardManagerTest.instance.NodeMake(matchingTiles[i].GetComponent<Transform>().position, matchingTiles[i].GetComponent<SpriteRenderer>().sprite.name);
                }
                matchingTiles[i].transform.GetChild(0).gameObject.SetActive(false);
                matchingTiles[i].transform.GetChild(1).gameObject.SetActive(false);
                matchingTiles[i].transform.GetChild(2).gameObject.SetActive(false);
                matchingTiles[i].transform.GetChild(3).gameObject.SetActive(false);
                matchingTiles[i].transform.GetChild(4).gameObject.SetActive(false);
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null; //sets sprite to null (invisible)   
            }
            matchFound = true; //set Match found
        }
    }

    public void ClearAllMatches()
    {
        if (renderer.sprite == null)
            return;


        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right }); //looks for matches horizontally
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down }); //looks for matches vertically
        if (matchFound)
        {

            BoardManagerTest.instance.NodeMake(transform.position, renderer.sprite.name);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
            transform.GetChild(4).gameObject.SetActive(false);
            renderer.sprite = null; //sets started sprite to null
            matchFound = false; //resets matchfound

            StopCoroutine(BoardManagerTest.instance.FindNullTiles()); 
            StartCoroutine(BoardManagerTest.instance.FindNullTiles()); //starts shifting tiles down
        }
    }

    public bool CheckMatches()
    {
        Vector2[] hori = new Vector2[2] { Vector2.left, Vector2.right }; //looks for matches horizontally
        Vector2[] vert = new Vector2[2] { Vector2.up, Vector2.down }; //looks for matches vertically
        List<GameObject> matchingHori = new List<GameObject>();
        List<GameObject> matchingVert = new List<GameObject>();
        for (int i = 0; i < hori.Length; i++)
        {
            matchingHori.AddRange(FindMatch(hori[i]));
        }
        for (int i = 0; i < vert.Length; i++)
        {
            matchingVert.AddRange(FindMatch(vert[i]));
        }

        if (matchingHori.Count >= 2 || matchingVert.Count >= 2) //checks how many matching tiles
        {
            return true;
        }
        return false;
    }

    public bool TestBoard(TileTest other)
    {
        Sprite temp = other.renderer.sprite;
        other.renderer.sprite = renderer.sprite; //swaps sprites with each other
        renderer.sprite = temp;
        if (other.CheckMatches() || CheckMatches())
        {
            return true;
        }
        else
        {
            Sprite temp2 = other.renderer.sprite;
            other.renderer.sprite = renderer.sprite; //swaps sprites with each other
            renderer.sprite = temp2;
            return false;
        }
    }

}

