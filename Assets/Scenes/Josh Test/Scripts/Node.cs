using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private new Transform transform;
    private Rigidbody2D rb;

    
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Awake");
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
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
            Debug.Log("moving");
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }

    public void moveHere(Vector2 end, float seconds = 0.5f)
    {
        Debug.Log("function");
        StartCoroutine(GoToPos(transform.position, end, seconds));
    }
}
