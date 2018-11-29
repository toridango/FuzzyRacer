using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/**
 *            ^
 *           / \
 *          /   \
 *         /     \
 *        /       \
 *  ------         ------
 *       x0   x1   x2
 **/
public class TriangleMF : TrapezoidMF
{
    public TriangleMF(String name, double x0, double x1, double x2)
            : base(name, x0, x1, x1, x2)
    {

    }
}
