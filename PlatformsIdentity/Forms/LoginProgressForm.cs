using System;
using System.Windows.Forms;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Forms
{
	/// <summary>
	/// Dialog showing a progress bar when logging in.
	/// </summary>
	internal partial class LoginProgressForm : Form
	{
		internal LoginProgressForm()
		{
			InitializeComponent();
		}

		internal Exception Exception { get; set; }
	}
}
