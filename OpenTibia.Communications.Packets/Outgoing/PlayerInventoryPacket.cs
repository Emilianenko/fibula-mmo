﻿// -----------------------------------------------------------------
// <copyright file="PlayerInventoryPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a player's inventory packet.
    /// </summary>
    public class PlayerInventoryPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerInventoryPacket"/> class.
        /// </summary>
        /// <param name="player">The player referenced.</param>
        public PlayerInventoryPacket(IPlayer player)
        {
            this.Player = player;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.InventoryItem;

        /// <summary>
        /// Gets a reference to the player.
        /// </summary>
        public IPlayer Player { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WritePlayerInventoryPacket(this);
        }
    }
}
