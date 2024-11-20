// Created by Andre Rene Biasi on 2024/09/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ARB.TextureLoader
{
    internal class Logger
    {
        public string Id { get; private set; }
        public LogLevel Level { get; private set; }

        public bool CanLogDebug => Level.HasFlag(LogLevel.Debug);
        public bool CanLogInfo => Level.HasFlag(LogLevel.Info);
        public bool CanLogWarnings => Level.HasFlag(LogLevel.Warning);
        public bool CanLogErrors => Level.HasFlag(LogLevel.Error);

        public Logger(string id, LogLevel logLevel)
        {
            Id = id;
            Level = logLevel;
        }

        public void Debug(string message, Object context = null, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            => Log(LogLevel.Debug, message, context, callerName, callerPath, callerLine);

        public void Info(string message, Object context = null, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            => Log(LogLevel.Info, message, context, callerName, callerPath, callerLine);

        public void Warning(string message, Object context = null, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            => Log(LogLevel.Warning, message, context, callerName, callerPath, callerLine);

        public void Error(string message, Object context = null, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            => Log(LogLevel.Error, message, context, callerName, callerPath, callerLine);

        private void Log(LogLevel logLevel, string message, Object context = null, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
        {
            if (logLevel == LogLevel.None) return;

            string fileName = Path.GetFileName(callerPath);
            message = $"[{Id}] {fileName}:{callerLine}:{callerName}: {message}";

            switch (logLevel)
            {
                default:
                case LogLevel.Info:
                    if (CanLogInfo) UnityEngine.Debug.Log(message, context);
                    break;
                case LogLevel.Warning:
                    if (CanLogWarnings) UnityEngine.Debug.LogWarning(message, context);
                    break;
                case LogLevel.Error:
                    if (CanLogErrors) UnityEngine.Debug.LogError(message, context);
                    break;
                case LogLevel.Debug:
                    if (CanLogDebug) UnityEngine.Debug.Log(message, context);
                    break;
            }
        }

        private string FormatWithColor(string message, string richTextColor)
            => Application.isEditor ? $"<color={richTextColor}>{message}</color>" : message;
    }
}