using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace FeatureAnalysisDemo.Common
{
    /// <summary>
    /// Summary description for Controls_CoverAnalysisBasePolygonDrawTool.
    /// </summary>
    [Guid("8b1597da-57f6-4675-bcc5-864a8bc43d7a")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("FeatureAnalysisDemo.Controls_CoverAnalysisBasePolygonDrawTool")]
    public sealed class Controls_CoverAnalysisBasePolygonDrawTool : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;
        private IToolbarControl m_toolbarControl = null;
        private IMapControl3 m_mapControl = null;

        public Controls_CoverAnalysisBasePolygonDrawTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "基于面状地物的覆盖分析"; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "";  //localizable text
            base.m_toolTip = "基于面状地物的覆盖分析";  //localizable text
            base.m_name = "PolygonAnalysis";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (hook is IMapControl3)
                {
                    m_mapControl = hook as IMapControl3;
                }
                else if (hook is IToolbarControl)
                {
                    m_toolbarControl = hook as IToolbarControl;
                    m_mapControl = m_toolbarControl.Buddy as IMapControl3;
                }
                if (m_hookHelper.ActiveView == null)
                {
                    m_hookHelper = null;
                }
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add Controls_CoverAnalysisBasePolygonDrawTool.OnClick implementation
            m_mapControl.CurrentTool = this;
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Controls_CoverAnalysisBasePolygonDrawTool.OnMouseDown implementation
            try
            {
                if (Button == 1)//表示左键
                {
                    //IGeometry需要引用Geometry
                   IGeometry polygon = m_mapControl.TrackPolygon();
                    //将值传入到另一个窗体
                   if (AnalysisForm.curForm == null || AnalysisForm.curForm.IsDisposed == true)
                   {
                       AnalysisForm form = new AnalysisForm();
                       form.MapControl = m_mapControl;
                       form.Geometry = polygon;
                       form.TopMost = true;//顶层显示
                       form.Show();
                       form.RefreshGeometry();
                   }
                   else
                   {
                       AnalysisForm.curForm.MapControl = m_mapControl;
                       AnalysisForm.curForm.Geometry = polygon;
                       AnalysisForm.curForm.TopMost = true;//顶层显示
                       AnalysisForm.curForm.Show();
                       AnalysisForm.curForm.RefreshGeometry();
                   }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Controls_CoverAnalysisBasePolygonDrawTool.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Controls_CoverAnalysisBasePolygonDrawTool.OnMouseUp implementation
        }
        #endregion
    }
}
