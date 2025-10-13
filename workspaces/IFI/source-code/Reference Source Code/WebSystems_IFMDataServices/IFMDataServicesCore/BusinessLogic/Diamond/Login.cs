using System;
using System.Web;
using IFM.PrimitiveExtensions;
using IDS = Insuresoft.DiamondServices;
using APIResponses = IFM.DataServices.API.ResponseObjects;
#if DEBUG
using System.Diagnostics;
#endif

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public static class Login
    {
        public static void LoginNowT(string username, string password, APIResponses.Common.ServiceResult sr)
        {
            using (var DS = IDS.LoginService.GetDiamTokenForUsernamePassword())
            {
                DS.RequestData.LoginName = username;
                DS.RequestData.Password = password;
                var response = DS.Invoke();
                if (response.ex != null)
                    sr.Messages.CreateErrorMessage(response.ex.Message);

                if (response.dv != null)
                {
                    foreach (var i in response.dv.ValidationItems)
                    {
                        sr.Messages.CreateErrorMessage(i.Message);
                    }
                }

                var Token = response?.DiamondResponse?.ResponseData?.DiamondSecurityToken;
                if (Token != null)
                {
                    HttpContext.Current.Session["DiamondUsername"] = username;
                    HttpContext.Current.Session["DiamondUserId"] = Token.DiamUserId;
                    Insuresoft.DiamondServices.Common.SetDiamondToken(Token);
                }
                else
                {
                    HttpContext.Current.Session.Remove("DiamondUsername");
                    HttpContext.Current.Session.Remove("DiamondUserId");
#if DEBUG
                    Debugger.Break();
#else
                    global::IFM.IFMErrorLogging.LogIssue($"Login failed - Username: '{username}'.", "IFMDATASERVICES -> Login.cs -> Function LoginNowT");
                    sr.Messages.CreateErrorMessage("Login failed.");
#endif
                }
            }
        }

        public static void LoginNow(string username, string password)
        {
            using (var DS = IDS.LoginService.GetDiamTokenForUsernamePassword())
            {
                DS.RequestData.LoginName = username;
                DS.RequestData.Password = password;
                var response = DS.Invoke()?.DiamondResponse;
                var Token = response?.ResponseData?.DiamondSecurityToken;
                if (Token != null)
                {
                    HttpContext.Current.Session["DiamondUsername"] = username;
                    HttpContext.Current.Session["DiamondUserId"] = Token.DiamUserId;
                    HttpContext.Current.Session["Diamond_DidLogin"] = true;
                    Insuresoft.DiamondServices.Common.SetDiamondToken(Token);
                }
                else
                {
                    string diamondValidations = "";
                    if (response != null && response.DiamondValidation != null && response.DiamondValidation.ValidationItems.Count > 0)
                    {
                        foreach (var val in response.DiamondValidation.ValidationItems)
                        {
                            if (diamondValidations.IsNullEmptyOrWhitespace())
                            {
                                diamondValidations = " DiamondValidations: ";
                            }
                            else
                            {
                                diamondValidations += ", ";
                            }
                            diamondValidations += val.Message;
                        }
                    }
                    else
                    {
                        if (response == null)
                        {
                            diamondValidations = " Response object is null.";
                        }
                    }
                    HttpContext.Current.Session.Remove("DiamondUsername");
                    HttpContext.Current.Session.Remove("DiamondUserId");
                    HttpContext.Current.Session.Remove("Diamond_DidLogin");
#if DEBUG
                    Debugger.Break();
#else
                        global::IFM.IFMErrorLogging.LogIssue($"Login failed. - Username: '{username}'.{diamondValidations}", "IFMDATASERVICES -> Login.cs -> Function LoginNow");
#endif
                }
            }
        }

        public static Int32 GetUserId()
        {
            if (System.Web.HttpContext.Current?.Session != null && System.Web.HttpContext.Current.Session["DiamondUserId"].ToString().HasValue() == true && System.Web.HttpContext.Current.Session["DiamondUserId"].ToString().IsNumeric() == true)
            {
                return System.Web.HttpContext.Current.Session["DiamondUserId"].ToString().TryToGetInt32();
            }
            else
            {
                using (var DS = IDS.SecurityService.GetSignedOnUser())
                {
                    var user = DS.Invoke()?.DiamondResponse?.ResponseData?.User;
                    if (user != null)
                        return user.UsersId;
                    else
                        return 0;
                }
            }
        }

        public static string GetUserName()
        {
            if (System.Web.HttpContext.Current?.Session != null && System.Web.HttpContext.Current.Session["DiamondUsername"].ToString().HasValue() == true)
            {
                return System.Web.HttpContext.Current.Session["DiamondUsername"].ToString();
            }
            else
            {
                using (var DS = IDS.SecurityService.GetSignedOnUser())
                {
                    try
                    {
                        var invoke = DS.Invoke();
                        var user = invoke?.DiamondResponse?.ResponseData?.User;
                        //var user = DS.Invoke()?.DiamondResponse?.ResponseData?.User;
                        if (user != null)
                            return user.LoginName;
                        else
                            return string.Empty;
                    }
                    catch
                    {
#if DEBUG
                        // would expect it to fail if not logged in so you should only hit this once per session
                        // 10-16-2018 this new session variable should stop the execption - HttpContext.Current.Session["Diamond_DidLogin"]
                        Debugger.Break();
#endif
                        return string.Empty;
                    }

                }
            }
        }

        public static bool IsLoggedIN()
        {
            return HttpContext.Current.Session["Diamond_DidLogin"] != null && GetUserName() != string.Empty;
            //return HttpContext.Current.Session["Diamond_DidLogin"] != null;
            // sure you might have a username but is it still valid ?????
        }

        public static void LogOut()
        {
            HttpContext.Current.Session["DiamondUsername"] = null;
            HttpContext.Current.Session["DiamondUserId"] = null;
            HttpContext.Current.Session.Remove("Diamond_DidLogin");
            IDS.Common.SetDiamondToken(null);
        }



    }
}