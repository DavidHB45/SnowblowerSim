using System;

namespace SnowSim.Core.App
{
    /// <summary>What the Boot scene constructs. Chosen by -mode=X or the dev key.</summary>
    public enum AppMode
    {
        Game = 0,
        SnowTest = 1,
        ControllerTest = 2,
    }

    public static class AppModeParser
    {
        public const string ArgPrefix = "-mode=";

        /// <summary>
        /// Returns the mode from the last "-mode=X" argument (case-insensitive,
        /// enum name only), or <paramref name="fallback"/> if absent or invalid.
        /// </summary>
        public static AppMode Parse(string[]? args, AppMode fallback = AppMode.Game)
        {
            if (args == null) return fallback;

            AppMode result = fallback;
            foreach (string arg in args)
            {
                if (arg == null || !arg.StartsWith(ArgPrefix, StringComparison.OrdinalIgnoreCase)) continue;
                string value = arg.Substring(ArgPrefix.Length);
                if (TryParseName(value, out AppMode mode)) result = mode;
            }
            return result;
        }

        /// <summary>Next mode in declaration order, wrapping. Used by the dev key.</summary>
        public static AppMode Next(AppMode mode)
        {
            int count = Enum.GetValues(typeof(AppMode)).Length;
            return (AppMode)(((int)mode + 1) % count);
        }

        static bool TryParseName(string value, out AppMode mode)
        {
            foreach (AppMode m in (AppMode[])Enum.GetValues(typeof(AppMode)))
            {
                if (string.Equals(m.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    mode = m;
                    return true;
                }
            }
            mode = default;
            return false;
        }
    }
}
