using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InsuresoftServiceProxyGenerator.Generator
{
    static class DiamondComments
    {
        static List<XElement> members = null;

        static DiamondComments()
        {
            XElement comments = XElement.Load(Form1.AssemblyFolder + "Diamond.Common.Services.xml");
            members = (from c in (from m in comments.Elements("members") select m).Elements("member") select c).ToList();
        }

        public static string GetCodeCommentForClass(string typename)
        {
            StringBuilder codeCommentText = new StringBuilder();
            var comment = (from c in members where c.Attribute("name")?.Value == $"T:{typename}" select c).FirstOrDefault();
            if (comment != null)
            {
                foreach (var i in comment.Descendants())
                {
                    codeCommentText.Append(@"///" + i.ToString());
                }
            }
            return codeCommentText.ToString().Replace("\r\n", string.Empty);
        }

        public static string GetCodeCommentForMethod(string methodName)
        {
            StringBuilder codeCommentText = new StringBuilder();
            var comment = (from c in members where c.Attribute("name")?.Value != null ? c.Attribute("name").Value.StartsWith($"M:{methodName}") : false select c).FirstOrDefault();
            if (comment != null)
            {
                foreach (var i in comment.Descendants())
                {
                    codeCommentText.Append(@"///" + i.ToString());
                }
            }
            return codeCommentText.ToString().Replace("\r\n", string.Empty); ;
        }

    }
}
