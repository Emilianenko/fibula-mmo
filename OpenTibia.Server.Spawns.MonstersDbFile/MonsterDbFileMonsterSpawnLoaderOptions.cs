﻿// -----------------------------------------------------------------
// <copyright file="MonsterDbFileMonsterSpawnLoaderOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Spawns.MonstersDbFile
{
    /// <summary>
    /// Class that represents options for the <see cref="MonsterDbFileMonsterSpawnLoader"/>.
    /// </summary>
    public class MonsterDbFileMonsterSpawnLoaderOptions
    {
        /// <summary>
        /// Gets or sets the path to the file to load.
        /// </summary>
        public string FilePath { get; set; }
    }
}
