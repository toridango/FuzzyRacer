using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



/**
 * 
 * Implementation for Fuzzy Logic-Based Movement 
 * 
 **/
public class FuzzyRacerBrain : MonoBehaviour
{

    public double m_speedModifier = 10.0D;
    private double m_currentSpeed = 0.0D;

    // Distance
    FuzzyVariable fv_dist = new FuzzyVariable("distance");
    FuzzyVariable fv_hspeed = new FuzzyVariable("horizontal_Speed");
    FuzzyVariable fv_incspeed = new FuzzyVariable("increase_Speed");
    GameObject line;

    // Use this for initialization
    void Start()
    {

        line = GameObject.Find("RaceLine");

        // Distance
        fv_dist.AddMembershipFunction(new DecreasingGradeMF("left", -0.3D, 0.0D, -0.5D));
        fv_dist.AddMembershipFunction(new TrapezoidMF("middle", -0.15D, -0.05D, 0.05D, 0.15D));
        fv_dist.AddMembershipFunction(new IncreasingGradeMF("right", 0.0D, 0.3D, 0.5D));
        /*fv_dist.AddMembershipFunction(new DecreasingGradeMF("left", -6.0D, -1.0D, -9.2D));
        fv_dist.AddMembershipFunction(new TrapezoidMF("middle", -3.0D, -1.0D, 1.0D, 3.0D));
        fv_dist.AddMembershipFunction(new IncreasingGradeMF("right", 1.0D, 6.0D, 9.2D));*/


        // Horizontal Speed
        fv_hspeed.AddMembershipFunction(new DecreasingGradeMF("leftwards", -0.5D, 0.0D, -0.5D));
        fv_hspeed.AddMembershipFunction(new TrapezoidMF("still", -0.4D, -0.15D, 0.15D, 0.4));
        fv_hspeed.AddMembershipFunction(new IncreasingGradeMF("rightwards", 0.0D, 0.5D, 0.5D));

        // Increase Speed (increment to be applied to speed)
        /*
        fv_incspeed.AddMembershipFunction(new DecreasingGradeMF("fastleft", -0.5D, -0.3D));
        fv_incspeed.AddMembershipFunction(new TriangleMF("left", -0.5D, -0.1666D, 0.1666D));
        fv_incspeed.AddMembershipFunction(new TrapezoidMF("stay", -0.1666D, -0.05D, 0.05D, 0.1666D));
        fv_incspeed.AddMembershipFunction(new TriangleMF("right", -0.1666D, 0.1666D, 0.5D));
        fv_incspeed.AddMembershipFunction(new IncreasingGradeMF("fastright", 0.3D, 0.5D));
        */
        fv_incspeed.AddMembershipFunction(new DecreasingGradeMF("fastleft", -0.5D, -0.3D, -0.5D));
        fv_incspeed.AddMembershipFunction(new TrapezoidMF("left", -0.5D, 0.4D, -0.1D, 0.1D));
        fv_incspeed.AddMembershipFunction(new TrapezoidMF("stay", -0.1666D, -0.05D, 0.05D, 0.1666D));
        fv_incspeed.AddMembershipFunction(new TrapezoidMF("right", 0.0D, 0.1D, 0.4D, 0.5D));
        fv_incspeed.AddMembershipFunction(new IncreasingGradeMF("fastright", 0.3D, 0.5D, 0.5D));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 line_pos = line.transform.position;

        double allowedWidth = (LineController.horizontalMax - LineController.horizontalMin);
        double dist = (double)(pos.x - line_pos.x);
        double hPos = dist / allowedWidth;

        if (dist > LineController.horizontalMax)
            hPos = 0.5;
        else if (dist < LineController.horizontalMin)
            hPos = -0.5;

        fv_dist.Fuzzify(hPos);
        fv_hspeed.Fuzzify(m_currentSpeed);

        if (hPos > 0.5 || hPos < -0.5)
            Debug.Log(string.Format("dist: {0}   hPos: {1}", dist, hPos));


        //Debug.Log(string.Format("dist left: {0}   dist middle: {1}   dist right: {2}", fv_dist.get("left"), fv_dist.get("middle"), fv_dist.get("right")));
        //Debug.Log(string.Format("s left: {0}   s still: {1}   s right: {2}", fv_hspeed.get("leftwards"), fv_hspeed.get("still"), fv_hspeed.get("rightwards")));


        /*
        double goright = FuzzyEngine.AND(fv_dist.get("left"), FuzzyEngine.NOT(fv_dist.get("middle"))) - 0.5;
        double goleft = FuzzyEngine.AND(fv_dist.get("right"), FuzzyEngine.NOT(fv_dist.get("middle"))) - 0.5;
        double speed = m_speedModifier * (goright - goleft);
        Debug.Log(string.Format("right: {0}   left: {1}   speed: {2}", goright, goleft, speed));
        pos.x += Time.deltaTime * (float)speed;
        transform.position = pos;
        */

        double degreeFastLeft = FuzzyEngine.AND(fv_dist.get("right"), fv_hspeed.get("rightwards"));
        double degreeLeft = FuzzyEngine.OR(
                                        FuzzyEngine.AND(fv_dist.get("right"), fv_hspeed.get("still")),
                                        FuzzyEngine.AND(fv_dist.get("middle"), fv_hspeed.get("rightwards"))
                                        );
        double degreeStay = FuzzyEngine.OR(
                                        FuzzyEngine.AND(fv_dist.get("right"), fv_hspeed.get("leftwards")),
                                        FuzzyEngine.AND(fv_dist.get("middle"), fv_hspeed.get("still")),
                                        FuzzyEngine.AND(fv_dist.get("left"), fv_hspeed.get("rightwards"))
                                        );
        double degreeRight = FuzzyEngine.OR(
                                        FuzzyEngine.AND(fv_dist.get("left"), fv_hspeed.get("still")),
                                        FuzzyEngine.AND(fv_dist.get("middle"), fv_hspeed.get("leftwards"))
                                        );
        double degreeFastRight = FuzzyEngine.AND(fv_dist.get("left"), fv_hspeed.get("leftwards"));


        //Debug.Log(string.Format("degreeFastLeft: {0}   degreeLeft: {1}   degreeStay: {2}   degreeRight: {3}   degreeFastRight: {4}", degreeFastLeft, degreeLeft, degreeStay, degreeRight, degreeFastRight));
        //Debug.Log(string.Format("degreeLeft: {0}   degreeStay: {1}   degreeRight: {2}", degreeLeft, degreeStay, degreeRight));

        List<Point> centroids = new List<Point>();
        if (degreeFastLeft != 0) centroids.Add(fv_incspeed.getMFPolygon("fastleft", degreeFastLeft).getCentroid());
        if (degreeLeft != 0) centroids.Add(fv_incspeed.getMFPolygon("left", degreeLeft).getCentroid());
        if (degreeStay != 0) centroids.Add(fv_incspeed.getMFPolygon("stay", degreeStay).getCentroid());
        if (degreeRight != 0) centroids.Add(fv_incspeed.getMFPolygon("right", degreeRight).getCentroid());
        if (degreeFastRight != 0) centroids.Add(fv_incspeed.getMFPolygon("fastright", degreeFastRight).getCentroid());

        double speedIncrement = FuzzyEngine.CentroidOfCluster(centroids).x;


        Debug.Log(string.Format("Current Speed: {0}   Speed Increment: {1}", m_currentSpeed, speedIncrement));
        //Debug.Log(string.Format("Speed: {0}", speedIncrement));

        //m_currentSpeed += m_speedModifier * speedIncrement;
        m_currentSpeed += speedIncrement;
        pos.x += Time.deltaTime * (float)(m_speedModifier * (m_currentSpeed));

        transform.position = pos;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -2.0f * (float)m_currentSpeed));
    }
}
