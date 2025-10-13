using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.DataServicesCore;
using IFM.PrimitiveExtensions;
using IFM.DataServicesCore.BusinessLogic;
using System.Configuration;
using IFM.DataServicesCore.CommonObjects.IFM.Auto;
using System.Collections.Generic;
using IFM.DataServicesCore.CommonObjects.OMP;
using DCO = Diamond.Common.Objects;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.OMP.Endorsements
{
    [TestClass]
    public class EndorsementTests : BaseTest
    {
        //private int polId = 1850706; Finalized not promoted - Added Driver - PPA2135601; polImageNum = 2;
        private int polId = 2471202;//105136;//1972236; //601029; //1645225; //601029; //PPA2087302
        private string polNum = "PPA2060547";//"PPA2087302";//"PPA2137846";//"PPA2034512";//"PPA2087302"; //"PPA2021502";//"PPA2087302";
        //private int polImageNum = 16;
        private int polImageNum = 0; //0; //42
        private int deletePolImageNum = 2; //51; //76;//51;
        private string TransDate = "01/30/2023";
        private string myUser = "DanLocalAPI";

        [TestMethod]
        public void Test1()
        {
            //var myAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            //myAI.Name = myAI.Name.NewIfNull();
            //myAI.Name.CommercialName = "Chase Bank";
            //myAI.Address = myAI.Address.NewIfNull();
            ////myAI.Address.StateId = 16;
            //myAI.Address.StateAbbrev = "IN";
            //myAI.Address.City = "Bloomington";
            //myAI.Address.Zip = "47403";
            //myAI.Address.HouseNumber = "2600";
            //myAI.Address.StreetName = "Walnut";
            //newVehicle.AdditionalInterests = new List<DataServicesCore.CommonObjects.OMP.AdditionalInterest>();
            //var newAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            //newAI.Name = newAI.Name.NewIfNull();
            //newAI.Address = newAI.Address.NewIfNull();
            ////newAI.Address.StateId = 36;
            //newAI.Address.StateAbbrev = "CA";
            //newAI.Address.City = "Fountain Valley";
            //newAI.Address.Zip = "92728";
            ////newAI.Address.HouseNumber = "";
            //newAI.Address.PoBox = "20835";
            //newAI.Name.CommercialName = "Hyundai Motor Finance";
            ////newAI.Name.FirstName = "Daniel";
            ////newAI.Name.LastName = "Gugenheim";
            //newAI.Name.IsPersonalName = true;
            //newAI.TypeId = 8; //8=losspayee, 53=Lienholder1
            ////newVehicle.AdditionalInterests.Add(newAI);
            //appHost.Start(session =>
            //{
            //    var data = ToJson( newAI);
            //    var lookupResult = session.PostJson($"ifm/additionalinterest/AILookupUniquesOnly", data);
            //    APIResponses.Common.ServiceResult srLookup = DeserializeServiceResponse(lookupResult);
            //    var aiLookup = this.DeserializeServiceResponseData<DCO.InsCollection<DCO.Policy.AdditionalInterestList>>(srLookup);
            //});



        }

        [TestMethod]
        public void TestDeleteEndorsement()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.NA;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.NA;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Delete;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = deletePolImageNum;
            endorsement.Username = myUser;

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestFinalizeEndorsement()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.NA;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.NA;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Finalize;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = deletePolImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestPromoteEndorsement()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.NA;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.NA;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Finalize;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = deletePolImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.UserID = 292;
            endorsement.Username = myUser;

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsementpromote", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestAddDriver()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Add;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Driver;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            //DataServicesCore.CommonObjects.OMP.PPA.Driver newDriver = new DataServicesCore.CommonObjects.OMP.PPA.Driver();
            //newDriver.Name = new DataServicesCore.CommonObjects.OMP.Name();
            //newDriver.DistanceToSchool = 101;
            //newDriver.DistantStudent = false;
            //newDriver.GoodStudent = false;
            //newDriver.RelationshipTypeId = 9; //1=Bro/Sis,2=Child,3=Employee,4=Parent/Guardian,5=PH2,6=No Vehicle Operator,7=Other,8=PH,9=Spouse,10=Unknown,11=Not Related,12=Employee,20=Additional PH
            //newDriver.LicenseStatusId = 2; //0="", 1=NA, 2=valid, 3=Suspended, 4=Revoked, 5=Expired, 6=Not Licensed
            //newDriver.RatedExcludedTypeId = 1; //0=NA, 1=Rated, 2=NonRated, 3=Excluded, 4=Included, 5=Watch
            //newDriver.Name.BirthDate = new DateTime(1968, 07, 23);
            //newDriver.Name.FirstName = "DOUGLAS";
            //newDriver.Name.MiddleName = "J";
            //newDriver.Name.LastName = "AADHWZ";
            //newDriver.Name.MartialStatusId = 2; //1=Single,2=Married,3=Divorced,4=Widowed
            //newDriver.Name.SexId = 1; //1=Male,2=Female
            //newDriver.Name.DLStateId = 16; //16=IN,15=IL,18=KY,36=OH
            //newDriver.Name.DLN = "8173525567";
            //newDriver.Name.DLDate = newDriver.Name.BirthDate.AddYears(16);
            //newDriver.Name.LicensedForThreeOrMoreYears = true;

            DataServicesCore.CommonObjects.OMP.PPA.Driver newDriver = new DataServicesCore.CommonObjects.OMP.PPA.Driver();
            newDriver.Name = new DataServicesCore.CommonObjects.OMP.Name();
            //newDriver.DistanceToSchool = 101;
            //newDriver.DistantStudent = false;
            //newDriver.GoodStudent = false;
            newDriver.RelationshipTypeId = 9; //1=Bro/Sis,2=Child,3=Employee,4=Parent/Guardian,5=PH2,6=No Vehicle Operator,7=Other,8=PH,9=Spouse,10=Unknown,11=Not Related,12=Employee,20=Additional PH
            newDriver.LicenseStatusId = 2; //0="", 1=NA, 2=valid, 3=Suspended, 4=Revoked, 5=Expired, 6=Not Licensed
            newDriver.RatedExcludedTypeId = 1; //0=NA, 1=Rated, 2=NonRated, 3=Excluded, 4=Included, 5=Watch
            newDriver.Name.BirthDate = new DateTime(1965, 06, 17);
            newDriver.Name.FirstName = "CURTIS";
            //newDriver.Name.MiddleName = "J";
            newDriver.Name.LastName = "LEMAN";
            newDriver.Name.MartialStatusId = 2; //1=Single,2=Married,3=Divorced,4=Widowed
            newDriver.Name.SexId = 1; //1=Male,2=Female
            newDriver.Name.DLStateId = 16; //16=IN,15=IL,18=KY,36=OH
            newDriver.Name.DLN = "8900969227";
            newDriver.Name.DLDate = newDriver.Name.BirthDate.AddYears(16);
            newDriver.Name.LicensedForThreeOrMoreYears = true;

            endorsement.Drivers.Add(newDriver);

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestEditDriver()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Edit;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Driver;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            DataServicesCore.CommonObjects.OMP.PPA.Driver updatedDriver = new DataServicesCore.CommonObjects.OMP.PPA.Driver();
            updatedDriver.DriverNum = 11;
            updatedDriver.LicenseStatusId = 2;
            updatedDriver.RelationshipTypeId = 1;
            updatedDriver.GoodStudent = true;
            updatedDriver.DistanceToSchool = 101;
            updatedDriver.DistantStudent = true;
            updatedDriver.RatedExcludedTypeId = 1;
            updatedDriver.Name = updatedDriver.Name.NewIfNull();
            updatedDriver.Name.FirstName = "NewFirstName";
            updatedDriver.Name.LastName = "NewLastName";
            updatedDriver.Name.BirthDate = new DateTime(1999, 8, 5);
            updatedDriver.Name.DLN = "3430726733";
            updatedDriver.Name.DLStateId = 16;
            updatedDriver.Name.MartialStatusId = 1;
            updatedDriver.Name.SexId = 1;
            updatedDriver.Name.DLDate = updatedDriver.Name.BirthDate.AddYears(16);

            endorsement.Drivers.Add(updatedDriver);

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestDeleteDriver()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Delete;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Driver;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            DataServicesCore.CommonObjects.OMP.PPA.Driver deleteDriver = new DataServicesCore.CommonObjects.OMP.PPA.Driver();
            deleteDriver.DriverNum = 11;
            endorsement.Drivers.Add(deleteDriver);

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestAddVehicle()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Add;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Vehicle;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            //endorsement.PolicyCoverages.hasAutoEnhancement = true;

            var newVehicle = new DataServicesCore.CommonObjects.OMP.PPA.Vehicle();
            newVehicle.AnnualMileage = 3000;
            //newVehicle.Year = 2021;
            //newVehicle.Make = "HONDA";
            //newVehicle.Model = "CIVIC";
            //newVehicle.VIN = "1FMCU0F62LUB90759";
            //newVehicle.VIN = "WAUZZZGE3LB000000";
            newVehicle.VIN = "JH4DA9350PS016433";
            newVehicle.UseTypeId = 6; //2 = Business, 4 = Farm, 6 = Personal
            newVehicle.GaragingAddress = new DataServicesCore.CommonObjects.OMP.Address();
            //newVehicle.GaragingAddress.HouseNumber = "212";
            //newVehicle.GaragingAddress.StreetName = "LAKE ROAD";
            //newVehicle.GaragingAddress.AptNum = "";
            //newVehicle.GaragingAddress.PoBox = "";
            //newVehicle.GaragingAddress.City = "LE ROY";
            //newVehicle.GaragingAddress.StateId = 15;
            //newVehicle.GaragingAddress.Zip = "61752";
            //newVehicle.GaragingAddress.County = "DE WITT";
            newVehicle.ComprehensiveDeductibleLimitId = 22; //0=NA,17=50,18=100,20=200,21=250,22=500,24=1000,153=2000,76=5000
            newVehicle.CollisionDeductibleLimitId = 22; //0=NA,21=250,22=500,24=1000,153=2000,76=5000
            newVehicle.TransportationExpenseLimitId = 0; //0=20/600,80=30/900,127=40,1200,128=50/1500
            newVehicle.TowingAndLaborDeductibleLimitId = 27; //0=NA,27=25,41=50,150=75,211=100,217=150,212=200,353=250,25=300
            newVehicle.HasLoanLeaseCoverage = false;
            newVehicle.HasLoanOrLease = true;
            newVehicle.AdditionalInterests = new List<DataServicesCore.CommonObjects.OMP.AdditionalInterest>();

            //newVehicle.BodyTypeId = 31;
            //newVehicle.PerformanceTypeId = 1;
            //newVehicle.RestraintTypeId = 4;
            //newVehicle.AntiTheftTypeId = 2;
            //newVehicle.AntiLockBrakesTypeId = 2;
            //newVehicle.CollSymbol = "H1";
            //newVehicle.CompSymbol = "H2";

            endorsement.Vehicles.Add(newVehicle);

            //var myAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            //myAI.Name = myAI.Name.NewIfNull();
            //myAI.Name.CommercialName = "Chas";

            //newVehicle.AdditionalInterests = new List<DataServicesCore.CommonObjects.OMP.AdditionalInterest>();
            //var newAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            //newAI.Name = newAI.Name.NewIfNull();
            //newAI.Address = newAI.Address.NewIfNull();
            //newAI.Address.StateAbbrev = "CA";
            //newAI.Address.City = "Fountain Valley";
            //newAI.Address.Zip = "92728";
            ////newAI.Address.HouseNumber = "";
            //newAI.Address.PoBox = "20835";
            //newAI.Name.CommercialName = "Hyundai Motor Finance";
            ////newAI.Name.FirstName = "Daniel";
            ////newAI.Name.LastName = "Gugenheim";
            //if (newAI.Name.CommercialName.HasValue())
            //{
            //    newAI.Name.IsPersonalName = false;
            //} else
            //{
            //    newAI.Name.IsPersonalName = true;
            //}
            //newAI.TypeId = 53; //8=losspayee, 53=Lienholder1
            //newVehicle.AdditionalInterests.Add(newAI);

            appHost.Start(session =>
            {
                DoVehicleLookup(newVehicle, session);
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestEditVehicle()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Edit;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Vehicle;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            var editVehicle = new DataServicesCore.CommonObjects.OMP.PPA.Vehicle();
            editVehicle.VehicleNum = 1; //Nissan
            editVehicle.AnnualMileage = 3000;
            //editVehicle.Year = 2016;
            //editVehicle.Make = "CHEVROLET";
            //editVehicle.Model = "CAMARO";
            editVehicle.VIN = "JH4DA9350PS016433";
            editVehicle.ComprehensiveDeductibleLimitId = 153;
            editVehicle.CollisionDeductibleLimitId = 22;
            editVehicle.TowingAndLaborDeductibleLimitId = 41;
            editVehicle.UseTypeId = 6; //2 = Business, 4 = Farm, 6 = Personal
            endorsement.Vehicles.Add(editVehicle);

            editVehicle.AdditionalInterests = new List<DataServicesCore.CommonObjects.OMP.AdditionalInterest>();
            var newAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            newAI.Name = newAI.Name.NewIfNull();
            newAI.Address = newAI.Address.NewIfNull();
            //newAI.Address.StateId = 36;
            newAI.Address.StateAbbrev = "IN";
            newAI.Address.City = "Avon";
            newAI.Address.Zip = "46123";
            newAI.Address.HouseNumber = "123";
            newAI.Address.StreetName = "Test Ave.";
            //newAI.Name.CommercialName = "Chase Bank";
            newAI.Name.FirstName = "Daniel";
            newAI.Name.LastName = "Gugenheim";
            newAI.Name.IsPersonalName = true;
            newAI.TypeId = 8; //8=losspayee, 53=Lienholder1
            editVehicle.AdditionalInterests.Add(newAI);

            editVehicle.HasLoanLeaseCoverage = true;
            editVehicle.HasLoanOrLease = true;

            appHost.Start(session =>
            {
                DoVehicleLookup(editVehicle, session);
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestDeleteVehicle()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Delete;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Vehicle;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            var deleteVehicle = new DataServicesCore.CommonObjects.OMP.PPA.Vehicle();
            deleteVehicle.VehicleNum = 6; //Nissan
            endorsement.Vehicles.Add(deleteVehicle);

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestAddLienholder()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Add;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Lienholder;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            var myVehicle = new DataServicesCore.CommonObjects.OMP.PPA.Vehicle();
            myVehicle.VehicleNum = 1;
            endorsement.Vehicles.Add(myVehicle);
            endorsement.Vehicles[0].AdditionalInterests = endorsement.Vehicles[0].AdditionalInterests.NewIfNull();

            var newAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            newAI.TypeId = 53; //8=losspayee, 53=Lienholder1
            newAI.Name = newAI.Name.NewIfNull();
            newAI.Address = newAI.Address.NewIfNull();
            newAI.Name.CommercialName = "Chase";
            newAI.Address.HouseNumber = "1234";
            newAI.Address.StreetName = "BUSINESS AVE";
            newAI.Address.Zip = "46123";
            newAI.Address.City = "AVON";
            newAI.Address.StateAbbrev = "IN";
            //newAI.Address.StateId = 16; //16=Indiana
            endorsement.Vehicles[0].AdditionalInterests.Add(newAI);

            //endorsement.PolicyImageNum = deletePolImageNum;
            //newAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            //newAI.TypeId = 53; //8=losspayee, 53=Lienholder1
            //newAI.Name = new DataServicesCore.CommonObjects.OMP.Name();
            //newAI.Address = new DataServicesCore.CommonObjects.OMP.Address();
            //newAI.Name.CommercialName = "Huntington Bank";
            //newAI.Address.HouseNumber = "5678";
            //newAI.Address.StreetName = "COMPANY AVE";
            //newAI.Address.Zip = "46121";
            //newAI.Address.City = "COATESVILLE";
            //newAI.Address.StateId = 16; //16=Indiana
            //endorsement.Vehicles[0].AdditionalInterests.Add(newAI);

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestEditLienholder()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Edit;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Lienholder;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            var myVehicle = new DataServicesCore.CommonObjects.OMP.PPA.Vehicle();
            myVehicle.VehicleNum = 6;
            endorsement.Vehicles.Add(myVehicle);

            endorsement.Vehicles[0].AdditionalInterests = endorsement.Vehicles[0].AdditionalInterests.NewIfNull();
            var myAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            myAI.AdditionalInterestNum = 4;
            myAI.TypeId = 53; //8=losspayee, 53=Lienholder1
            myAI.Name = new DataServicesCore.CommonObjects.OMP.Name();
            myAI.Address = new DataServicesCore.CommonObjects.OMP.Address();
            myAI.Name.CommercialName = "Huntington Bank";
            myAI.Address.HouseNumber = "9876";
            myAI.Address.StreetName = "HUNT AVE";
            myAI.Address.Zip = "46123";
            myAI.Address.City = "AVON";
            myAI.Address.StateId = 16; //16=Indiana
            endorsement.Vehicles[0].AdditionalInterests.Add(myAI);

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestDeleteLienholder()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementActionType.Delete;
            endorsement.ObjectType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementObjectType.Lienholder;
            endorsement.TransactionType = DataServicesCore.CommonObjects.OMP.Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            var myVehicle = new DataServicesCore.CommonObjects.OMP.PPA.Vehicle();
            myVehicle.VehicleNum = 6;
            endorsement.Vehicles.Add(myVehicle);

            endorsement.Vehicles[0].AdditionalInterests = endorsement.Vehicles[0].AdditionalInterests.NewIfNull();
            var myAI = new DataServicesCore.CommonObjects.OMP.AdditionalInterest();
            myAI.AdditionalInterestNum = 4;
            endorsement.Vehicles[0].AdditionalInterests.Add(myAI);

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        [TestMethod]
        public void TestPayPlanChange()
        {
            DataServicesCore.CommonObjects.OMP.Endorsement endorsement = new DataServicesCore.CommonObjects.OMP.Endorsement();
            endorsement.ActionType = Endorsement.EndorsementActionType.Edit;
            endorsement.ObjectType = Endorsement.EndorsementObjectType.PayPlan;
            endorsement.TransactionType = Endorsement.EndorsementTransactionType.Rate;
            endorsement.PolicyNumber = polNum;
            endorsement.PolicyId = polId;
            endorsement.PolicyImageNum = polImageNum;
            endorsement.TransactionDate = TransDate;
            endorsement.Username = myUser;

            //Annual = 12, Semi-Annual = 13, Quarterly = 14, Monthly = 15, Renewal EFT Monthly = 19, Renewal Credit Card Monthly 18
            endorsement.BillingInformation.PayPlanId = 15;

            appHost.Start(session =>
            {
                var data = ToJson(endorsement);
                var result = session.PostJson("omp/endorsements/endorsement", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }

        private void DoVehicleLookup(global::IFM.DataServicesCore.CommonObjects.OMP.PPA.Vehicle myVehicle, MvcIntegrationTestFramework.Browsing.BrowsingSession session)
        {
            APIResponses.Common.ServiceResult<List<VinLookupResult>> srLookup = null;
            if (myVehicle.VIN.HasValue())
            {
                var lookupResultWithVIN = session.Get($"ifm/auto/ModelInfoByVIN?vin={myVehicle.VIN}&versionId=245");
                srLookup = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<VinLookupResult>>>(lookupResultWithVIN);
            }
            else if (myVehicle.Model.HasValue() && myVehicle.Make.HasValue() && myVehicle.Year.HasValue())
            {
                var lookupResultNoVIN = session.Get($"ifm/auto/ModelInfoByYearMakeModel?year={myVehicle.Year}&make={myVehicle.Make}&model={myVehicle.Model}&versionId=245");
                srLookup = DeserializeServiceResponse<APIResponses.Common.ServiceResult<List<VinLookupResult>>>(lookupResultNoVIN);
            }
            if (srLookup != null)
            {
                //var vinLookup = this.DeserializeServiceResponseData(srLookup);
                if (srLookup?.ResponseData?.Count > 0)
                {
                    myVehicle.SetValuesFromVinResult(srLookup.ResponseData[0]);
                }
            }
        }

        private void DoAILookup(DataServicesCore.CommonObjects.OMP.PPA.Vehicle myVehicle, DataServicesCore.CommonObjects.OMP.AdditionalInterest myAI)
        {
            var myAIs = global::IFM.DataServicesCore.BusinessLogic.Diamond.AdditionalInterestHelper.AdditionalInterestLookup_MPAI(myAI);
            if (myAIs?.Count > 0)
            {
                myVehicle.AdditionalInterests.Add(myAIs[0]);
            }
            else
            {
                myVehicle.AdditionalInterests.Add(myAI);
            }
        }
    }
}
