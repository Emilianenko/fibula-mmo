﻿// -----------------------------------------------------------------
// <copyright file="IAutoMovementInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface that represents the auto movement information.
    /// </summary>
    public interface IAutoMovementInfo
    {
        /// <summary>
        /// Gets the movement directions.
        /// </summary>
        Direction[] Directions { get; }
    }
}