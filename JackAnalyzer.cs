using System;
using System.IO;

namespace JackAnalyzer
{
    /* Accepts a .jack file and compiles it into an .xml file */
    internal class JackAnalyzer
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            bool isDirectory = File.GetAttributes(filePath).HasFlag(FileAttributes.Directory);

            string[] filesInDir = { filePath };

            if (isDirectory)
            {
                filesInDir = Directory.GetFiles(filePath, "*.jack");
            }

            foreach (string fileName in filesInDir)
            {
                JackTokenizer tokenizer = new JackTokenizer(fileName);
                CompilationEngine engine = new CompilationEngine(tokenizer,  fileName.Replace(".jack", ".xml"));
                engine.CompileClass();
                engine.ShutDown();
            }
        }
    }
}
