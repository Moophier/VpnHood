﻿using System.Text.Json.Serialization;

namespace VpnHood.AppLib;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppConnectionState
{
    None,
    Initializing,
    Waiting,
    Diagnosing,
    Connecting,
    Connected,
    Disconnecting
}