using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsCW
{
    public struct Result
    {
        public int length;
        public int[] x;
        public int[] y;
    }

    public static class PointsColculator
    {
        public static Result linePoints(int x1, int y1, int x2, int y2)
        {
            //bratherhem line algorithm

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x2 > x1) ? 1 : -1;
            int sy = (y2 > y1) ? 1 : -1;

            int[] resx = new int[1000];
            int[] resy = new int[1000];

            if (dy <= dx)
            {
                int d = 2 * dy - dx;
                int d1 = 2 * dy;
                int d2 = 2 * (dy - dx);

                resx[0] = x1;
                resy[0] = y1;

                for (int x = x1 + sx, y = y1, i = 1; i <= dx; i++, x += sx)
                {
                    if (d > 0)
                    {
                        d += d2;
                        y += sy;
                    }
                    else
                        d += d1;

                    resx[i] = x;
                    resy[i] = y;
                }

                Result result = new Result();
                result.length = dx;
                result.x = resx;
                result.y = resy;
                return result;

            }
            else
            {
                int d = 2 * dx - dy;
                int d1 = 2 * dx;
                int d2 = 2 * (dx - dy);

                resx[0] = x1;
                resy[0] = y1;

                for (int x = x1, y = y1 + sy, i = 1; i <= dy; i++, y += sy)
                {
                    if (d > 0)
                    {
                        d += d2;
                        x += sx;
                    }
                    else
                        d += d1;

                    resx[i] = x;
                    resy[i] = y;
                }

                Result result = new Result();
                result.length = dy;
                result.x = resx;
                result.y = resy;
                return result;
            }
        }

        public static int[] vectorsMultiplication(Point3D start, Point3D destination)
        {
            int[] multiplication = new int[3];

            multiplication[0] = start.y * destination.z - start.z * destination.y;
            multiplication[1] = start.z * destination.x - start.x * destination.z;
            multiplication[2] = start.x * destination.y - start.y * destination.x;

            return multiplication;
        }

        public static double vectorsLeng(Point3D vector)
        {
            return Math.Sqrt(Math.Pow(vector.x, 2) + Math.Pow(vector.y, 2) + Math.Pow(vector.z, 2));
        }

        public static int[] surfaceKoefs(Point3D p0, Point3D p1, Point3D p2)
        {
            int[] koefs = new int[4];

            koefs[0] = (p1.y - p0.y) * (p2.z - p0.z) - (p2.y - p0.y) * (p1.z - p0.z);   //коэффициент А
            koefs[1] = -1 * ( (p1.x - p0.x) * (p2.z - p0.z) - (p2.x - p0.x) * (p1.z - p0.z) );   //коэффициент В
            koefs[2] = (p1.x - p0.x) * (p2.y - p0.y) - (p2.x - p0.x) * (p1.y - p0.y);   //коэффициент С
            koefs[3] = (-p0.x * koefs[0]) + (-p0.y * koefs[1]) + (-p0.z * koefs[2]);    //коэффициент D

            return koefs;
        }

        //получаем обратную матрицу. Для перевода из координат камеры в координаты мира
        public static double[,] inverseMatrix(double[,] matrix)
        {
            double[,] inverseMatrix = new double[4, 4];

            double det = matrix[0, 0] * matrix[1, 1] * matrix[2, 2] + matrix[0, 1] * matrix[1, 2] * matrix[2, 0] + matrix[1, 0] * matrix[2, 1] * matrix[0, 2] 
                - matrix[2, 0] * matrix[1, 1] * matrix[0, 2] - matrix[0, 1] * matrix[1, 0] * matrix[2, 2] - matrix[0, 0] * matrix[1, 2] * matrix[2, 1];

            inverseMatrix[0, 0] = matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1];
            inverseMatrix[0, 1] = -(matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0]);
            inverseMatrix[0, 2] = matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0];

            inverseMatrix[1, 0] = -(matrix[0, 1] * matrix[2, 2] - matrix[0, 2] * matrix[2, 1]);
            inverseMatrix[1, 1] = matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0];
            inverseMatrix[1, 2] = -(matrix[0, 0] * matrix[2, 1] - matrix[0, 1] * matrix[2, 0]);

            inverseMatrix[2, 0] = matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1];
            inverseMatrix[2, 1] = -(matrix[0, 0] * matrix[1, 2] - matrix[0, 2] * matrix[1, 0]);
            inverseMatrix[2, 2] = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    inverseMatrix[i, j] /= det;

            double[,] transponMatrix = new double[4, 4];

            //транспонирование
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    transponMatrix[i, j] = inverseMatrix[j, i];

            //для переноса обратно
            for (int i = 0; i < 3; i++)
                transponMatrix[3, i] = -matrix[3, i];

            transponMatrix[3, 3] = 1;

            return transponMatrix;
        }

        //перемножение вектора на матрицу
        public static int[] matrixsMultiplication(int[] pointCords, double[,] convertMatrix)
        {
            int[] res = new int[pointCords.Length];

            //i - столбец j - строка
            for (int i = 0; i < Math.Sqrt(convertMatrix.Length); i++)
            {
                double S = 0;
                for (int j = 0; j < Math.Sqrt(convertMatrix.Length); j++)
                    S += pointCords[j] * convertMatrix[j, i];

                res[i] = (int)S;
            }

            return res;
        }
    }
}
