using System.Collections.Generic;

namespace PowerToys_Run_CountrySearch_Generator.extractor;

public interface IMassExtractor : IExtractor
{
    bool CreateNewCountry()
    {
        return true;
    }

    Dictionary<string, string> Extract();
}