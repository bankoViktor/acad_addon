using AcadNetAddinWizard_Namespace;
using Addon.Forms;
using Addon.Infrastructure;
using Addon.Jigs;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Addon.Commands
{
    public static class MethodCommands
    {
        private static void PlaceOnPoint(IEnumerable<Drawable> drawables, Point3d basePt, Point3d moveToPt)
        {
            // Transform a set of entities to a new location
            var mat = Matrix3d.Displacement(basePt.GetVectorTo(moveToPt));

            foreach (var drawable in drawables)
            {
                (drawable as Entity).TransformBy(mat);
            }
        }

        private static Drawable[] CreteBlockRef(MainCircuitBreakerDialog dlg, Transaction tr, BlockTableRecord modelSpace, BlockTableRecord blockDef)
        {
            // Create reference a block
            var blockRef = new BlockReference(Point3d.Origin, blockDef.ObjectId);

            // Add the block reference to modelspace
            modelSpace.AppendEntity(blockRef);
            tr.AddNewlyCreatedDBObject(blockRef, true);

            // Geting definition DESC attribute
            var attrDefDesc = blockDef
                .Cast<ObjectId>()
                .Single(id =>
                    id.GetObject(OpenMode.ForRead) is AttributeDefinition attrDef &&
                    !attrDef.Constant &&
                    attrDef.Tag.Equals("DESC", StringComparison.InvariantCultureIgnoreCase)
                )
                .GetObject(OpenMode.ForRead) as AttributeDefinition;

            // Attribute value
            var sb = new StringBuilder();
            if (dlg.Existing) sb.AppendLine("EXISTING");
            sb.AppendLine($"{dlg.Voltage}V, {dlg.PoleType}, {dlg.Current}A, {dlg.Aic}AIC");
            sb.AppendLine($"NEMA {dlg.Nema.ToUpper()} {dlg.Material.ToUpper()}");
            sb.AppendLine(dlg.ProtectionType.ToUpper());
            sb.AppendLine("MAIN BREAKER");
            sb.AppendLine($"CB-{dlg.TagNumber}");

            // Create reference to DESC attribute
            var attrRefDesc = new AttributeReference();
            attrRefDesc.SetAttributeFromBlock(attrDefDesc, blockRef.BlockTransform);
            attrRefDesc.TextString = sb.ToString();

            // Add the AttributeReference to the BlockReference
            blockRef.AttributeCollection.AppendAttribute(attrRefDesc);
            tr.AddNewlyCreatedDBObject(attrRefDesc, true);

            return new Drawable[]
            {
                blockRef,
            };
        }

        private static void AddTransientGraphics(IEnumerable<Drawable> drawables)
        {
            // Create transient graphic for each drawable object of the block
            foreach (var drawable in drawables)
            {
                TransientManager.CurrentTransientManager.AddTransient(
                    drawable,
                    TransientDrawingMode.DirectShortTerm,
                    128,
                    new IntegerCollection()
                );
            }
        }

        private static void UpdateTransGraphics(IEnumerable<Drawable> drawables, Point3d currentPt, Point3d moveToPt)
        {
            // Displace each of our drawables
            var mat = Matrix3d.Displacement(currentPt.GetVectorTo(moveToPt));

            // Update their graphics
            foreach (var drawable in drawables)
            {
                (drawable as Entity).TransformBy(mat);
                TransientManager.CurrentTransientManager.UpdateTransient(
                    drawable,
                    new IntegerCollection()
                );
            }
        }

        private static void ClearTransientGraphics()
        {
            TransientManager.CurrentTransientManager.EraseTransients(
               TransientDrawingMode.DirectShortTerm,
               128,
               new IntegerCollection()
            );
        }



        [CommandMethod(CommandKeywords.MainCircuitBreaker)]
        public static void MainCircuitBreakerCommand()
        {
            const string blockName = "MAIN_CIRCUIT_BREAKER";

            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;

            // Input parameters
            var dlg = new MainCircuitBreakerDialog()
            {
                TagNumber = 1,
                Owner = Control.FromHandle(AcadApp.MainWindow.Handle) as Form,
                StartPosition = FormStartPosition.CenterParent,
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            using (var tr = doc.Database.TransactionManager.StartTransaction())
            {
                Drawable[] drawables;
                PointMonitorEventHandler handle = null;
                var currentPoint = Point3d.Origin;

                try
                {
                    // Getting root block table
                    var blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // Ensure exist the block in database
                    if (!blockTable.Has(blockName))
                    {
                        var blockPath = Path.Combine(Directories.BlockDefinitionsDir, $"{blockName}.dwg");
                        Utility.AddBlockDefinitionFromDwg(db, blockPath);
                    }

                    // getting block definition
                    var blockDef = blockTable[blockName].GetObject(OpenMode.ForRead) as BlockTableRecord;

                    // Also open modelspace - we'll be adding our BlockReference to it
                    var modelSpace = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    // Creating full new references to block
                    drawables = CreteBlockRef(dlg, tr, modelSpace, blockDef);

                    // Make all block parts as transient graphics
                    AddTransientGraphics(drawables);

                    handle = (object _, PointMonitorEventArgs e) =>
                    {
                        var pt = e.Context.RawPoint;
                        // Update our graphics and the current point
                        UpdateTransGraphics(drawables, currentPoint, pt);
                        currentPoint = pt;
                    };
                    ed.PointMonitor += handle;

                    // Input insertion point
                    var ppo = new PromptPointOptions("\nSpecify insertion point: ");
                    var ppr = ed.GetPoint(ppo);
                    if (ppr.Status == PromptStatus.OK)
                    {
                        // Move to insertion point
                        PlaceOnPoint(drawables, currentPoint, ppr.Value);

                        // Commit the transaction
                        tr.Commit();
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    ed.WriteMessage("\nException: {0}", ex.Message);
                }
                finally
                {
                    // Clear any transient graphics
                    ClearTransientGraphics();

                    // Remove the event handler
                    if (handle != null)
                        ed.PointMonitor -= handle;
                }
            }
        }


        [CommandMethod(CommandKeywords.CircuitBreaker)]
        public static void CircuitBreakerCommand()
        {
            const string blockName = "CIRCUIT_BREAKER";

            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // TODO code here

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage(ex.ToString());
                }
            }
        }


        [CommandMethod(CommandKeywords.JunctionBox)]
        public static void JunctionBoxCommand()
        {
            const string blockName = "JUNCTION_BOX";

            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            var db = doc.Database;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // TODO code here

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage(ex.ToString());
                }
            }
        }


        [CommandMethod("TEST")]
        public static void Test()
        {
            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            var pso = new PromptStringOptions("\nNumber: ")
            {
                AllowSpaces = false,
                UseDefaultValue = true,
                DefaultValue = "1",
            };
            var psr = ed.GetString(pso);

            if (psr.Status != PromptStatus.OK) return;

            var number = int.Parse(psr.StringResult);

            for (var i = 0; i < number; i++)
            {
                var ppo = new PromptPointOptions($"\nPick {i + 1} point: ")
                {

                };

                var ppr = ed.GetPoint(ppo);
                if (ppr.Status != PromptStatus.OK) return;


            }

            //using (var tr = db.TransactionManager.StartTransaction())
            //{
            //    var jig = new MoveJig(entities, ptResult.Value.TransformBy(ed.CurrentUserCoordinateSystem));
            //    var result = ed.Drag(jig);

            //    if (result.Status == PromptStatus.OK)
            //    {
            //        foreach (var ent in entities)
            //        {
            //            ent.UpgradeOpen();
            //            ent.TransformBy(jig.Displacement);
            //        }
            //    }
            //    tr.Commit();
            //}
        }

        [CommandMethod("TEST2")]
        public static void TestEntityJigger1_Method()
        {
            Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
            if (LineJig.Jig())
            {
                ed.WriteMessage("\nA line segment has been successfully jigged and added to the database.\n");
            }
            else
            {
                ed.WriteMessage("\nIt failed to jig and add a line segment to the database.\n");
            }
        }

        /*
        [CommandMethod("bat_bc")]
        public static void Sub16()
        {
            
            double segmentLength = 10;
            if (InputHelper.GetDouble(ref segmentLength, "\nДать длину сегмента отметки"))
            {
                double dimScale = EntityHelper.GetActiveDatabase().Dimscale;
                DBTextSub dbTextSub1 = new DBTextSub();
                DBTextSub dbTextSub2 = new DBTextSub();
                Line line0 = new Line();
                Line line1 = new Line();
                Line line2 = new Line();
                Polyline polyline = new Polyline();
                Jig djf = new DrawJigFramework();
                djf.JigEntity = new List<Entity>() { line0, line1, line2, polyline, dbTextSub1, dbTextSub2 };
                djf.JigPhases = new List<Phase>() {
                                         new PointPhase ("\nДать точку для определения максимальной отметки линейки", false),
                                         new PointPhase ("\nДать точку, чтобы определить наименьшую отметку линейки", false),
                                         new PointPhase ("\nДать точку для определения горизонтального положения линейки", false)};
                djf.JigUpdate = () =>
                {
                    if (!djf.JigPhases[0].IsDone) return false; // Возвращение false означает, что данные еще не готовы                    
                    double x = ((Point3d)djf.JigPhases[0].Value).X;
                    if (djf.JigPhases[1].IsDone) x = ((Point3d)djf.JigPhases[2].Value).X;
                    double y1 = Math.Ceiling(((Point3d)djf.JigPhases[0].Value).Y / segmentLength) * segmentLength;
                    double y2 = Math.Floor(((Point3d)djf.JigPhases[1].Value).Y / segmentLength) * segmentLength;
                    dbTextSub1.TextString = y1.S0();
                    dbTextSub1.Height = 3 * dimScale;
                    dbTextSub1.AdjustAlignmentPoint(new Point3d(x - dbTextSub1.GetTextWidth() * 0.5 * dimScale - 4 * dimScale, y1, 0));
                    dbTextSub2.TextString = y2.S0();
                    dbTextSub2.Height = 3 * dimScale;
                    dbTextSub2.AdjustAlignmentPoint(new Point3d(x - dbTextSub2.GetTextWidth() * 0.5 * dimScale - 4 * dimScale, y2, 0));
                    // Обновляем центральную ось
                    line0.StartPoint = new Point3d(x, y1, 0);
                    line0.EndPoint = new Point3d(x, y2, 0);
                    // Обновляем верхнюю галочку                
                    line1.StartPoint = new Point3d(x - 3 * dimScale, y1, 0);
                    line1.EndPoint = new Point3d(x, y1, 0);
                    // Обновляем нижний тик
                    line2.StartPoint = new Point3d(x - 3 * dimScale, y2, 0);
                    line2.EndPoint = new Point3d(x, y2, 0);
                    // Обновляем схему
                    polyline.Reset(false, 0);
                    polyline.AddVertexAt(0, new Point2d(x - 3 * dimScale / 2, y1), 0, 0, 0);
                    polyline.AddVertexAt(0, new Point2d(x + 3 * dimScale / 2, y1), 0, 0, 0);
                    polyline.AddVertexAt(0, new Point2d(x + 3 * dimScale / 2, y2), 0, 0, 0);
                    polyline.AddVertexAt(0, new Point2d(x - 3 * dimScale / 2, y2), 0, 0, 0);
                    polyline.AddVertexAt(0, new Point2d(x - 3 * dimScale / 2, y1), 0, 0, 0);
                    return true;
                };
                djf.JigEnding = () =>
                {
                    double x = ((Point3d)djf.JigPhases[2].Value).X;
                    double y1 = Math.Ceiling(((Point3d)djf.JigPhases[0].Value).Y / segmentLength) * segmentLength;
                    double y2 = Math.Floor(((Point3d)djf.JigPhases[1].Value).Y / segmentLength) * segmentLength;
                    double dy = y2 > y1 ? segmentLength : -segmentLength;
                    double y = y1 + dy;
                    while (y1 < y2 ? y < y2 : y > y2)
                    {
                        // Добавляем галочки
                        Line line = new Line();
                        djf.JigEntity.Add(line);
                        line.StartPoint = new Point3d(x - 3 * dimScale, y, 0);
                        line.EndPoint = new Point3d(x, y, 0);
                        // Добавляем текст высот
                        DBTextSub dbtext = new DBTextSub(y.S0(), 3 * dimScale);
                        djf.JigEntity.Add(dbtext);
                        dbtext.AdjustAlignmentPoint(new Point3d(x - dbTextSub1.GetTextWidth() * 0.5 * dimScale - 4 * dimScale, y, 0));
                        y = y + dy;
                    }
                    y = y1;
                    while (y1 < y2 ? y < y2 : y > y2)
                    {
                        // Добавляем левую мозаику
                        Polyline pl = new Polyline();
                        djf.JigEntity.Add(pl);
                        pl.AddVertexAt(0, new Point2d(x - 0.75 * dimScale, y), 0, 1.5 * dimScale, 1.5 * dimScale);
                        pl.AddVertexAt(0, new Point2d(x - 0.75 * dimScale, y + dy), 0, 1.5 * dimScale, 1.5 * dimScale);
                        y = y + 2 * dy;
                    }
                    y = y1 + dy;
                    while (y1 < y2 ? y < y2 : y > y2)
                    {
                        // Добавляем правую мозаику
                        Polyline pl = new Polyline();
                        djf.JigEntity.Add(pl);
                        pl.AddVertexAt(0, new Point2d(x + 0.75 * dimScale, y), 0, 1.5 * dimScale, 1.5 * dimScale);
                        pl.AddVertexAt(0, new Point2d(x + 0.75 * dimScale, y + dy), 0, 1.5 * dimScale, 1.5 * dimScale);
                        y = y + 2 * dy;
                    }
                    return true;
                };
                djf.Jig2CurrentSpace();
            }
        }
        */

        [CommandMethod("Test4")]
        public static void Test4()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // prompt for first point
            var options = new PromptPointOptions("\nFirst point: ");
            var result = ed.GetPoint(options);
            if (result.Status != PromptStatus.OK)
                return;
            var pt1 = result.Value;

            // prompt for second point
            options.Message = "\nSecond point: ";
            options.BasePoint = pt1;
            options.UseBasePoint = true;
            result = ed.GetPoint(options);
            if (result.Status != PromptStatus.OK)
                return;
            var pt2 = result.Value;

            // prompt for third point
            options.Message = "\nThird point: ";
            options.BasePoint = pt2;
            result = ed.GetPoint(options);
            if (result.Status != PromptStatus.OK)
                return;
            var pt3 = result.Value;

            // convert points to 2d points
            var plane = new Plane(Point3d.Origin, Vector3d.ZAxis);
            var p1 = pt1.Convert2d(plane);
            var p2 = pt2.Convert2d(plane);
            var p3 = pt3.Convert2d(plane);

            // compute the bulge of the second segment
            var angle1 = p1.GetVectorTo(p2).Angle;
            var angle2 = p2.GetVectorTo(p3).Angle;
            var bulge = Math.Tan((angle2 - angle1) / 2.0);

            // add the polyline to the current space
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                var curSpace = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                using (var pline = new Autodesk.AutoCAD.DatabaseServices.Polyline())
                {
                    pline.AddVertexAt(0, p1, 0.0, 0.0, 0.0);
                    pline.AddVertexAt(1, p2, bulge, 0.0, 0.0);
                    pline.AddVertexAt(2, p3, 0.0, 0.0, 0.0);
                    pline.TransformBy(ed.CurrentUserCoordinateSystem);
                    curSpace.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }
                tr.Commit();
            }
        }
    }
}
