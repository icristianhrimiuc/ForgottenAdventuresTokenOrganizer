using ForgottenAdventuresTokenOrganizer.FolderStructure;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;

namespace ForgottenAdventuresTokenOrganizer.Steps
{
    internal class Step6
    {
        private readonly ILogger _logger;

        public Step6(ILogger logger)
        {
            _logger = logger;
        }

        public void DeleteFilesByPattern(string workingPath, bool actuallyRemove = true)
        {
            _logger.Information($"{GetType().Name} - Removing files by {nameof(FolderStructure.FolderStructure.DeletePatterns)}");
            int i = 1;
            var deletePatterns = new DeletePatternsBuilder(_logger).GetTokensDeletePatterns(workingPath);
            foreach (string directory in deletePatterns.Keys)
            {
                _logger.Information($"\tProcessing directory: {Path.GetRelativePath(workingPath, directory)}");
                foreach (var filePath in Directory.GetFiles(directory))
                {
                    var fileName = Path.GetFileName(filePath).ToLower();
                    foreach (var pattern in deletePatterns[directory])
                    {
                        if(directory.Contains("_Unknown") && fileName.ToLower().Contains(pattern.ToLower()))
                        {
                            _logger.Information($"\t\t{i++} - Removing file: {Path.GetRelativePath(workingPath, filePath)}");
                            if (actuallyRemove)
                            {
                                File.Delete(filePath);
                                _logger.Information($"\t\tActually Removed file: {Path.GetRelativePath(workingPath, filePath)}");
                            }
                        }
                        else
                        {
                            var fileWords = fileName.Split(new char[] { '_' });
                            var patternWords = pattern.ToLower().Split();
                            if (IsMatch(patternWords, fileWords))
                            {
                                _logger.Information($"\t\t{i++} - Removing file: {Path.GetRelativePath(workingPath, filePath)}");
                                if (actuallyRemove)
                                {
                                    File.Delete(filePath);
                                    _logger.Information($"\t\tActually Removed file: {Path.GetRelativePath(workingPath, filePath)}");
                                }
                            }
                        }
                    }
                }
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
    }
}
