using System;
//using Mapster;
using IFM.PrimitiveExtensions;
using QuickQuote.CommonObjects;
using IFM.DataServicesCore.BusinessLogic;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Name : ModelBase
    {
        public string DisplayName
        {
            get { return this.TypeId == 1 ? $"{this.FirstName} {this.LastName}".Trim() : this.CommercialName; }
        }

        public string FullDisplayName
        {
            get { return this.TypeId == 1 ? $"{this.PrefixName} {this.FirstName} {this.MiddleName} {this.LastName} {this.SuffixName}".Trim().Replace("  ", " ") : this.CommercialName; }
        }

        public string TaxNumber { get; set; }
        public Int32 TypeId { get; set; }

        public bool IsPersonalName
        {
            get { return this.TypeId == 1; }
            set { }
        }

        public DateTime BirthDate { get; set; }
        public string DLN { get; set; }
        private DateTime _DLDate = DateTime.MinValue;
        public DateTime DLDate {
            get
            {
                return _DLDate;
            }
            set
            {
                _DLDate = value;
                if (DLDate.IsNotNull())
                {
                    if ((DateTime.Now - DLDate).TotalDays >= (3 * 365))
                    {
                        _licensedForThreeOrMoreYears = true;
                    }
                    else
                    {
                        _licensedForThreeOrMoreYears = false;
                    }
                }
            }
        }
        public Int32 DLStateId { get; set; }
        public string DLState { get; set; }
        private bool _licensedForThreeOrMoreYears;
        public bool LicensedForThreeOrMoreYears
        {
            get
            {
                return _licensedForThreeOrMoreYears;
            }
            set
            {
                _licensedForThreeOrMoreYears = value;
                if(DLDate == DateTime.MinValue && BirthDate.IsNotNull() && BirthDate != DateTime.MinValue && BirthDate != DateTime.MaxValue)
                {
                    var sixteenthBirthday = BirthDate.AddYears(16);
                    if (_licensedForThreeOrMoreYears == true)
                    {
                        DLDate = sixteenthBirthday;
                    }
                    else
                    {
                        var twoYearsAgoBirthDay = BirthDate;
                        twoYearsAgoBirthDay = twoYearsAgoBirthDay.AddYears((DateTime.Now.AddYears(-2)).Year - BirthDate.Year);
                        if(sixteenthBirthday > twoYearsAgoBirthDay)
                        {
                            DLDate = sixteenthBirthday;
                        }
                        else
                        {
                            DLDate = twoYearsAgoBirthDay;
                        }
                    }
                }
            }
        }
        public string PrefixName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SuffixName { get; set; }

        public string DbaName { get; set; }

        public Int32 MartialStatusId { get; set; }
        public string MartialStatus { get; set; }

        public Int32 SexId { get; set; }
        public string Sex { get; set; }

        //[AdaptMember("CommercialName1")]
        public string CommercialName { get; set; }

        public Name() { }

        internal Name(DCO.Name dName, bool noTaxInfo)
        {
            if (dName != null)
            {
                this.TaxNumber = noTaxInfo == false ? (string.IsNullOrWhiteSpace(dName.TaxNumber) == false) ? dName.TaxNumber.Trim() : string.Empty : string.Empty;
                this.TypeId = dName.TypeId;
                this.BirthDate = dName.BirthDate;
                this.DLDate = dName.DLDate.DateTime;
                this.DLN = dName.DLN;
                this.DLStateId = (int)dName.DLStateId;
                this.DLState = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.DLStateId.ToString());
                this.PrefixName = dName.PrefixName;
                this.FirstName = dName.FirstName;
                this.MiddleName = dName.MiddleName;
                this.LastName = dName.LastName;
                this.SuffixName = dName.SuffixName;
                this.MartialStatusId = dName.MaritalStatusId;
                this.MartialStatus = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.MaritalStatusId, this.MartialStatusId.ToString());
                this.SexId = dName.SexId;
                this.Sex = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.SexId, this.SexId.ToString());
                this.CommercialName = dName.CommercialName1;
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        internal Name(QuickQuoteName qqName, bool noTaxInfo)
        {
            if (qqName != null)
            {
                this.TaxNumber = noTaxInfo == false ? (string.IsNullOrWhiteSpace(qqName.TaxNumber) == false) ? qqName.TaxNumber.Trim() : string.Empty : string.Empty;
                this.TypeId = qqName.TypeId.TryToGetInt32();
                this.BirthDate = qqName.BirthDate.ToDateTime();
                this.DLDate = qqName.DriversLicenseDate.ToDateTime();
                this.DLN = qqName.DriversLicenseNumber;
                this.DLStateId = qqName.DriversLicenseStateId.TryToGetInt32();
                this.DLState = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.DLStateId.ToString());
                this.PrefixName = qqName.PrefixName;
                this.FirstName = qqName.FirstName;
                this.MiddleName = qqName.MiddleName;
                this.LastName = qqName.LastName;
                this.SuffixName = qqName.SuffixName;
                this.MartialStatusId = qqName.MaritalStatusId.TryToGetInt32();
                this.MartialStatus = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.MaritalStatusId, this.MartialStatusId.ToString());
                this.SexId = qqName.SexId.TryToGetInt32();
                this.Sex = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.SexId, this.SexId.ToString());
                this.CommercialName = qqName.CommercialName1;
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        public bool DoesEquateTo(string nameText)
        {
            if (nameText == null)
                return false;

            if (this.TypeId == 1)
            {
                nameText = nameText.ToAlphabetic().ToLower();
                string middleInitial = (this.MiddleName.IsNullEmptyOrWhitespace() == false) ? this.MiddleName.Trim()[0].ToString() : "";

                if (nameText == $"{this.FirstName} {this.LastName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.FirstName} {this.MiddleName} {this.LastName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.FirstName} {middleInitial} {this.LastName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.PrefixName} {this.FirstName} {this.MiddleName} {this.LastName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.PrefixName} {this.FirstName} {this.MiddleName} {this.LastName} {this.SuffixName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.PrefixName} {this.FirstName} {middleInitial} {this.LastName} {this.SuffixName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.FirstName} {this.MiddleName} {this.LastName} {this.SuffixName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.FirstName} {middleInitial} {this.LastName} {this.SuffixName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.PrefixName} {this.FirstName} {this.MiddleName} {this.LastName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.PrefixName} {this.FirstName} {this.LastName} {this.SuffixName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.FirstName} {this.LastName} {this.SuffixName}".ToAlphabetic().ToLower())
                    return true;
                if (nameText == $"{this.PrefixName} {this.FirstName} {this.LastName}".ToAlphabetic().ToLower())
                    return true;

                return false;
            }
            else
            {
                if (nameText.ToAlphabetic().ToLower() == this.CommercialName.ToAlphabetic().ToLower())
                    return true;

                if (nameText.PadRight(1,' ').ToLower().RemoveAny(" inc "," inc. ").ToAlphabetic() == this.CommercialName.PadRight(1, ' ').ToLower().RemoveAny(" inc "," inc. ").ToAlphabetic())
                    return true;

                return false;
            }
        }

        public void FillInIdInfo()
        {
            if(this.DLStateId.HasValue()) this.DLState = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StateId, this.DLStateId.ToString());
            if(this.MartialStatusId.HasValue()) GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.MaritalStatusId, this.MartialStatusId.ToString());
            if (this.SexId.HasValue()) this.Sex = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.SexId, this.SexId.ToString());
        }

        public QuickQuoteName ConvertToQuickQuoteName()
        {
            return MPObjToQQObj(new QuickQuoteName());
        }

        public QuickQuoteName UpdateQuickQuoteName(QuickQuoteName NameToUpdate = null)
        {
            NameToUpdate = NameToUpdate.NewIfNull();
            return MPObjToQQObj(NameToUpdate);
        }

        private QuickQuoteName MPObjToQQObj(QuickQuoteName QQName)
        {
            if(this.TaxNumber.HasValue()) QQName.TaxNumber = this.TaxNumber;
            if(this.TypeId.HasValue()) QQName.TypeId = this.TypeId.ToString();
            if(this.BirthDate.HasValue()) QQName.BirthDate = this.BirthDate.ToString();
            if(this.DLDate.HasValue())QQName.DriversLicenseDate = this.DLDate.ToShortDateString();
            if(this.DLN.HasValue()) QQName.DriversLicenseNumber = this.DLN;
            if(this.DLStateId.HasValue()) QQName.DriversLicenseStateId = this.DLStateId.ToString();
            if(this.PrefixName.HasValue()) QQName.PrefixName = this.PrefixName;
            if(this.FirstName.HasValue()) QQName.FirstName = this.FirstName;
            if(this.MiddleName.HasValue()) QQName.MiddleName = this.MiddleName;
            if(this.LastName.HasValue()) QQName.LastName = this.LastName;
            if(this.SuffixName.HasValue()) QQName.SuffixName = this.SuffixName;
            if(this.MartialStatusId.HasValue()) QQName.MaritalStatusId = this.MartialStatusId.ToString();
            if(this.SexId.HasValue()) QQName.SexId = this.SexId.ToString();
            if(this.CommercialName.HasValue()) QQName.CommercialName1 = this.CommercialName;
            //FillInIdInfo();
            return QQName;
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}