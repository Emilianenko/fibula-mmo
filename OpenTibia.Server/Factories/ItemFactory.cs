﻿// -----------------------------------------------------------------
// <copyright file="ItemFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Factories
{
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an <see cref="IItem"/> factory.
    /// </summary>
    public class ItemFactory : IItemFactory
    {
        /// <summary>
        /// Gets a reference to the item types catalog to use in this factory.
        /// </summary>
        private readonly IDictionary<ushort, IItemType> itemTypesCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemFactory"/> class.
        /// </summary>
        /// <param name="itemLoader">A reference to the item type loader to use.</param>
        public ItemFactory(IItemTypeLoader itemLoader)
        {
            itemLoader.ThrowIfNull(nameof(itemLoader));

            this.ItemTypeLoader = itemLoader;

            this.itemTypesCatalog = this.ItemTypeLoader.LoadTypes();
        }

        /// <summary>
        /// Gets the reference to the item type loader in use.
        /// </summary>
        public IItemTypeLoader ItemTypeLoader { get; }

        /// <summary>
        /// Creates a new <see cref="IItem"/> given the id of its type.
        /// </summary>
        /// <param name="typeId">The id of the type.</param>
        /// <returns>The new <see cref="IItem"/> instance.</returns>
        public IItem Create(ushort typeId)
        {
            if (typeId < 100 || !this.itemTypesCatalog.ContainsKey(typeId))
            {
                return null;
            }

            // TODO: chest actually means a quest chest...
            if (this.itemTypesCatalog[typeId].Flags.Contains(ItemFlag.Container) || this.itemTypesCatalog[typeId].Flags.Contains(ItemFlag.Chest))
            {
                return new ContainerItem(this.itemTypesCatalog[typeId]);
            }

            return new Item(this.itemTypesCatalog[typeId]);
        }
    }
}
