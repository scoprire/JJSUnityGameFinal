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
        renderer.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
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
        switch (nodeTag)
        {
            case "Circle":
                transform.GetChild(0).gameObject.SetActive(true);
                break;

            case "Triangle":
                transform.GetChild(1).gameObject.SetActive(true);
                break;

            case "9-Sliced":
                transform.GetChild(2).gameObject.SetActive(true);
                break;

            case "Hexagon Pointed-Top":
                transform.GetChild(3).gameObject.SetActive(true);
                break;

            case "Hexagon Flat-Top":
                transform.GetChild(4).gameObject.SetActive(true);
                break;

            default:
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
                transform.GetChild(4).gameObject.SetActive(false);
                break;
        }
        Debug.Log(nodeTag);
        StartCoroutine(GoToPos(transform.position, end, seconds));
    }


}
