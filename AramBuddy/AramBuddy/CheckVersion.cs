using System;
using System.Drawing;
using System.Net;
using AramBuddy.MainCore.Utility.MiscUtil;
using EloBuddy;
using EloBuddy.SDK.Notifications;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;
using Version = System.Version;

namespace AramBuddy
{
    /// <summary>
    ///     A class Used For Checking AramBuddy Version
    /// </summary>
    internal class CheckVersion
    {
        private static Text text;
        private static string UpdateMsg = string.Empty;
        private const string UpdateMsgPath = "https://raw.githubusercontent.com/plsfixrito/AramBuddy/master/AramBuddy/AramBuddy/msg.txt";
        private const string WebVersionPath = "https://raw.githubusercontent.com/plsfixrito/AramBuddy/master/AramBuddy/AramBuddy/Properties/AssemblyInfo.cs";
        private static readonly Version CurrentVersion = typeof(CheckVersion).Assembly.GetName().Version;
        public static bool Outdated;
        private static bool Sent;
        public static void Init()
        {
            try
            {
                Logger.Send("Checking For Updates..");
                var size = Drawing.Width <= 400 || Drawing.Height <= 400 ? 10F : 40F;
                text = new Text("YOUR ARAMBUDDY IS OUTDATED", new Font("Euphemia", size, FontStyle.Bold)) { Color = Color.White };
                using (var WebClient = new WebClient())
                {
                    using (var request = WebClient.DownloadStringTaskAsync(UpdateMsgPath))
                    {
                        if (request.IsFaulted || request.IsCanceled)
                        {
                            Logger.Send("Wrong Response, Or Request Was Cancelled", Logger.LogLevel.Warn);
                            Logger.Send(request?.Exception?.InnerException?.Message, Logger.LogLevel.Warn);
                            Console.WriteLine(request.Result);
                        }
                        else
                        {
                            UpdateMsg = request.Result;
                        }
                    }
                    using (var request2 = WebClient.DownloadStringTaskAsync(WebVersionPath))
                    {
                        if (request2.IsFaulted || request2.IsCanceled)
                        {
                            Logger.Send("Wrong Response, Or Request Was Cancelled", Logger.LogLevel.Warn);
                            Logger.Send(request2?.Exception?.InnerException?.Message, Logger.LogLevel.Warn);
                            Console.WriteLine(request2.Result);
                        }
                        else
                        {
                            if (!request2.Result.Contains(CurrentVersion.ToString()))
                            {
                                Drawing.OnEndScene += delegate
                                {
                                    text.Position = new Vector2(Drawing.Width * 0.01f, Drawing.Height * 0.1f);
                                    text.Draw();
                                };
                                Outdated = true;
                                Logger.Send("There is a new Update Available for AramBuddy!", Logger.LogLevel.Warn);
                                Logger.Send("Update Log: " + UpdateMsg);
                            }
                            else
                            {
                                Logger.Send("Your AramBuddy is updated !");
                            }
                        }
                    }
                }

                Game.OnTick += delegate
                {
                    if (UpdateMsg != string.Empty && !Sent && Outdated)
                    {
                        Chat.Print("<b>AramBuddy: There is a new Update Available for AramBuddy !</b>");
                        Chat.Print("<b>AramBuddy Update Log: " + UpdateMsg + "</b>");
                        Notifications.Show(new SimpleNotification("ARAMBUDDY OUTDATED", "Update Log: " + UpdateMsg), 25000);
                        Sent = true;
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Send("Failed To Check for Updates !", ex, Logger.LogLevel.Error);
            }
        }
    }
}
