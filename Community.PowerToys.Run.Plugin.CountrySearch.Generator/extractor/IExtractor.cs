namespace PowerToys_Run_CountrySearch_Generator.extractor;

public interface IExtractor
{
    string[] GetPropertyPath();

    bool OverrideIfSet()
    {
        return false;
    }
}