namespace FeatureAnalysisDemo
{
    partial class QurryDistanceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_Distance = new System.Windows.Forms.TextBox();
            this.Btu_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_Distance
            // 
            this.textBox_Distance.Location = new System.Drawing.Point(24, 22);
            this.textBox_Distance.Name = "textBox_Distance";
            this.textBox_Distance.Size = new System.Drawing.Size(159, 25);
            this.textBox_Distance.TabIndex = 0;
            // 
            // Btu_OK
            // 
            this.Btu_OK.Location = new System.Drawing.Point(238, 22);
            this.Btu_OK.Name = "Btu_OK";
            this.Btu_OK.Size = new System.Drawing.Size(81, 25);
            this.Btu_OK.TabIndex = 1;
            this.Btu_OK.Text = "确 定";
            this.Btu_OK.UseVisualStyleBackColor = true;
            this.Btu_OK.Click += new System.EventHandler(this.Btu_OK_Click);
            // 
            // QurryDistanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 68);
            this.Controls.Add(this.Btu_OK);
            this.Controls.Add(this.textBox_Distance);
            this.Name = "QurryDistanceForm";
            this.Text = "设置缓冲区距离";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Distance;
        private System.Windows.Forms.Button Btu_OK;
    }
}