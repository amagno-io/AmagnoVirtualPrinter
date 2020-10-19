namespace VirtualPrinter.Agent.Core
{
    public class UserRegistryConfig : IUserConfig
    {
        public bool RedirectEnabled { get; set; }

        public string RedirectPrinter { get; set; }

        public double? UserRenderDpi { get; set; }

        public string Format { get; set; }
    }
}