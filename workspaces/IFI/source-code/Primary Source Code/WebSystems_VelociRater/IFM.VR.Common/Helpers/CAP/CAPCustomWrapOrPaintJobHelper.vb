Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CAP
    Public Class CAPCustomWrapOrPaintJobHelper
        Private Shared _CAPCustomWrapOrPaintJobHelperSettings As NewFlagItem
        Public Shared ReadOnly Property _CAPCustomWrapOrPaintJobHelperSettingsSettings() As NewFlagItem
            Get
                If _CAPCustomWrapOrPaintJobHelperSettings Is Nothing Then
                    _CAPCustomWrapOrPaintJobHelperSettings = New NewFlagItem("VR_CAP_CustomWrapOrPaintJobHelper_Settings")
                End If
                Return _CAPCustomWrapOrPaintJobHelperSettings
            End Get
        End Property

        Public Shared Sub UpdateCustomWrapOrPaintJobSettings(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsCustomWrapOrPaintJobAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, _CAPCustomWrapOrPaintJobHelperSettingsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
