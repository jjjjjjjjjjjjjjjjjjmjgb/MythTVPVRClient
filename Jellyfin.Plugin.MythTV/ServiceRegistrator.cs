using System;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.MythTV;

/// <summary>
/// Class ServiceRegistrator.
/// </summary>
public class ServiceRegistrator
{
    /// <summary>
    /// Register plugin services
    /// </summary>
    public static void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<LiveTvService>();
    }
}
