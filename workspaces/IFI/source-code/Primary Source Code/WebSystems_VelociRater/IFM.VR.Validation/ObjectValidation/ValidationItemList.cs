using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation
{
    public class ValidationItemList : Collection<ValidationItem>
    {
        private string validationListID = "";

        public string ValidationListID { get { return validationListID; } }

        private List<ValidationBreadCrum> breadCrums = new List<ValidationBreadCrum>();

        public System.Collections.ObjectModel.ReadOnlyCollection<ValidationBreadCrum> BreadCrums
        {
            get { return new ReadOnlyCollection<ValidationBreadCrum>(breadCrums); }
        }

        public bool ListHasValidationId(string valID)
        {
            bool HasValItem = false;
            if (this.Any())
            {
                foreach (var v in this)
                {
                    if (v.FieldId == valID)
                    {
                        HasValItem = true;
                        break;
                    }
                }
            }
            return HasValItem;
        }

        public ValidationItemList(string ValidationListID)
            : base()
        {
            this.validationListID = ValidationListID;
        }

        internal void AddBreadCrum(ValidationBreadCrum bc)
        {
            this.breadCrums.Add(bc);
        }

        internal void AddBreadCrum(ValidationBreadCrum.BCType type, string value)
        {
            this.breadCrums.Add(new ValidationBreadCrum(type, value));
        }

        public ValidationItem GetValidationItemById(string valID)
        {
            if (this.Any())
            {
                foreach (var v in this)
                {
                    if (v.FieldId == valID)
                    {
                        return v;
                    }
                }
            }
            return null;
        }
    }
}