using PowerToys_Run_CountrySearch.model;

namespace PowerToys_Run_CountrySearch_Generator.extractor;

public interface ISingleExtractor : IExtractor
{
    string? Extract(Country country);
}