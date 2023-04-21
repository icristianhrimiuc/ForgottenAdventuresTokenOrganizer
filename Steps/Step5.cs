#pragma warning disable CA1416 // Validate platform compatibility
using ForgottenAdventuresTokenOrganizer.Usefull;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;
using System.Drawing;

namespace ForgottenAdventuresTokenOrganizer.Steps
{
    internal class Step5
    {
        private readonly ILogger _logger;
        private string _workingPath = string.Empty;
        private readonly double _percentageOfSimilarity = 0.9985;
        private readonly Dictionary<string, List<string>> _duplicatesDictionary;
        private readonly List<List<string>> _duplicatesList;

        public Step5(ILogger logger)
        {
            _logger = logger;
            _duplicatesDictionary = new Dictionary<string, List<string>>();
            _duplicatesList = new List<List<string>>();
        }

        public void RemoveDuplicatesByImageComparison(string workingPath)
        {
            _workingPath = workingPath;
            _duplicatesDictionary.Clear();
            var processor = new RecursiveFileProcessor(IdentifyDuplicates);
            processor.Process(new string[] { _workingPath });
            ConvertDictionaryToList();
            RemoveDuplicates();
        }

        private void IdentifyDuplicates(string originalFilePath)
        {
            var parentDirectory = Path.GetDirectoryName(originalFilePath);
            if (parentDirectory == null) return;
            var sortedFiles = Directory.GetFiles(parentDirectory).OrderBy(x => Path.GetFileName(x)).ToList();
            var startIndex = sortedFiles.FindIndex(file => Path.GetFileName(file) == Path.GetFileName(originalFilePath)) + 1;
            _logger.Information($"{GetType().Name} - Image comparison for {Path.GetRelativePath(_workingPath, originalFilePath)}");
            for (int i = startIndex; i < sortedFiles.Count; i++)
            {
                var filePath = sortedFiles[i];
                _logger.Information($"\tCompare to {Path.GetRelativePath(_workingPath, filePath)}");
                if (originalFilePath != filePath && ImagesAreSimilar(originalFilePath, filePath))
                {
                    _logger.Information($"\tIdentified duplicates for {Path.GetRelativePath(_workingPath, originalFilePath)}");
                    var key = originalFilePath;
                    _logger.Information($"\t\tKey: {Path.GetRelativePath(_workingPath, key)}");
                    if (!_duplicatesDictionary.ContainsKey(key))
                    {
                        _duplicatesDictionary.Add(key, new List<string>());
                    }
                    if (!_duplicatesDictionary[key].Contains(filePath))
                    {
                        _logger.Information($"\t\t\tAdding duplicate: {Path.GetRelativePath(_workingPath, filePath)}");
                        _duplicatesDictionary[key].Add(filePath);
                    }
                }
            }
        }

        private bool ImagesAreSimilar(string firstFilePath, string secondFilePath)
        {
            var firstBitmap = new Bitmap(firstFilePath);
            var secondBitmap = new Bitmap(secondFilePath);
            var iHash1 = GetHash(firstBitmap);
            var rotations = new RotateFlipType[4] { RotateFlipType.RotateNoneFlipNone, RotateFlipType.Rotate90FlipNone, RotateFlipType.Rotate180FlipNone, RotateFlipType.Rotate270FlipNone };

            var imagesAreSimilar = false;
            for (int i = 0; i < rotations.Length; i++)
            {
                secondBitmap.RotateFlip(rotations[i]);
                var iHash2 = GetHash(secondBitmap);
                int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);
                imagesAreSimilar = imagesAreSimilar || equalElements > iHash1.Count * _percentageOfSimilarity;
            }

            return imagesAreSimilar;
        }

        public static List<string> GetHash(Bitmap bmpSource)
        {
            var lResult = new List<string>();
            var bmpMin = new Bitmap(bmpSource, new System.Drawing.Size(32, 32));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).Name);
                }
            }
            return lResult;
        }

        private void ConvertDictionaryToList()
        {
            _duplicatesList.Clear();
            foreach (var key in _duplicatesDictionary.Keys)
            {
                var duplicates = new List<string>() { key };
                duplicates.AddRange(_duplicatesDictionary[key]);
                _duplicatesList.Add(duplicates);
                foreach (var duplicate in _duplicatesDictionary[key])
                {
                    _duplicatesDictionary.Remove(duplicate);
                }
            }
        }

        private void RemoveDuplicates(bool actuallyRemove = false)
        {
            _logger.Information($"{GetType().Name} - Removing duplicates");
            int i = 1;
            foreach (var duplicates in _duplicatesList)
            {
                if (duplicates.Count > 1)
                {
                    var sortedDuplicates = duplicates.OrderByDescending(x => new FileInfo(x).LastWriteTime);
                    var leftFile = sortedDuplicates.ElementAt(0);
                    _logger.Information($"\t{i++} - Duplicates for: {Path.GetRelativePath(_workingPath, Path.GetDirectoryName(leftFile) ?? string.Empty)}");
                    _logger.Information($"\t\tLeaving file: {Path.GetRelativePath(_workingPath, leftFile)}");
                    foreach (var filePath in sortedDuplicates.Skip(1))
                    {
                        _logger.Information($"\t\tRemoving file: {Path.GetRelativePath(_workingPath, filePath)}");
                        if (actuallyRemove)
                        {
                            var leftFileName = Path.GetFileName(leftFile);
                            if (filePath.Contains(leftFileName) || filePath.Contains(leftFileName[7..]))
                            {
                                File.Delete(filePath);
                                _logger.Information($"\t\tActually Removed file: {Path.GetRelativePath(_workingPath, filePath)}");
                            }
                        }
                    }
                    _logger.Information("");
                }
            }
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility