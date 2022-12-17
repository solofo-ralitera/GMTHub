using GMTHubLib.Com;
using GMTHubLib.Utils;
using System.Threading.Tasks;
using System;
using GMTHubUi.Classes;

namespace GMTHubUi
{
    partial class GMTHubUi
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GMTHubUi));
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.tabPageTelemetry = new System.Windows.Forms.TabPage();
            this.richTextBoxTelemetry = new ConsoleTextBox();
            this.tabPageConfiguration = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonJoystickCopyButton = new System.Windows.Forms.Button();
            this.textBoxJoystickListener = new System.Windows.Forms.TextBox();
            this.labelTextBoxJoystickListener = new System.Windows.Forms.Label();
            this.MainTabControl.SuspendLayout();
            this.tabPageTelemetry.SuspendLayout();
            this.tabPageConfiguration.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.tabPageTelemetry);
            this.MainTabControl.Controls.Add(this.tabPageConfiguration);
            this.MainTabControl.Controls.Add(this.tabPage1);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(800, 450);
            this.MainTabControl.TabIndex = 0;
            // 
            // tabPageTelemetry
            // 
            this.tabPageTelemetry.Controls.Add(this.richTextBoxTelemetry);
            this.tabPageTelemetry.Location = new System.Drawing.Point(4, 22);
            this.tabPageTelemetry.Name = "tabPageTelemetry";
            this.tabPageTelemetry.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTelemetry.Size = new System.Drawing.Size(792, 424);
            this.tabPageTelemetry.TabIndex = 0;
            this.tabPageTelemetry.Text = "Telemetry";
            this.tabPageTelemetry.UseVisualStyleBackColor = true;
            // 
            // richTextBoxTelemetry
            // 
            this.richTextBoxTelemetry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxTelemetry.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxTelemetry.Name = "richTextBoxTelemetry";
            this.richTextBoxTelemetry.ReadOnly = true;
            this.richTextBoxTelemetry.Size = new System.Drawing.Size(786, 418);
            this.richTextBoxTelemetry.TabIndex = 0;
            this.richTextBoxTelemetry.Text = "";
            // 
            // tabPageConfiguration
            // 
            this.tabPageConfiguration.Controls.Add(this.label1);
            this.tabPageConfiguration.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfiguration.Name = "tabPageConfiguration";
            this.tabPageConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfiguration.Size = new System.Drawing.Size(792, 424);
            this.tabPageConfiguration.TabIndex = 1;
            this.tabPageConfiguration.Text = "Configuration";
            this.tabPageConfiguration.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Work in progress";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.labelTextBoxJoystickListener);
            this.tabPage1.Controls.Add(this.buttonJoystickCopyButton);
            this.tabPage1.Controls.Add(this.textBoxJoystickListener);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 424);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Joystick";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonJoystickCopyButton
            // 
            this.buttonJoystickCopyButton.Location = new System.Drawing.Point(719, 27);
            this.buttonJoystickCopyButton.Name = "buttonJoystickCopyButton";
            this.buttonJoystickCopyButton.Size = new System.Drawing.Size(70, 20);
            this.buttonJoystickCopyButton.TabIndex = 1;
            this.buttonJoystickCopyButton.Text = "Copy";
            this.buttonJoystickCopyButton.UseVisualStyleBackColor = true;
            this.buttonJoystickCopyButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxJoystickListener
            // 
            this.textBoxJoystickListener.Location = new System.Drawing.Point(0, 27);
            this.textBoxJoystickListener.Name = "textBoxJoystickListener";
            this.textBoxJoystickListener.Size = new System.Drawing.Size(713, 20);
            this.textBoxJoystickListener.TabIndex = 0;
            // 
            // labelTextBoxJoystickListener
            // 
            this.labelTextBoxJoystickListener.AutoSize = true;
            this.labelTextBoxJoystickListener.Location = new System.Drawing.Point(6, 11);
            this.labelTextBoxJoystickListener.Name = "labelTextBoxJoystickListener";
            this.labelTextBoxJoystickListener.Size = new System.Drawing.Size(308, 13);
            this.labelTextBoxJoystickListener.TabIndex = 2;
            this.labelTextBoxJoystickListener.Text = "Last joystick button pressed (use in configuration file, page_key)";
            // 
            // GMTHubUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GMTHubUi";
            this.Text = "GMT Dashboard";
            this.Load += new System.EventHandler(this.GMTHubUi_Load);
            this.Shown += new System.EventHandler(this.GMTHubUi_Shown);
            this.MainTabControl.ResumeLayout(false);
            this.tabPageTelemetry.ResumeLayout(false);
            this.tabPageConfiguration.ResumeLayout(false);
            this.tabPageConfiguration.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage tabPageTelemetry;
        private System.Windows.Forms.TabPage tabPageConfiguration;
        private ConsoleTextBox richTextBoxTelemetry;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button buttonJoystickCopyButton;
        private System.Windows.Forms.TextBox textBoxJoystickListener;
        private System.Windows.Forms.Label labelTextBoxJoystickListener;
    }
}

