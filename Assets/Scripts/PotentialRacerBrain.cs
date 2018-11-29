using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/**
 * 
 * Implementation for Potential Function-Based Movement 
 * 
 **/
public class PotentialRacerBrain : MonoBehaviour
{
    /**
    * Potential:
    * 
    * U = -A/r^n  +  B/r^m
    * 
    * where A, B, n and m are parameters chosen for a realistic model of attraction and repulsion
    * 
    * 
    * Force:
    * 
    * F = -dU/dr = -nA/r^n+1  +  mB/r^m+1
    * 
    **/
    public float A;
    public float B;
    public float n;
    public float m;

    private GameObject m_raceline;

    // Use this for initialization
    void Start()
    {
        m_raceline = GameObject.Find("RaceLine");

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 line_pos = m_raceline.transform.position;
        Vector3 pos = transform.position;

        // Distance vector and unit direction vector
        Vector3 r = pos - line_pos;
        Vector2 u = r.normalized;

        float dist = r.magnitude;
        if (dist > 1E-8)
        {
            Vector3 force = u * ((n * -A) / Mathf.Pow(dist, n + 1) + (m * B) / Mathf.Pow(dist, m + 1));

            // Mass is considered 1 if force is not divided by anything
            float xspeed = Time.deltaTime * force.x;
            pos.x += Time.deltaTime * xspeed + 0.5f * force.x * Mathf.Pow(Time.deltaTime, 2);

            Debug.Log(String.Format("fx: {0,8}  speed: {1,8}  pos.x: {2,8}", force.x, xspeed, pos.x));

            transform.position = pos;

            // animate apparent steering

            // This for car-like (around z) steering
            // transform.rotation = Quaternion.Euler(-180.0f, 0.0f, xspeed);

            // This for plane-like (around y) steering
            //transform.rotation = Quaternion.Euler(-180.0f, -2.0f * xspeed, 0.0f);

            // This for both (hardcoded multiplication/division used to exaggerate/attenuate)
            transform.rotation = Quaternion.Euler(-180.0f, -2.0f * xspeed, xspeed / 2.0f);

        }

    }
}
