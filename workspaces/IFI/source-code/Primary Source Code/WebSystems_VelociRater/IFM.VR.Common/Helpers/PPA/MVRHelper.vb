Imports System.Data.SqlClient
Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.PPA

    Public Class MVRHelper

        'This helper is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        Public Shared Function QuoteHasNoHitMVRRecord(policyid As String) As Boolean
            Dim hasNoHit As Boolean = False
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                Using cmd As New SqlCommand()

                    conn.Open()
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "usp_Get_MVR_Statuses"
                    cmd.Parameters.AddWithValue("@PolicyId", policyid)

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                If (CBool(reader.GetInt32(0))) Then
                                    hasNoHit = True
                                    Exit While
                                End If
                            End While
                        End If
                    End Using
                End Using
            End Using

            Return hasNoHit
        End Function

        Public Shared Function HaveMVRReportForDriver(ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal DriverNum As String) As Boolean
            Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML
            Dim MVRData As String = ""
            Dim MVRErr As String = ""
            Dim MVRReportData As Diamond.Common.Objects.ThirdParty.ReportObjects.MVR.MVRReportData = Nothing
            Dim pdffile As Object = Nothing
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Try
                ' Call the service to get the MVR data
                MVRReportData = qqxml.GetMvrReportDataForDriver(topQuote, DriverNum, MVRData, MVRErr)

                ' Got Data? Errors?
                If MVRErr Is Nothing OrElse MVRErr = "" Then
                    ' No errors, check return data
                    If MVRReportData Is Nothing OrElse MVRReportData.Equals(New Diamond.Common.Objects.ThirdParty.ReportObjects.MVR.MVRReportData()) Then
                        Return False
                    Else
                        If MVRReportData.DrivingRecords IsNot Nothing AndAlso MVRReportData.DrivingRecords.Count > 0 Then
                            If MVRReportData.DrivingRecords(0).Description.ToLower().Trim() = "MVR RECORD NOT FOUND".ToLower() Then
                                Return False
                            End If
                        End If
                    End If
                End If
            Catch

            End Try
            Return True
        End Function

        Public Shared Function AllRatedDriversHaveaMVRReport(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Drivers IsNot Nothing And topQuote.Drivers.Any() Then
                    Dim driverNum As Integer = 1
                    For Each driver In topQuote.Drivers
                        If driver.DriverExcludeTypeId = "1" OrElse driver.DriverExcludeTypeId = "2" Then
                            If HaveMVRReportForDriver(topQuote, driverNum) = False Then
                                Return False
                            End If
                        End If
                        driverNum += 1
                    Next

                End If
            End If
            Return True
        End Function

    End Class

End Namespace