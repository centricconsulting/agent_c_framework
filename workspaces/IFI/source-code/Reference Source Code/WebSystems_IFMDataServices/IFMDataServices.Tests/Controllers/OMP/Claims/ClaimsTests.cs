using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFM.DataServicesCore.CommonObjects.OMP;
using System.Diagnostics;
using DCE= Diamond.Common.Enums;
using System.Collections.Generic;
using Newtonsoft.Json;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Tests.Controllers.OMP.Claims
{
    [TestClass]
    public class ClaimsTests : BaseTest
    {
        [TestMethod]        
        public void SubmitClaimForm()
        {
            ClaimReport report = new ClaimReport
            {
                EmailAddress = "fake@site.com",
                InjuriesExist = false,
                LossDateTime = DateTime.Now,
                IpAddress = "no ip",
                LossDescription = "Loss description",
                LossType = global::IFM.DataServicesCore.CommonObjects.Enums.Enums.ClaimLossType.Auto_Personal,
                Name = "Fake Guy",
                PhoneNumber = "(317) 555-1234",
                PolicyNumber = "PPA1234567"
            };

            appHost.Start(session =>
            {
                var data = ToJson(report);
                var result = session.PostJson("omp/claim/claimreport", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                Assert.IsFalse(sr.HasErrors);
            });

        }
        [TestMethod]
        public void SubmitClaim_FNOL()
        {
            FNOL fNOL = new FNOL();
            fNOL.SaveAttempt = DCE.Claims.SaveAttempt.First;
            fNOL.AdministratorId = "135";
            fNOL.ClaimLocation = "IndianapolisTest";
            fNOL.ClaimLossType = global::IFM.DataServicesCore.CommonObjects.Enums.Enums.FNOLClaimLossType.FallingObjectsOrMissiles;
            fNOL.ClaimSeverity_Id = global::IFM.DataServicesCore.CommonObjects.Enums.Enums.SeverityLoss.MinorLoss;
            fNOL.ClaimType = DCE.Claims.ClaimType.Normal;
            fNOL.claimOfficeID = "1";
            fNOL.ClaimFaultType = global::IFM.DataServicesCore.CommonObjects.Enums.Enums.ClaimFaultType.Undetermined;
            fNOL.PolicyNumber = "PPA2020853";//"HOM2114501";// "CAP1003394";//"CAP1000778";//"HOM1001813";////"PPA2020853";//"PPA1007462";
            fNOL.AgencyName = "Fabrum";
            fNOL.EntryUser = "MemberPortal";
            #region claimant
            if (fNOL.Claimants == null)
            {
                fNOL.Claimants = new List<Claimant>();
                fNOL.Claimants.Add(new Claimant()
                {
                    claimantTypeID = 15,
                    // City = "Elkhart",
                    Name = new Name
                    {
                        FirstName = "ROBERT",
                        LastName = "MOORE",
                    },
                    Phone = new List<Phone>() {
                     new Phone { Number = "(567)890-4321" , Type ="Home" ,TypeId =1},
                     new Phone{Number ="(876)765-9876",Type ="Business",TypeId =2}
                },

                    Address = new Address
                    {
                        City = "Elkhart",
                        HouseNumber = "56543",
                        State = "16",
                        StreetName = "WOODBINE LN",
                        Zip = "46516-0000",
                    },
                });
                //fNOL.Claimants.Add(new Claimant()
                //{

                //    claimantTypeID = 7,
                //    Address = new Address
                //    {
                //        City = "ELKHART",
                //        HouseNumber = "56543",
                //        StateId = 16,
                //        StreetName = "WOODBINE LN",
                //        Zip = "46516-0000"
                //    },
                //    Name = new Name
                //    {
                //        FirstName = "GABRIELA",
                //        LastName = "MOORE"
                //    }
                //});
            }
            #endregion
            fNOL.Description = "Hit a car";//"LossDescription";
            fNOL.InsideAdjusterId = "134";
            #region ContactOther
            if (fNOL.ContactOther==null)
            {
                fNOL.ContactOther = new Person();
                if(fNOL.ContactOther.Phone ==null)
                {
                    fNOL.ContactOther.Phone = new List<Phone>();
                    List<Phone> phone = new List<Phone>();
                    phone.Add(new Phone { Number = "(567)890-4321", TypeId = 1 });
                    foreach (var ph in phone)
                        fNOL.ContactOther.Phone.Add(ph);
                }
                if (fNOL.ContactOther.Name == null)
                {
                    fNOL.ContactOther.Name = new Name();
                    fNOL.ContactOther.Name.FirstName = "Test";
                    fNOL.ContactOther.Name.LastName = "Test";
                }
            }
            #endregion
            
            fNOL.MedicalAttention = true;
            #region Insured
            //address
            if (fNOL.Insured == null)
            {
                fNOL.Insured = new FNOL_Insured();
                if (fNOL.Insured.Address == null)
                {
                    //fNOL.Insured.Address = new Address();
                    //fNOL.Insured.Address.City = "ELKHART";
                    //fNOL.Insured.Address.HouseNumber = "56543";
                    //fNOL.Insured.Address.StateId = 16;
                    //fNOL.Insured.Address.StreetName = "WOODBINE LN";
                    //fNOL.Insured.Address.Zip = "46516-0000";
                }
                //contactinfo
                if (fNOL.Insured.Phone == null)
                {
                    fNOL.Insured.Phone = new List<Phone>();
                    List<Phone> phone = new List<Phone>();
                    phone.Add(new Phone { Number = "(567)890-4321", TypeId = 1 });
                    foreach (var ph in phone)
                        fNOL.Insured.Phone.Add(ph);
                }
                //Name 
                if (fNOL.Insured.Name == null)
                {
                    fNOL.Insured.Name = new Name();
                    //fNOL.Insured.Name.FirstName = "";// "robert";
                    //fNOL.Insured.Name.LastName = "Royer";
                    fNOL.Insured.Name.CommercialName = "Test";
                }
                if (fNOL.Insured.Email == null)
                {
                    fNOL.Insured.Email = new List<Email>();
                    List<Email> email = new List<Email>();
                    email.Add(new Email { EmailId = "bb@Ifm.com", TypeID = 1 });
                    foreach (var e in email)
                        fNOL.Insured.Email.Add(e);
                }
            }
            #endregion
            #region Second Insured
            //if (fNOL.SecondInsured == null)
            //{
            //    fNOL.SecondInsured = new FNOL_Insured();
            //    //address
            //    if (fNOL.SecondInsured.Address == null)
            //    {
            //        fNOL.SecondInsured.Address = new Address();
            //        fNOL.SecondInsured.Address.City = "GRANGER";
            //        fNOL.SecondInsured.Address.StateId = 16;
            //        fNOL.SecondInsured.Address.Zip = "46530 - 0000";
            //        fNOL.SecondInsured.Address.PoBox = "1308";
            //    }
            //    //contactinfo
            //    if (fNOL.SecondInsured.Phone == null)
            //    {
            //        fNOL.SecondInsured.Phone = new List<Phone>();
            //        Phone sph = new Phone();
            //        sph.Number = "(574)370-8588";
            //        sph.TypeId = 1; //home
            //        fNOL.SecondInsured.Phone.Add(sph);
            //    }
            //    //Name 
            //    if (fNOL.SecondInsured.Name == null)
            //    {
            //        fNOL.SecondInsured.Name = new Name();
            //        fNOL.SecondInsured.Name.FirstName = "MARCEIL";
            //        fNOL.SecondInsured.Name.LastName = "Royer";
            //        fNOL.SecondInsured.Name.MiddleName = "L";
            //    }
            //}
            #endregion
            #region LossAddress
            //LossAddress
            if (fNOL.LossAddress == null)
            {
                fNOL.LossAddress = new Address();
                fNOL.LossAddress.City = "GRANGER";
                fNOL.LossAddress.County = "";
                fNOL.LossAddress.HouseNumber = "";
                fNOL.LossAddress.PoBox = "1308";
                fNOL.LossAddress.StateId = 16;
                fNOL.LossAddress.StreetName = "";
                fNOL.LossAddress.Zip = "46530-0000";
            }
            #endregion
            fNOL.LossDate = new DateTime(2021, 11, 07);
            fNOL.PolicyID = 1519;
            fNOL.PolicyImage = 50;
            fNOL.PolicyVersionId = "173";
            fNOL.ReportedDate = new DateTime(2021, 12, 13);
         
            fNOL.StatusType = 0;// DCE.Claims.ClaimLnStatusType.NewClaim;
            fNOL.UserID = 10884;
            #region Vehicles
            if (fNOL.Vehicles == null)
            {
                fNOL.Vehicles = new List<FNOLVehicle>();
                fNOL.Vehicles.Add(new FNOLVehicle()
                {
                    Color = "",
                    DamageDescription = "",
                    Drivable = false,
                    EstimatedAmountOfDamage = 0,
                    InvolvedInLoss = false,
                    LicensePlate = "",
                    LicenseState = DataServicesCore.CommonObjects.Enums.Enums.State.INDIANA,
                  //  LocationOfAccidentAddress = new Address { AddressOther = "", AptNum = "", City = "", County = "", HouseNumber = "", PoBox = "", StateId = 0, StreetName = "", Zip = "" },
                    LossIndicatorType = DataServicesCore.CommonObjects.Enums.Enums.ClaimLossIndicatorType.None,
                    LossVehicleOperatorFirstName = "Marceil",
                    LossVehicleOperatorLastName = "Royer",
                    LossVehicleOperatorMiddleName = "L",
                    LossVehicleOwnerFirstName = "MARC",
                    LossVehicleOwnerLastName = "ROYER",
                    Make = "MERCEDES BENZ",
                    Model = "SL450",
                    VIN = "WDDJK6GA4JF052252",
                    Year = 2018,

                });

            }
            fNOL.FNOLType = DataServicesCore.CommonObjects.Enums.Enums.FNOL_LOB_Enum.Auto;
            ////Witness = new Witness();
            #endregion
            //if (fNOL.Properties == null)
            //{
            //    fNOL.Properties = new List<Property>();f
            //    fNOL.Properties.Add(new Property()
            //    {
            //        DwellingType = DataServicesCore.CommonObjects.Enums.Enums.DwellingType.NotAvailable,
            //        LocationOfLossType = DataServicesCore.CommonObjects.Enums.Enums.ClaimLocationOfLoss.OnPremises,
            //        DamageDescription ="Test",
            //        Location = new FNOLLocation
            //        {
            //            ApartmentNumber = "",
            //            City = "ELKHART",
            //            CountryID = DCE.Country.UnitedStates,
            //            HouseNumber = "56543",
            //            NameAddressSource = DCE.NameAddressSource.Location,
            //            State = DataServicesCore.CommonObjects.Enums.Enums.State.INDIANA,
            //            Status = DCE.StatusCode.Active
            //            ,
            //            StreetName = "WOODBINE LN",
            //            Zip = "46516-0000"
            //        }
            //    }) ;
            //}
            #region Properties

            #endregion
            var uploadFile = @"C:\Users\dagug\Desktop\test.html";

            byte[] uploadBytes = null;
            if (System.IO.File.Exists(uploadFile))
            {
                uploadBytes = System.IO.File.ReadAllBytes(uploadFile);
            }
            if (fNOL.FNOLAttachements == null)
            {
                fNOL.FNOLAttachements = new List<FNOLAttachement>();
                fNOL.FNOLAttachements.Add(new FNOLAttachement()
                {
                    FileBytes = uploadBytes,
                    FileName = System.IO.Path.GetFileName(uploadFile)
                });
            }
            // fNOL.FNOLAttachements = null;

            appHost.Start(session =>
            {
                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
                string json = JsonConvert.SerializeObject(fNOL, microsoftDateFormatSettings);
                var data = ToJson(fNOL);
                var result = session.PostJson("diamond/claim/FNOLClaim", data);
                APIResponses.Common.ServiceResult sr = DeserializeServiceResponse(result);
                var errorMsg = sr.Messages?.Count > 0 ? sr.Messages.ToString() : "";
                Assert.IsFalse(sr.HasErrors, errorMsg);
            });
        }
    }
}
