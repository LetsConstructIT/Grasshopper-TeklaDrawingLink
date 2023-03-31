# Grasshopper-Tekla Drawing Link
Set of Grasshopper components for interacting with Tekla drawing area.

Based on great job done by Sebastian Lindholm from Trimble available in [Tekla Warehouse](https://warehouse.tekla.com/#/catalog/details/b901f77d-cfe8-4a97-894b-f4053829c297).

Originally, drawing part was not under the scope of link. I hope that as a Tekla community we can fill this gap - **LetsConstructIT!**

## Installation
You have to point .gha file with drawing components to Rhino. It can be done in exactly same manner as the [Model link](https://support.tekla.com/pl/node/107964#setup). List of available releases of Drawing link can be found at [Releases](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/releases) section.

## License
Grasshopper-Tekla Drawing Link is provided as-is under the MIT license. For more information see [LICENSE](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/LICENSE).

## Available components

* Params
  * ![Drawing Object](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/DrawingObject.png) - selects drawing object
  * ![Drawing Part](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/DrawingPart.png) - selects drawing part (which can be mapped to model object)
  * ![Drawing Point](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/DrawingPoint.png) - selects point in drawing
* Drawing
  * ![Create A Drawing](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/CreateADrawing.png) - creates assembly drawing of selected in model object
  * ![Create CU Drawing](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/CreateCUDrawing.png) - creates cast unit drawing of selected in model object
  * ![Active Drawing](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/ActiveDrawing.png) - returns currently opened drawing
  * ![Open Drawing](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/OpenDrawing.png) - opens pointed drawing
  * ![Drawing Size](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/GetDrawingSize.png) - returns size of pointed drawing
  * ![Drawing Source](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/GetDrawingSourceObject.png) - returns model object which is the parent of drawing
  * ![Views in Drawing Source](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/ViewsAtDrawing.png) - lists views inside drawing
  * ![Close Drawing](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/CloseDrawing.png) - closes active drawing
* Drawing List
  * ![All Drawings](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/AllDrawings.png) - lists all drawings in model
  * ![Selected Drawings](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/SelectedDrawingsFromList.png) - lists only selected drawings from drawing list
* View
  * ![Detail View](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/DetailView.png) - creates detail view
  * ![Section View](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/SectionView.png) - creates section view
  * ![Part View](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/PartView.png) - creates part view (Front | Top | Bottom | Back or 3D)
  * ![Related Views](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/GetRelatedViews.png) - returns section/detail views connected to provided view
  * ![View Frame Geometry](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/ViewFrame.png) - returns size and anchor points of view
  * ![View Properties Geometry](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/ViewProperties.png) - returns name and type of view
* Parts
  * ![Drawing to Model Object](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/ConvertDrawingToModelObject.png) - converts drawing part to its model counterpart (enables inter Drawing/Model workflows)
  * ![Father View](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/GetViewFromDrawingObject.png) - returns view where provided drawing object resides
  * ![Select Object](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/SelectDrawingObject.png) - selects drawing object, used before running drawing macros
* Geometry
  * ![Transform Point to View](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/TransformPointToView.png) - transforms point from global coordinate system to pointed view coordinate system
  * ![Transform Point to Local](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/TransformPointToLocal.png) - transforms point from global coordinate system to local coordinate system
  * ![Transform Point to Global](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/TransformPointToGlobal.png) - transforms point from local coordinate system to global coordinate system
* Misc
  * ![Run Macro](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/RunMacro.png) - runs model or drawing macro already saved to file or created ad-hoc (right click to select mode)
  * ![Get COG](https://github.com/LetsConstructIT/Grasshopper-TeklaDrawingLink/blob/main/src/GrasshopperTeklaDrawingLink/Icons/GetCOG.png) - return center of gravity of provided model or drawing part
  
