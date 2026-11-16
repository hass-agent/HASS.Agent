using HASS.Agent.Forms;
using HASS.Agent.Models.Internal;

namespace HASS.Agent.Managers;

public static class WebViewManager
{
    private static Dictionary<WebViewInfo, WebView> _webViews = new();
    
    /// <summary>
    /// Shows a new webview form with the provided config
    /// </summary>
    /// <param name="webViewInfo"></param>
    /// <param name="url"></param>
    /// <param name="webViewId"></param>
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
        var webView =  (WebView)sender;
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
}