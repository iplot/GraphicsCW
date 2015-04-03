using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    class Eyes
    {
        Point3D startPoint;

        Point3D eyeCenter;
        Point3D secondEyeCenter;
        Point3D backEyeCenter;
        Point3D secondBackEyeCenter;

        int partsCircle;
        int rad;
        int length;

        List<Point3D> eye;
        List<Point3D> secondEye;

        List<Point3D> backEye;
        List<Point3D> secondBackEye;

        List<Point3D> down;

        Color color;

        static /*const*/ List<TrianglesIndexes> triangleVertex = new List<TrianglesIndexes>{
            new TrianglesIndexes(0,1,2), new TrianglesIndexes(0,3,2)};

        static bool[,] transmissionMatrix = new bool[4, 4] { { false, true, true, true }, { true, false, true, false }, 
        { true, true, false, true }, { true, false, true, false } };

        public Eyes(Point3D basePoint, Point3D point2Eyes, int neckWidth, int neckHeight, int eyeLength, int boxLength, Color col)
        {
            color = col;

            startPoint = new Point3D(basePoint.x - boxLength/2, basePoint.y - boxLength/2, basePoint.z + boxLength/2);
            eye = new List<Point3D>();
            down = new List<Point3D>();

            
            
            rad = neckWidth/4;

            eyeCenter = new Point3D(point2Eyes.x - rad, point2Eyes.y - rad, point2Eyes.z - eyeLength / 2);
            secondEyeCenter = new Point3D(eyeCenter.x - 2 * rad, eyeCenter.y, eyeCenter.z);
            backEyeCenter = new Point3D(eyeCenter.x, eyeCenter.y, eyeCenter.z + length + 30);   //+30 для конуса (просто смещен центр)
            secondBackEyeCenter = new Point3D(eyeCenter.x - 2 * rad, eyeCenter.y, eyeCenter.z + length + 30);


            partsCircle = 24;
            length = eyeLength;

            makeDown(point2Eyes, neckWidth);
            makeEyesCylinders(point2Eyes, neckWidth, neckHeight, eyeLength);

        }

        //подставка для глаз
        private void makeDown(Point3D point2Eyes, int neckWidth)
        {
            Point3D mainP = new Point3D(point2Eyes.x, point2Eyes.y, point2Eyes.z - 3);
            down.Add(mainP);
            down.Add(new Point3D(mainP.x, mainP.y, mainP.z + 6));
            down.Add(new Point3D(mainP.x - neckWidth, mainP.y, mainP.z + 6));
            down.Add(new Point3D(mainP.x - neckWidth, mainP.y, mainP.z));
        }

        //цилиндры глаз
        private void makeEyesCylinders(Point3D point2Eyes, int neckWidth, int neckHeight, int eyeLength)
        {
            eye.Add(new Point3D(eyeCenter.x, eyeCenter.y - rad, eyeCenter.z));

            int angle = 360 / partsCircle;    //a = 2*Pi/N
            for (int i = 1; i <= partsCircle; i++)
            {
                //Point3D newPoint = new Point3D(eye[i].x, eye[i].y, eye[i].z);
                Point3D newPoint = new Point3D(eye[0].x, eye[0].y, eye[0].z);
                List<Point3D> newPointList = new List<Point3D> { newPoint };
                int ang = angle * i;

                AffineConverter.move(newPointList, -eyeCenter.x, -eyeCenter.y, -eyeCenter.z);
                AffineConverter.Zrotate(null, newPointList, ang);
                AffineConverter.move(newPointList, eyeCenter.x, eyeCenter.y, eyeCenter.z);

                eye.Add(newPointList[0]);
            }

            secondEye = new List<Point3D>(eye);
            AffineConverter.move(secondEye, -rad * 2, 0, 0);
            //AffineConverter.move(secondEye, -startPoint.x, -startPoint.y, -startPoint.z);
            //AffineConverter.Xmirror(secondEye, rad * 2);
            //AffineConverter.move(secondEye, startPoint.x, startPoint.y, startPoint.z);

            backEye = new List<Point3D>(eye);
            AffineConverter.move(backEye, 0, 0, eyeLength);

            secondBackEye = new List<Point3D>(secondEye);
            AffineConverter.move(secondBackEye, 0, 0, eyeLength);
        }

        private void addCentersToList()
        {
            eye.Add(eyeCenter);
            secondEye.Add(secondEyeCenter);
            backEye.Add(backEyeCenter);
            secondBackEye.Add(secondBackEyeCenter);
        }

        private void removeCentersToList()
        {
            eyeCenter = eye[eye.Count - 1];
            secondEyeCenter = secondEye[secondEye.Count - 1];
            backEyeCenter = backEye[backEye.Count - 1];
            secondBackEyeCenter = secondBackEye[secondBackEye.Count - 1];

            eye.Remove(eye[eye.Count - 1]);
            secondEye.Remove(secondEye[secondEye.Count - 1]);
            backEye.Remove(backEye[backEye.Count - 1]);
            secondBackEye.Remove(secondBackEye[secondBackEye.Count - 1]);
        }

        public void moveEyes(int xleng, int yleng, int zleng)
        {
            startPoint.x += xleng;
            startPoint.y += yleng;
            startPoint.z += zleng;

            addCentersToList();

            AffineConverter.move(down, xleng, yleng, zleng);
            AffineConverter.move(eye, xleng, yleng, zleng);
            AffineConverter.move(backEye, xleng, yleng, zleng);
            AffineConverter.move(secondEye, xleng, yleng, zleng);
            AffineConverter.move(secondBackEye, xleng, yleng, zleng);

            removeCentersToList();

        }

        public void XrotateEyes(int angle)
        {
            AffineConverter.move(down, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, down, angle);
            AffineConverter.move(down, startPoint.x, startPoint.y, startPoint.z);

            addCentersToList();

            AffineConverter.move(eye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, eye, angle);
            AffineConverter.move(eye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, secondEye, angle);
            AffineConverter.move(secondEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(backEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, backEye, angle);
            AffineConverter.move(backEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondBackEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Xrotate(null, secondBackEye, angle);
            AffineConverter.move(secondBackEye, startPoint.x, startPoint.y, startPoint.z);

            removeCentersToList();
        }

        public void YrotateEyes(int angle)
        {
            AffineConverter.move(down, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, down, angle);
            AffineConverter.move(down, startPoint.x, startPoint.y, startPoint.z);

            addCentersToList();

            AffineConverter.move(eye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, eye, angle);
            AffineConverter.move(eye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, secondEye, angle);
            AffineConverter.move(secondEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(backEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, backEye, angle);
            AffineConverter.move(backEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondBackEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Yrotate(null, secondBackEye, angle);
            AffineConverter.move(secondBackEye, startPoint.x, startPoint.y, startPoint.z);

            removeCentersToList();
        }

        public void ZrotateEyes(int angle)
        {
            AffineConverter.move(down, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, down, angle);
            AffineConverter.move(down, startPoint.x, startPoint.y, startPoint.z);

            addCentersToList();

            AffineConverter.move(eye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, eye, angle);
            AffineConverter.move(eye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, secondEye, angle);
            AffineConverter.move(secondEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(backEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, backEye, angle);
            AffineConverter.move(backEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondBackEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.Zrotate(null, secondBackEye, angle);
            AffineConverter.move(secondBackEye, startPoint.x, startPoint.y, startPoint.z);

            removeCentersToList();
        }

        public void scaleEyes(double scaleX, double scaleY, double scaleZ)
        {
            AffineConverter.move(down, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(down, scaleX, scaleY, scaleZ);
            AffineConverter.move(down, startPoint.x, startPoint.y, startPoint.z);

            addCentersToList();

            AffineConverter.move(eye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(eye, scaleX, scaleY, scaleZ);
            AffineConverter.move(eye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(secondEye, scaleX, scaleY, scaleZ);
            AffineConverter.move(secondEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(backEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(backEye, scaleX, scaleY, scaleZ);
            AffineConverter.move(backEye, startPoint.x, startPoint.y, startPoint.z);

            AffineConverter.move(secondBackEye, -startPoint.x, -startPoint.y, -startPoint.z);
            AffineConverter.scale(secondBackEye, scaleX, scaleY, scaleZ);
            AffineConverter.move(secondBackEye, startPoint.x, startPoint.y, startPoint.z);

            removeCentersToList();
        }

        //не проверялась!
        public void camera2world(double[,] transformMatrix)
        {
            int[] cords;
            int[] result;

            //нижняя часть
            for (int i = 0; i < down.Count; i++)
            {
                cords = new int[4] { down[i].x, down[i].y, down[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

                down[i] = new Point3D(result);
            }

            addCentersToList();

            for (int i = 0; i < eye.Count; i++)
            {
                cords = new int[4] { eye[i].x, eye[i].y, eye[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                eye[i] = new Point3D(result);

                cords = new int[4] { secondEye[i].x, secondEye[i].y, secondEye[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                secondEye[i] = new Point3D(result);

                cords = new int[4] { backEye[i].x, backEye[i].y, backEye[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                backEye[i] = new Point3D(result);

                cords = new int[4] { secondBackEye[i].x, secondBackEye[i].y, secondBackEye[i].z, 1 };
                result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                secondBackEye[i] = new Point3D(result);
            }

            removeCentersToList();

            cords = new int[4] { startPoint.x, startPoint.y, startPoint.z, 1 };
            result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

            startPoint = new Point3D(result);
        }

        public List<List<Point3D>> getPoints()
        {
            List<List<Point3D>> points = new List<List<Point3D>>();
            points.Add(new List<Point3D>(down));

            addCentersToList();

            points.Add(new List<Point3D>(eye));
            points.Add(new List<Point3D>(backEye));
            points.Add(new List<Point3D>(secondEye));
            points.Add(new List<Point3D>(secondBackEye));

            removeCentersToList();

            return points;
        }

        public List<bool[,]> getTransmissionTab()
        {
            return new List<bool[,]> { transmissionMatrix };
        }

        public List<TrianglesIndexes> getIndexes()
        {
            return new List<TrianglesIndexes>(triangleVertex);
        }

        public List<Color> getColor()
        {
            return new List<Color> { color, color, color, color, color };
        }
    }
}
