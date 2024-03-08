using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackBar : MonoBehaviour
{
    new Transform transform;
    int countToAtk = 20;
    int currentCount = 0;
    float timer = 0f;
    float timeToCount = 1.5f;

    Vector3 start;

    bool stunned;
    int stunnedTimer = -1;
    // Start is called before the first frame update
    void Awake()
    {
        transform = GetComponent<Transform>();
        for (int i = 0; i < countToAtk; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(24).gameObject.SetActive(false);
        start = transform.position;
        stunned = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (stunnedTimer >= 0)
        {
            transform.GetChild(24).gameObject.SetActive(true);
        }
        else 
        {
            transform.GetChild(24).gameObject.SetActive(false);
        }

        if (timer > timeToCount)
        {
            if (!stunned)
            {
                currentCount++;
                stunnedTimer--;
                UpdateBar();
            }
            timer -= timeToCount;
        }
        
    }

    void UpdateBar()
    {
        if (!stunned)
        {
            if (currentCount == countToAtk)
            {
                currentCount = 0;
                return;
            }

            Color barColor;
            if (currentCount > 15)
            {
                barColor = Color.red;
                for (int i = 20; i < 24; i++)
                {
                    transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = barColor;
                }
            }
            else if (currentCount > 9)
            {
                barColor = Color.yellow;
                for (int i = 20; i < 24; i++)
                {
                    transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = barColor;
                }
            }
            else
            {
                barColor = Color.white;
                for (int i = 20; i < 24; i++)
                {
                    transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = barColor;
                }
            }

            for (int i = 0; i < currentCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = barColor;
            }



            for (int i = countToAtk - 1; i >= currentCount; i--)
            {
                transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = barColor;
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator Shake()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i % 2 == 0)
            {
                transform.position = new Vector3(start.x - 0.15f, start.y, start.z);
            }
            else
            {
                transform.position = new Vector3(start.x + 0.15f, start.y, start.z);
            }
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = start;
    }

    private IEnumerator Stunned()
    {
        currentCount = 0;
        stunned = true;

        for (int i = 0; i < 24; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = Color.gray;
        }
        yield return new WaitForSeconds(10f);
        stunned = false;
        stunnedTimer = 20;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == this.gameObject.tag && !stunned)
        {
            if (currentCount - 1 >= 0)
            {
                currentCount--;
                UpdateBar();
                StartCoroutine(Shake());
            }
            else 
            {
                if (stunnedTimer < 0)
                {
                    StartCoroutine(Stunned());
                }
                else 
                {
                    currentCount = 0;
                }

            }
            
        }
    }
}
