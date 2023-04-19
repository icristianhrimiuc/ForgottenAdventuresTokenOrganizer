# ForgottenAdventuresTokenOrganizer
This is a .NET Framework Console App designed to organize the topdown tokens from ForgottenAdventures Patreon Creator;
- To use it simply build the solution then launch it using the syntax: ForgottenAdventuresTokenOrganizer.exe -source "Path\To\Source";
- "Path\To\Source" can be a relative or absolute path;
- "Path\To\Source" should contain all the directories that are created when extracting the archives to separate folders. Only the directories, not the archives;
- Very little information is displayed on the console, but a log.txt file is created at the runtime location where a detailed run of the program actions can be found. The log file will be rewritten on each run;
- No files will be deleted from the source path, a duplicate directory will be created and used as working path;
- Files with the same name and size will be deleted straight away, older files are deleted, and the newest one is kept;
- The remaining files will be organized acording to the structure specified in "FolderStructure/Json/tokens_folder_structure.json". A file matching any of the "SearchPatterns" for a directory will be copied to that directory;
- Files with the same name but different file size will be saved with "Duplc1_" prefix acording to the "Date Modified" value. The newest file will keep the name while the older ones will be renamed;
- After organizing the files there will be a second search for duplicates that compare files by bytes. Again, older files are deleted, and the newest one is kept;
- The folder structure can be changed by modifying the the files mentioned below, and then regenerating "tokens_folder_structure.json" using the -rewritetokens option, or by modifying "tokens_folder_structure.json" directly;
- Extra files are deleted acording to "DeletePatterns" for each folder;
- The last step is to verify the number of files in each directory using "ExpectedFinalNumberOfFiles";

The following files are just components of "tokens_folder_structure.json", they are only used to rewrite tokens:
    - "commoners_folder_structure.json";
    - "creatures_folder_structure.json";
    - "heroes_folder_structure.json";
    - "spirit_commoners_folder_structure.json";
    - "spirit_creatures_folder_structure.json";
    - "spirit_heroes_folder_structure.json";
      
Example of directory structure:
{
  "DirectoryName": "Gnomes",
  "SearchPatterns": [
    "gnome commoner"
  ],
  "DeletePatterns": null,
  "ExpectedFinalNumberOfFiles": 12,
  "SubDirectories": null
}
