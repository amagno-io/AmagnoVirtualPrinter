using AmagnoVirtualPrinter.Agent.Core.Interfaces;

namespace AmagnoVirtualPrinter.Agent.Core.Model
{
    public struct SessionInfo : ISessionInfo
    {
        public int Id { get; set; }

        public string Desktop { get; set; }

        public string Sid { get; set; }
    }
}