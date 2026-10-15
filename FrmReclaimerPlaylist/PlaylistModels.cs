using System.Collections.Generic;
using Newtonsoft.Json;

public class Playlist
{
    [JsonProperty("version")]
    public int Version { get; set; } = 2;

    [JsonProperty("name")]
    public string Name { get; set; } = "A PLAYLIST";

    [JsonProperty("vote_seconds")]
    public int VoteSeconds { get; set; } = 20;

    [JsonProperty("games")]
    public List<PlaylistGame> Games { get; set; } = new List<PlaylistGame>();
}

public class PlaylistGame
{
    [JsonProperty("map")]
    public string Map { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("preview_image", NullValueHandling = NullValueHandling.Ignore)]
    public string PreviewImage { get; set; }

    [JsonProperty("game")]
    public string Game { get; set; }

    [JsonProperty("world", NullValueHandling = NullValueHandling.Ignore)]
    public WorldSettings World { get; set; }

    [JsonProperty("weight", NullValueHandling = NullValueHandling.Ignore)]
    public int? Weight { get; set; }

    [JsonProperty("rules", NullValueHandling = NullValueHandling.Ignore)]
    public Rules Rules { get; set; }

    [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object> Options { get; set; }
}

public class WorldSettings
{
    [JsonProperty("sprint")]
    public string Sprint { get; set; } = "off";
}

public class Rules
{
    [JsonProperty("score_to_win", NullValueHandling = NullValueHandling.Ignore)]
    public int? ScoreToWin { get; set; }

    [JsonProperty("kill_points", NullValueHandling = NullValueHandling.Ignore)]
    public int? KillPoints { get; set; }

    [JsonProperty("death_points", NullValueHandling = NullValueHandling.Ignore)]
    public int? DeathPoints { get; set; }

    [JsonProperty("suicide_points", NullValueHandling = NullValueHandling.Ignore)]
    public int? SuicidePoints { get; set; }

    [JsonProperty("betrayal_points", NullValueHandling = NullValueHandling.Ignore)]
    public int? BetrayalPoints { get; set; }
}