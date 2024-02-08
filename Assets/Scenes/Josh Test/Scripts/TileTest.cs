using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TileTest : MonoBehaviour
{
    private new SpriteRenderer renderer;
    private new Transform transform;

    private static Color startColor; //color when starting
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f); //color when selected
    private static TileTest previousSelected = null;

    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool matchFound = false;

    public float tileScale = 1f;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>(); //set renderer 
        transform = GetComponent<Transform>(); //set transform
        startColor = renderer.color; //sets startColor to starting color
        transform.localScale = new Vector3(tileScale, tileScale, tileScale);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Select()
    {
        isSelected = true;
        renderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<TileTest>();
    }

    private void Deselect()
    {
        isSelected = false;
        renderer.color = startColor;
        previousSelected = null;
    }

    void OnMouseDown()
    {

        if (renderer.sprite == null || BoardManagerTest.instance.IsShifting) //makes sure not currently swapping
        {
            return;
        }

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
                    SwapSprite(previousSelected.renderer);

                    previousSelected.ClearAllMatches(); //looks for matches of previousSelected

                    previousSelected.Deselect();

                    ClearAllMatches(); //looks for matches of current
                }
                else
                {
                    previousSelected.GetComponent<TileTest>().Deselect();
                    Select();
                }

            }

        }
    }

    public void SwapSprite(SpriteRenderer render2)
    {
        if (renderer.sprite == render2.sprite)
        {
            return;
        }

        Sprite tempSprite = render2.sprite; //sets targetsprite to temp
        render2.sprite = renderer.sprite; //changes targetsprite to selected sprite
        renderer.sprite = tempSprite; //changes selected sprite to temp
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
            for (int i = 0; i < matchingTiles.Count; i++) 
            {
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
            renderer.sprite = null; //sets started sprite to null
            matchFound = false; //resets matchfound

            StopCoroutine(BoardManagerTest.instance.FindNullTiles()); 
            StartCoroutine(BoardManagerTest.instance.FindNullTiles()); //starts shifting tiles down
        }
    }




}
