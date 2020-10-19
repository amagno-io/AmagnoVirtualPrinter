namespace VirtualPrinter.Agent.Core
{
    public interface ISessionInfo
    {
        int Id { get; set; }

        string Desktop { get; set; }

        string Sid { get; set; }
    }
}