using Newtonsoft.Json;

namespace HASS.Agent.Models.HomeAssistant
{
    public class NotificationAction
    {
        public string Action { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
    }

    public class NotificationInput
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
    }

    public class NotificationData
    {
        public const string NoAction = "noAction";
        public const string ImportanceHigh = "high";

        public int Duration { get; set; } = 0;
        public string Image { get; set; } = string.Empty;
        public string ClickAction { get; set; } = NoAction;
        public string Tag { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; } = string.Empty;
        public bool Sticky { get; set; }
        public string Importance { get; set; } = string.Empty;

        public List<NotificationAction> Actions { get; set; } = new List<NotificationAction>();

        public List<NotificationInput> Inputs { get; set; } = new List<NotificationInput>();
    }

    public class Notification
    {
        public string Message { get; set; }
        public string Title { get; set; }

        public NotificationData Data { get; set; }
    }
}
