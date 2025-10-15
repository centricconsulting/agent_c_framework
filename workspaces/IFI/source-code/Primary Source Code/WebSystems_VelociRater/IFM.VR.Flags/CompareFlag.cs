using IFM.Configuration.Extensions;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace IFM.VR.Flags
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CompareFlagImpl<T> where T : struct
    {
        private readonly string _configKey;
        private readonly Comparison<T> _comparison;

        internal CompareFlagImpl(string configKey, Comparison<T> comparison = default)
        {
            this._configKey=configKey;
            this._comparison=comparison ?? CompareFlagComparisons.EqualTo<T>;
        }

        public bool Passes(T testValue)
        {
            T keyValue = ConfigurationManager.AppSettings.As<T>(_configKey);

            return _comparison.Invoke(keyValue, testValue);
        }
    }

    public delegate bool CompareFlag<T>(T testValue) where T : struct;
    public delegate bool Comparison<T>(T keyValue, T testValue) where T : struct;

    public class CompareFlagFactory
    {
        public static CompareFlag<T> BuildFor<T>(string key, Comparison<T> test) where T : struct
        {
            var impl = new CompareFlagImpl<T>(key, test);

            return impl.Passes;
        }
    }

    public static class CompareFlagComparisons
    {       
        public static bool EqualTo<T>(T k, T v) where T : struct 
        {
            return k.Equals(v);
        }
        public static bool Date_AtOrLater(DateTime k, DateTime v)
        {
            return v.Date.CompareTo(k.Date) >= 0;
        }        
    }
}
