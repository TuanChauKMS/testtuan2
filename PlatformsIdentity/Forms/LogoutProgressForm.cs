using System.Windows.Forms;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Forms
{
	/// <summary>
	/// Dialog showing a progress bar when logging out.
	/// This is designed to be run modally, and the actual (hidden) LogoutForm run non-modally.
	/// When the LogoutForm completes and closes, this progress form closes as well.
	/// </summary>
	internal partial class LogoutProgressForm : Form
	{
		internal LogoutProgressForm()
		{
			InitializeComponent();
		}

		public LogoutProgressForm(LogoutForm logoutForm) : this()
		{
			logoutForm.FormClosed += (sender, e) => { Close(); };
		}
	}
}
