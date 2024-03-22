﻿using GTDrawingLink.Components;
using GTDrawingLink.Components.Annotations;
using GTDrawingLink.Components.AttributesComponents;
using GTDrawingLink.Components.Loops;
using GTDrawingLink.Components.ModifyComponents;
using GTDrawingLink.Components.Obsolete;
using GTDrawingLink.Components.Views;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GTDrawingLink.Tools
{
    public static class VersionSpecificConstants
    {
        private static string _tabHeading = null;

        public const string LoopStart = "3368FCF5-A321-4B54-944E-36A20DD01ED0";
        public const string LoopEnd = "13DC5668-65C5-43E3-A1EF-F9099205B878";
        public const string LinkParameter = "2E15CB40-F22E-47A9-AB91-311C496A6778";

        private static readonly Dictionary<Type, string> _typeGuids = new Dictionary<Type, string>
        {
            {
                typeof(TeklaDatabaseObjectFloatingParam),
                "8D457DE8-CBFF-452F-BB24-C0A7B58F7108"
            },
            {
                typeof(TeklaDatabaseObjectParam),
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
                "C187CE5C-27FE-40AD-8B30-399322FB287E"
            },
            {
                typeof(GetViewFrameGeometryComponentOLD),
                "7FD6C114-1624-4463-95DD-56FA02862BCC"
            },
            {
                typeof(GetViewsAtDrawingComponentOLD),
                "7F169461-6085-4A15-ABDA-12F4ABC714A5"
            },
            {
                typeof(GetViewsInDrawingComponent),
                "13F621E6-C50B-4677-94AC-DDB8F8309635"
            },
            {
                typeof(GetViewPropertiesComponent),
                "CBB7E9B7-963D-42B4-B063-A8C7AEDF909C"
            },
            {
                typeof(GetViewPropertiesComponentOLD),
                "01D3A4EF-043E-4149-A7FF-02398F010476"
            },
            {
                typeof(GetViewPropertiesComponentOLD2),
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
                "78395593-9996-4E5F-918B-60FA7B88C001"
            },
            {
                typeof(GetDrawingSizeComponentOLD),
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
                typeof(CreateGADrawingComponent),
                "0BC4FB66-6CA3-4D5E-B5A9-87DE7C2535FF"
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
                typeof(TransformPointToViewCSComponent),
                "2AADAF3C-8CDF-4D06-8C76-8DCA2C6DF5B9"
            },
            {
                typeof(TransformPointToGlobalCSComponent),
                "8AC5C2D3-405A-47E5-9673-701AE26E92EB"
            },
            {
                typeof(TransformPointToLocalCSComponent),
                "449329DC-7F49-41E3-9FC0-B8E919DA39CF"
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
            },
            {
                typeof(LineTypeAttributesParam),
                "D641C107-E2F9-42B4-92F0-4CFCAF8875E6"
            },
            {
                typeof(LineTypeAttributesComponent),
                "0DF6013A-F529-46C9-BC2A-B2585A8267F8"
            },
            {
                typeof(ModifyPartComponentOLD),
                "E18A1FF6-9CBA-488D-AE9E-7CAC8C03A6F9"
            },
            {
                typeof(GetSelectedComponent),
                "F7CA96CE-0274-4340-9048-EECF9AB6B87E"
            },
            {
                typeof(CreateLevelMarkComponent),
                "B63115EE-18D0-4996-860C-D4EB2D144F7A"
            },
            {
                typeof(CreateLevelMarkComponentOLD),
                "B197DF40-56B5-43EE-AFB9-141FE70BC4B9"
            },
            {
                typeof(ModelObjectHatchAttributesParam),
                "37856A90-5B50-432E-996C-998CF5674543"
            },
            {
                typeof(ModelObjectHatchAttributesComponent),
                "D1104F82-E3AA-4AF5-9E4C-49BD1B7EE7D5"
            },
            {
                typeof(SetDrawingUDAComponent),
                "D2D009DB-F89F-4A66-A7E5-7FD4C78AACCF"
            },
            {
                typeof(GetDrawingUDAValueComponent),
                "E59C9558-9764-4B6C-8131-73FABF8FECEC"
            },
            {
                typeof(GetDrawingAllUDAsComponent),
                "220E9AEC-EC8E-48EF-B072-A6425B1B868F"
            },
            {
                typeof(PickerInputComponent),
                "F652319A-8C77-4AAF-9BE8-45FA23E30A26"
            },
            {
                typeof(PickerInputTypeComponent),
                "782CBD7C-4844-4EAF-AFAB-6C2ADC33BE73"
            },
            {
                typeof(CreatePluginComponent),
                "1220E4FC-606E-4DCF-B131-E478817CEA25"
            },
            {
                typeof(CreatePluginComponentOLD),
                "81D25C54-4C0B-4724-B79D-02C836C16C60"
            },
            {
                typeof(StraightDimensionSetAttributesComponent),
                "E207603B-8583-4425-B147-ACBD7CB56E51"
            },
            {
                typeof(StraightDimensionSetAttributesComponentOLD),
                "99E2E30E-2C42-4BC8-9489-D9F1A400F354"
            },
            {
                typeof(StraightDimensionSetAttributesParam),
                "6A4DE637-B72D-4DF0-8D30-8A9418881033"
            },
            {
                typeof(DeconstructDimensionSetComponent),
                "9EE842B8-EA60-4B5F-A123-86640387E410"
            },
            {
                typeof(CreateStraightDimensionSetComponent),
                "9001FA9B-C60A-4919-BEFD-59E8DC5B309F"
            },
            {
                typeof(CreateStraightDimensionSetComponentOLD),
                "BCB39278-F65B-4F14-92C2-6D371E22ECE4"
            },
            {
                typeof(CreateDimensionLinkComponent),
                "9184C91C-AB99-418F-B8C0-DE8792A43037"
            },
            {
                typeof(GetExtremePointsComponent),
                "C899D93F-3390-4325-9A5D-5884A6387B16"
            },
            {
                typeof(GetPartLinesComponent),
                "ACDB464C-ABBF-4113-A4A3-77DBF4E66DBF"
            },
            {
                typeof(GetCustomPartPointsComponent),
                "4471F91D-084A-4571-85D2-BFDB5CB4965B"
            },
            {
                typeof(CreateAngleDimensionComponent),
                "A3C2779C-9C5F-4468-9F53-25FCF19781D4"
            },
            {
                typeof(CreateAngleDimensionComponentOLD),
                "52322B95-A7CA-4B4E-96B2-4E1B6D239CBA"
            },
            {
                typeof(ObjectMatchesToFilterComponent),
                "DD9FC705-1035-42D6-9D63-C169B1DC5A8E"
            },
            {
                typeof(ObjectMatchesToFilterComponentOLD),
                "37BF7499-39CD-4124-81D4-64BE134418F6"
            },
            {
                typeof(GroupObjectsComponent),
                "4717D364-FD0F-4A59-953B-F0123A831166"
            },
            {
                typeof(GroupObjectsComponentOLD),
                "D77A11DA-27EC-4CC1-A7B7-F3521D7783D7"
            },
            {
                typeof(GroupObjectsComponentOLD2),
                "CF46774B-064B-485C-A95D-E862768B0161"
            },
            {
                typeof(CreateTextComponent),
                "CEB5B9BF-6AA1-41E6-87B5-A177BA472B5E"
            },
            {
                typeof(CreateTextComponentOLD),
                "BB95F20B-A8AB-4CB6-A1B6-972AA0E1ED87"
            },
            {
                typeof(TextAttributesComponent),
                "2B9593A2-8F23-47E3-9156-82FF3A206C67"
            },
            {
                typeof(TextAttributesComponentOLD),
                "4C1BEABA-E671-488E-B20F-8A4FDD0B2A5F"
            },
            {
                typeof(FontAttributesParam),
                "CB121329-597E-4117-A75E-53C342FC3E8A"
            },
            {
                typeof(FontAttributesComponent),
                "1444CD7C-B6C1-4605-9A92-5A86B3481484"
            },
            {
                typeof(ArrowAttributesParam),
                "F8EAEBC0-5A1F-4057-85CD-E115FB0AE776"
            },
            {
                typeof(ArrowAttributesComponentOLD),
                "70C86C3F-993C-41EE-BEC7-185A7825E9DB"
            },
            {
                typeof(ArrowAttributesComponent),
                "BBEC0AA0-3E0B-45B1-86CC-52878123C843"
            },
            {
                typeof(DeleteDrawingObjectsComponent),
                "0F3832E1-2D16-4C56-96E1-2F63C1C5FFAC"
            },
            {
                typeof(GetObjectsFromViewComponent),
                "B8BF5B8F-F577-4543-8B39-E3F1B84AC2E0"
            },
            {
                typeof(GetObjectsFromViewComponentOLD),
                "D340933C-5C78-4052-A986-777FAE8241B0"
            },
            {
                typeof(RefreshViewComponent),
                "CB51563A-2CF6-46CF-A51F-CE8C695AD4DB"
            },
            {
                typeof(GetModelViewsComponent),
                "AEC509CC-8D1D-4DE2-A674-524649BA25D2"
            },
            {
                typeof(TeklaViewParam),
                "19D9E060-DA63-4484-969F-05F7E73ADF04"
            },
            {
                typeof(ConstructModelViewComponent),
                "69F67CC8-C21D-4ADB-BFDF-94C4E231C335"
            },
            {
                typeof(DeconstructModelViewComponent),
                "7A066A78-39B0-4DB5-A9E6-39DDEA080F1B"
            },
            {
                typeof(CreateModelViewComponent),
                "B4A454A8-0399-46F5-A199-06622288F3EE"
            },
            {
                typeof(ReinforcementMeshAttributesParam),
                "D41DBD10-5E53-46A0-950E-FDAAFFDAB421"
            },
            {
                typeof(ReinforcementMeshAttributesComponent),
                "3B4C6F04-2F33-49F9-9C98-2AC061A9EDFE"
            },
            {
                typeof(ReinforcementAttributesParam),
                "BA96DFCA-1060-4908-9274-E5F7875C7CEF"
            },
            {
                typeof(ReinforcementAttributesComponent),
                "41DEF423-BD56-4826-A5C0-ECF4A6E32DD9"
            },
            {
                typeof(ModifyRebarComponent),
                "0D6B1B6D-D2AE-4CFE-96B8-72CC0D9995F4"
            },
            {
                typeof(ModifyMeshComponent),
                "A007E703-1979-4BB3-8404-B9F916D2DA4D"
            },
            {
                typeof(FrameAttributesComponent),
                "0C287E3B-A35A-4CA8-B083-21276F4AAE16"
            },
            {
                typeof(FrameAttributesParam),
                "45767F13-A924-4BC7-B45B-1A9081F2B2EE"
            },
            {
                typeof(SymbolAttributesParam),
                "7A22469F-0E25-4BC7-8918-973B965D30E3"
            },
            {
                typeof(SymbolAttributesComponent),
                "368A230A-B959-40FE-B360-7578418E3243"
            },
            {
                typeof(CreateSymbolComponent),
                "26FF9BF1-01D3-465E-82FA-823FDE1EDCC5"
            },
            {
                typeof(CreateSymbolComponentOLD),
                "6B4A446F-BE7D-4267-974A-40F5B16FC483"
            },
            {
                typeof(SymbolSelectionComponent),
                "A222E53B-F020-439E-BDE1-0A23C6D9CAB9"
            },
            {
                typeof(SymbolInfoParam),
                "ED253FFE-56A7-46E1-A442-458FEA1B786F"
            },
            {
                typeof(PartAttributesParam),
                "89651017-00B9-4EFD-B595-B6F2AAA3E45D"
            },
            {
                typeof(PartAttributesComponent),
                "9CF81D45-6532-4755-9FCF-8976F897020D"
            },
            {
                typeof(ModifyPartComponent),
                "4E1B4B00-521E-45CD-ACD5-F60A4A67E7DA"
            },
            {
                typeof(DeleteDrawingComponent),
                "F1DD22E3-E5D1-44F4-AF3C-E0815DD643AA"
            },
            {
                typeof(GetDrawingsFromModelObjectComponent),
                "7F316FBD-CCAF-425F-AB4E-DAD134E9F492"
            },
            {
                typeof(OrderStraightDimensionSetComponent),
                "22FD71AB-4520-4A0A-BDAE-A73E9C12B1BE"
            },
            {
                typeof(BoltAttributesParam),
                "01DF170C-7782-47F8-BE39-C25E011EF209"
            },
            {
                typeof(BoltAttributesComponent),
                "285C891D-4A8F-484B-AD6C-7BA5E6D2BECF"
            },
            {
                typeof(ModifyBoltComponent),
                "EAF4CD74-D728-43BF-ACC1-6ACF0EF8D6F3"
            },
            {
                typeof(WeldAttributesParam),
                "3A00A7BB-16F0-4F85-B564-1501548596D0"
            },
            {
                typeof(WeldAttributesComponent),
                "C338DEF9-FE2C-4C37-BD91-B970F676C8B4"
            },
            {
                typeof(ModifyWeldComponent),
                "70A547A6-F2D3-48FE-A2E3-E5CA03CD0109"
            },
            {
                typeof(GetBoltPropertiesComponent),
                "84AD7FFD-7124-435C-ACB3-EBC311C0745E"
            },
            {
                typeof(GetReinforcementPropertiesComponent),
                "16DA2C14-E971-46A9-AF34-03C2F7B25A2E"
            },
            {
                typeof(SelectModelObjectComponent),
                "E74EA104-BF59-438A-91B0-B4EF3513AAF7"
            },
            {
                typeof(PerformNumberingComponent),
                "7DB6A0CB-4D87-47FA-A7E9-FA34DD908BEE"
            },
            {
                typeof(CreateAssociativeNoteComponent),
                "CA0BD263-FD65-4145-A31E-F8740A3C12C1"
            },
            {
                typeof(CreateAssociativeNoteComponentOLD),
                "7D4A48E0-1CB9-4D49-BAD6-8B04494C532E"
            },
            {
                typeof(TextAttributesParam),
                "00A1ED2E-9166-442F-9F17-DE7A08575A57"
            },
            {
                typeof(MarkAttributesParam),
                "7F904691-96A2-4F92-B95F-423DB7020E4D"
            },
            {
                typeof(MarkAttributesComponent),
                "F0939985-9E2E-4677-957F-F56C07AE6BEF"
            },
            {
                typeof(GetRelatedObjectsComponent),
                "597CE529-4306-4F4E-BF80-01C93E3613A5"
            },
            {
                typeof(ConvertModelToDrawingObjectComponent),
                "062D04D8-CA58-4DF3-A6CA-708EDB16518D"
            },
            {
                typeof(CreateMarkComponent),
                "EEA2C71F-2778-4F47-806A-8C23F238ABEB"
            },
            {
                typeof(CreateLineComponent),
                "94A8AC95-14FF-477F-99F0-6A2F1C9C9E09"
            },
            {
                typeof(CreateLineComponentOLD),
                "4D6C495A-FE2F-4990-9CBC-51B5BBEBBEA4"
            },
            {
                typeof(LineAttributesComponent),
                "DEA25FDD-43B1-4B44-9517-08891061592A"
            },
            {
                typeof(LineAttributesParam),
                "17ADE8BF-8120-43F4-9822-95E05E875428"
            },
            {
                typeof(CreatePolylineComponent),
                "620639B5-25F4-4636-835C-E5EDE57803BB"
            },
            {
                typeof(CreatePolylineComponentOLD),
                "EF81C339-5E7D-4D35-A8D5-2E1FEF1C4ACF"
            },
            {
                typeof(PolylineAttributesComponent),
                "B99FCD16-DDBF-4B0E-A45B-F38C7F3A258C"
            },
            {
                typeof(PolylineAttributesParam),
                "7D41B46D-B151-4F4C-832D-4459758EFA04"
            },
            {
                typeof(EmbeddedObjectAttributesParam),
                "E5A2A179-1400-4E9B-9CC7-6BD0D081F165"
            },
            {
                typeof(EmbeddedObjectAttributesComponent),
                "1726839C-46FF-4DE9-B7DC-C418C80C3EE4"
            },
            {
                typeof(CreateEmbeddedObjectComponent),
                "098B017F-FCD9-4DCE-BE22-BF797072B574"
            },
            {
                typeof(CreateEmbeddedObjectComponentOLD),
                "215A4809-255C-4D74-B19F-E6CF20B8F4AF"
            },
            {
                typeof(CreateDrawingLibraryComponent),
                "2563B865-F742-4BCD-B10C-97E27F3F3E44"
            },
            {
                typeof(CreateDrawingLibraryComponentOLD),
                "C896EED1-A8E9-412E-927B-1E2E80A0C046"
            },
            {
                typeof(LoopStartComponent),
                LoopStart
            },
            {
                typeof(LoopEndComponent),
                LoopEnd
            },
            {
                typeof(ModifyDrawingPropertiesComponent),
                "F8CED9AF-8543-435E-B0D1-AB29F5A65CEE"
            },
            {
                typeof(RotateViewComponent),
                "3CFBE786-A757-4700-94F3-3CF0976C421C"
            },
            {
                typeof(FindVisibleEdgesComponent),
                "CB585309-AE84-45AD-BA49-236237548D6E"
            },
            {
                typeof(BrepProjectionBorderComponent),
                "CAF205C7-EB01-454B-97DF-A79DD04B7442"
            },
            {
                typeof(SearchUsingKeyComponent),
                "9B5CF5F4-0B09-4E47-A07F-38155EDCBEC8"
            },
            {
                typeof(DimensionBoxComponent),
                "7B783C3F-FBD3-46A0-84EA-DFAD8044A07E"
            },
            {
                typeof(SortByVectorComponent),
                "6FD4AC31-DBFE-47FD-8783-24012B476EBE"
            },
            {
                typeof(SortByKeyComponent),
                "882B40E3-4380-44CA-8343-94C88F21DD7B"
            },
            {
                typeof(SortByKeyComponentOLD),
                "58C34E35-F00F-4339-AAD2-7B80F87CFA83"
            },
            {
                typeof(BakeToTeklaComponent),
                "04EFCB01-3336-47AC-BCC1-5F88BD92E075"
            },
            {
                typeof(SimpleOrientComponent),
                "BA991EA0-3D32-42C6-94B1-805E929B4EA9"
            },
            {
                typeof(CreateWDrawingComponent),
                "93A82029-7D54-444B-AF45-FB981DBA00D4"
            },
            {
                typeof(TeklaIndexComponent),
                "8F3115FD-0145-4B90-9465-AF6EE3481AEA"
            },
            {
                typeof(GetGridPropertiesComponent),
                "2F003252-EB97-4606-9F4C-EAA8FEB7BE7A"
            },
            {
                typeof(GetEditModeComponent),
                "0AFEE462-9765-4668-89FD-64C489EF51C1"
            },
            {
                typeof(DeconstructPluginComponent),
                "9FE80ACE-E012-4445-A09E-9D8520E7DE42"
            },
            {
                typeof(CreateDetailMarkComponent),
                "C409A121-516F-4322-8684-605CC9CD1A5C"
            },
            {
                typeof(CreateSectionMarkComponent),
                "DC8679D0-4117-404D-AF86-F7B21009AC47"
            },
            {
                typeof(SplitGeometryComponent),
                "484C79D1-1884-4AEC-A1F0-0330C50B58BB"
            },
            {
                typeof(GetSelectedModelObjectComponent),
                "4A4F31A7-66AC-422D-8239-71CAC78EA899"
            },
            {
                typeof(PlacingBaseParam),
                "DB23821E-C5D1-48CC-AA1F-DF4D4BCE0FAC"
            },
            {
                typeof(PlacingBaseComponent),
                "6A659AE3-3DE0-4B36-950C-2D90E56636B0"
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
