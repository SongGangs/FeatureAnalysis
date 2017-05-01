using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeatureAnalysisDemo
{
    public partial class QurryDistanceForm : Form
    {
        public double m_distance = 0;
        public QurryDistanceForm()
        {
            InitializeComponent();
        }

        private void Btu_OK_Click(object sender, EventArgs e)
        {
            double d = Double.Parse(this.textBox_Distance.Text.Trim());
            if (d<=0)
            {
                MessageBox.Show("请输入大于0的数");
                return;
            }
            m_distance=Double.Parse(this.textBox_Distance.Text.Trim());
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
    }
}
