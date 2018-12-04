using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 *            ------
 *           /      \
 *          /        \
 *         /          \
 *        /            \
 *  ------              ------
 *       x0  x1    x2  x3
 **/
public class TrapezoidMF : MembershipFunction
{

    private double x0 = 0;
    private double x1 = 0;
    private double x2 = 0;
    private double x3 = 0;

    public double X0 { get { return x0; } }
    public double X1 { get { return x1; } }
    public double X2 { get { return x2; } }
    public double X3 { get { return x3; } }


    public TrapezoidMF(String name, double x0, double x1, double x2, double x3)
        : base(name)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.x2 = x2;
        this.x3 = x3;
    }

    // Find degree of membership to the function given a value in the x-axis
    public override double Fuzzify(double inputValue)
    {
        m_input = inputValue;

        if (inputValue <= x0 || inputValue >= x3)
            m_fuzzyfiedInput = 0.0D;

        else if (x1 <= inputValue && inputValue <= x2)
            m_fuzzyfiedInput = 1.0D;

        else if (x0 < inputValue && inputValue < x1)
            m_fuzzyfiedInput = (inputValue / (x1 - x0)) - (x0 / (x1 - x0));

        else
            m_fuzzyfiedInput = (-inputValue / (x3 - x2)) + (x3 / (x3 - x2));
        

        return m_fuzzyfiedInput;
    }

    // Returns a list with vertices of the polygon cut at "mfDegree" height (clockwise order)
    public override List<Point> getVertices(double mfDegree)
    {
        List<Point> vertices = new List<Point>();

        vertices.Add(new Point(x0, 0.0D));
        double newX1 = mfDegree * (x1 - x0) + x0;
        vertices.Add(new Point(newX1, mfDegree));
        double newX2 = x3 - mfDegree * (x3 - x2);
        vertices.Add(new Point(newX2, mfDegree));
        vertices.Add(new Point(x3, 0.0D));


        return vertices;
    }


}
