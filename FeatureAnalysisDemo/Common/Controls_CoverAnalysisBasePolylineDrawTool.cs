using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace FeatureAnalysisDemo.Common
{
    /// <summary>
    /// Summary description for Controls_CoverAnalysisBasePolylineDrawTool.
    /// </summary>
    [Guid("4a10c637-e1e8-4e33-bf93-b45f8021e8fe")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("FeatureAnalysisDemo.Controls_CoverAnalysisBasePolylineDrawTool")]
    public sealed class Controls_CoverAnalysisBasePolylineDrawTool : BaseTool
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

        public Controls_CoverAnalysisBasePolylineDrawTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "基于线状地物的覆盖分析"; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "This should work in ArcMap/MapControl/PageLayoutControl";  //localizable text
            base.m_toolTip = "基于线状地物的覆盖分析";  //localizable text
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
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
            // TODO: Add Controls_CoverAnalysisBasePolylineDrawTool.OnClick implementation
            m_mapControl.CurrentTool = this;
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Controls_CoverAnalysisBasePolylineDrawTool.OnMouseDown implementation
            try
            {
                if (Button == 1)//表示左键
                {
                    //IGeometry需要引用Geometry
                    IGeometry Polyline = m_mapControl.TrackLine();

                    QurryDistanceForm f=new QurryDistanceForm();
                    if (f.ShowDialog()==DialogResult.OK)
                    {
                        if (AnalysisForm.curForm == null || AnalysisForm.curForm.IsDisposed == true)
                        {
                            AnalysisForm form = new AnalysisForm();
                            form.MapControl = m_mapControl;
                            form.Geometry = Polyline;
                            form.Radius = f.m_distance;
                            form.TopMost = true;//顶层显示
                            form.Show();
                            form.RefreshGeometry();
                        }
                        else
                        {
                            AnalysisForm.curForm.MapControl = m_mapControl;
                            AnalysisForm.curForm.Geometry = Polyline;
                            AnalysisForm.curForm.Radius = f.m_distance;
                            AnalysisForm.curForm.TopMost = true;//顶层显示
                            AnalysisForm.curForm.Show();
                            AnalysisForm.curForm.RefreshGeometry();
                        } 
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
            // TODO:  Add Controls_CoverAnalysisBasePolylineDrawTool.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Controls_CoverAnalysisBasePolylineDrawTool.OnMouseUp implementation
        }
        #endregion

       
    }
}
