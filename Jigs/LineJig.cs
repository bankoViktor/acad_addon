using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;


namespace AcadNetAddinWizard_Namespace
{
    public class LineJig : EntityJig
    {
        public Point3d _endPoint = new Point3d();

        public LineJig(Line ent) : base(ent)
        {
        }

        protected override bool Update()
        {
            (Entity as Line).EndPoint = _endPoint;

            return true;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var prOptions1 = new JigPromptPointOptions("\nNext point:")
            {
                BasePoint = (Entity as Line).StartPoint,
                UseBasePoint = true,
                UserInputControls = UserInputControls.Accept3dCoordinates | UserInputControls.AnyBlankTerminatesInput
                | UserInputControls.GovernedByOrthoMode | UserInputControls.GovernedByUCSDetect | UserInputControls.UseBasePointElevation
                | UserInputControls.InitialBlankTerminatesInput | UserInputControls.NullResponseAccepted,
            };

            var prResult1 = prompts.AcquirePoint(prOptions1);
            if (prResult1.Status == PromptStatus.Cancel) return SamplerStatus.Cancel;

            if (prResult1.Value.Equals(_endPoint))
            {
                return SamplerStatus.NoChange;
            }
            else
            {
                _endPoint = prResult1.Value;
                return SamplerStatus.OK;
            }
        }

        public static bool Jig()
        {
            try
            {
                var ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
                var db = HostApplicationServices.WorkingDatabase;

                var ppr = ed.GetPoint("\nStart point");
                if (ppr.Status != PromptStatus.OK) return false;

                var pt = ppr.Value;
                var ent = new Line(pt, pt);
                ent.TransformBy(AcadApp.DocumentManager.MdiActiveDocument.Editor.CurrentUserCoordinateSystem);
                var jigger = new LineJig(ent);
                var pr = ed.Drag(jigger);

                if (pr.Status == PromptStatus.OK)
                {
                    using (var tr = db.TransactionManager.StartTransaction())
                    {
                        var bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                        var btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        btr.AppendEntity(jigger.Entity);
                        tr.AddNewlyCreatedDBObject(jigger.Entity, true);
                        tr.Commit();
                    }
                }
                else
                {
                    ent.Dispose();
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}