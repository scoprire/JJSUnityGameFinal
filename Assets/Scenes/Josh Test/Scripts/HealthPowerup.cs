using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HealthPowerup: MonoBehaviour
{
    int healthCount;
    public new BoxCollider2D col;

    public TextMeshProUGUI healthText;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = healthCount + System.Environment.NewLine + "Health Up";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit");
        if (other.gameObject.tag == this.gameObject.tag)
        {
            healthCount++;
        }

    }
}
