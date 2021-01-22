using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using JetBrains.Annotations;

using NLog;

namespace AmagnoVirtualPrinter.Logging
{
    public class VirtualPrinterLogger<T> : VirtualPrinterLogger, IVirtualPrinterLogger<T>
    {
        public VirtualPrinterLogger() : base(ReadableTypeName.Generate(typeof(T)))
        {
        }
    }

    public static class ReadableTypeName
    {
        [NotNull]
        public static string Generate([NotNull]Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var provider = CodeDomProvider.CreateProvider("C#");
            var typeReferenceExpression = new CodeTypeReferenceExpression(type);
            using (var writer = new StringWriter())
            {
                provider.GenerateCodeFromExpression(typeReferenceExpression, writer, new CodeGeneratorOptions());
                return writer.GetStringBuilder().ToString();
            }
        }
    }

    public class VirtualPrinterLogger : IVirtualPrinterLogger
    {
        private readonly ILogger _logger;

        public VirtualPrinterLogger([NotNull]string loggerName)
        {
            if (string.IsNullOrWhiteSpace(loggerName))
            {
                throw new ArgumentNullException(nameof(loggerName));
            }

            _logger = LogManager.GetLogger(loggerName);
        }

        public string Name
        {
            get { return _logger.Name; }
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }

        public bool IsTraceEnabled
        {
            get { return _logger.IsTraceEnabled; }
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }
    }
}