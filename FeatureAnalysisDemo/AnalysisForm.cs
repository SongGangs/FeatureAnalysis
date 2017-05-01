using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;

namespace FeatureAnalysisDemo
{
    public partial class AnalysisForm : Form
    {
        private IGeometry m_Geometry = null;
        private IMapControl3 m_mapControl = null;
        private IFeatureClass m_featureClass = null;
        private IArray m_array = null;
        public static AnalysisForm curForm = null;
        private  double m_Radius = 0;//缓冲区半径
        /// <summary>
        /// 规划要素
        /// </summary>
        public IGeometry Geometry
        {
            get { return m_Geometry; }
            set { m_Geometry = value; }
        }

        /// <summary>
        /// 外部地图控件
        /// </summary>
        public IMapControl3 MapControl
        {
            get { return m_mapControl; }
            set { m_mapControl = value; }
        }

        /// <summary>
        /// 获取和设置要素类属性的主要接口
        /// </summary>
        public IFeatureClass FeatureClass
        {
            get { return m_featureClass; }
            set { m_featureClass = value; }
        }

        /// <summary>
        /// 缓冲区半径
        /// </summary>
        public double Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }

        public AnalysisForm()
        {
            InitializeComponent();
        }

       
        private void Btu_Analysis_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            
            try
            {
                listView1.Items.Clear();
                if (m_Geometry == null)
                {
                    return;
                }
                IField filed = null;
                int index = 0;
                for (int i = 0; i < m_featureClass.Fields.FieldCount; i++)
                {
                    if (comboBox_Filed.SelectedItem.ToString() == m_featureClass.Fields.get_Field(i).AliasName)
                    {
                        //获取选择字段的 字段名与在要素集中的索引位置
                        filed = m_featureClass.Fields.get_Field(i);
                        index = m_featureClass.Fields.FindFieldByAliasName(comboBox_Filed.SelectedItem.ToString());
                        break;
                    }
                }
                if (filed == null)
                {
                    return;
                }
                // ITopologicalOperator接口用来通过对已存在的几何对象做空间拓扑运算以产生新的结合对象。
                ITopologicalOperator Topo = null;
                IGeometry BuferGeo = null;
                if (m_Geometry.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    BuferGeo = m_Geometry;
                    Help.MapHelper.seek_IntersectFeature(m_Geometry, m_featureClass,ref m_array);
                }
                else
                {
                    Topo = m_Geometry as ITopologicalOperator;
                    double radius = m_Radius; //对线要素建立缓冲区 设置半径赋值赋值
                    BuferGeo = Topo.Buffer(radius);
                    Help.MapHelper.DrwaElementTransparent(BuferGeo, m_mapControl);
                    //m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null,null);
                    Help.MapHelper.seek_IntersectFeature(BuferGeo, m_featureClass, ref m_array);
                }
                //IFeature 要素   IElement元素
                IFeature feature = null;
                IGeometry geometry = null;
                bool Exist = false;//判断要素是否在listview存在
                double Area = 0;//面积
                ListViewItem item = null;
                for (int i = 0; i < m_array.Count; i++)
                {
                    Exist = false;
                    feature = m_array.get_Element(i) as IFeature;
                    geometry = Help.MapHelper.seek_InnerGeometry(feature.Shape, BuferGeo);
                    Area = ((IArea) geometry).Area;
                    for (int j = 0; j < listView1.Items.Count; j++)
                    {
                        if (listView1.Items[j].Text==feature.get_Value(index).ToString())
                        {
                            Exist = true;
                            Area = Area + Convert.ToDouble(listView1.Items[j].SubItems[1].Text);
                            listView1.Items[j].SubItems[1].Text = Area.ToString("0.00000");
                            break;
                        }
                    }
                    if (Exist==false)
                    {
                        item=new ListViewItem();
                        item.Text = feature.get_Value(index).ToString();
                        item.SubItems.Add(Area.ToString("0.00000"));
                        listView1.Items.Add(item);
                       //this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
                    }
                }
                
               // Help.MapHelper.ExporterMap(m_mapControl.ActiveView, Application.StartupPath+"\\tip.jpg",100,13,10);
                Help.MapHelper.ExporterMap(m_mapControl.ActiveView, Application.StartupPath+"\\tip.jpg");
                System.IO.FileStream fs = new System.IO.FileStream(Application.StartupPath + "\\tip.jpg",FileMode.Open,FileAccess.Read);
                pictureBox1.Image=Image.FromStream(fs);
                fs.Close();
                fs.Dispose();
            }
            catch (Exception)
            {

            }
        }

        
        public void RefreshGeometry()
        {
            //清楚地图中已经绘制的要素
            IGraphicsContainer graphics = m_mapControl.Map as IGraphicsContainer;
            graphics.DeleteAllElements();
            Help.MapHelper.DrwaElementTransparent(m_Geometry, m_mapControl);
            m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        

      

        

        private void comboBox_Layer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < m_mapControl.LayerCount; i++)
                {
                    //提取地图的要素集————FeatureClass就相当于一个shapeFile,
                    if (((IFeatureLayer)m_mapControl.get_Layer(i)).FeatureClass.ShapeType==esriGeometryType.esriGeometryPolygon&& ((IFeatureLayer)m_mapControl.get_Layer(i)).FeatureClass.AliasName==comboBox_Layer.SelectedItem.ToString())
                    {
                        m_featureClass = ((IFeatureLayer) m_mapControl.get_Layer(i)).FeatureClass;
                        break;
                    }
                }
                if (m_featureClass==null)
                {
                    return;;
                }
                //更新字段 但是在之前先清空字段  
                comboBox_Filed.Items.Clear();
                for (int i = 0; i < m_featureClass.Fields.FieldCount; i++)
                {
                    //判断要素字段的种类 取出string类型的
                    if (m_featureClass.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeOID || m_featureClass.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry || m_featureClass.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeDouble || m_featureClass.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeSingle)
                    {
                        continue;
                    }
                    else
                    {
                        comboBox_Filed.Items.Add(m_featureClass.Fields.get_Field(i).AliasName);
                    }
                }
                for (int i = 0; i < comboBox_Filed.Items.Count; i++)
                {
                    if (comboBox_Filed.Items[i].ToString()=="地类名称")
                    {
                        comboBox_Filed.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void AnalysisForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            if (m_mapControl != null)
            {
                for (int i = 0; i < m_mapControl.LayerCount; i++)
                {
                    //判断底图是面域
                    if (((IFeatureLayer) m_mapControl.get_Layer(i)).FeatureClass.ShapeType ==
                        esriGeometryType.esriGeometryPolygon) 
                    {
                        comboBox_Layer.Items.Add(((IFeatureLayer) m_mapControl.get_Layer(i)).FeatureClass.AliasName);
                    }
                }
                if (comboBox_Layer.Items.Count > 0)
                {
                    if (m_featureClass != null)
                    {
                        for (int i = 0; i < comboBox_Layer.Items.Count; i++)
                        {
                            if (comboBox_Layer.Items[i].ToString() == m_featureClass.AliasName)
                            {
                                comboBox_Layer.SelectedIndex = i;
                                break;
                            }
                        }
                        if (comboBox_Layer.SelectedItem == null)
                        {
                            comboBox_Layer.SelectedIndex = 0;
                        }
                    }
                }
            }
        }


    }
}
