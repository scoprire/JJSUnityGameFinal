using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Tile : MonoBehaviour
{
    private new SpriteRenderer renderer;

    private static Color startColor; //color when starting
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f); //color when selected
    private static Tile previousSelected = null;

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
        previousSelected = gameObject.GetComponent<Tile>();
    }

    private void Deselect()
    {
        isSelected = false;
        renderer.color = startColor;
        previousSelected = null;
    }

    void OnMouseDown()
    {
        
        if (renderer.sprite == null || BoardManager.instance.IsShifting) //makes sure not currently swapping
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
                //SwapSprite(previousSelected.renderer);
                //previousSelected.Deselect();

                
                Debug.Log(previousSelected.ToString());
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                {
                    Debug.Log("Swapped");
                    SwapSprite(previousSelected.renderer); 
                    previousSelected.Deselect();
                }
                else
                { 
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
                
            }

        }
        Debug.Log(isSelected);
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
            Debug.Log("Hit");
            Debug.Log(hit.collider.gameObject.GetInstanceID());
            Debug.Log(hit.collider.gameObject.transform.position.x);
            Debug.Log(hit.collider.gameObject.transform.position.y);
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        /*for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }*/
        adjacentTiles.Add(GetAdjacent(Vector2.up));
        adjacentTiles.Add(GetAdjacent(Vector2.down));
        adjacentTiles.Add(GetAdjacent(Vector2.left));
        adjacentTiles.Add(GetAdjacent(Vector2.right));
        return adjacentTiles;
    }
    

}
