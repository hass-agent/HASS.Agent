using Windows.UI.Notifications;
using HASS.Agent.API;
using HASS.Agent.Functions;
using HASS.Agent.HomeAssistant;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Notification = HASS.Agent.Models.HomeAssistant.Notification;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppLifecycle;
using System.Windows.Markup;
using System.Text;
using System.Net;
using HASS.Agent.Resources.Localization;

namespace HASS.Agent.Managers
{
    internal static class NotificationManager
    {
        public const string NotificationLaunchArgument = "----AppNotificationActivated:";

        public static bool Ready { get; private set; } = false;

        private const string ActionKey = "action";
        private const string UriKey = "uri";
        private const string ClickActionKey = "clickAction";

        private const string ActionPrefix = $"{ActionKey}=";
        private const string UriPrefix = $"{UriKey}=";
        private const string ClickActionPrefix = $"{ClickActionKey}=";

        private const string SpecialClear = "clear_notification";


        private static readonly AppNotificationManager _notificationManager = AppNotificationManager.Default;


        /// <summary>
        /// Initializes the notification manager
        /// </summary>
        internal static void Initialize()
        {
            try
            {
                if (!Variables.AppSettings.NotificationsEnabled)
                {
                    Log.Information("[NOTIFIER] Disabled");

                    return;
                }

                if (!Variables.AppSettings.LocalApiEnabled && !Variables.AppSettings.MqttEnabled)
                {
                    Log.Warning("[NOTIFIER] Both local API and MQTT are disabled, unable to receive notifications");

                    return;
                }

                if (!Variables.AppSettings.MqttEnabled)
                    Log.Warning("[NOTIFIER] MQTT is disabled, not all aspects of actions might work as expected");
                else
                    _ = Task.Run(Variables.MqttManager.SubscribeNotificationsAsync);

                if (_notificationManager.Setting != AppNotificationSetting.Enabled)
                    Log.Warning("[NOTIFIER] Showing notifications might fail, reason: {r}", _notificationManager.Setting.ToString());


                _notificationManager.NotificationInvoked += OnNotificationInvoked;

                _notificationManager.Register();
                Ready = true;

                Log.Information("[NOTIFIER] Ready");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[NOTIFIER] Error while initializing: {err}", ex.Message);
            }
        }

        private static string EncodeNotificationParameter(string parameter)
        {
            var encodedParameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameter));
            // for some reason, Windows App SDK URL encodes the arguments even if they are already encoded
            // this is the reason the WebUtility.UrlEncode is missing from here
            return encodedParameter;
        }

        private static string DecodeNotificationParameter(string encodedParameter)
        {
            var urlDecodedParameter = WebUtility.UrlDecode(encodedParameter);
            return Encoding.UTF8.GetString(Convert.FromBase64String(urlDecodedParameter));
        }

        /// <summary>
        /// Show a notification object as a toast message
        /// </summary>
        /// <param name="notification"></param>
        internal static async void ShowNotification(Notification notification)
        {
            if (!Ready)
                throw new Exception("NotificationManager is not initialized");

            try
            {
                if (!Variables.AppSettings.NotificationsEnabled || _notificationManager == null)
                    return;

                if (notification.Message == SpecialClear)
                {
                    if (!string.IsNullOrWhiteSpace(notification.Data.Tag) && !string.IsNullOrWhiteSpace(notification.Data.Group))
                        await _notificationManager.RemoveByTagAndGroupAsync(notification.Data.Tag, notification.Data.Group);
                    else if (!string.IsNullOrWhiteSpace(notification.Data.Tag))
                        await _notificationManager.RemoveByTagAsync(notification.Data.Tag);

                    return;
                }

                var toastBuilder = new AppNotificationBuilder()
                    .AddText(notification.Title)
                    .AddText(notification.Message);

                if (notification.Data.ClickAction != Models.HomeAssistant.NotificationData.NoAction)
                    toastBuilder.AddArgument(ClickActionKey, EncodeNotificationParameter(notification.Data.ClickAction));

                if (!string.IsNullOrWhiteSpace(notification.Data?.Image))
                {
                    var (success, localFile) = await StorageManager.DownloadImageAsync(notification.Data.Image);
                    if (success)
                        toastBuilder.SetInlineImage(new Uri(localFile));
                    else
                        Log.Error("[NOTIFIER] Image download failed, dropping: {img}", notification.Data.Image);
                }

                if (notification.Data?.Actions.Count > 0)
                {
                    foreach (var action in notification.Data.Actions)
                    {
                        if (string.IsNullOrEmpty(action.Action))
                            continue;

                        var button = new AppNotificationButton(action.Title)
                            .AddArgument(ActionKey, EncodeNotificationParameter(action.Action));

                        if (action.Uri != null)
                            button.AddArgument(UriKey, EncodeNotificationParameter(action.Uri));

                        toastBuilder.AddButton(button);
                    }
                }

                if (notification.Data?.Inputs.Count > 0)
                {
                    foreach (var input in notification.Data.Inputs)
                    {
                        if (string.IsNullOrEmpty(input.Id))
                            continue;

                        toastBuilder.AddTextBox(input.Id, input.Text, input.Title);
                    }
                }

                if (!string.IsNullOrWhiteSpace(notification.Data.Group))
                    toastBuilder.SetGroup(notification.Data.Group);
                if (!string.IsNullOrWhiteSpace(notification.Data.Tag))
                    toastBuilder.SetTag(notification.Data.Tag);

                if (notification.Data.Sticky)
                {
                    toastBuilder.SetScenario(AppNotificationScenario.Reminder);
                    if (notification.Data.Actions.Count == 0)
                        toastBuilder.AddButton(new AppNotificationButton(Languages.Notification_Dismiss)); //Note(Amadeo): required for reminder scenario
                }

                if (AppNotificationBuilder.IsUrgentScenarioSupported() && notification.Data.Importance == Models.HomeAssistant.NotificationData.ImportanceHigh)
                {
                    toastBuilder.SetScenario(AppNotificationScenario.Urgent);
                    if (notification.Data.Sticky)
                        Log.Warning("[NOTIFIER] Notification importance overrides sticky", notification.Title);
                }

                if (!string.IsNullOrWhiteSpace(notification.Data.IconUrl))
                    toastBuilder.SetAppLogoOverride(new Uri(notification.Data.IconUrl));

                var toast = toastBuilder.BuildNotification();

                if (notification.Data?.Duration > 0)
                {
                    //TODO: unreliable
                    toast.Expiration = DateTime.Now.AddSeconds(notification.Data.Duration);
                }

                _notificationManager.Show(toast);

                if (toast.Id == 0)
                {
                    Log.Error("[NOTIFIER] Notification '{err}' failed to show", notification.Title);
                }

            }
            catch (Exception ex)
            {
                if (Variables.ExtendedLogging)
                    Log.Fatal(ex, "[NOTIFIER] Error while showing notification: {err}\r\n{json}", ex.Message, JsonConvert.SerializeObject(notification, Formatting.Indented));
                else
                    Log.Fatal(ex, "[NOTIFIER] Error while showing notification: {err}", ex.Message);
            }
        }

        private static string GetValueFromEventArgs(AppNotificationActivatedEventArgs e, string startText)
        {
            var start = e.Argument.IndexOf(startText) + startText.Length;
            if (start < startText.Length)
                return null;

            var separatorIndex = e.Argument.IndexOf(";", start);
            var end = separatorIndex < 0 ? e.Argument.Length : separatorIndex;
            return DecodeNotificationParameter(e.Argument[start..end]);
        }

        private static IDictionary<string, string> GetInputFromEventArgs(AppNotificationActivatedEventArgs e) => e.UserInput.Count > 0 ? e.UserInput : null;

        private static async void OnNotificationInvoked(AppNotificationManager _, AppNotificationActivatedEventArgs e) => await HandleAppNotificationActivation(e);

        private static async Task HandleAppNotificationActivation(AppNotificationActivatedEventArgs e)
        {
            try
            {
                var action = GetValueFromEventArgs(e, ActionPrefix);
                var input = GetInputFromEventArgs(e);
                var uri = GetValueFromEventArgs(e, UriPrefix);
                var clickAction = GetValueFromEventArgs(e, ClickActionPrefix);

                if (string.IsNullOrWhiteSpace(uri) && Variables.AppSettings.NotificationsOpenActionUri)
                    HelperFunctions.LaunchUrl(uri);
                else if (!string.IsNullOrWhiteSpace(clickAction))
                    HelperFunctions.LaunchUrl(clickAction);

                var haEventTask = HassApiManager.FireEvent("hass_agent_notifications", new
                {
                    device_name = HelperFunctions.GetConfiguredDeviceName(),
                    action,
                    input,
                    uri
                });

                if (Variables.AppSettings.MqttEnabled)
                {
                    var haMessageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/notifications/{Variables.DeviceConfig.Name}/actions")
                        .WithPayload(JsonConvert.SerializeObject(new
                        {
                            action,
                            input,
                            uri
                        }));

                    var mqttTask = Variables.MqttManager.PublishAsync(haMessageBuilder.Build());

                    await Task.WhenAny(haEventTask, mqttTask);
                }
                else
                {
                    await haEventTask;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[NOTIFIER] Unable to process notification activation: {err}", ex.Message);
            }
        }

        internal static async void HandleNotificationLaunch()
        {
            if (!Ready)
                throw new Exception("NotificationManager is not initialized");

            Log.Information("[NOTIFIER] Launched with notification action");

            var args = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (args.Kind != ExtendedActivationKind.AppNotification)
                return;

            var appNotificationArgs = args.Data as AppNotificationActivatedEventArgs;
            if (appNotificationArgs.Argument == null)
                return;

            await HandleAppNotificationActivation(appNotificationArgs);

            Log.Information("[NOTIFIER] Finished handling notification action");
        }

        internal static void Exit()
        {
            if (!Ready)
                throw new Exception("NotificationManager is not initialized");

            _notificationManager.Unregister();
        }
    }
}