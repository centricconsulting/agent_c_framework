Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports PopupMessageClass

Namespace IFM.VR.Common.Helpers.BOP
    Public Class MinBldgLimitsHelper
        Public Shared Sub CheckWindHailMinimumLimits(Quote As QuickQuoteObject, locIndex As Integer, myPage As Web.UI.Page)
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                'For Each l In Quote.Locations
                If Quote.Locations.HasItemAtIndex(locIndex) Then
                    Dim l As QuickQuoteLocation = Quote.Locations(locIndex)
                    Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
                    If l IsNot Nothing AndAlso (Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote OrElse (Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso qqh.IsQuickQuoteLocationNewToImage(l, Quote))) Then
                        Dim locationLimitTotal As Integer = 0
                        Dim showMinReqdLimitMsg As Boolean = False
                        Dim strPercent As String = String.Empty
                        Dim strMinLimit As String = String.Empty
                        Dim strMinReqdLimitMsg As String = String.Empty
                        If IsNullEmptyorWhitespace(l.WindHailDeductibleLimitId) = False AndAlso l.WindHailDeductibleLimitId <> 0 Then
                            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                For Each b As QuickQuoteBuilding In l.Buildings
                                    Dim buildingLimit As Integer = qqh.IntegerForString(b.Limit)
                                    Dim personalPropertyLimit As Integer = qqh.IntegerForString(b.PersonalPropertyLimit)
                                    If buildingLimit > 0 Then
                                        locationLimitTotal += buildingLimit
                                    End If
                                    If personalPropertyLimit > 0 Then
                                        locationLimitTotal += personalPropertyLimit
                                    End If
                                Next
                            End If

                            Select Case l.WindHailDeductibleLimitId
                                Case "222"
                                    '1% Wind Hail Deductible
                                    Select Case l.PropertyDeductibleId
                                        Case "22"
                                            '$500 Property Deductible, limit total must be >= $50,000
                                            If locationLimitTotal < 50000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "50,000"
                                            End If
                                        Case "24"
                                            '$1,000 Property Deductible, limit total must be >= $100,000
                                            If locationLimitTotal < 100000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "100,000"
                                            End If
                                        Case "75"
                                            '$2,500 Property Deductible, limit total must be >= $250,000
                                            If locationLimitTotal < 250000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "250,000"
                                            End If
                                        Case "76"
                                            '$5,000 Property Deductible, limit total must be >= $500,000
                                            If locationLimitTotal < 500000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "500,000"
                                            End If
                                        Case "333"
                                            '$7,500 Property Deductible, limit total must be >= $750,000
                                            If locationLimitTotal < 750000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "750,000"
                                            End If
                                        Case "157"
                                            '$10,000 Property Deductible, limit total must be >= $1,000,000
                                            If locationLimitTotal < 1000000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "1,000,000"
                                            End If
                                    End Select
                                Case "223"
                                    '2% Wind Hail Deductible
                                    Select Case l.PropertyDeductibleId
                                        Case "22"
                                            '$500 Property Deductible, limit total must be >= $25,000
                                            If locationLimitTotal < 25000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "25,000"
                                            End If
                                        Case "24"
                                            '$1,000 Property Deductible, limit total must be >= $50,000
                                            If locationLimitTotal < 50000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "50,000"
                                            End If
                                        Case "75"
                                            '$2,500 Property Deductible, limit total must be >= $125,000
                                            If locationLimitTotal < 125000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "125,000"
                                            End If
                                        Case "76"
                                            '$5,000 Property Deductible, limit total must be >= $250,000
                                            If locationLimitTotal < 250000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "250,000"
                                            End If
                                        Case "333"
                                            '$7,500 Property Deductible, limit total must be >= $375,000
                                            If locationLimitTotal < 375000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "375,000"
                                            End If
                                        Case "157"
                                            '$10,000 Property Deductible, limit total must be >= $500,000
                                            If locationLimitTotal < 500000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "500,000"
                                            End If
                                    End Select
                                Case "224"
                                    '5% Wind Hail Deductible
                                    Select Case l.PropertyDeductibleId
                                        Case "22"
                                            '$500 Property Deductible, limit total must be >= $10,000
                                            If locationLimitTotal < 10000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "10,000"
                                            End If
                                        Case "24"
                                            '$1,000 Property Deductible, limit total must be >= $20,000
                                            If locationLimitTotal < 20000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "20,000"
                                            End If
                                        Case "75"
                                            '$2,500 Property Deductible, limit total must be >= $50,000
                                            If locationLimitTotal < 50000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "50,000"
                                            End If
                                        Case "76"
                                            '$5,000 Property Deductible, limit total must be >= $100,000
                                            If locationLimitTotal < 100000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "100,000"
                                            End If
                                        Case "333"
                                            '$7,500 Property Deductible, limit total must be >= $150,000
                                            If locationLimitTotal < 150000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "150,000"
                                            End If
                                        Case "157"
                                            '$10,000 Property Deductible, limit total must be >= $200,000
                                            If locationLimitTotal < 200000 Then
                                                showMinReqdLimitMsg = True
                                                strMinLimit = "200,000"
                                            End If
                                    End Select
                            End Select
                            If showMinReqdLimitMsg Then
                                strPercent = qqh.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.WindHailDeductibleLimitId, l.WindHailDeductibleLimitId, Quote.LobType)
                                strMinReqdLimitMsg = "To select a " & strPercent & " wind hail deductible, the total combined Building and Business Personal Property limits for the location must be at least $" & strMinLimit & "."
                                Using popup As New PopupMessageObject(myPage, strMinReqdLimitMsg)
                                    With popup
                                        .Title = "Wind Hail Deductible"
                                        .isFixedPositionOnScreen = True
                                        .ZIndexOfPopup = 2
                                        .isModal = True
                                        .Image = PopupMessageObject.ImageOptions.None
                                        .hideCloseButton = True
                                        .AddButton("OK", True)
                                        .CreateDynamicPopUpWindow()
                                    End With
                                End Using

                                'set Wind Hail Limit to N/A
                                l.WindHailDeductibleLimitId = "0"
                            End If
                        End If
                    End If
                    'Next
                End If
            End If
        End Sub
    End Class
End Namespace
