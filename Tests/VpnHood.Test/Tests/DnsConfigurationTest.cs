﻿using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VpnHood.Common.Net;
using VpnHood.Server.Access.Configurations;

namespace VpnHood.Test.Tests;

[TestClass]
public class DnsConfigurationTest
{
    [TestMethod]
    public async Task Server_specify_dns_servers()
    {
        // create server
        var fileAccessManagerOptions = TestHelper.CreateFileAccessManagerOptions();
        fileAccessManagerOptions.DnsServers = [IPAddress.Parse("1.1.1.1"), IPAddress.Parse("1.1.1.2")];
        await using var server = TestHelper.CreateServer(fileAccessManagerOptions);

        // create client
        var token = TestHelper.CreateAccessToken(server);
        await using var client = await TestHelper.CreateClient(token);

        CollectionAssert.AreEqual(fileAccessManagerOptions.DnsServers, client.DnsServers);
        Assert.IsTrue(client.Stat.IsDnsServersAccepted);
    }

    [TestMethod]
    public async Task Client_specify_dns_servers()
    {
        // create server
        var fileAccessManagerOptions = TestHelper.CreateFileAccessManagerOptions();
        fileAccessManagerOptions.DnsServers = [IPAddress.Parse("1.1.1.1"), IPAddress.Parse("1.1.1.2")];
        await using var server = TestHelper.CreateServer(fileAccessManagerOptions);

        // create client
        var token = TestHelper.CreateAccessToken(server);
        var clientOptions = TestHelper.CreateClientOptions();
        clientOptions.DnsServers = [IPAddress.Parse("200.0.0.1"), IPAddress.Parse("200.0.0.2")];
        await using var client = await TestHelper.CreateClient(token, clientOptions: clientOptions);

        CollectionAssert.AreEqual(clientOptions.DnsServers, client.DnsServers);
        Assert.IsTrue(client.Stat.IsDnsServersAccepted);
    }

    [TestMethod]
    public async Task Server_override_dns_servers()
    {
        var clientDnsServers = new[] {IPAddress.Parse("200.0.0.1"), IPAddress.Parse("200.0.0.2")};

        // create server
        var fileAccessManagerOptions = TestHelper.CreateFileAccessManagerOptions();
        fileAccessManagerOptions.DnsServers = [IPAddress.Parse("1.1.1.1"), IPAddress.Parse("1.1.1.2")];
        fileAccessManagerOptions.NetFilterOptions = new NetFilterOptions()
        {
            ExcludeIpRanges = clientDnsServers.Select(x => new IpRange(x)).ToArray()
        };
        await using var server = TestHelper.CreateServer(fileAccessManagerOptions);

        // create client
        var token = TestHelper.CreateAccessToken(server);
        var clientOptions = TestHelper.CreateClientOptions();
        clientOptions.DnsServers = clientDnsServers;
        await using var client = await TestHelper.CreateClient(token, clientOptions: clientOptions);

        CollectionAssert.AreEqual(fileAccessManagerOptions.DnsServers, client.DnsServers);
        Assert.IsFalse(client.Stat.IsDnsServersAccepted);
    }
}