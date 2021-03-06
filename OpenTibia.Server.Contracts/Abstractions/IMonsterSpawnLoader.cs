﻿// -----------------------------------------------------------------
// <copyright file="IMonsterSpawnLoader.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for an <see cref="IMonsterSpawnLoader"/> loader.
    /// </summary>
    public interface IMonsterSpawnLoader
    {
        /// <summary>
        /// Attempts to load the monster spawns.
        /// </summary>
        /// <returns>The collection of loaded monster spawns.</returns>
        IEnumerable<Spawn> LoadSpawns();
    }
}
