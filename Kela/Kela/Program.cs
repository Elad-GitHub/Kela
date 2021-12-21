using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kela
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\Elad Cohen\Desktop\Kela\Kela\Kela\path_to_scan\";

            Pattern[] patternsInput = new[]
            {
                new Pattern() { PatternType = PatternType.Name, Value = "doc1.txt" },
                new Pattern() { PatternType = PatternType.Name, Value = "doc3.txt" },
                new Pattern() { PatternType = PatternType.Extension, Value = "log" }
            };

            Scanner(path, patternsInput);
        }


        public class Pattern
        {
            public PatternType PatternType{ get; set; }
            public string Value { get; set; }
        }

        public enum PatternType
        {
            Name,
            Extension
        }

        public static string Scanner(string directoryPath, Pattern[] patterns)
        {
            string output = string.Empty;
            
            foreach (string file in GetFiles(directoryPath))
            {
                FileInfo fileInfo = new FileInfo(file);

                if (patterns.Any(x => (x.PatternType == PatternType.Name && x.Value.Equals(fileInfo.Name)) || (x.PatternType == PatternType.Extension && x.Value.Equals(fileInfo.Extension))))
                {
                    var fullNameParts = fileInfo.FullName.Split('\\');
                    var relativePath = $"/{fullNameParts[fullNameParts.Length - 1]}";
                    output += relativePath + ',' + fileInfo.Name.Split('.')[0] + ',' + fileInfo.Extension + ',';
                }
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
