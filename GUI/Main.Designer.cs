﻿namespace Grimoire.GUI
{
    partial class Main
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabs_cMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabs_cMenu_clear = new System.Windows.Forms.ToolStripMenuItem();
            this.tabs_cMenu_close = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.new_list = new System.Windows.Forms.ComboBox();
            this.settings = new System.Windows.Forms.Button();
            this.tabs_cMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.ContextMenuStrip = this.tabs_cMenu;
            this.tabs.Location = new System.Drawing.Point(9, 32);
            this.tabs.Margin = new System.Windows.Forms.Padding(2);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(760, 472);
            this.tabs.TabIndex = 1;
            // 
            // tabs_cMenu
            // 
            this.tabs_cMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tabs_cMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tabs_cMenu_clear,
            this.tabs_cMenu_close});
            this.tabs_cMenu.Name = "grid_cMenu";
            this.tabs_cMenu.Size = new System.Drawing.Size(104, 48);
            // 
            // tabs_cMenu_clear
            // 
            this.tabs_cMenu_clear.Name = "tabs_cMenu_clear";
            this.tabs_cMenu_clear.Size = new System.Drawing.Size(103, 22);
            this.tabs_cMenu_clear.Text = "Clear";
            this.tabs_cMenu_clear.Click += new System.EventHandler(this.tabs_cMenu_clear_Click);
            // 
            // tabs_cMenu_close
            // 
            this.tabs_cMenu_close.Name = "tabs_cMenu_close";
            this.tabs_cMenu_close.Size = new System.Drawing.Size(103, 22);
            this.tabs_cMenu_close.Text = "Close";
            this.tabs_cMenu_close.Click += new System.EventHandler(this.tabs_cMenu_close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "New:";
            // 
            // new_list
            // 
            this.new_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.new_list.FormattingEnabled = true;
            this.new_list.Items.AddRange(new object[] {
            "RDB",
            "HASHER"});
            this.new_list.Location = new System.Drawing.Point(43, 7);
            this.new_list.Margin = new System.Windows.Forms.Padding(2);
            this.new_list.Name = "new_list";
            this.new_list.Size = new System.Drawing.Size(92, 21);
            this.new_list.TabIndex = 3;
            this.new_list.SelectedIndexChanged += new System.EventHandler(this.new_list_SelectedIndexChanged);
            // 
            // settings
            // 
            this.settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settings.Location = new System.Drawing.Point(712, 5);
            this.settings.Margin = new System.Windows.Forms.Padding(2);
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(56, 23);
            this.settings.TabIndex = 4;
            this.settings.Text = "Settings";
            this.settings.UseVisualStyleBackColor = true;
            this.settings.Click += new System.EventHandler(this.settings_Click);
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 521);
            this.Controls.Add(this.settings);
            this.Controls.Add(this.new_list);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabs);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(795, 560);
            this.Name = "Main";
            this.Text = "Grimoire";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            this.tabs_cMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox new_list;
        private System.Windows.Forms.ContextMenuStrip tabs_cMenu;
        private System.Windows.Forms.ToolStripMenuItem tabs_cMenu_clear;
        private System.Windows.Forms.ToolStripMenuItem tabs_cMenu_close;
        private System.Windows.Forms.Button settings;
    }
}

