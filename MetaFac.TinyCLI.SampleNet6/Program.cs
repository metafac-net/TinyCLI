﻿// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MiniCLI;
using MyCommands;

ValueTask<DateTime> UnixDateToDateTime(double unixDate)
{
    var result = DateTime.UnixEpoch.AddDays(unixDate).ToLocalTime();
    return new ValueTask<DateTime>(result);
}

var command1 = new Cmd<double, DateTime>(
                "unix2dt",
                "Converts a Unix date number to a DateTime",
                new Arg<double>("n", "number", "The Unix date number", double.Parse),
                UnixDateToDateTime, null);

//int exitCode = await command1.Run(args);

//var commands = new UnixTools();

//using var loggerFactory =
//    LoggerFactory.Create(builder =>
//        builder
//        .SetMinimumLevel(LogLevel.Trace)
//        .AddSimpleConsole(options =>
//        {
//            options.ColorBehavior = LoggerColorBehavior.Enabled;
//            options.IncludeScopes = true;
//            options.SingleLine = true;
//            options.TimestampFormat = "HH:mm:ss ";
//        }));

ILogger? logger = null; // loggerFactory.CreateLogger<OuterCommands>();

var logOptions = new LogOptions(minimumLevel: LogLevel.Trace, allowMarkup: false, externalLogger: logger);
var options = new CmdOptions(logOptions: logOptions);
var commands = new OuterCommands(options);
int exitCode = await commands.Run(args);
return exitCode;
