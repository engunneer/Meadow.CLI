﻿using CliFx.Attributes;
using Microsoft.Extensions.Logging;

namespace Meadow.CLI.Commands.DeviceManagement;

[Command("runtime enable", Description = "Sets the runtime to run on the Meadow board then resets it")]
public class RuntimeEnableCommand : BaseDeviceCommand<RuntimeEnableCommand>
{
    public RuntimeEnableCommand(MeadowConnectionManager connectionManager, ILoggerFactory loggerFactory)
        : base(connectionManager, loggerFactory)
    {
        Logger?.LogInformation($"Enabling runtime...");
    }

    protected override async ValueTask ExecuteCommand()
    {
        var connection = await GetCurrentConnection();

        if (connection == null)
        {
            return;
        }

        await connection.Device.RuntimeEnable(CancellationToken);

        var state = await connection.Device.IsRuntimeEnabled(CancellationToken);

        Logger?.LogInformation($"Runtime is {(state ? "ENABLED" : "DISABLED")}");
    }
}
