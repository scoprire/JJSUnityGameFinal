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
    int donutsCount = 0;
    new BoxCollider2D col;
    public GameObject donuts;

    public TextMeshProUGUI donutsLText;
    public TextMeshProUGUI donutsRText;
    public TextMeshProUGUI donutsMText;

    float atkBarX = -8.4f;
    float atkBatheight = 0;

    bool spawning = false;

    int donutsNeeded = 30;
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
            StartCoroutine(DonutExplosion());
        }

    }

    IEnumerator DonutExplosion()
    {
        GameObject newNode1 = Instantiate(donuts, new Vector3(transform.position.x, transform.position.y, transform.position.z - 2f), donuts.transform.rotation, transform);
        Vector2 end = new Vector2(11, 2);
        float seconds = 2f;
        newNode1.GetComponent<Node>().moveHere(end, seconds, this.gameObject.tag);
        donutsCount--;

        GameObject newNode2 = Instantiate(donuts, new Vector3(-transform.position.x, transform.position.y, transform.position.z - 2f), donuts.transform.rotation, transform);
        end = new Vector2(-11, 2);
        newNode2.GetComponent<Node>().moveHere(end, seconds, this.gameObject.tag);
        donutsCount--;

        yield return new WaitForSeconds(seconds + 0.5f);

        for (int i = 0; i < 30; i++)
        {
            Vector2 enemy = new Vector2(0, 4.2f);
            seconds = Random.Range(0.5f, 2f);
            float yRandom = Random.Range(1f, 5f);
            if (i % 2 == 0)
            {
                GameObject nodeSwarmLeft = Instantiate(donuts, new Vector3(-11, yRandom, transform.position.z - 2f), donuts.transform.rotation, transform);
                nodeSwarmLeft.GetComponent<Node>().moveHere(enemy, seconds, this.gameObject.tag);

            }
            else 
            {
                GameObject nodeSwarmRight = Instantiate(donuts, new Vector3(11, yRandom, transform.position.z - 2f), donuts.transform.rotation, transform);
                nodeSwarmRight.GetComponent<Node>().moveHere(enemy, seconds, this.gameObject.tag);
            }
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(1f/30f);
            BoardManagerTest.instance.EnemyTakeDmg(1);
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
