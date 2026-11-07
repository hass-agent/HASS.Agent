namespace HASS.Agent.Models.Internal
{
    public class SerialPortInfoCollection
    {
        public SerialPortInfoCollection()
        {
            //
        }

        public SerialPortInfoCollection(IEnumerable<SerialPortInfo> serialPorts)
        {
            SerialPorts.AddRange(serialPorts);
        }

        public List<SerialPortInfo> SerialPorts { get; set; } = new();
    }

    public class SerialPortInfo
    {
        /// <summary>
        /// The port name, eg. COM3
        /// </summary>
        public string Port { get; set; } = string.Empty;

        /// <summary>
        /// The friendly name, eg. 'USB Serial Device (COM3)'
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// USB vendor ID as 4-digit hex, if known
        /// </summary>
        public string VendorId { get; set; } = string.Empty;

        /// <summary>
        /// USB product ID as 4-digit hex, if known
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// The device interface ID, or the kernel device name for registry-only ports
        /// </summary>
        public string Id { get; set; } = string.Empty;
    }
}
