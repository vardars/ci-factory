namespace NCover.Framework.Views
{
    using NCover.Interfaces.View;
    using System;
    using System.Runtime.CompilerServices;

    public class CoverageViewRebasedPath : ICoverageViewRebasedPath, IEquatable<ICoverageViewRebasedPath>
    {
        public CoverageViewRebasedPath()
        {
            this.RebasedPath = string.Empty;
            this.SourcePath = string.Empty;
        }

        public CoverageViewRebasedPath(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                throw new ArgumentOutOfRangeException("from", "Cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentOutOfRangeException("to", "Cannot be null or empty.");
            }
            this.SourcePath = from.ToLowerInvariant();
            this.RebasedPath = to.ToLowerInvariant();
        }

        public bool Equals(ICoverageViewRebasedPath other)
        {
            if (other == null)
            {
                return false;
            }
            return this.SourcePath.IgnoreCaseEq(other.SourcePath);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ICoverageViewRebasedPath);
        }

        public override int GetHashCode()
        {
            return string.Format("{0}{1}", this.SourcePath ?? "NULL", this.RebasedPath ?? "NULL").GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("source:{0}  rebase:{1}", this.SourcePath ?? "NULL", this.RebasedPath ?? "NULL");
        }

        public string RebasedPath { get; set; }

        public string SourcePath { get; set; }
    }
}
