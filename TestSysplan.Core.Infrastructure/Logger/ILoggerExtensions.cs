using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TestSysplan.Core.Infrastructure.Logger
{
    public static class ILoggerExtensions
    {
        #region Log base
        internal delegate void LogAction(string message, object[] args);
        internal delegate void LogError(Exception error, string message, object[] args);

        private static string LogTraceMessage(
            Type loggerType,
            Exception error,
            string message,
            string memberName,
            string sourceFile,
            int sourceLine)
        {
            string targetName = loggerType.IsGenericType ? 
                loggerType.GetGenericArguments()?.FirstOrDefault()?.Name : null;

            int errorLine = 0;
            string stackTrace = null;

            if(error != null)
            {
                var st = new StackTrace(error, true);
                var sf = st.GetFrame(st.FrameCount - 1);
                errorLine = sf?.GetFileLineNumber() ?? errorLine;
                stackTrace = error.StackTrace?.Replace(" at ", "\r\n\t\t└► at ", StringComparison.InvariantCultureIgnoreCase);
                memberName = string.IsNullOrEmpty(memberName) ? sf?.GetMethod()?.Name : memberName;
                sourceFile = string.IsNullOrEmpty(sourceFile) ? sf?.GetFileName() : sourceFile;
            }

            return new StringBuilder()
               .Append("Message: ").AppendLine(message ?? error?.Message ?? throw new ArgumentNullException(nameof(message)))
               .AppendLineIf(targetName != null, "\t► Target Name: ", targetName)
               .AppendLineIf(memberName != null, "\t► Member Name: ", memberName)
               .AppendLineIf(sourceFile != null, "\t► Source File: ", sourceFile)
               .AppendLineIf(sourceLine > 0, "\t► Source Line: ", sourceLine)
               .AppendLineIf(errorLine > 0, "\t► Error Line : ", errorLine)
               .AppendLineIf(stackTrace != null, "\t► Stack Trace: ", stackTrace)
               .ToString();
        }

        internal static void Log(
            LogAction action,
            LogError errAction,
            Type loggerType,
            Exception error,
            string message,
            object[] args,
            string memberName,
            string sourceFile,
            int sourceLine)
        {
            string trace = LogTraceMessage(
                       loggerType,
                       error,
                       message,
                       memberName,
                       sourceFile,
                       sourceLine);

            if (errAction != null && error != null)
            {
                errAction.Invoke(error, trace, args);
            }
            else
            {
                action.Invoke(trace, args);
            }
        }

        internal static void Log(
            LogAction action,
            Type loggerType,
            string message,
            object[] args,
            string memberName,
            string sourceFile,
            int sourceLine)
        {
            Log(action,
                null,
                loggerType,
                null,
                message,
                args,
                memberName,
                sourceFile,
                sourceLine);
        }
        #endregion

        #region Critical
        public static void LogC(this ILogger logger, string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogCritical, logger.GetType(), message, args, memberName, filePath, lineNumber);
        }

        public static void LogC(this ILogger logger,
            Exception error,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogCritical,
                logger.LogCritical,
                logger.GetType(), error, message, args, memberName, filePath, lineNumber);
        }
        #endregion

        #region Debug
        public static void LogD(this ILogger logger, string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogDebug, logger.GetType(), message, args, memberName, filePath, lineNumber);
        }

        public static void LogD(this ILogger logger,
            Exception error,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogDebug,
                logger.LogDebug,
                logger.GetType(), error, message, args, memberName, filePath, lineNumber);
        }
        #endregion

        #region Error
        public static void LogE(this ILogger logger, string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogError, logger.GetType(), message, args, memberName, filePath, lineNumber);
        }

        public static void LogE(this ILogger logger,
            Exception error,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogError, 
                logger.LogError, 
                logger.GetType(), error, message, args, memberName, filePath, lineNumber);
        }
        #endregion

        #region Info
        public static void LogI(this ILogger logger, string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogInformation, logger.GetType(), message, args, memberName, filePath, lineNumber);
        }
        #endregion

        #region Trace
        public static void LogT(this ILogger logger, string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogTrace, logger.GetType(), message, args, memberName, filePath, lineNumber);
        }
        
        public static void LogT(this ILogger logger,
            Exception error,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogTrace, 
                logger.LogTrace, 
                logger.GetType(), error, message, args, memberName, filePath, lineNumber);
        }
        #endregion

        #region Warn
        public static void LogW(this ILogger logger, string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogWarning, logger.GetType(), message, args, memberName, filePath, lineNumber);
        }

        public static void LogW(this ILogger logger,
            Exception error,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(logger.LogWarning,
                logger.LogWarning,
                logger.GetType(), error, message, args, memberName, filePath, lineNumber);
        }
        #endregion
    }
}
