using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using cc = IFM_CreditCardProcessing;
//using common = IFM_CreditCardProcessing.Common;

namespace IFM.DataServicesCore.CommonObjects.Fiserv
{
    [System.Serializable]
    public class ModelBase
    {
        //protected IFM_CreditCardProcessing.CreditCardMethods creditCardMethods { get { return new IFM_CreditCardProcessing.CreditCardMethods(); } }
        private IFM_CreditCardProcessing.CreditCardMethods _creditCardMethods;
        protected IFM_CreditCardProcessing.CreditCardMethods creditCardMethods
        {
            get
            {
                if (_creditCardMethods == null)
                {
                    _creditCardMethods = new IFM_CreditCardProcessing.CreditCardMethods();
                }
                return _creditCardMethods;
            }
        }

        private BusinessLogic.Fiserv.GeneralHelper _helper;
        protected BusinessLogic.Fiserv.GeneralHelper helper
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new BusinessLogic.Fiserv.GeneralHelper();
                }
                return _helper;
            }
        }

    }
}
