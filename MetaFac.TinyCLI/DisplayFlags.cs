using System;

namespace MetaFac.TinyCLI
{
    [Flags]
    public enum DisplayFlags
    {
        Default = 0,
        // bits
        HideTitle = 1,
        HideCredits = 2,
        HideInputs = 4,
        HideExitCode = 8,
        HideLogs = 16,
        // all
        HideAll = 31,
    }
}