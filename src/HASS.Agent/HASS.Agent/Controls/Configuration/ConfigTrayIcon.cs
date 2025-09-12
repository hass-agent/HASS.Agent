using HASS.Agent.Functions;
using HASS.Agent.Models.Internal;
using Syncfusion.Windows.Forms.Tools;

namespace HASS.Agent.Controls.Configuration
{
	public partial class ConfigTrayIcon : UserControl
	{
		internal int SelectedScreen { get; set; }

		public ConfigTrayIcon()
		{
			InitializeComponent();
		}

		private void ConfigTrayIcon_Load(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(TbWebViewUrl.Text)) TbWebViewUrl.Text = Variables.AppSettings.HassUri;
			InitMultiScreenConfig();
		}

		private void InitMultiScreenConfig()
		{
			var primaryScreenIndex = 0;
			var displays = Screen.AllScreens;

			if (Screen.AllScreens.Length == 1)
			{
				Variables.AppSettings.TrayIconWebViewScreen = 0;
				NumWebViewScreen.Visible = true;
				NumWebViewScreen.Items.Add("Single Screen Mode");
				NumWebViewScreen.Enabled = false;
			}
			else
			{
				for (var i = 0; i < displays.Length; i++)
				{
					var display = displays[i];

					if (display.Primary)
					{
						primaryScreenIndex = i;
					}

					string label = display.Primary ? $"{display.DeviceName} (Primary)" : display.DeviceName;
					NumWebViewScreen.Items.Add(label);
				}
			}

			var selectedScreenIndex = Variables.AppSettings.TrayIconWebViewScreen;

			if (selectedScreenIndex == -1)
			{
				selectedScreenIndex = primaryScreenIndex;
			}

			NumWebViewScreen.SelectedIndex = selectedScreenIndex;
			SelectedScreen = selectedScreenIndex;
		}

		private void CbDefaultMenu_CheckedChanged(object sender, EventArgs e)
		{
			CbShowWebView.Checked = !CbDefaultMenu.Checked;
		}

		private void CbShowWebView_CheckedChanged(object sender, EventArgs e)
		{
			CbDefaultMenu.Checked = !CbShowWebView.Checked;

			TbWebViewUrl.Enabled = CbShowWebView.Checked;
			NumWebViewWidth.Enabled = CbShowWebView.Checked;
			NumWebViewHeight.Enabled = CbShowWebView.Checked;
			BtnShowWebViewPreview.Enabled = CbShowWebView.Checked;
			BtnWebViewReset.Enabled = CbShowWebView.Checked;
			CbWebViewKeepLoaded.Enabled = CbShowWebView.Checked;
			CbWebViewShowMenuOnLeftClick.Enabled = CbShowWebView.Checked;
			LblInfo2.Enabled = CbShowWebView.Checked;
		}

		private void BtnShowWebViewPreview_Click(object sender, EventArgs e)
		{
			var webView = new WebViewInfo
			{
				Url = TbWebViewUrl.Text,
				Height = (int)NumWebViewHeight.Value,
				Width = (int)NumWebViewWidth.Value,
				IsTrayIconWebView = true,
				IsTrayIconPreview = true
			};
			HelperFunctions.LaunchTrayIconWebView(webView, NumWebViewScreen.SelectedIndex);
		}

		private void BtnWebViewReset_Click(object sender, EventArgs e)
		{
			NumWebViewWidth.Value = 700;
			NumWebViewHeight.Value = 560;
		}

		private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
		{
			SelectedScreen = ((ComboBoxAdv)sender).SelectedIndex;
		}
	}
}
