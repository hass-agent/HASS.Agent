using MQTTnet;
using NdefLibrary.Ndef;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Radios;
using Windows.Devices.SmartCards;
using Windows.Networking.Proximity;

namespace HASS.Agent.Managers
{
    internal static class RadioManager
    {
        public static List<Radio> AvailableRadio { get; private set; } = new();
        public static List<string> AvailableRadioNames => AvailableRadio.Select(r => r.Name).ToList();

        public static List<ProximityDevice> AvailableNFCReader { get; private set; } = new();
        public static List<string> AvailableNFCReaderNames => AvailableRadioNames.Select(n => n.Normalize()).ToList();

        private static long s_subscriptionId = -1;
        private static ProximityDevice s_selectedNFCReader = null;
        public static ProximityDevice SelectedNFCReader
        {
            get => s_selectedNFCReader;
            set
            {
                if (s_selectedNFCReader == value)
                    return;

                if (s_selectedNFCReader != null && s_subscriptionId != -1)
                {
                    s_selectedNFCReader.StopSubscribingForMessage(s_subscriptionId);
                }

                s_subscriptionId = s_selectedNFCReader.SubscribeForMessage("NDEF", MessageReceivedHandler);
                s_selectedNFCReader = value;
            }
        }

        public static async Task Initialize()
        {
            Log.Debug("[RADIOMGR] Initialization started");

            var accessStatus = await Radio.RequestAccessAsync();
            if (accessStatus == RadioAccessStatus.Allowed)
            {
                foreach (var radio in await Radio.GetRadiosAsync())
                {
                    AvailableRadio.Add(radio);
                }

                Log.Information("[RADIOMGR] Radio management permission granted");
            }
            else
            {
                Log.Fatal("[RADIOMGR] No permission granted for radio management - privacy settings may be restricting the access");
            }

            Log.Debug("[RADIOMGR] Enumerating proximity/NFC devices");
            try
            {
                var proximityDevices = await DeviceInformation.FindAllAsync(ProximityDevice.GetDeviceSelector());
                foreach (var device in proximityDevices)
                {
                    var proximityReader = ProximityDevice.FromId(device.Id);
                    AvailableNFCReader.Add(proximityReader);
                }
            }
            catch
            {
                Log.Fatal("[RADIOMGR] Error initializing proximity/NFC devices");
            }

            Log.Information("[RADIOMGR] Ready");
        }

        private static void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
        {
            try
            {
                var rawMsg = message.Data.ToArray();
                var ndefMessage = NdefMessage.FromByteArray(rawMsg);

                foreach (var record in ndefMessage)
                {
                    Log.Debug("Record type: " + Encoding.UTF8.GetString(record.Type, 0, record.Type.Length));
                    if (record.CheckSpecializedType(false) == typeof(NdefUriRecord))
                    {
                        var uriRecord = new NdefUriRecord(record);
                        Log.Debug($"URI: {uriRecord.Uri}");

                        if (uriRecord.Uri.StartsWith("https://www.home-assistant.io/tag/"))
                        {
                            var tagId = uriRecord.Uri.Split('/').LastOrDefault();
                            if(string.IsNullOrWhiteSpace(tagId))
                                return;

                            var tagScannedMessage = new MqttApplicationMessageBuilder()
                                .WithTopic($"hass.agent/devices/{Variables.DeviceConfig.Name}/tag_scanned")
                                .WithPayload(JsonConvert.SerializeObject(new
                                {
                                    Time = DateTime.UtcNow.ToString("s"),
                                    UID = tagId
                                }))
                                .Build();

                            Variables.MqttManager.PublishAsync(tagScannedMessage);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
