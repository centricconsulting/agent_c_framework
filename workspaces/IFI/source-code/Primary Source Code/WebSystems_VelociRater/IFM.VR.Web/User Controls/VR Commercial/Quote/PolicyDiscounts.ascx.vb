Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteObject

Public Class PolicyDiscounts
    Inherits VRControlBase

    Const COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT = "80558"
    Protected discounts As New List(Of SummaryPremiumDataItem)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True
    End Sub

    Protected Structure SummaryPremiumDataItem
        Public Property Description As String
        Public Property Limit As String
        Public Property Premium As Decimal
    End Structure

    Public Overrides Sub Populate()
        Populate(QuickQuoteLobType.None)
    End Sub
    Public Overloads Sub Populate(Optional ByVal packagePartLobType As QuickQuoteLobType = QuickQuoteLobType.None)

        Try
            If Me.GoverningStateQuote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then

                Dim LPDP_Coverage As QuickQuoteCoverage = Nothing

                If packagePartLobType = QuickQuoteLobType.None Then
                    'todo ! work-around becuase Workflow manager is calling populate on specific controls as well as all of it's child controls, causing them to be rendered twice.
                    'todo ! we need to fix this condition
                    discounts.Clear()
                    If Me.GoverningStateQuote.LobType = QuickQuoteLobType.CommercialPackage Then

                        Populate(QuickQuoteLobType.CommercialProperty)
                        Populate(QuickQuoteLobType.CommercialGeneralLiability)
                    Else
                        LPDP_Coverage = SummaryHelperClass.Find_First_PolicyLevelCoverage(SubQuotes, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
                    End If
                Else
                    Dim ppList = SubQuotes.SelectMany(Of QuickQuotePackagePart)(Function(sq) sq.PackageParts).ToList()
                    Dim partForType = QQHelper.PackagePartForLobType(ppList, packagePartLobType)
                    If partForType IsNot Nothing Then
                        LPDP_Coverage = SummaryHelperClass.Find_First_PackageLevelCoverage(partForType, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
                    End If
                End If

                If LPDP_Coverage IsNot Nothing Then
                    Dim lpdpItem As New SummaryPremiumDataItem With
                        {
                            .Premium = Decimal.Parse(LPDP_Coverage.FullTermPremium)
                        }

                    With lpdpItem
                        Select Case Me.GoverningStateQuote.LobType
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                Select Case packagePartLobType
                                    Case QuickQuoteLobType.CommercialProperty
                                        .Description = "Total Ohio Property Premium Discount"
                                    Case QuickQuoteLobType.CommercialGeneralLiability
                                        .Description = "Total Ohio General Liability Premium Discount"
                                End Select
                            Case QuickQuoteLobType.CommercialGeneralLiability
                                .Description = "Total Ohio General Liability Premium Discount"
                            Case QuickQuoteLobType.CommercialProperty
                                .Description = "Total Ohio Property Premium Discount"
                        End Select
                    End With
                    If lpdpItem.Premium <> 0 Then
                        discounts.Add(lpdpItem)
                    End If
                End If
            End If

            If packagePartLobType = QuickQuoteLobType.None Then
                If discounts.Count > 0 Then

                    divPolicyDiscounts.Visible = True
                    lvPolicyDiscounts.DataSource = discounts
                    lvPolicyDiscounts.DataBind()
                Else
                    lvPolicyDiscounts.DataSource = Nothing
                    divPolicyDiscounts.Visible = False
                End If
            End If
        Catch ex As Exception
            HandleError(NameOf(ctl_CGL_PFSummary), NameOf(Populate), ex)
        End Try
    End Sub

    Public Overrides Sub LoadStaticData()
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Function Save() As Boolean
        'Throw New NotImplementedException()
    End Function

    Public Overrides Sub AddScriptAlways()
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Throw New NotImplementedException()
    End Sub
End Class