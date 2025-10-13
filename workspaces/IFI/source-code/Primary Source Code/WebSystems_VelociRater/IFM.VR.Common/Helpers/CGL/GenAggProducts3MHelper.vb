Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports System.Web.UI.WebControls

Namespace IFM.VR.Common.Helpers.CGL
    Public Class GenAggProducts3MHelper
        Private Shared _ThreeMillOptionSettings As NewFlagItem
        Public Shared ReadOnly Property ThreeMillOptionSettings() As NewFlagItem
            Get
                If _ThreeMillOptionSettings Is Nothing Then
                    _ThreeMillOptionSettings = New NewFlagItem("VR_CGL_CPP_3M_Option_Settings")
                End If
                Return _ThreeMillOptionSettings
            End Get
        End Property

        'Added 6/14/2022 for task 72947 MLW
        Const ThreeMillOptionWarningMsg As String = "$3,000,000 is now an option in ..."
        Const ThreeMillOptionRemovedMsg As String = "$3,000,000 has been removed as an option ..."
        Public Shared Function ThreeMillOptionNumOfUnitsEnabled() As Boolean
            Return ThreeMillOptionSettings.EnabledFlag
        End Function

        Public Shared Function ThreeMillOptionNumOfUnitsEffDate() As Date
            Return ThreeMillOptionSettings.GetStartDateOrDefault("7/1/2022")
        End Function

        Public Shared Sub UpdateThreeMillOptionStove(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    NeedsWarningMessage = False 'No Message Required
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(ThreeMillOptionWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    For Each sq In SubQuotesFor(Quote)
                        sq.GeneralAggregateLimitId = "65"
                        sq.ProductsCompletedOperationsAggregateLimitId = "65"
                        NeedsWarningMessage = False 'No Message Required
                    Next
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(ThreeMillOptionRemovedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsThreeMillOptionNumOfUnitsAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, ThreeMillOptionSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

        Public Shared Sub RemoveDropDownOptionIfNecessary(quote As QuickQuoteObject, ByRef ddl As DropDownList)
            If IsThreeMillOptionNumOfUnitsAvailable(quote) = False Then
                'remove $3M if unavailable.
                Dim Item = New ListItem("3,000,000", "167")
                ddl.Items.Remove(Item)
            End If
        End Sub

    End Class
End Namespace
