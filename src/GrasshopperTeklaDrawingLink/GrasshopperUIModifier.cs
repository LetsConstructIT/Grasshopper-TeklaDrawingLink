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
        private const string MenuName = "Drawing Link";

        private static readonly object _registrationLock = new object();
        private static bool _registered;

        public override GH_LoadingInstruction PriorityLoad()
        {
            EnsureRegistered();

            return GH_LoadingInstruction.Proceed;
        }

        /// <summary>
        /// Adds the "Drawing Link" menu exactly once. Safe to call multiple times and from different
        /// load paths: in Rhino 9 the Tekla loader may load this assembly via ParseGHA after the
        /// Grasshopper canvas already exists, in which case CanvasCreated would never fire again.
        /// </summary>
        internal static void EnsureRegistered()
        {
            try
            {
                lock (_registrationLock)
                {
                    if (_registered)
                        return;

                    _registered = true;
                }

                if (Instances.DocumentEditor != null)
                    AddTeklaMenu(Instances.DocumentEditor);
                else
                    Instances.CanvasCreated += OnCanvasCreated;
            }
            catch (Exception)
            {
                // Never let a UI customization failure break loading of the components.
            }
        }

        private static void OnCanvasCreated(GH_Canvas canvas)
        {
            Instances.CanvasCreated -= OnCanvasCreated;
            var documentEditor = Instances.DocumentEditor;
            if (documentEditor != null)
                AddTeklaMenu(documentEditor);
        }

        private static void AddTeklaMenu(Grasshopper.GUI.GH_DocumentEditor documentEditor)
        {
            var menuStrip = documentEditor.MainMenuStrip;
            if (menuStrip == null)
                return;

            if (menuStrip.InvokeRequired)
            {
                menuStrip.BeginInvoke(new Action(() => AddTeklaMenu(documentEditor)));
                return;
            }

            if (menuStrip.Items.OfType<ToolStripMenuItem>().Any(i => i.Text == MenuName))
                return;

            menuStrip.Items.Add(GetTeklaMenu());
        }

        private static ToolStripItem GetTeklaMenu()
        {
            var teklaMenu = new ToolStripMenuItem(MenuName);
            teklaMenu.DropDownItems.AddRange(new ToolStripItem[]
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
                },
                new ToolStripSeparator(),
                new ToolStripMenuItem($"Grasshopper-Tekla Drawing Link {GrasshopperTeklaDrawingLinkInfo.TSVersion}.{GrasshopperTeklaDrawingLinkInfo.LinkVersion}", null)
                {
                    ToolTipText = "Developed by Greg Olszewski",
                    Enabled =false
                },
            });
            return teklaMenu;
        }

        private static void SelectObjectsMenuItem_Clicked(object sender, EventArgs e)
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

        private static void DeleteObjectsMenuItem_Clicked(object sender, EventArgs e)
        {
            foreach (var component in GetAllComponentsOfType<IBakeable>())
            {
                component.DeleteObjects();
            }
        }

        private static void BakeMenuItem_Clicked(object sender, EventArgs e)
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
