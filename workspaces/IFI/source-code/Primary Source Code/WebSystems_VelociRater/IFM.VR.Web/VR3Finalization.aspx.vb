Imports QuickQuote.CommonMethods

Public Class VR3Finalization
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Request.QueryString("quoteids") IsNot Nothing Then
            If Request.QueryString("quoteids").Contains("|") Then
                BuildIframes(Request.QueryString("quoteids").Split("|"))
            Else
                BuildIframes(New String() {Request.QueryString("quoteids")})
            End If
        End If
    End Sub

    Private Sub BuildIframes(quoteIds As String())
        Dim html As String = ""
        For Each quoteid In quoteIds
            Dim quote = IFM.VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(quoteid)
            If quote IsNot Nothing Then
#If Not DEBUG Then ' do not attempt when running on dev machine it will fail to take you to the makeapayment.aspx page
                    Dim newPolicyNumber As String = ""
                    If PromoteQuote(quoteid,quote, newPolicyNumber) Then
                        ' remove this quote from recent quote log
                        Helpers.WebHelper_Personal.RemoveQuoteIdFromSessionHistory(Session, quoteid)

                        html += BuildFrame(newPolicyNumber, quoteid)
                    Else
                        html += "<h3>" + newPolicyNumber + "(" + quote.QuoteNumber + ")</h3>"
                        html += "<div>"
                        html += "This application failed promotion."
                        html += "</div>"
                    End If
#End If
            End If
        Next
        Me.litFrames.Text = html
    End Sub

    Private Function BuildFrame(policyNumber As String, quoteId As String) As String

        Dim iFrameUrl As String = System.Configuration.ConfigurationManager.AppSettings("MakeAPaymentLink") + policyNumber + "&quote=Yes&frame=y"
#If DEBUG Then
        iFrameUrl = "myvelocirater.aspx" ' the other would fail to load when running locally
#End If
        Dim html As String = ""
        html += "<h3>" + policyNumber + "</h3>"
        html += "<div>"
        html += "<iframe style=""width: 100%; height: 600px;"" src=""" + iFrameUrl + """></iframe>"
        html += "</div>"
        Return html
    End Function

    Private Function PromoteQuote(quoteid As String, quote As QuickQuote.CommonObjects.QuickQuoteObject, ByRef newPolicyNumber As String) As Boolean
        Dim QQxml As New QuickQuoteXML()
        Dim policyNumber As String = ""
        Dim errorMsg As String = ""
        If QQxml.DiamondService_SuccessfullyPromotedQuote(quote, policyNumber, "", errorMsg) Then
            newPolicyNumber = policyNumber
            Return True
        End If
        Return False
    End Function

End Class