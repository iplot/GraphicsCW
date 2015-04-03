using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    public struct TrianglesIndexes
    {
        public int fPointIndex;
        public int sPointIndex;
        public int tPointIndex;

        public TrianglesIndexes(int firstP, int secondP, int thirdP):this()
        {
            fPointIndex = firstP;
            sPointIndex = secondP;
            tPointIndex = thirdP;
        }
    }

    class Housing
    {
        static /*const*/ List<TrianglesIndexes> triangleVertex = new List<TrianglesIndexes>{
            new TrianglesIndexes(0,1,2), new TrianglesIndexes(0,3,2), new TrianglesIndexes(0,1,5), new TrianglesIndexes(0,4,5), new TrianglesIndexes(0,3,4),
            new TrianglesIndexes(3,4,7), new TrianglesIndexes(3,2,7), new TrianglesIndexes(2,6,7), new TrianglesIndexes(1,5,6), new TrianglesIndexes(2,6,1),
            new TrianglesIndexes(4,5,7), new TrianglesIndexes(5,6,7)};

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

        List<Point3D> vertexes;
        Point3D startPoint;

        Color color;

        public Housing(Point3D basePoint, int length, Color col)
        {
            color = col;
            //создание объекта в системе координат текущей камеры. Потом координаты должны перевестись в мир

            vertexes = new List<Point3D>();
            startPoint = new Point3D(basePoint.x - length/2, basePoint.y - length/2, basePoint.z + length/2);

            // нижнее основание
            vertexes.Add( new Point3D(basePoint.x, basePoint.y, basePoint.z) );
            vertexes.Add( new Point3D(basePoint.x, basePoint.y, basePoint.z + length) ); //аналог по z

            vertexes.Add( new Point3D(basePoint.x - length, basePoint.y, basePoint.z + length) ); //аналог по z
            vertexes.Add( new Point3D(basePoint.x - length, basePoint.y, basePoint.z) ); 

            //верхнее основание
            vertexes.Add( new Point3D(basePoint.x, basePoint.y - length, basePoint.z) );
            vertexes.Add( new Point3D(basePoint.x, basePoint.y - length, basePoint.z + length) ); // аналог по z

            vertexes.Add( new Point3D(basePoint.x - length, basePoint.y - length, basePoint.z + length) ); // аналог по z
            vertexes.Add( new Point3D(basePoint.x - length, basePoint.y - length, basePoint.z) ); 
        }

        public  void moveHousing(int xleng, int yleng, int zleng)
        {
            startPoint.x += xleng;
            startPoint.y += yleng;
            startPoint.z += zleng;
            AffineConverter.move(vertexes, xleng, yleng, zleng);
        }

        public void XrotateHousing(int angle)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, vertexes, angle);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

        public void YrotateHousing(int angle)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, vertexes, angle);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

        public void ZrotateHousing(int angle)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, vertexes, angle);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

        public void scaleHousing(double scaleX, double scaleY, double scaleZ)
        {
            AffineConverter.move(vertexes, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(vertexes, scaleX, scaleY, scaleZ);
            AffineConverter.move(vertexes, startPoint.x, startPoint.y, startPoint.z);
        }

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
            List<Point3D> list = new List<Point3D>();
            list.AddRange(vertexes);
            return list;
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
