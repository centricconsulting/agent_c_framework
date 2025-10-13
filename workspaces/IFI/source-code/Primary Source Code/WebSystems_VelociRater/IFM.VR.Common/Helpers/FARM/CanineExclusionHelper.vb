Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class CanineExclusionHelper
        Private Shared _CanineSettings As NewFlagItem
        Public Shared ReadOnly Property CanineSettings() As NewFlagItem
            Get
                If _CanineSettings Is Nothing Then
                    _CanineSettings = New NewFlagItem("VR_Far_Canine_Settings")
                End If
                Return _CanineSettings
            End Get
        End Property

        Const CanineExclusionWarningMsg As String = "Canine Exclusion coverage is unavailable before 09/01/22. The coverage has been removed."
        Public Shared Sub UpdateCanineExclusion(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim MyLocation As QuickQuoteLocation = Quote.Locations(0)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Added Coverage Possibility
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Coverage Possibility
                    If MyLocation.SectionIICoverages IsNot Nothing AndAlso MyLocation.SectionIICoverages.FindAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion).IsLoaded Then
                        MyLocation.SectionIICoverages.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)
                        NeedsWarningMessage = True
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(CanineExclusionWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If

                    'MyLocation.SectionIICoverages.RemoveAll(Function(sc) sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)
            End Select
        End Sub

        Public Shared Function isCanineExclusionAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CanineSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False


        End Function
    End Class



End Namespace
