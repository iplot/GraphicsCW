using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    public struct WallesParams
    {
        public int housingLength;
        public int handsLength;
        public int handsWidth;
        public int trunkWidth;
        public int trunkHeight;
        public int neckWidth;
        public int neckHeight;
        public int eyeCylinderHeight;
        public int trackCylinderRad;
        public int panelsNumber;
    }

    public class Walle
    {
        //Point3D basePoint;
        //в мировой системе
        Point3D viewPoint;
        Point3D screenCenter;

        Point3D basePoint;

        int borderCubeLeng;
        Point3D borderCubePoint;

        WallesParams param;
        List<Color> colors;

        // составляющие
        Housing housing;
        Hands hands;
        Neck neck;
        Trunk trunk;
        Eyes eyes;
        Track track;

        Panells panells;

        //возможно это будущий конструктор
        public Walle(Point3D basePoint, WallesParams param, Point3D viewPoint, Point3D screenCenter, List<Color> colors)
        {
            this.colors = new List<Color>(colors);
            this.param = param;
            //надо знать где мы создавали объект
            this.viewPoint = new Point3D(viewPoint);
            this.screenCenter = new Point3D(screenCenter);

            this.basePoint = basePoint;

            housing = new Housing(basePoint, param.housingLength, colors[0]);
            hands = new Hands(basePoint, param.housingLength, param.handsLength, param.handsWidth, colors[1]);
            neck = new Neck(basePoint, param.housingLength, param.neckWidth, param.neckHeight, colors[2]);
            trunk = new Trunk(basePoint, param.housingLength, param.trunkWidth, param.trunkHeight, colors[3]);
            eyes = new Eyes(basePoint, neck.getPoint2Eyes(), param.neckWidth, param.neckHeight, param.eyeCylinderHeight, param.housingLength, colors[4]);
            track = new Track(basePoint, param.housingLength, param.trackCylinderRad, colors[5]);

            panells = new Panells(basePoint, param.housingLength, param.panelsNumber, colors[6]);

            //75 - высота гусениц 100 - +50 свободного пространства в каждую сторону
            int l = param.housingLength + param.neckHeight + param.neckWidth / 2 + param.housingLength - 75;    //высота walle
            borderCubeLeng = (l + 100);

            borderCubePoint = new Point3D(basePoint.x - param.housingLength / 2 + borderCubeLeng/2, basePoint.y - param.housingLength / 2 + borderCubeLeng/2,
                basePoint.z + param.housingLength / 2 - borderCubeLeng/2);

        }

        public Point3D WallesViewPoint
        {
            get { return viewPoint; }
        }

        public Point3D WallesScreenCenter
        {
            get { return screenCenter; }
        }

        public Point3D BasePoint
        {
            get { return basePoint; }
        }

        public WallesParams Params
        {
            get { return param; }
        }

        public List<Color> Colors
        {
            get { return colors; }
        }

        public void moveWalle(int xleng, int yleng, int zleng)
        {
            basePoint.x += xleng;
            basePoint.y += yleng;
            basePoint.z += zleng;

            housing.moveHousing(xleng, yleng, zleng);
            hands.moveHands(xleng, yleng, zleng);
            neck.moveNeck(xleng, yleng, zleng);
            trunk.moveTrunk(xleng, yleng, zleng);
            eyes.moveEyes(xleng, yleng, zleng);
            track.moveTrack(xleng, yleng, zleng);

            panells.movePanells(xleng, yleng, zleng);
        }

        public void XrotateWalle(int angle)
        {
            housing.XrotateHousing(angle);
            hands.XrotateHands(angle);
            neck.XrotateNeck(angle);
            trunk.XrotateTrunk(angle);
            eyes.XrotateEyes(angle);
            track.XrotateTrack(angle);

            panells.XrotatePanells(angle);
        }

        public void YrotateWalle(int angle)
        {
            housing.YrotateHousing(angle);
            hands.YrotateHands(angle);
            neck.YrotateNeck(angle);
            trunk.YrotateTrunk(angle);
            eyes.YrotateEyes(angle);
            track.YrotateTrack(angle);

            panells.YrotatePanells(angle);
        }

        public void ZrotateWalle(int angle)
        {
            housing.ZrotateHousing(angle);
            hands.ZrotateHands(angle);
            neck.ZrotateNeck(angle);
            trunk.ZrotateTrunk(angle);
            eyes.ZrotateEyes(angle);
            track.ZrotateTrack(angle);

            panells.ZrotatePanells(angle);
        }

        public void scaleWalle(double scaleX, double scaleY, double scaleZ)
        {
            housing.scaleHousing(scaleX, scaleY, scaleZ);
            hands.scaleHands(scaleX, scaleY, scaleZ);
            neck.scaleNeck(scaleX, scaleY, scaleZ);
            trunk.scaleTrunk(scaleX, scaleY, scaleZ);
            eyes.scaleEyes(scaleX, scaleY, scaleZ);
            track.scaleTrack(scaleX, scaleY, scaleZ);

            panells.scalePanells(scaleX, scaleY, scaleZ);
        }

        //из координат камеры в координаты мира
        public void cameraCoordsAndWorld(double[,] transformMatrix)
        {
            housing.camera2world(transformMatrix);
            hands.camera2world(transformMatrix);
            neck.camera2world(transformMatrix);
            trunk.camera2world(transformMatrix);
            eyes.camera2world(transformMatrix);
            track.camera2world(transformMatrix);

            panells.camera2world(transformMatrix);
        }

        public List<List<Point3D>> getPoints()
        {
            List<List<Point3D>> points = new List<List<Point3D>>();

            points.Add(new List<Point3D>(housing.getPoints()));

            points.Add(new List<Point3D>(hands.getHand1()));
            points.Add(new List<Point3D>(hands.getHand2()));

            points.Add(new List<Point3D>(neck.getPoints()));

            points.Add(new List<Point3D>(trunk.getPoints()));

            points.AddRange(panells.getPoints());

            points.AddRange(eyes.getPoints());

            points.AddRange(track.getTrack1());
            points.AddRange(track.getTrack2());
            points.AddRange(track.getTrackTape());

            return points;
        }

        public List<List<bool[,]>> getTransmissionTabs()
        {
            List<List<bool[,]>> transmissionTabs = new List<List<bool[,]>>();

            transmissionTabs.Add(housing.getTransmissionTab());
            transmissionTabs.Add(hands.getTransmissionTab());
            transmissionTabs.Add(hands.getTransmissionTab());   //для второй руки
            transmissionTabs.Add(neck.getTransmissionTab());
            transmissionTabs.Add(trunk.getTransmissionTab());

            for (int i = 0; i < param.panelsNumber; i++)
                transmissionTabs.Add(panells.getTransmissionTab());

            transmissionTabs.Add(eyes.getTransmissionTab());

            return transmissionTabs;
        }

        public List<List<TrianglesIndexes>> getIndexes()
        {
            List<List<TrianglesIndexes>> indexes = new List<List<TrianglesIndexes>>();

            indexes.Add(housing.getIndexes());
            indexes.Add(hands.getIndexes());
            indexes.Add(hands.getIndexes());
            indexes.Add(neck.getIndexes());
            indexes.Add(trunk.getIndexes());

            for (int i = 0; i < param.panelsNumber; i++)
                indexes.Add(panells.getIndexes());

            indexes.Add(eyes.getIndexes());

            return indexes;
        }

        public List<Color> getColors()
        {
            List<Color> col = new List<Color>();

            col.AddRange(housing.getColor());
            col.AddRange(hands.getColor());
            col.AddRange(neck.getColor());
            col.AddRange(trunk.getColor());
            col.AddRange(panells.getColor());
            col.AddRange(eyes.getColor());
            col.AddRange(track.getColor());

            return col;
        }

        public int getPanellsCount()
        {
            return param.panelsNumber;
        }
    }
}
