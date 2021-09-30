using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Timers;
using System.Collections.Generic;
using HSNXT.DSharpPlus.ModernEmbedBuilder;
using System.Drawing;
using RestSharp;
using System.IO;
using SharpLink;
using Discord.WebSocket;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using System.Linq;

namespace DiscordBot
{
    class Program
    { 

        static Timer pingtimer;
        static bool ping = false;

        static Timer rowtimer;
        static bool row = true;

        static string mainmessage = null;
        static DSharpPlus.EventArgs.MessageCreateEventArgs ev = null;
        static DiscordClient mainds = null;

        static List<string> commands;
        static List<string> lmaocommands;
        static List<string> managercommands;

        static int WideChlebaDevided = 20;

        static DSharpPlus.Lavalink.LavalinkTrack[] tracks = new DSharpPlus.Lavalink.LavalinkTrack[100];
        static int trackscount = 0;

        static LavalinkExtension lavalink;

        static DiscordGuild voiceguild;

        static DiscordClient discord;

        static DiscordChannel voicechannel;

        static DiscordActivity act;
        static void Main(string[] args)
        {
            pingtimer = new Timer(500);
            pingtimer.Elapsed += Pingtimer_Elapsed;
            pingtimer.AutoReset = true;

            rowtimer = new Timer(1000);
            rowtimer.Elapsed += Rowtimer_Elapsed;
            rowtimer.AutoReset = true;

            act = new DiscordActivity("Minecraft", ActivityType.Playing);

            commands = new List<string> {"help",
                "chleba",
                "chleba (name)", 
                "wide chleba",
                "wide chleba (number)",
                "",
                "Music:",
                "join",
                "leave",
                "play",
                "pause",
                "resume",
                "skip",
                "clear"};

            lmaocommands = new List<string> {"help lmao",
                "ping (string) (x)",
                "delete (number)"
            };

            managercommands = new List<string> {"help manager",
                "addrole (name) (role)",
                "removerole (role)",
                "removerole (name) (role)",
                "createrole (name)" ,
                "Delay (x)",
                "Delay settings"
            };

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            int delay = 500;
            bool stop = false;


            int i = 0;
            int x = 0;

          
            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "ODg2MTYzNzAyNDc3Mzg5OTA1.YTxmQg.Yrbc9vLsesG2qeQ3RQErpuEWNGA",
                TokenType = TokenType.Bot
            });

            DiscordSocketClient client = new DiscordSocketClient();
            
            LavalinkManager lavalinkManager = new LavalinkManager(client, new LavalinkManagerConfig
            {
                RESTHost = "localhost",
                RESTPort = 2333,
                WebSocketHost = "localhost",
                WebSocketPort = 2333,
                Authorization = "YOUR_SECRET_AUTHORIZATION_KEY",
                TotalShards = 1
            });

            client.Ready += async () =>
            {
                await lavalinkManager.StartAsync();
            };

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            lavalink = discord.UseLavalink();

            discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("stop"))
                {
                    x = 0;
                    i = 0;
                    rowtimer.Stop();
                    row = true;
                }

                if (e.Message.Content.ToLower().StartsWith("ping") && row)
                {
                    var id = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(id);
                    var roles = member.Result.Roles;

                    foreach (var r in roles)
                    {
                        if (r.Name == "Lmao")
                        {
                            string[] message = e.Message.Content.Split();
                            i = Convert.ToInt32(message[2]);
                            mainmessage = message[1];
                            mainds = discord;
                            ev = e;
                            x = 0;
                            row = false;
                            rowtimer.Start();
                        }
                    }
                }

                if (i > x)
                {
                    if (ping || x == 0)
                    {
                        x++;
                        pingtimer.Interval = delay;
                        pingtimer.Start();
                        ping = false;
                    }
                }

                if (e.Message.Content.ToLower().Contains("wide") && e.Message.Content.ToLower().Contains("chleba") && i == x && row)
                {
                    var message = e.Message.Content.Split();
                    if(message.Length == 3)
                    {
                        Bitmap original = new Bitmap(@"images\chleba.png");
                        Bitmap map = new Bitmap(original, new Size(Convert.ToInt32(message[2])/WideChlebaDevided, original.Height/WideChlebaDevided));
                        
                        s.SendMessageAsync(e.Channel, UploadImage(map));
                    }
                    else if (message.Length == 2)
                    {
                        s.SendMessageAsync(e.Channel, "https://imgur.com/a/eyfMcaT");
                    }
                    else if (message.Length == 4)
                    {
                        if(message[2] == "devided")
                        WideChlebaDevided = Convert.ToInt32(message[3]);
                    }
                }
                else 
                if (e.Message.Content.ToLower().Contains("chleba") && i == x && row && !e.Message.Content.ToLower().Contains("wide"))
                {
                    var message = e.Message.Content.Split();

                    if (e.Message.MentionedUsers.Count == 0)
                    {
                        s.SendMessageAsync(e.Channel, "https://imgur.com/a/ZlKNHnX");
                    }
                    else if (message.Length == 2)
                    {
                        var Username = message[1];

                        Bitmap original = new Bitmap(@"images\chleba.png");
                        Bitmap picture = new Bitmap(270,270);


                        System.Net.WebRequest request = System.Net.WebRequest.Create(e.Message.MentionedUsers[0].AvatarUrl);
                        System.Net.WebResponse response = request.GetResponse();
                        System.IO.Stream responseStream = response.GetResponseStream();
                        Bitmap bitmap2 = new Bitmap(responseStream);
                        picture =  new Bitmap(bitmap2,bitmap2.Size);

                        Graphics g = Graphics.FromImage(original);
                        g.DrawImage(picture, 220, 1470, 270, 270);

                        s.SendMessageAsync(e.Channel, UploadImage(original));
                    }
                }

                if (e.Message.Content.ToLower() == "delay settings")
                {
                    var id = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(id);
                    var roles = member.Result.Roles;



                    foreach (var r in roles)
                    {
                        if (r.Name == "Manager")
                        {
                            s.SendMessageAsync(e.Channel, "Delay je nastaven na: " + delay.ToString());
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("delay") && row)
                {
                    var id = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(id);
                    var roles = member.Result.Roles;



                    foreach (var r in roles)
                    {
                        if (r.Name == "Manager")
                        {
                            string[] message = e.Message.Content.Split();
                            delay = Convert.ToInt32(message[1]);
                            s.SendMessageAsync(e.Channel, "Delay byl nastaven na: " + message[1]);
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("help"))
                {
                    string[] allmessage = e.Message.Content.Split();

                    var id = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(id);
                    var roles = member.Result.Roles;

                    foreach (var r in roles)
                    {
                        if(e.Message.Content.ToLower() == "help")
                        {
                            string message = null;
                            foreach (var c in commands)
                            {
                                message = message + c + "\n";
                            }
                            s.SendMessageAsync(e.Channel, "`" + message + "`");
                            break;
                        }
                        else
                        if (allmessage[1] == "lmao") 
                        {
                            if (r.Name == "Lmao")
                            {
                                string message = null;
                                foreach (var c in lmaocommands)
                                {
                                    message = message + c + "\n";
                                }
                                s.SendMessageAsync(e.Channel, "`" + message + "`");
                                break;
                            }
                            
                        }
                        else
                        if (allmessage[1] == "manager") 
                        {
                            if (r.Name == "Manager")
                            {
                                string message = null;
                                foreach (var c in managercommands)
                                {
                                    message = message + c + "\n";
                                }
                                s.SendMessageAsync(e.Channel, "`" + message + "`");
                                break;
                            }
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("addrole"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida);
                    var roles = member.Result.Roles;

                    foreach (var r in roles)
                    {
                        if (r.Name == "Manager")
                        {
                            string[] message = e.Message.Content.Split();
                            string Username = message[1];
                            string role = message[2];

                            var x = e.Guild.Members.Values;

                            foreach (var y in x)
                            {
                                if (Username.Contains(y.Id.ToString()))
                                {
                                    var l = e.Guild.Roles.Values;
                                    DiscordRole id = null;

                                    foreach (var z in l)
                                    {
                                        if (z.Mention.ToString() == role)
                                        {
                                            id = z;
                                        }

                                    }
                                    if (id != null)
                                        y.GrantRoleAsync(id);

                                }
                            }
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("createrole"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida);
                    var roles = member.Result.Roles;

                    foreach (var r in roles)
                    {
                        if (r.Name == "Manager")
                        {
                            string[] message = e.Message.Content.Split();
                            string role = message[1];

                            e.Guild.CreateRoleAsync(role.Replace("@", ""));
                            break;
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("removerole"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida);
                    var roles = member.Result.Roles;

                    foreach (var r in roles)
                    {
                        if (r.Name == "Manager")
                        {
                            string[] message = e.Message.Content.Split();
                            var role = e.Guild.Roles.Values;

                            if (message.Length == 3)
                            {
                                var Username = message[1];
                                var removerole = message[2];

                                var x = e.Guild.Members.Values;

                                foreach (var y in x)
                                {
                                    if (Username.Contains(y.Id.ToString()))
                                    {
                                        var l = e.Guild.Roles.Values;
                                        DiscordRole id = null;

                                        foreach (var z in l)
                                        {
                                            if (z.Mention.ToString() == removerole)
                                            {
                                                id = z;
                                            }

                                        }
                                        if (id != null)
                                            y.RevokeRoleAsync(id);
                                    }
                                }
                            }
                            else if (message.Length == 2)
                            {
                                string removerole = message[1];

                                foreach (var b in role)
                                {
                                    if (b.Mention.ToString() == removerole)
                                    {
                                        b.DeleteAsync();
                                    }
                                }
                            }
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("delete"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida);
                    var roles = member.Result.Roles;

                    foreach (var r in roles)
                    {
                        if (r.Name == "Lmao")
                        {
                            string[] message = e.Message.Content.Split();
                            var number = Convert.ToInt32(message[1]);

                            var delete = e.Channel.GetMessagesAsync(number +1).Result;

                            e.Channel.DeleteMessagesAsync(delete);
                        }
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("join"))
                {
                    join(e, s, lavalink);
                }

                if (e.Message.Content.ToLower().StartsWith("leave"))
                {
                    var lava = lavalink;

                    if (!lava.ConnectedNodes.Any())
                    {
                        s.SendMessageAsync(e.Channel, "Toban ma někde problem");
                        return;
                    }

                    var node = lava.ConnectedNodes.Values.First();

                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida);

                    if (member.Result.VoiceState == null)
                    {
                        s.SendMessageAsync(e.Channel, "Nejsi nikde připojen lmao");
                        return;
                    }

                    var channel = member.Result.VoiceState.Channel;

                    if (channel == null || channel.Type != ChannelType.Voice)
                    {
                        s.SendMessageAsync(e.Channel, "Nejsi ve voicu");
                        return;
                    }

                    var conn = node.GetGuildConnection(channel.Guild);

                    if (conn == null)
                    {
                        s.SendMessageAsync(e.Channel, "Lavalink není připojen lmao.");
                        return;
                    }

                    await conn.DisconnectAsync();
                }

                if (e.Message.Content.ToLower().StartsWith("play"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida).Result;

                    var lava = lavalink;
                    var node = lava.ConnectedNodes.Values.First();
                    var conn = node.GetGuildConnection(member.VoiceState.Guild);

                    voicechannel = e.Message.Channel;

                    if (conn == null)
                    {
                        join(e, s, lavalink);
                    }
                    else
                    {
                        play(e, s, lavalink);
                    }
                }

                if (e.Message.Content.ToLower().StartsWith("pause"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida).Result;

                    if (member.VoiceState == null || member.VoiceState.Channel == null)
                    {
                        s.SendMessageAsync(e.Channel, "Nejsi ve voicu");
                        return;
                    }

                    var lava = lavalink;
                    var node = lava.ConnectedNodes.Values.First();
                    var conn = node.GetGuildConnection(member.VoiceState.Guild);

                    if (conn == null)
                    {
                        s.SendMessageAsync(e.Channel, "Bot není připojen");
                        return;
                    }

                    if (conn.CurrentState.CurrentTrack == null)
                    {
                        return;
                    }

                    await conn.PauseAsync();
                }

                if (e.Message.Content.ToLower().StartsWith("resume"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida).Result;

                    if (member.VoiceState == null || member.VoiceState.Channel == null)
                    {
                        s.SendMessageAsync(e.Channel, "Nejsi ve voicu");
                        return;
                    }

                    var lava = lavalink;
                    var node = lava.ConnectedNodes.Values.First();
                    var conn = node.GetGuildConnection(member.VoiceState.Guild);

                    if (conn == null)
                    {
                        s.SendMessageAsync(e.Channel, "Bot není připojen");
                        return;
                    }

                    if (conn.CurrentState.CurrentTrack == null)
                    {
                        return;
                    }

                    await conn.ResumeAsync();
                }

                if (e.Message.Content.ToLower().StartsWith("clear"))
                {
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida).Result;

                    if (member.VoiceState == null || member.VoiceState.Channel == null)
                    {
                        s.SendMessageAsync(e.Channel, "Nejsi ve voicu");
                        return;
                    }

                    var lava = lavalink;
                    var node = lava.ConnectedNodes.Values.First();
                    var conn = node.GetGuildConnection(member.VoiceState.Guild);

                    if (conn == null)
                    {
                        s.SendMessageAsync(e.Channel, "Bot není připojen");
                        return;
                    }

                    for (int i = 0; i < trackscount; i++)
                    {
                        tracks[i] = null;
                    }

                    trackscount = 0; 

                    conn.StopAsync();
                }

                if (e.Message.Content.ToLower().StartsWith("skip") && row)
                {
                    row = false;
                    rowtimer.Start();
                    var ida = e.Message.Author.Id;
                    var member = e.Guild.GetMemberAsync(ida).Result;

                    if (member.VoiceState == null || member.VoiceState.Channel == null)
                    {
                        s.SendMessageAsync(e.Channel, "Nejsi ve voicu");
                        return;
                    }

                    var lava = lavalink;
                    var node = lava.ConnectedNodes.Values.First();
                    var conn = node.GetGuildConnection(member.VoiceState.Guild);

                    if (conn == null)
                    {
                        s.SendMessageAsync(e.Channel, "Bot není připojen");
                        return;
                    }

                    if (conn.CurrentState.CurrentTrack == null)
                    {
                        return;
                    }

                    if (tracks[0] != null)
                    {
                        playtrack(tracks[0]);

                        SkipSong();
                    }
                    else
                    {
                        conn.StopAsync();
                    }
                }
   
            };
            
            

            await discord.ConnectAsync(act,UserStatus.Online);
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }

        static async void play(DSharpPlus.EventArgs.MessageCreateEventArgs e,DSharpPlus.DiscordClient s,LavalinkExtension lavalink)
        {
            string message = e.Message.Content;
            string search = message.Replace("play ","");

            var ida = e.Message.Author.Id;
            var member = e.Guild.GetMemberAsync(ida).Result;

            if (member.VoiceState == null || member.VoiceState.Channel == null)
            {
                s.SendMessageAsync(voicechannel, "Nejsi ve voicu");
                return;
            }

            var lava = lavalink;
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(member.VoiceState.Guild);

            voiceguild = member.VoiceState.Guild;

            if (conn == null)
            {
                s.SendMessageAsync(voicechannel, "Bot není připojený");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(search);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                s.SendMessageAsync(voicechannel, $"Chyba při hledání {search}.");
                return;
            }

            var track = loadResult.Tracks.First();

            if (conn.CurrentState.CurrentTrack == null)
            {
                playtrack(track);
            }
            else
            {
                s.SendMessageAsync(voicechannel, musicembed("Přidáno do řady", track));
                tracks[trackscount] = track;
                trackscount++;
            }
            conn.PlaybackFinished += Conn_PlaybackFinished;
        }

        static async void playtrack(DSharpPlus.Lavalink.LavalinkTrack track)
        {
            var lava = lavalink;
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(voiceguild);

            discord.SendMessageAsync(voicechannel, musicembed("Právě vibuješ", track));
            conn.PlayAsync(track);
        }

        private static Task Conn_PlaybackFinished(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs e)
        {
            if(tracks[0] != null)
            {
                playtrack(tracks[0]);

                SkipSong();
            }

            return null;
        }

        static async void SkipSong()
        {
            for (int i = 0; i < trackscount; i++)
            {
                if (i + 1 != trackscount)
                {
                    tracks[i] = tracks[i + 1];
                }
                else
                {
                    tracks[i] = null;
                }
            }
            trackscount = trackscount - 1;
        }

        static async void join(DSharpPlus.EventArgs.MessageCreateEventArgs e, DSharpPlus.DiscordClient s, LavalinkExtension lavalink)
        {
            var lava = lavalink;

            if (!lava.ConnectedNodes.Any())
            {
                s.SendMessageAsync(e.Channel, "hej nějakej problem tu je");
                return;
            }

            var node = lava.ConnectedNodes.Values.First();

            var ida = e.Message.Author.Id;
            var member = e.Guild.GetMemberAsync(ida);

            if (member.Result.VoiceState == null)
            {
                s.SendMessageAsync(e.Channel, "Toban ma někde problem");
                return;
            }

            var channel = member.Result.VoiceState.Channel;

            if (channel.Type != ChannelType.Voice)
            {
                s.SendMessageAsync(e.Channel, "vadnej voice");
                return;
            }

            voicechannel = e.Message.Channel;
            await node.ConnectAsync(channel);
            
            play(e, s, lavalink);
        }

        static DiscordEmbed musicembed(string title, DSharpPlus.Lavalink.LavalinkTrack track)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.WithColor(DiscordColor.Red);
            embed.WithTitle(title);
            embed.WithThumbnail("https://img.youtube.com/vi/" + track.Uri.ToString().Replace("https://www.youtube.com/watch?v=", "") + "/0.jpg", 100);
            embed.WithDescription(track.Title);

            return embed;
        }

        private static void Pingtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            pingtimer.Stop();
            sendping(mainds, ev, mainmessage);
            ping = true;
        }

        static void sendping(DiscordClient discord, DSharpPlus.EventArgs.MessageCreateEventArgs e, string message)
        {
            if (discord == null)
                return;

            discord.SendMessageAsync(e.Channel, message);
        }

        private static void Rowtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            rowtimer.Stop();
            row = true;
        }

        static string UploadImage(Bitmap image)
        {
            byte[] imageArray = ImageToByte(image);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            var client = new RestClient("https://api.imgur.com/3/image");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Client-ID e8c4657b855423a");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("image",base64ImageRepresentation);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            var x = response.Content.Split(',');
            string z = x[27].Remove(0,8);
            z = z.Remove(z.Length - 2, 2);
            z = z.Replace(@"\","");
            return z;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
      
    }
}

