using Addon.Infrastructure;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using System;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Addon.Jigs
{
    public class WireLabelJig : DrawJig
    {
        private const double _ellipseWidth = 0.0747;
        private const double _ellipseHeight = 0.0373;
        private const double _minHorizLine = Utility.Inch_1_16;
        private const double _minVertLine = Utility.Inch_1_16;
        private const string _textStyleName = "EDEC 0.1";
        private const string _kwCornerRadius = "Radius";
        private const string _kwNoText = "noText";
        private const string _kwText = "Text";
        private const string _kwMultilabel = "Multilabel";

        /// <summary>
        /// Base point on wire (center ellipse point).
        /// </summary>
        private Point3d _basePoint;

        /// <summary>
        /// Current cursor point.
        /// </summary>
        private Point3d _lastPoint;

        /// <summary>
        /// Total stages of JIG.
        /// </summary>
        private const int _jigStageCount = 2;

        /// <summary>
        /// Current stage of JIG.
        /// </summary>
        private int _jigStageIndex = 1;

        /// <summary>
        /// Radius of corner annotation line.
        /// </summary>
        public static double CornerRadius { get; set; } = Utility.Inch_1_16;

        /// <summary>
        /// Wire lable offset from annotation line.
        /// </summary>
        public static double TextOffset { get; set; } = Utility.Inch_1_16;

        /// <summary>
        /// No label.
        /// </summary>
        public static bool IsNoText { get; set; }

        /// <summary>
        /// Autocad Current Document Editor.
        /// </summary>
        private Editor AcEditor => AcadApp.DocumentManager.MdiActiveDocument.Editor;


        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            JigPromptPointOptions ppo;
            PromptPointResult ppr;

            switch (_jigStageIndex)
            {
                case 1:
                    // Obtaining an insert point
                    ppo = new JigPromptPointOptions("\nSpecify point on wire: ")
                    {
                        UserInputControls = UserInputControls.GovernedByOrthoMode | UserInputControls.GovernedByUCSDetect,
                        Keywords =
                        {
                            _kwCornerRadius,
                            IsNoText ? _kwText : _kwNoText,
                            //_kwMultilabel,
                        },
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
                        Keywords =
                        {
                            _kwCornerRadius,
                            IsNoText ? _kwText : _kwNoText,
                            //_kwMultilabel,
                        },
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
            var geo = draw.Geometry;
            if (geo != null)
            {
                var isVertical = Math.Abs(_basePoint.Y - _lastPoint.Y) > _ellipseWidth + _minVertLine + CornerRadius - Tolerance.Global.EqualPoint;

                geo.PushModelTransform(AcEditor.CurrentUserCoordinateSystem);

                geo.EllipticalArc(
                    center: _basePoint,
                    normal: Vector3d.ZAxis,
                    majorAxisLength: _ellipseWidth,
                    minorAxisLength: -_ellipseHeight,
                    startDegreeInRads: Utility.DegreesToRadians(120),
                    endDegreeInRads: Utility.DegreesToRadians(60),
                    tiltDegreeInRads: Utility.DegreesToRadians(_jigStageIndex > 1 && isVertical ? 90 : 0),
                    arcType: ArcType.ArcSimple
                );

                var isLeft = isVertical
                    ? _lastPoint.X <= _basePoint.X - CornerRadius - _minHorizLine + Tolerance.Global.EqualPoint
                    : _lastPoint.X <= _basePoint.X - _ellipseWidth - _minHorizLine + Tolerance.Global.EqualPoint;
                var isRight = isVertical
                    ? _lastPoint.X > _basePoint.X + CornerRadius + _minHorizLine - Tolerance.Global.EqualPoint
                    : _lastPoint.X >= _basePoint.X + _ellipseWidth + _minHorizLine - Tolerance.Global.EqualPoint;
                var isValid = isRight || isLeft;

                if (_jigStageIndex > 1 && isValid)
                {
                    var isUp = _lastPoint.Y >= _basePoint.Y + _ellipseWidth + _minVertLine + CornerRadius;
                    var isDown = _lastPoint.Y <= _basePoint.Y - _ellipseWidth - _minVertLine - CornerRadius;

                    var pline = new Autodesk.AutoCAD.DatabaseServices.Polyline();
                    var segments = 1;

                    var startPoint = new Point2d(
                        x: _basePoint.X + _ellipseWidth * (isRight ? 1 : -1) * Convert.ToDouble(!isVertical),
                        y: _basePoint.Y + _ellipseWidth * (isUp ? 1 : -1) * Convert.ToDouble(isVertical)
                    );
                    pline.AddVertexAt(0, startPoint, 0, 0, 0);

                    if (isVertical)
                    {
                        var arcBeginPt = new Point2d(
                            x: _basePoint.X,
                            y: _lastPoint.Y + CornerRadius * (isUp ? -1 : 1)
                        );

                        var bulge = Math.Tan(Utility.DegreesToRadians(90) / 4) * (isUp && isRight || isDown && isLeft ? -1 : 1);
                        pline.AddVertexAt(1, arcBeginPt, bulge, 0, 0);

                        var arcEndPt = new Point2d(
                            x: _basePoint.X + CornerRadius * (isRight ? 1 : -1),
                            y: _lastPoint.Y
                        );
                        pline.AddVertexAt(2, arcEndPt, 0, 0, 0);

                        var lastPt = new Point2d(
                            x: _lastPoint.X,
                            y: _lastPoint.Y
                        );
                        pline.AddVertexAt(3, lastPt, 0, 0, 0);

                        segments = 3;
                    }
                    else
                    {
                        var lastPt = new Point2d(
                            x: _lastPoint.X,
                            y: _basePoint.Y
                        );
                        pline.AddVertexAt(1, lastPt, 0, 0, 0);
                    }

                    geo.Polyline(pline, 0, segments);

                    // Text

                    if (!IsNoText)
                    {
                        var textStyle = GetTextStyle(_textStyleName);
                        var text = "LABEL";
                        var extents = textStyle.ExtentsBox(text, false, true, draw);
                        var x = isRight
                            ? _lastPoint.X - extents.MinPoint.X + TextOffset
                            : _lastPoint.X - extents.MaxPoint.X - TextOffset;
                        var y = isVertical
                            ? _lastPoint.Y
                            : _basePoint.Y;
                        var textPoint = new Point3d(
                            x: x,
                            y: y - textStyle.TextSize / 2,
                            z: _lastPoint.Z
                        );

                        geo.Text(textPoint, Vector3d.ZAxis, Vector3d.XAxis, text, true, textStyle);
                    }
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

        private TextStyle GetTextStyle(string name) => new TextStyle(
            fontName: "RomanS.shx",
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
            name
        );

        private void KeywordHandle(string keyword)
        {
            if (keyword.Equals(_kwCornerRadius, StringComparison.InvariantCultureIgnoreCase))
            {
                var pdo = new PromptDoubleOptions("\nInput new Corner radius value: ")
                {
                    AllowZero = false,
                    AllowNegative = false,
                    UseDefaultValue = true,
                    DefaultValue = CornerRadius,
                };

                var pdr = AcEditor.GetDouble(pdo);
                if (pdr.Status == PromptStatus.OK && pdr.Value != CornerRadius)
                {
                    CornerRadius = pdr.Value;
                    AcEditor.WriteMessage($"\nChanged Corner Radius on {CornerRadius}\"");
                }
            }
            else if (keyword.Equals(_kwNoText, StringComparison.InvariantCultureIgnoreCase) || keyword.Equals(_kwText, StringComparison.InvariantCultureIgnoreCase))
            {
                IsNoText = !IsNoText;
                AcEditor.WriteMessage($"\nText {(IsNoText ? "OFF" : "ON")}");
            }
            else if (keyword.Equals(_kwMultilabel, StringComparison.InvariantCultureIgnoreCase))
            {
                AcEditor.WriteMessage($"\nMultilabel mode");
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

                            _jigStageIndex++;
                            break;

                        default:
                            break;
                    }
                } while (
                    pr.Status != PromptStatus.Cancel &&
                    pr.Status != PromptStatus.Error &&
                    _jigStageIndex <= _jigStageCount
                );

                return _jigStageIndex == _jigStageCount + 1;
            }
            catch (System.Exception exc)
            {
                AcEditor.WriteMessage($"\nException: {exc}");
                return false;
            }
        }

        [CommandMethod("W")]
        public static void Command()
        {
            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;

            var jigger = new WireLabelJig();
            if (jigger.Jig())
            {
                //using (var tr = db.TransactionManager.StartTransaction())
                //{

                //}
            }
        }
    }
}
