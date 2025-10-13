Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls

Namespace IFM.VR.Common.Helpers.FARM
    Public Class CosmeticDamageExHelper
        Private Shared _CosmeticDamageExSettings As NewFlagItem
        Public Shared ReadOnly Property CosmeticDamageExSettings() As NewFlagItem
            Get
                If _CosmeticDamageExSettings Is Nothing Then
                    _CosmeticDamageExSettings = New NewFlagItem("VR_Far_CosmeticDamageExHelper_Settings")
                End If
                Return _CosmeticDamageExSettings
            End Get
        End Property


        Public Shared Function CosmeticDamageExEnabled() As Boolean
            Return CosmeticDamageExSettings.EnabledFlag
        End Function

        Public Shared Function CosmeticDamageExEffDate() As Date
            Return CosmeticDamageExSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateCosmeticDamageEx(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
            End Select
        End Sub

        Public Shared Function IsCosmeticDamageExAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CosmeticDamageExSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
