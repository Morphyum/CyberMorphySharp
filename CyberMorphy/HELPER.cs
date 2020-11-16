using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberMorphy
{
    class HELPER
    {
        public static bool newViewer(String channel, String viewer)
        {
            String filePath = "settings/" + channel + "/viewers.txt";
            (new FileInfo(filePath)).Directory.Create();
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(viewer);
            }
            return true;
        }

        public static List<string> readViewer(String channel)
        {
            List<string> viewer = new List<string>();
            String filePath = "settings/" + channel + "/viewers.txt";
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        viewer.Add(reader.ReadLine());
                    }
                }
                return viewer;
            }
            return new List<string>();
        }

        public static bool saveAllSettings(String channel, CyberMorphy bot)
        {
            string save = JsonConvert.SerializeObject(bot);
            String filePath = "settings/" + channel + "/settings.json";
            (new FileInfo(filePath)).Directory.Create();
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(save);
            }
            return true;
        }

        public static CyberMorphy loadSettings(string channel)
        {
            Console.WriteLine(channel);
            string filePath = "settings/" + channel + "/settings.json";
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {

                    return JsonConvert.DeserializeObject<CyberMorphy>(reader.ReadToEnd());
                }
            }
            return new CyberMorphy();
        }

        public static bool saveModCommand(List<Command> modcommands, String channel)
        {
            string save = JsonConvert.SerializeObject(modcommands);
            String filePath = "settings/" + channel + "/modcommands.json";
            (new FileInfo(filePath)).Directory.Create();
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(save);
            }
            return false;

        }

        public static bool saveCommand(List<Command> commands, String channel)
        {
            string save = JsonConvert.SerializeObject(commands);
            String filePath = "settings/" + channel + "/commands.json";
            (new FileInfo(filePath)).Directory.Create();
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(save);
            }
            return false;

        }
        public static List<Command> readModCommands(String channel)
        {
            String filePath = "settings/" + channel + "/modcommands.json";
            List<Command> modcommands = new List<Command>();
            if (File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<Command>>(json);
                }
            }
            return new List<Command>();
        }

        public static List<Command> readCommands(String channel)
        {
            String filePath = "settings/" + channel + "/commands.json";
            if (File.Exists(filePath))
            {
                List<Command> modcommands = new List<Command>();
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<Command>>(json);
                }
            }
            return new List<Command>();
        }

        public static bool deleteModCommand(String command, String channel)
        {
            List<Command> modcommands = readModCommands(channel);
            bool found = false;
            for (int i = 0; i < modcommands.Count(); i++)
            {
                if (modcommands[i].getHead().ToLower().Equals(command))
                {
                    modcommands.RemoveAt(i);
                    found = true;
                    break;
                }
            }
            if (found)
            {
                saveModCommand(modcommands, channel);
            }
            return true;

        }

        public static bool deleteCommand(String command, String channel)
        {
            List<Command> commands = readCommands(channel);
            bool found = false;
            for (int i = 0; i < commands.Count(); i++)
            {
                if (commands[i].getHead().ToLower().Equals(command))
                {
                    commands.RemoveAt(i);
                    found = true;
                    break;
                }
            }
            if (found)
            {
                string save = JsonConvert.SerializeObject(commands);
                String filePath = "settings/" + channel + "/commands.txt";
                (new FileInfo(filePath)).Directory.Create();
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.WriteLine(save);
                }
            }
            return true;

        }
    }
}
