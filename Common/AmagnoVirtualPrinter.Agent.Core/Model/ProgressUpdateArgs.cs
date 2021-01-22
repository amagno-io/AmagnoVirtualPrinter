using System;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Model
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