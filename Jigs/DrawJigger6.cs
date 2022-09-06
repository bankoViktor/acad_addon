using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadNetAddinWizard_Namespace
{
    public class DrawJigger6 : DrawJig
    {
        private Point3dCollection mAllVertexes = new Point3dCollection();
        private Point3d mLastVertex;

        public Point3d LastVertex
        {
            get { return mLastVertex; }
            set { mLastVertex = value; }
        }

        public Matrix3d UCS => AcadApp.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem;


        #region Overrides

        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
        {
            var geo = draw.Geometry;
            if (geo != null)
            {
                geo.PushModelTransform(UCS);

                var tempPts = new Point3dCollection();
                foreach (Point3d pt in mAllVertexes)
                {
                    tempPts.Add(pt);
                }

                if (mLastVertex != null)
                    tempPts.Add(mLastVertex);
                if (tempPts.Count > 0)
                    geo.Polyline(tempPts, Vector3d.ZAxis, IntPtr.Zero);

                geo.PopModelTransform();
            }

            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var prOptions1 = new JigPromptPointOptions("\nVertex(Enter to finish)")
            {
                UseBasePoint = false,
                UserInputControls =
                    UserInputControls.NullResponseAccepted |
                    UserInputControls.Accept3dCoordinates |
                    UserInputControls.GovernedByUCSDetect |
                    UserInputControls.GovernedByOrthoMode |
                    UserInputControls.AcceptMouseUpAsPoint,
            };

            var prResult1 = prompts.AcquirePoint(prOptions1);
            if (prResult1.Status == PromptStatus.Cancel || prResult1.Status == PromptStatus.Error)
                return SamplerStatus.Cancel;

            var tempPt = prResult1.Value.TransformBy(UCS.Inverse());
            mLastVertex = tempPt;

            return SamplerStatus.OK;
        }

        #endregion

        #region Commands

        [CommandMethod("TestDrawJigger6")]
        public static void TestDrawJigger6_Method()
        {
            var db = HostApplicationServices.WorkingDatabase;
            var doc = AcadApp.DocumentManager.MdiActiveDocument;

            try
            {
                var jigger = new DrawJigger6();
                PromptResult jigRes;

                do
                {
                    jigRes = doc.Editor.Drag(jigger);
                    if (jigRes.Status == PromptStatus.OK)
                        jigger.mAllVertexes.Add(jigger.mLastVertex);
                } while (jigRes.Status == PromptStatus.OK);

                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var btr = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

                    var ent = new Polyline();
                    ent.SetDatabaseDefaults();
                    for (int i = 0; i < jigger.mAllVertexes.Count; i++)
                    {
                        Point3d pt3d = jigger.mAllVertexes[i];
                        Point2d pt2d = new Point2d(pt3d.X, pt3d.Y);
                        ent.AddVertexAt(i, pt2d, 0, db.Plinewid, db.Plinewid);
                    }

                    ent.TransformBy(jigger.UCS);
                    btr.AppendEntity(ent);
                    tr.AddNewlyCreatedDBObject(ent, true);

                    tr.Commit();
                }
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage(ex.ToString());
            }
        }

        #endregion
    }
}