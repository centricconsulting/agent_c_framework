Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class TrampolineUnitsHelper

        Private Shared _TrampolineSettings As NewFlagItem
        Public Shared ReadOnly Property TrampolineSettings() As NewFlagItem
            Get
                If _TrampolineSettings Is Nothing Then
                    _TrampolineSettings = New NewFlagItem("VR_Far_TrampolineUnits_Settings")
                End If
                Return _TrampolineSettings
            End Get
        End Property

        Const TrampolineWarningMsg As String = "The number of Trampolines for location one have been set to one. Please update the quote locations page, for all locations, if more than one are present."
        Const TrampolineRemovedMsg As String = "Rating for Trampoline Surcharge has changed based on the effective date."
        Public Shared Sub UpdateTrampolineUnits(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Added Coverage Possibility
                    For Each l In Quote.Locations
                        If l.TrampolineSurcharge = True Then
                            NeedsWarningMessage = True
                            If l.TrampolineSurcharge_NumberOfUnits < 1 Then
                                l.TrampolineSurcharge_NumberOfUnits = 1

                            End If
                        End If
                    Next
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(TrampolineWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Coverage Possibility
                    For Each l In Quote.Locations

                        If Quote.Locations.IndexOf(l) > 0 Then
                            l.TrampolineSurcharge = False
                        End If

                        l.TrampolineSurcharge_NumberOfUnits = 0

                        If l.TrampolineSurcharge Then
                            NeedsWarningMessage = True
                        End If
                    Next
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(TrampolineRemovedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If

            End Select
        End Sub

        Public Shared Function isTrampolineUnitsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
                Dim IsCorrectProgramType As Boolean = SubQuoteFirst?.ProgramTypeId = "6" 'FO only

                TrampolineSettings.OtherQualifiers = IsCorrectProgramType AndAlso IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, TrampolineSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function
    End Class

End Namespace
