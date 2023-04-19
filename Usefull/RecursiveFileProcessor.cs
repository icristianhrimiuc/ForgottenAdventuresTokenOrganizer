using System;
using System.IO;

namespace ForgottenAdventuresTokenOrganizer.Usefull
{
    public class RecursiveFileProcessor
    {
        private Action<string> ProcessFile;

        public RecursiveFileProcessor(Action<string> processFile)
        {
            ProcessFile += processFile;
        }

        public void Process(string[] directories)
        {
            foreach (string path in directories)
            {
                if (File.Exists(path))
                {
                    // This path is a file
                    ProcessFile(path);
                }
                else if (Directory.Exists(path))
                {
                    // This path is a directory
                    ProcessDirectory(path);
                }
                else
                {
                    throw new Exception($"{path} is not a valid file or directory.");
                }
            }
        }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
    }
}
