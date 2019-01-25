using System;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Forms;
using QSR.NVivo.Plugins.PlatformsIdentity.Services;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Forms
{
	/// <summary>
	/// This form allows the user to either login through the Auth0's authentication dialog
	/// or signup via an external browser window
	/// </summary>
	internal partial class LoginForm
	{
		#region Constructor

		/// <summary>
		/// The public constructor
		/// </summary>
		internal LoginForm()
		{
			InitializeComponent();

			// Programatically centralize the controls on form
			loginButton.Left = (ClientSize.Width - loginButton.Width) / 2;
			orLabel.Left = (ClientSize.Width - orLabel.Width) / 2;
			signupLinkLabel.Left = (ClientSize.Width - signupLinkLabel.Width) / 2;

			mySignUpUrl = Auth0ServiceProxy.GetExternalSignUpUrl();
			myAuthorizeEndpoint = Auth0ServiceProxy.GetAuth0AuthorizeEndpoint();

			// This prevents the web browser control from stealing the focus away
			// from the initial panel controls when it has navigated
			browserPanel.Enabled = false;

			webBrowserControl.Navigate(myAuthorizeEndpoint);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Launch the browser and navigate to the sign-up page
		/// </summary>
		private void SignupLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(mySignUpUrl);
		}

		/// <summary>
		/// Launch the browser and navigate to the sign-up  page
		/// </summary>
		private void LoginButton_Clicked(object sender, EventArgs e)
		{
			initialPanel.Hide();

			// disable tabbing for the button and link label
			initialPanel.Enabled = false;

			// enable the browser panel again only when the button is clicked
			// if the browser panel is enabled elsewhere before the login button is clicked, the browser will
			// steal the current focus away
			browserPanel.Enabled = true;
			webBrowserControl.TabStop = true;
			if (!webBrowserControl.Visible)
			{
				webBrowserControl.Show();
			}
			webBrowserControl.Focus();
		}

		/// <summary>
		/// Called when the web browser control has navigated to a new page; we
		/// use this event to sniff tokens from the browser's URL query string
		/// </summary>
		private void WebBrowserControl_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			NameValueCollection urlArgs = HttpUtility.ParseQueryString(e.Url.Query);

			if (urlArgs["error"] != null)
			{
				// TODO: find out what errors may occur and update this
				if (urlArgs["error"] == "access_denied")
				{
					DialogResult = DialogResult.Cancel;
					return;
				}

				// TODO: find out what errors may occur and update this
				ErrorMessage = urlArgs["error_description"];
				DialogResult = DialogResult.Cancel;
				return;
			}

			if (urlArgs["code"] != null)
			{
				// capture the authorization code from the auth redirect URI and 
				// exchange it for access and refresh tokens using RESTful API calls
				AuthorizationCode = urlArgs["code"];

				// can now close the auth dialog; user should have access
				DialogResult = DialogResult.OK;
			}
		}

		/// <summary>
		/// Show the web browser when the web document is fully loaded
		/// This is part of the fix to prevent the browser from stealing the current focus away
		/// </summary>
		private void WebBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (!webBrowserControl.Visible)
			{
				webBrowserControl.Show();
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// This value is set with the result from a successful auth request
		/// </summary>
		public string AuthorizationCode { get; private set; }

		/// <summary>
		/// Any error message which may need to be returned to the consumer code
		/// </summary>
		public string ErrorMessage { get; private set; }

		/// <summary>
		/// The exception to throw
		/// </summary>
		public Exception Exception { get; set; }

		#endregion

		#region Private Members

		/// <summary>
		/// The Url to the user sign-up process that will be opened in an external browser window
		/// </summary>
		private readonly string mySignUpUrl;

		/// <summary>
		/// The Url to the Uluru Auth0 Authorize endpoint
		/// </summary>
		private readonly string myAuthorizeEndpoint;

		#endregion
	}
}
