using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace IFI.Integrations.Response
{
    public class WtwSis
    {
        public double allPerils { get; set; }
        public double fire { get; set; }
        public double liability { get; set; }
        public double other { get; set; }
        public double theft { get; set; }
        public double water { get; set; }
        public double weather { get; set; }
    }
}
