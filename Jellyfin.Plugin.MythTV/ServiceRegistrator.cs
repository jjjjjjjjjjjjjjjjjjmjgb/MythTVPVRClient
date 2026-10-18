using System;
using Jellyfin.Plugin.MythTV.Configuration;
using MediaBrowser.Common.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.MythTV;

/// <summary>
/// Class ServiceRegistrator.
/// </summary>
public class ServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerConfigurationManager serverConfigurationManager)
    {
        serviceCollection.AddSingleton<LiveTvService>();
    }
}
