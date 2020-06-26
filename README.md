# XLSMapPlugin
Skater XL Server Plugin for Map Rotation and Control - 0.9.2+

# Development

**Requires 0.9.2 Server code or newer to compile with.**

Reference XLMultiplayerServer.dll and Newtonsoft.Json.dll

# Installation

Drag and drop folder from `.zip` in Releases tab above.

# Configuration

```json
{
  "Interval": 15,
  "AllowOverrides": true,
  "OverrideRatio": 60,
  "RandomizeMaps": true
}
```

**Interval** - Minute interval for changing maps automatically. If a vote goes through the timer will reset.

**AllowOverrides** - Allows votes from users to override next map change.

**OverrideRatio** - 0 to 100. If X% of players have a vote, override the timer.

**RandomizeMaps** - Randomize the next map.
