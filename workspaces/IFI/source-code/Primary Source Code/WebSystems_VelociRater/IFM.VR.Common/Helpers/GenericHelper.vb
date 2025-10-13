Imports CommonHelperClass
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers
    Public Class GenericHelper

        Private Shared chc As New CommonHelperClass

        Private Shared Function GetLOBsThatRequireEffectiveDateAtQuoteStart() As List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
            Dim LOBs As New List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
            Dim LOBsStringList As New List(Of String)
            Dim LobString As String = chc.ConfigurationAppSettingValueAsString("LOBsThatRequireEffectiveDateAtQuoteStart")
            If String.IsNullOrWhiteSpace(LobString) = False Then
                If LobString.Contains(",") Then
                    LOBsStringList = LobString.Split(",").ToList()
                Else
                    LOBsStringList.Add(LobString)
                End If

                For Each LOBType As System.Enum In System.Enum.GetValues(GetType(QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType))
                    If LOBsStringList.Contains(LOBType.ToString()) Then
                        LOBs.Add(LOBType)
                    End If
                Next
            End If

            Return LOBs
        End Function

        Public Shared Function LOBRequiresEffectiveDateAtQuoteStart(LOBType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As Boolean
            Dim returnVar As Boolean = False

            If LOBType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None Then
                Dim LOBs As List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) = GetLOBsThatRequireEffectiveDateAtQuoteStart()
                If LOBs.Contains(LOBType) Then
                    returnVar = True
                End If
            End If

            Return returnVar
        End Function

        Public Shared Function GetAppSettingsValueForString(KeyName As String, ByRef Value As String) As Boolean
            Dim chc As New CommonHelperClass
            Dim keyExists As Boolean = False
            Dim myVal As String = chc.ConfigurationAppSettingValueAsString(KeyName, keyExists)

            If keyExists AndAlso myVal <> Nothing Then
                Value = myVal
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function GetAppSettingsValueForInteger(KeyName As String, ByRef Value As Integer) As Boolean
            Dim chc As New CommonHelperClass
            Dim keyExists As Boolean = False
            Dim myVal As Integer = chc.ConfigurationAppSettingValueAsInteger(KeyName, keyExists)

            If keyExists AndAlso myVal <> Nothing Then
                Value = myVal
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function GetAppSettingsValueForBoolean(KeyName As String, ByRef Value As Boolean) As Boolean
            Dim chc As New CommonHelperClass
            Dim keyExists As Boolean = False
            Dim myVal As Boolean = chc.ConfigurationAppSettingValueAsBoolean(KeyName, keyExists)

            If keyExists AndAlso myVal <> Nothing Then
                Value = myVal
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function GetAppSettingsValueForDecimal(KeyName As String, ByRef Value As Decimal) As Boolean
            Dim chc As New CommonHelperClass
            Dim keyExists As Boolean = False
            Dim myVal As Decimal = chc.ConfigurationAppSettingValueAsDecimal(KeyName, keyExists)

            If keyExists AndAlso myVal <> Nothing Then
                Value = myVal
                Return True
            Else
                Return False
            End If
        End Function

        Private Shared lastSystemDateCheck As DateTime = DateTime.MinValue
        Private Shared lastSystemDateValue As DateTime = DateTime.MinValue
        Private Shared ReadOnly systemDateLockObject As New Object()
        Public Shared Function GetDiamondSystemDate() As DateTime
            Dim returnVar As DateTime = DateTime.Parse(DateTime.Now.ToShortDateString())

            SyncLock systemDateLockObject
                If lastSystemDateCheck = DateTime.MinValue OrElse lastSystemDateCheck < DateTime.Now.AddSeconds(-30) Then 'need to update cache?
                    lastSystemDateCheck = DateTime.Now
                    Dim myReader As System.Data.SqlClient.SqlDataReader

                    Using sql As New SQLselectObject(chc.ConfigurationAppSettingValueAsString("connDiamond"))
                        sql.queryOrStoredProc = "SELECT sysdate FROM SystemDate"
                        myReader = sql.GetDataReader()
                        If myReader IsNot Nothing AndAlso myReader.HasRows Then
                            Do While myReader.Read
                                Dim myDate As Date
                                If myReader.Item("sysdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(myReader.Item("sysdate").ToString()) = False AndAlso Date.TryParse(myReader.Item("sysdate").ToString(), myDate) Then
                                    returnVar = myDate
                                    lastSystemDateValue = returnVar 'set cache
                                End If
                            Loop
                        End If
                    End Using
                Else
                    returnVar = lastSystemDateValue 'return cached
                End If
            End SyncLock

            Return returnVar
        End Function

        Public Shared Function StopOnOrAfterDate(lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As DateTime
            Dim OnOrAfterDate As String = System.Configuration.ConfigurationManager.AppSettings("VR_" & lob.ToString() & "_StopQuoteRateByDate_DatesOnOrAfter")
            Dim myDate As DateTime = DateTime.MaxValue
            If OnOrAfterDate.NoneAreNullEmptyorWhitespace() AndAlso Information.IsDate(OnOrAfterDate) Then
                DateTime.TryParse(OnOrAfterDate, myDate)
            End If
            Return myDate
        End Function

        Public Shared Function StopOnOrAfterDateMsg(lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As String
            Dim msg As String = System.Configuration.ConfigurationManager.AppSettings($"VR_{lob}_StopQuoteRateByDate_DatesOnOrAfter_ErrorMessage")

            If msg.IsNullEmptyorWhitespace() Then
                msg = $"The effective date is currently locked to dates before {StopOnOrAfterDate(lob).ToShortDateString()}. Please select a date before this."
            End If
            Return msg
        End Function

        Public Shared Function StopOnOrBeforeDate(lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As DateTime
            Dim OnOrBeforeDate As String = System.Configuration.ConfigurationManager.AppSettings($"VR_{lob}_StopQuoteRateByDate_DatesOnOrBefore")
            Dim myDate As DateTime = DateTime.MinValue
            If OnOrBeforeDate.NoneAreNullEmptyorWhitespace() AndAlso Information.IsDate(OnOrBeforeDate) Then
                DateTime.TryParse(OnOrBeforeDate, myDate)
            End If
            Return myDate
        End Function

        Public Shared Function StopOnOrBeforeDateMsg(lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As String
            Dim msg As String = System.Configuration.ConfigurationManager.AppSettings($"VR_{lob}_StopQuoteRateByDate_DatesOnOrBefore_ErrorMessage")

            If msg.IsNullEmptyorWhitespace() Then
                msg = $"The effective date is currently locked to dates after {StopOnOrBeforeDate(lob).ToShortDateString()}. Please select a date after this."
            End If
            Return msg
        End Function

        ''' <summary>
        ''' Return the Ohio expansion effective date
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' DEPRECATED --  can remove after all references have been changed to  no longer needs this
        ''' </remarks>
        Public Shared Function GetOhioEffectiveDate() As DateTime
            If System.Configuration.ConfigurationManager.AppSettings("VR_OhioEffDate") IsNot Nothing AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_OhioEffDate")) Then
                Return CDate(System.Configuration.ConfigurationManager.AppSettings("VR_OhioEffDate"))
            Else
                Return CDate("1/1/1980")
            End If
        End Function

        'added 12/15/2022
        Public Shared Function SaveToDiamondOnNewBusinessRouteToUnderwriting() As Boolean
            Dim useSingleSQLQuery As Boolean = False
            If GetAppSettingsValueForBoolean("VR_SaveToDiamondOnNewBusinessRouteToUnderwriting", useSingleSQLQuery) AndAlso useSingleSQLQuery = True Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class
End Namespace
