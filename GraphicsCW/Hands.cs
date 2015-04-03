using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    class Hands
    {
        static /*const*/ List<TrianglesIndexes> triangleVertex = new List<TrianglesIndexes>{/*коробка руки*/
            new TrianglesIndexes(0,1,2), new TrianglesIndexes(0,3,2), new TrianglesIndexes(0,1,5), new TrianglesIndexes(0,4,5), new TrianglesIndexes(0,3,4),
            new TrianglesIndexes(3,4,7), new TrianglesIndexes(3,2,7), new TrianglesIndexes(2,6,7), new TrianglesIndexes(1,5,6), new TrianglesIndexes(2,6,1),
            new TrianglesIndexes(4,5,7), new TrianglesIndexes(5,6,7), /*маленький палец*/ new TrianglesIndexes(8,9,12), new TrianglesIndexes(8, 11,12), 
            new TrianglesIndexes(8, 10, 11), new TrianglesIndexes(9, 13, 12), new TrianglesIndexes(9, 10, 13), new TrianglesIndexes(10, 11, 13), new TrianglesIndexes(11, 12, 13), 
            new TrianglesIndexes(8, 9, 10), /*большой палец*/new TrianglesIndexes(8+6,9+6,12+6), new TrianglesIndexes(8+6, 11+6,12+6), new TrianglesIndexes(8+6, 10+6, 11+6),
            new TrianglesIndexes(9+6, 13+6, 12+6), new TrianglesIndexes(9+6, 10+6, 13+6), new TrianglesIndexes(10+6, 11+6, 13+6), new TrianglesIndexes(11+6, 12+6, 13+6), 
            new TrianglesIndexes(8+6, 9+6, 10+6)};

        static /*const*/ bool[,] transmisionMatrix = new bool[8, 8]{
            {false, true, true, true, true, true, false, false},
            {false, false, true, false, false, true, true, false},
            {true, true, false, true, false, false, true, true},
            {true, false, true, false, true, false, false, true},
            {true, false, false, true, false, true, false, true},
            {true, true, false, false, true, false, true, true},
            {false, true, true, false, false, true, false, true},
            {false, false, true, true, true, true, true, false}
        };

        static bool[,] fingersTransmisionMatrix = new bool[6, 6]{
            {false, true, true, true, true, false},
            {true, false, true, false, true, true},
            {true, true, false, true, false, true},
            {true, false, true, false, true, true},
            {true, true, false, true, false, true},
            {false, true, true, true, true, false}
        };
        

        Point3D startPoint;
        List<Point3D> hand1;
        List<Point3D> hand2;

        Color color;

        public Hands(Point3D basePoint,int boxlength, int length, int width, Color col)
        {
            color = col;

            hand1 = new List<Point3D>();

            startPoint = new Point3D(basePoint.x - boxlength/2, basePoint.y - boxlength/2, basePoint.z + boxlength/2);

            Point3D mainP = new Point3D(basePoint.x + width, basePoint.y - boxlength + 5 + width, basePoint.z + boxlength / 2 - length);

            //нижнее основание
            hand1.Add(new Point3D(mainP.x, mainP.y, mainP.z));
            hand1.Add(new Point3D(mainP.x, mainP.y, mainP.z + length));
            hand1.Add(new Point3D(mainP.x - width, mainP.y, mainP.z + length));
            hand1.Add(new Point3D(mainP.x - width, mainP.y, mainP.z));

            //верхнее основание
            hand1.Add(new Point3D(mainP.x, mainP.y - width, mainP.z));
            hand1.Add(new Point3D(mainP.x, mainP.y - width, mainP.z + length));
            hand1.Add(new Point3D(mainP.x - width, mainP.y - width, mainP.z + length));
            hand1.Add(new Point3D(mainP.x - width, mainP.y - width, mainP.z));

            Point3D p = hand1[hand1.Count - 1];

            //маленький палец
            hand1.Add(mainP);
            hand1.Add(new Point3D(mainP.x, mainP.y - width, mainP.z));
            hand1.Add(new Point3D(mainP.x - width / 2, mainP.y - width, mainP.z));
            hand1.Add(new Point3D(mainP.x - width / 2, mainP.y, mainP.z));
            hand1.Add(new Point3D(mainP.x, mainP.y, mainP.z - 10));
            hand1.Add(new Point3D(mainP.x, mainP.y - width, mainP.z - 10));

            //большой палец
            hand1.Add(new Point3D(p.x, p.y - 5, p.z));
            hand1.Add(new Point3D(p.x, p.y + width + 5, p.z));
            hand1.Add(new Point3D(p.x + width / 2, p.y + width + 5, p.z));
            hand1.Add(new Point3D(p.x + width / 2, p.y - 5, p.z));
            hand1.Add(new Point3D(p.x, p.y - 5, p.z - 10));
            hand1.Add(new Point3D(p.x, p.y + width + 5, p.z - 10));

            hand2 = new List<Point3D>(hand1);
            AffineConverter.move(hand2, -(boxlength + width), 0, 0);
            //XmirrorHand(hand2, boxlength+width);

            //поворот первой руки, что бы выглядело зеркально
            AffineConverter.move(hand1, -mainP.x, -mainP.y, -mainP.z);
            AffineConverter.Zrotate(null, hand1, 180);
            AffineConverter.move(hand1, mainP.x - width, mainP.y - width, mainP.z);
        }

        public void moveHands(int xleng, int yleng, int zleng)
        {
            startPoint.x += xleng;
            startPoint.y += yleng;
            startPoint.z += zleng;

            AffineConverter.move(hand1, xleng, yleng, zleng);
            AffineConverter.move(hand2, xleng, yleng, zleng);
        }

        public void XrotateHands(int angle)
        {
            AffineConverter.move(hand1, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, hand1, angle);
            AffineConverter.move(hand1, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(hand2, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, hand2, angle);
            AffineConverter.move(hand2, startPoint.x, startPoint.y, startPoint.z);

        }

        public void YrotateHands(int angle)
        {
            AffineConverter.move(hand1, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, hand1, angle);
            AffineConverter.move(hand1, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(hand2, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, hand2, angle);
            AffineConverter.move(hand2, startPoint.x, startPoint.y, startPoint.z);
        }

        public void ZrotateHands(int angle)
        {
            AffineConverter.move(hand1, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, hand1, angle);
            AffineConverter.move(hand1, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(hand2, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, hand2, angle);
            AffineConverter.move(hand2, startPoint.x, startPoint.y, startPoint.z);
        }

        public void scaleHands(double scaleX, double scaleY, double scaleZ)
        {
            AffineConverter.move(hand1, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(hand1, scaleX, scaleY, scaleZ);
            AffineConverter.move(hand1, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(hand2, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(hand2, scaleX, scaleY, scaleZ);
            AffineConverter.move(hand2, startPoint.x, startPoint.y, startPoint.z);
        }

        public void camera2world(double[,] transformMatrix)
        {
            int[] cords;
            int[] result;

            for (int i = 0; i < hand1.Count; i++)
            {
                cords = new int[4] { hand1[i].x, hand1[i].y, hand1[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                hand1[i] = new Point3D(result);

                cords = new int[4] { hand2[i].x, hand2[i].y, hand2[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                hand2[i] = new Point3D(result);
            }

            cords = new int[4] { startPoint.x, startPoint.y, startPoint.z, 1 };
            result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

            startPoint = new Point3D(result);
        }

        public List<Point3D> getHand1()
        {
            return hand1;
        }

        public List<Point3D> getHand2()
        {
            return hand2;
        }

        public List<bool[,]> getTransmissionTab()
        {
            return new List<bool[,]> { transmisionMatrix, fingersTransmisionMatrix, fingersTransmisionMatrix };
        }

        public List<TrianglesIndexes> getIndexes()
        {
            return new List<TrianglesIndexes>(triangleVertex);
        }

        public List<Color> getColor()
        {
            return new List<Color>{color, color};
        }
    }
}
