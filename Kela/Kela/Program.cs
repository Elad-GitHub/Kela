using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kela
{
    partial class Program
    {
        static void Main(string[] args)
        {
            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            string path = $"{projectDirectory}\\path_to_scan\\";

            Pattern[] patternsInput = new[]
            {
                new Pattern() { PatternType = PatternType.Name, Value = "doc1.txt" },
                new Pattern() { PatternType = PatternType.Name, Value = "doc3.txt" },
                new Pattern() { PatternType = PatternType.Extension, Value = "log" }
            };

            Scanner(path, patternsInput);

            Console.WriteLine("Enter configuration file path");
            
            string path2 = Console.ReadLine();

            string[] lines = System.IO.File.ReadAllLines(path);
        }

        public static string Scanner(string directoryPath, Pattern[] patterns)
        {
            string output = string.Empty;
            
            foreach (string file in GetFiles(directoryPath))
            {
                FileInfo fileInfo = new FileInfo(file);
                output = PatternChecker(patterns, output, fileInfo);
            }

            return output;
        }

        private static string PatternChecker(Pattern[] patterns, string output, FileInfo fileInfo)
        {
            if (patterns.Any(x => (x.PatternType == PatternType.Name && x.Value.Equals(fileInfo.Name)) || 
                                    (x.PatternType == PatternType.Extension && x.Value.Equals(fileInfo.Extension) ||
                                    (x.PatternType == PatternType.Size && x.Value.Equals(fileInfo.Length)) )))
            {
                var fullNameParts = fileInfo.FullName.Split('\\');
                var relativePath = $"/{fullNameParts[fullNameParts.Length - 2]}";
                output += relativePath + ',' + fileInfo.Name.Split('.')[0] + ',' + fileInfo.Extension + ',' + fileInfo.Length + ',';
            }

            return output;
        }

        static IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
    }
}
