using System;
using System.Collections.Generic;
using System.IO;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using static AramBuddy.Config;

namespace AramBuddy.MainCore.Utility
{
    internal class Chatting
    {
        private static string Start;
        private static string End;

        private static readonly List<string> StartMsg = new List<string>
        {
            "Hi", "Hello", "Greetings", "GL", "HF", "GLHF", "GL HF"
        };

        private static readonly List<string> EndMsg = new List<string>
        {
            "GG", "WP", "GGWP", "GG WP"
        };

        public static void Init()
        {
            var startfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\AramBuddy\\Chat\\Start.txt";
            var endfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\AramBuddy\\Chat\\End.txt";
            var random = new Random();

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\AramBuddy\\Chat\\"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\AramBuddy\\Chat\\");
            }
            if (!File.Exists(startfile))
            {
                File.Create(startfile);
            }
            if (!File.Exists(endfile))
            {
                File.Create(endfile);
            }

            Start = File.ReadAllLines(startfile).Length == 0 ? StartMsg[random.Next(StartMsg.Count)] : File.ReadAllLines(startfile)[random.Next(File.ReadAllLines(startfile).Length)];
            End = File.ReadAllLines(endfile).Length == 0 ? EndMsg[random.Next(EndMsg.Count)] : File.ReadAllLines(endfile)[random.Next(File.ReadAllLines(endfile).Length)];

            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
            Events.OnGameEnd += Events_OnGameEnd;
        }

        private static void Events_OnGameEnd(EventArgs args)
        {
            if(EnableChat)
                Core.DelayAction(() => Chat.Say("/all " + End), new Random().Next(500 + Game.Ping, 2000 + Game.Ping));
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if(Game.Time <= 200 && EnableChat)
                Core.DelayAction(() => Chat.Say("/all " + Start), new Random().Next(500 + Game.Ping, (5000 + Game.Ping) * 2));
        }
    }
}
