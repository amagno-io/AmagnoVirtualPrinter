using System;

using JetBrains.Annotations;

using VirtualPrinter.Agent.Core.Interfaces;

namespace VirtualPrinter.Agent.Lib.Model
{
    public class ProgressUpdateArgs : EventArgs
    {
        public ProgressUpdateArgs([NotNull] IJob job, uint val)
        {
            Job = job;
            Value = val;
        }

        [NotNull]
        public IJob Job { get; }

        public uint Value { get; }
    }
}