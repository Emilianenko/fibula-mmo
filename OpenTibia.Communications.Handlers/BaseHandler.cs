﻿// -----------------------------------------------------------------
// <copyright file="BaseHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that serves as the base implementation for all packet handlers in the service.
    /// </summary>
    public abstract class BaseHandler : IHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHandler"/> class.
        /// </summary>
        protected BaseHandler()
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public abstract byte ForPacketType { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A value tuple with a value indicating whether the handler intends to respond, and a collection of <see cref="IOutgoingPacket"/>s that compose that response.</returns>
        public abstract (bool IntendsToRespond, IEnumerable<IOutgoingPacket> ResponsePackets) HandleRequest(INetworkMessage message, IConnection connection);

        /// <summary>
        /// Prepares a <see cref="INetworkMessage"/> with the reponse packets supplied.
        /// </summary>
        /// <param name="responsePackets">The packets that compose that response.</param>
        /// <returns>The response as a <see cref="INetworkMessage"/>.</returns>
        public virtual INetworkMessage PrepareResponse(IEnumerable<IOutgoingPacket> responsePackets)
        {
            if (responsePackets == null || !responsePackets.Any())
            {
                return null;
            }

            INetworkMessage outgoingMessage = new NetworkMessage();

            foreach (var outPacket in responsePackets)
            {
                outPacket.WriteToMessage(outgoingMessage);
            }

            return outgoingMessage;
        }
    }
}