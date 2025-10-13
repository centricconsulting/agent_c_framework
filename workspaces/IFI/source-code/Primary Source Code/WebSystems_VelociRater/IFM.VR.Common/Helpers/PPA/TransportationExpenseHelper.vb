Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.PPA
    Public Class TransportationExpenseHelper
        Private Shared _TransportationExpenseSettings As NewFlagItem
        Public Shared ReadOnly Property TransportationExpenseSettings() As NewFlagItem
            Get
                If _TransportationExpenseSettings Is Nothing Then
                    _TransportationExpenseSettings = New NewFlagItem("VR_PPA_TransportationExpense_Settings")
                End If
                Return _TransportationExpenseSettings
            End Get
        End Property

        'Added 8/9/2023 for task WS-
        Const TransportationExpenseWarningMsg As String = "Please verify Transportation Expense Coverages, it may have been removed or reduced due to the change of effective date."
        Public Shared Function TransportationExpenseEnabled() As Boolean
            Return TransportationExpenseSettings.EnabledFlag
        End Function

        Public Shared Function TransportationExpenseEffDate() As Date
            Return TransportationExpenseSettings.GetStartDateOrDefault("12/1/2023")
        End Function

        Public Shared Sub UpdateTransportationExpense(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    For Each l In Quote.Vehicles
                        If Quote.HasAutoPlusEnhancement Then
                            l.TransportationExpenseLimitId = "447"
                        ElseIf Quote.HasBusinessMasterEnhancement Then
                            l.TransportationExpenseLimitId = "446"
                        Else
                            l.TransportationExpenseLimitId = "446"
                        End If
                    Next
                  
                    If ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(TransportationExpenseWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove Number of units, but still keep checked if checked before
                     For Each l In Quote.Vehicles
                        If Quote.HasAutoPlusEnhancement Then
                            l.TransportationExpenseLimitId = "127"
                        ElseIf Quote.HasBusinessMasterEnhancement Then
                            l.TransportationExpenseLimitId = "80"
                        Else
                            l.TransportationExpenseLimitId = "0"
                        End If
                    Next
                    If ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(TransportationExpenseWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsTransportationExpenseAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal

                TransportationExpenseSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, TransportationExpenseSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace