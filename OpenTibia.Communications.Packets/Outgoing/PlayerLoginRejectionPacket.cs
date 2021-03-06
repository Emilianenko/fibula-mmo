﻿// <copyright file="PlayerLoginRejectionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;

    public class PlayerLoginRejectionPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoginRejectionPacket"/> class.
        /// </summary>
        /// <param name="reason"></param>
        public PlayerLoginRejectionPacket(byte reason)
        {
            this.Reason = reason;
        }

        public byte PacketType => (byte)OutgoingManagementPacketType.Error;

        public byte Reason { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerLoginRejectionPacket(this);
        }
    }
}
