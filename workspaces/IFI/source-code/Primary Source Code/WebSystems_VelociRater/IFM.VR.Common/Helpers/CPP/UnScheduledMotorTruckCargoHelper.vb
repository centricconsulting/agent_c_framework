Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CPP
    Public Class UnScheduledMotorTruckCargoHelper
        Private Shared _UnScheduledMotorTruckCargoSettings As NewFlagItem
        Public Shared ReadOnly Property UnScheduledMotorTruckCargoSettings() As NewFlagItem
            Get
                If _UnScheduledMotorTruckCargoSettings Is Nothing Then
                    _UnScheduledMotorTruckCargoSettings = New NewFlagItem("VR_CPP_CIM_UnScheduledMotorTruckCargo_Settings")
                End If
                Return _UnScheduledMotorTruckCargoSettings
            End Get
        End Property

        Const UnScheduledMotorTruckCargoUpdatedMsg As String = ""

        Public Shared Function UnScheduledMotorTruckCargoEnabled() As Boolean
            Return UnScheduledMotorTruckCargoSettings.EnabledFlag
        End Function

        Public Shared Function UnScheduledMotorTruckCargoEffDate() As Date
            Return UnScheduledMotorTruckCargoSettings.GetStartDateOrDefault("9/1/2025")
        End Function

        Public Shared Sub UpdateUnScheduledMotorTruckCargo(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing Then
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                End Select
            End If
        End Sub

        Public Shared Function IsUnScheduledMotorTruckCargoAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                UnScheduledMotorTruckCargoSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UnScheduledMotorTruckCargoSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
