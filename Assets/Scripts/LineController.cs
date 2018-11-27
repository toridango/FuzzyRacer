using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{

    public float movementSpeed = 2f;
    public static float horizontalMax = 9.2f;
    public static float horizontalMin = -9.2f;

    private enum MButtons { LEFT, RIGHT, MIDDLE };


    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if (!Input.GetMouseButton((int) MButtons.LEFT))
        {
            if (Input.GetKey("d"))
            {
                pos.x = Mathf.Min(pos.x + movementSpeed * Time.deltaTime, horizontalMax);
            }
            if (Input.GetKey("a"))
            {
                pos.x = Mathf.Max(pos.x - movementSpeed * Time.deltaTime, horizontalMin);
            }
        }
        else
        {
            pos.x = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, horizontalMin, horizontalMax);
        }


        transform.position = pos;
    }

}
