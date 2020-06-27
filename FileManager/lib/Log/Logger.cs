using System;
using System.Runtime.CompilerServices;

namespace FileManager.lib.Log
{
    public class Logger
    {
        private readonly string datetimeFormat;
        private readonly string logFilename;

        /// <summary>
        /// Initiate an instance of SimpleLogger class constructor.
        /// If log file does not exist, it will be created automatically.
        /// </summary>
        internal Logger()
        {
            datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            logFilename = FileManagerCore.logFileName;

            // Log file header line
            string logHeader = logFilename + " is created.";
            if (!System.IO.File.Exists(logFilename))
            {
                WriteLine(DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
            }
        }

        /// <summary>
        /// Log a DEBUG message
        /// </summary>
        /// <param name="obj">Called calss</param>
        /// <param name="text">Message</param>
        public void Debug(object obj, string text)
        {
            WriteFormattedLog(LogLevel.DEBUG, obj, text);
        }

        /// <summary>
        /// Log an ERROR message
        /// </summary>
        /// <param name="obj">Called calss</param>
        /// <param name="text">Message</param>
        public void Error(object obj, string text)
        {
            WriteFormattedLog(LogLevel.ERROR, obj, text);
        }

        /// <summary>
        /// Log a FATAL ERROR message
        /// </summary>
        /// <param name="obj">Called calss</param>
        /// <param name="text">Message</param>
        public void Fatal(object obj, string text)
        {
            WriteFormattedLog(LogLevel.FATAL, obj, text);
        }

        /// <summary>
        /// Log an INFO message
        /// </summary>
        /// <param name="obj">Called calss</param>
        /// <param name="text">Message</param>
        public void Info(object obj, string text)
        {
            WriteFormattedLog(LogLevel.INFO, obj, text);
        }

        /// <summary>
        /// Log a TRACE message
        /// </summary>
        /// <param name="obj">Called calss</param>
        /// <param name="text">Message</param>
        public void Trace(object obj, string text)
        {
            WriteFormattedLog(LogLevel.TRACE, obj, text);
        }

        /// <summary>
        /// Log a WARNING message
        /// </summary>
        /// <param name="obj">Called calss</param>
        /// <param name="text">Message</param>
        public void Warning(object obj, string text)
        {
            WriteFormattedLog(LogLevel.WARNING, obj, text);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void WriteLine(string text, bool append = true)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilename, append, System.Text.Encoding.UTF8))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        Console.WriteLine(text);
                        writer.WriteLine(text);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void WriteFormattedLog(LogLevel level, object obj, string text)
        {
            string pretext = DateTime.Now.ToString(datetimeFormat);
            pretext += " [" + obj.GetType().Name + "]";

            switch (level)
            {
                case LogLevel.TRACE:
                    pretext += " [TRACE] ";
                    break;
                case LogLevel.INFO:
                    pretext += " [INFO] ";
                    break;
                case LogLevel.DEBUG:
                    pretext += " [DEBUG] ";
                    break;
                case LogLevel.WARNING:
                    pretext += " [WARNING] ";
                    break;
                case LogLevel.ERROR:
                    pretext += " [ERROR] ";
                    break;
                case LogLevel.FATAL:
                    pretext += " [FATAL] ";
                    break;
                default:
                    pretext = string.Empty;
                    break;
            }

            WriteLine(pretext + text);
        }

        [Flags]
        private enum LogLevel
        {
            TRACE,
            INFO,
            DEBUG,
            WARNING,
            ERROR,
            FATAL
        }
    }
}
