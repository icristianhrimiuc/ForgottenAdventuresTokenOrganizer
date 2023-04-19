using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ForgottenAdventuresTokenOrganizer.FolderStructure
{
    internal class FolderStructureWritter : JsonFileHandler
    {
        public void RewriteTokens()
        {
            var tokens = new List<FolderStructure>
            {
                new FolderStructure()
                {
                    DirectoryName = "_Unknown",
                    DeletePatterns = new string[]{ "[CR" },
                },
                new FolderStructure()
                {
                    DirectoryName = "Commoners",
                    SubDirectories = JsonSerializer.Deserialize<List<FolderStructure>>(File.ReadAllText(Path.Combine(JsonPath, "commoners_folder_structure.json"))) ?? new List<FolderStructure>().OrderBy(x => x.DirectoryName).ToList()
                },
                new FolderStructure()
                {
                    DirectoryName = "Creatures",
                    SubDirectories = JsonSerializer.Deserialize<List<FolderStructure>>(File.ReadAllText(Path.Combine(JsonPath, "creatures_folder_structure.json"))) ?? new List<FolderStructure>().OrderBy(x => x.DirectoryName).ToList()
                },
                new FolderStructure()
                {
                    DirectoryName = "Heroes",
                    SubDirectories = JsonSerializer.Deserialize<List<FolderStructure>>(File.ReadAllText(Path.Combine(JsonPath, "heroes_folder_structure.json"))) ?? new List<FolderStructure>().OrderBy(x => x.DirectoryName).ToList()
                },
                new FolderStructure()
                {
                    DirectoryName = "Spirit Commoners",
                    SubDirectories = JsonSerializer.Deserialize<List<FolderStructure>>(File.ReadAllText(Path.Combine(JsonPath, "spirit_commoners_folder_structure.json"))) ?? new List<FolderStructure>().OrderBy(x => x.DirectoryName).ToList()
                },
                new FolderStructure()
                {
                    DirectoryName = "Spirit Creatures",
                    SubDirectories = JsonSerializer.Deserialize<List<FolderStructure>>(File.ReadAllText(Path.Combine(JsonPath, "spirit_creatures_folder_structure.json"))) ?? new List<FolderStructure>().OrderBy(x => x.DirectoryName).ToList()
                },
                new FolderStructure()
                {
                    DirectoryName = "Spirit Heroes",
                    SubDirectories = JsonSerializer.Deserialize<List<FolderStructure>>(File.ReadAllText(Path.Combine(JsonPath, "spirit_heroes_folder_structure.json"))) ?? new List<FolderStructure>().OrderBy(x => x.DirectoryName).ToList()
                }
            };
            tokens = tokens.OrderBy(x => x.DirectoryName).ToList();
            File.WriteAllText(Path.Combine(JsonPath, "tokens_folder_structure.json"), JsonSerializer.Serialize(tokens));
        }

        public void RewriteSpirits()
        {
            RewriteSpiritCommoners();
            RewriteSpiritCreatures();
            RewriteSpiritHeroes();
        }

        public void RewriteSpiritCommoners()
        {
            RewriteSpirits("commoners_folder_structure.json", "spirit_commoners_folder_structure.json");
        }

        public void RewriteSpiritCreatures()
        {
            RewriteSpirits("creatures_folder_structure.json", "spirit_creatures_folder_structure.json");
        }

        public void RewriteSpiritHeroes()
        {
            RewriteSpirits("heroes_folder_structure.json", "spirit_heroes_folder_structure.json");
        }

        public void RewriteSpirits(string sourceJsonFolderStructureFileName, string destinationJsonFolderStructureFileName)
        {
            var json = File.ReadAllText(Path.Combine(JsonPath, sourceJsonFolderStructureFileName));
            var oldList = JsonSerializer.Deserialize<List<FolderStructure>>(json) ?? new List<FolderStructure>();
            var newList = new List<FolderStructure>();
            foreach (var directory in oldList)
            {
                newList.Add(MakeSpirits(directory));
            }
            newList = newList.OrderBy(x => x.DirectoryName).ToList();
            File.WriteAllText(Path.Combine(JsonPath, destinationJsonFolderStructureFileName), JsonSerializer.Serialize(newList));
        }

        private FolderStructure MakeSpirits(FolderStructure oldFS)
        {
            var newFS = new FolderStructure();
            newFS.DirectoryName = oldFS.DirectoryName;
            if (oldFS.SearchPatterns != null)
            {
                newFS.SearchPatterns = FixMistakes(oldFS.SearchPatterns.Select(sp => RemoveCreatureType(sp) + " spirit").Distinct().ToList()).ToArray();
            }
            newFS.DeletePatterns = oldFS.DeletePatterns;
            if (oldFS.SubDirectories != null)
            {
                newFS.SubDirectories = oldFS.SubDirectories.Select(sd => MakeSpirits(sd)).ToList();
            }

            return newFS;
        }

        private string RemoveCreatureType(string searchPattern)
        {
            var creatureTypes = new List<string> {
                "aberration",
                "beast",
                "celestial",
                "construct",
                "dragon",
                "elemental",
                "fey",
                "fiend",
                "giant",
                "humanoid",
                "merfolk",
                "monstrosity",
                "ooze",
                "plant",
                "undead"
            };
            foreach (var creatureType in creatureTypes)
            {
                var words = searchPattern.Split();
                if (words[words.Length - 1] == creatureType)
                {
                    words = words.Where((w, i) => i < words.Length - 1).ToArray();
                    return string.Join(" ", words);
                }
            }

            return searchPattern;
        }

        private List<string> FixMistakes(List<string> searchPatterns)
        {
            var mistakes = new string[]
            {
                "dwarf fighter spirit",
                "dwarf ranger spirit",
                "elf warlock spirit"
            };
            var fixes = new string[]
            {
                "dwarf fighter spirits",
                "dwarf ranger spirits",
                "elf warlock spirits"
            };
            foreach (var searchPattern in searchPatterns)
            {
                for (int i = 0; i < mistakes.Length; i++)
                {
                    if (searchPattern == mistakes[i])
                    {
                        searchPatterns.Add(fixes[i]);
                        return searchPatterns;
                    }
                }
            }

            return searchPatterns;
        }

        public void RewriteAllOldFSToNewFS(string rootPath)
        {
            RewriteOldFSToNewFS(Path.Combine(rootPath, "Commoners"), CommonersJsonFilePath, CommonersJsonFilePath);
            RewriteOldFSToNewFS(Path.Combine(rootPath, "Creatures"), CreaturesJsonFilePath, CreaturesJsonFilePath);
            RewriteOldFSToNewFS(Path.Combine(rootPath, "Heroes"), HeroesJsonFilePath, HeroesJsonFilePath);
            RewriteOldFSToNewFS(Path.Combine(rootPath, "Spirit Commoners"), SpiritCommonersJsonFilePath, SpiritCommonersJsonFilePath);
            RewriteOldFSToNewFS(Path.Combine(rootPath, "Spirit Creatures"), SpiritCreaturesJsonFilePath, SpiritCreaturesJsonFilePath);
            RewriteOldFSToNewFS(Path.Combine(rootPath, "Spirit Heroes"), SpiritHeroesJsonFilePath, SpiritHeroesJsonFilePath);
        }

        public void RewriteOldFSToNewFS(string rootPath, string sourceJsonFolderStructureFileName, string destinationJsonFolderStructureFileName)
        {
            var json = File.ReadAllText(Path.Combine(JsonPath, sourceJsonFolderStructureFileName));
            var oldList = JsonSerializer.Deserialize<List<FolderStructure>>(json) ?? new List<FolderStructure>();
            var newList = new List<FolderStructureNew>();
            foreach (var oldFS in oldList)
            {
                newList.Add(MapOldFSToNewFS(rootPath, oldFS));
            }
            newList = newList.OrderBy(x => x.DirectoryName).ToList();
            File.WriteAllText(Path.Combine(JsonPath, destinationJsonFolderStructureFileName), JsonSerializer.Serialize(newList));
        }

        private FolderStructureNew MapOldFSToNewFS(string root, FolderStructure oldFS)
        {
            var specificDirectory = Path.Combine(root, oldFS.DirectoryName);
            var nrOfFiles = Directory.Exists(specificDirectory) ? Directory.GetFiles(specificDirectory).Count() : 0 ;
            return new FolderStructureNew
            {
                DirectoryName = oldFS.DirectoryName,
                SearchPatterns = oldFS.SearchPatterns,
                DeletePatterns = oldFS.DeletePatterns,
                ExpectedFinalNumberOfFiles = nrOfFiles,
                SubDirectories = oldFS.SubDirectories == null ? null : oldFS.SubDirectories.OrderBy(x => x.DirectoryName).Select(subDirectory => MapOldFSToNewFS(specificDirectory, subDirectory)).ToList()
            };
        }
    }
}
