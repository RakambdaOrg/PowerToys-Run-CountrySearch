namespace PowerToys_Run_CountrySearch_Generator.extractor;

public interface IExtractor
{
    string[] GetJsonPath();

    bool OverrideIfSet()
    {
        return false;
    }
}