namespace AmagnoVirtualPrinter.Utils
{
    public struct Files
    {
        public const string FILES = @"Files";
        public const string PRINTER_SERVICE_EXE = "AmagnoPrinterAgent.exe";
        public const string SETUP_DRIVER_EXE = "setupdrv.exe";
        public const string AGENT_PROGRESS_EXE = "AmagnoPrinterAgentProgress.exe";
        public const string DELIVERY_EXE = "delivery.exe";
        public const string LICENCE_FILE = "LICENSE.rtf";
        public const string PRE_CONVERTER = @"C:\Program Files (x86)\MyPreConverter.exe ARG";
        public const string POST_CONVERTER = @"C:\Program Files (x86)\MyPostConverter.exe ARG";
    }

    public struct Keys
    {
        public const string PRINTER_DRIVER_KEY32 = @"SOFTWARE\vpd\PrinterDriver";
        public const string PRINTER_DRIVER_KEY64 = @"SOFTWARE\Wow6432Node\vpd\PrinterDriver";
        public const string POSTCONVERTER_KEY = @"Application\Postconverter";
        public const string PRECONVERTER_KEY = @"Application\Preconverter";
        public const string CONVERTER_KEY = @"Converter";
        public const string CONVERTER_PDF_KEY = CONVERTER_KEY + @"\PDF";
        public const string CONVERTER_TIFF_KEY = CONVERTER_KEY + @"\TIFF";
        public const string CONVERTER_REDIRECT_KEY = CONVERTER_KEY + @"\Redirect";
    }

    public struct KeyNames
    {
        public const string CUSTOM_REDIRECTION_KEY = "CustomRedirection";
        public const string EXECUTABLE_FILE = "Executable File";
        public const string INSTALLATION_DIR = "Installation Directory";
        public const string THREADS = "Threads";
        public const string SHOW_PROGRESS = "Show Progress";
        public const string PAGES_PER_SHEET = "Pages per Sheet";
        public const string FILE_NAME_MASK = "File name mask";
        public const string OUTPUT_DIR = "Output Directory";
        public const string ENABLED = "Enabled";
        public const string MULTIPAGE = "Multipage";
        public const string PRODUCE_PDFA = "Produce PDFA";
        public const string ALLOW_PRINTING = "Allow printing";
        public const string ALLOW_COPYING = "Allow printing";
        public const string SUBSETTING = "Subsetting";
        public const string QUALITY = "Image Quality";
        public const string BITS_PIXEL = "Bits per pixel";
        public const string COMPRESSION = "Compression";
        public const string SERVER_PORT = "Server port";
        public const string RENDER_DPI = "RENDER: DPI";
        public const string FORMAT = "Intermediate Format";
        public const string PRINT_FORMAT = "Format";
        public const string PRINTER = "Printer";
    }
}