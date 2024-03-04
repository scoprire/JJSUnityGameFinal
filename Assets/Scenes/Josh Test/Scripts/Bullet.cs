using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private new Transform transform;
    private new SpriteRenderer renderer;
    private new CircleCollider2D col;
    private int count;
    private Vector2 startPoint;
    private bool moving = false;

    float scaleStart = 0.5f;
    float scaleInc = 0.3f;
    public int countToShoot;
    // Start is called before the first frame update
    void Awake()
    {
        transform = GetComponent<Transform>();
        renderer = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();
        count = 0;
        startPoint = transform.position;
        renderer.enabled = false;
        transform.localScale = new Vector3(scaleStart, scaleStart, scaleStart);

        countToShoot = 3;

        if (countToShoot > 1)
        {
            scaleInc = (scaleInc / (float)(countToShoot - 1));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator GoToPos(Vector2 start, Vector2 end, float seconds = 1f)
    {
        moving = true;
        for (float t = 0; t < 1; t += Time.deltaTime / seconds)
        {
            transform.position = Vector2.Lerp(start, end, t);
            Debug.Log("moving");
            yield return null;
        }
        renderer.enabled = false;
        transform.position = startPoint;
        count = 0;
        moving = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit");
        if (other.gameObject.tag == this.gameObject.tag && !moving)
        {
            renderer.enabled = true;
            count++;
            float change = scaleStart + ((count - 1) * scaleInc);
            transform.localScale = new Vector3(change, change, change);
            if (count == countToShoot && !moving)
            {
                StartCoroutine(GoToPos(startPoint, new Vector2(7, -2)));
            }
        }

    }

}
