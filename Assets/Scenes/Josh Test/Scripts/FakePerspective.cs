using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePerspective : MonoBehaviour
{
    new Transform tr;
    Vector3 scaleStart;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        scaleStart = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(scaleStart.x - tr.position.z * 0.01f, scaleStart.y - tr.position.z * 0.01f, scaleStart.z - tr.position.z * 0.01f);
    }
}
