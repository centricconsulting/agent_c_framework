using System;
//using Mapster;
using QuickQuote.CommonObjects;
using IFM.PrimitiveExtensions;
using IFM.DataServicesCore.BusinessLogic;
using DCO = Diamond.Common.Objects;
using System.Text.RegularExpressions;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Address : ModelBase
    {
        public string Line1
        {
            get
            {
                string poBox = (string.IsNullOrWhiteSpace(PoBox) == false) ? "PO Box " + PoBox : String.Empty;
                //string aptNum = (string.IsNullOrWhiteSpace(AptNum) == false) ? "Apt " + AptNum : String.Empty;
                return $"{HouseNumber ?? String.Empty} {StreetName ?? String.Empty} {poBox} {AptNum}".Replace("  ", " ").Trim();
            }
        }

        public string Line2
        {
            get { return $"{City ?? String.Empty} {StateAbbrev ?? String.Empty} {Zip5 ?? String.Empty}"; }
        }

        private string _aptNum;

        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        //[AdaptMember("ApartmentNumber")]
        public string AptNum { 
            get 
            {
                return _aptNum;
            }
            set 
            {
                _aptNum = FixAPTNum(value);
            } 
        }
        //[AdaptMember("POBox")]
        public string PoBox { get; set; }
        public string City { get; set; }
        public Int32 StateId { get; set; }
        //[AdaptMember("StateName")]
        public string State { get; set; }
        //[AdaptMember("StateAbbreviation")]
        public string StateAbbrev { get; set; }
        public string Zip { get; set; }

        public string AddressOther { get; set; }

        public string Zip5
        {
            get { return Zip != null && Zip.Length >= 5 ? Zip.Substring(0, 5) : ""; }
        }

        public string County { get; set; }

        public Address() { }
        internal Address(DCO.Address dAddress)
        {
            if (dAddress != null)
            {
                // this does work but if the Diamond object was to change converting it yourself would let you know that a change has happened
                //dAddress.Adapt(this, typeof (DCO.Address), typeof (Address));

                this.HouseNumber = dAddress.HouseNumber;
                this.StreetName = dAddress.StreetName;
                this.AptNum = dAddress.ApartmentNumber;
                this.PoBox = dAddress.POBox;
                this.City = dAddress.City;
                this.StateId = dAddress.StateId;
                this.State = dAddress.StateName;
                this.StateAbbrev = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
                this.Zip = dAddress.Zip;
                this.County = dAddress.County;
            }
#if DEBUG
            else
            {
               // Debugger.Break();
            }
#endif
        }

        internal Address(QuickQuoteAddress qqAddress)
        {
            if (qqAddress != null)
            {
                // this does work but if the Diamond object was to change converting it yourself would let you know that a change has happened
                //dAddress.Adapt(this, typeof (DCO.Address), typeof (Address));

                this.HouseNumber = qqAddress.HouseNum;
                this.StreetName = qqAddress.StreetName;
                this.AptNum = qqAddress.ApartmentNumber;
                this.PoBox = qqAddress.POBox;
                this.City = qqAddress.City;
                this.StateId = qqAddress.StateId.TryToGetInt32();
                this.State = qqAddress.State;
                this.StateAbbrev = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
                this.Zip = qqAddress.Zip;
                this.County = qqAddress.County;
            }
#if DEBUG
            else
            {
                // Debugger.Break();
            }
#endif
        }

        private string FixAPTNum(string myAptNum)
        {
            if (myAptNum.HasValue())
            {
                string returnVar = myAptNum.Regex_MatchFirstOrDefault(@"^\d+[a-zA-Z]+$|^\d+$|^[a-zA-Z]$", ' ');
                if (returnVar.HasValue())
                {
                    return "APT/STE " + returnVar;
                }
                else
                {
                    return myAptNum;
                }
            }
            return "";
        }

        public void FillInIdInfo()
        {
            if (this.StateId.HasValue())
            {
                this.StateAbbrev = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
                this.State = GetStaticDataText2ForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
            }
            else if (this.State.HasValue())
            {
                this.StateId = GetStaticDataValueForText2AsInt(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.State);
                this.StateAbbrev = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
            }
            else if (this.StateAbbrev.HasValue())
            {
                this.StateId = GetStaticDataValueForTextAsInt(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateAbbrev);
                this.State = GetStaticDataText2ForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
            }
        }

        public QuickQuoteAddress UpdateQuickQuoteAddress(QuickQuoteAddress AddressToUpdate = null)
        {
            AddressToUpdate = AddressToUpdate.NewIfNull();
            return MPObjToQQObj(AddressToUpdate);
        }

        private QuickQuoteAddress MPObjToQQObj(QuickQuoteAddress QQAdress)
        {
            //if (this.Name.IsNotNull()) QQDriver.Name = this.Name.ConvertToQuickQuoteName();
            if (this.HouseNumber.HasValue()) QQAdress.HouseNum = this.HouseNumber;
            if (this.StreetName.HasValue()) QQAdress.StreetName = this.StreetName;
            if (this.AptNum.HasValue()) QQAdress.ApartmentNumber = this.AptNum;
            if (this.PoBox.HasValue()) QQAdress.POBox = this.PoBox;
            if (this.City.HasValue()) QQAdress.City = this.City;
            if (this.StateId.HasValue())
            {
                QQAdress.StateId = this.StateId.ToString();
                //if (this.State.HasValue())
                //    QQAdress.State = this.State;
                //else
                    //QQAdress.State = GetStaticDataText2ForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
            }
            else if (this.State.HasValue())
            {
                QQAdress.State = this.State;
                //QQAdress.StateId = GetStaticDataValueForText2(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.State);
            }
            else if (this.StateAbbrev.HasValue())
            {
                QQAdress.StateId = GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateAbbrev);
                //QQAdress.State = GetStaticDataText2ForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.StateId.ToString());
            }

            //this.StateAbbrev;
            if (this.Zip.HasValue()) QQAdress.Zip = this.Zip;
            if (this.County.HasValue()) QQAdress.County = this.County;
            return QQAdress;
        }

        override public string ToString()
        {
            return $"{Line1} {Line2}";
        }
    }
}