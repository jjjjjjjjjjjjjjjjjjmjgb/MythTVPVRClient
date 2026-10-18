# Jellyfin MythTV Plugin

A Jellyfin plugin for integrating MythTV Live TV functionality with Jellyfin Server.

## Installation

### Option 1: Add Repository to Jellyfin (Recommended)

1. Open Jellyfin Dashboard
2. Go to **Plugins** → **Repositories**
3. Click **+** to add a new repository
4. Enter this URL:
   ```
   https://raw.githubusercontent.com/jjjjjjjjjjjjjjjjjmjgb/MythTVPVRClient/main/manifest.json
   ```
5. Click **Save**
6. Go to **Plugins** → **Catalog**
7. Find "MythTV" in the list and click **Install**

### Option 2: Manual Installation

1. Go to the [Actions tab](https://github.com/jjjjjjjjjjjjjjjjjmjgb/MythTVPVRClient/actions)
2. Click on the latest workflow run
3. Download the artifact
4. Extract the ZIP file
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
