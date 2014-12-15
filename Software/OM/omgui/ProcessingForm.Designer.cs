﻿namespace OmGui
{
    partial class ProcessingForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessingForm));
            this.groupBoxProgress = new System.Windows.Forms.GroupBox();
            this.textBoxProgress = new System.Windows.Forms.TextBox();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.pictureBoxProgress = new System.Windows.Forms.PictureBox();
            this.groupBoxProgress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxProgress
            // 
            this.groupBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProgress.Controls.Add(this.textBoxProgress);
            this.groupBoxProgress.Location = new System.Drawing.Point(13, 13);
            this.groupBoxProgress.Name = "groupBoxProgress";
            this.groupBoxProgress.Size = new System.Drawing.Size(750, 345);
            this.groupBoxProgress.TabIndex = 0;
            this.groupBoxProgress.TabStop = false;
            this.groupBoxProgress.Text = "Progress";
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProgress.Location = new System.Drawing.Point(7, 16);
            this.textBoxProgress.Multiline = true;
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ReadOnly = true;
            this.textBoxProgress.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxProgress.Size = new System.Drawing.Size(737, 323);
            this.textBoxProgress.TabIndex = 0;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 250;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(543, 364);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(104, 45);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(653, 364);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(104, 45);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // pictureBoxProgress
            // 
            this.pictureBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBoxProgress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxProgress.Image = global::OmGui.Properties.Resources.loading2;
            this.pictureBoxProgress.InitialImage = null;
            this.pictureBoxProgress.Location = new System.Drawing.Point(20, 364);
            this.pictureBoxProgress.Name = "pictureBoxProgress";
            this.pictureBoxProgress.Size = new System.Drawing.Size(47, 45);
            this.pictureBoxProgress.TabIndex = 2;
            this.pictureBoxProgress.TabStop = false;
            this.pictureBoxProgress.Visible = false;
            // 
            // ProcessingForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(775, 421);
            this.Controls.Add(this.pictureBoxProgress);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProcessingForm";
            this.Text = "Processing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcessingForm_FormClosing);
            this.Load += new System.EventHandler(this.ProcessingForm_Load);
            this.groupBoxProgress.ResumeLayout(false);
            this.groupBoxProgress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProgress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxProgress;
        private System.Windows.Forms.TextBox textBoxProgress;
        private System.Windows.Forms.Timer timerUpdate;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.PictureBox pictureBoxProgress;
    }
}