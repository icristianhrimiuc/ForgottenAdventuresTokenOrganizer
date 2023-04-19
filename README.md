# ForgottenAdventuresTokenOrganizer
This is a .NET Framework Console App designed to organize the topdown tokens from ForgottenAdventures Patreon Creator;

Download: 
To download the latest version of this tool check the available versions in Releasese, just to the right of this text ====>

Example usages:
- ForgottenAdventuresTokenOrganizer.exe -organizefiles "Path\To\Source\Files"
    + "Path\To\Source\Files" can be a relative or absolute path;
    + "Path\To\Source\Files" should contain all the directories that are created when extracting the archives to separate folders;
    + Only the directories, not the archives;
- ForgottenAdventuresTokenOrganizer.exe -verifyFiles "Path\To\Organized\Files"
    + "Path\To\Organized\Files" can be a relative or absolute path;
    + "Path\To\Organized\Files" should contain the organized files. This is designed to be used as a quick test of the final output;
- ForgottenAdventuresTokenOrganizer.exe -rewriteTokens
    + This will use the folder structure from the independent .json files to rewrite the contents of "tokens_folder_structure.json". It can be used to check the validity of the .json files;

Step description:
- Step1 will copy the files from the source path to "FA Chaotic Files" which will serve as workingpath for Step2, as such no files will be deleted from the source path;
- Step2 will remove non-image files then identify duplicates based on filename and filesize. The newest file will be kept, the older files will be deleted;
- Step3 will go trough the remaining files in the "FA Chaotic Files" and copy them to "FA Organized Files" according to the folderstructure defined in "tokens_folder_structure.json" using the "SearchPatterns" ;
    + Some files from the "FA Chaotic Files" directory will have the same filename but not the same filesize, these files are considered duplicates, and will be copied according to the folderstructure but renamed using the prefix "Duplc1_" acording to the "Date Modified" value. "Duplc1_" should be newer than "Duplc2_";
    + It is possible to copy a file to multiple destinations by using the same search pattern on different directories;
    + The folder structure can be changed by modifying the the files in "FolderStructure/Json", and then regenerating "tokens_folder_structure.json" or by modifying "tokens_folder_structure.json" directly;
- Step4 will go trough the directories in "FA Organized Files" and will try to identify duplicates based on byte comparison, ignorif the filename. Again, the newest file will be kept, the older files will be deleted;
- Step5 was supposed to compare images by rotating them, but it is slow and innacurate, I'm leaving it here in case someone has a better idea;
- Step6 will remove extra files using the values in "DeletePatterns";
- Step7 will verify the final number of files in each directory at the end using the "ExpectedFinalNumberOfFiles";

Very little information is displayed on the console, but a log.txt file is created at the runtime location where a detailed run of the program actions can be found. The log file will be rewritten on each run;

FolderStructure definition:
- "DirectoryName" => The name of the final derectory
- "SearchPatterns" => A list of lowercase strings where for each string the words are separated by space. These will be used to match files to are to be stored in this specific directory;
- "DeletePatterns" => A list of lowercase strings where for each string the words are separated by space. These will be used to match files that are considered as extras and that will be deleted in Step6;
- "ExpectedFinalNumberOfFiles" => A integer that specifies the final number of files expected in this directory at the end of the process. This will be used during Step7;
- "SubDirectories" => A list of FolderStructure objects that specifies the subdirectories of this directory and their specifics. Using this property an arborescent folder structure can be build;

The following files are just independent .json files, they are used to rewrite the contents of "tokens_folder_structure.json":
- "commoners_folder_structure.json";
- "creatures_folder_structure.json";
- "heroes_folder_structure.json";
- "spirit_commoners_folder_structure.json";
- "spirit_creatures_folder_structure.json";
- "spirit_heroes_folder_structure.json";
