Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls

Namespace IFM.VR.Common.Helpers.FARM
    Public Class ILMineSubsidenceHelper
        Private Shared _ILMineSubsidenceSettings As NewFlagItem
        Public Shared ReadOnly Property ILMineSubsidenceSettings() As NewFlagItem
            Get
                If _ILMineSubsidenceSettings Is Nothing Then
                    _ILMineSubsidenceSettings = New NewFlagItem("VR_Far_ILMineSubsidenceHelper_Settings")
                End If
                Return _ILMineSubsidenceSettings
            End Get
        End Property


        Public Shared Function ILMineSubsidenceEnabled() As Boolean
            Return ILMineSubsidenceSettings.EnabledFlag
        End Function

        Public Shared Function ILMineSubsidenceEffDate() As Date
            Return ILMineSubsidenceSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateILMineSubsidence(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
            End Select
        End Sub

        Public Shared Function IsILMineSubsidenceAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, ILMineSubsidenceSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
