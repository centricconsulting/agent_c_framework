Imports IFM.PrimativeExtensions
Public Class billingFutureInfo
    Inherits VRControlBase

    Public Property FuturePremium As String
    Public Property FutureMiscCharges As Decimal
    Public Property FutureList() As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future)
        Get
            If ViewState.Item("FutureDt") Is Nothing Then
                Return New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future)
            Else
                Return ViewState.Item("FutureDt")
            End If
        End Get
        Set(ByVal value As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future))
            If ViewState.Item("FutureDt") Is Nothing Then
                ViewState.Add("FutureDt", value)
            Else
                ViewState.Item("FutureDt") = value
            End If
        End Set
    End Property

    Public Property FutureCombinedList() As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future)
        Get
            If ViewState.Item("FutureCombinedDt") Is Nothing Then
                Return New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future)
            Else
                Return ViewState.Item("FutureCombinedDt")
            End If
        End Get
        Set(ByVal value As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future))
            If ViewState.Item("FutureCombinedDt") Is Nothing Then
                ViewState.Add("FutureCombinedDt", value)
            Else
                ViewState.Item("FutureCombinedDt") = value
            End If
        End Set
    End Property

    Public ReadOnly Property IsBillingUpdate() As Boolean
        Get
            Dim result As Boolean = False
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("isBillingUpdate").ToString) = False Then
                result = CBool(Request.QueryString("isBillingUpdate").ToString)
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("isBillingUpdate").ToString) = False Then
                result = CBool(Page.RouteData.Values("isBillingUpdate").ToString)
            Else
                If Me.Quote IsNot Nothing AndAlso Me.Quote.IsBillingEndorsement = True Then
                    result = True
                End If
            End If
            Return result
        End Get

    End Property

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        txtFuturePremium.Text = FuturePremium.TryToFormatAsCurreny

        'Calculated Misc Charges
        CombineMiscChargesOfSameType()

        txtFutureMiscCharges.Text = FutureMiscCharges.ToString("c")

        If FutureList.Length < 1 Then
            DataGrid_FutureInfo.Visible = False
            DataGrid_FutureCombined.Visible = False
        End If

        If ViewState.Item("FutureCombinedDt") Is Nothing Then
            CreateFutureCombinedList()
        End If

        Me.DataGrid_FutureInfo.DataSource = FutureList
        Me.DataGrid_FutureInfo.DataBind()

        Me.DataGrid_FutureCombined.DataSource = FutureCombinedList
        Me.DataGrid_FutureCombined.DataBind()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If IsBillingUpdate() Then
            Me.VRScript.CreateAccordion(BillFutureInfo.ClientID, hdnAccordGenInfo, "1")
        Else
            Me.VRScript.CreateAccordion(BillFutureInfo.ClientID, hdnAccordGenInfo, "0")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
    End Function

    Public Sub CreateFutureCombinedList()
        ' CAH 07/23/2019 - 'IS' code
        Dim tempKey As Integer = 9000 'Create a key because a dictionary does not allow duplicate keys (billinginstallmentnum = 0)
        Dim billingInstallmentKey As String = String.Empty
        Dim combinedDictionary As New Dictionary(Of String, Diamond.Common.Objects.Billing.Future) '<billinginstallmentnum + k + renewalver, future object>
        Dim FutureListTemp As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future) = FutureList().Clone
        FutureListTemp.Sort(Function(x As Diamond.Common.Objects.Billing.Future) x.BillingChargeTypeCategoryId) 'Sort by chargetypecategory so all premium will be first

        For Each f As Diamond.Common.Objects.Billing.Future In FutureListTemp
            If f.BillingInstallmentNum > 0 Then
                billingInstallmentKey = f.BillingInstallmentNum & "k" & f.RenewalVer & "k" & f.PolicyId
                Select Case f.BillingChargeTypeCategoryId
                    Case Diamond.Common.Enums.Billing.BillingChargeTypeCategory.Premium 'if it is premium and has an installmentnum, must be an installment
                        combinedDictionary.Add(billingInstallmentKey, f) 'so add it to the dictionary with the installmentnum as the key
                    Case Diamond.Common.Enums.Billing.BillingChargeTypeCategory.Schg, Diamond.Common.Enums.Billing.BillingChargeTypeCategory.Mchg
                        If combinedDictionary.ContainsKey(billingInstallmentKey) AndAlso combinedDictionary(billingInstallmentKey) IsNot Nothing Then
                            combinedDictionary(billingInstallmentKey).Amount += f.Amount 'for service/misc charges, add the charge amount to the installment
                        ElseIf f.BillingChargeTypeCategoryId = Diamond.Common.Enums.Billing.BillingChargeTypeCategory.Mchg Then
                            combinedDictionary.Add(CStr(tempKey), f) 'could be a MChg installment (A MChg with its own installment num)
                            tempKey += 1
                        End If
                    Case Else
                        combinedDictionary.Add(CStr(tempKey), f) 'else, add it to the dictionary so it isn't lost
                        tempKey += 1
                End Select
            Else
                combinedDictionary.Add(CStr(tempKey), f) 'if it is not an installment, add it to the dictionary with the temporary key
                tempKey += 1
            End If
        Next

        If combinedDictionary.Values.Count > 0 Then
            Dim FutureCombinedListTemp As New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Future)
            FutureCombinedListTemp.Clear()
            FutureCombinedListTemp.Add(combinedDictionary.Values.ToList) 'convert the dictionary.values to an inscollection
            FutureCombinedListTemp.Sort(Function(x As Diamond.Common.Objects.Billing.Future) x.TranDate) 'sort it by trandate to get the collection back in the original format
            FutureCombinedList() = FutureCombinedListTemp

        End If
    End Sub

    Private Sub CombineMiscChargesOfSameType()
        'Dim newMiscCharges As New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.MiscCharge)
        'Dim totalFeeAmount As Decimal

        'If Me.FutureMiscCharges.Length > 0 Then
        '    For Each miscCharge As Diamond.Common.Objects.Policy.MiscCharge In Me.FutureMiscCharges
        '        Dim newMiscCharge As Diamond.Common.Objects.Policy.MiscCharge = newMiscCharges.ItemForValue(Function(x) x.BillingChargesCreditsTypeId = miscCharge.BillingChargesCreditsTypeId)

        '        If newMiscCharge IsNot Nothing Then
        '            ' Add to the total
        '            newMiscCharge.Amount += miscCharge.Amount
        '        ElseIf miscCharge.Amount <> 0 Then
        '            ' Charge not in new collection and has an amount add to collection
        '            newMiscCharges.Add(miscCharge.MakeCopy())
        '        End If

        '        totalFeeAmount += miscCharge.Amount
        '    Next
        'End If

        'txtFutureMiscCharges.Text = totalFeeAmount.ToString("c")

        'Return totalFeeAmount
        'Return New Tuple(Of Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.MiscCharge), Decimal)(newMiscCharges, totalFeeAmount)
    End Sub

    Public Sub DataGrid_FutureInfo_Command(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid_FutureInfo.ItemCommand

        Select Case e.CommandName
            Case "Page"
                If ViewState.Item("FutureDt") IsNot Nothing Then
                    Me.DataGrid_FutureInfo.CurrentPageIndex = e.CommandArgument - 1
                    DataGrid_FutureInfo.DataSource = FutureList
                    DataGrid_FutureInfo.DataBind()
                End If

        End Select

    End Sub

    Public Sub DataGrid_FutureCombined_Command(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid_FutureCombined.ItemCommand

        Select Case e.CommandName
            Case "Page"
                If ViewState.Item("FutureCombinedDt") IsNot Nothing Then
                    Me.DataGrid_FutureInfo.CurrentPageIndex = e.CommandArgument - 1
                    DataGrid_FutureInfo.DataSource = FutureCombinedList
                    DataGrid_FutureInfo.DataBind()
                End If

        End Select

    End Sub
End Class