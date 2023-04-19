using ForgottenAdventuresTokenOrganizer.Usefull;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ForgottenAdventuresTokenOrganizer.FolderStructure
{
    internal class DeletePatternsBuilder : JsonFileHandler
    {
        private readonly ILogger _logger;
        private Dictionary<string, List<string>> _deletePatterns;

        public DeletePatternsBuilder(ILogger logger)
        {
            _logger = logger;
            _deletePatterns = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, List<string>> GetCommonersDeletePatterns(string rootPath)
        {
            return GetDeletePatterns(rootPath, CommonersJsonFilePath);
        }

        public Dictionary<string, List<string>> GetCreaturesDeletePatterns(string rootPath)
        {
            return GetDeletePatterns(rootPath, CreaturesJsonFilePath);
        }

        public Dictionary<string, List<string>> GetHeroesDeletePatterns(string rootPath)
        {
            return GetDeletePatterns(rootPath, HeroesJsonFilePath);
        }

        public Dictionary<string, List<string>> GetSpiritCommonersDeletePatterns(string rootPath)
        {
            return GetDeletePatterns(rootPath, SpiritCommonersJsonFilePath);
        }

        public Dictionary<string, List<string>> GetSpiritCreaturesDeletePatterns(string rootPath)
        {
            return GetDeletePatterns(rootPath, SpiritCreaturesJsonFilePath);
        }

        public Dictionary<string, List<string>> GetSpiritHeroesDeletePatterns(string rootPath)
        {
            return GetDeletePatterns(rootPath, SpiritHeroesJsonFilePath);
        }

        public Dictionary<string, List<string>> GetTokensDeletePatterns(string rootPath)
        {
            return GetDeletePatterns(rootPath, TokensJsonFilePath);
        }

        public Dictionary<string, List<string>> GetDeletePatterns(string rootPath, string jsonFolderStructureFileName)
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
            if (directory.DeletePatterns != null)
            {
                if (_deletePatterns.ContainsKey(specificDirectory))
                {
                    _logger.Warning($"{GetType().Name} - Directory \"{specificDirectory}\" already exists in dictionary! Adding multiple delete patterns for it");
                    _deletePatterns[specificDirectory].AddRange(directory.DeletePatterns);
                }
                else
                {
                    _deletePatterns.Add(specificDirectory, directory.DeletePatterns.ToList());
                }
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
