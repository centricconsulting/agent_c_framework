using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.DataServices.API.ResponseObjects.Common
{
    [System.Serializable]
    public class BaseResult
    {
        public APIResponseForClientModel APIResponseForClientModel;
        public MessagesList Messages { get; } = new MessagesList();
        public MessagesList DetailedErrorMessages { get; } = new MessagesList();

        public bool HasErrors
        {
            get
            {
                return (from m in Messages where m.MessageType == Enums.MessageType.ValidationMessage select m).Any();
            }
            set
            {
                // here just for serialization
                //#If DEBUG Then
                //                Throw New Exception("This is a readonly property.")
                //#End If
            }
        }

        public bool HasFullStopErrors
        {
            get
            {
                return (from m in Messages where m.MessageType == Enums.MessageType.ValidationMessage && m.MessageSeverityType == Enums.MessageSeverityType.FullStopError select m).Any();
            }
            set
            {
                // here just for serialization
                //#If DEBUG Then
                //                Throw New Exception("This is a readonly property.")
                //#End If
            }
        }
    }
}