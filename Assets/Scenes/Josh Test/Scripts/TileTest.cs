using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TileTest : MonoBehaviour
{
    private new SpriteRenderer renderer;

    private static Color startColor; //color when starting
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f); //color when selected
    private static TileTest previousSelected = null;

    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>(); //set renderer 
        startColor = renderer.color; //sets startColor to starting color
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
                    previousSelected.Deselect();
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }


}