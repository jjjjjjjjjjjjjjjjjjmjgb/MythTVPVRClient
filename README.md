# Jellyfin MythTV Plugin

A Jellyfin plugin for integrating MythTV Live TV functionality with Jellyfin Server.

## Status

**Current State**: Build configuration ready - uses GitHub Actions to build the plugin automatically.

## What's Included

✅ **Plugin Structure**: Based on the official NextPVR plugin structure
✅ **Configuration**: MythTV-specific configuration (Server IP, Port, API Key, etc.)
✅ **Web UI**: Configuration page for MythTV settings
✅ **Service Skeleton**: ILiveTvService implementation with TODO markers for MythTV API integration
✅ **GitHub Actions**: Automated build workflow using the Jellyfin Plugin Repository Manager

## Building

This plugin is built automatically by GitHub Actions when you push to the main branch. The workflow uses the [Jellyfin Plugin Repository Manager](https://github.com/oddstr13/jellyfin-plugin-repository-manager) to compile the plugin.

### Manual Build

To build locally:

```bash
cd Jellyfin.Plugin.MythTV
dotnet build --configuration Release
```

## Installation

### From GitHub Actions Artifacts

1. Go to the "Actions" tab in this repository
2. Click on the latest workflow run
3. Download the artifact
4. Extract the DLL
5. Place in your Jellyfin plugins directory:
   - Windows: `%ProgramData%\Jellyfin\Server\plugins\MythTV\`
   - Linux: `/var/lib/jellyfin/plugins/MythTV/`
6. Restart Jellyfin

## What Needs Implementation

The following methods in `LiveTvService.cs` have TODO markers and need MythTV API implementation:

1. **GetChannelsAsync** - Fetch channel list from MythTV
2. **GetProgramsAsync** - Fetch EPG/guide data from MythTV
3. **GetChannelStream** - Generate live stream URLs
4. **GetRecordingStream** - Generate recording stream URLs
5. **GetRecordingsAsync** - Fetch recordings list
6. **CreateTimerAsync** - Schedule single recording
7. **CreateSeriesTimerAsync** - Schedule series recording
8. **UpdateTimerAsync** - Update recording
9. **CancelRecordingAsync** - Cancel recording
10. **GetTimersAsync** - Fetch scheduled recordings

## MythTV API Integration

To complete the plugin, you'll need to implement MythTV API calls. Refer to MythTV API documentation:

- MythTV API Guide: https://www.mythtv.org/wiki/MythTV_API
- Typical endpoints:
  - `/api/Channels` - Channel list
  - `/api/Guide` - EPG data
  - `/api/Stream` - Live streaming
  - `/api/Dvr/Recordings` - Recordings
  - `/api/Dvr/GetSchedule` - Scheduled recordings

## Configuration

- **Server IP**: MythTV backend IP address or hostname
- **Server Port**: MythTV API port (default: 6544)
- **API Key**: Optional security key if configured
- **Connection Timeout**: API request timeout in seconds
- **Tuner Count**: Number of tuners to expose to Jellyfin
- **Debug Logging**: Enable verbose logging

## Compatibility

- **Jellyfin Version**: 12.2.0+
- **.NET Version**: 10.0
- **Target ABI**: 12.0.0.0

## License

MIT License - see LICENSE file for details

## Credits

- Based on the [Jellyfin NextPVR Plugin](https://github.com/jellyfin/jellyfin-plugin-nextpvr)
- Uses [Jellyfin Plugin Repository Manager](https://github.com/oddstr13/jellyfin-plugin-repository-manager) for automated builds
