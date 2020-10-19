namespace VirtualPrinter.Agent.Core
{
    public struct SessionInfo : ISessionInfo
    {
        public int Id { get; set; }

        public string Desktop { get; set; }

        public string Sid { get; set; }
    }
}