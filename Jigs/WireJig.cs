using Addon.Infrastructure;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using System;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Addon.Jigs
{
    public class WireJig : DrawJig
    {
        private const double ellipseWidth = 0.0747;
        private const double ellipseHeight = 0.0373;
        private const double cornerRadius = Utility.Inch_1_16;
        private const double minLineAfterCorner = Utility.Inch_1_16;
        private const string KW_Inverse = "Inverse";
        private const string KW_Inverse2 = "iNnverse";
        private const string KW_Offset = "Offset";


        private double _textOffset = Utility.Inch_1_16;
        private bool _inverse = false;
        private Point3d _basePoint;
        private Point3d _lastPoint;
        private const int _jigFactorCount = 2;
        private int _jigFactorIndex = 1;


        private Editor AcEditor => AcadApp.DocumentManager.MdiActiveDocument.Editor;


        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions ppo;
            PromptPointResult ppr;

            switch (_jigFactorIndex)
            {
                case 1:
                    // Obtaining an insert point
                    ppo = new JigPromptPointOptions("\nSpecify point on wire: ")
                    {
                    };

                    ppr = prompts.AcquirePoint(ppo);
                    if (ppr.Status == PromptStatus.Cancel && ppr.Status == PromptStatus.Error)
                        return SamplerStatus.Cancel;

                    _basePoint = ppr.Value;

                    return SamplerStatus.OK;

                case 2:
                    // Obtaining a text insert point
                    ppo = new JigPromptPointOptions("\nText insertion point: ")
                    {
                    };

                    ppr = prompts.AcquirePoint(ppo);
                    if (ppr.Status == PromptStatus.Cancel && ppr.Status == PromptStatus.Error)
                        return SamplerStatus.Cancel;

                    var pt = ppr.Value;

                    //if (Utility.IsOrthogonal)
                    //{
                    //    pt = Utility.GetOrthoPoint(_basePoint, pt);
                    //}

                    //var pio = new PromptIntegerOptions("\nCount: ")
                    //{
                    //    UpperLimit = 10,
                    //    LowerLimit = 1,
                    //    DefaultValue = 1,
                    //};
                    //var pir = AcEditor.GetInteger(pio);
                    //if (pir.Status != PromptStatus.OK)
                    //{
                    //    AcEditor.WriteMessage($"\nNumber = {pir.Value}");
                    //}

                    _lastPoint = pt;

                    return SamplerStatus.OK;
            }

            return SamplerStatus.OK;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            //Keys mods = Control.ModifierKeys;
            //var isInverse = (mods & Keys.Control) > 0;

            var geo = draw.Geometry;
            if (geo != null)
            {
                var xAngleDeg = _basePoint.GetVectorTo(_lastPoint).GetAngleTo(Vector3d.XAxis, Vector3d.ZAxis.Negate()) * 180 / Math.PI;
                var isVertical = xAngleDeg > 45 && xAngleDeg < 135 || xAngleDeg > 225 && xAngleDeg < 315;

                geo.PushModelTransform(AcEditor.CurrentUserCoordinateSystem);

                geo.EllipticalArc(
                    center: _basePoint,
                    normal: Vector3d.ZAxis,
                    majorAxisLength: ellipseWidth,
                    minorAxisLength: -ellipseHeight,
                    startDegreeInRads: Utility.DegreesToRadians(120),
                    endDegreeInRads: Utility.DegreesToRadians(60),
                    tiltDegreeInRads: Utility.DegreesToRadians(_jigFactorIndex > 1 && isVertical ? 90 : 0),
                    arcType: ArcType.ArcSimple
                );

                var isLeft = _lastPoint.X < _basePoint.X - ellipseWidth;
                var isRight = _lastPoint.X > _basePoint.X + ellipseWidth;
                var isUp = _lastPoint.Y > _basePoint.Y + ellipseWidth;
                var isDown = _lastPoint.Y < _basePoint.Y - ellipseWidth;

                var isVertDirValid =
                    Math.Abs(_lastPoint.X - _basePoint.X) > cornerRadius + minLineAfterCorner + _textOffset &&
                    Math.Abs(_lastPoint.Y - _basePoint.Y) > ellipseWidth + cornerRadius + minLineAfterCorner;

                var isHorizDirValid = Math.Abs(_lastPoint.X - _basePoint.X) > ellipseWidth + minLineAfterCorner + _textOffset;

                var isValid = isVertical
                    ? (isUp || isDown) && isVertDirValid
                    : (isLeft || isRight) && isHorizDirValid;

                if (_jigFactorIndex > 1 && isValid)
                {
                    var pline = new Autodesk.AutoCAD.DatabaseServices.Polyline();
                    var segments = 1;

                    var startPoint = new Point2d(
                        x: _basePoint.X + ellipseWidth * (isRight ? 1 : -1) * Convert.ToDouble(!isVertical),
                        y: _basePoint.Y + ellipseWidth * (isUp ? 1 : -1) * Convert.ToDouble(isVertical)
                    );
                    pline.AddVertexAt(0, startPoint, 0, 0, 0);

                    if (isVertical && !Utility.IsOrthogonal)
                    {
                        var isVertUp = _lastPoint.Y > _basePoint.Y + cornerRadius + minLineAfterCorner;
                        var isVertRight = _lastPoint.X > _basePoint.X + cornerRadius + minLineAfterCorner;

                        var arcBeginPt = new Point2d(
                            x: _basePoint.X,
                            y: _lastPoint.Y + cornerRadius * (isVertUp ? -1 : 1)
                        );

                        // TODO расчитать bulge
                        var bulge = 0.401426d * (isUp && isRight || isDown && isLeft ? -1 : 1);
                        pline.AddVertexAt(1, arcBeginPt, bulge, 0, 0);

                        var arcEndPt = new Point2d(
                            x: _basePoint.X + cornerRadius * (isVertRight ? 1 : -1),
                            y: _lastPoint.Y
                        );
                        pline.AddVertexAt(2, arcEndPt, 0, 0, 0);

                        var lastPt = new Point2d(
                            x: _lastPoint.X + _textOffset * (isVertRight ? -1 : 1),
                            y: _lastPoint.Y
                        );
                        pline.AddVertexAt(3, lastPt, 0, 0, 0);

                        segments = 3;
                    }
                    else
                    {
                        var lastPt = new Point2d(
                            x: _lastPoint.X + _textOffset * (isRight ? -1 : 1),
                            y: _basePoint.Y
                        );
                        pline.AddVertexAt(1, lastPt, 0, 0, 0);
                    }

                    geo.Polyline(pline, 0, segments);

                    // Text

                    var textStyle = new TextStyle(
                        fontName: "romans.shx",
                        bigFontName: "",
                        textSize: 0.1,
                        x: 1,
                        obliqueAngle: 0,
                        trackingPercent: 100,
                        isBackward: false,
                        isUpsideDown: false,
                        isVertical: false,
                        isOverLined: false,
                        isUnderlined: false,
                        isStrikethrough: false,
                        "EDEC Spec"
                    );

                    var msg = "4 # 13";

                    var y = isVertical && (isUp || isDown)
                        ? isRight || isLeft ? _lastPoint.Y : _basePoint.Y
                        : _basePoint.Y;

                    var x = isRight
                        ? _lastPoint.X
                        : _lastPoint.X - textStyle.TextSize * msg.Length;

                    var textPoint = new Point3d(
                        x: x,
                        y: y - textStyle.TextSize / 2,
                        z: _lastPoint.Z
                    );

                    geo.Text(textPoint, Vector3d.ZAxis, Vector3d.XAxis, msg, true, textStyle);
                }



                //TextStyleTableRecord edec01textStyle;

                //using (var tr = doc.Database.TransactionManager.StartTransaction())
                //{
                //    var textStyleTable = tr.GetObject(doc.Database.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                //    if (textStyleTable != null && textStyleTable.Has("EDEC 0.15"))
                //    {
                //        edec01textStyle = textStyleTable["EDEC 0.15"].GetObject(OpenMode.ForRead) as TextStyleTableRecord;
                //    }
                //    else
                //    {
                //        textStyleTable.UpgradeOpen();

                //        edec01textStyle = new TextStyleTableRecord()
                //        {
                //            FileName = "romans.shx",
                //            Name = "EDEC 0.15",
                //            TextSize = 0.1,
                //            XScale = 0.8,
                //        };

                //        textStyleTable.Add(edec01textStyle);
                //        tr.AddNewlyCreatedDBObject(edec01textStyle, true);
                //    }

                //    tr.Commit();
                //}

                //var textPoint = new Point3d();
                //geo.Text(
                //    position: textPoint,
                //    normal: Vector3d.ZAxis,
                //    direction: Vector3d.XAxis,
                //    message: "TEXT",
                //    raw: true,
                //    textStyle: new TextStyle()
                //    {
                //        Font = edec01textStyle.Font,
                //        StyleName = "EDEC 0.15",
                //    }
                //);

                geo.PopModelTransform();
            }

            return true;
        }

        private void KeywordHandle(string keyword)
        {
            if (keyword.Equals(KW_Offset, StringComparison.InvariantCultureIgnoreCase))
            {
                var pdo = new PromptDoubleOptions("\nOffset value or")
                {
                    AllowZero = false,
                    AllowNegative = false,
                    UseDefaultValue = true,
                    DefaultValue = _textOffset,
                    Keywords =
                    {
                        "kw1",
                        "kw2",
                        "kw3",
                    },
                };

                pdo.Keywords.Default = "kw3";
                pdo.AllowNone = true;

                var pdr = AcEditor.GetDouble(pdo);
                if (pdr.Status == PromptStatus.OK)
                {
                    _textOffset = pdr.Value;
                }
            }
            else if (keyword.Equals(KW_Inverse, StringComparison.InvariantCultureIgnoreCase))
            {
                var pio = new PromptIntegerOptions("\nInverse value: ")
                {
                    AllowZero = false,
                    AllowNegative = false,
                    UseDefaultValue = true,
                    DefaultValue = Convert.ToInt32(_inverse),
                };

                var pir = AcEditor.GetInteger(pio);
                if (pir.Status == PromptStatus.OK)
                {
                    _inverse = Convert.ToBoolean(pir.Value);
                }
            }
            else if (keyword.Equals(KW_Inverse2, StringComparison.InvariantCultureIgnoreCase))
            {
                _inverse = !_inverse;
                AcEditor.WriteMessage($"\nInversion {(_inverse ? "ON" : "OFF")}");
            }
        }

        private bool Jig()
        {
            try
            {
                PromptResult pr;
                do
                {
                    pr = AcEditor.Drag(this);

                    switch (pr.Status)
                    {
                        case PromptStatus.Keyword:
                            KeywordHandle(pr.StringResult);
                            break;

                        case PromptStatus.OK:
                            if (pr is PromptPointResult ppr)
                            {
                                _basePoint = ppr.Value;
                            }

                            _jigFactorIndex++;
                            break;

                        default:
                            break;
                    }
                } while (
                    pr.Status != PromptStatus.Cancel &&
                    pr.Status != PromptStatus.Error &&
                    _jigFactorIndex <= _jigFactorCount
                );

                return _jigFactorIndex == _jigFactorCount + 1;
            }
            catch
            {
                return false;
            }
        }

        [CommandMethod("W")]
        public static void Command()
        {
            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            var jigger = new WireJig();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    if (jigger.Jig())
                        tr.Commit();
                    else
                        tr.Abort();
                }
                catch (System.Exception)
                {
                    tr.Abort();
                }
            }
        }
    }
}
