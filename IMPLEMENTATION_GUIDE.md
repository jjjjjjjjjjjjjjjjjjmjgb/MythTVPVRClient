# MythTV API Implementation Guide

This guide shows you how to implement the MythTV API integration in the LiveTvService.cs file.

## MythTV API Basics

MythTV backend runs on port 6544 (default) and provides HTTP endpoints:
- Base URL: `http://<server-ip>:6544`
- All responses are JSON
- No authentication required by default (unless configured)

## Configuration

First, add a helper method to get the base URL:

```csharp
private string GetBaseUrl()
{
    var config = Plugin.Instance.Configuration;
    return $"http://{config.ServerIp}:{config.ServerPort}";
}
```

## 1. Get Channels

MythTV API endpoint: `/Channel/GetChannelInfoList`

```csharp
public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
{
    var config = Plugin.Instance.Configuration;
    var baseUrl = GetBaseUrl();
    var url = $"{baseUrl}/Channel/GetChannelInfoList";

    _logger.LogInformation("Getting channels from MythTV: {Url}", url);

    try
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get channels: {StatusCode}", response.StatusCode);
            return Enumerable.Empty<ChannelInfo>();
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var mythResponse = JsonSerializer.Deserialize<MythChannelListResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var channels = mythResponse?.ChannelInfoList?.ChannelInfos?.Select(c => new ChannelInfo
        {
            Id = c.ChanId.ToString(),
            Name = c.ChannelName,
            Number = c.ChanNum,
            ImageUrl = c.IconURL,
            IsFavorite = false,
            IsHD = !string.IsNullOrEmpty(c.IconURL) // Simple heuristic
        }) ?? Enumerable.Empty<ChannelInfo>();

        _logger.LogInformation("Retrieved {Count} channels from MythTV", channels.Count());
        return channels;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting channels from MythTV");
        return Enumerable.Empty<ChannelInfo>();
    }
}
```

You'll need to create response models:
```csharp
public class MythChannelListResponse
{
    public MythChannelInfoList? ChannelInfoList { get; set; }
}

public class MythChannelInfoList
{
    public List<MythChannelInfo>? ChannelInfos { get; set; }
}

public class MythChannelInfo
{
    public int ChanId { get; set; }
    public string? ChannelName { get; set; }
    public string? ChanNum { get; set; }
    public string? IconURL { get; set; }
}
```

## 2. Get Program Guide (EPG)

MythTV API endpoint: `/Guide/GetProgramGuide`

```csharp
public async Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken)
{
    var baseUrl = GetBaseUrl();
    var startTime = startDateUtc.ToString("yyyy-MM-ddTHH:mm:ss");
    var endTime = endDateUtc.ToString("yyyy-MM-ddTHH:mm:ss");
    var url = $"{baseUrl}/Guide/GetProgramGuide?StartTime={startTime}&EndTime={endTime}&Details=true";

    _logger.LogInformation("Getting programs for channel {ChannelId} from MythTV", channelId);

    try
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get programs: {StatusCode}", response.StatusCode);
            return Enumerable.Empty<ProgramInfo>();
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var mythResponse = JsonSerializer.Deserialize<MythProgramGuideResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var programs = mythResponse?.ProgramGuide?.Programs?
            .Where(p => p.ChanId.ToString() == channelId)
            .Select(p => new ProgramInfo
            {
                Id = $"{p.ChanId}_{p.StartTime}",
                ChannelId = p.ChanId.ToString(),
                Title = p.Title,
                Overview = p.Description,
                StartDate = p.StartTime,
                EndDate = p.EndTime,
                IsMovie = p.CategoryType == "movie",
                IsSeries = p.CategoryType == "series",
                EpisodeTitle = p.SubTitle,
                SeasonNumber = p.Season,
                EpisodeNumber = p.Episode,
                OriginalAirDate = p.AirDate,
                Genres = p.Category?.Split(',').ToList() ?? new List<string>(),
                ImageUrl = p.IconURL
            }) ?? Enumerable.Empty<ProgramInfo>();

        _logger.LogInformation("Retrieved {Count} programs for channel {ChannelId}", programs.Count(), channelId);
        return programs;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting programs from MythTV");
        return Enumerable.Empty<ProgramInfo>();
    }
}
```

Response models:
```csharp
public class MythProgramGuideResponse
{
    public MythProgramGuide? ProgramGuide { get; set; }
}

public class MythProgramGuide
{
    public List<MythProgram>? Programs { get; set; }
}

public class MythProgram
{
    public int ChanId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? CategoryType { get; set; }
    public int? Season { get; set; }
    public int? Episode { get; set; }
    public DateTime? AirDate { get; set; }
    public string? IconURL { get; set; }
}
```

## 3. Get Recordings

MythTV API endpoint: `/Dvr/GetRecordedList`

```csharp
public async Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(CancellationToken cancellationToken)
{
    var baseUrl = GetBaseUrl();
    var url = $"{baseUrl}/Dvr/GetRecordedList";

    _logger.LogInformation("Getting recordings from MythTV");

    try
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get recordings: {StatusCode}", response.StatusCode);
            return Enumerable.Empty<RecordingInfo>();
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var mythResponse = JsonSerializer.Deserialize<MythRecordedListResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var recordings = mythResponse?.RecordedList?.Recordings?
            .Select(r => new RecordingInfo
            {
                Id = r.RecordingId.ToString(),
                ChannelId = r.ChanId.ToString(),
                Title = r.Title,
                Overview = r.Description,
                StartDate = r.StartTime,
                EndDate = r.EndTime,
                IsMovie = r.CategoryType == "movie",
                IsSeries = r.CategoryType == "series",
                EpisodeTitle = r.SubTitle,
                SeasonNumber = r.Season,
                EpisodeNumber = r.Episode,
                OriginalAirDate = r.AirDate,
                Genres = r.Category?.Split(',').ToList() ?? new List<string>(),
                Status = r.RecordingStatus == "Recording" ? RecordingStatus.InProgress : RecordingStatus.Completed,
                Path = r.FileName
            }) ?? Enumerable.Empty<RecordingInfo>();

        _logger.LogInformation("Retrieved {Count} recordings from MythTV", recordings.Count());
        return recordings;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting recordings from MythTV");
        return Enumerable.Empty<RecordingInfo>();
    }
}
```

Response models:
```csharp
public class MythRecordedListResponse
{
    public MythRecordedList? RecordedList { get; set; }
}

public class MythRecordedList
{
    public List<MythRecording>? Recordings { get; set; }
}

public class MythRecording
{
    public int RecordingId { get; set; }
    public int ChanId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? CategoryType { get; set; }
    public int? Season { get; set; }
    public int? Episode { get; set; }
    public DateTime? AirDate { get; set; }
    public string? RecordingStatus { get; set; }
    public string? FileName { get; set; }
}
```

## 4. Get Live Stream

MythTV API endpoint: `/Content/GetLiveStream`

```csharp
public async Task<MediaSourceInfo> GetChannelStream(string channelId, string streamId, CancellationToken cancellationToken)
{
    var baseUrl = GetBaseUrl();
    var url = $"{baseUrl}/Content/GetLiveStream?ChanId={channelId}";

    _logger.LogInformation("Getting stream for channel {ChannelId}", channelId);

    try
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get stream: {StatusCode}", response.StatusCode);
            throw new Exception("Failed to get live stream");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var mythResponse = JsonSerializer.Deserialize<MythLiveStreamResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var mediaSource = new MediaSourceInfo
        {
            Id = streamId,
            Path = mythResponse?.LiveStream?.URL ?? string.Empty,
            Protocol = MediaProtocol.Http,
            IsLiveStream = true,
            SupportsDirectPlay = true,
            SupportsDirectStream = true,
            SupportsTranscoding = false
        };

        _logger.LogInformation("Retrieved stream URL: {Url}", mediaSource.Path);
        return mediaSource;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting stream from MythTV");
        throw;
    }
}
```

Response model:
```csharp
public class MythLiveStreamResponse
{
    public MythLiveStream? LiveStream { get; set; }
}

public class MythLiveStream
{
    public string? URL { get; set; }
    public int? Port { get; set; }
    public string? Host { get; set; }
}
```

## 5. Schedule Recording

MythTV API endpoint: `/Dvr/AddRecordSchedule`

```csharp
public async Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
{
    var baseUrl = GetBaseUrl();
    var url = $"{baseUrl}/Dvr/AddRecordSchedule?ChanId={info.ChannelId}&StartTime={info.StartDate:yyyy-MM-ddTHH:mm:ss}&Type=Single";

    _logger.LogInformation("Creating timer for channel {ChannelId}", info.ChannelId);

    try
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create timer: {StatusCode}", response.StatusCode);
            throw new Exception("Failed to create recording schedule");
        }

        _logger.LogInformation("Successfully created timer for channel {ChannelId}", info.ChannelId);
        await Task.CompletedTask;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating timer in MythTV");
        throw;
    }
}
```

## Testing

After implementing these methods:

1. Build the plugin: `dotnet build --configuration Release`
2. Install in Jellyfin
3. Configure your MythTV server IP and port
4. Test connection
5. Import channels
6. Verify EPG data loads
7. Test live streaming

## Notes

- All MythTV API responses are JSON
- Times are in UTC
- Error handling is important - MythTV might be unavailable
- You may need to adjust field names based on your MythTV version
- Check MythTV API docs: https://www.mythtv.org/wiki/Services_API
