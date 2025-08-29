using System;
using System.Collections.Generic;

public static class DifficultyDisplay
{
    // Internal canonical names (do not change; match your old level file names)
    public const string Easy = "Easy";
    public const string Normal = "Normal";
    public const string Hard = "Hard";
    public const string Extreme = "Extreme";

    // Display names (player-facing)
    private static readonly Dictionary<string, string> ToDisplay = new(StringComparer.Ordinal)
    {
        { Easy,    "Apprentice" },
        { Normal,  "Adept"      },
        { Hard,    "Wizard"     },
        { Extreme, "Archmage"   },
    };

    // Accept either internal OR display and return the internal canonical
    private static readonly Dictionary<string, string> ToInternal = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Easy",      Easy    }, { "Apprentice", Easy    },
        { "Normal",    Normal  }, { "Adept",      Normal  },
        { "Hard",      Hard    }, { "Wizard",     Hard    },
        { "Extreme",   Extreme }, { "Archmage",   Extreme },
    };

    public static string GetDisplay(string internalName)
        => (internalName != null && ToDisplay.TryGetValue(internalName, out var disp)) ? disp : internalName;

    public static string GetInternal(string anyName)
        => (anyName != null && ToInternal.TryGetValue(anyName, out var canon)) ? canon : anyName ?? Normal;
}
