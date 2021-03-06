﻿// -----------------------------------------------------------------
// <copyright file="GenericNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents generic notification arguments.
    /// </summary>
    internal class GenericNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNotificationArguments"/> class.
        /// </summary>
        /// <param name="outgoingPackets">The packets to send as part of this notification.</param>
        public GenericNotificationArguments(params IOutgoingPacket[] outgoingPackets)
        {
            if (outgoingPackets == null || !outgoingPackets.Any())
            {
                throw new ArgumentNullException(nameof(outgoingPackets));
            }

            this.OutgoingPackets = outgoingPackets;
        }

        /// <summary>
        /// Gets the packets to be included in this notification.
        /// </summary>
        public IEnumerable<IOutgoingPacket> OutgoingPackets { get; }
    }
}