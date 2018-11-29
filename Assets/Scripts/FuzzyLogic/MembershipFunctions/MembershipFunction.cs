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

    public abstract double Fuzzify(double inputValue);

    // Used to deffuzify (input into getCentroid)
    public abstract List<Point> getVertices(double mfDegree);
}
