using ForgottenAdventuresTokenOrganizer.Usefull;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;

namespace ForgottenAdventuresTokenOrganizer.Steps
{
    internal class Step2
    {
        private readonly ILogger _logger;
        private string _workingPath = string.Empty;
        private Dictionary<string, List<string>> _duplicatesDictionary;
        private List<string> _extraFileExtension = new List<string> { ".url", ".pdf", ".jpg", ".zip", ".rar" };

        public Step2(ILogger logger)
        {
            _logger = logger;
            _duplicatesDictionary = new Dictionary<string, List<string>>();
        }

        public void RemoveDuplicatesByFileName(string workingPath)
        {
            _workingPath = workingPath;
            _duplicatesDictionary.Clear();
            var processor = new RecursiveFileProcessor(IdentifyDuplicates);
            processor.Process(new string[] { _workingPath });
            RemoveDuplicates(true);
        }

        private void IdentifyDuplicates(string filePath)
        {
            if (_extraFileExtension.Contains(Path.GetExtension(filePath)))
            {
                File.Delete(filePath);
                return;
            }

            var key = Path.GetFileNameWithoutExtension(filePath) + " - " + new FileInfo(filePath).Length;
            if (!_duplicatesDictionary.ContainsKey(key))
            {
                _duplicatesDictionary.Add(key, new List<string>());
            }
            _logger.Information($"{GetType().Name} - Identified duplicates for Key: {key} Adding duplicate: {Path.GetRelativePath(_workingPath, filePath)}");
            _duplicatesDictionary[key].Add(filePath);
        }

        private void RemoveDuplicates(bool actuallyRemove = false)
        {
            _logger.Information($"{GetType().Name} - Removing duplicates");
            int i = 1;
            foreach (var key in _duplicatesDictionary.Keys)
            {
                if (_duplicatesDictionary[key].Count > 1)
                {
                    _logger.Information($"\t{i++} - Key: {key}");
                    var sortedDuplicates = _duplicatesDictionary[key].OrderByDescending(x => new FileInfo(x).LastWriteTime);
                    _logger.Information($"\t\tLeaving file: {Path.GetRelativePath(_workingPath, sortedDuplicates.ElementAt(0))}");
                    foreach (var filePath in sortedDuplicates.Skip(1))
                    {
                        _logger.Information($"\t\tRemoving file: {Path.GetRelativePath(_workingPath, filePath)}");
                        if (actuallyRemove) File.Delete(filePath);
                    }
                }
            }
        }
    }
}
