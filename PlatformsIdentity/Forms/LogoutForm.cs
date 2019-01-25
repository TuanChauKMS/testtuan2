using System;
using System.Windows.Forms;
using QSR.NVivo.Plugins.PlatformsIdentity.Services;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Forms
{
	/// <summary>
	/// This form is used to navigate to a Logout URL; this action must occur within a browser session to work correctly.
	/// </summary>
	internal partial class LogoutForm
	{
		#region Constructor

		internal LogoutForm(IAuth0ServiceProxy aAuth0ServiceProxy, Uri logoutUri)
		{
			InitializeComponent();
			_auth0ServiceProxy = aAuth0ServiceProxy;
			_logoutUri = logoutUri;
		}

		#endregion

		#region Base Overrides

		/// <inheritdoc />
		/// <summary>
		/// Navigate to the Uluru Auth0 logout endpoint
		/// </summary>
		protected override async void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// Start the timer. When it expires, this form is closed.
			timeoutTimer.Interval = TimeoutMs;
			timeoutTimer.Tick += (sender, args) => { Close(); };
			timeoutTimer.Start();

			try
			{
				webBrowser.Url = _logoutUri;
				await _auth0ServiceProxy.RevokeRefreshToken();
			}
			catch 
			{
				// Silently fail - ignore the failed attempt at revoking refresh token (for now)
			}

			BeginInvoke(new MethodInvoker(Close));
		}

		#endregion

		#region Private Members

		private readonly IAuth0ServiceProxy _auth0ServiceProxy;

		private readonly Uri _logoutUri;

		#endregion

		#region Constants

		private const int TimeoutMs = 5000;

		#endregion
	}
}
