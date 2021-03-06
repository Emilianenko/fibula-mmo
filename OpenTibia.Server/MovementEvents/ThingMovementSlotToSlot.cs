﻿// -----------------------------------------------------------------
// <copyright file="ThingMovementSlotToSlot.cs" company="2Dudes">
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

    internal class ThingMovementSlotToSlot : BaseMovementEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingMovementSlotToSlot"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="thingMoving"></param>
        /// <param name="fromLocation"></param>
        /// <param name="toLocation"></param>
        /// <param name="count"></param>
        public ThingMovementSlotToSlot(uint requestorId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(requestorId, EvaluationTime.OnExecute)
        {
            // intentionally left thing null check out. Handled by Perform().
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            this.FromLocation = fromLocation;
            this.FromSlot = (byte)this.FromLocation.Slot;

            this.ToLocation = toLocation;
            this.ToSlot = (byte)this.ToLocation.Slot;

            this.Item = thingMoving as IItem;
            this.Count = count;

            this.Conditions.Add(new SlotContainsItemAndCountEventCondition(requestorId, this.Item, this.FromSlot, this.Count));

            this.ActionsOnPass.Add(new GenericEventAction(this.MoveBetweenSlots));
        }

        public Location FromLocation { get; }

        public byte FromSlot { get; }

        public Location ToLocation { get; }

        public byte ToSlot { get; }

        public IItem Item { get; }

        public byte Count { get; }

        private void MoveBetweenSlots()
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

            // attempt to place the intended item at the slot.
            if (!this.Requestor.Inventory.Add(movingItem, out IItem addedItem, this.ToSlot, movingItem.Count))
            {
                // failed to add to the slot, add again to the source slot
                if (!this.Requestor.Inventory.Add(movingItem, out addedItem, this.FromSlot, movingItem.Count))
                {
                    // and we somehow failed to re-add it to the source container...
                    // throw to the ground.
                    IThing thing = movingItem;
                    this.Requestor.Tile.AddThing(ref thing, movingItem.Count);

                    // notify all spectator players of that tile.
                    Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.Requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.Requestor.Location)), this.Requestor.Location);

                    // call any collision events again.
                    if (this.Requestor.Tile.HasCollisionEvents)
                    {
                        foreach (var itemWithCollision in this.Requestor.Tile.ItemsWithCollision)
                        {
                            var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                            var candidate =
                                collisionEvents.FirstOrDefault(
                                    e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId &&
                                         e.Setup(itemWithCollision, thing) && e.CanBeExecuted);

                            // Execute all actions.
                            candidate?.Execute();
                        }
                    }
                }
            }
            else
            {
                if (addedItem == null)
                {
                    return;
                }

                // added the new item to the slot
                if (!this.Requestor.Inventory.Add(addedItem, out IItem extraAddedItem, this.FromSlot, movingItem.Count))
                {
                    // we exchanged or got some leftover item, place back in the source container at any index.
                    IThing remainderThing = extraAddedItem;

                    this.Requestor.Tile.AddThing(ref remainderThing, remainderThing.Count);

                    // notify all spectator players of that tile.
                    Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.Requestor.Tile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.Requestor.Location)), this.Requestor.Location);

                    // call any collision events again.
                    if (!this.Requestor.Tile.HasCollisionEvents)
                    {
                        return;
                    }

                    foreach (var itemWithCollision in this.Requestor.Tile.ItemsWithCollision)
                    {
                        var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                        var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, remainderThing) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }
            }
        }
    }
}