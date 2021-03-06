﻿// -----------------------------------------------------------------
// <copyright file="ConfigurationRootExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.CosmosDB
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Data.Contracts.Abstractions;

    /// <summary>
    /// Static class that adds convenient methods to add the concrete implementations contained in this library.
    /// </summary>
    public static class ConfigurationRootExtensions
    {
        /// <summary>
        /// Adds all implementations related to CosmosDb contained in this library to the services collection.
        /// Additionally, registers the options related to the concrete implementations added, such as:
        ///     <see cref="CosmosDbConfigurationOptions"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="configuration">The configuration reference.</param>
        public static void AddCosmosDBDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));

            // configure options
            services.Configure<CosmosDbConfigurationOptions>(configuration.GetSection(nameof(CosmosDbConfigurationOptions)));

            services.AddTransient<IOpenTibiaDbContext, OpenTibiaCosmosDbContext>();
        }
    }
}
