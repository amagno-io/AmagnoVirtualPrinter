using System;

using JetBrains.Annotations;

using AmagnoVirtualPrinter.Agent.Core.Interfaces;

namespace AmagnoVirtualPrinter.Agent.Lib.Model
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