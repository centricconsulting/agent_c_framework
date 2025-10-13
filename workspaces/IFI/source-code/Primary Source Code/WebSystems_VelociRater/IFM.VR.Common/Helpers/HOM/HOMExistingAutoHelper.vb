Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.HOM

    Public Class HOMExistingAutoHelper

        Const CalendarDateKey As String = "VR_HOM_HOMExistingAuto_CalendarDateKey"

        Private Shared _HOMExistingAutoSettings As NewFlagItem
        Public Shared ReadOnly Property HOMExistingAutoSettings() As NewFlagItem
            Get
                If _HOMExistingAutoSettings Is Nothing Then
                    _HOMExistingAutoSettings = New NewFlagItem("VR_HOM_HOMExistingAuto_Settings")
                End If
                Return _HOMExistingAutoSettings
            End Get
        End Property

        Public Shared Function HOMExistingAutoEnabled() As Boolean
            Return HOMExistingAutoSettings.EnabledFlag
        End Function

        Public Shared Function HOMExistingAutoEffDate() As Date
            Return HOMExistingAutoSettings.GetStartDateOrDefault("8/1/2024")
        End Function

        Public Shared Sub UpdateHOMExistingAuto(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD

                Case Helper.EnumsHelper.CrossDirectionEnum.BACK

            End Select
        End Sub

        Public Shared Function IsHOMExistingAutoAvailable(ByRef Quote As QuickQuoteObject)
            If Quote IsNot Nothing Then
                HOMExistingAutoSettings.OtherQualifiers = DoesQuoteQualifyByCalendarDate()
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(Quote, HOMExistingAutoSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        Private Shared Function DoesQuoteQualifyByCalendarDate() As Boolean
            Return DateTime.Now >= CalendarEffectiveDate()
        End Function

        Private Shared Function CalendarEffectiveDate() As DateTime
            Dim dateString As String = GetConfigStringForKey(CalendarDateKey)
            Return DateFromString(dateString)
        End Function

        Private Shared Function GetConfigStringForKey(key As String) As String
            Dim c As New CommonHelperClass
            Return c.ConfigurationAppSettingValueAsString(key)
        End Function

        Private Shared Function DateFromString(dateString As String) As Date
            Dim c As New CommonHelperClass
            If c.IsDateString(dateString) Then
                Return dateString.ToDateTime
            End If
            Return Date.MinValue
        End Function
    End Class
End Namespace

