using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADLineDrawing
{
    public class LineDrawingCommands
    {
        /// <summary>
        /// Command to draw a simple line in AutoCAD
        /// Usage: Type "DRAWLINE" in AutoCAD command line
        /// </summary>
        [CommandMethod("DRAWLINE")]
        public void DrawLine()
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Define start and end points for the line
            Point3d startPoint = new Point3d(0, 0, 0);
            Point3d endPoint = new Point3d(100, 100, 0);

            // Start a transaction
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Open the Block table for read
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    
                    // Open the Block table record Model space for write
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    
                    // Create a new line object
                    Line line = new Line(startPoint, endPoint);
                    
                    // Set line properties (optional)
                    line.ColorIndex = 1; // Red color
                    
                    // Add the line to the block table record
                    btr.AppendEntity(line);
                    
                    // Add the line to the transaction
                    tr.AddNewlyCreatedDBObject(line, true);
                    
                    // Commit the transaction
                    tr.Commit();
                    
                    ed.WriteMessage("\nLine drawn successfully from (0,0,0) to (100,100,0)");
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage($"\nError drawing line: {ex.Message}");
                    tr.Abort();
                }
            }
        }

        /// <summary>
        /// Interactive command to draw a line with user input
        /// Usage: Type "DRAWLINEINTERACTIVE" in AutoCAD command line
        /// </summary>
        [CommandMethod("DRAWLINEINTERACTIVE")]
        public void DrawLineInteractive()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            try
            {
                // Prompt user for start point
                PromptPointOptions ppo1 = new PromptPointOptions("\nSelect start point: ");
                PromptPointResult ppr1 = ed.GetPoint(ppo1);
                
                if (ppr1.Status != PromptStatus.OK)
                    return;

                // Prompt user for end point
                PromptPointOptions ppo2 = new PromptPointOptions("\nSelect end point: ");
                ppo2.BasePoint = ppr1.Value;
                ppo2.UseBasePoint = true;
                PromptPointResult ppr2 = ed.GetPoint(ppo2);
                
                if (ppr2.Status != PromptStatus.OK)
                    return;

                // Start a transaction
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // Open the Block table for read
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    
                    // Open the Block table record Model space for write
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    
                    // Create a new line object with user-selected points
                    Line line = new Line(ppr1.Value, ppr2.Value);
                    
                    // Add the line to the block table record
                    btr.AppendEntity(line);
                    
                    // Add the line to the transaction
                    tr.AddNewlyCreatedDBObject(line, true);
                    
                    // Commit the transaction
                    tr.Commit();
                    
                    ed.WriteMessage($"\nLine drawn from {ppr1.Value} to {ppr2.Value}");
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\nError: {ex.Message}");
            }
        }

        /// <summary>
        /// Draw multiple lines to create a simple rectangle
        /// Usage: Type "DRAWRECTANGLE" in AutoCAD command line
        /// </summary>
        [CommandMethod("DRAWRECTANGLE")]
        public void DrawRectangle()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Define rectangle corners
            Point3d corner1 = new Point3d(0, 0, 0);
            Point3d corner2 = new Point3d(100, 0, 0);
            Point3d corner3 = new Point3d(100, 50, 0);
            Point3d corner4 = new Point3d(0, 50, 0);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    // Create four lines to form a rectangle
                    Line[] lines = new Line[]
                    {
                        new Line(corner1, corner2), // Bottom
                        new Line(corner2, corner3), // Right
                        new Line(corner3, corner4), // Top
                        new Line(corner4, corner1)  // Left
                    };

                    foreach (Line line in lines)
                    {
                        line.ColorIndex = 2; // Yellow color
                        btr.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);
                    }

                    tr.Commit();
                    ed.WriteMessage("\nRectangle drawn successfully");
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage($"\nError drawing rectangle: {ex.Message}");
                    tr.Abort();
                }
            }
        }
    }
}