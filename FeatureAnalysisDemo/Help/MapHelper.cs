using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;

namespace FeatureAnalysisDemo.Help
{
    internal class MapHelper
    {
        /// <summary>
        /// 获取覆盖区域
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="featureClass"></param>
        public static void seek_IntersectFeature(IGeometry geometry, IFeatureClass featureClass, ref IArray array)
        {
            try
            {
                IFeature feature = null;
                if (array != null)
                {
                    //让我们显示控制COM对象的生存==你调用这个方法，就是通过.NET环境，告诉COM对象，不用你了，马上消失。）
                    Marshal.ReleaseComObject(array);
                }
                array = new ESRI.ArcGIS.esriSystem.ArrayClass();
                //SpatialFilter空间查询
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                IQueryFilter pQueryFilter = spatialFilter as IQueryFilter;

                // 设置空间过滤器的范围（多边形）  
                spatialFilter.Geometry = geometry;
                // 设置空间过滤器空间关系类型
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialFilter.WhereClause = null;
                //获取FeatureCursor游标
                IFeatureCursor featureCursor = featureClass.Search(pQueryFilter, false);
                //遍历FeatureCursor
                feature = featureCursor.NextFeature();
                while(feature != null)
                {
                    //获取要素对象
                    array.Add(feature);
                    feature = featureCursor.NextFeature();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 裁剪输入的几何要素，保留相交的要素部分
        /// </summary>
        /// <param name="SrcGeo"></param>
        /// <param name="ClipGeo"></param>
        /// <returns></returns>
        public static IGeometry seek_InnerGeometry(IGeometry SrcGeo, IGeometry ClipGeo)
        {
            try
            {
                if (ClipGeo.GeometryType != esriGeometryType.esriGeometryPolygon)
                {
                    MessageBox.Show("请检查覆盖区域!");
                    return null;
                }
                //统一参考系
                if (SrcGeo.SpatialReference != null && ClipGeo.SpatialReference != null)
                {
                    ClipGeo.Project(SrcGeo.SpatialReference);
                }
                IGeometry geometry = null;
                ITopologicalOperator topo = SrcGeo as ITopologicalOperator;
                topo.Simplify();//是拓扑关系最简化
                geometry = topo.Intersect(ClipGeo, esriGeometryDimension.esriGeometry2Dimension);
                return geometry;
            }
            catch (Exception)
            {
                return null;
            }
        }
       
        
        /// <summary>
        /// 出图
        /// </summary>
        /// <param name="pActiveView"></param>
        /// <param name="fileName"></param>
        public static void ExporterMap(IActiveView pActiveView, string fileName)
        {
            IExport pExporter = new ExportJPEGClass();
            //IEnvelope是指地物的外接矩形，用来表示地物图形的大体位置和形状，一般可用于检索地物，判断地物间的拓扑关系，
            //可以使得检索、判断的速度加快，因为有了IEnvelope，可以首先判断该外接矩形是否在检索范围内，而判断一个外接矩形是比较简单的。
            IEnvelope pEnvelope = new EnvelopeClass();
            // 设置跟踪取消对象   可用于取消操作
            ITrackCancel pTrackCancel = new CancelTrackerClass();
            tagRECT ptagRECT;
            ptagRECT.left = 0;
            ptagRECT.top = 0;
            ptagRECT.right = (int)pActiveView.Extent.Width;
            ptagRECT.bottom = (int)pActiveView.Extent.Height;
            // 获取输出分辨率 96 ——高清 
            int pResolution = (int)(pActiveView.ScreenDisplay.DisplayTransformation.Resolution);
            // 设置一个边框范围
            pEnvelope.PutCoords(ptagRECT.left, ptagRECT.bottom, ptagRECT.right, ptagRECT.top);
            //出图分辨率
            pExporter.Resolution = pResolution;
            pExporter.ExportFileName = fileName;
            // 将打印像素范围 设置给输出对象
            pExporter.PixelBounds = pEnvelope;
            // 进行视图控件的视图输出操作，设置对应参数 
            pActiveView.Output(pExporter.StartExporting(), pResolution, ref ptagRECT, pActiveView.Extent, pTrackCancel);
            pExporter.FinishExporting();
            //释放资源
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pExporter);
        }

        /// <summary>
        /// 绘制图元
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="mapControl"></param>
        public static void DrwaElementTransparent(IGeometry geometry, IMapControl3 mapControl)
        {
            try
            {
                //IGraphicsContainer主要是管理map上Element对象的
                IGraphicsContainer graphics = mapControl.Map as IGraphicsContainer;
                if (geometry.IsEmpty)
                {
                    return;
                }
                ILineElement lineElement = null;
                IPolygonElement polygonElement = null;
                IElement element = null;
                //线状几何要素
                if (geometry.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    lineElement = new LineElementClass();
                    element = lineElement as IElement;
                }
                //面状几何要素
                else if (geometry.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    polygonElement = new PolygonElementClass();
                    element = polygonElement as IElement;
                }
                element.Geometry = geometry;
                IRgbColor color =  new RgbColorClass(){Red = 0,Blue = 255,Green = 0 };

                color.Transparency = 255;//透明度 0为透明 255为不透明
                //产生一个线符号对象
                ILineSymbol outline = new SimpleLineSymbolClass();
                //设置线符号的属性
                outline.Color = color;
                outline.Width = 1;
                if (geometry.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    lineElement.Symbol = outline;
                    element = lineElement as IElement;
                    graphics.AddElement(element, 0);
                    //ARCGIS部分刷新  IActiveView代表Map对象
                    /*刷新图层： 
pMap.PartialRefresh(esriViewGeography, pLayer, null);
刷新所有图层： 
pMap.PartialRefresh(esriViewGeography, null, null);
刷新所选择的对象： 
pMap.PartialRefresh(esriViewGeoSelection, null, null);
刷新标注： 
pMap.PartialRefresh(esriViewGraphics, null, null);
刷新图元 
pLayout.PartialRefresh(esriViewGraphics, pElement, null);
刷新所有图元 
pLayout.PartialRefresh(esriViewGraphics, null, null);
刷新所选择的图元 
pLayout.PartialRefresh(esriViewGraphicSelection, null, null);
                     */
                    mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    return;
                }
                //设置颜色属性
                color = new RgbColorClass() { Red = 255, Blue = 0, Green = 0 };
                color.Transparency = 0;//将面欲设置为透明
                //设置填充符号的属性
                IFillSymbol fillSymbol = new SimpleFillSymbolClass();
                fillSymbol.Color = color;

                //产生一个线符号对象
                outline = new SimpleLineSymbolClass();
                //设置线符号的属性
                color.Transparency = 255;//透明度 0为透明 255为不透明
                outline.Color = color;
                outline.Width = 2;
                fillSymbol.Outline = outline;

                IFillShapeElement fillShapeElement = element as IFillShapeElement;
                fillShapeElement.Symbol = fillSymbol;
                element = fillShapeElement as IElement;
                graphics.AddElement(element, 0);
                IEnvelope envelope = element.Geometry.Envelope;
                envelope.Expand(1.1, 1.1, true);//适当缩小覆盖区域
                mapControl.Extent = envelope;
                mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch (Exception)
            {
            }
        }
    }
}
