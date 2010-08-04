namespace NCover.Interfaces.View
{
    using System;

    public interface ICoverageViewRebasedPath : IEquatable<ICoverageViewRebasedPath>
    {
        string RebasedPath { get; }

        string SourcePath { get; }
    }
}
