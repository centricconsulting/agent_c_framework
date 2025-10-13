Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls
Namespace IFM.VR.Common.Helpers.FARM
    Public Class GlassBreakageForCabs

        Private Shared _GlassBreakageForCabs As NewFlagItem
        Public Shared ReadOnly Property GlassBreakageForCabs() As NewFlagItem
            Get
                If _GlassBreakageForCabs Is Nothing Then
                    _GlassBreakageForCabs = New NewFlagItem("VR_Far_GlassBreakageForCabs_Settings")
                End If
                Return _GlassBreakageForCabs
            End Get
        End Property

        Const GlassBreakageForCabsWarningMsg As String = ""
        Public Shared Function GlassBreakageForCabsEnabled() As Boolean
            Return GlassBreakageForCabs.EnabledFlag
        End Function

        Public Shared Function GlassBreakageForCabsEffDate() As Date
            Return GlassBreakageForCabs.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateGlassBreakageForCabs(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Allow coverage                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove coverage
            End Select
        End Sub

        Public Shared Function IsGlassBreakageForCabsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim SubQuoteFirst As QuickQuoteObject
                If quote.MultiStateQuotes.IsLoaded Then
                    SubQuoteFirst = quote.MultiStateQuotes(0)
                Else
                    SubQuoteFirst = quote
                End If
                GlassBreakageForCabs.OtherQualifiers = SubQuoteFirst.ProgramTypeId = "6" OrElse SubQuoteFirst.ProgramTypeId = "7"
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, GlassBreakageForCabs, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class

End Namespace