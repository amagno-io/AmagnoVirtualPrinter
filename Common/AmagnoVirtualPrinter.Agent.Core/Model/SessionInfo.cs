using AmagnoVirtualPrinter.Agent.Core.Interfaces;

namespace AmagnoVirtualPrinter.Agent.Core.Model
{
    public class SessionInfo : ISessionInfo
    {
        public int Id { get; set; }

        public string Desktop { get; set; }

        public string Sid { get; set; }
        
        public bool FoundDomain { get; set; }
    }
}