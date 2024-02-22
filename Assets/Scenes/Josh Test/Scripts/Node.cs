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
        transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(GoToPos(transform.position, new Vector2(3,3)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GoToPos(Vector2 start, Vector2 end, float speed = 0.5f)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / speed) 
        {
            transform.position = Vector2.Lerp(start, end, t);
            Debug.Log("moving");
            yield return null;
        }
    }
    public void moveHere()
    {
        StartCoroutine(GoToPos(transform.position, new Vector2(3, 3)));
    }
}
