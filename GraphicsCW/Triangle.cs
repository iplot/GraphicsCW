using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    public struct TrianglePoints
    {
        public Point3D screenPoint;
        public Point3D cameraPoint;

        public TrianglePoints(Point3D screen, Point3D camera)
            : this()
        {
            this.screenPoint = new Point3D(screen);
            this.cameraPoint = new Point3D(camera);
        }
    }
    class Triangle
    {
        List<TrianglePoints> vertexes;
        Color color;

        public Triangle(List<Point3D> screenP, List<Point3D> cameraP, Color col)
        {
            vertexes = new List<TrianglePoints>();

            for (int i = 0; i < 3; i++)
            {
                vertexes.Add(new TrianglePoints(screenP[i], cameraP[i]));
            }

            vertexes.Sort(new Compar());

            color = col;
        }

        public double getAverageZ()
        {
            double S = 0;
            foreach (TrianglePoints point in vertexes)
                S += point.cameraPoint.z;

            return S / 3;
        }

        public Color TriangleColor
        {
            get { return color; }
            set { this.color = value; }
        }

        public Point3D getCameraPoint(int i)
        {
            return vertexes[i].cameraPoint;
        }

        public Point3D getScreenPoint(int i)
        {
            return vertexes[i].screenPoint;
        }
    }

    class Compar : IComparer<TrianglePoints>
    {
        public int Compare(TrianglePoints p1, TrianglePoints p2)
        {
            return p1.screenPoint.y - p2.screenPoint.y;
        }
    }
}
