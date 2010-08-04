namespace NCover.Framework.Views
{
    using NCover.Interfaces.View;
    using System;
    using System.Collections.Generic;

    public class RebasedPathEqualForInsertion : IEqualityComparer<CoverageViewRebasedPath>, IEqualityComparer<ICoverageViewRebasedPath>
    {
        public bool Equals(CoverageViewRebasedPath x, CoverageViewRebasedPath y)
        {
            return this.Equals((ICoverageViewRebasedPath) x, (ICoverageViewRebasedPath) y);
        }

        public bool Equals(ICoverageViewRebasedPath x, ICoverageViewRebasedPath y)
        {
            return x.SourcePath.Equals(y.SourcePath);
        }

        public int GetHashCode(CoverageViewRebasedPath obj)
        {
            return this.GetHashCode((ICoverageViewRebasedPath) obj);
        }

        public int GetHashCode(ICoverageViewRebasedPath obj)
        {
            return obj.SourcePath.GetHashCode();
        }
    }
}
