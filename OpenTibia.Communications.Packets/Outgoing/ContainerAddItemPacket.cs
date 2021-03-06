﻿// -----------------------------------------------------------------
// <copyright file="ContainerAddItemPacket.cs" company="2Dudes">
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

    public class ContainerAddItemPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerAddItemPacket"/> class.
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="item"></param>
        public ContainerAddItemPacket(byte containerId, IItem item)
        {
            this.ContainerId = containerId;
            this.Item = item;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.ContainerAddItem;

        public byte ContainerId { get; }

        public IItem Item { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteContainerAddItemPacket(this);
        }
    }
}