namespace Nightwolf.Regex
{
    /// <summary>
    /// Class that encapsulates an automatic reset flag.
    /// </summary>
    internal sealed class AutoResetFlag
    {
        /// <summary>Current value of the flag. This is a one-shot var that resets on read.</summary>
        private bool flag;

        /// <summary>Create a new one-shot flag</summary>
        public AutoResetFlag(bool value)
        {
            this.flag = value;
        }

        /// <summary>Set the one-shot flag.</summary>
        public void Set()
        {
            this.flag = true;
        }

        /// <summary>Get the current value of the flag. This resets on read.</summary>
        /// <returns>Value of the flag</returns>
        public bool Read()
        {
            var val = this.flag;
            this.flag = false;
            return val;
        }

        /// <summary>Peek the current value of the flag. This does not reset on read.</summary>
        /// <returns>Value of the flag</returns>
        public bool Peek()
        {
            return this.flag;
        }
    }
}
