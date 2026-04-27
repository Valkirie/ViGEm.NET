using System;
using System.Runtime.Serialization;

namespace Nefarius.ViGEm.Client.Exceptions
{
    [Serializable]
    public class VigemDeviceNotReadyException : Exception
    {
        public VigemDeviceNotReadyException()
            : base() { }

        public VigemDeviceNotReadyException(string message)
            : base(message) { }

        public VigemDeviceNotReadyException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public VigemDeviceNotReadyException(string message, Exception innerException)
            : base(message, innerException) { }

        public VigemDeviceNotReadyException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected VigemDeviceNotReadyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
