using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    class SceneCodeParser
    {
        private static List<String> segments;

        private static Camera cam;
        private static Drawer draw;
        private static List<Walle> walles;

        public static bool wireMod;

        public static void parse(String sceneCode, int sceneWidth, int sceneHeight)
        {
            segments = new List<string>();

            List<String> search = new List<string> { "<Camera>", "</Camera>", "<Drawer>", "</Drawer>"};

            for (int i = 0; i < search.Count - 1; i += 2)
            {
                int firstInd = sceneCode.IndexOf(search[i]) + search[i].Length ;
                int secondInd = sceneCode.IndexOf(search[i + 1]);

                String segment = sceneCode.Substring(firstInd, secondInd - firstInd);
                segments.Add(segment);
            }

            int pos = 0;
            while (sceneCode.IndexOf("<Walle>", pos) != -1)
            {
                int firstInd = sceneCode.IndexOf("<Walle>", pos) + "Walle".Length;
                pos = sceneCode.IndexOf("</Walle>", pos);

                String segment = sceneCode.Substring(firstInd, pos - firstInd);
                pos += "</Walle>".Length;
                segments.Add(segment);
            }

            cameraMaker(segments[0], sceneWidth, sceneHeight);
            drawerMaker(segments[1], sceneWidth, sceneHeight);

            walles = new List<Walle>();
            for(int i = 2; i < segments.Count; i++)
                walleMaker(segments[i], sceneWidth, sceneHeight);
        }

        private static void cameraMaker(String segment, int sceneWidth, int sceneHeight)
        {
            Point3D visionP = null;
            Point3D screenP = null;
            int horizAng = 0;
            int verticAng = 0;
            int minDist = 0;
            bool centralProj;

            String str = getStr(segment, "<VisionPoint>", "</VisionPoint");
            if (!str.Equals(""))
               visionP = pointsMaker(str);

            str = getStr(segment, "<ScreenCenter>", "</ScreenCenter");
            if (!str.Equals(""))
                screenP = pointsMaker(str);

            str = getStr(segment, "<HorizontalAngle>", "</HorizontalAngle");
            if (!str.Equals(""))
                horizAng = Convert.ToInt32(str);

            str = getStr(segment, "<VerticalAngle>", "</VerticalAngle");
            if (!str.Equals(""))
                verticAng = Convert.ToInt32(str);

            str = getStr(segment, "<MinDistance>", "</MinDistance");
            if (!str.Equals(""))
                minDist = Convert.ToInt32(str);

            str = getStr(segment, "<CentralProjection>", "</CentralProjection");
            if (!str.Equals(""))
                centralProj = Convert.ToBoolean(str);

            str = getStr(segment, "<WireMod>", "</WireMod");
            if (!str.Equals(""))
                wireMod = Convert.ToBoolean(str);

            cam = new Camera(horizAng, verticAng, sceneWidth, sceneHeight);
            cam.setPoints(visionP, screenP);
            cam.MinDistance = minDist;
        }

        private static void drawerMaker(String segment, int sceneWidth, int sceneHeight)
        {
            bool mode = true;

            String str = getStr(segment, "<UnFillMode>", "</UnFillMode>");
            if (!str.Equals(""))
                mode = Convert.ToBoolean(str);

            draw = new Drawer(sceneWidth, sceneHeight);
            draw.UnFillMode = mode;
        }

        private static void walleMaker(String segment, int sceneWidth, int sceneHeight)
        {
            Point3D walleVisionPoint = null;
            Point3D walleScreenCenter = null;
            Point3D walleBasePoint = null;
            WallesParams param = new WallesParams();

            String str = getStr(segment, "<WalleVisionPoint>", "</WalleVisionPoint");
            if (!str.Equals(""))
                walleVisionPoint = pointsMaker(str);

            str = getStr(segment, "<WalleScreenCenter>", "</WalleScreenCenter");
            if (!str.Equals(""))
                walleScreenCenter = pointsMaker(str);

            str = getStr(segment, "<BasePoint>", "</BasePoint");
            if (!str.Equals(""))
                walleBasePoint = pointsMaker(str);

            str = getStr(segment, "<HousingLength>", "</HousingLength");
            if (!str.Equals(""))
                param.housingLength = Convert.ToInt32(str);

            str = getStr(segment, "<HandsWidth>", "</HandsWidth");
            if (!str.Equals(""))
                param.handsWidth = Convert.ToInt32(str);

            str = getStr(segment, "<HandsLength>", "</HandsLength");
            if (!str.Equals(""))
                param.handsLength = Convert.ToInt32(str);

            str = getStr(segment, "<TrunkWidth>", "</TrunkWidth");
            if (!str.Equals(""))
                param.trunkWidth = Convert.ToInt32(str);

            str = getStr(segment, "<TrunkHeight>", "</TrunkHeight");
            if (!str.Equals(""))
                param.trunkHeight = Convert.ToInt32(str);

            str = getStr(segment, "<NeckWidth>", "</NeckWidth");
            if (!str.Equals(""))
                param.neckWidth = Convert.ToInt32(str);

            str = getStr(segment, "<NeckHeight>", "</NeckHeight");
            if (!str.Equals(""))
                param.neckHeight = Convert.ToInt32(str);

            str = getStr(segment, "<TrackCylinderRad>", "</TrackCylinderRad");
            if (!str.Equals(""))
                param.trackCylinderRad = Convert.ToInt32(str);

            str = getStr(segment, "<EyeCylinderHeight>", "</EyeCylinderHeight");
            if (!str.Equals(""))
                param.eyeCylinderHeight = Convert.ToInt32(str);

            str = getStr(segment, "<PanelsNumber>", "</PanelsNumber");
            if (!str.Equals(""))
                param.panelsNumber = Convert.ToInt32(str);

            List<Color> colors = new List<Color>();

            str = getStr(segment, "<HousingColor>", "</HousingColor>");
            if (!str.Equals(""))
                colors.Add(ColorTranslator.FromHtml(str));

            str = getStr(segment, "<HandsColor>", "</HandsColor>");
            if (!str.Equals(""))
                colors.Add(ColorTranslator.FromHtml(str));

            str = getStr(segment, "<NeckColor>", "</NeckColor>");
            if (!str.Equals(""))
                colors.Add(ColorTranslator.FromHtml(str));

            str = getStr(segment, "<TrunkColor>", "</TrunkColor>");
            if (!str.Equals(""))
                colors.Add(ColorTranslator.FromHtml(str));

            str = getStr(segment, "<PanellsColor>", "</PanellsColor>");
            if (!str.Equals(""))
                colors.Add(ColorTranslator.FromHtml(str));

            str = getStr(segment, "<EyesColor>", "</EyesColor>");
            if (!str.Equals(""))
                colors.Add(ColorTranslator.FromHtml(str));

            str = getStr(segment, "<TracksColor>", "</TracksColor>");
            if (!str.Equals(""))
                colors.Add(ColorTranslator.FromHtml(str));

            walleBasePoint.x -= sceneWidth / 2;
            walleBasePoint.y -= sceneHeight / 2;
            Walle w = new Walle(walleBasePoint, param, walleVisionPoint, walleScreenCenter, colors);
            walles.Add(w);
        }

        private static String getStr(String segment, String str1, String str2)
        {
            String str = "";

            int ind1 = segment.IndexOf(str1);
            int ind2 = segment.IndexOf(str2);

            if (ind1 != -1 && ind2 != -1)
            {
                str = segment.Substring(ind1 + str1.Length, ind2 - (ind1 + str1.Length));
                while (str[0].Equals(' '))
                    str = str.Remove(0);
            }

            return str;
        }

        private static Point3D pointsMaker(String str)
        {
            int ind = str.IndexOf(" ");

            int x = Convert.ToInt32(str.Substring(0, ind));

            int ind2 = str.IndexOf(" ", ind + 1);

            int y = Convert.ToInt32(str.Substring(ind + 1, ind2 - (ind + 1) ));

            int z = Convert.ToInt32(str.Substring(ind2 + 1));

            Point3D p = new Point3D(x, y, z);

            return p;
        }

        public static Camera getCamera()
        {
            return cam;
        }

        public static Drawer getDrawer()
        {
            return draw;
        }

        public static List<Walle> getWalles()
        {
            return walles;
        }
    }
}
