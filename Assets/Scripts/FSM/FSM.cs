using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum STATE
{
    MaintainCourse,
    MoveRight,
    MoveLeft,
    FlipSpeed
}


public class FSM
{

    private List<STATE> m_stateStack;

    public FSM()
    {
        m_stateStack = new List<STATE>();
        m_stateStack.Add(STATE.MaintainCourse);
    }

    public void updateState(float distance, float hspeed)
    {
        STATE currentState = m_stateStack.Last();
        switch (currentState)
        {
            case STATE.MaintainCourse:
                // Is right and not going left
                if (distance > 0 && hspeed >= 0) m_stateStack.Add(STATE.MoveLeft);
                // Is left and not going right
                else if (distance < 0 && hspeed <= 0) m_stateStack.Add(STATE.MoveRight);
                break;
            case STATE.MoveRight:
                // Reached line and is moving
                if (distance == 0 && hspeed != 0) m_stateStack.Add(STATE.MaintainCourse);
                // Is right and has to suddenly go left
                else if (distance > 0 && hspeed > 0) m_stateStack.Add(STATE.FlipSpeed);
                break;
            case STATE.MoveLeft:
                // Reached line and is moving
                if (distance == 0 && hspeed != 0) m_stateStack.Add(STATE.MaintainCourse);
                // Is left and has to suddenly go right
                else if (distance < 0 && hspeed < 0) m_stateStack.Add(STATE.FlipSpeed);
                break;
            case STATE.FlipSpeed: 
                // Created Flip speed to avoid the "vibrations" that occured when suddenly changing the position of the line 
                // (due to the speed suddenly being in the wrong direction)

                // If it came from moving right, go to move left
                if (m_stateStack[m_stateStack.Count - 2] == STATE.MoveLeft) m_stateStack.Add(STATE.MoveRight);
                // If it came from moving left, go to move right
                else if (m_stateStack[m_stateStack.Count - 2] == STATE.MoveRight) m_stateStack.Add(STATE.MoveLeft);
                break;
            default:
                // This shouldn't happen
                m_stateStack.Add(STATE.MaintainCourse);
                break;
        }
        // Debug.Log(string.Format("m_currentState: {0, 5} after distance: {1, 5} and hspeed: {2,5}", currentState, distance, hspeed));
    }

    public float currentOutput(float distance, float hspeed)
    {
        float speed = 0.0f;
        STATE currentState = m_stateStack.Last();
        switch (currentState)
        {
            case STATE.MaintainCourse:
                break;
            case STATE.MoveRight:
                speed = -(5.0f + hspeed / 10.0f) * distance;
                break;
            case STATE.MoveLeft:
                speed = -(5.0f + hspeed / 10.0f) * distance;
                break;
            case STATE.FlipSpeed:
                speed = -hspeed;
                break;
            default:
                break;
        }
        //Debug.Log(string.Format("speed: {0, 5} after distance: {1, 5} and hspeed: {2,5}", speed, distance, hspeed));

        return speed;
    }


}

