using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace CyberMorphy
{
    class CyberMorphy
    {
        [JsonIgnore]
        TwitchClient client;
        [JsonProperty]
        String streamerName = "nonamesetyet";
        [JsonProperty]
        bool greetings = false;
        [JsonIgnore]
        List<string> viewers = new List<string>();
        [JsonProperty]
        String welcomeBack = "٩( ᐛ )و";
        [JsonProperty]
        String greeting = "٩( ᐛ )و";
        [JsonProperty]
        bool modonly = false;
        [JsonIgnore]
        List<Command> commands = new List<Command>();
        [JsonIgnore]
        List<Command> modcommands = new List<Command>();

        public CyberMorphy()
        {
        }
        public CyberMorphy(string channel)
        {
            Console.WriteLine(channel);
            CyberMorphy temp = HELPER.loadSettings(channel);
            this.streamerName = temp.streamerName;
            this.greetings = temp.greetings;
            this.welcomeBack = temp.welcomeBack;
            this.greeting = temp.greeting;
            this.modonly = temp.modonly;
            this.commands = HELPER.readCommands(channel);
            this.modcommands = HELPER.readModCommands(channel);
            this.viewers = HELPER.readViewer(channel);

            ConnectionCredentials credentials = new ConnectionCredentials(Secret.USERNAME, Secret.PASSWORD);
            client = new TwitchClient();
            client.Initialize(credentials, channel);
            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnBeingHosted += Client_OnBeingHosted;
            client.Connect();
        }

        private void Client_OnBeingHosted(object sender, OnBeingHostedArgs e)
        {
            client.SendMessage(e.BeingHostedNotification.Channel, ":O " + e.BeingHostedNotification.HostedByChannel + " is hosting us! Woop Woop");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            client.SendMessage(e.Channel, "Yay! I'm back, type !help to get to know me.");
            if (streamerName.Equals("nonamesetyet"))
            {
                streamerName = e.Channel;
            }
        }


        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (modonly == false || e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
            {
                if (((e.ChatMessage.Message.ToLower()).StartsWith("!setname")) && (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster))
                {
                    streamerName = e.ChatMessage.Message.Substring(9);
                    client.SendMessage(e.ChatMessage.Channel, "Ok i will call you " + streamerName + " from now on :)");
                    HELPER.saveAllSettings(e.ChatMessage.Channel, this);
                }
                else if ((e.ChatMessage.Message.ToLower()).Equals("!casaflip"))
                {
                    client.SendMessage(e.ChatMessage.Channel, "\u0028" + "\uFF89" + "\u0CA5" + "\u76CA" + "\u0CA5" + "\uFF09" + "\uFF89" + " " + "\u253B" + "\u2501" + "\u253B");

                }
                else if ((e.ChatMessage.Message.ToLower()).Contains("teh urn"))
                {
                    client.SendMessage(e.ChatMessage.Channel, "It's tEh URN!!");
                }
                else if ((e.ChatMessage.Message.ToLower()).Contains("potato"))
                {
                    client.SendMessage(e.ChatMessage.Channel, "POTATO! :3 <3");
                }
                else if (e.ChatMessage.Message.ToLower().StartsWith("!greet"))
                {
                    client.SendMessage(e.ChatMessage.Channel, handleGreeting(e));
                }
                else if (e.ChatMessage.Message.ToLower().Equals("!commands"))
                {

                    String commandlist = "";
                    for (int i = 0; i < commands.Count(); i++)
                    {
                        if(commandlist.Length > 400)
                        {
                            client.SendMessage(e.ChatMessage.Channel, commandlist);
                            commandlist = "";
                        }
                        commandlist += commands[i].getHead() + " | ";
                    }
                    client.SendMessage(e.ChatMessage.Channel, commandlist);

                }
                else if (e.ChatMessage.Message.ToLower().StartsWith("-!"))
                {
                    if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                    {
                        if (e.ChatMessage.Message.ToLower().StartsWith("--!"))
                        {
                            String modcommand = e.ChatMessage.Message.Replace("--", "");
                            for (int i = 0; i < commands.Count(); i++)
                            {
                                if (modcommands[i].getHead().Equals(modcommand))
                                {
                                    modcommands.RemoveAt(i);
                                    HELPER.deleteModCommand(modcommand, e.ChatMessage.Channel);
                                    client.SendMessage(e.ChatMessage.Channel, "Mod Command " + modcommand + " deleted");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            String command = e.ChatMessage.Message.Replace("-", "");
                            for (int i = 0; i < commands.Count(); i++)
                            {
                                if (commands[i].getHead().Equals(command))
                                {
                                    commands.RemoveAt(i);
                                    HELPER.deleteCommand(command, e.ChatMessage.Channel);
                                    client.SendMessage(e.ChatMessage.Channel, "Command " + command + " deleted");
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (e.ChatMessage.Message.ToLower().Contains("+!"))
                {
                    if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                    {
                        if (e.ChatMessage.Message.ToLower().StartsWith("++!"))
                        {
                            client.SendMessage(e.ChatMessage.Channel, "Mod Command start");
                            String[] commandparts = e.ChatMessage.Message.Split(' ');
                            String body = "";
                            for (int i = 1; i < commandparts.Length; i++)
                            {
                                body += commandparts[i] + " ";
                            }
                            if (checkModCommands(commandparts[0].Replace("++", "")).Equals(""))
                            {
                                string head = commandparts[0].Replace("++", "");
                                if (head != "!") {
                                    modcommands.Add(new Command(head, body));
                                    HELPER.saveModCommand(modcommands, e.ChatMessage.Channel);
                                    client.SendMessage(e.ChatMessage.Channel, "Mod Command " + commandparts[0].Replace("++", "") + " added");
                                } else {
                                    client.SendMessage(e.ChatMessage.Channel, "Please use no space between ! and the command trigger.");
                                }

                            }
                            else
                            {
                                client.SendMessage(e.ChatMessage.Channel, "Mod Command " + commandparts[0].Replace("++", "") + " is already on the Commandlist");
                            }
                        }
                        else
                        {
                            String[] commandparts = e.ChatMessage.Message.Split(' ');
                            String body = "";
                            for (int i = 1; i < commandparts.Length; i++)
                            {
                                body += commandparts[i] + " ";
                            }
                            if (checkCommands(commandparts[0].Replace("+", "")).Equals(""))
                            {
                                commands.Add(new Command(commandparts[0].Replace("+", ""), body));
                                HELPER.saveCommand(commands, e.ChatMessage.Channel);
                                client.SendMessage(e.ChatMessage.Channel, "Command " + commandparts[0].Replace("+", "") + " added");
                            }
                            else
                            {
                                client.SendMessage(e.ChatMessage.Channel, "Command " + commandparts[0].Replace("+", "") + " is already on the Commandlist");
                            }
                        }
                    }

                }
                else
                {
                    if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                    {
                        String modcommand = checkModCommands(e.ChatMessage.Message);
                        if (!modcommand.Equals(""))
                        {
                            client.SendMessage(e.ChatMessage.Channel, modcommand);
                        }
                        else
                        {
                            String command = checkCommands(e.ChatMessage.Message);
                            if (!command.Equals(""))
                            {
                                client.SendMessage(e.ChatMessage.Channel, command);
                            }
                        }
                    }
                    else
                    {
                        String command = checkCommands(e.ChatMessage.Message);
                        if (!command.Equals(""))
                        {
                            client.SendMessage(e.ChatMessage.Channel, command);
                        }
                    }
                }
            }
            System.Threading.Thread.Sleep(3000);
        }


        private String checkModCommands(String message)
        {
            for (int i = 0; i < modcommands.Count(); i++)
            {
                if (message.ToLower().Equals(modcommands[i].getHead()))
                {
                    return modcommands[i].getBody();
                }
            }
            return "";
        }

        private String checkCommands(String message)
        {
            for (int i = 0; i < commands.Count(); i++)
            {
                if (message.ToLower().Equals(commands[i].getHead()))
                {
                    return commands[i].getBody();
                }
            }
            return "";
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            if (e.Username.ToLower().Equals("morphyum"))
            {
                client.SendMessage(e.Channel, "My Creator is back, good time to praise him or make requests! Just ping @Morphyum");
            }
            else if (greetings)
            {
                if (viewers.Contains(e.Username))
                {
                    client.SendMessage(e.Channel, welcomeBack.Replace("(NICK)", e.Username));
                }
                else
                {
                    client.SendMessage(e.Channel, greeting.Replace("(NICK)", e.Username));
                    viewers.Add(e.Username);
                    HELPER.newViewer(e.Channel, e.Username);
                }
            }
            System.Threading.Thread.Sleep(3000);
        }

        private String handleGreeting(OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
            {
                if (e.ChatMessage.Message.ToLower().Equals("!greet on"))
                {
                    greetings = true;
                    HELPER.saveAllSettings(e.ChatMessage.Channel, this);
                    return "Greeting activated";
                }
                else if (e.ChatMessage.Message.ToLower().Equals("!greet off"))
                {
                    greetings = false;
                    HELPER.saveAllSettings(e.ChatMessage.Channel, this);
                    return "Greeting deactivated";
                }
                else if (e.ChatMessage.Message.ToLower().Contains("!greet set"))
                {
                    greeting = e.ChatMessage.Message.Substring(11);
                    HELPER.saveAllSettings(e.ChatMessage.Channel, this);
                    return "New greeting will be: " + greeting;

                }
                else if (e.ChatMessage.Message.ToLower().Contains("!greet welcome"))
                {
                    welcomeBack = e.ChatMessage.Message.Substring(15);
                    HELPER.saveAllSettings(e.ChatMessage.Channel, this);
                    return "New welcome back message will be: " + welcomeBack;
                }
                else
                {
                    return "Wrong Syntax please use !greet [ on/off/set [MESSAGE]/welcome [MESSAGE] ]";
                }
            }
            else
            {
                return "Only Mods can (de)activate or set greetings Kappa";
            }
        }
    }
}
