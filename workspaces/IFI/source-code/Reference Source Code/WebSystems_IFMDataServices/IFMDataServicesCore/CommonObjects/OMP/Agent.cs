using System;
using System.Collections.Generic;
using System.Linq;
//using Mapster;
using DCO = Diamond.Common.Objects;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Agent : ModelBase
    {
        public Name Name { get; set; }
        public Address Address { get; set; }
        public DateTime CloseDate { get; set; }
        public List<string> Emails { get; set; }
        public List<AgencyProducer> AgencyProducers { get; set; }
        public List<Phone> Phones { get; set; }
        //[AdaptMember("Name.CommercialWeb")]
        public string WebSiteAddress { get; set; }
        public Int32 AgencyId { get; set; }
        //[AdaptMember("Code")]
        public string AgencyCode { get; set; }

        internal bool IsHomeOfficeAgency {
            get
            {
                return (!string.IsNullOrWhiteSpace(this.AgencyCode)) ? global::IFM.DataServicesCore.BusinessLogic.AppConfig.HomeOfficeAgencyCodes.Contains(this.AgencyCode) : false;
            }
         }

        public Agent() { }
        internal Agent(DCO.Policy.Image image)
        {
            if (image != null)
            {
                if (image.Agency != null)
                {
                    AgencyId = image.Agency.AgencyId;
                    AgencyCode = image.Agency.Code;

                    this.Name = new Name(image.Agency.Name, true);// MUST NEVER SEND TAX INFO
                    this.Address = new Address(image.Agency.Address);

                    this.CloseDate = image.Agency.CloseDate;
                    if (IsHomeOfficeAgency)
                        this.Emails = new List<string> { global::IFM.DataServicesCore.BusinessLogic.AppConfig.HomeOfficeAgencyEmailAddress };
                    else
                    {
                        if (image.Agency.Emails != null && image.Agency.Emails.Any())
                            this.Emails = (from e in image.Agency.Emails select e.Address).ToList();
                    }

                    foreach(var ap in image.Agency.AgencyProducers)
                    {
                        var myAP = new AgencyProducer(ap);
                        AgencyProducers = new List<AgencyProducer>();
                        AgencyProducers.Add(myAP);
                    }

                    this.Phones = new List<Phone>();
                    if (IsHomeOfficeAgency)
                    {
                        var p = new Phone();
                        p.Number = global::IFM.DataServicesCore.BusinessLogic.AppConfig.HomeOfficeAgencyPhoneNumber;
                        this.Phones.Add(p);
                    }
                    else
                    {
                        if (image.Agency.Phones != null && image.Agency.Phones.Any())
                        {
                            foreach (var p in image.Agency.Phones)
                            {
                                this.Phones.Add(new Phone(p));
                            }
                        }
                    }


                    if (IsHomeOfficeAgency)
                        this.WebSiteAddress = global::IFM.DataServicesCore.BusinessLogic.AppConfig.HomeOfficeAgencyWebSiteAddress;
                     else
                        this.WebSiteAddress = image.Agency.Name?.CommercialWeb;
                }
                else
                {
#if !DEBUG
                    global::IFM.IFMErrorLogging.LogIssue($"Agency was null. - Policy #{((image != null) ? image.PolicyNumber : String.Empty)}", "IFMDATASERVICES -> Agent.cs -> Function Agent");
#else
                    Debugger.Break();
#endif
                }
            }
            else
            {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogIssue($"Image was null.", "IFMDATASERVICES - Agent.cs -> Function Agent");
#else
                Debugger.Break();
#endif
            }
        }


        public override string ToString()
        {
            return this.Name != null ? Name.ToString() : "Name is NUll";
        }
    }
}