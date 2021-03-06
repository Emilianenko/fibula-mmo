﻿// -----------------------------------------------------------------
// <copyright file="UseItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.MovementEvents
{
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications;
    using Serilog;

    /// <summary>
    /// Class that represents an event for an item use.
    /// </summary>
    public class UseItemEvent : BaseEvent
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemEvent"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The id of the item being used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromStackPos">The position in the stack from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        /// <param name="evaluationTime">The time to evaluate the movement.</param>
        public UseItemEvent(
            ILogger logger,
            IGame game,
            IConnectionManager connectionManager,
            ITileAccessor tileAccessor,
            ICreatureFinder creatureFinder,
            uint requestorId,
            ushort typeId,
            Location fromLocation,
            byte fromStackPos = byte.MaxValue,
            byte index = 1,
            EvaluationTime evaluationTime = EvaluationTime.OnBoth)
            : base(logger, requestorId, evaluationTime)
        {
            game.ThrowIfNull(nameof(game));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            connectionManager.ThrowIfNull(nameof(connectionManager));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.ConnectionManager = connectionManager;
            this.CreatureFinder = creatureFinder;
            this.Game = game;

            this.ActionsOnFail.Add(new GenericEventAction(this.NotifyOfFailure));

            var onPassAction = new GenericEventAction(() =>
            {
                var fromCylinder = this.Game.GetCyclinder(fromLocation, ref fromStackPos, ref index, this.Requestor);
                var item = this.Game.FindItemByIdAtLocation(typeId, fromLocation, this.Requestor);

                bool successfulUse = this.Game.PerformItemUse(item, fromCylinder, (byte)(fromLocation.Type == LocationType.InventorySlot ? 0xFF : index), this.Requestor);

                if (!successfulUse)
                {
                    // handles check for isPlayer.
                    this.NotifyOfFailure();

                    return;
                }
            });

            this.ActionsOnPass.Add(onPassAction);
        }

        /// <summary>
        /// Gets the connection manager in use.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Gets the creature finder instance in use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets a reference to the game instance.
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (this.RequestorId > 0 && this.requestor == null)
                {
                    this.requestor = this.CreatureFinder.FindCreatureById(this.RequestorId);
                }

                return this.requestor;
            }
        }

        /// <summary>
        /// Notifies the requestor player, if any,of this failure.
        /// </summary>
        protected void NotifyOfFailure()
        {
            if (this.Requestor is Player player)
            {
                IEnumerable<IConnection> FindPlayerConnectionFunc()
                {
                    return this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem();
                }

                var notificationArgs = new GenericNotificationArguments(
                            new PlayerWalkCancelPacket(player.Direction),
                            new TextMessagePacket(MessageType.StatusSmall, this.ErrorMessage ?? "Sorry, not possible."));

                var notification = new GenericNotification(this.Logger, FindPlayerConnectionFunc, notificationArgs);

                this.Game.RequestNofitication(notification);
            }
        }
    }
}
