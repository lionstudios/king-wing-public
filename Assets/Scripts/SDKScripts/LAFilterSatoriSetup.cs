using System.Collections.Generic;
using LionStudios.Suite;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Core;
using UnityEngine;

public static class LAFilterSatoriSetup
{

    private const string WHITELIST_PRIORITY_LEVEL_KEY = "whitelist_priority_level";
    private const string ADDITIONAL_WHITELIST_COUNTRIES = "additional_whitelist_countries";

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        if (LionCore.IsInitialized)
            SetupFilter();
        else
            LionCore.OnInitialized += SetupFilter;
    }

    private static void SetupFilter()
    {
        if (SatoriController.TryGetValue(WHITELIST_PRIORITY_LEVEL_KEY, out EventPriorityLevel priorityLevel))
        {
            LionAnalytics.SetWhitelistPriorityLevel(priorityLevel);
        }
        if (SatoriController.TryGetValue(ADDITIONAL_WHITELIST_COUNTRIES, out List<string> additionalCountries))
        {
            LionAnalytics.AddWhitelistCountries(additionalCountries);
        }
    }
    
}
