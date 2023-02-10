namespace MiniCLI
{
    public class CmdOptions
    {
        public int AbnormalExitCode { get; }
        public bool CaseSensitiveTags { get; }
        public bool AllowExtraArguments { get; }
        public DisplayFlags DisplayFlags { get; }
        public LogOptions LogOptions { get; }
        public CmdOptions(
            int abnormalExitCode = -1,
            bool caseSensitiveTags = false,
            bool allowExtraArguments = false,
            DisplayFlags displayFlags = DisplayFlags.Default,
            LogOptions? logOptions = null)
        {
            AbnormalExitCode = abnormalExitCode;
            CaseSensitiveTags = caseSensitiveTags;
            AllowExtraArguments = allowExtraArguments;
            DisplayFlags = displayFlags;
            LogOptions = logOptions ?? new LogOptions();
        }

    }
}