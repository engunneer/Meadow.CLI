using System.Text;

namespace Meadow.Hcom
{
    public class DeviceInfo
    {
        public Dictionary<string, string> Properties { get; }

        internal DeviceInfo(Dictionary<string, string> properties)
        {
            Properties = properties;
        }

        public string? this[string propname]
        {
            get
            {
                return Properties.Keys.Contains(propname) ? Properties[propname] : null;
            }
        }

        // DEV NOTE: these strings are hard-coded because they are the values that come from the F7 HCOM request.  Don't change them!
        public string? Product => this["Product"];
        public string? Model => this["Model"];
        public string? ProcessorType => this["ProcessorType"];
        public string? CoprocessorType => this["CoprocessorType"];
        public string? OsVersion => this["OSVersion"];
        public string? CoprocessorOsVersion => this["CoprocessorVersion"];
        public string? ProcessorId => this["ProcessorId"];
        public string? HardwareVersion => this["Hardware"];
        public string? DeviceName => this["DeviceName"];
        public string? RuntimeVersion => this["MonoVersion"];
        public string? SerialNumber => this["SerialNo"];
        public string? MacAddress => this["WiFiMAC"];
        public string? SoftAPMacAddress => this["SoftAPMac"];

        /// <summary>
        /// String representation of an unknown MAC address.
        /// </summary>
        private const string UNKNOWN_MAC_ADDRESS = "00:00:00:00:00:00";

        public override string ToString()
        {
            var info = new Dictionary<string, Dictionary<string, string>>(); // section, value name, value

            var board = new Dictionary<string, string>();
            if (Product != null) board.Add("Product", Product);
            if (Model != null) board.Add("Model", Model);
            if (HardwareVersion != null) board.Add("Hardware version", HardwareVersion);
            if (DeviceName != null) board.Add("Device name", DeviceName);
            if (board.Count > 0)
            {
                info.Add("Device Information", board);
            }

            var hardware = new Dictionary<string, string>();
            if (ProcessorType != null) hardware.Add("Processor type", ProcessorType);
            if (ProcessorId != null) hardware.Add("ID", ProcessorId);
            if (SerialNumber != null) hardware.Add("Serial number", SerialNumber);
            if (CoprocessorType != null) hardware.Add("Coprocessor type", CoprocessorType);
            if (!string.IsNullOrEmpty(MacAddress) && MacAddress != UNKNOWN_MAC_ADDRESS) hardware.Add("WiFi", MacAddress);
            if (!string.IsNullOrEmpty(SoftAPMacAddress) && MacAddress != UNKNOWN_MAC_ADDRESS) hardware.Add("AP", SoftAPMacAddress);

            if (hardware.Count > 0)
            {
                info.Add("Hardware Information", hardware);
            }

            var firmware = new Dictionary<string, string>();
            if (OsVersion != null) firmware.Add("OS", OsVersion);
            if (RuntimeVersion != null) firmware.Add("Runtime", RuntimeVersion);
            if (CoprocessorOsVersion != null) firmware.Add("Coprocessor", CoprocessorOsVersion);
            firmware.Add("Protocol", Protocol.HCOM_PROTOCOL_HCOM_VERSION_NUMBER.ToString());
            if (firmware.Count > 0)
            {
                info.Add("Firmware Information", firmware);
            }

            var deviceInfo = new StringBuilder();

            foreach (var section in info.Keys)
            {
                deviceInfo.AppendLine(section);

                foreach (var value in info[section])
                {
                    deviceInfo.AppendLine($"\t{value.Key}: {value.Value}");
                }
            }

            return deviceInfo.ToString();
        }
    }
}