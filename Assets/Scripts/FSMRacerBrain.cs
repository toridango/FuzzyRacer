using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FSMRacerBrain : MonoBehaviour
{

    public GameObject m_raceline;


    private FSM fsm;
    private float hspeed;


    // Use this for initialization
    void Start()
    {
        if (!m_raceline) m_raceline = GameObject.Find("RaceLine");
        hspeed = 0.0f;
        fsm = new FSM();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        float distance = pos.x - m_raceline.transform.position.x;

        // Update FSM
        fsm.updateState(distance, hspeed);

        // Get action that corresponds to the current state
        hspeed = fsm.currentOutput(distance, hspeed);

        Debug.Log(String.Format("speed: {0,8}", hspeed));

        pos.x += Time.deltaTime * hspeed;
        transform.position = pos;
    }
}
