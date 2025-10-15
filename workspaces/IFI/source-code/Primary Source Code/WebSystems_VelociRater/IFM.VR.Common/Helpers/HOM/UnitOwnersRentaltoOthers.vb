Imports QuickQuote.CommonObjects
Namespace IFM.VR.Common.Helpers.HOM
    Public Class UnitOwnersRentaltoOthers
        Private Shared _UnitOwnersRentaltoOthersSettings As NewFlagItem
        Public Shared ReadOnly Property UnitOwnersRentaltoOthersSettings() As NewFlagItem
            Get
                If _UnitOwnersRentaltoOthersSettings Is Nothing Then
                    _UnitOwnersRentaltoOthersSettings = New NewFlagItem("VR_HOM_UnitOwnersRentaltoOthers_Settings")
                End If
                Return _UnitOwnersRentaltoOthersSettings
            End Get
        End Property

        Public Shared Function UnitOwnersRentaltoOthersEnabled() As Boolean
            Return UnitOwnersRentaltoOthersSettings.EnabledFlag
        End Function

        Public Shared Function UnitOwnersRentaltoOthersEffDate() As Date
            Return UnitOwnersRentaltoOthersSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateUnitOwnersRentaltoOthers(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsUnitOwnersRentaltoOthersAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UnitOwnersRentaltoOthersSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
