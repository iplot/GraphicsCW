using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    public class Zcomponent
    {
        public double z = 540;
        public Color color = Color.White;
    }

    class Drawer
    {
        int screenWidth;
        int screenHeight;

        byte[] canvas;

        bool unfillmod = true;

        Zcomponent[][] zbuffer;

        public Drawer(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            update();
        }

        public void update()
        {
            canvas = new byte[3 * screenWidth * screenHeight];

            for (int i = 0; i < canvas.Length; i++)
                canvas[i] = 255;

            //создание z буфера
            zbuffer = new Zcomponent[screenHeight][];
            for (int i = 0; i < zbuffer.Length; i++)
            {
                zbuffer[i] = new Zcomponent[screenWidth];
                for (int j = 0; j < zbuffer[i].Length; j++)
                    zbuffer[i][j] = new Zcomponent();
            }
        }

        public bool UnFillMode
        {
            get { return unfillmod; }
            set { unfillmod = value; }
        }

        public void drawObjectWithTable(List<Point3D> points, List<bool[,]> transmission, Color col)
        {
            int koef = 0;

            for (int m = 0; m < transmission.Count; m++)    //для каждой карты
            {
                for (int i = 0; i < Math.Sqrt(transmission[m].Length) - 1; i++)
                {
                    if (points[i + koef] == null)
                        continue;

                    for (int j = i + 1; j < Math.Sqrt(transmission[m].Length); j++)
                    {
                        if (points[j + koef] == null)
                            continue;

                        if (transmission[m][i, j])
                        {
                            Result result = PointsColculator.linePoints(points[i + koef].x, points[i + koef].y, points[j + koef].x, points[j + koef].y);

                            for (int k = 0; k < result.length; k++)
                            {
                                int index = (result.y[k] * screenWidth + result.x[k]) * 3;

                                //canvas[index] = col.R;
                                //canvas[index + 1] = col.G;
                                //canvas[index + 2] = col.B;
                                canvas[index] = col.B;
                                canvas[index + 1] = col.G;
                                canvas[index + 2] = col.R;
                            }

                        }
                    }
                }

                koef += (int)Math.Sqrt(transmission[m].Length); //отступ (равен количеству строк в таблице перехода)
            }
        }


        public void drawRotationObject(List<Point3D> base1, List<Point3D> base2, bool flag, Color col)
        {
            if (flag)
            {
                Point3D center1 = (base1[base1.Count - 1] != null) ? new Point3D(base1[base1.Count - 1]) : null;
                base1.Remove(base1[base1.Count - 1]);

                Point3D center2 = (base2[base2.Count - 1] != null) ? new Point3D(base2[base2.Count - 1]) : null;
                base2.Remove(base1[base1.Count - 1]);

                //окружности
                if(center1 != null)
                    drawCylindersBases(center1, base1, col);

                if(center2 != null)
                    drawCylindersBases(center2, base2, col);
            }

            //соединение окружностей
            drawCylinders(base1, base2, col);
        }

        private void drawCylindersBases(Point3D center, List<Point3D> points, Color col)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point3D p1 = points[i];
                Point3D p2 = points[i + 1];

                if (p1 == null || p2 == null)
                    continue;

                List<Result> results = new List<Result>{PointsColculator.linePoints(center.x, center.y, p1.x, p1.y), PointsColculator.linePoints(center.x, center.y, p2.x, p2.y),
                    PointsColculator.linePoints(p1.x, p1.y, p2.x, p2.y)};

                for (int j = 0; j < results.Count - 1; j++)
                {
                    Result res = results[j];

                    for (int m = 0; m < res.length; m++)
                    {
                        int index = (res.y[m] * screenWidth + res.x[m]) * 3;

                        if (index >= 0)
                        {
                            //canvas[index] = col.R;
                            //canvas[index + 1] = col.G;
                            //canvas[index + 2] = col.B;
                            canvas[index] = col.B;
                            canvas[index + 1] = col.G;
                            canvas[index + 2] = col.R;
                        }
                    }
                }
            }
        }

        private void drawCylinders(List<Point3D> cyl, List<Point3D> backCyl, Color col)
        {
            for (int i = 0; i < cyl.Count - 1; i++)
            {
                int j = i + 1;

                if (cyl[i] == null || cyl[j] == null || backCyl[i] == null || backCyl[j] == null)
                    continue;


                List<Result> list = new List<Result>{PointsColculator.linePoints(cyl[i].x, cyl[i].y, backCyl[i].x, backCyl[i].y),
                    PointsColculator.linePoints(cyl[j].x, cyl[j].y, backCyl[j].x, backCyl[j].y),
                    PointsColculator.linePoints(cyl[j].x, cyl[j].y, backCyl[i].x, backCyl[i].y)};

                for (int k = 0; k < list.Count; k++)
                {
                    Result res = list[k];

                    for (int m = 0; m < res.length; m++)
                    {
                        int index = (res.y[m] * screenWidth + res.x[m]) * 3;

                        if (index >= 0)
                        {
                            //canvas[index] = col.R;
                            //canvas[index + 1] = col.G;
                            //canvas[index + 2] = col.B;
                            canvas[index] = col.B;
                            canvas[index + 1] = col.G;
                            canvas[index + 2] = col.R;
                        }
                    }
                }
            }
        }


        public byte[] getCanvas()
        {
            return canvas;
        }

        public void drawbuf()
        {
            for(int i = 0; i < zbuffer.Length; i++)
                for (int j = 0; j < zbuffer[i].Length; j++)
                {
                    int ind = (i*screenWidth + j)*3;
                    canvas[ind] = zbuffer[i][j].color.B;
                    canvas[ind + 1] = zbuffer[i][j].color.G;
                    canvas[ind + 2] = zbuffer[i][j].color.R;
                }
        }

        public void zBuffer(List<Triangle> triangles, bool centralProj)
        {
            for (int k = 0; k < triangles.Count; k++)
            {

                int[] koefs = PointsColculator.surfaceKoefs(triangles[k].getCameraPoint(0), triangles[k].getCameraPoint(1), triangles[k].getCameraPoint(2));

                //линии треугольника
                Result line1 = PointsColculator.linePoints(triangles[k].getScreenPoint(0).x, triangles[k].getScreenPoint(0).y,
                    triangles[k].getScreenPoint(1).x, triangles[k].getScreenPoint(1).y);

                List<Result> inTrianglePoints = fill(line1, triangles[k].getScreenPoint(2).x, triangles[k].getScreenPoint(2).y);


                for (int i = 0; i < inTrianglePoints.Count; i++)
                {
                    //выбор цвета заливки
                    Color fon;
                    if (unfillmod == true)
                        fon = Color.White;
                    else
                        fon = triangles[k].TriangleColor;

                    //проверка и установка значений в з-буффере 
                    setBuffer(inTrianglePoints[i], centralProj, koefs, triangles[k].getScreenPoint(0).z, fon, triangles[k].getAverageZ());
                }

                if (unfillmod)
                {
                    //прогонка сторон треугольника для удаления скрытых линий
                    Result l1 = PointsColculator.linePoints(triangles[k].getScreenPoint(0).x, triangles[k].getScreenPoint(0).y,
                    triangles[k].getScreenPoint(1).x, triangles[k].getScreenPoint(1).y);

                    Result l2 = PointsColculator.linePoints(triangles[k].getScreenPoint(0).x, triangles[k].getScreenPoint(0).y,
                    triangles[k].getScreenPoint(2).x, triangles[k].getScreenPoint(2).y);

                    Result l3 = PointsColculator.linePoints(triangles[k].getScreenPoint(2).x, triangles[k].getScreenPoint(2).y,
                    triangles[k].getScreenPoint(1).x, triangles[k].getScreenPoint(1).y);

                    setBuffer(l1, centralProj, koefs, triangles[k].getScreenPoint(0).z, triangles[k].TriangleColor, triangles[k].getAverageZ());
                    setBuffer(l2, centralProj, koefs, triangles[k].getScreenPoint(0).z, triangles[k].TriangleColor, triangles[k].getAverageZ());
                    setBuffer(l3, centralProj, koefs, triangles[k].getScreenPoint(0).z, triangles[k].TriangleColor, triangles[k].getAverageZ());
                }
            }
        }

        private void setBuffer(Result line, bool centralProj, int[] koefs, int screenz, Color col, double triangz)
        {
            for (int j = 0; j < line.length; j++)
            {
                int[] p1;
                if (centralProj)
                    p1 = new int[3] { 0, 0, 0 };
                else
                    p1 = new int[3] { line.x[j] - screenWidth / 2, line.y[j] - screenHeight / 2, 0 };

                int[] p2 = new int[3] { line.x[j] - screenWidth / 2, line.y[j] - screenHeight / 2, screenz };

                double num = 0; double den = 0;
                for (int h = 0; h < p1.Length; h++)
                {
                    num += -koefs[h] * p1[h];
                    den += koefs[h] * (p2[h] - p1[h]);
                }
                num += -koefs[3];

                double t = num / den;

                double z = Math.Round(t * (p2[2] - p1[2]) + p1[2]);

                //!!!!
                if (num == 0)
                    //z = 0;
                    z = triangz;

                if (z <= zbuffer[line.y[j]][line.x[j]].z)
                {
                    zbuffer[line.y[j]][line.x[j]].z = z;
                    //указываем цвет
                    zbuffer[line.y[j]][line.x[j]].color = col;
                }
            }
        }

        private bool checkZbuffer(Point3D p1, Point3D p2, Point3D p3)
        {
            if (p1.z > zbuffer[p1.y + screenHeight/2][p1.x + screenWidth/2].z && p2.z > zbuffer[p2.y + screenHeight/2][p2.x + screenWidth/2].z 
                && p3.z > zbuffer[p3.y + screenHeight/2][p3.x + screenWidth/2].z)
                return false;

            return true;
        }

        private List<Result> fill(Result line1, int x, int y)
        {
            List<Result> results = new List<Result>();

            for (int i = 0; i < line1.length; i++)
                results.Add(PointsColculator.linePoints(line1.x[i], line1.y[i], x, y));

            return results;
        }

    }

    class CompareLines : IComparer<Result>
    {
        public int Compare(Result res1, Result res2)
        {
            return res1.length - res2.length;
        }
    }
}
