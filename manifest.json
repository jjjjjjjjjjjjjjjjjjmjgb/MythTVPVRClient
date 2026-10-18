using System;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.MythTV;

/// <summary>
/// Class PluginConfiguration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        ServerIp = "localhost";
        ServerPort = 6544;
        ApiKey = string.Empty;
        EnableDebugLogging = false;
        ConnectionTimeoutSeconds = 30;
        TunerCount = 4;
    }

    /// <summary>
    /// Gets or sets the MythTV backend server IP address or hostname.
    /// </summary>
    public string ServerIp { get; set; }

    /// <summary>
    /// Gets or sets the MythTV backend port (default: 6544).
    /// </summary>
    public int ServerPort { get; set; }

    /// <summary>
    /// Gets or sets the MythTV API security key (optional).
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether additional debug logging is written.
    /// </summary>
    public bool EnableDebugLogging { get; set; }

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    public int ConnectionTimeoutSeconds { get; set; }

    /// <summary>
    /// Gets or sets the number of tuners to simulate in Jellyfin.
    /// </summary>
    public int TunerCount { get; set; }
}
