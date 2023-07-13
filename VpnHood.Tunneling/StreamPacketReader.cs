﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PacketDotNet;
using VpnHood.Common.Utils;

namespace VpnHood.Tunneling;

public class StreamPacketReader : IAsyncDisposable
{
    private readonly List<IPPacket> _ipPackets = new();
    private readonly ReadCacheStream _stream;
    private byte[] _packetBuffer = new byte[1600];
    private int _packetBufferCount;

    public StreamPacketReader(Stream stream)
    {
        _stream = new ReadCacheStream(stream, true, 15000); // max batch
    }


    /// <returns>null if read nothing</returns>
    public async Task<IPPacket[]?> ReadAsync(CancellationToken cancellationToken)
    {
        _ipPackets.Clear();

        while (true)
        {
            // read packet header
            const int minPacketSize = 20;
            if (_packetBufferCount < minPacketSize)
            {
                var toRead = minPacketSize - _packetBufferCount;
                var read = await _stream.ReadAsync(_packetBuffer, _packetBufferCount, toRead, cancellationToken);
                _packetBufferCount += read;
                
                // is eof?
                if (read == 0 && _packetBufferCount == 0)
                    return null;

                // is unexpected eof?
                if (read == 0)
                    throw new Exception("Stream has been unexpectedly closed before reading the rest of packet.");

                // is uncompleted header?
                if (toRead != read)
                    break; 

                // is just header?
                if (!_stream.DataAvailableInCache)
                    break;
            }

            // find packet length
            var packetLength = PacketUtil.ReadPacketLength(_packetBuffer, 0);
            if (_packetBufferCount < packetLength)
            {
                //not sure we get any packet more than 1600
                if (_packetBufferCount > _packetBuffer.Length) 
                    Array.Resize(ref _packetBuffer, _packetBufferCount); 

                var toRead = packetLength - _packetBufferCount;
                var read = await _stream.ReadAsync(_packetBuffer, _packetBufferCount, toRead, cancellationToken);
                _packetBufferCount += read;
                if (read == 0)
                    throw new Exception("Stream has been unexpectedly closed before reading the rest of packet.");

                // is packet read?
                if (toRead != read)
                    break; 
            }

            var ipPacket = Packet.ParsePacket(LinkLayers.Raw, _packetBuffer).Extract<IPPacket>();
            _ipPackets.Add(ipPacket);
            _packetBufferCount = 0;

            // Don't try to read more packet if there is no data in cache
            if (!_stream.DataAvailableInCache)
                break;
        }

        return _ipPackets.ToArray();
    }

    public ValueTask DisposeAsync()
    {
        return _stream.DisposeAsync();
    }
}