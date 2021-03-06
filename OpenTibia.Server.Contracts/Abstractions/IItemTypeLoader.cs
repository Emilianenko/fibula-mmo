﻿// -----------------------------------------------------------------
// <copyright file="IItemTypeLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for an <see cref="IItemType"/> loader.
    /// </summary>
    public interface IItemTypeLoader
    {
        /// <summary>
        /// Attempts to load the item catalog.
        /// </summary>
        /// <returns>The catalog, containing a mapping of loaded id to the item types.</returns>
        IDictionary<ushort, IItemType> LoadTypes();
    }
}
