using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    public class AffineConverter
    {
        static double[,] MoveMatrix = new double[4, 4];
        static double[,] RotateMatrix = new double[4, 4];
        static double[,] ScaleMatrix;

        public static void move(List<Point3D> shapesPoints, int xleng, int yleng, int zleng)
        {
            MoveMatrix = new double[4, 4];
            //матрица преобразований
            MoveMatrix[0, 0] = 1;
            MoveMatrix[1, 1] = 1;
            MoveMatrix[2, 2] = 1;
            MoveMatrix[3, 3] = 1;
            MoveMatrix[3, 0] = xleng;
            MoveMatrix[3, 1] = yleng;
            MoveMatrix[3, 2] = zleng;

            //меняем координаты для каждой точки
            for(int i = 0; i < shapesPoints.Count; i++)
            {
                int[] homCord = new int[4] { shapesPoints[i].x, shapesPoints[i].y,shapesPoints[i].z ,1 }; //однородные координаты

                int[] res = PointsColculator.matrixsMultiplication(homCord, MoveMatrix);

                shapesPoints[i] = new Point3D(res[0], res[1], res[2]);
            }
        }

        public static void Xrotate(Point3D basePoint, List<Point3D> points, int angle)
        {
            RotateMatrix = new double[4, 4];

            RotateMatrix[0, 0] = 1;
            RotateMatrix[1, 1] = Math.Cos(angle * Math.PI / 180);
            RotateMatrix[1, 2] = Math.Sin(angle * Math.PI / 180);
            RotateMatrix[2, 1] = -1*Math.Sin(angle * Math.PI / 180);
            RotateMatrix[2, 2] = Math.Cos(angle * Math.PI / 180);
            RotateMatrix[3, 3] = 1;

            makeOperation(basePoint, points, RotateMatrix);
        }

        public static void Yrotate(Point3D basePoint, List<Point3D> points, int angle)
        {
            RotateMatrix = new double[4, 4];

            RotateMatrix[0, 0] = Math.Cos(angle * Math.PI / 180);
            RotateMatrix[1, 1] = 1;
            RotateMatrix[0, 2] = -1 * Math.Sin(angle * Math.PI / 180);
            RotateMatrix[2, 0] = Math.Sin(angle * Math.PI / 180);
            RotateMatrix[2, 2] = Math.Cos(angle * Math.PI / 180);
            RotateMatrix[3, 3] = 1;

            makeOperation(basePoint, points, RotateMatrix);
        }

        public static void Zrotate(Point3D basePoint, List<Point3D> points, int angle)
        {
            RotateMatrix = new double[4, 4];

            RotateMatrix[0, 0] = Math.Cos(angle * Math.PI / 180);
            RotateMatrix[0, 1] = Math.Sin(angle * Math.PI / 180);
            RotateMatrix[1, 0] = -Math.Sin(angle * Math.PI / 180);
            RotateMatrix[1, 1] = Math.Cos(angle * Math.PI / 180);
            RotateMatrix[2, 2] = 1;
            RotateMatrix[3, 3] = 1;

            makeOperation(basePoint, points, RotateMatrix);
        }

        public static void scale(List<Point3D> points, double scaleX, double scaleY, double scaleZ)
        {
            ScaleMatrix = new double[4, 4];

            ScaleMatrix[0, 0] = scaleX;
            ScaleMatrix[1, 1] = scaleY;
            ScaleMatrix[2, 2] = scaleZ;
            ScaleMatrix[3, 3] = 1;

            makeOperation(null, points, ScaleMatrix);
        }

        private static void makeOperation(Point3D basePoint, List<Point3D> points, double[,] matrix)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (basePoint != points[i])
                {
                    int[] homCord = new int[4] { points[i].x, points[i].y, points[i].z, 1 };
                    int[] res = PointsColculator.matrixsMultiplication(homCord, matrix);

                    points[i].x = res[0];
                    points[i].y = res[1];
                    points[i].z = res[2];
                }
            }
        }
    }
}
