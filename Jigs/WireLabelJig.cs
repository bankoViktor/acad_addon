using AcadNetAddinWizard_Namespace;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Addon.Jigs
{
    public class WireLabelJig : EntityJig
    {
        private Point3d _endPoint = Point3d.Origin;
        private Point3d _basePoint = Point3d.Origin;
        private double _baseAngle = 0;
        private Stage _stage = Stage.PlaceBase;

        private enum Stage
        { 
            PlaceBase = 1,
            OrientationBase = 2,
            OrientationAndLengthLine = 3,
        }

        public WireLabelJig(Entity entity) : base(entity)
        {

        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            switch (_stage)
            {
                case Stage.PlaceBase:
                    var jppo = new JigPromptPointOptions("\nInsertion base point: ")
                    {
                        //BasePoint = (Entity as Line).StartPoint,
                        //UseBasePoint = true,
                        UserInputControls =
                            UserInputControls.Accept3dCoordinates |
                            UserInputControls.AnyBlankTerminatesInput |
                            UserInputControls.GovernedByOrthoMode |
                            UserInputControls.GovernedByUCSDetect |
                            UserInputControls.UseBasePointElevation |
                            UserInputControls.InitialBlankTerminatesInput |
                            UserInputControls.NullResponseAccepted,
                    };
                    var ppr = prompts.AcquirePoint(jppo);
                    if (ppr.Status == PromptStatus.Cancel) return SamplerStatus.Cancel;

                    _basePoint = ppr.Value;
                    break;

                case Stage.OrientationBase:
                    var jpao = new JigPromptAngleOptions("\nSelect base orientation: ")
                    {
                        UseBasePoint = true,
                        BasePoint = _basePoint,
                        UserInputControls =
                            UserInputControls.Accept3dCoordinates |
                            UserInputControls.AnyBlankTerminatesInput |
                            UserInputControls.GovernedByOrthoMode |
                            UserInputControls.GovernedByUCSDetect |
                            UserInputControls.UseBasePointElevation |
                            UserInputControls.InitialBlankTerminatesInput |
                            UserInputControls.NullResponseAccepted,
                    };
                    var par = prompts.AcquireAngle(jpao);
                    if (par.Status == PromptStatus.Cancel) return SamplerStatus.Cancel;

                    _baseAngle = par.Value;
                    break;

                case Stage.OrientationAndLengthLine:
                    break;
            }

            

            return SamplerStatus.OK;
        }

        protected override bool Update()
        {
            //(Entity as Ellipse).Center = _endPoint;

            switch (_stage)
            {
                case Stage.PlaceBase:
                    break;

                case Stage.OrientationBase:
                    break;

                case Stage.OrientationAndLengthLine:
                    break;
            }

            return true;
        }

        public static bool Jig()
        {
            var ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
            var db = HostApplicationServices.WorkingDatabase;

            var pt = Point3d.Origin;

            var ent = new Ellipse(
                center: pt,
                unitNormal: Vector3d.ZAxis,
                majorAxis: -Vector3d.XAxis,
                radiusRatio: 0.5d,
                startAngle: DegreesToRadians(140),
                endAngle: DegreesToRadians(40)
            );

            var jigger = new WireLabelJig(ent);
            PromptResult pr;
            do
            {


                pr = ed.Drag(jigger);

            } while (pr.Status != PromptStatus.Cancel && pr.Status != PromptStatus.Error && jigger.NextStage());

            return false;
        }

        private static double DegreesToRadians(double degrees) => degrees / 180 * Math.PI;

        public bool NextStage() => (int)_stage++ <= 3;
    }
}
