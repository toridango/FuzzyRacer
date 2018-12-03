using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FuzzyVariable
{
    public string name;

    private List<MembershipFunction> mfuncs;
    private List<double> memberships;
    private bool fuzzified = false;

    public FuzzyVariable(string name)
    {
        this.name = name;
        this.mfuncs = new List<MembershipFunction>();
        this.memberships = new List<double>();
    }

    public void AddMembershipFunction(MembershipFunction mf)
    {
        mfuncs.Add(mf);
    }


    public void Fuzzify(double inputValue)
    {
        if (memberships != null) memberships.Clear();
        foreach (MembershipFunction mf in mfuncs)
        {
            memberships.Add(mf.Fuzzify(inputValue));
        }
        fuzzified = true;
    }

    public bool isFuzzified()
    {
        return fuzzified;
    }



    // Returns the membership to the requested function of this variable
    public double get(string mf_name)
    {
        if (!fuzzified) { throw new System.ArgumentException("Variable hasn't been fuzzified", "fuzzified"); }

        int i = 0;
        double value = 0;
        bool found = false;

        while (i < mfuncs.Count && !found)
        {
            if (mfuncs[i].name == mf_name)
            {
                value = memberships[i];
                found = true;
            }
            else
                i++;
        }

        if (!found) { throw new System.ArgumentException(String.Format("Error: Membership Function '{0}' not found", mf_name), "mf_name"); }

        return value;
    }
    

    public Polygon getMFPolygon(string mf_name, double mfDegree)
    {
        int i = 0;
        bool found = false;
        Polygon p = new Polygon();

        while (i < mfuncs.Count && !found)
        {
            if (mfuncs[i].name == mf_name)
            {
                p = new Polygon(mfuncs[i].getVertices(mfDegree));

                found = true;
            }
            else
                i++;
        }

        if (!found) throw new ArgumentException(String.Format("Error: Membership Function '{0}' not found", mf_name));

        return p;
    }
}
