using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;
using MgdAcApplication = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AcadNetAddinWizard_Namespace
{
    public class DrawJigger7 : DrawJig
    {
        #region Fields

        private Point3d mBase;
        private Point3d mLocation;
        List<Entity> mEntities;

        #endregion

        #region Constructors

        public DrawJigger7(Point3d basePt)
        {
            mBase = basePt.TransformBy(UCS);
            mEntities = new List<Entity>();
        }

        #endregion

        #region Properties

        public Point3d Base
        {
            get { return mLocation; }
            set { mLocation = value; }
        }

        public Point3d Location
        {
            get { return mLocation; }
            set { mLocation = value; }
        }

        public Matrix3d UCS
        {
            get
            {
                return MgdAcApplication.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem;
            }
        }

        #endregion

        #region Methods

        public void AddEntity(Entity ent)
        {
            mEntities.Add(ent);
        }

        public void TransformEntities()
        {
            Matrix3d mat = Matrix3d.Displacement(mBase.GetVectorTo(mLocation));

            foreach (Entity ent in mEntities)
            {
                ent.TransformBy(mat);
            }
        }

        #endregion

        #region Overrides

        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
        {
            Matrix3d mat = Matrix3d.Displacement(mBase.GetVectorTo(mLocation));

            WorldGeometry geo = draw.Geometry;
            if (geo != null)
            {
                geo.PushModelTransform(mat);

                foreach (Entity ent in mEntities)
                {
                    geo.Draw(ent);
                }

                geo.PopModelTransform();
            }

            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions prOptions1 = new JigPromptPointOptions("\nNew location:");
            prOptions1.UseBasePoint = false;

            PromptPointResult prResult1 = prompts.AcquirePoint(prOptions1);
            if (prResult1.Status == PromptStatus.Cancel || prResult1.Status == PromptStatus.Error)
                return SamplerStatus.Cancel;

            if (!mLocation.IsEqualTo(prResult1.Value, new Tolerance(10e-10, 10e-10)))
            {
                mLocation = prResult1.Value;
                return SamplerStatus.OK;
            }
            else
                return SamplerStatus.NoChange;
        }

        #endregion

        #region Commands

        public static DrawJigger7 jigger;

        [CommandMethod("TestDrawJigger7")]
        public static void TestDrawJigger7_Method()
        {
            try
            {
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = MgdAcApplication.DocumentManager.MdiActiveDocument.Editor;

                PromptSelectionResult selRes = ed.GetSelection();
                if (selRes.Status != PromptStatus.OK) return;

                PromptPointOptions prOpt = new PromptPointOptions("\nBase point:");
                PromptPointResult pr = ed.GetPoint(prOpt);
                if (pr.Status != PromptStatus.OK) return;

                jigger = new DrawJigger7(pr.Value);
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in selRes.Value.GetObjectIds())
                    {
                        Entity ent = (Entity)tr.GetObject(id, OpenMode.ForWrite);
                        jigger.AddEntity(ent);
                    }

                    PromptResult jigRes = MgdAcApplication.DocumentManager.MdiActiveDocument.Editor.Drag(jigger);
                    if (jigRes.Status == PromptStatus.OK)
                    {
                        jigger.TransformEntities();
                        tr.Commit();
                    }
                    else
                        tr.Abort();
                }
            }
            catch (System.Exception ex)
            {
                MgdAcApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.ToString());
            }
        }

        #endregion
    }
}