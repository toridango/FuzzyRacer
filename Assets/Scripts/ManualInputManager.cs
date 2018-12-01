using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ManualInputManager : MonoBehaviour
{


    public InputField in_distance;
    public InputField in_velocity;
    public Text out_FIS;
    public Text out_Pot;
    public Text out_FSM;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        NumberStyles style = NumberStyles.Number;
        CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;

        double distance = 0.0;
        double hSpeed = 0.0;

        if (in_distance.text != "" && in_velocity.text != "")
        {
            if (!double.TryParse(in_distance.text, style, culture, out distance))
                Debug.Log("Wrong format for distance input (" + in_distance.text + ")");

            if (!double.TryParse(in_velocity.text, style, culture, out hSpeed))
                Debug.Log("Wrong format for linear velocity input (" + in_velocity.text + ")");
        }

        out_FIS.text = String.Format("{0, 8}", getSpeedFIS(distance, hSpeed));
        out_Pot.text = String.Format("{0, 8}", getForcePotential(distance, hSpeed));
        out_FSM.text = String.Format("{0, 8}", getSpeedFSM(distance, hSpeed));

    }




    double getSpeedFIS(double distance, double hSpeed)
    {

        double speedModifier = 25.0;
        // Distance
        FuzzyVariable fv_dist = new FuzzyVariable("distance");
        // Speed
        FuzzyVariable fv_hspeed = new FuzzyVariable("horizontal_Speed");
        // Output Speed
        FuzzyVariable fv_incspeed = new FuzzyVariable("increase_Speed");

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


        double allowedWidth = (LineController.horizontalMax - LineController.horizontalMin);
        distance = distance / allowedWidth;

        fv_dist.Fuzzify(distance);
        fv_hspeed.Fuzzify(hSpeed);



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
                                        FuzzyEngine.AND(fv_dist.get("left"), fv_hspeed.get("still")),
                                        FuzzyEngine.AND(fv_dist.get("middle"), fv_hspeed.get("leftwards"))
                                        );
        double degreeFastRight = FuzzyEngine.AND(
                                        fv_dist.get("left"),
                                        fv_hspeed.get("leftwards"));


        List<Point> centroids = new List<Point>();
        if (degreeFastLeft != 0) centroids.Add(fv_incspeed.getMFPolygon("fastleft", degreeFastLeft).getCentroid());
        if (degreeLeft != 0) centroids.Add(fv_incspeed.getMFPolygon("left", degreeLeft).getCentroid());
        if (degreeStay != 0) centroids.Add(fv_incspeed.getMFPolygon("stay", degreeStay).getCentroid());
        if (degreeRight != 0) centroids.Add(fv_incspeed.getMFPolygon("right", degreeRight).getCentroid());
        if (degreeFastRight != 0) centroids.Add(fv_incspeed.getMFPolygon("fastright", degreeFastRight).getCentroid());

        double speedIncrement = FuzzyEngine.CentroidOfCluster(centroids).x;


        hSpeed += speedIncrement;

        // change stay to be 1 only at middle or check against left and right too
        if (degreeStay == 1 && degreeLeft == 0 && degreeRight == 0)
            hSpeed = 0.0;

        return speedModifier * hSpeed;

    }

    double getForcePotential(double distance, double hSpeed)
    {
        double force = 0.0;
        if (distance > 1E-8)
        {
            force = (distance / Mathf.Abs((float)distance)) * ((-2 * 100) / Mathf.Pow((float)distance, -2 + 1));
        }
        // Mass is considered 1 if force is not divided by anything
        return force;

    }

    float getSpeedFSM(double distance, double hSpeed)
    {

        FSM fsm = new FSM();

        fsm.updateState((float)distance, (float)hSpeed);
        return fsm.currentOutput((float)distance, (float)hSpeed);


    }
}
