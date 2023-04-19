namespace ForgottenAdventuresTokenOrganizer.FolderStructure
{
    internal abstract class JsonFileHandler
    {
        protected string JsonPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FolderStructure\\Json"); } }
        protected string CommonersJsonFilePath = "commoners_folder_structure.json";
        protected string CreaturesJsonFilePath = "creatures_folder_structure.json";
        protected string HeroesJsonFilePath = "heroes_folder_structure.json";
        protected string SpiritCommonersJsonFilePath = "spirit_commoners_folder_structure.json";
        protected string SpiritCreaturesJsonFilePath = "spirit_creatures_folder_structure.json";
        protected string SpiritHeroesJsonFilePath = "spirit_heroes_folder_structure.json";
        protected string TokensJsonFilePath = "tokens_folder_structure.json";

        protected string GetCommonersJsonContent()
        {
            return File.ReadAllText(Path.Combine(JsonPath, CommonersJsonFilePath));
        }

        protected string GetCreaturesJsonContent()
        {
            return File.ReadAllText(Path.Combine(JsonPath, CreaturesJsonFilePath));
        }

        protected string GetHeroesJsonContent()
        {
            return File.ReadAllText(Path.Combine(JsonPath, HeroesJsonFilePath));
        }

        protected string GetSpiritCommonersJsonContent()
        {
            return File.ReadAllText(Path.Combine(JsonPath, SpiritCommonersJsonFilePath));
        }

        protected string GetSpiritCreaturesJsonContent()
        {
            return File.ReadAllText(Path.Combine(JsonPath, SpiritCreaturesJsonFilePath));
        }

        protected string GetSpiritHeroesJsonContent()
        {
            return File.ReadAllText(Path.Combine(JsonPath, SpiritHeroesJsonFilePath));
        }

        protected string GetTokensJsonContent()
        {
            return File.ReadAllText(Path.Combine(JsonPath, TokensJsonFilePath));
        }
    }
}
