using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    new Transform transform;
    Slider healthBar;
    [SerializeField] int healthBarValue = 50;
    int playerMaxHealth = 100;
    Color flashColor;
    bool isFlashing;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = playerMaxHealth;
        flashColor = transform.GetChild(1).GetComponent<SpriteRenderer>().color;
        isFlashing = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == this.gameObject.tag)
        {
            if (healthBarValue > 100)
            {
                healthBarValue = 100;
            }
            else
            {
                healthBarValue += 5;
                if (!isFlashing)
                {
                    StopCoroutine(FlashHealth());
                    StartCoroutine(FlashHealth());
                }
            }
        }
    }
    
    private IEnumerator FlashHealth()
    {
        isFlashing = true;
        Color end = new Color(flashColor.r, flashColor.g, flashColor.b, 1f);

        for (float t = 0f; t < 1f; t += Time.deltaTime / 0.5f)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.Lerp(flashColor, end, t);
            yield return null;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().color = end;
        healthBar.value = healthBarValue;

        for (float t = 0f; t < 1f; t += Time.deltaTime / 0.5f)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.Lerp(end, flashColor, t);
            yield return null;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().color = flashColor;

        isFlashing = false;
    }
    
}
