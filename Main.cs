using XLMultiplayerServer;
using System.Timers;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XLSMapPlugin
{
    public class Main
    {
        private static Plugin pluginInfo;
        private static Timer mapTimer;

        private static string nextMapHash;

        [JsonProperty("Interval")]
        private static int INTERVAL { get; set; } = 15;

        [JsonProperty("OverrideRatio")]
        private static int RATIO { get; set; } = 60;

        [JsonProperty("RandomizeMaps")]
        private static bool RANDOMIZE { get; set; } = true;

        [JsonProperty("AllowOverrides")]
        private static bool ALLOW_OVERRIDES { get; set; } = true;

        private static int currentMapIndex;

        private static bool firstRun;

        public static void Load(Plugin info)
        {
            firstRun = true;
            currentMapIndex = 0;
            pluginInfo = info;
            if (File.Exists(Path.Combine(pluginInfo.path, "Config.json")))
            {
                JsonConvert.DeserializeObject<Main>(File.ReadAllText(Path.Combine(pluginInfo.path, "Config.json")));
            }
            pluginInfo.OnToggle = OnToggle;
            pluginInfo.PlayerCommand = OnPlayerCommand;
        }

        private static void OnToggle(bool enabled)
        {
            if (enabled)
            {
                SetTimer();
                if (ALLOW_OVERRIDES)
                {
                    CheckVotes();
                } else
                {
                    ClearVotes();
                }
                SetNextMapHash();
            }
        }

        private static void OnPlayerCommand(string message, Player player)
        {
        }

        private static void SetTimer()
        {
            mapTimer = new Timer(TimeSpan.FromMinutes(15).TotalMilliseconds);
            mapTimer.Elapsed += OnTimerEvent;
            mapTimer.AutoReset = true;
            mapTimer.Enabled = true;
        }

        private static void OnTimerEvent(Object source, ElapsedEventArgs e)
        {
            pluginInfo.ChangeMap(nextMapHash);
            SetNextMapHash();
        }

        private static void SetNextMapHash()
        {
            if (!firstRun)
            {
                if (RANDOMIZE)
                {
                    pluginInfo.LogMessage(pluginInfo.mapList.Count.ToString(), ConsoleColor.Red);
                    Random ran = new Random();
                    string choice = pluginInfo.mapList.ElementAt(ran.Next(0, pluginInfo.mapList.Count - 1)).Key;
                    string mapName = pluginInfo.mapList[choice];
                    pluginInfo.SendServerAnnouncement("Map Will Change in " + INTERVAL.ToString() + " minutes!\nNext Map in Rotation: " + mapName, 15, "da32e3");
                    nextMapHash = choice;
                } else
                {
                    nextMapHash = pluginInfo.mapList.ElementAt(currentMapIndex + 1).Key;
                    currentMapIndex++;
                }
            } else
            {
                firstRun = false;
            }
        }

        private static async void ClearVotes()
        {
            while (pluginInfo.enabled)
            {
                foreach (Player p in pluginInfo.playerList)
                {
                    p.currentVote = "current";
                }
                await Task.Delay(5000);
            }
        }

        private static async void CheckVotes()
        {
            while (pluginInfo.enabled)
            {
                List<Player> nonCurrentList = new List<Player>();
                foreach (Player p in pluginInfo.playerList)
                {
                    if (p.currentVote != "current")
                    {
                        nonCurrentList.Add(p);
                    }
                }
                if (nonCurrentList.Count != 0)
                {
                    if ((pluginInfo.maxPlayers / nonCurrentList.Count) >= RATIO)
                    {
                        mapTimer.Stop();
                        mapTimer.Start();
                    }
                }
                await Task.Delay(10000);
            }
        }
    }
}
