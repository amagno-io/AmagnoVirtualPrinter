namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface ISessionInfo
    {
        int Id { get; set; }

        string Desktop { get; set; }

        string Sid { get; set; }
    }
}