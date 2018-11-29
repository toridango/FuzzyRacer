using System.Collections;
using System.Collections.Generic;
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

        fsm.updateState(distance, hspeed);
        hspeed = fsm.currentOutput(distance, hspeed);

        pos.x += Time.deltaTime * hspeed;
        transform.position = pos;
    }
}
