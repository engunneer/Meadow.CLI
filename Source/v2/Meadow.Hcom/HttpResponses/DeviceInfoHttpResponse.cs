using System.Text.Json.Serialization;

namespace Meadow.Hcom;

internal class DeviceInfoHttpResponse
{
    /*
    {
        "service": "Wilderness Labs Meadow.Daemon",
        "up_time": 1691423994,
        "version": "1.0",
        "status": "Running",
        "device_info": {
            "serial_number": "d2096851d77a47ad74ff22a862aca5f2",
            "device_name": "DESKTOP-PGERLRJ",
            "platform": "MeadowForLinux",
            "os_version": "#1 SMP Fri Jan 27 02:56:13 UTC 2023",
            "os_release": "5.15.90.1-microsoft-standard-WSL2",
            "os_name": "Linux",
            "machine": "x86_64"
        },
        "public_key": "-----BEGIN RSA PUBLIC KEY-----\nMIICCgKCAgEArTGLJ7vjN36PuzVUbIegTNmleCYrdkeVDYr4kvCwxpY/8dn2V/FQ\nwzmg6D516wsA2jsYbk6ozDpDCpr6dyjoxwQGLYCD89o56X5rDp3TsesQyTgDU6Hf\ni6/cq9WEmMn0Zr5pF9JaUR5slv8ujSTbU5IXasIZoDPCz4L3390a6GuxtOI/LHgE\no2FFQn9vFl3LoTRAiQ/z8UgJWbNao8uio8lZaN9vkydTq6mrr+YjaeM89PxP9SG/\ns8ZrkJumZlkZRmzPNbHXb5aWXZDnkUyzLqXMRLVHfas/f1Ls2/KPbgNeynLxJNSO\nfzkXn05+luD2j/URQg/THXDXANNNNzp1fNQI+o4aIzGmo2phRbq2O6gpOqUmdwJ2\njGl2qwtQXLZa7L9RLIuCzBNRdsz1oT/2J8pWVO9duVLB5WOXmB5/VsHDq/d/PwN4\ne8VwWT+1JnQdDO6BikyN2dfdq7UF4QNaauxVTCp62Wsa1u5AWmBvcIFzMHXktjCU\nqtE5lJhxQ5SWBgn/JIEkrSyAD3vecLFE07gJsIPiMbzwxfgDd1EGAbRUR/EgQzQh\n8Kb8icZT214b03pvSpkwFfS2p9H20agcAH+h0o/uI0m0RtWAN4XUQ+qgmXwlwkRp\n7gZ4IPs4+s1Gb6ULcQzBLv4qyaMwlwqJx9sa4VBoCzyxNNs/5uF9CfECAwEAAQ==\n-----END RSA PUBLIC KEY-----\n"
    }
    */
    [JsonPropertyName("service")]
    public string ServiceName { get; set; } = default!;
    [JsonPropertyName("version")]
    public string ServiceVersion { get; set; } = default!;
    [JsonPropertyName("status")]
    public string ServiceStatus { get; set; } = default!;
    [JsonPropertyName("device_info")]
    public DeviceFields DeviceInfo { get; set; } = default!;
    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; } = default!;

    internal class DeviceFields
    {
        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; } = default!;
        [JsonPropertyName("device_name")]
        public string DeviceName { get; set; } = default!;
        [JsonPropertyName("platform")]
        public string Platform { get; set; } = default!;
        [JsonPropertyName("os_version")]
        public string OsVersion { get; set; } = default!;
        [JsonPropertyName("os_release")]
        public string OsRelease { get; set; } = default!;
        [JsonPropertyName("os_name")]
        public string OsName { get; set; } = default!;
        [JsonPropertyName("machine")]
        public string Machine { get; set; } = default!;
    }

    public Dictionary<string, string> ToDictionary()
    {
        var d = new Dictionary<string, string>
        {
            { "CoprocessorVersion", $"{ServiceName} v{ServiceVersion}" },
            { "SerialNo", DeviceInfo.SerialNumber },
            { "DeviceName", DeviceInfo.DeviceName },
            { "OSVersion", DeviceInfo.OsRelease },
            { "OsName", DeviceInfo.OsName },
            { "Product", DeviceInfo.Platform },
            { "Model", DeviceInfo.DeviceName },
            { "ProcessorType", DeviceInfo.Machine }
        };
        return d;
    }
}
