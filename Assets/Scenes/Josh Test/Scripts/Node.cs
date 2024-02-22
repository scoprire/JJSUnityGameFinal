using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private new Transform transform;
    private new Rigidbody2D rb;
    private new CircleCollider2D col;
    private new SpriteRenderer renderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = true;
        //StartCoroutine(GoToPos(transform.position, new Vector2(3,3)));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator GoToPos(Vector2 start, Vector2 end, float seconds)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / seconds) 
        {
            transform.position = Vector2.Lerp(start, end, t);
            yield return null;
        }
        renderer.enabled = false;
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }

    public void moveHere(Vector2 end, float seconds = 0.5f, string nodeTag = "Resource")
    {
        gameObject.tag = nodeTag;
        StartCoroutine(GoToPos(transform.position, end, seconds));
    }


}
