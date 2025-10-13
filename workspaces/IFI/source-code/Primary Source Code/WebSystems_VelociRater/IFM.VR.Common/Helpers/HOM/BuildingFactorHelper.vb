Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM
    Public Class BuildingFactorHelper
        Private Shared _BuildingFactorSettings As NewFlagItem
        Public Shared ReadOnly Property BuildingFactorSettings() As NewFlagItem
            Get
                If _BuildingFactorSettings Is Nothing Then
                    _BuildingFactorSettings = New NewFlagItem("VR_HOM_BuildingFactor_Settings")
                End If
                Return _BuildingFactorSettings
            End Get
        End Property

        Public Shared Function BuildingFactorEnabled() As Boolean
            Return BuildingFactorSettings.EnabledFlag
        End Function

        Public Shared Function BuildingFactorEffDate() As Date
            Return BuildingFactorSettings.GetStartDateOrDefault("9/1/2023")
        End Function

        Public Shared Sub UpdateBuildingFactor(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsBuildingFactorAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, BuildingFactorSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
