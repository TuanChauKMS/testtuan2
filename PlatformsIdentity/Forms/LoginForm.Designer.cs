using System.Windows.Forms;
using System.ComponentModel;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Forms
{
	partial class LoginForm : Form
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.webBrowserControl = new System.Windows.Forms.WebBrowser();
			this.initialPanel = new System.Windows.Forms.Panel();
			this.iconPictureBox = new System.Windows.Forms.PictureBox();
			this.loginButton = new System.Windows.Forms.Button();
			this.signupLinkLabel = new System.Windows.Forms.LinkLabel();
			this.orLabel = new System.Windows.Forms.Label();
			this.browserPanel = new System.Windows.Forms.Panel();
			this.initialPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
			this.browserPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// webBrowserControl
			// 
			this.webBrowserControl.AllowWebBrowserDrop = false;
			resources.ApplyResources(this.webBrowserControl, "webBrowserControl");
			this.webBrowserControl.IsWebBrowserContextMenuEnabled = false;
			this.webBrowserControl.Name = "webBrowserControl";
			this.webBrowserControl.ScriptErrorsSuppressed = true;
			this.webBrowserControl.TabStop = false;
			this.webBrowserControl.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WebBrowserControl_DocumentCompleted);
			this.webBrowserControl.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserControl_Navigated);
			// 
			// initialPanel
			// 
			this.initialPanel.Controls.Add(this.iconPictureBox);
			this.initialPanel.Controls.Add(this.loginButton);
			this.initialPanel.Controls.Add(this.signupLinkLabel);
			this.initialPanel.Controls.Add(this.orLabel);
			resources.ApplyResources(this.initialPanel, "initialPanel");
			this.initialPanel.Name = "initialPanel";
			// 
			// iconPictureBox
			// 
			resources.ApplyResources(this.iconPictureBox, "iconPictureBox");
			this.iconPictureBox.Name = "iconPictureBox";
			this.iconPictureBox.TabStop = false;
			// 
			// loginButton
			// 
			resources.ApplyResources(this.loginButton, "loginButton");
			this.loginButton.Name = "loginButton";
			this.loginButton.UseVisualStyleBackColor = true;
			this.loginButton.Click += new System.EventHandler(this.LoginButton_Clicked);
			// 
			// signupLinkLabel
			// 
			resources.ApplyResources(this.signupLinkLabel, "signupLinkLabel");
			this.signupLinkLabel.Name = "signupLinkLabel";
			this.signupLinkLabel.TabStop = true;
			this.signupLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SignupLinkLabel_LinkClicked);
			// 
			// orLabel
			// 
			resources.ApplyResources(this.orLabel, "orLabel");
			this.orLabel.Name = "orLabel";
			// 
			// browserPanel
			// 
			this.browserPanel.Controls.Add(this.webBrowserControl);
			resources.ApplyResources(this.browserPanel, "browserPanel");
			this.browserPanel.Name = "browserPanel";
			// 
			// LoginForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.initialPanel);
			this.Controls.Add(this.browserPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.initialPanel.ResumeLayout(false);
			this.initialPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
			this.browserPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

        #endregion

		private WebBrowser webBrowserControl;
		private Panel initialPanel;
		private Button loginButton;
		private Label orLabel;
		private LinkLabel signupLinkLabel;
		private PictureBox iconPictureBox;
		private Panel browserPanel;
	}
}