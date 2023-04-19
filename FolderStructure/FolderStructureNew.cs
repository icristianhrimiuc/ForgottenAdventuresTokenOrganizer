using System.Collections.Generic;

namespace ForgottenAdventuresTokenOrganizer.FolderStructure
{
    internal class FolderStructureNew
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string[] SearchPatterns { get; set; }
        public string[] DeletePatterns { get; set; }
        public int ExpectedFinalNumberOfFiles { get; set; }
        public List<FolderStructureNew> SubDirectories { get; set; }
    }
}
