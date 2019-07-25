using System;
using System.IO;
using System.Linq;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

using Tricentis.TCAddOns;

namespace Tricentis.Automation.WseToApiMigrationAddOn.Shared {
    /// <summary>
    /// Singleton class which can be used for logging.
    /// A separate Log Repository is created when the logger is created. This ensures that we are not affecting any other logger settings which
    /// will be used in Tosca
    /// Logs are stored in ActiveWorkspacePath with name "WseToApiMigrationLogs.txt".
    /// </summary>
    public class FileLogger {
        #region Static Fields

        private static ILog fileLogger;

        private static FileLogger instance;

        private static readonly object Mutex = new object();

        #endregion

        #region Constructors and Destructors

        private FileLogger() {
            InitializeConfiguration();
        }

        #endregion

        #region Public Properties

        public static FileLogger Instance {
            get {
                lock (Mutex) {
                    return instance ?? (instance = new FileLogger());
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Debug(object message) {
            fileLogger.Debug(message);
        }

        public void Error(object message) {
            fileLogger.Error(message);
        }

        public void Error(object message, Exception exception) {
            fileLogger.Error(message, exception);
        }

        public void Fatal(object message) {
            fileLogger.Fatal(message);
        }

        public void Fatal(object message, Exception exception) {
            fileLogger.Fatal(message, exception);
        }

        public static string GetActiveWorkspacePath() {
            string reportPath;
            try {
                reportPath = Path.GetDirectoryName(TCAddOn.ActiveWorkspace.LocationInfo);
                reportPath = reportPath?.Split(new[] { "=" }, StringSplitOptions.None).Last();
            }
            catch (Exception) {
                reportPath = Path.GetTempPath();
            }

            return reportPath;
        }

        public void Info(object message) {
            fileLogger.Info(message);
        }

        public void Warn(object message) {
            fileLogger.Warn(message);
        }

        #endregion

        #region Methods

        private static void InitializeConfiguration() {
            LogManager.CreateRepository("WseToApiMigrationAddOn");
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository("WseToApiMigrationAddOn");
            PatternLayout patternLayout = new PatternLayout {
                    ConversionPattern = "%date [%thread] %-5level - %message%newline"
            };
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender {
                    Threshold = Level.Trace,
                    Name = "WseToApiFileLogger",
                    AppendToFile = false,
                    File = Path.Combine(GetActiveWorkspacePath(), "WseToApiMigrationLogs.txt"),
                    Layout = patternLayout,
                    MaxSizeRollBackups = 100,
                    RollingStyle = RollingFileAppender.RollingMode.Once,
                    StaticLogFileName = true
            };
            roller.ActivateOptions();

            hierarchy.Root.AddAppender(roller);
            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
            fileLogger = LogManager.GetLogger("WseToApiMigrationAddOn", "WseToApiFileLogger");
        }

        #endregion
    }
}