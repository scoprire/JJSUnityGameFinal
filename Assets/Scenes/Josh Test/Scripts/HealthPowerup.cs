using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class HealthPowerup : MonoBehaviour
{
    int healthCount;
    new BoxCollider2D col;
    public GameObject medkits;

    public TextMeshProUGUI healthText;

    float atkBarX = 8.4f;

    bool spawning = false;

    int healthNeeded = 4;
    int boxes = 3;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = healthCount + System.Environment.NewLine + "Heal Me" + System.Environment.NewLine + "Cost: " + healthNeeded;
    }

    private void OnMouseDown()
    {
        if (healthCount >= healthNeeded)
        {
            healthCount -= healthNeeded;
            for (int i = 0; i < boxes; i++)
            {
                GameObject newNode = Instantiate(medkits, new Vector3(transform.position.x, transform.position.y, transform.position.z - 2f), medkits.transform.rotation, transform);
                Vector2 end = new Vector2(atkBarX, Random.Range(-2f, 1.5f));
                float seconds = Random.Range(0.5f, 0.8f);

                newNode.GetComponent<Node>().moveHere(end, seconds, this.gameObject.tag);
                healthCount--;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == this.gameObject.tag && !spawning)
        {
            healthCount++;
        }
    }
}
