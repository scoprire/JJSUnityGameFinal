using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AttackStall: MonoBehaviour
{
    int stallCount;
    new BoxCollider2D col;
    public GameObject emp;
     
    public TextMeshProUGUI stallText;

    float atkBarX = -8.4f;
    float atkBatheight = 0;

    bool spawning = false;

    int stallNeeded = 4;
    int stuns = 3;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        stallText.text = stallCount + System.Environment.NewLine + "Stall Attack" + System.Environment.NewLine + "Cost: " + stallNeeded;
    }

    private void OnMouseDown()
    {
        if (stallCount >= stallNeeded)
        {
            stallCount -= stallNeeded;
            for (int i = 0; i < stuns; i++)
            {
                GameObject newNode = Instantiate(emp, new Vector3(transform.position.x, transform.position.y, transform.position.z - 2f), emp.transform.rotation, transform);
                Vector2 end = new Vector2(atkBarX, Random.Range(-2f, 1.5f));
                float seconds = Random.Range(0.5f, 0.8f);

                newNode.GetComponent<Node>().moveHere(end, seconds, this.gameObject.tag);
                stallCount--;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == this.gameObject.tag && !spawning)
        {
            stallCount++;
        }
    }
}
