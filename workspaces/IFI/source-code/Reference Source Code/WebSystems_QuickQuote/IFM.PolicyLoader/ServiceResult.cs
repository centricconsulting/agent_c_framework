using QuickQuote.CommonObjects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.PolicyLoader
{
    public class ServiceResult<T>// where T:class
    {
        public bool Success { get; set; } = false;
        public T Data { get; set; }
        public IEnumerable<string> Messages => _messages;

        public ServiceResult<Object> Previous { get; set; }

        public ServiceResult<Object> Next { get; set; }

        public void AddMessage(string msg)
        {
            _messages.Add(msg);
        }

        public void AddMessages(IEnumerable<string> messageList)
        {
            _messages.AddRange(messageList);
        }

        private readonly List<string> _messages = new List<string>();

        public ServiceResult<object> AsObjectResult()
        {
            var retval = new ServiceResult<object>
            {
                Data = this.Data,
                Previous = this.Previous,
                Success = this.Success
            };

            retval.AddMessages(this.Messages);
            return retval;
        }
    }

}
