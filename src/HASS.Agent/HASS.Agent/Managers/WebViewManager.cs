using HASS.Agent.Forms;
using HASS.Agent.Models.Internal;

namespace HASS.Agent.Managers;

public static class WebViewManager
{
    private static readonly Dictionary<WebViewInfo, WebView> _webViews = new();

    /// <summary>
    /// Shows a new webview form with the provided config
    /// </summary>
    /// <param name="webViewInfo"></param>
    /// <param name="url"></param>
    internal static void LaunchWebView(WebViewInfo webViewInfo, string url = "")
    {
        if (!string.IsNullOrEmpty(url))
        {
            webViewInfo.Url = url;
        }

        if (_webViews.TryGetValue(webViewInfo, out var oldWebView))
        {
            Variables.MainForm.Invoke(new MethodInvoker(delegate
            {
                oldWebView.ForceClose();
            }));

            _webViews.Remove(webViewInfo);
        }

        var webView = new WebView(webViewInfo); //NOTE(Amadeo): do this better in v3...
        webView.Closed += WebViewOnClosed;
        Variables.MainForm.Invoke(new MethodInvoker(delegate
        {
            webView.Opacity = 0;
            webView.Show();
        }));

        _webViews.Add(webViewInfo, webView);
    }

    private static void WebViewOnClosed(object sender, EventArgs e)
    {
        var webView = (WebView)sender;
        if (webView == null)
        {
            return;
        }

        webView.Closed -= WebViewOnClosed;
        Variables.Commands.FirstOrDefault(command => command.EntityName == webView.WebViewInfo.Id)?.TurnOff();
    }

    internal static void CloseWebView(WebViewInfo webViewInfo)
    {
        if (!_webViews.TryGetValue(webViewInfo, out var oldWebView))
        {
            return;
        }

        Variables.MainForm.Invoke(new MethodInvoker(delegate
        {
            oldWebView.ForceClose();
        }));

        _webViews.Remove(webViewInfo);
    }

    /// <summary>
    /// Prepares and loads the tray icon's webview
    /// </summary>
    internal static void PrepareTrayIconWebView(int screenIndex = 0)
    {
        // prepare the webview info
        var webViewInfo = new WebViewInfo
        {
            Url = Variables.AppSettings.TrayIconWebViewUrl,
            Height = Variables.AppSettings.TrayIconWebViewHeight,
            Width = Variables.AppSettings.TrayIconWebViewWidth,
            IsTrayIconWebView = true
        };

        var x = Screen.AllScreens[screenIndex].Bounds.Right - webViewInfo.Width;
        var y = Screen.AllScreens[screenIndex].Bounds.Bottom - webViewInfo.Height;

        webViewInfo.X = x;
        webViewInfo.Y = y;

        webViewInfo.TopMost = true;
        webViewInfo.ShowTitleBar = false;
        webViewInfo.CenterScreen = false;
        webViewInfo.IsTrayIconWebView = true;

        // prepare the webview
        Variables.MainForm.Invoke(new MethodInvoker(delegate
        {
            // optionally close an existing one
            Variables.TrayIconWebView?.ForceClose();

            // bind the new one
            Variables.TrayIconWebView = new WebView(webViewInfo);
            Variables.TrayIconWebView.Opacity = 0;
            Variables.TrayIconWebView.Show();
        }));
    }

    /// <summary>
    /// Shows the user-configured webview form near the tray icon
    /// </summary>
    internal static void LaunchTrayIconWebView()
    {
        var webView = new WebViewInfo
        {
            Url = Variables.AppSettings.TrayIconWebViewUrl,
            Height = Variables.AppSettings.TrayIconWebViewHeight,
            Width = Variables.AppSettings.TrayIconWebViewWidth,
        };

        LaunchTrayIconWebView(webView, Variables.AppSettings.TrayIconWebViewScreen);
    }

    /// <summary>
    /// Shows a new webview form near the tray icon
    /// </summary>
    /// <param name="webViewInfo"></param>
    /// <param name="screenIndex"></param>
    internal static void LaunchTrayIconWebView(WebViewInfo webViewInfo, int screenIndex = 0)
    {
        // are we previewing?
        if (webViewInfo.IsTrayIconPreview)
        {
            // yep, show as configured
            LaunchTrayIconCustomWebView(webViewInfo, screenIndex);

            // done
            return;
        }

        // is background loading enabled?
        if (Variables.AppSettings.TrayIconWebViewBackgroundLoading)
        {
            // yep
            LaunchTrayIconBackgroundLoadedWebView(screenIndex);

            // done
            return;
        }

        // show a new webview from within the UI thread
        LaunchTrayIconCustomWebView(webViewInfo, screenIndex);
    }

    private static void LaunchTrayIconBackgroundLoadedWebView(int screenIndex)
    {
        Variables.MainForm.Invoke(new MethodInvoker(delegate
        {
            // make sure it's ready
            if (Variables.TrayIconWebView == null || Variables.TrayIconWebView.IsDisposed)
            {
                PrepareTrayIconWebView();
            }

            // show it
            Variables.TrayIconWebView?.MakeVisible();
        }));
    }

    private static void LaunchTrayIconCustomWebView(WebViewInfo webViewInfo, int screenIndex = 0)
    {
        if (screenIndex == -1)
        {
            InitMultiScreenConfig();
            screenIndex = Variables.AppSettings.TrayIconWebViewScreen;
        }

        Variables.MainForm.Invoke(new MethodInvoker(delegate
        {
            var targetScreen = Screen.AllScreens[screenIndex];

            var x = targetScreen.Bounds.X + webViewInfo.Width;
            var y = targetScreen.WorkingArea.Height - webViewInfo.Height;

            webViewInfo.X = x;
            webViewInfo.Y = y;

            webViewInfo.TopMost = true;
            webViewInfo.ShowTitleBar = false;
            webViewInfo.CenterScreen = false;

            var webView = new WebView(webViewInfo);
            webView.Opacity = 0;
            webView.AutoScaleMode = AutoScaleMode.Font;
            webView.Show();

            webView.Left = targetScreen.WorkingArea.Right - webView.Width;
            webView.Top = targetScreen.WorkingArea.Bottom - webView.Height;

            Variables.TrayIconWebView = webView;
        }));
    }

    public static void InitMultiScreenConfig()
    {
        var primaryScreenIndex = 0;
        var displays = Screen.AllScreens;

        if (Screen.AllScreens.Length == 1)
        {
            Variables.AppSettings.TrayIconWebViewScreen = 0;
        }
        else
        {
            for (var i = 0; i < displays.Length; i++)
            {
                var display = displays[i];

                if (!display.Primary)
                {
                    continue;
                }

                primaryScreenIndex = i;
                break;
            }
        }

        Variables.AppSettings.TrayIconWebViewScreen = primaryScreenIndex;
    }

    public static void CloseTrayIconWebView()
    {
        Variables.MainForm.Invoke(() =>
        {
            if (Variables.TrayIconWebView == null)
            {
                return;
            }

            Variables.TrayIconWebView.Opacity = 0;

            if (Variables.AppSettings.TrayIconWebViewBackgroundLoading)
            {
                return;
            }

            Variables.TrayIconWebView.ForceClose();
            Variables.TrayIconWebView.Dispose();
        });
    }
}