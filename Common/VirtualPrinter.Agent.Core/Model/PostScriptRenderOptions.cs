namespace VirtualPrinter.Agent.Core
{
    public struct PostScriptRenderOptions
    {
        public double? UserRenderDpi { get; set; }
        public PostScriptRenderPdfOptions PdfOptions { get; set; }
        public PostScriptRenderTiffOptions TiffOptions { get; set; }
    }
}