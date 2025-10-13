using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using IFM.PrimitiveExtensions;
using IFM.DataServicesCore.BusinessLogic;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class AdditionalInterest : ModelBase
    {
        public Name Name { get; set; }
        public Int32 AdditionalInterestNum { get; set; }
        public Address Address { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<string> Emails { get; set; }
        public List<Phone> Phones { get; set; }
        public Int32 AdditionalInterestListId { get; set; }
        public Int32 AdditionalInterestListGroupTypeId { get; set; }
        public Int32 TypeId { get; set; }
        public string Type { get; set; }

        public AdditionalInterest() { }
        public AdditionalInterest(DCO.Policy.AdditionalInterest dAi)
        {
            if (dAi != null)
            {
                if (dAi.AdditionalInterestList != null)
                {
                    CopyAdditionalInterestListInfo(this, dAi.AdditionalInterestList);
                    this.TypeId = dAi.AdditionalInterestTypeId;
                    this.AdditionalInterestNum = dAi.AdditionalInterestNum;
                }
            }
        }

        public AdditionalInterest(DCO.Policy.AdditionalInterestList dAi)
        {
            if (dAi != null)
            {
                if (dAi != null)
                {
                    CopyAdditionalInterestListInfo(this, dAi);
                }
            }
        }

        public AdditionalInterest(DCO.Policy.AdditionalInterestList dAi, int typeId)
        {
            if (dAi != null)
            {
                if (dAi != null)
                {
                    CopyAdditionalInterestListInfo(this, dAi);
                    this.TypeId = typeId;
                }
            }
        }

        public AdditionalInterest(DCO.Policy.AdditionalInterestList dAi, int typeId, int aiNum)
        {
            if (dAi != null)
            {
                if (dAi != null)
                {
                    CopyAdditionalInterestListInfo(this, dAi);
                    this.TypeId = typeId;
                    this.AdditionalInterestNum = aiNum;
                }
            }
        }

        public AdditionalInterest(QuickQuoteAdditionalInterest qqAi)
        {
            if (qqAi != null)
            {
                CopyAdditionalInterestListInfo(this, qqAi.List);
                this.TypeId = qqAi.TypeId.TryToGetInt32();
                this.AdditionalInterestNum = qqAi.Num.TryToGetInt32();
            }
        }

        public AdditionalInterest(QuickQuoteAdditionalInterestList qqAil)
        {
            if (qqAil != null)
            {
                CopyAdditionalInterestListInfo(this, qqAil);
            }
        }

        public AdditionalInterest(QuickQuoteAdditionalInterestList qqAil, int typeID)
        {
            if (qqAil != null)
            {
                CopyAdditionalInterestListInfo(this, qqAil);
                this.TypeId = typeID;
                this.AdditionalInterestNum = 0;
            }
        }

        public AdditionalInterest(QuickQuoteAdditionalInterestList qqAil, int typeID, int aiNum)
        {
            if (qqAil != null)
            {
                CopyAdditionalInterestListInfo(this, qqAil);
                this.TypeId = typeID;
                this.AdditionalInterestNum = aiNum;
            }
        }

        private void CopyAdditionalInterestListInfo(AdditionalInterest thisAI, DCO.Policy.AdditionalInterestList dAi)
        {
            thisAI.Name = new Name(dAi.Name, false);
            thisAI.Address = new Address(dAi.Address);
            if(dAi.AddedDate != null && dAi.AddedDate.Value.HasValue())
            {
                thisAI.AddedDate = dAi.AddedDate.Value;
            }
            if (dAi.LastModifiedDate != null && dAi.LastModifiedDate.Value.HasValue())
            {
                thisAI.LastModifiedDate = dAi.LastModifiedDate.Value;
            }
            if (dAi.Emails != null && dAi.Emails.Any())
            {
                thisAI.Emails = new List<string>();
                foreach (var email in dAi.Emails)
                {
                    thisAI.Emails.Add(email.Address);
                }
            }
            if (dAi.Phones != null && dAi.Phones.Any())
            {
                thisAI.Phones = new List<Phone>();
                foreach (var p in dAi.Phones)
                {
                    thisAI.Phones.Add(new Phone(p));
                }
            }
            thisAI.AdditionalInterestListGroupTypeId = dAi.AdditionalInterestGroupTypeId;
            //this.AdditionalInterestNum = dAi.AdditionalInterestNum;
            thisAI.AdditionalInterestListId = dAi.AdditionalInterestListId;
            //this.TypeId = dAi.type.AdditionalInterestTypeId;
            if (thisAI.TypeId.HasValue())
            {
                thisAI.Type = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, thisAI.TypeId.ToString());
            }
        }

        private void CopyAdditionalInterestListInfo(AdditionalInterest thisAI, QuickQuoteAdditionalInterestList qqAil)
        {
            thisAI.Name = new Name(qqAil.Name, false);
            thisAI.Address = new Address(qqAil.Address);
            if (qqAil.Emails != null && qqAil.Emails.Any())
            {
                thisAI.Emails = new List<string>();
                foreach (var email in qqAil.Emails)
                {
                    thisAI.Emails.Add(email.Address);
                }
            }
            if (qqAil.Phones != null && qqAil.Phones.Any())
            {
                thisAI.Phones = new List<Phone>();
                foreach (var p in qqAil.Phones)
                {
                    thisAI.Phones.Add(new Phone(p));
                }
            }
            thisAI.AdditionalInterestListGroupTypeId = qqAil.GroupTypeId.TryToGetInt32();
            //this.AdditionalInterestNum = qqAil.AdditionalInterestNum;
            thisAI.AdditionalInterestListId = qqAil.ListId.TryToGetInt32();
            //this.TypeId = qqAil.type.AdditionalInterestTypeId;
            if (thisAI.TypeId.HasValue())
            {
                thisAI.Type = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, thisAI.TypeId.ToString());
            }
        }

        public void FillInIdInfo()
        {
            if (this.TypeId.HasValue())
            {
                this.Type = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, this.TypeId.ToString());
            }
            Name.FillInIdInfo();
            Address.FillInIdInfo();
        }

        public QuickQuoteAdditionalInterest UpdateQuickQuoteAdditionalInterest(QuickQuoteAdditionalInterest aiToUpdate = null)
        {
            aiToUpdate = aiToUpdate.NewIfNull();
            aiToUpdate.Name = this.Name.UpdateQuickQuoteName(aiToUpdate.Name);
            aiToUpdate.Address = this.Address.UpdateQuickQuoteAddress(aiToUpdate.Address);
            //aiToUpdate.Emails = this.ConvertEmailListToQuickQuoteEmailList(); //Not currently editing in MP, lets leave them alone for now.
            //aiToUpdate.Phones = this.ConvertPhoneListToQuickQuotePhoneList(); //Not currently editing in MP, lets leave them alone for now.
            return MPObjToQQObj(aiToUpdate);
        }

        private QuickQuoteAdditionalInterest MPObjToQQObj(QuickQuoteAdditionalInterest QQai)
        {
            //if(this.AdditionalInterestNum.HasValue()) QQai.Num = this.AdditionalInterestNum.ToString(); //Even when we edit, we are deleting then adding so this shouldn't be needed.
            //if (this.AdditionalInterestListId.HasValue()) QQai.ListId = this.AdditionalInterestListId.ToString(); //We may not want to ever bring this over unless we found it in a lookup - in particular an edit scenario
            if (this.TypeId.HasValue())
            {
                QQai.TypeId = this.TypeId.ToString();
                QQai.TypeName = (QuickQuoteAdditionalInterest.AdditionalInterestType)this.TypeId;
            }
            return QQai;
        }
    }
}