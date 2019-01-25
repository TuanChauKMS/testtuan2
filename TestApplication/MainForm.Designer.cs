namespace TestApplication
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
			this.loginButton = new System.Windows.Forms.Button();
			this.reAuthenticateButton = new System.Windows.Forms.Button();
			this.logoutButton = new System.Windows.Forms.Button();
			this.GetAccessToken = new System.Windows.Forms.Button();
			this.btnRedirectEncryption = new System.Windows.Forms.Button();
			this.generalTxtBox = new System.Windows.Forms.TextBox();
			this.decryptJsonButton = new System.Windows.Forms.Button();
			this.getCreditButtn = new System.Windows.Forms.Button();
			this.getCreditAsyncButtn = new System.Windows.Forms.Button();
			this.buyCreditBttn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// loginButton
			// 
			this.loginButton.Location = new System.Drawing.Point(12, 12);
			this.loginButton.Name = "loginButton";
			this.loginButton.Size = new System.Drawing.Size(111, 35);
			this.loginButton.TabIndex = 0;
			this.loginButton.Text = "Login";
			this.loginButton.UseVisualStyleBackColor = true;
			this.loginButton.Click += new System.EventHandler(this.LoginButton_Click);
			// 
			// reAuthenticateButton
			// 
			this.reAuthenticateButton.Location = new System.Drawing.Point(144, 12);
			this.reAuthenticateButton.Name = "reAuthenticateButton";
			this.reAuthenticateButton.Size = new System.Drawing.Size(111, 35);
			this.reAuthenticateButton.TabIndex = 1;
			this.reAuthenticateButton.Text = "Re-Authenticate";
			this.reAuthenticateButton.UseVisualStyleBackColor = true;
			this.reAuthenticateButton.Click += new System.EventHandler(this.ReAuthenticateButton_Click);
			// 
			// logoutButton
			// 
			this.logoutButton.Location = new System.Drawing.Point(275, 12);
			this.logoutButton.Name = "logoutButton";
			this.logoutButton.Size = new System.Drawing.Size(111, 35);
			this.logoutButton.TabIndex = 2;
			this.logoutButton.Text = "Logout";
			this.logoutButton.UseVisualStyleBackColor = true;
			this.logoutButton.Click += new System.EventHandler(this.LogoutButton_Click);
			// 
			// GetAccessToken
			// 
			this.GetAccessToken.Location = new System.Drawing.Point(408, 12);
			this.GetAccessToken.Name = "GetAccessToken";
			this.GetAccessToken.Size = new System.Drawing.Size(111, 35);
			this.GetAccessToken.TabIndex = 3;
			this.GetAccessToken.Text = "GetAccessToken";
			this.GetAccessToken.UseVisualStyleBackColor = true;
			this.GetAccessToken.Click += new System.EventHandler(this.GetAccessToken_Click);
			// 
			// btnRedirectEncryption
			// 
			this.btnRedirectEncryption.Location = new System.Drawing.Point(12, 68);
			this.btnRedirectEncryption.Name = "btnRedirectEncryption";
			this.btnRedirectEncryption.Size = new System.Drawing.Size(111, 44);
			this.btnRedirectEncryption.TabIndex = 4;
			this.btnRedirectEncryption.Text = "Encrypt Json";
			this.btnRedirectEncryption.UseVisualStyleBackColor = true;
			this.btnRedirectEncryption.Click += new System.EventHandler(this.BtnRedirectEncryption_Click);
			// 
			// generalTxtBox
			// 
			this.generalTxtBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.generalTxtBox.Location = new System.Drawing.Point(12, 129);
			this.generalTxtBox.Multiline = true;
			this.generalTxtBox.Name = "generalTxtBox";
			this.generalTxtBox.Size = new System.Drawing.Size(763, 309);
			this.generalTxtBox.TabIndex = 5;
			this.generalTxtBox.Text = resources.GetString("generalTxtBox.Text");
			// 
			// decryptJsonButton
			// 
			this.decryptJsonButton.Location = new System.Drawing.Point(144, 68);
			this.decryptJsonButton.Name = "decryptJsonButton";
			this.decryptJsonButton.Size = new System.Drawing.Size(111, 44);
			this.decryptJsonButton.TabIndex = 6;
			this.decryptJsonButton.Text = "Decrypt Json";
			this.decryptJsonButton.UseVisualStyleBackColor = true;
			this.decryptJsonButton.Click += new System.EventHandler(this.DecryptButton_Click);
			// 
			// getCreditButtn
			// 
			this.getCreditButtn.Location = new System.Drawing.Point(275, 68);
			this.getCreditButtn.Name = "getCreditButtn";
			this.getCreditButtn.Size = new System.Drawing.Size(111, 44);
			this.getCreditButtn.TabIndex = 7;
			this.getCreditButtn.Text = "Get Credits";
			this.getCreditButtn.UseVisualStyleBackColor = true;
			this.getCreditButtn.Click += new System.EventHandler(this.GetCreditButtn_Click);
			// 
			// getCreditAsyncButtn
			// 
			this.getCreditAsyncButtn.Location = new System.Drawing.Point(408, 68);
			this.getCreditAsyncButtn.Name = "getCreditAsyncButtn";
			this.getCreditAsyncButtn.Size = new System.Drawing.Size(111, 44);
			this.getCreditAsyncButtn.TabIndex = 8;
			this.getCreditAsyncButtn.Text = "Get Credits Async";
			this.getCreditAsyncButtn.UseVisualStyleBackColor = true;
			this.getCreditAsyncButtn.Click += new System.EventHandler(this.GetCreditAsyncButtn_Click);
			// 
			// buyCreditBttn
			// 
			this.buyCreditBttn.Location = new System.Drawing.Point(543, 68);
			this.buyCreditBttn.Name = "buyCreditBttn";
			this.buyCreditBttn.Size = new System.Drawing.Size(111, 44);
			this.buyCreditBttn.TabIndex = 9;
			this.buyCreditBttn.Text = "Open Buy Credit";
			this.buyCreditBttn.UseVisualStyleBackColor = true;
			this.buyCreditBttn.Click += new System.EventHandler(this.BuyCreditBttn_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.buyCreditBttn);
			this.Controls.Add(this.getCreditAsyncButtn);
			this.Controls.Add(this.getCreditButtn);
			this.Controls.Add(this.decryptJsonButton);
			this.Controls.Add(this.generalTxtBox);
			this.Controls.Add(this.btnRedirectEncryption);
			this.Controls.Add(this.GetAccessToken);
			this.Controls.Add(this.logoutButton);
			this.Controls.Add(this.reAuthenticateButton);
			this.Controls.Add(this.loginButton);
			this.Name = "MainForm";
			this.Text = "Test App";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button loginButton;
		private System.Windows.Forms.Button reAuthenticateButton;
		private System.Windows.Forms.Button logoutButton;
		private System.Windows.Forms.Button GetAccessToken;
		private System.Windows.Forms.Button btnRedirectEncryption;
		private System.Windows.Forms.TextBox generalTxtBox;
		private System.Windows.Forms.Button decryptJsonButton;
		private System.Windows.Forms.Button getCreditButtn;
		private System.Windows.Forms.Button getCreditAsyncButtn;
		private System.Windows.Forms.Button buyCreditBttn;
	}
}

