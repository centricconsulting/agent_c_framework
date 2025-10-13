Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.PPA
    Public Class VehicleLookupPopupHelper
        Private Shared _VehicleLookupPopupSettings As NewFlagItem
        Public Shared ReadOnly Property VehicleLookupPopupSettings() As NewFlagItem
            Get
                If _VehicleLookupPopupSettings Is Nothing Then
                    _VehicleLookupPopupSettings = New NewFlagItem("VR_PPA_VehicleLookupPopup_Settings")
                End If
                Return _VehicleLookupPopupSettings
            End Get
        End Property

        Public Shared Sub UpdateVehicleLookupPopup(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsVehicleLookupPopupAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, VehicleLookupPopupSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
