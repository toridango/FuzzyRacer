using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Point
{
    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public double x { get; set; }
    public double y { get; set; }
}


public class Polygon
{
    private List<Point> m_vertices;

    public Polygon()
    {
        m_vertices = new List<Point>();
    }

    public Polygon(List<Point> vertices)
    {
        m_vertices = new List<Point>(vertices);
    }

    public Point this[int i]
    {
        get { return m_vertices[i]; }
        set { m_vertices[i] = value; }
    }

    public void add(Point v)
    {
        m_vertices.Add(v);
    }

    public int getVertexCount()
    {
        return m_vertices.Count();
    }

    public Point getCentroid()
    {
        double accumulatedArea = 0.0D;
        double centreX = 0.0D;
        double centreY = 0.0D;

        for (int i = 0, j = m_vertices.Count() - 1; i < m_vertices.Count(); j = i++)
        {

            double temp = m_vertices[i].x * m_vertices[j].y - m_vertices[j].x * m_vertices[i].y;
            accumulatedArea += temp;
            centreX += (m_vertices[i].x + m_vertices[j].x) * temp;
            centreY += (m_vertices[i].y + m_vertices[j].y) * temp;
        }

        if (Math.Abs(accumulatedArea) < 1E-7D)
            return new Point(0.0D, 0.0D);

        accumulatedArea *= 0.5D;
        centreX *= 1.0D / (6.0D * accumulatedArea);
        centreY *= 1.0D / (6.0D * accumulatedArea);
        


        return new Point(centreX, centreY);
    }
}



public class FuzzyEngine
{

    public static double AND(double A, double B)
    {
        return Math.Min(A, B);
    }

    public static double AND(double A, double B, double C)
    {
        return Math.Min(Math.Min(A, B), C);
    }

    public static double OR(double A, double B)
    {
        return Math.Max(A, B);
    }

    public static double OR(double A, double B, double C)
    {
        return Math.Max(Math.Max(A, B), C);
    }

    public static double NOT(double A)
    {
        return 1.0 - A;
    }

    public static double VERY(double A)
    {
        return Math.Pow(A,2);
    }

    public static double NOT_VERY(double A)
    {
        return Math.Pow(A, 0.5f);
    }
    

    public static Point CentroidOfCluster(List<Point> cluster)
    {
        if (!cluster.Any()) throw new ArgumentException("Can't calculate centroid of empty cluster");

        int i;
        int emptyCount = 0;
        double sumX = 0.0D;
        double sumY = 0.0D;

        for (i = 0; i < cluster.Count(); ++i)
        {
            if (cluster[i].x == 0.0D && cluster[i].y == 0.0D)
            {
                ++emptyCount;
            }
            else
            {
                sumX += cluster[i].x;
                sumY += cluster[i].y;
            }
        }
        double total = (double)(i - emptyCount);
        if (total == 0.0D)
            return new Point(0.0D, 0.0D);


        return new Point(sumX / total, sumY / total);
    }



}
