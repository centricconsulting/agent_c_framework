using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.InternalExtensions
{
    internal static class Extensions
    {
        public static string AppendText(this string txt, string txtToAppend, string splitter = "", string titleForAppendedText = "")
        {
            if (string.IsNullOrWhiteSpace(txtToAppend) == false)
            {
                StringBuilder sb;
                if (string.IsNullOrWhiteSpace(txt) == false)
                {
                    sb = new StringBuilder(txt);
                    if (string.IsNullOrWhiteSpace(splitter) == false)
                    {
                        sb.Append(splitter);
                    }
                }
                else
                {
                    sb = new StringBuilder();
                }

                if (string.IsNullOrWhiteSpace(titleForAppendedText))
                {
                    sb.Append(titleForAppendedText);
                }
                sb.Append(txtToAppend);

                return sb.ToString().Trim();
            }
            else
            {
                return txt;
            }
        }
    }
}
