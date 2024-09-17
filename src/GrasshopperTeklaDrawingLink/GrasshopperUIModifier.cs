using Grasshopper;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using GTDrawingLink.Components;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GTDrawingLink
{
    public class GrasshopperUIModifier : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Instances.CanvasCreated += new Instances.CanvasCreatedEventHandler(this.AddTeklaMenu);

            return GH_LoadingInstruction.Proceed;
        }

        private void AddTeklaMenu(GH_Canvas canvas)
        {
            Instances.CanvasCreated -= new Instances.CanvasCreatedEventHandler(this.AddTeklaMenu);
            var documentEditor = Instances.DocumentEditor;
            if (documentEditor != null)
            {
                documentEditor.MainMenuStrip.Items.Add(GetTeklaMenu());
            }           
        }

        private ToolStripItem GetTeklaMenu()
        {
            var teklaMenu = new ToolStripMenuItem("Drawing Link");
            teklaMenu.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                new ToolStripMenuItem(ParamInfos.SelectAllTeklaObjects.Name, null, SelectObjectsMenuItem_Clicked)
                {
                    ToolTipText = ParamInfos.SelectAllTeklaObjects.Description
                },
                new ToolStripMenuItem(ParamInfos.DeleteAllTeklaObjects.Name, null, DeleteObjectsMenuItem_Clicked)
                {
                    ToolTipText = ParamInfos.DeleteAllTeklaObjects.Description
                },
                new ToolStripMenuItem(ParamInfos.BakeAllToTekla.Name, null, BakeMenuItem_Clicked)
                {
                    ToolTipText = ParamInfos.BakeAllToTekla.Description
                }
            });
            return teklaMenu;
        }

        private void SelectObjectsMenuItem_Clicked(object sender, EventArgs e)
        {
            var drawingObjects = new List<Tekla.Structures.Drawing.DrawingObject>();
            foreach (var component in GetAllComponentsOfType<IBakeable>())
            {
                component.GetObjects().ForEach(o =>
                {
                    if (o is Tekla.Structures.Drawing.DrawingObject drawingObject)
                        drawingObjects.Add(drawingObject);
                });
            }

            DrawingInteractor.Highlight(drawingObjects);
        }

        private void DeleteObjectsMenuItem_Clicked(object sender, EventArgs e)
        {
            foreach (var component in GetAllComponentsOfType<IBakeable>())
            {
                component.DeleteObjects();
            }
        }

        private void BakeMenuItem_Clicked(object sender, EventArgs e)
        {
            foreach (var component in GetAllComponentsOfType<IBakeable>())
                component.BakeToTekla();
        }

        private static GH_Document GetDocument()
        {
            return Instances.ActiveCanvas.Document;
        }

        private static IEnumerable<T> GetAllComponentsOfType<T>()
        {
            return GetDocument().Objects.Where((IGH_DocumentObject o) => o is T).Cast<T>();
        }
    }
}
