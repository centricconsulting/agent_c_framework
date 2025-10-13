using Diamond.Common.ChoicePoint.Library.ProductTransactionRecords.Report.Ncf.PublicRecords;
using Diamond.Common.Enums;
using Diamond.Common.Services.Messages.ThirdPartyService.OrderMvr;
using DCO = Diamond.Common.Objects;
using DCS = Diamond.Common.Services;
using ID = Insuresoft.DiamondServices;
using IFM.DataServicesCore.CommonObjects.OMP;
using IFM.DataServicesCore.CommonObjects.OMP.PPA;
//using Mapster.Adapters;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IFM.PrimitiveExtensions;
using NameAddressContact;
using IFM.DataServicesCore.BusinessLogic.Diamond;
using System.Diagnostics;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class Endorsements : BusinessLogicBase
    {

        public static Endorsement ProcessEndorsement(Endorsement endorsement)
        {
            Endorsement ratedEndorsement = null;
            Diamond.Login.LoginNow(AppConfig.EndorsementUsername, AppConfig.EndorsementPassword);
            if (Diamond.Login.IsLoggedIN() == true)
            {
                switch (endorsement.TransactionType)
                {
                    case Endorsement.EndorsementTransactionType.Rate:
                        ratedEndorsement = ProcessEndorsementObjectType(endorsement);
                        break;
                    case Endorsement.EndorsementTransactionType.Finalize:
                        ratedEndorsement = FinalizeEndorsement(endorsement);
                        break;
                    case Endorsement.EndorsementTransactionType.Delete:
                        ratedEndorsement = DeleteEndorsement(endorsement);
                        break;
                }
            }
            else
            {
                ratedEndorsement = new Endorsement();
                ratedEndorsement.ErrorMessage = "Unable to login to Diamond";
                IFM.IFMErrorLogging.LogIssue("Unable to login to Diamond", $"IFMDataServices - Attempted to ProcessEndorsement() but could not login to Diamond using {HttpContext.Current.Session["DiamondUsername"]}.");
            }
            return ratedEndorsement;
        }

        private static Endorsement FinalizeEndorsement(Endorsement endorsement)
        {
            //Endorsement ratedEndorsement = new Endorsement();
            //ratedEndorsement.EndorsementStatus.FinalizeAttempted = true;
            endorsement.EndorsementStatus.FinalizeAttempted = true;
            QuickQuoteXML qqXML = new QuickQuoteXML();
            QuickQuoteObject qqe = LoadEndorsement(endorsement);
            //qqe.TransactionRemark = CreateRemarks(endorsement, qqe); //I believe this won't be needed as the remarks should already have been added before this.
            if (String.IsNullOrWhiteSpace(endorsement.ErrorMessage))
            {
                endorsement = DoFinalizeEndorsement(endorsement, qqe);
            }
            return endorsement;
        }

        private static Endorsement DoFinalizeEndorsement(Endorsement endorsement, QuickQuote.CommonObjects.QuickQuoteObject qqe)
        {
            QuickQuoteXML qqXML = new QuickQuoteXML();
            qqXML.FinalizeEndorsement(qqe, true, ref endorsement.EndorsementStatus.refErrorMessage, ref endorsement.EndorsementStatus.refFinalizeSuccessful, ref endorsement.EndorsementStatus.refPromoteAttempted, ref endorsement.EndorsementStatus.refPromoteSuccessful);
            endorsement.SetPropertiesByReferenceVars();
            if (endorsement.EndorsementStatus.FinalizeSuccessful == true)
            {
                endorsement = PromoteEndorsement(endorsement);
            }

            return endorsement;
        }

        public static Endorsement PromoteEndorsement(Endorsement endorsement)
        {
            if (Diamond.Login.IsLoggedIN() == false)
            {
                //Diamond.Login.LoginNow(AppConfig.PrintUserName, AppConfig.PrintUserPassword);
                Diamond.Login.LoginNow(AppConfig.EndorsementUsername, AppConfig.EndorsementPassword);
            }
            if (Diamond.Login.IsLoggedIN() == true)
            {
                if (endorsement != null && endorsement.PolicyId > 0 && endorsement.PolicyImageNum > 0 && endorsement.TransactionDate.IsDate())
                {
                    var errorMsgs = new List<string>();
                    endorsement.EndorsementStatus.PromoteAttempted = true;
                    var promoted = PolicyQuoteProcessing.IssueQuoteAndUpdateSTPDatabase(endorsement.PolicyId, endorsement.PolicyImageNum, CommonObjects.Enums.Enums.PolicyQuoteProcessingType.Change, CommonObjects.Enums.Enums.PolicyQuotingApplication.MemberPortal, endorsement.UserID, endorsement.Username, errorMsgs);
                    if (promoted && errorMsgs.Count == 0)
                        endorsement.EndorsementStatus.PromoteSuccessful = true;
                    else
                    {
                        endorsement.EndorsementStatus.PromoteSuccessful = false;
                        endorsement.ErrorMessage = string.Join("; ", errorMsgs);
                    }
                }
            }
            else
            {
                endorsement.ErrorMessage = "Unable to login to Diamond.";
            }
            return endorsement;
        }

        private static Endorsement DeleteEndorsement(Endorsement endorsement)
        {
            bool hasError = false;
            QuickQuoteXML qqXML = new QuickQuoteXML();
            QuickQuoteObject qqe = LoadEndorsement(endorsement);
            if (String.IsNullOrWhiteSpace(endorsement.ErrorMessage))
            {
                hasError = !qqXML.SuccessfullyDeletedPendingQuickQuoteEndorsementInDiamond(qqe, ref endorsement.EndorsementStatus.refErrorMessage);
                endorsement.SetErrorMessageByReferenceVar();
                endorsement.EndorsementStatus.DeleteEndorsementAttempted = true;
            }
            else
            {
                hasError = true;
            }

            if (hasError == false)
            {
                endorsement.EndorsementStatus.DeleteEndorsementSuccessful = true;
            }
            return endorsement;
        }

        private static Type GetObjectType(Endorsement endorsement)
        {
            switch (endorsement.ObjectType)
            {
                case Endorsement.EndorsementObjectType.Driver:
                    return typeof(Driver);
                case Endorsement.EndorsementObjectType.Lienholder:
                    return typeof(AdditionalInterest);
                case Endorsement.EndorsementObjectType.Vehicle:
                    return typeof(Vehicle);
                case Endorsement.EndorsementObjectType.MailingAddress:
                    return typeof(Address);
                case Endorsement.EndorsementObjectType.LoanLenderInfo:
                    return typeof(AdditionalInterest);
                case Endorsement.EndorsementObjectType.PayPlan:
                    return typeof(BillingInformation);
            }
            return null;
        }

        private static Endorsement ProcessEndorsementObjectType(Endorsement endorsement)
        {
            Endorsement ratedEndorsement = null;
            QuickQuoteObject qqe = StartOrLoadEndorsement(endorsement);
            if (qqe != null && String.IsNullOrWhiteSpace(endorsement.ErrorMessage))
            {
                ratedEndorsement = ProcessObject(qqe, endorsement);
            }
            else
            {
                ratedEndorsement = new Endorsement();
                if (String.IsNullOrWhiteSpace(endorsement.ErrorMessage))
                    if (qqe == null)
                        ratedEndorsement.ErrorMessage = "Unable to start endorsement quote.";
                    else
                        ratedEndorsement.ErrorMessage = "Unknown error occured.";
                else
                    ratedEndorsement.ErrorMessage = endorsement.ErrorMessage;
            }
            return ratedEndorsement;
        }

        private static QuickQuoteObject StartOrLoadEndorsement(Endorsement endorsement)
        {
            if (endorsement.PolicyImageNum > 0)
            {
                endorsement.EndorsementStatus.IsNewEndorsement = false;
                endorsement.EndorsementStatus.NeedsObjectAdded = false;
                return LoadEndorsement(endorsement);
            }
            else
            {
                endorsement.EndorsementStatus.IsNewEndorsement = true;
                endorsement.EndorsementStatus.NeedsObjectAdded = true;
                return StartEndorsement(endorsement);
            }
        }
        private static QuickQuote.CommonObjects.QuickQuoteObject StartEndorsement(Endorsement endorsement)
        {
            QuickQuoteXML qqXML = new QuickQuoteXML();
            CommonHelperClass chc = new CommonHelperClass();
            var qqeInput = new QuickQuoteEndorsementForPolicyIdAndTransactionDateInput();
            qqeInput.PolicyId = endorsement.PolicyId;
            qqeInput.PolicyImageNum = endorsement.PolicyImageNum;
            qqeInput.TransactionDate = endorsement.TransactionDate;
            qqeInput.DaysBack = chc.GetApplicationXMLSettingForInteger("Endorsements_TransactionDate_DaysBackAllowed", "Endorsements.xml");
            qqeInput.DaysForward = chc.GetApplicationXMLSettingForInteger("Endorsements_TransactionDate_DaysForwardAllowed", "Endorsements.xml");
            qqeInput.EndorsementOriginType = QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes.MemberPortal;
            qqeInput.EndorsementRemarks = CreateRemarks(endorsement);
            qqeInput.ValidateTransactionDate = true;
            qqeInput.ReturnExistingPendingQuickQuoteEndorsement = false;
            qqeInput.OnlyReturnPendingQuickQuoteEndorsementWhenDateMatches = true;
            qqeInput.IsBillingUpdate = endorsement.ObjectType == Endorsement.EndorsementObjectType.PayPlan ? true : false;
            qqeInput.TransactionSourceId = TransSource.Policyholder;

            var qqe = qqXML.NewQuickQuoteEndorsementForPolicyIdAndTransactionDate(qqeInput);

            if (qqeInput.ErrorMessage == "you already have a pending endorsement for that date" && qqeInput.LatestPendingEndorsementImageNum > 0)
            {
                endorsement.PolicyImageNum = qqeInput.LatestPendingEndorsementImageNum;
                endorsement.EndorsementStatus.IsNewEndorsement = false;
                endorsement.EndorsementStatus.CheckForAbondonedObject = true;
                endorsement.EndorsementStatus.NeedsObjectAdded = true;
                qqe = LoadEndorsement(endorsement);
            }
            else
            {
                endorsement.ErrorMessage = qqeInput.ErrorMessage;
                if (qqe != null && qqe.PolicyImageNum.IsNotNull())
                {
                    endorsement.PolicyImageNum = qqe.PolicyImageNum.TryToGetInt32();
                }
            }
            return qqe;
        }

        private static QuickQuoteObject LoadEndorsement(Endorsement endorsement)
        {
            QuickQuoteXML qqXML = new QuickQuoteXML();
            var qqe = qqXML.QuickQuoteEndorsementForPolicyIdAndImageNum(endorsement.PolicyId, endorsement.PolicyImageNum, ref endorsement.EndorsementStatus.refErrorMessage);
            endorsement.SetErrorMessageByReferenceVar();
            return qqe;
        }

        private static Endorsement ProcessObject(QuickQuoteObject qqe, Endorsement endorsement) //reference count is misleading, this is used but through a reflection call. That is why it shows up as 0 references.
        {
            Endorsement ratedEndorsement = null;
            InitialFillInIdInfo(endorsement);
            RemoveAbandonedObjects(qqe, endorsement);
            GetEndorsementReadyForEditingCurrentImageObjects(qqe, endorsement);
            endorsement.SetQQPolicyCoverages(qqe);
            switch (endorsement.ActionType)
            {
                case Endorsement.EndorsementActionType.Add:
                    if (endorsement.EndorsementStatus.IsNewEndorsement || endorsement.EndorsementStatus.NeedsObjectAdded)
                    {
                        ratedEndorsement = AddObject(qqe, endorsement);
                    }
                    else
                    {
                        ratedEndorsement = EditObject(qqe, endorsement);
                    }
                    break;
                case Endorsement.EndorsementActionType.Edit:
                    ratedEndorsement = EditObject(qqe, endorsement);
                    break;
                case Endorsement.EndorsementActionType.Delete:
                    ratedEndorsement = DeleteObject(qqe, endorsement);
                    break;
            }
            return ratedEndorsement;
        }
        private static void InitialFillInIdInfo(Endorsement endorsement)
        {
            if (endorsement.ActionType != Endorsement.EndorsementActionType.Delete)
            {
                switch (endorsement.ObjectType)
                {
                    case Endorsement.EndorsementObjectType.Driver:
                        foreach (var d in endorsement.Drivers)
                        {
                            d.FillInIdInfo();
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Vehicle:
                        foreach (var v in endorsement.Vehicles)
                        {
                            v.FillInIdInfo();
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Lienholder:
                        foreach (var v in endorsement.Vehicles)
                        {
                            foreach (var ai in v.AdditionalInterests)
                            {
                                ai.FillInIdInfo();
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.LoanLenderInfo:
                        foreach (var l in endorsement.Locations)
                        {
                            foreach (var ai in l.AdditionalInterests)
                            {
                                ai.FillInIdInfo();
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.PayPlan:
                        //endorsement.BillingInformation
                        break;
                    case Endorsement.EndorsementObjectType.MailingAddress:
                        endorsement.BillingInformation.BillingAddress.Address.FillInIdInfo();
                        break;
                }
            }
        }

        private static void RemoveAbandonedObjects(QuickQuoteObject qqe, Endorsement endorsement)
        {
            bool itemsRemoved = false;
            if (endorsement.EndorsementStatus.CheckForAbondonedObject)
            {
                switch (endorsement.ObjectType)
                {
                    case Endorsement.EndorsementObjectType.Driver:
                        List<QuickQuote.CommonObjects.QuickQuoteDriver> abandonedDrivers = qqe.Drivers.FindAll(x => x.DriverNum == "" && x.AddedImageNum == "0");
                        if (abandonedDrivers != null && abandonedDrivers.Count > 0)
                        {
                            foreach (var abandonedDriver in abandonedDrivers)
                            {
                                qqe.Drivers.Remove(abandonedDriver);
                                itemsRemoved = true;
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Vehicle:
                        List<QuickQuote.CommonObjects.QuickQuoteVehicle> abandonedVehicles = qqe.Vehicles.FindAll(x => x.VehicleNum == "" && x.AddedImageNum == "0");
                        if (abandonedVehicles.IsLoaded())
                        {
                            foreach (var abandonedVehicle in abandonedVehicles)
                            {
                                qqe.Vehicles.Remove(abandonedVehicle);
                                itemsRemoved = true;
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Lienholder:
                        QuickQuote.CommonObjects.QuickQuoteVehicle selectedVehicle = qqe.Vehicles.Find(x => x.VehicleNum == endorsement.Vehicles[0].VehicleNum.ToString());
                        if (selectedVehicle != null)
                        {
                            List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest> abandonedLienholderAIs = selectedVehicle.AdditionalInterests.FindAll(x => x.Num == endorsement.Vehicles[0].AdditionalInterests.Last().AdditionalInterestNum.ToString());
                            if (abandonedLienholderAIs.IsLoaded())
                            {
                                foreach (var abandonedAI in abandonedLienholderAIs)
                                {
                                    selectedVehicle.AdditionalInterests.Remove(abandonedAI);
                                    itemsRemoved = true;
                                }
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.LoanLenderInfo:
                        List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest> abandonedLoanLenderAIs = qqe.Locations[0].AdditionalInterests.FindAll(x => x.Num == endorsement.Locations[0].AdditionalInterests.Last().AdditionalInterestNum.ToString());
                        if (abandonedLoanLenderAIs.IsLoaded())
                        {
                            foreach (var abandonedAI in abandonedLoanLenderAIs)
                            {
                                qqe.Locations[0].AdditionalInterests.Remove(abandonedAI);
                                itemsRemoved = true;
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.MailingAddress:
                        break;
                    case Endorsement.EndorsementObjectType.PayPlan:
                        break;
                }
                if (itemsRemoved == false)
                {
                    endorsement.EndorsementStatus.NeedsObjectAdded = false;
                }
            }
        }

        private static void GetEndorsementReadyForEditingCurrentImageObjects(QuickQuoteObject qqe, Endorsement endorsement)
        {
            if (endorsement.EndorsementStatus.NeedsObjectAdded == false && endorsement.EndorsementStatus.CheckForAbondonedObject == true)
            {
                switch (endorsement.ObjectType)
                {
                    case Endorsement.EndorsementObjectType.Driver:
                        List<QuickQuote.CommonObjects.QuickQuoteDriver> myQQDrivers = qqe.Drivers.FindAll(x => x.AddedImageNum == endorsement.PolicyImageNum.ToString());
                        if (myQQDrivers?.Count > 0)
                        {
                            foreach (var d in endorsement.Drivers)
                            {
                                foreach (var qqd in myQQDrivers)
                                {
                                    if (d.Name.FirstName == qqd.Name.FirstName && d.Name.LastName == qqd.Name.LastName)
                                    {
                                        d.DriverNum = qqd.DriverNum.TryToGetInt32();
                                    }
                                }
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Vehicle:
                        List<QuickQuote.CommonObjects.QuickQuoteVehicle> myQQVehicles = qqe.Vehicles.FindAll(x => x.AddedImageNum == endorsement.PolicyImageNum.ToString());
                        if (myQQVehicles?.Count > 0)
                        {
                            foreach (var v in endorsement.Vehicles)
                            {
                                foreach (var myQQV in myQQVehicles)
                                {
                                    if (v.Make == myQQV.Make && v.Model == myQQV.Model && v.Year == myQQV.Year.TryToGetInt32() && v.VIN == myQQV.Vin)
                                    {
                                        v.VehicleNum = myQQV.VehicleNum.TryToGetInt32();
                                    }
                                }
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Lienholder:
                        QuickQuote.CommonObjects.QuickQuoteVehicle qqv = null;
                        List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest> qqAIs = null;
                        foreach (var v in endorsement.Vehicles)
                        {
                            qqv = qqe.Vehicles.Find(x => x.VehicleNum == v.VehicleNum.ToString());
                            if (qqv != null)
                            {
                                qqAIs = qqv.AdditionalInterests.FindAll(x => x.PolicyImageNum == endorsement.PolicyImageNum.ToString());
                                if (qqAIs?.Count > 0)
                                {
                                    foreach (var ai in v.AdditionalInterests)
                                    {
                                        foreach (var qqAI in qqAIs)
                                        {
                                            if (ai.Name.DisplayName == qqAI.Name.DisplayName && ai.Address.City == qqAI.Address.City && ai.Address.StreetName == qqAI.Address.StreetName && ai.Address.HouseNumber == qqAI.Address.HouseNum && ai.Address.PoBox == qqAI.Address.POBox)
                                            {
                                                ai.AdditionalInterestNum = qqAI.Num.TryToGetInt32();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.LoanLenderInfo:
                        List<QuickQuote.CommonObjects.QuickQuoteAdditionalInterest> qqLLAIs = qqe.Locations[0].AdditionalInterests.FindAll(x => x.PolicyImageNum == endorsement.PolicyImageNum.ToString());
                        if (qqLLAIs?.Count > 0)
                        {
                            foreach (var ai in endorsement.Locations[0].AdditionalInterests)
                            {
                                foreach (var qqAI in qqLLAIs)
                                {
                                    if (ai.Name.DisplayName == qqAI.Name.DisplayName && ai.Address.City == qqAI.Address.City && ai.Address.StreetName == qqAI.Address.StreetName && ai.Address.HouseNumber == qqAI.Address.HouseNum && ai.Address.PoBox == qqAI.Address.POBox)
                                    {
                                        ai.AdditionalInterestNum = qqAI.Num.TryToGetInt32();
                                    }
                                }
                            }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.MailingAddress:
                        break;
                    case Endorsement.EndorsementObjectType.PayPlan:
                        break;
                }
            }
        }

        public static Endorsement AddObject(QuickQuoteObject qqe, Endorsement endorsement)
        {
            Endorsement ratedEndorsement = null;
            switch (endorsement.ObjectType)
            {
                case Endorsement.EndorsementObjectType.Driver:
                    ratedEndorsement = AddDriver(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.Vehicle:
                    ratedEndorsement = AddVehicle(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.Lienholder:
                    ratedEndorsement = AddLienholder(qqe, endorsement);
                    break;
            }
            return ratedEndorsement;
        }

        public static Endorsement EditObject(QuickQuoteObject qqe, Endorsement endorsement)
        {
            Endorsement ratedEndorsement = null;
            switch (endorsement.ObjectType)
            {
                case Endorsement.EndorsementObjectType.Driver:
                    ratedEndorsement = EditDriver(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.Vehicle:
                    ratedEndorsement = EditVehicle(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.Lienholder:
                    ratedEndorsement = EditLienholder(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.LoanLenderInfo:
                    ratedEndorsement = EditLoanLenderInfo(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.PayPlan:
                    ratedEndorsement = EditPayPlan(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.MailingAddress:
                    ratedEndorsement = EditMailingAddress(qqe, endorsement);
                    break;
            }
            return ratedEndorsement;
        }

        public static Endorsement DeleteObject(QuickQuoteObject qqe, Endorsement endorsement)
        {
            Endorsement ratedEndorsement = null;
            switch (endorsement.ObjectType)
            {
                case Endorsement.EndorsementObjectType.Driver:
                    ratedEndorsement = DeleteDriver(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.Vehicle:
                    ratedEndorsement = DeleteVehicle(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.Lienholder:
                    ratedEndorsement = DeleteLienholder(qqe, endorsement);
                    break;
                case Endorsement.EndorsementObjectType.LoanLenderInfo:
                case Endorsement.EndorsementObjectType.MailingAddress:
                case Endorsement.EndorsementObjectType.PayPlan:
                    break;
            }
            return ratedEndorsement;
        }

        private static Endorsement RateEndorsement(QuickQuoteObject qqe, Endorsement endorsement)
        {
            endorsement.EndorsementStatus.RateAttempted = true;
            QuickQuoteXML qqXML = new QuickQuoteXML();
            QuickQuoteObject qqeResults = new QuickQuoteObject();
            bool replacedObjectPassedIn = false;
            endorsement.EndorsementStatus.SaveAttempted = true;
            endorsement.EndorsementStatus.RateAttempted = true;
            ItemsToCompleteBeforeFirstRate(endorsement, qqe);
            bool successfullySavedAndRated = qqXML.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(ref qqe, qqeResults: ref qqeResults, saveSuccessful: ref endorsement.EndorsementStatus.refSaveSuccessful, rateSuccessful: ref endorsement.EndorsementStatus.refRateSuccessful, replacedObjectPassedIn: ref replacedObjectPassedIn, errorMessage: ref endorsement.EndorsementStatus.refErrorMessage);
            bool needsSecondRate = ItemsToCompleteAfterFirstRate(endorsement, qqe);
            if (needsSecondRate)
            {
                successfullySavedAndRated = qqXML.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(ref qqe, qqeResults: ref qqeResults, saveSuccessful: ref endorsement.EndorsementStatus.refSaveSuccessful, rateSuccessful: ref endorsement.EndorsementStatus.refRateSuccessful, replacedObjectPassedIn: ref replacedObjectPassedIn, errorMessage: ref endorsement.EndorsementStatus.refErrorMessage);
            }
            endorsement.ValidationItems = new ValidationItems(qqeResults.ValidationItems);
            endorsement.SetPropertiesByReferenceVars();
            if (successfullySavedAndRated)
            {
                return GetRatedEndorsementResults(qqe, endorsement);
            }
            return endorsement;
        }

        private static Endorsement GetRatedEndorsementResults(QuickQuoteObject ratedQQE, Endorsement endorsement)
        {
            CommonHelperClass chc = new CommonHelperClass();
            Endorsement ratedEndorsement = chc.DeepCloneObject(ref endorsement);
            if (chc.GetApplicationXMLSettingForBoolean("Endorsements_UseFullTermPremiumAsVisibleRate", "Endorsements.xml"))
            {
                ratedEndorsement.NewRate = ratedQQE.FullTermPremium;
                ratedEndorsement.DifferenceInRate = ratedQQE.DifferenceChangeInFullTermPremium;
            }
            else
            {
                ratedEndorsement.NewRate = ratedQQE.WrittenPremium;
                ratedEndorsement.DifferenceInRate = ratedQQE.ChangeInWrittenPremium;
            }
            ratedEndorsement.PolicyEffectiveDate = ratedQQE.OriginalEffectiveDate;
            ratedEndorsement.PolicyExpirationDate = ratedQQE.OriginalExpirationDate;
            GetObjectNum(ratedQQE, ratedEndorsement);
            GetNeededPremiums(ratedQQE, ratedEndorsement);
            PassBackUpdatedObjectsForDisplayPurposes(ratedEndorsement, ratedQQE);
            return ratedEndorsement;
        }

        private static Endorsement RateAndFinalize(Endorsement endorsement)
        {
            Endorsement ratedEndorsement = null;
            QuickQuoteXML qqXML = new QuickQuoteXML();
            QuickQuoteObject qqe = LoadEndorsement(endorsement);
            ratedEndorsement = RateEndorsement(qqe, endorsement);
            if (ratedEndorsement.EndorsementStatus.RateSuccessful)
            {
                ratedEndorsement = DoFinalizeEndorsement(ratedEndorsement, qqe);
            }
            return ratedEndorsement;
        }

        private static Endorsement AddDriver(QuickQuoteObject qqe, Endorsement endorsement)
        {
            string errMsg = CopySpouseToPH2WhenPH2DoesNotExist(endorsement, qqe);
            if (String.IsNullOrWhiteSpace(errMsg) == false)
            {
                return new Endorsement(errMsg);
            }
            else
            {
                qqe.Drivers.Add(endorsement.Drivers[0].UpdateQuickQuoteDriver());
                return RateEndorsement(qqe, endorsement);
            }
        }

        private static Endorsement EditDriver(QuickQuoteObject qqe, Endorsement endorsement)
        {
            QuickQuoteDriver UpdatedQQDriver = qqe.Drivers.Find(x => x.DriverNum == endorsement.Drivers[0].DriverNum.ToString());
            if (UpdatedQQDriver != null)
            {
                //qqe.Drivers.Remove(UpdatedQQDriver); //Should we delete the old record and insert an updated "edited" version? Or update the existing record in place? Going to try and just do an inplace update.
                string errMsg = CopySpouseToPH2WhenPH2DoesNotExist(endorsement, qqe);
                if (String.IsNullOrWhiteSpace(errMsg) == false)
                {
                    endorsement.ErrorMessage = errMsg;
                    return endorsement;
                }
                else
                {
                    UpdatedQQDriver = endorsement.Drivers[0].UpdateQuickQuoteDriver(UpdatedQQDriver);
                    return RateEndorsement(qqe, endorsement);
                }
            }
            else
            {
                return new Endorsement("No driver found on policy with DriverNum of " + endorsement.Drivers[0].DriverNum.ToString());
            }
        }

        private static Endorsement DeleteDriver(QuickQuoteObject qqe, Endorsement endorsement)
        {
            QuickQuoteDriver DriverToDelete = qqe.Drivers.Find(x => x.DriverNum == endorsement.Drivers[0].DriverNum.ToString());
            if (DriverToDelete != null)
            {
                qqe.Drivers.Remove(DriverToDelete);
                return RateEndorsement(qqe, endorsement);
            }
            else
            {
                return new Endorsement("No driver found on policy with DriverNum of " + endorsement.Drivers[0].DriverNum.ToString());
            }
        }

        private static Endorsement AddLienholder(QuickQuoteObject qqe, Endorsement endorsement)
        {
            foreach (var v in endorsement.Vehicles)
            {
                IfPredefinedAIExistsOverwriteExistingAI(v.AdditionalInterests);
                QuickQuoteVehicle qqv = qqe.Vehicles.Find(x => x.VehicleNum == v.VehicleNum.ToString());
                if (qqv != null)
                {
                    qqv.AdditionalInterests = qqv.AdditionalInterests.NewIfNull();
                    foreach (var ai in v.AdditionalInterests)
                    {
                        qqv.AdditionalInterests.Add(ai.UpdateQuickQuoteAdditionalInterest());
                    }
                }
                else
                {
                    return new Endorsement("No vehicle found on policy with VehicleNum of " + v.VehicleNum.ToString());
                }
            }
            return RateEndorsement(qqe, endorsement);
        }

        private static Endorsement EditLienholder(QuickQuoteObject qqe, Endorsement endorsement)
        {
            foreach (var v in endorsement.Vehicles)
            {
                IfPredefinedAIExistsOverwriteExistingAI(v.AdditionalInterests);
                QuickQuoteVehicle qqv = qqe.Vehicles?.Find(x => x.VehicleNum == v.VehicleNum.ToString());
                if (qqv != null)
                {
                    foreach (var ai in v.AdditionalInterests)
                    {
                        QuickQuoteAdditionalInterest updatedQQAI = qqv.AdditionalInterests?.Find(x => x.Num == ai.AdditionalInterestNum.ToString());
                        if (updatedQQAI != null)
                        {
                            qqv.AdditionalInterests.Remove(updatedQQAI);
                            updatedQQAI = ai.UpdateQuickQuoteAdditionalInterest();
                            qqv.AdditionalInterests.Add(updatedQQAI);
                        }
                        else
                        {
                            return new Endorsement("No additional interest found on policy with AdditionalInterestNum of " + v.AdditionalInterests.Last().AdditionalInterestNum.ToString());
                        }
                    }
                }
                else
                {
                    return new Endorsement("No vehicle found on policy with VehicleNum of " + v.VehicleNum.ToString());
                }
            }
            return RateEndorsement(qqe, endorsement);
        }

        private static Endorsement DeleteLienholder(QuickQuoteObject qqe, Endorsement endorsement)
        {
            QuickQuoteVehicle qqv = qqe.Vehicles.Find(v => v.VehicleNum == endorsement.Vehicles[0].VehicleNum.ToString());
            if (qqv != null)
            {
                QuickQuoteAdditionalInterest LienholderToDelete = qqv.AdditionalInterests?.Find(x => x.Num == endorsement.Vehicles[0].AdditionalInterests.Last().AdditionalInterestNum.ToString());
                if (LienholderToDelete != null)
                {
                    qqv.AdditionalInterests.Remove(LienholderToDelete);
                    return RateEndorsement(qqe, endorsement);
                }
                else
                {
                    return new Endorsement("No Vehicle -> Additional Interest found on policy with AdditionalInterestNum of " + endorsement.Vehicles[0].AdditionalInterests.Last().AdditionalInterestNum.ToString());
                }
            }
            else
            {
                return new Endorsement("No vehicle found on policy with VehicleNum of " + endorsement.Vehicles[0].VehicleNum.ToString());
            }
        }

        private static Endorsement AddVehicle(QuickQuoteObject qqe, Endorsement endorsement)
        {
            string errorMessage = "";
            bool isOutOfState = endorsement.Vehicles[0].IsGaragingAddressOutOfState(out errorMessage, qqe.Policyholder.Address.State);
            if (errorMessage.NoneAreNullEmptyOrWhitespace())
            {
                endorsement.ErrorMessage = errorMessage;
                return endorsement;
            }
            else
            {
                AddVehicleToEndorsement(qqe, endorsement, isOutOfState);
                return RateEndorsement(qqe, endorsement);
            }
        }

        private static Endorsement EditVehicle(QuickQuoteObject qqe, Endorsement endorsement) //Needs to "replace" the existing vehicle?
        {
            var chc = new CommonHelperClass();
            bool doesKeyExist = false;
            bool hasParseError = false;
            bool doAddThenDeleteVehicleOnEdits = chc.GetApplicationXMLSettingForBoolean("Endorsements_AddThenDeleteVehicleOnEdits", ref doesKeyExist, ref hasParseError, "Endorsements.xml");
            if (doAddThenDeleteVehicleOnEdits == true || doesKeyExist == false)
            {
                if (endorsement.EndorsementStatus.IsNewEndorsement == true)
                {
                    return AddNewVehicleAndDeleteOldVehicle(qqe, endorsement);
                }
                else
                {
                    return EditNewVehicle(qqe, endorsement);
                }
            }
            else
            {
                if(endorsement.EndorsementStatus.IsNewEndorsement == true)
                {
                    return ReplaceVehicle(qqe, endorsement);
                }
                else
                {
                    return EditNewVehicle(qqe, endorsement);
                }
            }
        }

        private static Endorsement ReplaceVehicle(QuickQuoteObject qqe, Endorsement endorsement)
        {
            string errorMessage = "";
            bool isOutOfState = endorsement.Vehicles[0].IsGaragingAddressOutOfState(out errorMessage, qqe.Policyholder.Address.State);
            if (errorMessage.NoneAreNullEmptyOrWhitespace())
            {
                endorsement.ErrorMessage = errorMessage;
                return endorsement;
            }
            else
            {
                QuickQuoteVehicle UpdatedQQVehicle = qqe.Vehicles.Find(x => x.VehicleNum == endorsement.Vehicles[0].VehicleNum.ToString());
                if (UpdatedQQVehicle != null)
                {
                    endorsement.ReplacedVehicle = new Vehicle(UpdatedQQVehicle);
                    //CopyRiskLevelVehicleCoveragesToNewVehicle(qqe, UpdatedQQVehicle);
                    CopyRiskLevelVehicleCoveragesToNewVehicleWithReplacedVehicleNum(qqe, UpdatedQQVehicle, endorsement.Vehicles[0].VehicleNum);
                    //UpdatedQQVehicle = endorsement.Vehicle.UpdateQuickQuoteVehicle(UpdatedQQVehicle);
                    UpdatedQQVehicle = endorsement.Vehicles[0].ClearUniqueVehicleItemsThenUpdateQuickQuoteVehicle(UpdatedQQVehicle, qqe);
                    UpdatedQQVehicle.DriverOutOfStateSurcharge = isOutOfState;
                    return RateEndorsement(qqe, endorsement);
                }
                else
                {
                    return new Endorsement("No vehicle found on policy with VehicleNum of " + endorsement.Vehicles[0].VehicleNum.ToString());
                }
            }
        }

        private static Endorsement EditNewVehicle(QuickQuoteObject qqe, Endorsement endorsement)
        {
            string errorMessage = "";
            bool isOutOfState = endorsement.Vehicles[0].IsGaragingAddressOutOfState(out errorMessage, qqe.Policyholder.Address.State);
            if (errorMessage.NoneAreNullEmptyOrWhitespace())
            {
                endorsement.ErrorMessage = errorMessage;
                return endorsement;
            }
            else
            {
                QuickQuoteVehicle UpdatedQQVehicle = qqe.Vehicles.Find(x => x.VehicleNum == endorsement.Vehicles[0].VehicleNum.ToString());
                if (UpdatedQQVehicle != null)
                {
                    UpdatedQQVehicle = endorsement.Vehicles[0].UpdateQuickQuoteVehicle(UpdatedQQVehicle);
                    UpdatedQQVehicle.DriverOutOfStateSurcharge = isOutOfState;
                    return RateEndorsement(qqe, endorsement);
                }
                else
                {
                    return new Endorsement("No vehicle found on policy with VehicleNum of " + endorsement.Vehicles[0].VehicleNum.ToString());
                }
            }
        }

        private static Endorsement AddNewVehicleAndDeleteOldVehicle(QuickQuoteObject qqe, Endorsement endorsement)
        {
            string errorMessage = "";
            bool isOutOfState = endorsement.Vehicles[0].IsGaragingAddressOutOfState(out errorMessage, qqe.Policyholder.Address.State);
            if (errorMessage.HasValue())
            {
                endorsement.ErrorMessage = errorMessage;
                return endorsement;
            }
            else
            {
                QuickQuoteVehicle QQVehicleToRemove = qqe.Vehicles.Find(x => x.VehicleNum == endorsement.Vehicles[0].VehicleNum.ToString());
                if (QQVehicleToRemove != null)
                {
                    //AddVehicleToEndorsement(qqe, endorsement, isOutOfState);
                    AddVehicleToEndorsementWithReplacedVehicleNum(qqe, endorsement, isOutOfState, endorsement.Vehicles[0].VehicleNum);
                    endorsement.ReplacedVehicle = new Vehicle(QQVehicleToRemove);
                    qqe.Vehicles.Remove(QQVehicleToRemove);
                    return RateEndorsement(qqe, endorsement);
                }
                else
                {
                    return new Endorsement("No vehicle found on policy with VehicleNum of " + endorsement.Vehicles[0].VehicleNum.ToString());
                }
            }
        }

        private static void AddVehicleToEndorsement(QuickQuoteObject qqe, Endorsement endorsement, bool isGaragingAddressOutOfState)
        {
            //QuickQuoteVehicle qqVehicle = new QuickQuoteVehicle();
            //IfPredefinedAIExistsOverwriteExistingAI(endorsement.Vehicles[0].AdditionalInterests);
            //qqe.Vehicles.Add(qqVehicle);
            //qqVehicle = endorsement.Vehicles[0].UpdateQuickQuoteVehicle(qqVehicle, qqe); //Doing it in this order so that qqVehicle is connected to the quote object so that code in UpdateQuickQuoteVehicle has access to the toplevel quote.
            //qqVehicle.DriverOutOfStateSurcharge = isGaragingAddressOutOfState;
            //CopyRiskLevelVehicleCoveragesToNewVehicle(qqe, qqVehicle);
            AddVehicleToEndorsementWithReplacedVehicleNum(qqe, endorsement, isGaragingAddressOutOfState, 0);
        }
        private static void AddVehicleToEndorsementWithReplacedVehicleNum(QuickQuoteObject qqe, Endorsement endorsement, bool isGaragingAddressOutOfState, int vehNum)
        {
            QuickQuoteVehicle qqVehicle = new QuickQuoteVehicle();
            IfPredefinedAIExistsOverwriteExistingAI(endorsement.Vehicles[0].AdditionalInterests);
            qqe.Vehicles.Add(qqVehicle);
            qqVehicle = endorsement.Vehicles[0].UpdateQuickQuoteVehicle(qqVehicle, qqe); //Doing it in this order so that qqVehicle is connected to the quote object so that code in UpdateQuickQuoteVehicle has access to the toplevel quote.
            qqVehicle.DriverOutOfStateSurcharge = isGaragingAddressOutOfState;
            CopyRiskLevelVehicleCoveragesToNewVehicleWithReplacedVehicleNum(qqe, qqVehicle, vehNum);
        }

        private static Endorsement DeleteVehicle(QuickQuoteObject qqe, Endorsement endorsement)
        {
            QuickQuoteVehicle vehicleToDelete = qqe.Vehicles.Find(x => x.VehicleNum == endorsement.Vehicles[0].VehicleNum.ToString());
            if (vehicleToDelete != null)
            {
                qqe.Vehicles.Remove(vehicleToDelete);
                return RateEndorsement(qqe, endorsement);
            }
            else
            {
                return new Endorsement("No vehicle found on policy with VehicleNum of " + endorsement.Vehicles[0].VehicleNum.ToString());
            }
        }

        private static void CopyRiskLevelVehicleCoveragesToNewVehicle(QuickQuoteObject qqe, QuickQuoteVehicle newV)
        {
            //if (qqe != null & qqe.Vehicles.IsLoaded())
            //{
            //    QuickQuoteVehicle qqV = qqe.Vehicles.Find(x => x.VehicleNum.TryToGetInt32() > 0 && x.AddedImageNum != qqe.PolicyImageNum); //Make sure we select a vehicle OTHER than the one we added during this endorsement transaction.
            //    if (qqV != null)
            //    {
            //        newV.BodilyInjuryLiabilityLimitId = qqV.BodilyInjuryLiabilityLimitId;
            //        newV.PropertyDamageLimitId = qqV.PropertyDamageLimitId;
            //        newV.MedicalPaymentsLimitId = qqV.MedicalPaymentsLimitId;
            //        newV.UnderinsuredCombinedSingleLimitId = qqV.UnderinsuredCombinedSingleLimitId;
            //        newV.UninsuredCombinedSingleLimitId = qqV.UninsuredCombinedSingleLimitId;
            //        newV.UnderinsuredBodilyInjuryLimitId = qqV.UnderinsuredBodilyInjuryLimitId;
            //        newV.UninsuredBodilyInjuryLimitId = qqV.UninsuredBodilyInjuryLimitId;
            //        newV.UninsuredMotoristPropertyDamageDeductibleLimitId = qqV.UninsuredMotoristPropertyDamageDeductibleLimitId;
            //        newV.UninsuredMotoristPropertyDamageLimitId = qqV.UninsuredMotoristPropertyDamageLimitId;
            //        newV.UninsuredMotoristLiabilityLimitId = qqV.UninsuredMotoristLiabilityLimitId;
            //        newV.Liability_UM_UIM_LimitId = qqV.Liability_UM_UIM_LimitId;
            //    }
            //    else
            //    {
            //        //What if we just somehow added the only vehicle on the policy? Not sure if this is possible, I would imagine we would force a replace if we were down to the last vehicle rather than a delete/add situation.
            //    }
            //}
            CopyRiskLevelVehicleCoveragesToNewVehicleWithReplacedVehicleNum(qqe, newV, 0);
        }
        private static void CopyRiskLevelVehicleCoveragesToNewVehicleWithReplacedVehicleNum(QuickQuoteObject qqe, QuickQuoteVehicle newV, int vehNum)
        {
            if (Endorsements_CopyRiskLevelCoveragesToNewVehicleWithVehiclePriority() == true)
            {
                CopyRiskLevelCoveragesToNewVehicleWithVehiclePriority(qqe, newV, vehNum);
            }
            else
            {
                if (qqe != null & qqe.Vehicles.IsLoaded())
                {
                    QuickQuoteVehicle qqV = qqe.Vehicles.Find(x => x.VehicleNum.TryToGetInt32() > 0 && x.AddedImageNum != qqe.PolicyImageNum); //Make sure we select a vehicle OTHER than the one we added during this endorsement transaction.
                    if (qqV != null)
                    {
                        newV.BodilyInjuryLiabilityLimitId = qqV.BodilyInjuryLiabilityLimitId;
                        newV.PropertyDamageLimitId = qqV.PropertyDamageLimitId;
                        newV.MedicalPaymentsLimitId = qqV.MedicalPaymentsLimitId;
                        newV.UnderinsuredCombinedSingleLimitId = qqV.UnderinsuredCombinedSingleLimitId;
                        newV.UninsuredCombinedSingleLimitId = qqV.UninsuredCombinedSingleLimitId;
                        newV.UnderinsuredBodilyInjuryLimitId = qqV.UnderinsuredBodilyInjuryLimitId;
                        newV.UninsuredBodilyInjuryLimitId = qqV.UninsuredBodilyInjuryLimitId;
                        newV.UninsuredMotoristPropertyDamageDeductibleLimitId = qqV.UninsuredMotoristPropertyDamageDeductibleLimitId;
                        newV.UninsuredMotoristPropertyDamageLimitId = qqV.UninsuredMotoristPropertyDamageLimitId;
                        newV.UninsuredMotoristLiabilityLimitId = qqV.UninsuredMotoristLiabilityLimitId;
                        newV.Liability_UM_UIM_LimitId = qqV.Liability_UM_UIM_LimitId;
                    }
                    else
                    {
                        //What if we just somehow added the only vehicle on the policy? Not sure if this is possible, I would imagine we would force a replace if we were down to the last vehicle rather than a delete/add situation.
                    }
                }
            }            
        }

        private static bool VehicleHasRiskLevelCovs(QuickQuoteVehicle qqv)
        {
            bool hasIt = false;

            if (qqv != null)
            {
                QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();
                if (qqHelper.IsPositiveIntegerString(qqv.BodilyInjuryLiabilityLimitId) == true || qqHelper.IsPositiveIntegerString(qqv.PropertyDamageLimitId) == true
                    || qqHelper.IsPositiveIntegerString(qqv.MedicalPaymentsLimitId) == true || qqHelper.IsPositiveIntegerString(qqv.UnderinsuredCombinedSingleLimitId) == true
                    || qqHelper.IsPositiveIntegerString(qqv.UninsuredCombinedSingleLimitId) == true || qqHelper.IsPositiveIntegerString(qqv.UnderinsuredBodilyInjuryLimitId) == true
                    || qqHelper.IsPositiveIntegerString(qqv.UninsuredBodilyInjuryLimitId) == true || qqHelper.IsPositiveIntegerString(qqv.UninsuredMotoristPropertyDamageDeductibleLimitId) == true
                    || qqHelper.IsPositiveIntegerString(qqv.UninsuredMotoristPropertyDamageLimitId) == true || qqHelper.IsPositiveIntegerString(qqv.UninsuredMotoristLiabilityLimitId) == true
                    || qqHelper.IsPositiveIntegerString(qqv.Liability_UM_UIM_LimitId) == true)
                {
                    hasIt = true;
                }
            }

            return hasIt;
        }
        private static void CopyVehicleRiskLevelCovs(QuickQuoteVehicle fromV, QuickQuoteVehicle toV)
        {
            if (fromV != null && toV != null)
            {
                toV.BodilyInjuryLiabilityLimitId = fromV.BodilyInjuryLiabilityLimitId;
                toV.PropertyDamageLimitId = fromV.PropertyDamageLimitId;
                toV.MedicalPaymentsLimitId = fromV.MedicalPaymentsLimitId;
                toV.UnderinsuredCombinedSingleLimitId = fromV.UnderinsuredCombinedSingleLimitId;
                toV.UninsuredCombinedSingleLimitId = fromV.UninsuredCombinedSingleLimitId;
                toV.UnderinsuredBodilyInjuryLimitId = fromV.UnderinsuredBodilyInjuryLimitId;
                toV.UninsuredBodilyInjuryLimitId = fromV.UninsuredBodilyInjuryLimitId;
                toV.UninsuredMotoristPropertyDamageDeductibleLimitId = fromV.UninsuredMotoristPropertyDamageDeductibleLimitId;
                toV.UninsuredMotoristPropertyDamageLimitId = fromV.UninsuredMotoristPropertyDamageLimitId;
                toV.UninsuredMotoristLiabilityLimitId = fromV.UninsuredMotoristLiabilityLimitId;
                toV.Liability_UM_UIM_LimitId = fromV.Liability_UM_UIM_LimitId;
            }
        }
        private static QuickQuoteVehicle VehicleForVehicleNum(List<QuickQuoteVehicle> vehs, int vehicleNum)
        {
            QuickQuoteVehicle qqV = null;

            if (vehs.IsLoaded() && vehicleNum > 0)
            {
                qqV = vehs.Find(x => x.VehicleNum.TryToGetInt32() == vehicleNum);
            }

            return qqV;
        }
        private static QuickQuoteVehicle FirstVehicleWithRiskLevelCovs(List<QuickQuoteVehicle> vehs)
        {
            QuickQuoteVehicle qqV = null;

            if (vehs.IsLoaded())
            {
                foreach (QuickQuoteVehicle v in vehs)
                {
                    if (v != null && VehicleHasRiskLevelCovs(v) == true)
                    {
                        qqV = v;
                        break;
                    }
                }
            }

            return qqV;
        }
        private static List<QuickQuoteVehicle> VehiclesEligibleForRiskLevelCovCopy(QuickQuoteObject qqe)
        {
            List<QuickQuoteVehicle> vehs = null;

            if (qqe != null & qqe.Vehicles.IsLoaded())
            {
                vehs = qqe.Vehicles.FindAll(x => x.VehicleNum.TryToGetInt32() > 0 && x.AddedImageNum != qqe.PolicyImageNum);
            }

            return vehs;
        }
        private static void CopyRiskLevelCoveragesToNewVehicleWithVehiclePriority(QuickQuoteObject qqe, QuickQuoteVehicle newV, int preferredVehicleNum)
        {
            if (qqe != null & qqe.Vehicles.IsLoaded() && newV != null)
            {
                List<QuickQuoteVehicle> vehs = VehiclesEligibleForRiskLevelCovCopy(qqe);
                if (vehs.IsLoaded())
                {
                    QuickQuoteVehicle vehToUse = null;
                    bool okayToCopy = true;
                    if (preferredVehicleNum > 0)
                    {
                        QuickQuoteVehicle vehPreferred = VehicleForVehicleNum(vehs, preferredVehicleNum);
                        if (vehPreferred != null && VehicleHasRiskLevelCovs(vehPreferred))
                        {
                            vehToUse = vehPreferred;
                        }
                    }
                    if (vehToUse == null)
                    {
                        vehToUse = FirstVehicleWithRiskLevelCovs(vehs);
                        if (vehToUse == null)
                        {
                            vehToUse = vehs.First();
                            if (vehToUse != null && VehicleHasRiskLevelCovs(newV) == true)
                            {
                                okayToCopy = false;
                            }
                        }
                    }

                    if (vehToUse != null && okayToCopy == true)
                    {
                        CopyVehicleRiskLevelCovs(vehToUse, newV);
                    }
                }
            }
        }
        public static bool Endorsements_CopyRiskLevelCoveragesToNewVehicleWithVehiclePriority()
        {
            bool isOkay = false;

            CommonHelperClass chc = new CommonHelperClass();
            isOkay = chc.GetApplicationXMLSettingForBoolean("Endorsements_CopyRiskLevelCoveragesToNewVehicleWithVehiclePriority", "Endorsements.xml");

            return isOkay;
        }

        private static Endorsement EditLoanLenderInfo(QuickQuoteObject qqe, Endorsement endorsement)
        {
            foreach (var l in endorsement.Locations)
            {
                QuickQuoteLocation qql = qqe.Locations?.Find(x => x.LocationNum == l.LocationNum.ToString());
                if (qql != null)
                {
                    foreach (var ai in l.AdditionalInterests)
                    {
                        IfPredefinedAIExistsOverwriteExistingAI(ai);
                        QuickQuoteAdditionalInterest updatedQQAI = qql.AdditionalInterests?.Find(x => x.Num == ai.AdditionalInterestNum.ToString());
                        if (updatedQQAI != null)
                        {
                            qql.AdditionalInterests.Remove(updatedQQAI);
                            updatedQQAI = ai.UpdateQuickQuoteAdditionalInterest();
                            qql.AdditionalInterests.Add(updatedQQAI);
                        }
                        else
                        {
                            return new Endorsement("No Location -> Additional Interest found on policy with AdditionalInterestNum of " + l.AdditionalInterests.Last().AdditionalInterestNum.ToString());
                        }
                    }
                }
                else
                {
                    return new Endorsement("No location found on policy with LocationNum of " + l.LocationNum.ToString());
                }
            }
            return RateEndorsement(qqe, endorsement);
        }

        private static Endorsement EditPayPlan(QuickQuoteObject qqe, Endorsement endorsement)
        {
            if (qqe != null)
            {
                //Annual = 12, Semi-Annual = 13, Quarterly = 14, Monthly = 15, Renewal EFT Monthly = 19, Renewal Credit Card Monthly 18
                qqe.BillingPayPlanId = endorsement.BillingInformation.PayPlanId.ToString();
                qqe.BillMethodId = "2";
                qqe.BillToId = "1";
            }
            return RateEndorsement(qqe, endorsement);
        }

        private static Endorsement EditMailingAddress(QuickQuoteObject qqe, Endorsement endorsement)
        {
            if (qqe != null)
            {
                //qqe.Policyholder.Address = endorsement.Policyholders[0].Address.UpdateQuickQuoteAddress();
                qqe.BillingAddressee.Address = endorsement.BillingInformation.BillingAddress.Address.UpdateQuickQuoteAddress(); //It was decided that we would be using BillingAddress for this scenario.
            }
            return RateEndorsement(qqe, endorsement);
        }

        private static void GetObjectNum(QuickQuoteObject qqe, Endorsement endorsement)
        {
            QuickQuoteVehicle qqv = null;
            QuickQuoteDriver qqd = null;
            if (qqe != null && endorsement != null && endorsement.ActionType == Endorsement.EndorsementActionType.Add)
            {
                switch (endorsement.ObjectType)
                {
                    case Endorsement.EndorsementObjectType.Vehicle:
                        qqv = qqe.Vehicles.Find(x => x.AddedImageNum == endorsement.PolicyImageNum.ToString());
                        if (qqv != null)
                        {
                            if (qqv.VehicleNum.HasValue()) { endorsement.Vehicles[0].VehicleNum = qqv.VehicleNum.TryToGetInt32(); }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Driver:
                        qqd = qqe.Drivers.Find(x => x.AddedImageNum == endorsement.PolicyImageNum.ToString());
                        if (qqd != null)
                        {
                            if (qqd.DriverNum.HasValue()) { endorsement.Drivers[0].DriverNum = qqd.DriverNum.TryToGetInt32(); }
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Lienholder:
                        qqv = qqe.Vehicles.Find(v => v.VehicleNum == endorsement.Vehicles[0].VehicleNum.ToString());
                        if (qqv != null)
                        {
                            var qqAI = qqv.AdditionalInterests.Find(x => x.PolicyImageNum == endorsement.PolicyImageNum.ToString());
                            if(qqAI != null)
                            {
                                if (qqAI.Num.HasValue()) { endorsement.Vehicles[0].AdditionalInterests[0].AdditionalInterestNum = qqAI.Num.TryToGetInt32(); }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private static void GetNeededPremiums(QuickQuoteObject qqe, Endorsement endorsement)
        {
            if (qqe != null && endorsement != null)
            {
                switch (endorsement.LOBType)
                {
                    case Endorsement.EndorsementLOBType.PPA:
                        GetNeededPPAPremiums(qqe, endorsement);
                        break;
                }
            }
        }

        private static void GetNeededPPAPremiums(QuickQuoteObject qqe, Endorsement endorsement)
        {
            switch (endorsement.ObjectType)
            {
                case Endorsement.EndorsementObjectType.Vehicle:
                    if (qqe.AutoPlusEnhancement_QuotedPremium.HasValue()) { endorsement.PolicyCoverages.AutoPlusEnhancementPremium = qqe.AutoPlusEnhancement_QuotedPremium; } //AutoPlusEnhancement
                    if (qqe.BusinessMasterEnhancementQuotedPremium.HasValue()) { endorsement.PolicyCoverages.AutoEnhancementPremium = qqe.BusinessMasterEnhancementQuotedPremium; } //AutoEnhancement
                    foreach (var v in endorsement.Vehicles)
                    {
                        QuickQuoteVehicle qqv = qqe.Vehicles.Find(x => x.VehicleNum == v.VehicleNum.ToString());
                        if (qqv != null)
                        {
                            v.GetNeededPremiums(qqv);
                        }
                    }
                    break;
            }
        }

        private static string CopySpouseToPH2WhenPH2DoesNotExist(Endorsement endorsement, QuickQuoteObject qqe)
        {
            if(endorsement.Drivers[0].RelationshipTypeId == 9)
            {
                bool copyInfo = false;
                if (qqe.Policyholder2.HasData == false) //If PH2 is empty and newly added driver is spouse, setup newly added driver as PH2
                {
                    copyInfo = true;
                }
                else
                {
                    ////Try to check and see if we have a false PH2 setup.... We found in testing sometimes PH2 "HasData" was true, rightly so, but only one minor piece of data was available. No name, no almost anything... but... something was filled in for some reason
                    //if(qqe.Policyholder2.Name.DisplayName.HasValue() == false)
                    //{
                    //    //If a PH2 doesn't have a display name... I am guessing we have a false positive... lets try to copy over information and clear out whatever was already here.
                    //    //Unfortunately, we are blocking changes to PH information... So this won't work.
                    //    qqe.Policyholder2 = new QuickQuotePolicyholder();
                    //    copyInfo = true;
                    //}
                }

                if (copyInfo)
                {
                    QuickQuoteDriver PHDriverMatch = qqe.Drivers.Find(d => d.Name.DisplayName == qqe.Policyholder.Name.DisplayName);
                    if (PHDriverMatch != null)
                    {
                        qqe.Policyholder.Name.MaritalStatusId = "2"; //Do we need both? //Not allowed to update PH during endorsement????? //Validation was changed and allows MaritalStatusID to be changed for PH#1
                        endorsement.Drivers[0].Name.MartialStatusId = 2; //Do we need both?
                        PHDriverMatch.Name.MaritalStatusId = "2"; //Switch to married when ph2 has been added as spouse
                        endorsement.Drivers[0].RelationshipTypeId = 5; //instead of spouse, we should set as PH#2 per Diamond Validation received when attempting to do so // 0=None, 1=Brother/Sister of PH, 2=Child of PH, 3=Employee of PH, 4=Parent/Guardian of PH, 5=PH2, 6=No Vehicle Operator, 7=Other Relation to PH, 8=PH, 9=Spouse, 10=Relationship Not Known, 11=Not Related to PH, 12=Employee, 13=Additional PH
                        endorsement.Drivers[0].FillInIdInfo();
                        qqe.Policyholder2.Name = endorsement.Drivers[0].Name.UpdateQuickQuoteName(); //Not allowed to update PH during endorsement?????
                    }
                    else
                    {
                        return "New driver added as spouse. Attempted to match PH1 to driver but could not find a match.";
                    }
                }
            }
            return "";
        }

        //Ideally adds in a completed object for MP to display updated info. Such as added coverages and etc.
        private static void PassBackUpdatedObjectsForDisplayPurposes(Endorsement endorsement, QuickQuoteObject qqe)
        {
            if(endorsement != null && qqe != null && endorsement.ActionType != Endorsement.EndorsementActionType.Delete)
            {
                switch(endorsement.ObjectType)
                {
                    case Endorsement.EndorsementObjectType.Vehicle:
                        if(endorsement.ActionType == Endorsement.EndorsementActionType.Add)
                        {
                            QuickQuoteVehicle myVehicle = qqe.Vehicles.Find(v => v.VehicleNum.TryToGetInt32() == endorsement.Vehicles[0].VehicleNum);
                            endorsement.Vehicles.Clear();
                            endorsement.Vehicles.Add(new Vehicle(myVehicle));
                        }
                        else if (endorsement.ActionType == Endorsement.EndorsementActionType.Edit && endorsement.PolicyImageNum.HasValue())
                        {
                            QuickQuoteVehicle myVehicle = qqe.Vehicles.Find(v => v.AddedImageNum == endorsement.PolicyImageNum.ToString());
                            endorsement.Vehicles.Clear();
                            endorsement.Vehicles.Add(new Vehicle(myVehicle));
                        }
                        break;
                }
            }
        }

        private static void ItemsToCompleteBeforeFirstRate(Endorsement endorsement, QuickQuoteObject qqe)
        {
            if (endorsement.EndorsementStatus.IsNewEndorsement == true)
            {
                qqe.TransactionRemark = CreateRemarks(endorsement, qqe);
            }
            else
            {
                //non-new endorsement items
            }
        }

        //Thought this was going to be needed for ordering reports however a bug was fixed to not require this. Keeping here in case we ever end up needing it.
        private static bool ItemsToCompleteAfterFirstRate(Endorsement endorsement, QuickQuoteObject qqe)
        {
            bool needSecondRate = false;
            if (endorsement.EndorsementStatus.IsNewEndorsement == true)
            {
                //new endorsement items
            }
            else
            {
                //non-new endorsement items
            }
            return needSecondRate;
        }

        //This should attempt to use pre-existing AIs when possible instead of creating new ones since the MP_UI doesn't have an AI lookup.
        private static void IfPredefinedAIExistsOverwriteExistingAI(List<AdditionalInterest> myAIList)
        {
            if (myAIList.IsLoaded())
            {
                var myAI = myAIList[0];
                IfPredefinedAIExistsOverwriteExistingAI(myAI);
            }
        }

        private static void IfPredefinedAIExistsOverwriteExistingAI(AdditionalInterest myAI)
        {
            List<AdditionalInterest> foundAIs = null;
            foundAIs = IFM.DataServicesCore.BusinessLogic.Diamond.AdditionalInterestHelper.AdditionalInterestLookup_MPAI(myAI);
            if (foundAIs?.Count > 0)
            {
                foundAIs[0].TypeId = myAI.TypeId;
                if (myAI.AdditionalInterestNum.HasValue())
                    foundAIs[0].AdditionalInterestNum = myAI.AdditionalInterestNum;
                myAI = foundAIs[0];
            }
        }

        private static string CreateRemarks(Endorsement endorsement, QuickQuoteObject qqe = null)
        {
            string myRemark = "";
            if(endorsement != null)
            {
                myRemark = $"Endorsement via Member Portal on {DateTime.Now.ToShortDateString()} - {GetObjectTypeRemarkWording(endorsement.ObjectType)} {GetActionTypeRemarkWording(endorsement)} {GetObjectTypeInfoForRemarks(endorsement, qqe)}";
            }
            return myRemark;
        }

        private static string GetObjectTypeInfoForRemarks(Endorsement endo, QuickQuoteObject qqe = null)
        {
            if(endo != null)
            {
                QuickQuoteDriver qqd = null;
                QuickQuoteVehicle qqv = null;
                QuickQuoteAdditionalInterest qqai = null;
                QuickQuoteLocation qql = null;
                QuickQuoteAddress qqa = null;
                switch (endo.ObjectType)
                {
                    case Endorsement.EndorsementObjectType.Driver:
                        switch(endo.ActionType)
                        {
                            case Endorsement.EndorsementActionType.Add:
                                return $"[{endo.Drivers[0].Name.LastName}, {endo.Drivers[0].Name.FirstName}]";
                            case Endorsement.EndorsementActionType.Edit:
                            case Endorsement.EndorsementActionType.Delete:
                                if (qqe != null)
                                {
                                    qqd = qqe.Drivers?.Find(d => d.DriverNum == endo.Drivers[0].DriverNum.ToString());
                                    if(qqd != null)
                                    {
                                        return $"[Driver #{endo.Drivers[0]?.DriverNum} - {qqd.Name.LastName}, {qqd.Name.FirstName}]";
                                    }
                                }
                                return $"[Driver #{endo.Drivers[0]?.DriverNum}]";
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Vehicle:
                        switch (endo.ActionType)
                        {
                            case Endorsement.EndorsementActionType.Add:
                                return $"[{endo.Vehicles[0].Year} {endo.Vehicles[0].Make} {endo.Vehicles[0].Model}]";
                            case Endorsement.EndorsementActionType.Edit:
                                if (endo.ReplacedVehicle != null)
                                {
                                    return $"[Vehicle #{endo.Vehicles[0].VehicleNum} - {endo.ReplacedVehicle.Year} {endo.ReplacedVehicle.Make} {endo.ReplacedVehicle.Model} with {endo.Vehicles[0].Year} {endo.Vehicles[0].Make} {endo.Vehicles[0].Model}]";
                                }
                                return $"[Vehicle #{endo.Vehicles[0].VehicleNum} with {endo.Vehicles[0].Year} {endo.Vehicles[0].Make} {endo.Vehicles[0].Model}]";
                            case Endorsement.EndorsementActionType.Delete:
                                if (qqe != null)
                                {
                                    qqv = qqe.Vehicles?.Find(v => v.VehicleNum == endo.Vehicles[0].VehicleNum.ToString());
                                    if (qqv != null)
                                    {
                                        return $"[Vehicle #{endo.Vehicles[0]?.VehicleNum} - {qqv.Year} {qqv.Make} {qqv.Model}]";
                                    }
                                }
                                return $"[Vehicle #{endo.Vehicles[0]?.VehicleNum}]";
                        }
                        break;
                    case Endorsement.EndorsementObjectType.Lienholder:
                        switch(endo.ActionType)
                        {
                            case Endorsement.EndorsementActionType.Add:
                                return $"[{endo.Vehicles[0].AdditionalInterests[0].Name.DisplayName}]";
                            case Endorsement.EndorsementActionType.Edit:
                            case Endorsement.EndorsementActionType.Delete:
                                if (qqe != null)
                                {
                                    qqv = qqe.Vehicles?.Find(v => v.VehicleNum == endo.Vehicles[0].VehicleNum.ToString());
                                    if (qqv != null)
                                    {
                                        qqai = qqv.AdditionalInterests.Find(ai => ai.Num == endo.Vehicles[0].AdditionalInterests[0].AdditionalInterestNum.ToString());
                                        if (qqai != null)
                                        {
                                            return $"[Additional Interest #{endo.Vehicles[0].AdditionalInterests[0].AdditionalInterestNum} - {qqai.Name.DisplayName}]";
                                        }
                                    }
                                }
                                return $"[Additional Interest #{endo.Vehicles[0].AdditionalInterests[0].AdditionalInterestNum}]";
                        }
                        return $"";
                    case Endorsement.EndorsementObjectType.LoanLenderInfo:
                        switch (endo.ActionType)
                        {
                            case Endorsement.EndorsementActionType.Edit:
                                if(qqe != null)
                                {
                                    qql = qqe.Locations.Find(l => l.LocationNum == endo.Locations[0].LocationNum.ToString());
                                    if(qql != null)
                                    {
                                        qqai = qql.AdditionalInterests.Find(ai => ai.Num == endo.Locations[0].AdditionalInterests[0].AdditionalInterestNum.ToString());
                                        if (qqai != null)
                                        {
                                            return $"[Additional Interest #{endo.Locations[0].AdditionalInterests[0].AdditionalInterestNum} - {qqai.Name.DisplayName}]";
                                        }
                                    }
                                }
                                break;
                        }
                        return $"";
                    case Endorsement.EndorsementObjectType.MailingAddress:
                        switch (endo.ActionType)
                        {
                            case Endorsement.EndorsementActionType.Edit:
                                if(qqe != null)
                                {
                                    return $"[{qqe.BillingAddressee.Address.DisplayAddress} to {endo.BillingInformation.BillingAddress.Address.ToString()}]";
                                }
                                break;
                        }
                        return $"";
                    case Endorsement.EndorsementObjectType.PayPlan:
                        switch (endo.ActionType)
                        {
                            case Endorsement.EndorsementActionType.Edit:
                                if(qqe != null)
                                {
                                    string OldPayPlan = CommonObjects.IFM.StaticData.StaticDataHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, qqe.BillingPayPlanId); ;
                                    string OldBillMethod = CommonObjects.IFM.StaticData.StaticDataHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, qqe.BillMethodId); ;
                                    string OldBillTo = CommonObjects.IFM.StaticData.StaticDataHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, qqe.BillToId); ;
                                    return $"[Old Pay Plan: {OldPayPlan}, Old Bill Method: { OldBillMethod}, Old Bill To: { OldBillTo} with New Pay Plan: {endo.BillingInformation.PayPlan}, New Bill Method: {endo.BillingInformation.BillingMethod}, New Bill To: {endo.BillingInformation.BillTo}" ;
                                }
                                break;
                        }
                        return $"";
                }
            }
            return "";
        }

        private static string GetObjectTypeRemarkWording(Endorsement.EndorsementObjectType ot)
        {
            switch(ot)
            {
                case Endorsement.EndorsementObjectType.Driver:
                case Endorsement.EndorsementObjectType.Lienholder:
                case Endorsement.EndorsementObjectType.Vehicle:
                    return ot.ToString();
                case Endorsement.EndorsementObjectType.LoanLenderInfo:
                    return "Loan Lender Info";
                case Endorsement.EndorsementObjectType.MailingAddress:
                    return "Mailing Address";
                case Endorsement.EndorsementObjectType.PayPlan:
                    return "Pay Plan";
            }
            return "";
        }

        private static string GetActionTypeRemarkWording(Endorsement endo)
        {
            switch (endo.ActionType)
            {
                case Endorsement.EndorsementActionType.Add:
                    return "Added";
                case Endorsement.EndorsementActionType.Edit:
                    if (endo.ObjectType == Endorsement.EndorsementObjectType.Vehicle)
                        return "Replaced";
                    else
                        return "Edited";
                case Endorsement.EndorsementActionType.Delete:
                    return "Deleted";
            }
            return "";
        }
    }
}
