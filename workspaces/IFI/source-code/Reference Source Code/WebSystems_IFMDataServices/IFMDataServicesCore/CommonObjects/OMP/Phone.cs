using QuickQuote.CommonObjects;
using System;
using IFM.PrimitiveExtensions;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Phone : ModelBase
    {
        public string Number { get; set; }
        public Int32 Extension { get; set; }
        public Int32 TypeId { get; set; }
        public string Type { get; set; }
        public Phone() { }

        internal Phone(DCO.Phone DPhone)
        {
            this.Number = DPhone.Number;
            this.Extension = DPhone.Extension;
            this.TypeId = (int)DPhone.TypeId;
            this.Type = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, this.TypeId.ToString());
        }

        internal Phone(QuickQuotePhone qqPhone)
        {
            this.Number = qqPhone.Number;
            this.Extension = qqPhone.Extension.TryToGetInt32();
            this.TypeId = qqPhone.TypeId.TryToGetInt32();
            this.Type = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePhone, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, this.TypeId.ToString());
        }

        public QuickQuotePhone UpdateQuickQuotePhone(QuickQuotePhone UpdatedPhone = null)
        {
            if (UpdatedPhone == null) { UpdatedPhone = new QuickQuotePhone(); }
            if (this.Number.HasValue()) { UpdatedPhone.Number = this.Number; }
            if (this.Extension.HasValue()) { UpdatedPhone.Extension = this.Extension.ToString(); }
            if (this.TypeId.HasValue()) { UpdatedPhone.TypeId = this.TypeId.ToString(); }
            if (this.Type.HasValue()) { UpdatedPhone.Type = this.Type; }
            return UpdatedPhone;
        }

        public override string ToString()
        {
            return Number;
        }
    }
}