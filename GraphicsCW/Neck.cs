using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    class Neck
    {
        static List<TrianglesIndexes> triangleVertex = new List<TrianglesIndexes>{new TrianglesIndexes(0,1,4), new TrianglesIndexes(0, 3,4), 
            new TrianglesIndexes(0, 2, 3), new TrianglesIndexes(1, 5, 4), new TrianglesIndexes(1, 2, 5), new TrianglesIndexes(2, 3, 5), new TrianglesIndexes(3, 4, 5), 
            new TrianglesIndexes(0, 1, 2), /*большой палец*/new TrianglesIndexes(0+6,1+6,4+6), new TrianglesIndexes(0+6, 3+6,4+6), new TrianglesIndexes(0+6, 2+6, 3+6),
            new TrianglesIndexes(1+6, 5+6, 4+6), new TrianglesIndexes(1+6, 2+6, 5+6), new TrianglesIndexes(2+6, 3+6, 5+6), new TrianglesIndexes(3+6, 4+6, 5+6), 
            new TrianglesIndexes(0+6, 1+6, 2+6)};

        static bool[,] transmisionMatrix = new bool[6, 6]{
            {false, true, true, true, true, false},
            {true, false, true, false, true, true},
            {true, true, false, true, false, true},
            {true, false, true, false, true, true},
            {true, true, false, true, false, true},
            {false, true, true, true, true, false}
        };

        Point3D point2Eyes;

        //List<Point3D> vertexes;
        private List<Point3D> points;
        Point3D startPoint;

        Color color;

        public Neck(Point3D basePoint, int boxLength, int width, int height, Color col)
        {
            color = col;

            points = new List<Point3D>();
            int litleHeight = height / 4;

            int leng = 20;
            Point3D beginPoint = new Point3D(basePoint.x - boxLength / 2 + width / 2, basePoint.y - boxLength, basePoint.z + boxLength / 2 + leng / 2);
            //Point3D beginPoint = new Point3D(basePoint.X - boxLength / 2 + width / 2, basePoint.Y - boxLength - litleHeight, boxLength / 2 + leng / 2);
            startPoint = new Point3D(basePoint.x - boxLength/2, basePoint.y - boxLength/2, basePoint.z + boxLength/2);

            //нижний wedge
            points.Add(beginPoint);
            points.Add(new Point3D(beginPoint.x - width, beginPoint.y, beginPoint.z));
            points.Add(new Point3D(beginPoint.x - width, beginPoint.y, beginPoint.z - leng));
            points.Add(new Point3D(beginPoint.x, beginPoint.y, beginPoint.z - leng));
            points.Add(new Point3D(beginPoint.x, beginPoint.y - litleHeight, beginPoint.z));
            points.Add(new Point3D(beginPoint.x - width, beginPoint.y - litleHeight, beginPoint.z));

            ////верхний wedge
            int bigHeight = height - litleHeight;
            leng = 10;
            List<Point3D> transformMass = new List<Point3D>();

            beginPoint = new Point3D(beginPoint.x, beginPoint.y - litleHeight, beginPoint.z);
            points.Add(new Point3D(beginPoint.x, beginPoint.y, beginPoint.z));
            points.Add(new Point3D(beginPoint.x - width, beginPoint.y, beginPoint.z));
            points.Add(new Point3D(beginPoint.x - width, beginPoint.y, beginPoint.z - leng));
            points.Add(new Point3D(beginPoint.x, beginPoint.y, beginPoint.z - leng));
            points.Add(new Point3D(beginPoint.x, beginPoint.y - bigHeight, beginPoint.z));
            points.Add(new Point3D(beginPoint.x - width, beginPoint.y - bigHeight, beginPoint.z));

            for (int i = 6; i < points.Count; i++)
                transformMass.Add(points[i]);

            AffineConverter.move(transformMass, -beginPoint.x, -beginPoint.y, -beginPoint.z);
            AffineConverter.Xrotate(null, transformMass, 30);
            AffineConverter.move(transformMass, beginPoint.x, beginPoint.y, beginPoint.z);


            for (int i = 0; i < transformMass.Count; i++)
                points[i + 6] = transformMass[i];

            point2Eyes = new Point3D(points[10].x, points[10].y, points[10].z);
        }

        public Point3D getPoint2Eyes()
        {
            return point2Eyes;
        }

        public void moveNeck(int xleng, int yleng, int zleng)
        {
            startPoint.x += xleng;
            startPoint.y += yleng;
            startPoint.z += zleng;

            AffineConverter.move(points, xleng, yleng, zleng);
        }

        public void XrotateNeck(int angle)
        {
            AffineConverter.move(points, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, points, angle);
            AffineConverter.move(points, startPoint.x, startPoint.y, startPoint.z);
        }

        public void YrotateNeck(int angle)
        {
            AffineConverter.move(points, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, points, angle);
            AffineConverter.move(points, startPoint.x, startPoint.y, startPoint.z);
        }

        public void ZrotateNeck(int angle)
        {
            AffineConverter.move(points, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, points, angle);
            AffineConverter.move(points, startPoint.x, startPoint.y, startPoint.z);
        }

        public void scaleNeck(double scaleX, double scaleY, double scaleZ)
        {
            AffineConverter.move(points, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(points, scaleX, scaleY, scaleZ);
            AffineConverter.move(points, startPoint.x, startPoint.y, startPoint.z);
        }

        //не проверялась!
        public void camera2world(double[,] transformMatrix)
        {
            int[] cords;
            int[] result;

            for (int i = 0; i < points.Count; i++)
            {
                cords = new int[4] { points[i].x, points[i].y, points[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

                points[i] = new Point3D(result);
            }

            cords = new int[4] { startPoint.x, startPoint.y, startPoint.z, 1 };
            result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

            startPoint = new Point3D(result);
        }

        public List<Point3D> getPoints()
        {
            return points;
        }

        public List<bool[,]> getTransmissionTab()
        {
            return new List<bool[,]> { transmisionMatrix, transmisionMatrix };
        }

        public List<TrianglesIndexes> getIndexes()
        {
            return new List<TrianglesIndexes>(triangleVertex);
        }

        public List<Color> getColor()
        {
            return new List<Color> { color };
        }
    }
}
