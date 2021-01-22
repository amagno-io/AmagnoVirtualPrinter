namespace AmagnoVirtualPrinter.Agent.Core.Model
{
    public struct PostScriptRenderOptions
    {
        public double? UserRenderDpi { get; set; }
        public PostScriptRenderPdfOptions PdfOptions { get; set; }
        public PostScriptRenderTiffOptions TiffOptions { get; set; }
    }
}