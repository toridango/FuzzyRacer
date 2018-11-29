using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/**
 *            ------
 *           /
 *          /
 *         / 
 *        /   
 *  ------ 
 *       x0  x1    x2
 **/
public class IncreasingGradeMF : MembershipFunction
{

    private double x0 = 0;
    private double x1 = 0;
    private double x2 = 0;

    public double X0 { get { return x0; } }
    public double X1 { get { return x1; } }
    public double X2 { get { return x2; } }

    // X2 is where it ends
    public IncreasingGradeMF(String name, double x0, double x1, double x2)
            : base(name)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.x2 = x2;
    }

    public override double Fuzzify(double inputValue)
    {
        double result = 0.0D;

        if (inputValue <= x0)
            result = 0.0D;
        else if (inputValue >= x1)
            result = 1.0D;
        else
            result = (inputValue / (x1 - x0)) - (x0 / (x1 - x0));

        return result;
    }

    public override List<Point> getVertices(double mfDegree)
    {
        List<Point> vertices = new List<Point>();

        vertices.Add(new Point(x0, 0.0D));
        double newX1 = mfDegree * (x1 - x0) + x0;
        vertices.Add(new Point(newX1, mfDegree));
        if (x2 != newX1)
            vertices.Add(new Point(x2, mfDegree));
        vertices.Add(new Point(x2, 0.0D));


        return vertices;
    }

}
