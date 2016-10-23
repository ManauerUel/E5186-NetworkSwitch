namespace E5186_NetworkSwitch
{
    partial class FormAbout
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
            this.labelProgramm = new System.Windows.Forms.Label();
            this.labelCreator = new System.Windows.Forms.Label();
            this.linkLabelLocation = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelProgramm
            // 
            this.labelProgramm.AutoSize = true;
            this.labelProgramm.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgramm.Location = new System.Drawing.Point(80, 10);
            this.labelProgramm.Name = "labelProgramm";
            this.labelProgramm.Size = new System.Drawing.Size(163, 19);
            this.labelProgramm.TabIndex = 0;
            this.labelProgramm.Text = "E5186 Network-Switch";
            // 
            // labelCreator
            // 
            this.labelCreator.AutoSize = true;
            this.labelCreator.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCreator.Location = new System.Drawing.Point(156, 32);
            this.labelCreator.Name = "labelCreator";
            this.labelCreator.Size = new System.Drawing.Size(87, 23);
            this.labelCreator.TabIndex = 1;
            this.labelCreator.Text = "Blaubeere";
            // 
            // linkLabelLocation
            // 
            this.linkLabelLocation.ActiveLinkColor = System.Drawing.Color.Black;
            this.linkLabelLocation.AutoSize = true;
            this.linkLabelLocation.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelLocation.LinkColor = System.Drawing.Color.Black;
            this.linkLabelLocation.Location = new System.Drawing.Point(163, 61);
            this.linkLabelLocation.Name = "linkLabelLocation";
            this.linkLabelLocation.Size = new System.Drawing.Size(79, 14);
            this.linkLabelLocation.TabIndex = 2;
            this.linkLabelLocation.TabStop = true;
            this.linkLabelLocation.Text = "@lteforum.at";
            this.linkLabelLocation.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkLabelLocation.VisitedLinkColor = System.Drawing.Color.Black;
            this.linkLabelLocation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLocation_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::E5186_NetworkSwitch.Properties.Resources.iconmonstr_share_1_240;
            this.pictureBox1.Location = new System.Drawing.Point(12, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(62, 68);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // FormAbout
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(252, 90);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.linkLabelLocation);
            this.Controls.Add(this.labelCreator);
            this.Controls.Add(this.labelProgramm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(268, 124);
            this.MinimumSize = new System.Drawing.Size(268, 124);
            this.Name = "FormAbout";
            this.Text = "Über";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelProgramm;
        private System.Windows.Forms.Label labelCreator;
        private System.Windows.Forms.LinkLabel linkLabelLocation;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}