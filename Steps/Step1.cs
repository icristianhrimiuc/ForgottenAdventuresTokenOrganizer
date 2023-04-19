using ForgottenAdventuresTokenOrganizer.Usefull.Logging;
using System.IO;

namespace ForgottenAdventuresTokenOrganizer.Steps
{
    internal class Step1
    {
        private readonly ILogger _logger;

        public Step1(ILogger logger)
        {
            _logger = logger;
        }
    
        public void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            _logger.Information($"{GetType().Name} - Copy directory {diSource.Name} to {diTarget.Name}");
            CopyDirectory(diSource, diTarget);
        }

        public void CopyDirectory(DirectoryInfo source, DirectoryInfo target, int nrOfTabs = 0)
        {
            var tabs = string.Empty;
            for(int i=0; i<nrOfTabs; i++) { tabs += '\t'; }
            _logger.Information($"{tabs}Copy directory {source.Name} to {target.Name}");
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir, nrOfTabs + 1);
            }
        }
    }
}
