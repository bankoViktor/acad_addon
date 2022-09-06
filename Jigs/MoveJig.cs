using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace Addon.Jigs
{
    public class MoveJig : DrawJig
    {
        protected Point3d _basePt;
        protected Entity[] _entities;

        public Matrix3d Displacement { get; private set; }

        public MoveJig(Entity[] entities, Point3d basePt)
        {
            _entities = entities;
            _basePt = basePt;
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var options = new JigPromptPointOptions("\nSecond point: ")
            {
                UserInputControls = UserInputControls.Accept3dCoordinates,
                BasePoint = _basePt,
                UseBasePoint = true,
                Cursor = CursorType.RubberBand
            };
            var result = prompts.AcquirePoint(options);

            if (_basePt.DistanceTo(result.Value) < Tolerance.Global.EqualPoint)
                return SamplerStatus.NoChange;

            Displacement = Matrix3d.Displacement(result.Value - _basePt);

            return SamplerStatus.OK;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            var geo = draw.Geometry;
            if (geo != null)
            {
                geo.PushModelTransform(Displacement);
                foreach (var ent in _entities)
                {
                    geo.Draw(ent);
                }

                geo.PopModelTransform();
            }

            return true;
        }
    }
}
