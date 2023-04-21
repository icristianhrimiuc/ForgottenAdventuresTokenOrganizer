using ForgottenAdventuresTokenOrganizer.FolderStructure;
using ForgottenAdventuresTokenOrganizer.Usefull.Logging;

namespace ForgottenAdventuresTokenOrganizer.Steps
{
    internal class Step7
    {
        private readonly ILogger _logger;

        public Step7(ILogger logger)
        {
            _logger = logger;
        }

        public void VerifyExpectedFinalNumberOfFiles(string workingPath)
        {
            _logger.Information($"{GetType().Name} - Verifying final number of files using {nameof(FolderStructure.FolderStructure.ExpectedFinalNumberOfFiles)}");
            var expectedNrOfFiles = new ExpectedNrOfFilesBuilder(_logger).GetTokensExpectedNrOfFiles(workingPath);
            var actualTotalNumberOfFiles = 0;
            var expectedTotalNumberOfFiles = 0;
            foreach (string directory in expectedNrOfFiles.Keys)
            {
                expectedTotalNumberOfFiles += expectedNrOfFiles[directory];
                if (Directory.Exists(directory))
                {
                    var actualNrOfFiles = Directory.GetFiles(directory).Count();
                    actualTotalNumberOfFiles += actualNrOfFiles;
                    if (actualNrOfFiles != expectedNrOfFiles[directory])
                    {
                        var diff = Math.Abs(actualNrOfFiles - expectedNrOfFiles[directory]);
                        var textDiff = actualNrOfFiles > expectedNrOfFiles[directory] ? "more" : "less";
                        _logger.Warning($"\tDirectory {Path.GetRelativePath(workingPath, directory)} has {diff} {textDiff} files than expected!");
                    }
                }
                else
                {
                    _logger.Warning($"\tDirectory {Path.GetRelativePath(workingPath, directory)} does not exist!");
                }
            }
            _logger.Information($"{GetType().Name} - Done verifying! Total number of files is Actual: {actualTotalNumberOfFiles} Expected: {expectedTotalNumberOfFiles}");
        }
    }
}
