using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class MembershipFunction
{
    
    public string name;
    protected double m_input;
    protected double m_fuzzyfiedInput;


    public MembershipFunction(string name)
    {
        this.name = name;
    }

    // Find degree of membership to the function given a value in the x-axis
    public abstract double Fuzzify(double inputValue);

    // Used to deffuzify (input into getCentroid)
    // Returns a list with vertices of the polygon cut at "mfDegree" height (clockwise order)
    public abstract List<Point> getVertices(double mfDegree);
}
