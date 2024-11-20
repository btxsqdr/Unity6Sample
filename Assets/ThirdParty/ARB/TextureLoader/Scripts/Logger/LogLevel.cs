using System;

namespace ARB.TextureLoader
{
    [Flags]
    public enum LogLevel
    {
        None = 0,
        Debug = 2,
        Info = 4,
        Warning = 8,
        Error = 16,
        All = ~0
    }
}