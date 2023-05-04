using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
namespace sfm
{
    public enum ViewMode
    {
        Files,
        Directories
    }
    public class PATH
    {
        public List<string> paths;
        public PATH()
        {
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                paths = new List<string>() { "C:\\Windows\\System32\\" };
            }
            else
            {
                paths = new List<string>() { "/bin/", "/sbin/", "/usr/bin/", "/usr/sbin/", "/usr/local/bin/", "/usr/local/sbin/" };
            }
        }
        public void StartFromPATH(string program)
        {
            foreach(string path in paths)
            {
                if (File.Exists($"{path}{program.Split()[0]}"))
                {
                    Process.Start($"{path}{program}");
                    return;
                }
                else if(File.Exists($"{path}{program.Split()[0]}.exe"))
                {
                    Process.Start($"{path}{program}.exe");
                    return;
                }
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            
            char fstp = '/'; // Set to '/' if you use Linux
            string currentFile = "NONE";
            ViewMode viewMode = ViewMode.Files;
            Environment.CurrentDirectory = $"{fstp}";
            PATH path = new PATH();
            while(true)
            {
                Console.WriteLine($"DIR: {Environment.CurrentDirectory}    FILE: {currentFile}    View Mode: {viewMode}");
                string[] files;
                if (viewMode == ViewMode.Files)
                {
                    files = Directory.GetFiles(Environment.CurrentDirectory);
                }
                else
                {
                    files = Directory.GetDirectories(Environment.CurrentDirectory);
                }
                for (int i = 0; i < 26; i++)
                {
                    try
                    {
                        if (File.Exists(files[i]))
                        {
                            Console.WriteLine($"[{i}]  :  {new FileInfo(files[i]).Name}  :  FILE");
                        }
                        else
                        {
                            Console.WriteLine($"[{i}]  :  {new DirectoryInfo(files[i]).Name}  :  DIR");
                        }
                    }
                    catch { }
                }
                Console.WriteLine($"[RE] Read file    [RM] Remove file    [MV] Move file\n[MF] Make file    [MD] Make dir       [EX] Execute file\n[SF] Switch to file view    [SD] Switch to dir view    [HE] Get help");
                string input = Console.ReadLine();
                int selectedFile;
                bool isInputInt = int.TryParse(input, out selectedFile);
                if (isInputInt)
                {
                    if (File.Exists(files[selectedFile]))
                    {
                        currentFile = new FileInfo(files[selectedFile]).Name;
                    }
                    else if(Directory.Exists(files[selectedFile]))
                    {
                        Environment.CurrentDirectory = files[selectedFile];
                    }
                    Console.Clear();
                }
                else
                {
                    switch (input.Split()[0])
                    {
                        case "RE":
                            try
                            {
                                if (File.Exists(currentFile))
                                {
                                    Console.Clear();
                                    Console.WriteLine(File.ReadAllText(currentFile));
                                    Console.ReadKey();
                                    Console.Clear();
                                }
                            } catch { }
                            break;
                        case "RM":
                            if (File.Exists(currentFile))
                            {
                                File.Delete(currentFile);
                            }
                            Console.Clear();
                            break;
                        case "MV":
                            if (File.Exists(currentFile))
                            {
                                File.Move(currentFile, $"{input.Split()[1]}{new FileInfo(currentFile).Name}");
                                Console.WriteLine("Moved");
                            }
                            Console.Clear();
                            break;
                        case "MF":
                            if (!File.Exists(input.Split()[1]))
                            {
                                File.Create(input.Split()[1]).Close();
                            }
                            Console.Clear();
                            break;
                        case "MD":
                            if (!Directory.Exists(input.Split()[1]))
                            {
                                Directory.CreateDirectory(input.Split()[1]);
                            }
                            Console.Clear();
                            break;
                        case ":cd":
                            if (Directory.Exists(input.Split()[1]))
                            {
                                Environment.CurrentDirectory = input.Split()[1];
                            }
                            Console.Clear();
                            break;
                        case ":pex":
                            Console.Clear();
                            path.StartFromPATH(input.Replace(":pex ", ""));
                            break;
                        case ":none":
                            Console.Clear();
                            currentFile = "NONE";
                            break;
                        case "SD":
                            viewMode = ViewMode.Directories;
                            Console.Clear();
                            break;
                        case "SF":
                            viewMode = ViewMode.Files;
                            Console.Clear();
                            break;
                        case "EX":
                            Console.Clear();
                            Process.Start(currentFile);
                            break;
                        default:
                            Console.Clear();
                            break;
                        case "HE":
                            Console.Clear();
                            Console.WriteLine($"DIR: {Environment.CurrentDirectory}    FILE: {currentFile}    View Mode: {viewMode}");
                            Console.WriteLine("Here is help:\n[RE] - Reads current file's content\n[RM] - Removes current file\n[MV] (directory) - Moves file into directory");
                            Console.WriteLine("[MF] (file name) - Creates new file\n[MD] (dir name) - Creates new dir\n[SD] - Switches view mode to directories");
                            Console.WriteLine("[SF] - Switches view mode to files\n[EX] - Executes current file");
                            Console.WriteLine(":cd (path) - Changes path\n:none - Sets current file to NONE\n:pex (program) - Executes program from PATH\n:finf - Gets info about current file");
                            Console.WriteLine(":sfmsc - Get info about system and SFM\n:q - Quit");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        case ":finf":
                            if (File.Exists(currentFile))
                            {
                                Console.Clear();
                                Console.WriteLine($"DIR: {Environment.CurrentDirectory}    FILE: {currentFile}    View Mode: {viewMode}");
                                FileInfo info = new FileInfo(currentFile);
                                Console.WriteLine($"Information about file {info.Name}:\nFull path: {info.FullName}\nCreation date: {info.CreationTime}\nExtension: {info.Extension}\nDirectory of file: {info.Directory}\nLast write time: {info.LastWriteTimeUtc}");
                                Console.ReadKey();
                                Console.Clear();
                            }
                            break;
                        case ":q":
                            return;
                        case ":sfmsc":
                            Console.Clear();
                            Console.WriteLine($"DIR: {Environment.CurrentDirectory}    FILE: {currentFile}    View Mode: {viewMode}");
                            Console.WriteLine("___________");
                            Console.WriteLine($"|M       M|    SFM: 23.05");
                            Console.WriteLine($"|M  ___  M|    OSV: {Environment.OSVersion}");
                            Console.WriteLine($"|M  |    M|    MEM: {MathF.Round((float)System.Diagnostics.Process.GetCurrentProcess().PagedMemorySize/1024f/1024f, 2)}Mb");
                            Console.WriteLine($"|M  |__  M|    UST: {System.Diagnostics.Process.GetCurrentProcess().Threads.Count} threads");
                            Console.WriteLine($"|M  |    M|    SYD: {Environment.SystemDirectory}");
                            Console.WriteLine($"|M  |    M|    FSI: {Environment.GetLogicalDrives().Length} logical disk(s)");
                            Console.WriteLine($"|M_______M|");
                            Console.WriteLine($"|MMMMMMMMM|");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                    }
                }
            }
        }
    }
}