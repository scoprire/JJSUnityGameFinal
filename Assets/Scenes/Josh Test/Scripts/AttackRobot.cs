using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AttackRobot : MonoBehaviour
{
    int ammoCount;
    new BoxCollider2D col;
    public GameObject ammo;

    public TextMeshProUGUI ammoText;

    float enemies = 5f;

    bool spawning = false;

    int ammoNeeded = 4;
    int bullets = 3;


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ammoText.text = ammoCount + System.Environment.NewLine + "Attack Robot" + System.Environment.NewLine + "Cost: " + ammoNeeded;
    }

    private void OnMouseDown()
    {
        if (ammoCount >= ammoNeeded)
        {
            ammoCount -= ammoNeeded;
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        for (int i = 0; i < bullets; i++)
        {
            GameObject newNode = Instantiate(ammo, new Vector3(transform.position.x, transform.position.y, transform.position.z - 2f), ammo.transform.rotation, transform);
            Vector2 end = new Vector2(0, 4.2f);
            float seconds = Random.Range(0.5f, 1f);

            newNode.GetComponent<Node>().moveHere(end, seconds, this.gameObject.tag);
            ammoCount--;
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < bullets; i++)
        {
            yield return new WaitForSeconds(1f / bullets);
            BoardManagerTest.instance.EnemyTakeDmg(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == this.gameObject.tag && !spawning)
        {
            ammoCount++;
        }
    }
}
