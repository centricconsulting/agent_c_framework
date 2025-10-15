using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace IFM.Configuration.Extensions
{
    public static class AppSettings
    {
        public static T As<T>(this NameValueCollection nvc, string key)
            where T : struct
        {
            T retval = default(T);
            try
            {
                retval = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(nvc.Get(key));
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Flag key {key} was not found or is not convertable to {typeof(T)}", ex);
            }

            return retval;
        }
        public static T As<T>(this NameValueCollection nvc, string key, T notFoundValue)
            where T : struct
        {
            T retval = notFoundValue;

            try
            {
                if (nvc.AllKeys.Contains(key))
                {
                    retval = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(nvc.Get(key));
                }             
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Flag key {key} was not found or is not convertable to {typeof(T)}", ex);
            }

            return retval;
        }

        public static T ConstructWith<T>(this NameValueCollection nvc, string key)
           where T : class
        {
            T retval = default(T);
            try
            {
                retval = (T)TypeDescriptor.CreateInstance(null, typeof(T), new Type[] { typeof(string) }, new object[] { nvc.Get(key) });
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Flag key {key} was not found or is a constructor of {typeof(T)} could not be found that accepts the config value", ex);
            }

            return retval;
        }
        public static T ConstructWith<T>(this NameValueCollection nvc, string key, T notFoundValue)
           where T : class
        {
            T retval = notFoundValue;
            try
            {
                if (nvc.AllKeys.Contains(key))
                {
                    retval = (T)TypeDescriptor.CreateInstance(null, typeof(T), new Type[] { typeof(string) }, new object[] { nvc.Get(key) });
                }
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Flag key {key} was not found or is a constructor of {typeof(T)} could not be foundthat accepts the config value", ex);
            }

            return retval;
        }
        public static T ConstructWithAs<T, U>(this NameValueCollection nvc, string key)
           where T : class
            where U : struct
        {
            T retval = default(T);
            try
            {
                retval = (T)TypeDescriptor.CreateInstance(null, typeof(T), new Type[] { typeof(U) }, new object[] { nvc.As<U>(key) });
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Flag key {key} was not found or is a constructor of {typeof(T)} could not be foundthat accepts the config value", ex);
            }

            return retval;
        }

        public static IList<T> CsvAsList<T>(this NameValueCollection nvc, string key, params string[] separators)
        {

            List<T> retval = new List<T>();
            separators = separators ?? new string[] { "," };

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));

                string listAsString = nvc.Get(key);

                if (!string.IsNullOrWhiteSpace(listAsString))
                {
                    var tmpList = listAsString.Split(separators, StringSplitOptions.RemoveEmptyEntries); //in .net 5, we can trim at split too
                    var convertedList = tmpList.Select(str => (T)converter.ConvertFromString(str));
                    retval.AddRange(convertedList);
                }
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException($"Flag key {key} was not found or is not convertable to {typeof(T)}", ex);
            }

            return retval;
        }
    }
}
