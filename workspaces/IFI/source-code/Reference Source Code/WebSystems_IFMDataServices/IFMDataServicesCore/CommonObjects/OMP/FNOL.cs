using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCE = Diamond.Common.Enums;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class FNOL : ModelBase
    {
        public FNOL(int fnolUserId, int fnolPolicyID, int fnolPolicyImage, DateTime fnolLossDate, string fnolDescription, 
            DCE.Claims.SaveAttempt fnolSaveAttempt, DCE.Claims.ClaimLnStatusType fnolStatusType, DCE.Claims.ClaimType fnolClaimType, 
            CommonObjects.Enums.Enums.ClaimLossType fnolClaimLossType, CommonObjects.Enums.Enums.ClaimFaultType fnolClaimFaultType, 
            string fnolInsuredFirstName, string fnolInsuredLastName, string fnolClaimLossLocation)
        {
            SaveAttempt = fnolSaveAttempt;
            StatusType = fnolStatusType;
            UserID = fnolUserId;
            LossDate = fnolLossDate;
            ClaimType = fnolClaimType;
            PolicyID = fnolPolicyID;
            PolicyImage = fnolPolicyImage;
            Description = fnolDescription;
            ClaimLossType =(Enums.Enums.FNOLClaimLossType)fnolClaimLossType;
            ClaimFaultType = fnolClaimFaultType;

            insuredFirstName = fnolInsuredFirstName;
            insuredLastName = fnolInsuredLastName;
            ClaimLocation = fnolClaimLossLocation;
        }
        public FNOL(int fnolUserId, int fnolPolicyID, int fnolPolicyImage, DateTime fnolLossDate, string fnolDescription, 
            DCE.Claims.SaveAttempt fnolSaveAttempt, DCE.Claims.ClaimLnStatusType fnolStatusType, 
            DCE.Claims.ClaimType fnolClaimType, CommonObjects.Enums.Enums.ClaimLossType fnolClaimLossType)
        {
            SaveAttempt = fnolSaveAttempt;
            StatusType = fnolStatusType;
            UserID = fnolUserId;
            LossDate = fnolLossDate;
            ClaimType = fnolClaimType;
            PolicyID = fnolPolicyID;
            PolicyImage = fnolPolicyImage;
            Description = fnolDescription;
            ClaimLossType = (Enums.Enums.FNOLClaimLossType)fnolClaimLossType;
        }
        public FNOL() { }
        //public string lossAddressCity { get; set; }
        //public string lossAddressState { get; set; }
        //public string lossAddressZip { get; set; }
        public Address LossAddress { get; set; }
        //public string insuredHomePhone { get; set; }
        //public string insuredBusinessPhone { get; set; }
        //public string insuredMobilePhone { get; set; }
        //public string insuredHomeEmail { get; set; }
        //public string insuredBusinessEmail { get; set; }
        public int packagePartType { get; set; }
        public string lossAddressHouseNum { get; set; }
        public string claimOfficeID { get; set; }
        public string InsideAdjusterId { get; set; }
        public string AdministratorId { get; set; }
        public List<Witness> Witnesses { get; set; }
        public FNOL_Insured Insured { get; set; }
        //public Reporter Reporter { get; set; }
        //public string insuredOtherEmail { get; set; }
        public FNOL_Insured SecondInsured { get; set; }
        public DateTime ReportedDate { get; set; }
        public string ReportedBy { get; set; }
        public string WitnessRemarks { get; set; }
       
       // public string lossAddressStreetName { get; set; }
        public string insuredLastName { get; set; }
        public string PolicyVersionId { get; set; }
        public DCE.Claims.SaveAttempt SaveAttempt { get; set; }
        public DCE.Claims.ClaimLnStatusType StatusType { get; set; }
        public int UserID { get; set; }
        public DateTime LossDate { get; set; }
        public DCE.Claims.ClaimType ClaimType { get; set; }
        public string ClaimLocation { get; set; }
        public int PolicyImage { get; set; }
        public string Description { get; set; }
        public string PolicyStatus { get; set; }
        public int PolicyID { get; set; }
        public string PolicyNumber { get; set; }
        public CommonObjects.Enums.Enums.ClaimFaultType ClaimFaultType { get; set; }
        public string ClaimNumber { get; }
        public int ClaimControlId { get; }
        public List<FNOLVehicle> Vehicles { get; set; }
        public List<Property> Properties { get; set; }
        public List<Claimant> Claimants { get; set; }
        public string insuredFirstName { get; set; }
        public CommonObjects.Enums.Enums.FNOLClaimLossType ClaimLossType { get; set; }
        public CommonObjects.Enums.Enums.SeverityLoss ClaimSeverity_Id { get; set; }
        public OMP.Person ContactOther { get; set; }
        public OMP.Person VehicleOwner { get; set; }
        public OMP.Person VehicleDriver { get; set; }
        public string AgencyName { get; set; }
        public string AgencyPhone { get; set; }
        public string AgencyFax { get; set; }
        public CommonObjects.Enums.Enums.FNOL_LOB_Enum FNOLType { get; set; }
        public string EntryUser { get; set; }
        public List<FNOLAttachement> FNOLAttachements { get; set; }
        public  CommonObjects.OMP.Person OtherContactInfo { get; set; }
        public bool MedicalAttention { get; set; }
        public int VersionId { get; set; }

    }

    [System.Serializable]
    public class FNOL_Insured : OMP.Person
    {

    }
    [System.Serializable]
    public class FNOLAttachement
    {
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        
    }

}
