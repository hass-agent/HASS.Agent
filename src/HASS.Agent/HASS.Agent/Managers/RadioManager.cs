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
            var accessStatus = await Radio.RequestAccessAsync();
            if (accessStatus == RadioAccessStatus.Allowed)
            {
                foreach (var radio in await Radio.GetRadiosAsync())
                {
                    AvailableRadio.Add(radio);
                }

                Log.Information("[RADIOMGR] Ready");
            }
            else
            {
                Log.Fatal("[RADIOMGR] No permission granted for Bluetooth radio management");
            }

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
                Log.Fatal("[RADIOMGR] Error initializing NFC devices");
            }
        }

        private static void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message)
        {
            try
            {
/*                var rawMsg = message.Data.ToArray();
                var ndefMessage = NdefMessage.FromByteArray(rawMsg);

                // Loop over all records contained in the NDEF message
                foreach (NdefRecord record in ndefMessage)
                {
                    Console.WriteLine("Record type: " + Encoding.UTF8.GetString(record.Type, 0, record.Type.Length));
                    // Go through each record, check if it's a Smart Poster
                    if (record.CheckSpecializedType(false) == typeof(NdefUriRecord))
                    {
                        var spRecord = new NdefUriRecord(record);
                        Console.WriteLine($"URI: {spRecord.Uri}");
                    }
                }*/

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
