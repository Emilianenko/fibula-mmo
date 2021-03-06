﻿// -----------------------------------------------------------------
// <copyright file="ThingMovementSlotToGround.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Movement
{
    using System;
    using System.Linq;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Movement.EventConditions;
    using OpenTibia.Server.Notifications;

    internal class ThingMovementSlotToGround : BaseMovementEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingMovementSlotToGround"/> class.
        /// </summary>
        /// <param name="creatureRequestingId"></param>
        /// <param name="thingMoving"></param>
        /// <param name="fromLocation"></param>
        /// <param name="toLocation"></param>
        /// <param name="count"></param>
        public ThingMovementSlotToGround(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(creatureRequestingId, EvaluationTime.OnExecute)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            this.FromLocation = fromLocation;
            this.FromSlot = (byte)fromLocation.Slot;

            this.ToLocation = toLocation;
            this.ToTile = Game.Instance.GetTileAt(this.ToLocation);

            this.Item = thingMoving as IItem;
            this.Count = count;

            if (this.Requestor != null)
            {
                this.Conditions.Add(new CanThrowBetweenEventCondition(this.RequestorId, this.Requestor.Location, this.ToLocation));
            }

            this.Conditions.Add(new SlotContainsItemAndCountEventCondition(creatureRequestingId, this.Item, this.FromSlot, this.Count));
            this.Conditions.Add(new LocationNotObstructedEventCondition(this.RequestorId, this.Item, this.ToLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(this.ToLocation));

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveFromSlotToGround));
        }

        public Location FromLocation { get; }

        public byte FromSlot { get; }

        public Location ToLocation { get; }

        public ITile ToTile { get; }

        public IItem Item { get; }

        public byte Count { get; }

        private void MoveFromSlotToGround()
        {
            if (this.Item == null || this.Requestor == null)
            {
                return;
            }

            // attempt to remove the item from the inventory
            var movingItem = this.Requestor.Inventory?.Remove(this.FromSlot, this.Count, out bool partialRemove);

            if (movingItem == null)
            {
                return;
            }

            // add the remaining item to the destination tile.
            IThing thing = movingItem;
            this.ToTile.AddThing(ref thing, movingItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.ToTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.ToTile.Location)), this.ToTile.Location);

            // and handle collision.
            if (!this.ToTile.HasCollisionEvents)
            {
                return;
            }

            foreach (var itemWithCollision in this.ToTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing, this.Requestor as IPlayer) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}