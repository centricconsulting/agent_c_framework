using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFM.Common.InputValidation
{
    public class InputHelpers
    {        
        // ******************************************************************************************************************************************************************************
        // ******************************************************************************************************************************************************************************
        //  DO NOT ADD ANY ADDITIONAL REFERENCES TO THIS LIBRARY - WE WANT IT TO BE PORTABLE BECAUSE ONCE YOU START TO USE THESE YOU WILL WANT THEM EVERYWHERE
        //  IN ORDER FOR IT TO BE PORTABLE DONT BIND IT TO THINGS LIKE DIAMOND EITHER DIRECTLY OR INDIRECTLY BY ADDING REFERENCE TO SOMETHING THAT HAS DIAMOND LIBS AS A DEPENDENCY
        // ******************************************************************************************************************************************************************************
        // ******************************************************************************************************************************************************************************

        public const string DiamondDefaultDate_string = "1/1/1800";

        public static double TryToGetDouble(string inputText)
        {
            double d = 0;
            if (string.IsNullOrWhiteSpace(inputText) == false)
            {
                inputText = CleanInputForDoubleConversion(inputText);

                double.TryParse(inputText, out d);
            }
            return d;
        }

        public static Int32 TryToGetInt32(string inputText)
        {            
            return Convert.ToInt32(TryToGetDouble(inputText));
        }

        public static Int64 TryToGetInt64(string inputText)
        {
            Int64 d = 0;
            if (string.IsNullOrWhiteSpace(inputText) == false)
            {
                inputText = CleanInputForDoubleConversion(inputText);
                inputText = inputText.Split('.')[0];
                Int64.TryParse(inputText, out d);
            }
            return d;
        }


        public static double TryToSum(string value1, string value2)
        {
            return TryToGetDouble(value1) + TryToGetDouble(value2);
        }

        public static string TryToFormatAsCurrency(string inputText, bool showCents = true)
        {
            double dolVal = TryToGetDouble(inputText);
            if (showCents)
            {
                return string.Format("{0:C2}", dolVal);
            }
            else
            {
                return string.Format("{0:C0}", dolVal);
            }
        }

        private static string CleanInputForDoubleConversion(string inputText)
        {
            if (string.IsNullOrWhiteSpace(inputText) == false)
            {
                inputText = inputText.Replace("$", "");
                inputText = inputText.Replace(",", "");
                inputText = inputText.Replace("%", "");
                return inputText.Trim();
            }
            return inputText;
        }

        public static string RemovePossibleDefaultedNumericValue(string text)
        {
            if (text.Trim() == "0")
            {
                return "";
            }
            return text;
        }

        public static string RemovePossibleDefaultedDateValue(string text)
        {
            if (Information.IsDate(text))
            {
                if (Convert.ToDateTime(text).ToShortDateString() == Convert.ToDateTime(DiamondDefaultDate_string).ToShortDateString())
                {
                    return "";
                }
            }
            //just keep what is there now
            return text;
        }

        public static List<string> CSVtoList(string csvText)
        {
            if (string.IsNullOrWhiteSpace(csvText) == false && csvText.Contains(","))
                return csvText.Split(Convert.ToChar(",")).ToList();
            return new List<string>();//return empty list
        }

        public static string ListToCSV(List<string> lst)
        {
            string text = "";
            foreach (var st in lst)
            {
                text += st + ",";
            }
            return text.Trim().TrimEnd(',');
        }

        public const string EllipsisSuffix = "...";

        public static string EllipsisText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                if (text.Length > Math.Abs(maxLength))
                {
                    return text.Substring(0, Math.Abs(maxLength)) + EllipsisSuffix;
                }
                else
                {
                    return text;
                }
            }
            return "";
        }

        public static bool StringHasNumericValue(string s)
        {
            return string.IsNullOrWhiteSpace(s) != true && Microsoft.VisualBasic.Information.IsNumeric(s);
        }

        public static bool StringHasAnyValue(string s)
        {
            return string.IsNullOrWhiteSpace(s) != true;
        }
    }
}