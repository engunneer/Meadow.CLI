﻿using CliFx.Attributes;
using Microsoft.Extensions.Logging;

namespace Meadow.CLI.Commands.DeviceManagement;

[Command("flash erase", Description = "Erases the device's flash storage")]
public class FlashEraseCommand : BaseDeviceCommand<FlashEraseCommand>
{
    public FlashEraseCommand(MeadowConnectionManager connectionManager, ILoggerFactory loggerFactory)
        : base(connectionManager, loggerFactory)
    {
    }

    protected override async ValueTask ExecuteCommand()
    {
        var connection = await GetCurrentConnection();

        if (connection == null)
        {
            return;
        }

        Logger?.LogInformation($"Erasing flash...");

        connection.DeviceMessageReceived += (s, e) =>
        {
            Logger?.LogInformation(e.message);
        };

        await connection.Device.EraseFlash(CancellationToken);
    }
}
