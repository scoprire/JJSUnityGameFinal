using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    new Transform transform;
    Slider healthBar;
    int healthBarValue;
    int playerMaxHealth = 100;
    Color healthBarStart;
    Color flashColorHeal;
    Color flashColorDmg;
    bool isFlashing;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = playerMaxHealth;
        healthBarValue = playerMaxHealth;
        healthBarStart = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        flashColorHeal = transform.GetChild(1).GetComponent<SpriteRenderer>().color;
        flashColorDmg = Color.red;
        flashColorDmg = new Color(flashColorDmg.r, flashColorDmg.g, flashColorDmg.b, 0f);
        isFlashing = false;
        StartCoroutine(FlashHealth());
    }

    // Update is called once per frame
    void Update()
    {
        BoardManagerTest.instance.currentPlayerHealth = healthBarValue;
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
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = flashColorHeal;
        isFlashing = true;
        Color end = new Color(flashColorHeal.r, flashColorHeal.g, flashColorHeal.b, 1f);

        for (float t = 0f; t < 1f; t += Time.deltaTime / 0.25f)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.Lerp(flashColorHeal, end, t);
            yield return null;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().color = end;
        healthBar.value = healthBarValue;

        for (float t = 0f; t < 1f; t += Time.deltaTime / 0.25f)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.Lerp(end, flashColorHeal, t);
            yield return null;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().color = flashColorHeal;

        isFlashing = false;
    }

    public IEnumerator TakeDamage(int damage)
    {
        Debug.Log("Recieved function");
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = flashColorDmg;
        isFlashing = true;
        Color endshow = new Color(flashColorDmg.r, flashColorDmg.g, flashColorDmg.b, 1f);
        Color endhide = new Color(healthBarStart.r, healthBarStart.g, healthBarStart.b, 0f);

        for (float t = 0f; t < 1f; t += Time.deltaTime / 0.25f)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.Lerp(flashColorDmg, endshow, t);
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.Lerp(healthBarStart, endhide, t);
            yield return null;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().color = endshow;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = endhide;
        healthBarValue -= damage;
        healthBar.value = healthBarValue;

        for (float t = 0f; t < 1f; t += Time.deltaTime / 0.25f)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.Lerp(endshow, flashColorDmg, t);
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.Lerp(endhide, healthBarStart, t);
            yield return null;
        }

        transform.GetChild(1).GetComponent<SpriteRenderer>().color = flashColorDmg;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = healthBarStart;

        isFlashing = false;
    }

}
