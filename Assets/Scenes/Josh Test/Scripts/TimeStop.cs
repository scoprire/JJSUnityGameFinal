using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TimeStop : MonoBehaviour
{
    int donutsCount;
    new BoxCollider2D col;
    public GameObject donuts;

    public TextMeshProUGUI donutsLText;
    public TextMeshProUGUI donutsRText;
    public TextMeshProUGUI donutsMText;

    float atkBarX = -8.4f;
    float atkBatheight = 0;

    bool spawning = false;

    int donutsNeeded = 4;
    int stuns = 3;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        donutsLText.text = donutsCount +  " Donuts";
        donutsRText.text = donutsCount +  " Donuts";
        donutsMText.text =  "Donuts Needed: " + donutsNeeded;
    }

    private void OnMouseDown()
    {
        if (donutsCount >= donutsNeeded)
        {
            donutsCount -= donutsNeeded;
            for (int i = 0; i < stuns; i++)
            {
                GameObject newNode = Instantiate(donuts, new Vector3(transform.position.x, transform.position.y, transform.position.z - 2f), donuts.transform.rotation, transform);
                Vector2 end = new Vector2(atkBarX, Random.Range(-2f, 1.5f));
                float seconds = Random.Range(0.5f, 0.8f);

                newNode.GetComponent<Node>().moveHere(end, seconds, this.gameObject.tag);
                donutsCount--;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == this.gameObject.tag && !spawning)
        {
            donutsCount++;
        }
    }
}
