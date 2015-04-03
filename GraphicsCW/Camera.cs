using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsCW
{
    //!!!НЕ ПРОВЕРЯЛОСЬ!!! ЕСЛИ ЧТО ЕСТЬ КОПИЯ ВЕРНУТЬСЯ!!!
    class Camera
    {
        bool centralProjection = true;

        int screenWidth;
        int screenHeight;

        Point3D visionPoint;    //точка зрения
        Point3D screenCenter;   //центр экрана

        double[,] UVNMatrix;
        double[,] inverseUVNMatrix;

        int horizontalAngle;    //ширина обзора в градусах по горизонтали
        int verticalAngle;  //ширина обзора в градусач по вертикали

        int minDistance;    //дистантное растояние
        int maxDistance;    //расстояние до самой глубокой границы (там система координат объектов)

        List<Point3D> pyramidBasePoints;  //точки основания пирамиды видимости

        public Camera(int horizontalAngle, int verticalAngle, int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            this.horizontalAngle = horizontalAngle;
            this.verticalAngle = verticalAngle;

            minDistance = 250;
            maxDistance = 540;

            visionPoint = new Point3D(0, 0, 0);
            screenCenter = new Point3D(0, 0, minDistance);
            //screenCenter = new Point3D(0, 0, minDistance);

            createCameraMatrix();
            pyramidBase();
        }

        private void createCameraMatrix()
        {
            Point3D nVector = new Point3D(screenCenter.x - visionPoint.x, screenCenter.y - visionPoint.y, screenCenter.z - visionPoint.z);
            Point3D vVector;

            //if(nVector.z != 0)
            //    vVector = new Point3D(0, 1, 0); //начальное значение
            //else
            //    vVector = new Point3D(0, 0, 1); //начальное значение

            if (nVector.y == -minDistance)
                vVector = new Point3D(0, 0, 1); //начальное значение
            else
                if (nVector.y == minDistance)
                    vVector = new Point3D(0, 0, -1);
                else
                    vVector = new Point3D(0, 1, 0); //начальное значение

            Point3D uVector = new Point3D(PointsColculator.vectorsMultiplication(vVector, nVector));
            vVector = new Point3D(PointsColculator.vectorsMultiplication(nVector, uVector));

            UVNMatrix = new double[4, 4];
            List<Point3D> list = new List<Point3D> { uVector, vVector, nVector };
            double[] lengths = new double[3] { PointsColculator.vectorsLeng(uVector), PointsColculator.vectorsLeng(vVector), PointsColculator.vectorsLeng(nVector) };

            for (int i = 0; i < 3; i++)
            {
                //заносим в матрицу нормализованные координаты вектора
                UVNMatrix[0, i] = list[i].x / lengths[i];
                UVNMatrix[1, i] = list[i].y / lengths[i];
                UVNMatrix[2, i] = list[i].z / lengths[i];
            }

            UVNMatrix[3, 0] = -visionPoint.x;
            UVNMatrix[3, 1] = -visionPoint.y;
            UVNMatrix[3, 2] = -visionPoint.z;
            UVNMatrix[3, 3] = 1;

            inverseUVNMatrix = PointsColculator.inverseMatrix(UVNMatrix);
        }

        private void pyramidBase()
        {
            int halfLength = (int)(maxDistance * Math.Tan(horizontalAngle / 2 * Math.PI / 180));
            int halfHeight = (int)(maxDistance * Math.Tan(verticalAngle / 2 * Math.PI / 180));

            //тут работатет система координат камеры
            pyramidBasePoints = new List<Point3D>(){new Point3D(screenCenter.x + halfLength, screenCenter.y + halfHeight, maxDistance),
                new Point3D(screenCenter.x + halfLength, screenCenter.y - halfHeight, maxDistance), 
                new Point3D(screenCenter.x - halfLength, screenCenter.y - halfHeight, maxDistance),
                new Point3D(screenCenter.x - halfLength, screenCenter.y + halfHeight,maxDistance)
            };
        }

        public double[,] getUVNMatrix()
        {
            return UVNMatrix;
        }

        public double[,] getInverseUVNMatrix()
        {
            return inverseUVNMatrix;
        }

        public Point3D getVisionPoint()
        {
            return visionPoint;
        }

        public Point3D getRealVisionPoint()
        {
            transformCameraPoints(inverseUVNMatrix);
            Point3D p = new Point3D(visionPoint);
            transformCameraPoints(UVNMatrix);

            return p;
        }

        public Point3D getScreenCenter()
        {
            return screenCenter;
        }

        public Point3D getRealScreenCenter()
        {
            transformCameraPoints(inverseUVNMatrix);
            Point3D p = new Point3D(screenCenter);
            transformCameraPoints(UVNMatrix);

            return p;
        }

        public int HorizontalAngle
        {
            get { return horizontalAngle; }
            set { horizontalAngle = value; pyramidBase(); }
        }

        public int VerticalAngle
        {
            get { return verticalAngle; }
            set { verticalAngle = value; pyramidBase(); }
        }

        public bool IsCentralProjection
        {
            get { return centralProjection; }
            set { centralProjection = value; }
        }

        public int MinDistance
        {
            get{return minDistance;}
            set { minDistance = value; screenCenter = new Point3D(0, 0, minDistance); }
        }

        //!!!
        public void setPoints(Point3D viewPoint, Point3D screenCenter)
        {
            this.visionPoint = new Point3D(viewPoint);
            this.screenCenter = new Point3D(screenCenter);

            createCameraMatrix();
            transformCameraPoints(UVNMatrix);
            pyramidBase();
        }


        public List<Point3D> checkPoints(List<Point3D> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].z < maxDistance && points[i].z > minDistance)

                    if (centralProjection)
                    {
                        points[i] = (centralCheck(points[i])) ? points[i] : null;
                    }
                    else
                    {
                        points[i] = (parallelCheck(points[i])) ? points[i] : null;
                    }
                else
                    points[i] = null;
            }

            return points;
        }

        private bool parallelCheck(Point3D point)
        {
            if (point.x > screenWidth / 2 || point.x < -screenWidth / 2)
                return false;

            if (point.y > screenHeight / 2 || point.y < -screenHeight / 2)
                return false;

            return true;
        }

        private bool centralCheck(Point3D point)
        {
            int[] surface1, surface2, surface3, surface4;

            //плоскости для проверки по х
            surface1 = PointsColculator.surfaceKoefs(visionPoint, pyramidBasePoints[0], pyramidBasePoints[1]);
            surface2 = PointsColculator.surfaceKoefs(visionPoint, pyramidBasePoints[2], pyramidBasePoints[3]);
            //плоскости для проверки по у
            surface3 = PointsColculator.surfaceKoefs(visionPoint, pyramidBasePoints[1], pyramidBasePoints[2]);
            surface4 = PointsColculator.surfaceKoefs(visionPoint, pyramidBasePoints[3], pyramidBasePoints[0]);

            int res1 = point.x * surface1[0] + point.y * surface1[1] + point.z * surface1[2] + surface1[3];
            int res2 = point.x * surface2[0] + point.y * surface2[1] + point.z * surface2[2] + surface2[3];
            int res3 = point.x * surface3[0] + point.y * surface3[1] + point.z * surface3[2] + surface3[3];
            int res4 = point.x * surface4[0] + point.y * surface4[1] + point.z * surface4[2] + surface4[3];

            res1 = (res1 > 0) ? 1 : -1;
            res2 = (res2 > 0) ? 1 : -1;
            res3 = (res3 > 0) ? 1 : -1;
            res4 = (res4 > 0) ? 1 : -1;

            //if (res1 == res2)
            if(!(res1 < 0 && res2 < 0 && res3 < 0 && res4 < 0)) //если у обоих плостей <0 то внутри
                return false;
            else
                return true;
        }

        public List<Point3D> projectingOnScreen(List<Point3D> points)
        {
            for(int i = 0; i < points.Count; i++)
            {
                if (points[i] == null)
                    continue;

                if (centralProjection)
                {
                    points[i] = centralProjecting(points[i]);
                }
                else
                {
                    points[i] = parallelProjecting(points[i]);
                }
            }

                return points;
        }

        private Point3D centralProjecting(Point3D point)
        {
            int[] p1 = new int[3] { visionPoint.x, visionPoint.y, visionPoint.z };
            int[] p2 = new int[3] { point.x, point.y, point.z };
            int[] screen = new int[3] { screenCenter.x, screenCenter.y, screenCenter.z };
            int[] normal = new int[3] { 0, 0, 1 };

            double num = 0, den = 0;
            for (int i = 0; i < p1.Length; i++)
            {
                //num += -p1[i] * UVNMatrix[i, 2] + screen[i] * UVNMatrix[i, 2];
                num += -p1[i] * normal[i] + screen[i] * normal[i];
                //den += (p1[i] - p2[i]) * UVNMatrix[i, 2];
                den += (p1[i] - p2[i]) * normal[i];
            }

            double t = num / den;

            for (int i = 0; i < p1.Length; i++)
                p1[i] = (-1) * (int)Math.Round(t * (p2[i] - p1[i]) + p1[i]);  //!!!!!

            p1[0] = screenWidth / 2 + p1[0];
            p1[1] = screenHeight / 2 + p1[1];

            if (p1[0] > 0 && p1[1] > 0)
                return point = new Point3D(p1);
            else
                return null;
        }

        private Point3D parallelProjecting(Point3D point)
        {
            //point.x = screenWidth / 2 + point.x;
            //point.y = screenHeight / 2 + point.y;

            return new Point3D(screenWidth / 2 + point.x, screenHeight / 2 + point.y, minDistance); //!!!!!
        }

        public void moveCamera(int xleng, int yleng, int zleng)
        {
            transformCameraPoints(inverseUVNMatrix);

            List<Point3D> points = new List<Point3D> { visionPoint, screenCenter };
            AffineConverter.move(points, xleng, yleng, zleng);

            visionPoint = points[0];
            screenCenter = points[1];

            createCameraMatrix();   //считаем новую матрицу камеры
            transformCameraPoints(UVNMatrix);   //переводим точки камеры из мировой системы в свою
            pyramidBase();  //находим точки основания пирамиды видимости
        }

        //поворот по Х
        public void XrotateCamera(int angle)
        {
            transformCameraPoints(inverseUVNMatrix);

            List<Point3D> points = new List<Point3D> { visionPoint, screenCenter };

            AffineConverter.move(points, -visionPoint.x, -visionPoint.y, -visionPoint.z);
            AffineConverter.Xrotate(null, points, angle);
            AffineConverter.move(points, visionPoint.x, visionPoint.y, visionPoint.z);

            visionPoint = points[0];
            screenCenter = points[1];

            createCameraMatrix();
            transformCameraPoints(UVNMatrix);
            pyramidBase();
        }

        //поворот по Y
        public void YrotateCamera(int angle)
        {
            transformCameraPoints(inverseUVNMatrix);

            List<Point3D> points = new List<Point3D> { visionPoint, screenCenter };

            AffineConverter.move(points, -visionPoint.x, -visionPoint.y, -visionPoint.z);
            AffineConverter.Yrotate(null, points, angle);
            AffineConverter.move(points, visionPoint.x, visionPoint.y, visionPoint.z);

            visionPoint = points[0];
            screenCenter = points[1];

            createCameraMatrix();
            transformCameraPoints(UVNMatrix);
            pyramidBase();
        }

        //поворот по Z
        public void ZrotateCamera(int angle)
        {
            transformCameraPoints(inverseUVNMatrix);

            List<Point3D> points = new List<Point3D> { visionPoint, screenCenter };

            AffineConverter.move(points, -visionPoint.x, -visionPoint.y, -visionPoint.z);
            AffineConverter.Zrotate(null, points, angle);
            AffineConverter.move(points, visionPoint.x, visionPoint.y, visionPoint.z);

            visionPoint = points[0];
            screenCenter = points[1];

            createCameraMatrix();
            transformCameraPoints(UVNMatrix);
            pyramidBase();
        }

        private void transformCameraPoints(double[,] transformMatrix)
        {
            int[] coords = new int[4] { visionPoint.x, visionPoint.y, visionPoint.z, 1 };
            visionPoint = new Point3D(PointsColculator.matrixsMultiplication(coords, transformMatrix));

            coords = new int[4] { screenCenter.x, screenCenter.y, screenCenter.z, 1 };
            screenCenter = new Point3D(PointsColculator.matrixsMultiplication(coords, transformMatrix));
        }

        public void zoom(int length)
        {
            minDistance += length;
            screenCenter = new Point3D(0, 0, minDistance);

        }

        public bool isCentralProj()
        {
            return centralProjection;
        }
    }
}
