using IFM.Configuration.Extensions;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace IFM.VR.Flags
{
    internal class ListFlagImpl<T> where T : struct, IComparable<T>
    {
        private readonly string _configKey;
        private readonly string[] _separators;

        internal ListFlagImpl(string configKey, params string[] separators)
        {
            this._configKey = configKey;
            this._separators=separators??new string[] { "," };
        }
        public bool Contains(T testValue)
        {
            var list = ConfigurationManager.AppSettings.CsvAsList<T>(_configKey, _separators);
            return list.Contains(testValue);
        }

    }

    public delegate bool ListFlag<T>(T testValue);

    public class ListFlagFactory
    {
        public static CompareFlag<T> BuildFor<T>(string key, params string[] separators) where T : struct, IComparable<T>
        {
            var impl = new ListFlagImpl<T>(key);

            return impl.Contains;
        }
    }
}
