using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.PolicyLoader
{
    public class LoadPolicyResult<T> : ServiceResult<T> where T : class, new()
    {
        public bool PolicyLoaded { get; set; }
        public bool PassedValidation { get; set; }

        public List<string> TranslationMessages { get; set; } = new List<string>();
        
        public T PolicyRef
        {
            get => base.Data;
            set => base.Data = value;
        }
    }
}
