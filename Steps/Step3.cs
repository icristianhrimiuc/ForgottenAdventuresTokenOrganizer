using ForgottenAdventuresTokenOrganizer.FolderStructure;
using ForgottenAdventuresTokenOrganizer.Usefull;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;

namespace ForgottenAdventuresTokenOrganizer.Steps
{
    internal class Step3
    {
        private readonly ILogger _logger;
        private string _sourcePath = string.Empty;
        private string _destinationPath = string.Empty;
        private Dictionary<string, List<string>> _folderStructureDictionary;

        public Step3(ILogger logger)
        {
            _logger = logger;
            _folderStructureDictionary = new Dictionary<string, List<string>>();
        }

        public void CopyChaoticFilesToOrganizedFolders(string sourcePath, string destinationPath)
        {
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
            _folderStructureDictionary = new FolderStructureBuilder(_logger).GetTokensFolderStructure(destinationPath);
            var fileOrganizer = new RecursiveFileProcessor(OrganizeFile);
            fileOrganizer.Process(new string[] { _sourcePath });
        }

        private void OrganizeFile(string oldFilePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(oldFilePath).ToLower().Replace(' ', '_');
            bool identified = false;
            var orderedKeysAsTuple = _folderStructureDictionary.Keys.Select(k => (key: k, patternWords: k.Split())).OrderByDescending(t => t.patternWords.Length).ThenByDescending(t => t.key);
            foreach (var tuple in orderedKeysAsTuple)
            {
                var fileWords = fileName.Split(new char[] { '_' });
                if (!identified && IsMatch(tuple.patternWords, fileWords))
                {
                    _logger.Information($"{GetType().Name} - File {Path.GetRelativePath(_sourcePath, oldFilePath)} is match for \"{tuple.key}\"");
                    foreach (var newDirectory in _folderStructureDictionary[tuple.key])
                    {
                        if (!Directory.Exists(newDirectory)) Directory.CreateDirectory(newDirectory);
                        var newFilePath = Path.Combine(newDirectory, Path.GetFileName(oldFilePath));
                        _logger.Information($"\tCopy file to {Path.GetRelativePath(_destinationPath, newDirectory)}");
                        CopyFileSafely(oldFilePath, newFilePath, 1);
                    }
                    identified = true;
                }
            }
            if (!identified)
            {
                _logger.Warning($"{GetType().Name} - File {Path.GetRelativePath(_sourcePath, oldFilePath)} was not matched!");
                var unknownDirectory = Path.Combine(_destinationPath, "_Unknown");
                Directory.CreateDirectory(unknownDirectory);
                var newFilePath = Path.Combine(unknownDirectory, Path.GetFileName(oldFilePath)); 
                _logger.Information($"\tCopy file to {Path.GetRelativePath(_destinationPath, unknownDirectory)}");
                CopyFileSafely(oldFilePath, newFilePath, 1);
            }
        }

        private bool IsMatch(string[] patternWords, string[] fileWords)
        {
            var wordsMatched = 0;
            var lastMatch = 0;
            if (patternWords[0] == fileWords[0])
            {
                wordsMatched++;
                for (int i = 1; i < patternWords.Length; i++)
                {
                    for (int j = lastMatch + 1; j < fileWords.Length; j++)
                    {
                        if (fileWords[j] == patternWords[i])
                        {
                            lastMatch = j;
                            wordsMatched++;
                            break;
                        }
                    }
                }
            }

            return wordsMatched == patternWords.Length;
        }

        private void CopyFileSafely(string oldFilePath, string newFilePath, int nrOfCalls)
        {
            if (File.Exists(newFilePath))
            {
                var oldFileName = Path.GetFileName(oldFilePath);
                if (oldFileName.Contains("Duplc")) oldFileName = oldFileName.Substring(7, oldFileName.Length - 7);
                var duplicateFilePath = Path.Combine(Directory.GetParent(newFilePath).FullName, $"Duplc{nrOfCalls++}_" + oldFileName);
                if (new FileInfo(oldFilePath).LastWriteTime < new FileInfo(newFilePath).LastWriteTime)
                {
                    CopyFileSafely(oldFilePath, duplicateFilePath, nrOfCalls);
                }
                else
                {
                    CopyFileSafely(newFilePath, duplicateFilePath, nrOfCalls);
                    File.Delete(newFilePath);
                    File.Copy(oldFilePath, newFilePath);
                }
            }
            else
            {
                File.Copy(oldFilePath, newFilePath);
            }
        }
    }
}
