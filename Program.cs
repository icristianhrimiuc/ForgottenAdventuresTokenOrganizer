using ForgottenAdventuresTokenOrganizer.Usefull.Logging;
using ForgottenAdventuresTokenOrganizer.Steps;
using ForgottenAdventuresTokenOrganizer.FolderStructure;

namespace ForgottenAdventuresTokenOrganizer
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            var logger = new Logger();
            try
            {
                logger.Information("Starting up Program");
                Console.WriteLine("Program started, check the log file for more information...");
                if (args.Length > 0)
                {
                    switch (args[0].ToLower())
                    {
                        case "-organizefiles":
                            OrganizeFiles(args[1], logger);
                            break;
                        case "-rewritetokens":
                            RewriteTokens();
                            break;
                        case "-verifyfiles":
                            Verify(args[1], logger);
                            break;
                        default:
                            WriteHelp();
                            break;
                    }
                }
                else
                {
                    WriteHelp();
                }

                logger.Information("Closing Program");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Program execution failed");
            }
            finally
            {
                logger.CloseAndFlush();
            }
        }

        private static void OrganizeFiles(string sourcePath, ILogger logger)
        {
            var path1 = Path.IsPathRooted(sourcePath) ? sourcePath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sourcePath);
            if (Directory.Exists(path1))
            {
                var rootPath = Directory.GetParent(path1).FullName;
                var chaoticFiles = Path.Combine(rootPath, "FA Chaotic Files");
                var organizedFiles = Path.Combine(rootPath, "FA Organized Files");

                new Step1(logger).CopyDirectory(path1, chaoticFiles); // No need to copy again if done ahead OR already executed once
                new Step2(logger).RemoveDuplicatesByFileName(chaoticFiles);  // No need to remove again if already executed once
                new Step3(logger).CopyChaoticFilesToOrganizedFolders(chaoticFiles, organizedFiles);
                new Step4(logger).RemoveDuplicatesByByteComparison(organizedFiles);
                // Step5 is slow and innacurate, skip this one
                new Step6(logger).DeleteFilesByPattern(organizedFiles);
                new Step7(logger).VerifyExpectedFinalNumberOfFiles(organizedFiles);
            }
            else
            {
                logger.Warning($"Source directory {path1} does not exist!");
            }
        }

        private static void RewriteTokens()
        {
            new FolderStructureWritter().RewriteSpirits();
            new FolderStructureWritter().RewriteTokens();
        }

        private static void Verify(string organizedFiles, ILogger logger)
        {
            new Step7(logger).VerifyExpectedFinalNumberOfFiles(organizedFiles);
        }

        private static void WriteHelp()
        {
            Console.WriteLine($"Wrong argumets!");
            Console.WriteLine("Usage example1: ForgottenAdventuresTokenOrganizer.exe -organizefiles \"pathToSource\"");
            Console.WriteLine("Usage example2: ForgottenAdventuresTokenOrganizer.exe -rewritetokens");
            Console.WriteLine("Usage example2: ForgottenAdventuresTokenOrganizer.exe -verifyfiles");
        }
    }
}
