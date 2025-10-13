Imports System.Globalization
Imports System.Linq
Imports System.Text.RegularExpressions
Imports Diamond.Common.Enums.VehicleInfoLookupType
Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonMethods


Namespace IFM.VR.Common.Helpers

    Public Class QuickQuoteObjectHelper

        ''' <summary>
        ''' This checks the first name of each policyholder object. If the firstname isn't empty it will count that as a policyholder.
        ''' </summary>
        ''' <param name="topQuote"></param>
        ''' <returns></returns>
        Public Shared Function PolicyHolderCount(ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Int32
            Dim phcount As Int32 = 0

            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Policyholder IsNot Nothing Then
                    If topQuote.Policyholder.Name IsNot Nothing Then
                        If String.IsNullOrWhiteSpace(topQuote.Policyholder.Name.FirstName) = False Then
                            phcount += 1
                        End If
                    End If
                End If

                If topQuote.Policyholder2 IsNot Nothing Then
                    If topQuote.Policyholder2.Name IsNot Nothing Then
                        If String.IsNullOrWhiteSpace(topQuote.Policyholder2.Name.FirstName) = False Then
                            phcount += 1
                        End If
                    End If
                End If

            End If

            Return phcount
        End Function

        Public Shared Function UserHasAgencyIdAccess(agencyID As String) As Boolean
            Dim qqHelper As New QuickQuoteHelperClass()
            Return qqHelper.IsAgencyIdOkayForUser(agencyID)
        End Function

        Public Shared Function IsStaff() As Boolean
            Dim qqHelper As New QuickQuoteHelperClass()
            Return qqHelper.IsHomeOfficeStaffUser()
        End Function

        ''' <summary>
        ''' Returns a count of vehicles that are motorized
        ''' </summary>
        ''' <param name="vehicles">a List(Of QuickQuote.CommonObjects.QuickQuoteVehicle)</param>
        ''' <returns>the number of vehicles that are motorized in a list </returns>
        Public Shared Function GetMotorizedVehicleCount(vehicles As List(Of QuickQuote.CommonObjects.QuickQuoteVehicle)) As Int32
            Dim Count = 0
            If vehicles IsNot Nothing AndAlso vehicles.Count > 0 Then
                For Each vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle In vehicles
                    If String.IsNullOrWhiteSpace(vehicle.ClassCode) = False AndAlso vehicle.ClassCode.Substring(0, 1) = "6" Then
                        Continue For
                    Else
                        Count += 1
                    End If
                Next
            End If
            Return Count
        End Function

        ''' <summary>
        ''' Takes a list of strings formatted as Money sums them and returns the result as a string formatted as Money
        ''' </summary>
        ''' <param name="items">Money Strings as a List</param>
        ''' <returns></returns>
        Public Shared Function SumMoneyStrings(items As List(Of String)) As String
            Dim sum = 0
            For Each number As String In items
                Dim result = 0
                If Decimal.TryParse(number, NumberStyles.Currency, CultureInfo.CurrentCulture, result) Then
                    sum += result
                End If
            Next
            Return sum.ToString("c")
        End Function

        Public Shared Sub CheckQuoteForKillorStopEvent(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, page As System.Web.UI.Page, response As System.Web.HttpResponse, session As System.Web.SessionState.HttpSessionState)
            ' New Logic to test for killed quotes
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim ShowMsg = False
                Dim bodyMsg = String.Empty
                Dim titleMsg = "Important Notice"

                ' Set Message
                If topQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled OrElse topQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled OrElse topQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped OrElse topQuote.QuoteStatus = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped Then
                    Select Case topQuote.QuoteStatus
                        Case QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteKilled
                            Dim msg As QuickQuote.CommonObjects.QuickQuoteMessage = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteMessageForMessageParams(topQuote.Messages, msgType:=QuickQuote.CommonMethods.QuickQuoteXML.QuoteMessageType.QuoteKilled, active:=True)
                            If msg IsNot Nothing Then
                                bodyMsg = msg.QuoteMessageText
                            Else
                                bodyMsg = "This quote will need to be submitted to your underwriter for review and consideration."
                            End If
                            ShowMsg = True
                        Case QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppKilled
                            Dim msg As QuickQuote.CommonObjects.QuickQuoteMessage = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteMessageForMessageParams(topQuote.Messages, msgType:=QuickQuote.CommonMethods.QuickQuoteXML.QuoteMessageType.AppKilled, active:=True)
                            If msg IsNot Nothing Then
                                bodyMsg = msg.QuoteMessageText
                            Else
                                bodyMsg = "This quote will need to be submitted to your underwriter for review and consideration."
                            End If
                            ShowMsg = True
                        Case QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteStopped
                            Dim msg As QuickQuote.CommonObjects.QuickQuoteMessage = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteMessageForMessageParams(topQuote.Messages, msgType:=QuickQuote.CommonMethods.QuickQuoteXML.QuoteMessageType.QuoteStopped, active:=True)
                            If msg IsNot Nothing Then
                                bodyMsg = msg.QuoteMessageText
                            Else
                                bodyMsg = "This quote is not eligible for coverage with Indiana Farmers Mutual Insurance Company."
                            End If
                            ShowMsg = True
                        Case QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppStopped
                            Dim msg As QuickQuote.CommonObjects.QuickQuoteMessage = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteMessageForMessageParams(topQuote.Messages, msgType:=QuickQuote.CommonMethods.QuickQuoteXML.QuoteMessageType.AppStopped, active:=True)
                            If msg IsNot Nothing Then
                                bodyMsg = msg.QuoteMessageText
                            Else
                                bodyMsg = "This quote is not eligible for coverage with Indiana Farmers Mutual Insurance Company."
                            End If
                            ShowMsg = True

                    End Select
                    If ShowMsg Then
                        session.Add("QuoteStopOrKill_ShowMsg", ShowMsg)
                        session.Add("QuoteStopOrKill_titleMsg", titleMsg)
                        session.Add("QuoteStopOrKill_bodyMsg", bodyMsg)
                    End If

                End If

                If ShowMsg Then
                    response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage"))
                End If
            End If
        End Sub

        Public Shared Function GetCurrentVersionId(ByVal LobId As String, Optional ByVal DateToCheck As String = Nothing) As String
            Dim conn As New System.Data.SqlClient.SqlConnection()
            Dim cmd As New System.Data.SqlClient.SqlCommand
            Dim rtn As String = Nothing

            conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings("connQQ")
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "usp_VR_GetLOBVersionId"
            cmd.Parameters.AddWithValue("@LobId", LobId)
            ' If no date is passed use the current date
            If DateToCheck IsNot Nothing Then
                cmd.Parameters.AddWithValue("@InDate", DateToCheck)
            Else
                cmd.Parameters.AddWithValue("@InDate", DateTime.Now.ToShortDateString())
            End If

            rtn = cmd.ExecuteScalar()
            If rtn IsNot Nothing Then
                Return rtn.ToString()
            Else
                Return "-1"
            End If
        End Function


    End Class


End Namespace