using System;
using System.Collections.Generic;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public static class General
    {
        public static bool SendEmail(IFM.DataServicesCore.CommonObjects.EmailDocument doc)
        {
            return SendEmail(doc.ToAddress, doc.FromAddress, doc.Subject, doc.Body, "", "", null);
        }

        public static bool SendEmail(string toaddress, string @from, string subject, string body)
        {
            return SendEmail(toaddress, from, subject, body, "", "", null);
        }

        public static bool SendEmail(string toaddress, string @from, string subject, string body, string ccaddress, string bccaddress)
        {
            return SendEmail(toaddress, from, subject, body, ccaddress, bccaddress, null);
        }

        public static bool SendEmail(string toaddress, string @from, string subject, string body, string ccaddress, string bccaddress, System.Net.Mail.Attachment attachment)
        {
            using (EmailObject email = new EmailObject(System.Configuration.ConfigurationManager.AppSettings["RelayMailhost"]))
            {
                if (string.IsNullOrEmpty(toaddress.Trim()) == false)
                {
                    email.EmailFromAddress = @from;
                    email.EmailToAddresses = new System.Collections.ArrayList();
                    email.EmailToAddresses.AddRange(toaddress.Split(';'));
                    if (String.IsNullOrWhiteSpace(ccaddress) == false)
                    {
                        email.EmailCCAddresses = new System.Collections.ArrayList();
                        email.EmailCCAddresses.AddRange(ccaddress.Split(';'));
                    }
                    if (String.IsNullOrWhiteSpace(bccaddress) == false)
                    {
                        email.EmailBCCAddresses = new System.Collections.ArrayList();
                        email.EmailBCCAddresses.AddRange(bccaddress.Split(';'));
                    }
                    email.EmailBody = body;
                    email.EmailSubject = subject;
                    email.EmailAttachment = attachment;
                    email.SendEmail();

                    if (email.hasError)
                    {
                        IFM.IFMErrorLogging.LogIssue(email.errorMsg, "IFMDataServices -> BussinessLogic -> OMP -> General.cs -> SendEmail");
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}