using ForgottenAdventuresTokenOrganizer.Usefull;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;

namespace ForgottenAdventuresTokenOrganizer.Steps
{
    internal class Step4
    {
        private readonly ILogger _logger;
        private string _workingPath = string.Empty;
        private readonly Dictionary<string, List<string>> _duplicatesDictionary;

        public Step4(ILogger logger)
        {
            _logger = logger;
            _duplicatesDictionary = new Dictionary<string, List<string>>();
        }

        public void RemoveDuplicatesByByteComparison(string workingPath)
        {
            _workingPath = workingPath;
            _duplicatesDictionary.Clear();
            var processor = new RecursiveFileProcessor(IdentifyDuplicates);
            processor.Process(new string[] { _workingPath });
            RemoveDuplicates(true);
        }

        private void IdentifyDuplicates(string originalFilePath)
        {
            var originalFileSize = new FileInfo(originalFilePath).Length;
            var parentDirectory = Path.GetDirectoryName(originalFilePath);
            if (parentDirectory == null) return;
            foreach (var filePath in Directory.GetFiles(parentDirectory))
            {
                if (originalFilePath != filePath && FilesAreEqual(originalFilePath, filePath))
                {
                    _logger.Information($"{GetType().Name} - Identified duplicates for {Extensions.GetRelativePath(_workingPath, originalFilePath)}");
                    var key = $"{parentDirectory} - {originalFileSize}";
                    _logger.Information($"\tKey: {Extensions.GetRelativePath(_workingPath, key)}");
                    if (!_duplicatesDictionary.ContainsKey(key))
                    {
                        _duplicatesDictionary.Add(key, new List<string>());
                    }
                    if (!_duplicatesDictionary[key].Contains(filePath))
                    {
                        _logger.Information($"\t\tAdding duplicate: {Extensions.GetRelativePath(_workingPath, filePath)}");
                        _duplicatesDictionary[key].Add(filePath);
                    }
                }
            }
        }

        private static bool FilesAreEqual(string firstFilePath, string secondFilePath)
        {
            int BYTES_TO_READ = sizeof(long);
            var first = new FileInfo(firstFilePath);
            var second = new FileInfo(secondFilePath);
            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }

        private void RemoveDuplicates(bool actuallyRemove = false)
        {
            _logger.Information($"{GetType().Name} - Removing duplicates");
            int i = 1;
            foreach (var key in _duplicatesDictionary.Keys)
            {
                if (_duplicatesDictionary[key].Count > 1)
                {
                    _logger.Information($"\t{i++} - Key: {Extensions.GetRelativePath(_workingPath, key)}");
                    var sortedDuplicates = _duplicatesDictionary[key].OrderByDescending(x => new FileInfo(x).LastWriteTime);
                    _logger.Information($"\t\tLeaving file: {Extensions.GetRelativePath(_workingPath, sortedDuplicates.ElementAt(0))}");
                    foreach (var filePath in sortedDuplicates.Skip(1))
                    {
                        _logger.Information($"\t\tRemoving file: {Extensions.GetRelativePath(_workingPath, filePath)}");
                        if (actuallyRemove)
                        {
                            File.Delete(filePath);
                            _logger.Information($"\t\tActually Removed file: {Extensions.GetRelativePath(_workingPath, filePath)}");
                        }
                    }
                }
            }
        }
    }
}
