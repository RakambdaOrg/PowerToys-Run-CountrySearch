using System.Collections.Generic;

namespace PowerToys_Run_CountrySearch_Generator;

public interface IExtractor
{
    string[] GetJsonPath();

    bool OverrideIfSet()
    {
        return false;
    }

    Dictionary<string, string> Extract();
}