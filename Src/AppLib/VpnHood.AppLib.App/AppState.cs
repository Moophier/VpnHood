﻿using VpnHood.AppLib.Abstractions;
using VpnHood.AppLib.ClientProfiles;
using VpnHood.Core.Client;
using VpnHood.Core.Common.ApiClients;
using VpnHood.Core.Common.Messaging;
using VpnHood.Core.Common.Tokens;

namespace VpnHood.AppLib;

public class AppState
{
    public required DateTime ConfigTime { get; init; }
    public required DateTime? ConnectRequestTime { get; init; }
    public required AppConnectionState ConnectionState { get; init; }
    public required ApiError? LastError { get; init; }
    public required ClientProfileBaseInfo? ClientProfile { get; init; }
    public required ServerLocationInfo? ServerLocationInfo { get; init; }
    public required bool IsIdle { get; init; }
    public required bool PromptForLog { get; init; }
    public required bool LogExists { get; init; }
    public required bool HasDiagnoseRequested { get; init; }
    public required bool HasDisconnectedByUser { get; init; }
    public required bool HasProblemDetected { get; init; }
    public required SessionStatus? SessionStatus { get; init; }
    public required Traffic Speed { get; init; }
    public required Traffic SessionTraffic { get; init; }
    public required Traffic AccountTraffic { get; init; }
    public required string? ClientCountryCode { get; init; }
    public required string? ClientCountryName { get; init; }
    public required bool IsWaitingForAd { get; init; }
    public required VersionStatus VersionStatus { get; init; }
    public required PublishInfo? LastPublishInfo { get; init; }
    public required bool? IsUdpChannelSupported { get; init; }
    public required bool CanDisconnect { get; init; }
    public required bool CanConnect { get; init; }
    public required bool CanDiagnose { get; init; }
    public required UiCultureInfo CurrentUiCultureInfo { get; init; }
    public required UiCultureInfo SystemUiCultureInfo { get; init; }
    public required BillingPurchaseState? PurchaseState { get; init; }
    public required int? TcpTunnelledCount { get; init; }
    public required int? TcpPassthruCount { get; init; }
}