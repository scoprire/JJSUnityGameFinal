using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.VFX;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class TileCheck : MonoBehaviour
{
    private new SpriteRenderer renderer;
    public new Transform transform;
    public new Rigidbody2D rb;

    public Color startColor; //color when starting

    [SerializeField] private float tileScale = 1f;

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

    public bool TestBoard(TileCheck other)
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

