using GTDrawingLink.Components;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GTDrawingLink.Tools
{
    public static class VersionSpecificConstants
    {
        private static string _tabHeading = null;

        private static Dictionary<Type, string> _typeGuids = new Dictionary<Type, string>
        {
            {
                typeof(TeklaDrawingObjectFloatingParam),
                "8D457DE8-CBFF-452F-BB24-C0A7B58F7108"
            },
            {
                typeof(TeklaDrawingObjectParam),
                "966E988B-A021-49A7-813A-3AAC15D88348"
            },
            {
                typeof(TeklaDrawingPartParam),
                "2267ACA0-867D-4529-92D5-AA9771E3B904"
            },
            {
                typeof(TeklaDrawingPointParam),
                "315E3AB6-0C7E-445B-B4E4-B6737B4AC02B"
            },
            {
                typeof(ConvertDrawingToModelObjectComponent),
                "29F72A64-9760-4E49-92A4-D7CC87DBBB45"
            },
            {
                typeof(GetViewFromDrawingObjectComponent),
                "11C98726-EE10-422C-864F-2B3AE6B4CF3F"
            },
            {
                typeof(GetSelectedDrawingsOnListComponent),
                "E253DF94-4E21-4DDA-A3EE-1B746A0B24E7"
            },
            {
                typeof(GetActiveDrawingComponent),
                "EBEC9738-117A-4E3A-92F6-D8E69A7DF8DD"
            },
            {
                typeof(CreatePartViewComponent),
                "D6397266-E7A0-41D9-8D93-704F2EB48866"
            },
            {
                typeof(GetViewFrameGeometryComponent),
                "7FD6C114-1624-4463-95DD-56FA02862BCC"
            },
            {
                typeof(GetViewsAtDrawingComponent),
                "7F169461-6085-4A15-ABDA-12F4ABC714A5"
            },
            {
                typeof(GetViewPropertiesComponent),
                "659B5B28-ADB8-41B4-B19F-E68F77023EA4"
            },
            {
                typeof(GetRelatedViewsComponent),
                "9E6C2190-4D2C-498D-9A0A-99E27227C55D"
            },
            {
                typeof(MoveViewComponent),
                "B1132E88-80C0-43B7-9597-3323CAB35570"
            },
            {
                typeof(CloseDrawingComponent),
                "7A4834BA-B785-4668-9A25-48158AFCD15E"
            },
            {
                typeof(OpenDrawingComponent),
                "7E0CCEDE-6746-44A8-9E48-458AA59F5214"
            },
            {
                typeof(GetDrawingSizeComponent),
                "99CA7A46-2B6B-4727-AE82-F8489A4E67F3"
            },
            {
                typeof(GetDrawingSourceObjectComponent),
                "77DB5D67-38A7-404D-9ABB-4406189DFCBA"
            },
            {
                typeof(GetDrawingsComponent),
                "4196402D-71D6-45D8-B12A-8E8023B0332B"
            },
            {
                typeof(CreateCUDrawingComponent),
                "F37C442F-2E2E-4B03-8B1F-81DC16149287"
            },
            {
                typeof(CreateADrawingComponent),
                "7B9407BF-DC5A-441E-9FFD-E314E32AC9B1"
            },
            {
                typeof(CreateDetailViewComponent),
                "FAAF7EF8-F686-46C9-9605-6D8056D41383"
            },
            {
                typeof(CreateSectionViewComponent),
                "F33C9387-8C89-468D-A7CC-E259A1FE695B"
            },
            {
                typeof(TeklaDrawingViewBaseParam),
                "5E08DC0B-4DFE-46CD-A3AF-95A0A9846E6C"
            },
            {
                typeof(TeklaDrawingViewParam),
                "45D3D55F-AED9-4DE6-8160-BDE0B8F6DA91"
            },
            {
                typeof(TeklaDrawingParam),
                "3FF64AFA-EE2F-4D3B-8ED2-CB0A02C859FC"
            },
            {
                typeof(TransformPointComponent),
                "2AADAF3C-8CDF-4D06-8C76-8DCA2C6DF5B9"
            },
            {
                typeof(SelectDrawingObjectComponent),
                "F0AB948C-CB4B-4428-9160-60761FABAEA9"
            },
            {
                typeof(RunMacroComponent),
                "2DDA4F5D-C05A-4566-8716-84788FC88C5D"
            },
            {
                typeof(GetCOGComponent),
                "E7F13457-4F00-4F1C-A2EB-4DB9E0E61774"
            },
            {
                typeof(TeklaGravityObjectParam),
                "B5725678-82C3-49CB-B3C6-BBF9062C168B"
            }
        };

        public static string TabHeading
        {
            get
            {
                if (_tabHeading == null)
                    _tabHeading = $"Tekla Drawing {GrasshopperTeklaDrawingLinkInfo.TSVersion}";

                return _tabHeading;
            }
        }

        public static Guid GetGuid(Type type)
        {
            try
            {
                return new Guid(_typeGuids[type]);
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageBox.Show("Guid format wrong for class " + type.ToShortString());
                }
                else
                {
                    MessageBox.Show("Guid not found for class " + type.ToShortString() + "\n" + ex.ToString());
                }
                throw new Exception("Correct guid not found for class " + type.ToShortString());
            }
        }

    }
}
