using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.IO;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Addon.Infrastructure
{
    public static class Utility
    {
        private const string OrthModeVarName = "ORTHOMODE";

        public const double Inch_1_2 = 0.5d;
        public const double Inch_1_4 = 0.25d;
        public const double Inch_1_8 = 0.125d;
        public const double Inch_1_16 = 0.0625d;
        public const double Inch_1_32 = 0.03125d;
        public const double Inch_1_64 = 0.015625d;
        public static void AddBlockDefinitionFromDwg(Database db, string blockPath)
        {
            var blkDb = new Database(false, true);
            blkDb.ReadDwgFile(blockPath, FileShare.Read, true, "");
            var blockName = Path.GetFileNameWithoutExtension(blockPath).ToUpper();
            db.Insert(blockName, blkDb, true);
        }

        public static double DegreesToRadians(double degrees) => degrees / 180 * Math.PI;

        public static bool IsOrthogonal
        {
            get
            {
                var value = AcadApp.GetSystemVariable(OrthModeVarName);
                return int.TryParse(value.ToString(), out int result) && result > 0;
            }
        }

        public static Point3d GetOrthoPoint(Point3d basePt, Point3d pt)
        {
            // Apply an orthographic mode
            double x = pt.X;
            double y = pt.Y;
            double z = pt.Z;

            var vec = basePt.GetVectorTo(pt);
            var absX = Math.Abs(vec.X);
            var absY = Math.Abs(vec.Y);
            var absZ = Math.Abs(vec.Z);

            if (absX < absY && absX < absZ)
                x = basePt.X;
            else if (absY < absX && absY < absZ)
                y = basePt.Y;
            else
                z = pt.Z;

            return new Point3d(x, y, z);
        }
    }
}
