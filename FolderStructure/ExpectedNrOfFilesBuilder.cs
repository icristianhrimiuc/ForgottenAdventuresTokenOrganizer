using ForgottenAdventuresTokenOrganizer.Usefull;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ForgottenAdventuresTokenOrganizer.FolderStructure
{
    internal class ExpectedNrOfFilesBuilder : JsonFileHandler
    {
        private readonly ILogger _logger;
        private Dictionary<string, int> _deletePatterns;

        public ExpectedNrOfFilesBuilder(ILogger logger)
        {
            _logger = logger;
            _deletePatterns = new Dictionary<string, int>();
        }

        public Dictionary<string, int> GetCommonersExpectedNrOfFiles(string rootPath)
        {
            return GetExpectedNrOfFiles(rootPath, CommonersJsonFilePath);
        }

        public Dictionary<string, int> GetCreaturesExpectedNrOfFiles(string rootPath)
        {
            return GetExpectedNrOfFiles(rootPath, CreaturesJsonFilePath);
        }

        public Dictionary<string, int> GetHeroesExpectedNrOfFiles(string rootPath)
        {
            return GetExpectedNrOfFiles(rootPath, HeroesJsonFilePath);
        }

        public Dictionary<string, int> GetSpiritCommonersExpectedNrOfFiles(string rootPath)
        {
            return GetExpectedNrOfFiles(rootPath, SpiritCommonersJsonFilePath);
        }

        public Dictionary<string, int> GetSpiritCreaturesExpectedNrOfFiles(string rootPath)
        {
            return GetExpectedNrOfFiles(rootPath, SpiritCreaturesJsonFilePath);
        }

        public Dictionary<string, int> GetSpiritHeroesExpectedNrOfFiles(string rootPath)
        {
            return GetExpectedNrOfFiles(rootPath, SpiritHeroesJsonFilePath);
        }

        public Dictionary<string, int> GetTokensExpectedNrOfFiles(string rootPath)
        {
            return GetExpectedNrOfFiles(rootPath, TokensJsonFilePath);
        }

        public Dictionary<string, int> GetExpectedNrOfFiles(string rootPath, string jsonFolderStructureFileName)
        {
            _deletePatterns.Clear();
            var jsonFolderStructure = File.ReadAllText(Path.Combine(JsonPath, jsonFolderStructureFileName));
            var jsonList = JsonSerializer.Deserialize<List<FolderStructure>>(jsonFolderStructure) ?? new List<FolderStructure>();
            foreach (var directory in jsonList)
            {
                ProcessDirectory(rootPath, directory);
            }

            return _deletePatterns;
        }

        private void ProcessDirectory(string rootPath, FolderStructure directory)
        {
            var specificDirectory = Path.Combine(rootPath, directory.DirectoryName.ToTitleCase());
            if (_deletePatterns.ContainsKey(specificDirectory))
            {
                _logger.Warning($"{GetType().Name} - Directory \"{specificDirectory}\" already exists in dictionary");
            }
            else
            {
                _deletePatterns.Add(specificDirectory, directory.ExpectedFinalNumberOfFiles);
            }
            if (directory.SubDirectories != null)
            {
                foreach (var subDirectory in directory.SubDirectories)
                {
                    ProcessDirectory(specificDirectory, subDirectory);
                }
            }
        }
    }
}
