using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuBuilder
{
    public class DatabaseMessage
    {
        public string Message { get; set; }
        public BuilderMessageType MessageType { get; set; }

        public enum BuilderMessageType
        {
            Information,
            Warning,
            Error
        }
    }

    /// Callback method to logg messages from the builder
    /// </summary>
    /// <param name="msg">Message to log</param>
    public delegate void DatabaseLogger(DatabaseMessage msg);
}
