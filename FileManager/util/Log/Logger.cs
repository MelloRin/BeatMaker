/*
MIT License
Copyright (c) 2019 Heiswayi Nrird
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

// SimpleLogger.cs from https://gist.github.com/heiswayi/69ef5413c0f28b3a58d964447c275058
// some parts are moded by MelloRin

using System;
using System.Runtime.CompilerServices;

namespace FileManager.util.Log
{
    public class Logger
    {
        private readonly string datetimeFormat;
        private readonly string logFilename;

        internal Logger(string filename)
        {
            datetimeFormat = "yyyy-MM-dd HH:mm:ss.ff";
            logFilename = filename;

            // Log file header line
            string logHeader = logFilename + " is created.";
            if (!System.IO.File.Exists(logFilename))
            {
                WriteLine(DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
            }
        }

        public void Debug(object obj, string text)
        {
            WriteFormattedLog(LogLevel.DEBUG, obj, text);
        }

        public void Error(object obj, string text)
        {
            WriteFormattedLog(LogLevel.ERROR, obj, text);
        }

        public void Fatal(object obj, string text)
        {
            WriteFormattedLog(LogLevel.FATAL, obj, text);
        }

        public void Info(object obj, string text)
        {
            WriteFormattedLog(LogLevel.INFO, obj, text);
        }
        
        public void Trace(object obj, string text)
        {
            WriteFormattedLog(LogLevel.TRACE, obj, text);
        }

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
