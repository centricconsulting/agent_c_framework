Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.PPA
    Public Class CustomEquipmentHelper
        Private Shared _CustomEquipmentSettings As NewFlagItem
        Public Shared ReadOnly Property CustomEquipmentSettings() As NewFlagItem
            Get
                If _CustomEquipmentSettings Is Nothing Then
                    _CustomEquipmentSettings = New NewFlagItem("VR_PPA_CustomEquipment_Settings")
                End If
                Return _CustomEquipmentSettings
            End Get
        End Property

        Public Shared Function CustomEquipmentEnabled() As Boolean
            Return CustomEquipmentSettings.EnabledFlag
        End Function

        Public Shared Function CustomEquipmentEffDate() As Date
            Return CustomEquipmentSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateCustomEquipment(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsCustomEquipmentAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CustomEquipmentSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
