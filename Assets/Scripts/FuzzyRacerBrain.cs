using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;



/**
 * 
 * Implementation for Fuzzy Logic-Based Movement 
 * 
 **/
public class FuzzyRacerBrain : MonoBehaviour
{

    public double m_speedModifier = 10.0D;
    private double m_currentSpeed = 0.0D;

    public GameObject line;
    public Text[] m_feedback;

    // Distance
    private FuzzyVariable fv_dist = new FuzzyVariable("distance");
    // Speed
    private FuzzyVariable fv_hspeed = new FuzzyVariable("horizontal_Speed");
    // Output Speed
    private FuzzyVariable fv_incspeed = new FuzzyVariable("increase_Speed");

    private List<String> m_fdbText;
    private List<String> m_outText;

    // Use this for initialization
    void Start()
    {
        m_fdbText = new List<string>(m_feedback.Length);
        m_outText = new List<string>(m_feedback.Length);

        m_fdbText.Add("Degree Fast Left: ");
        m_fdbText.Add("Degree Left: ");
        m_fdbText.Add("Degree Stay: ");
        m_fdbText.Add("Degree Right: ");
        m_fdbText.Add("Degree Fast Right: ");

        m_outText.Add("");
        m_outText.Add("");
        m_outText.Add("");
        m_outText.Add("");
        m_outText.Add("");

        if (!line) line = GameObject.Find("RaceLine");

        // Distance
        fv_dist.AddMembershipFunction(new DecreasingGradeMF("left", -0.3D, -0.005D, -0.5D));
        fv_dist.AddMembershipFunction(new TrapezoidMF("middle", -0.15D, -0.005D, 0.005D, 0.15D));
        fv_dist.AddMembershipFunction(new IncreasingGradeMF("right", 0.005D, 0.3D, 0.5D));


        // Horizontal Speed
        fv_hspeed.AddMembershipFunction(new DecreasingGradeMF("leftwards", -0.5D, -0.025D, -0.5D));
        fv_hspeed.AddMembershipFunction(new TrapezoidMF("still", -0.3D, -0.05D, 0.05D, 0.3D));
        fv_hspeed.AddMembershipFunction(new IncreasingGradeMF("rightwards", 0.025D, 0.5D, 0.5D));

        // Increase Speed (increment to be applied to speed)
        fv_incspeed.AddMembershipFunction(new DecreasingGradeMF("fastleft", -0.5D, -0.3D, -0.5D));
        fv_incspeed.AddMembershipFunction(new TrapezoidMF("left", -0.5D, -0.4D, -0.2D, -0.005D));
        fv_incspeed.AddMembershipFunction(new TrapezoidMF("stay", -0.2D, -0.005D, 0.005D, 0.2D));
        fv_incspeed.AddMembershipFunction(new TrapezoidMF("right", 0.005D, 0.2D, 0.4D, 0.5D));
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
        Debug.Log(string.Format("dist: {0}   speed: {1}", hPos, m_currentSpeed));

        if (hPos > 0.5 || hPos < -0.5)
            Debug.Log(string.Format("dist: {0}   hPos: {1}", dist, hPos));


        //Debug.Log(string.Format("dist left: {0}   dist middle: {1}   dist right: {2}", fv_dist.get("left"), fv_dist.get("middle"), fv_dist.get("right")));
        //Debug.Log(string.Format("s left: {0}   s still: {1}   s right: {2}", fv_hspeed.get("leftwards"), fv_hspeed.get("still"), fv_hspeed.get("rightwards")));
        

        double degreeFastLeft = FuzzyEngine.AND(
                                        fv_dist.get("right"),
                                        fv_hspeed.get("rightwards"));
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
                                        FuzzyEngine.AND(fv_dist.get("left"),fv_hspeed.get("still")),
                                        FuzzyEngine.AND(fv_dist.get("middle"), fv_hspeed.get("leftwards"))
                                        );
        double degreeFastRight = FuzzyEngine.AND(
                                        fv_dist.get("left"),
                                        fv_hspeed.get("leftwards"));



        m_outText[0] = string.Format("{0, 5}", degreeFastLeft);
        m_outText[1] = string.Format("{0, 5}", degreeLeft);
        m_outText[2] = string.Format("{0, 5}", degreeStay);
        m_outText[3] = string.Format("{0, 5}", degreeRight);
        m_outText[4] = string.Format("{0, 5}", degreeFastRight);

        for (int i = 0; i < m_feedback.Length; ++i)
        {
            m_feedback[i].text = m_fdbText[i] + m_outText[i];
        }

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
        
        m_currentSpeed += speedIncrement;

        // change stay to be 1 only at middle or check against left and right too
        if (degreeStay == 1 && degreeLeft == 0 && degreeRight == 0)
            m_currentSpeed = 0.0;

        pos.x += Time.deltaTime * (float)(m_speedModifier * (m_currentSpeed));

        transform.position = pos;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -10.0f * (float)m_currentSpeed));
    }
}
