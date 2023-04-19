using ForgottenAdventuresTokenOrganizer.Usefull;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ForgottenAdventuresTokenOrganizer.FolderStructure
{
    internal class FolderStructureBuilder : JsonFileHandler
    {
        private readonly ILogger _logger;
        private Dictionary<string, List<string>> _folderStructure;

        public FolderStructureBuilder(ILogger logger)
        {
            _logger = logger;
            _folderStructure = new Dictionary<string, List<string>>();
        }


        public Dictionary<string, List<string>> GetCommonersFolderStructure(string rootPath, bool createDirectories = true)
        {
            return GetFolderStructure(rootPath, CommonersJsonFilePath, createDirectories);
        }

        public Dictionary<string, List<string>> GetCreaturesFolderStructure(string rootPath, bool createDirectories = true)
        {
            return GetFolderStructure(rootPath, CreaturesJsonFilePath, createDirectories);
        }

        public Dictionary<string, List<string>> GetHeroesFolderStructure(string rootPath, bool createDirectories = true)
        {
            return GetFolderStructure(rootPath, HeroesJsonFilePath, createDirectories);
        }

        public Dictionary<string, List<string>> GetSpiritCommonersFolderStructure(string rootPath, bool createDirectories = true)
        {
            return GetFolderStructure(rootPath, SpiritCommonersJsonFilePath, createDirectories);
        }

        public Dictionary<string, List<string>> GetSpiritCreaturesFolderStructure(string rootPath, bool createDirectories = true)
        {
            return GetFolderStructure(rootPath, SpiritCreaturesJsonFilePath, createDirectories);
        }

        public Dictionary<string, List<string>> GetSpiritHeroesFolderStructure(string rootPath, bool createDirectories = true)
        {
            return GetFolderStructure(rootPath, SpiritHeroesJsonFilePath, createDirectories);
        }

        public Dictionary<string, List<string>> GetTokensFolderStructure(string rootPath, bool createDirectories = true)
        {
            return GetFolderStructure(rootPath, TokensJsonFilePath, createDirectories);
        }

        public Dictionary<string, List<string>> GetFolderStructure(string rootPath, string jsonFolderStructureFileName, bool createDirectories = true)
        {
            _folderStructure.Clear();
            if (createDirectories) Directory.CreateDirectory(rootPath);
            var jsonFolderStructure = File.ReadAllText(Path.Combine(JsonPath, jsonFolderStructureFileName));
            var jsonList = JsonSerializer.Deserialize<List<FolderStructure>>(jsonFolderStructure) ?? new List<FolderStructure>();
            foreach (var directory in jsonList)
            {
                ProcessDirectory(rootPath, directory, createDirectories);
            }

            return _folderStructure;
        }

        private void ProcessDirectory(string rootPath, FolderStructure directory, bool createDirectories)
        {
            var specificDirectory = Path.Combine(rootPath, directory.DirectoryName.ToTitleCase());
            if (createDirectories) Directory.CreateDirectory(specificDirectory);
            if (directory.SearchPatterns != null)
            {
                foreach (var pattern in directory.SearchPatterns)
                {
                    if (_folderStructure.ContainsKey(pattern))
                    {
                        _logger.Information($"{GetType().Name} - Pattern \"{pattern}\" already exists in dictionary! Adding multiple destinations for it");
                        _folderStructure[pattern].Add(specificDirectory);
                    }
                    else
                    {
                        _folderStructure.Add(pattern, new List<string> { specificDirectory });
                    }
                }
            }
            if (directory.SubDirectories != null)
            {
                foreach (var subDirectory in directory.SubDirectories)
                {
                    ProcessDirectory(specificDirectory, subDirectory, createDirectories);
                }
            }
        }
    }
}
