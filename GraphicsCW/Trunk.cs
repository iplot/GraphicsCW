using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    class Trunk
    {
        static /*const*/ List<TrianglesIndexes> triangleVertex = new List<TrianglesIndexes>{/*коробка багажника*/
            new TrianglesIndexes(0,1,2), new TrianglesIndexes(0,3,2), new TrianglesIndexes(0,1,5), new TrianglesIndexes(0,4,5), new TrianglesIndexes(0,3,4),
            new TrianglesIndexes(3,4,7), new TrianglesIndexes(3,2,7), new TrianglesIndexes(2,6,7), new TrianglesIndexes(1,5,6), new TrianglesIndexes(2,6,1),
            new TrianglesIndexes(4,5,7), new TrianglesIndexes(5,6,7), /*крышка багажника*/ new TrianglesIndexes(4,8,9), new TrianglesIndexes(4,7,9),
            new TrianglesIndexes(6,7,9), new TrianglesIndexes(6,8,9), new TrianglesIndexes(6,5,8), new TrianglesIndexes(5,4,8)};

        static /*const*/ bool[,] transmisionMatrix = new bool[10, 10]{
            {false, true, true, true, true, true, false, false, false, false},
            {false, false, true, false, false, true, true, false, false, false},
            {true, true, false, true, false, false, true, true, false, false},
            {true, false, true, false, true, false, false, true, false, false},
            {true, false, false, true, false, true, false, true, true, true},
            {true, true, false, false, true, false, true, true, true, false},
            {false, true, true, false, false, true, false, true, true, true},
            {false, false, true, true, true, true, true, false, false, true},
            {false,false,false,false, true, true, true, false, false, true},
            {false,false,false,false, true, false, true, true, true, false}
        };

        List<Point3D> vertexes;
        Point3D startPoint;

        Color color;

        public Trunk(Point3D basePoint, int boxLength, int trunkWidth, int trunkHeight, Color col)
        {
            color = col;

            startPoint = new Point3D(basePoint.x - boxLength/2, basePoint.y - boxLength/2, basePoint.z + boxLength/2);
            vertexes = new List<Point3D>();

            int z = 20;
            Point3D mainP = new Point3D(basePoint.x - (boxLength - trunkWidth) / 2, basePoint.y - (boxLength - trunkHeight) / 2, basePoint.z + boxLength);

            //нижнее основание
            vertexes.Add(new Point3D(mainP.x, mainP.y, mainP.z));
            vertexes.Add(new Point3D(mainP.x, mainP.y, mainP.z + z));
            vertexes.Add(new Point3D(mainP.x - trunkWidth, mainP.y, mainP.z + z));
            vertexes.Add(new Point3D(mainP.x - trunkWidth, mainP.y, mainP.z));

            //верхнее основание
            vertexes.Add(new Point3D(mainP.x, mainP.y - trunkHeight * 3 / 4, mainP.z));
            vertexes.Add(new Point3D(mainP.x, mainP.y - trunkHeight * 3 / 4, mainP.z + z));
            vertexes.Add(new Point3D(mainP.x - trunkWidth, mainP.y - trunkHeight * 3 / 4, mainP.z + z));
            vertexes.Add(new Point3D(mainP.x - trunkWidth, mainP.y - trunkHeight * 3 / 4, mainP.z));

            //верхушка клина
            int h = trunkHeight - (trunkHeight * 3 / 4);
            mainP = new Point3D(mainP.x, mainP.y - trunkHeight * 3 / 4, mainP.z);

            vertexes.Add(new Point3D(mainP.x, mainP.y - h, mainP.z));
            vertexes.Add(new Point3D(mainP.x - trunkWidth, mainP.y - h, mainP.z));
        }

        public void moveTrunk(int xleng, int yleng, int zleng)
        {
            startPoint.x += xleng;
            startPoint.y += yleng;
            startPoint.z += zleng;
            AffineConverter.move(vertexes, xleng, yleng, zleng);
        }

        public void XrotateTrunk(int angle)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, vertexes, angle);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

        public void YrotateTrunk(int angle)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, vertexes, angle);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

        public void ZrotateTrunk(int angle)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, vertexes, angle);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

        public void scaleTrunk(double scaleX, double scaleY, double scaleZ)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(vertexes, scaleX, scaleY, scaleZ);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

        //не проверялась!
        public void camera2world(double[,] transformMatrix)
        {
            int[] cords;
            int[] result;

            for (int i = 0; i < vertexes.Count; i++)
            {
                cords = new int[4] { vertexes[i].x, vertexes[i].y, vertexes[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

                vertexes[i] = new Point3D(result);
            }

            cords = new int[4] { startPoint.x, startPoint.y, startPoint.z, 1 };
            result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

            startPoint = new Point3D(result);
        }

        public List<Point3D> getPoints()
        {
            return vertexes;
        }

        public List<bool[,]> getTransmissionTab()
        {
            return new List<bool[,]> { transmisionMatrix };
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
