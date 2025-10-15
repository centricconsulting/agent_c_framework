using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class StaticDataQuery
    {
        public string QueryTerm { get; set; }
        public List<global::IFM.DataServicesCore.CommonObjects.IFM.StaticData.StaticDataOption> Results { get; set; }

        private StaticDataQuery()
        {
        }

        public StaticDataQuery(string searchTerm)
        {
            this.QueryTerm = searchTerm != null ? searchTerm.ToLower().Trim() : string.Empty;
            ExecuteQuery();
        }

        private void ExecuteQuery()
        {
            if (this.Results == null)
            {
                var dotNotation = this.QueryTerm;
                if (dotNotation != null && dotNotation.Contains("."))
                {
                    var parts = dotNotation.Split('.');

                    string lobName = "";
                    string objectName = "";
                    string propertyName = "";

                    switch (parts.Count())
                    {
                        case 3:
                            lobName = parts[0];
                            objectName = parts[1];
                            propertyName = parts[2];
                            break;

                        case 2:
                            lobName = QuickQuoteObject.QuickQuoteLobType.AutoPersonal.ToString();
                            objectName = parts[0];
                            propertyName = parts[1];
                            break;
                    }

                    this.Results = GetStaticData(lobName, objectName, propertyName);
                }
            }
        }

        private List<global::IFM.DataServicesCore.CommonObjects.IFM.StaticData.StaticDataOption> GetStaticData(string lobName, string className, string propertyName)
        {
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();
            className = className != null ? className.Trim() : string.Empty;
            if (className.StartsWith("quickquote") == false)
            {
                className = "quickquote" + className;
            }

            propertyName = propertyName != null ? propertyName.Trim() : string.Empty;
            lobName = lobName != null ? lobName.Trim() : string.Empty;

            //QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId
            try
            {
                var results = qqHelper.GetStaticDataOptions(
                    (QuickQuoteHelperClass.QuickQuoteClassName)Enum.Parse(typeof(QuickQuoteHelperClass.QuickQuoteClassName), className, true),
                    (QuickQuoteHelperClass.QuickQuotePropertyName)Enum.Parse(typeof(QuickQuoteHelperClass.QuickQuotePropertyName), propertyName, true),
                    (QuickQuoteObject.QuickQuoteLobType)Enum.Parse(typeof(QuickQuoteObject.QuickQuoteLobType), lobName, true),
                    QuickQuoteHelperClass.PersOrComm.Pers);

                return (
                    from result in results
                    select new global::IFM.DataServicesCore.CommonObjects.IFM.StaticData.StaticDataOption() {
                        Text = result.Text,
                        Value = result.Value
                    }
                    ).ToList();
            }
            catch 
            {
                //just ignore it
            }
            return new List<global::IFM.DataServicesCore.CommonObjects.IFM.StaticData.StaticDataOption>();
        }

        public static List<StaticDataQuery> QueryMultipleTerms(List<string> terms)
        {
            List<StaticDataQuery> lst = new List<StaticDataQuery>();
            if (terms != null)
            {
                foreach (var term in terms)
                {
                    lst.Add(new StaticDataQuery(term));
                }
            }
            return lst;
        }
    }
}