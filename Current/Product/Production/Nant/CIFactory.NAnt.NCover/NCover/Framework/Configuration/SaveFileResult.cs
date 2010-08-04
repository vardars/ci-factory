namespace NCover.Framework.Configuration
{
    using System;
    using System.Runtime.CompilerServices;

    public class SaveFileResult
    {
        public static readonly SaveFileResult SuccessfulSave = new SaveFileResult();

        private SaveFileResult()
        {
            this.Success = true;
        }

        public SaveFileResult(Exception thrownException)
        {
            if (thrownException == null)
            {
                throw new ArgumentNullException("thrownException");
            }
            this.Success = false;
            this.ThrownException = thrownException;
        }

        public bool Success { get; private set; }

        public Exception ThrownException { get; private set; }
    }
}
