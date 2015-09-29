using System.Diagnostics;

namespace ApiLogging
{

    /// <summary>
    /// An enum of eventIds to use. The list starts at 1. Information = 1, Warning = 2,
    /// Error = 3, Success = 4, Failure = 5
    /// </summary>
    public enum EventLoggerEventId { Information = 1, Warning, Error, Success, Failure };

    /// <summary>
    /// Creates a new log and registers a log source if it doesn't already exist. The log
    /// and source name are the Namespace name of the executing application. The class is
    /// a singleton.
    /// 
    /// Usage: EventLogger.Instance.WriteInfo("log this!")
    /// </summary>
    public class EventLogger
    {
        private static EventLogger instance;
        private static string logName;
        private static string logSource;

        /// <summary>
        /// Private constructor that creates the log and source if it doesn't already exist.
        /// </summary>
        private EventLogger()
        {
            logName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            logSource = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            CreateLog(logName, logSource);
        }

        /// <summary>
        /// Creates a log and log source if they don't already exist. The log name and source
        /// can be the same.
        /// </summary>
        /// <param name="logName">The name of the log in which to register the source.</param>
        /// <param name="logSource">The name of the source to register to the log.</param>
        /// <returns></returns>
        private bool CreateLog(string logName, string logSource)
        {
            bool result = false;
            try
            {
                if (!EventLog.SourceExists(logSource))
                {
                    EventLog.CreateEventSource(logName, logSource);
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        public static EventLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventLogger();
                }
                return instance;
            }
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteInfo(string msg)
        {
            EventLog.WriteEntry(logSource, msg, EventLogEntryType.Information, (int)EventLoggerEventId.Information);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteError(string msg)
        {
            EventLog.WriteEntry(logSource, msg, EventLogEntryType.Error, (int)EventLoggerEventId.Error);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteWarning(string msg)
        {
            EventLog.WriteEntry(logSource, msg, EventLogEntryType.Warning, (int)EventLoggerEventId.Warning);
        }

        /// <summary>
        /// Logs a success audit message. There seems to be a bug where Microsoft doesn't use the correct
        /// icon with calls of type EventLogEntryType.SuccessAudit.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteSuccessAudit(string msg)
        {
            EventLog.WriteEntry(logSource, msg, EventLogEntryType.SuccessAudit, (int)EventLoggerEventId.Success);
        }

        /// <summary>
        /// Logs a failure audit message. There seems to be a bug where Microsoft doesn't use the correct
        /// icon with calls of type EventLogEntryType.FailureAudit.
        /// </summary>
        /// <param name="msg"></param>
        public void WriteFailureAudit(string msg)
        {
            EventLog.WriteEntry(logSource, msg, EventLogEntryType.FailureAudit, (int)EventLoggerEventId.Failure);
        }

    }
}