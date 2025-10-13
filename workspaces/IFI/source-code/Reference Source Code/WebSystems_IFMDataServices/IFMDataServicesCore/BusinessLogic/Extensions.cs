using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace IFM.DataServicesCore.BusinessLogic
{
    public static class Extensions
    {
        public static string ToXml<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var xmlserializer = new XmlSerializer(typeof(T));
            var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter))
            {
                xmlserializer.Serialize(writer, value);
                return stringWriter.ToString();
            }
        }

        public static T NewIfNull<T>(this T t) where T : new()
        {
            if(t == null)
            {
                t = new T();
            }
            return t;
        }

        //public static string ScrubHTML(this string value)
        //{
        //    var step1 = System.Text.RegularExpressions.Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
        //    return System.Text.RegularExpressions.Regex.Replace(step1, @"\s{2,}", " ");
        //}
    }
}