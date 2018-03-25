using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ue4Toolkit
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripFileButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripOpenProjectItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripConnectEngineItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLoadedProjectLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLoadedEngineLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFileButton,
            this.toolStripLoadedProjectLabel,
            this.toolStripLoadedEngineLabel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(563, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripFileButton
            // 
            this.toolStripFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripFileButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripOpenProjectItem,
            this.toolStripConnectEngineItem});
            this.toolStripFileButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripFileButton.Image")));
            this.toolStripFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripFileButton.Name = "toolStripFileButton";
            this.toolStripFileButton.Size = new System.Drawing.Size(38, 22);
            this.toolStripFileButton.Text = "File";
            // 
            // toolStripOpenProjectItem
            // 
            this.toolStripOpenProjectItem.Name = "toolStripOpenProjectItem";
            this.toolStripOpenProjectItem.Size = new System.Drawing.Size(180, 22);
            this.toolStripOpenProjectItem.Text = "Open UE4 Project";
            // 
            // toolStripConnectEngineItem
            // 
            this.toolStripConnectEngineItem.Name = "toolStripConnectEngineItem";
            this.toolStripConnectEngineItem.Size = new System.Drawing.Size(180, 22);
            this.toolStripConnectEngineItem.Text = "Set UE4 Engine Path";
            // 
            // toolStripLoadedProjectLabel
            // 
            this.toolStripLoadedProjectLabel.Name = "toolStripLoadedProjectLabel";
            this.toolStripLoadedProjectLabel.Size = new System.Drawing.Size(63, 22);
            this.toolStripLoadedProjectLabel.Text = "No Project";
            // 
            // toolStripLoadedEngineLabel
            // 
            this.toolStripLoadedEngineLabel.Name = "toolStripLoadedEngineLabel";
            this.toolStripLoadedEngineLabel.Size = new System.Drawing.Size(121, 22);
            this.toolStripLoadedEngineLabel.Text = "No Engine Assosiated";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 185);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.Text = "UE4 Toolkit";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripDropDownButton toolStripFileButton;
        private ToolStripMenuItem toolStripOpenProjectItem;
        private ToolStripMenuItem toolStripConnectEngineItem;
        private ToolStripLabel toolStripLoadedProjectLabel;
        private ToolStripLabel toolStripLoadedEngineLabel;
    }
}

