using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using JetBrains.Annotations;

using NamedPipeWrapper;

using VirtualPrinter.ProgressInfo.Lib.Interfaces;

namespace VirtualPrinter.ProgressInfo.Lib
{
    public class ProgressInfoServerFactory : IProgressInfoServerFactory
    {
        private const string PipeName = "vdpagent";

        [NotNull]
        private readonly Func<NamedPipeServer<Core.Message.Message>, IProgressInfoServer> _factorInfoServer;

        public ProgressInfoServerFactory
        (
            [NotNull]Func<NamedPipeServer<Core.Message.Message>, IProgressInfoServer> factorInfoServer
        )
        {
            _factorInfoServer = factorInfoServer;
        }

        public IProgressInfoServer Create()
        {
            var namedPipeServer = new NamedPipeServer<Core.Message.Message>(PipeName, GetPipeSecurity());

            return _factorInfoServer(namedPipeServer);
        }

        [NotNull]
        private static PipeSecurity GetPipeSecurity()
        {
            var security = new PipeSecurity();

            security.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            security.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), PipeAccessRights.FullControl, AccessControlType.Allow));

            return security;
        }
    }
}
