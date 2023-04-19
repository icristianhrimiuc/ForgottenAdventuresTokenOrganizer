namespace ForgottenAdventuresTokenOrganizer.FolderStructure
{
    internal class FolderStructure
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string[]? SearchPatterns { get; set; }
        public string[]? DeletePatterns { get; set; }
        public int ExpectedFinalNumberOfFiles { get; set; }
        public List<FolderStructure>? SubDirectories { get; set; }
    }
}
