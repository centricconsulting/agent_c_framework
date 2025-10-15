Public Class ctlPolicyHistory
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Me.VRScript.StopEventPropagation(Me.lnkViewSelection.ClientID, False)
        'Me.VRScript.StopEventPropagation(Me.HeaderCheckBox.ClientID, False)

        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#HistorySelectionDiv"").accordion({collapsible: false, heightStyle: ""content""});")
        _script.AddScriptLine("$("".HistorySelectionSection"").accordion({collapsible: false, heightStyle: ""content"", icons: false});")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim hists As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Policy.History) = Nothing
        If Me.Quote IsNot Nothing Then
            Dim qqXml As New QuickQuote.CommonMethods.QuickQuoteXML
            hists = qqXml.GetPolicyHistories(QQHelper.IntegerForString(Me.Quote.PolicyId))
            'If hists IsNot Nothing AndAlso hists.Count > 0 Then
            '    For Each h As Diamond.Common.Objects.Policy.History In hists
            '        'h.PolicyId 'not column
            '        'h.RenewalVersion
            '        'h.TransUser
            '        'h.TransType 'just 1-letter abbreviation
            '        'h.Description'full text for TransType
            '        'h.TransReason
            '        'h.PolicyImageNum
            '        'h.TransRemark
            '        'h.TEffDate
            '        'h.TExpDate
            '        'h.PremiumWritten
            '    Next
            'End If
        End If
        Me.rptPolicyHistory.DataSource = hists
        Me.rptPolicyHistory.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    'Added 02/08/2021 for CAP Endorsements Task 52982 MLW
    Private Sub rptPolicyHistory_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPolicyHistory.ItemDataBound
        Dim tblDocPrint As HtmlTable = e.Item.FindControl("tblDocPrint")
        Dim lblLink As Label = e.Item.FindControl("lblLink")
        Dim lblVerImgNum As Label = e.Item.FindControl("lblVerImgNum")
        Dim lblUser As Label = e.Item.FindControl("lblUser")
        Dim lblTypeReason As Label = e.Item.FindControl("lblTypeReason")
        Dim lblRemark As Label = e.Item.FindControl("lblRemark")
        Dim lblTEff As Label = e.Item.FindControl("lblTEff")
        Dim lblTExp As Label = e.Item.FindControl("lblTExp")
        Dim lblWPrem As Label = e.Item.FindControl("lblWPrem")
        'Below was the original link when the fields were not populated through the itemDataBound (this) sub.
        '<td class="View"><a href="MyVelociRater.aspx?<%# If(UCase(Trim(DataBinder.Eval(Container.DataItem, "TransType"))) = "E" AndAlso UCase(Trim(DataBinder.Eval(Container.DataItem, "Status"))) = "P", "EndorsementPolicyIdAndImageNum", "ReadOnlyPolicyIdAndImageNum") & "=" & DataBinder.Eval(Container.DataItem, "PolicyId")%>|<%# DataBinder.Eval(Container.DataItem, "PolicyImageNum")%>" style="visibility:<%# If(Me.Quote Is Nothing OrElse Me.Quote.PolicyId <> DataBinder.Eval(Container.DataItem, "PolicyId").ToString OrElse Me.Quote.PolicyImageNum <> DataBinder.Eval(Container.DataItem, "PolicyImageNum").ToString, "visible", If(UCase(Trim(DataBinder.Eval(Container.DataItem, "TransType"))) = "E" AndAlso UCase(Trim(DataBinder.Eval(Container.DataItem, "Status"))) = "P", If(Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "hidden", "visible"), If(Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage, "hidden", "visible")))%>"><%# If(UCase(Trim(DataBinder.Eval(Container.DataItem, "TransType"))) = "E" AndAlso UCase(Trim(DataBinder.Eval(Container.DataItem, "Status"))) = "P", "Edit", "View")%></a></td>

        Dim h As Diamond.Common.Objects.Policy.History = e.Item.DataItem
        Dim strPolicyImageVar As String = "ReadOnlyPolicyIdAndImageNum"
        Dim strLinkText As String = "View"
        If UCase(Trim(h.TransType)) = "E" AndAlso UCase(Trim(h.Status)) = "P" Then
            strPolicyImageVar = "EndorsementPolicyIdAndImageNum"
            strLinkText = "Edit"
        End If
        Dim strHref As String = "MyVelociRater.aspx?" & strPolicyImageVar & "=" & h.PolicyId & "|" & h.PolicyImageNum
        lblLink.Text = "<a href='" & strHref & "'>" & strLinkText & "</a>"

        Dim showLink As Boolean = False
        If Me.Quote Is Nothing OrElse Me.Quote.PolicyId <> h.PolicyId.ToString OrElse Me.Quote.PolicyImageNum <> h.PolicyImageNum.ToString Then
            'This will likely be most images - not the image we are currently viewing
            showLink = True
        ElseIf (UCase(Trim(h.TransType)) = "E" AndAlso UCase(Trim(h.Status)) = "P") Then
            If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                'We are in the endorsement edit workflow - should not ever get here
                showLink = False
            Else
                'should not ever get here
                showLink = True
            End If
        Else
            If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                'This is the current image we are viewing. Do not show a link since we are already viewing it.
                showLink = False
            Else
                showLink = True
            End If
        End If

        'Do not show endorsements edit link on endorsements that were started in Diamond for CAP since CAP is locked down from editing most things
        If Me.Quote IsNot Nothing AndAlso showLink = True AndAlso strLinkText = "Edit" Then
            Select Case Me.Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto _
                    , QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP _
                    , QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage 'Updated 11/23/21 for CPP Endorsements Task 65412 MLW
                    Dim errorMessage As String = ""
                    Dim qqXML As New QuickQuote.CommonMethods.QuickQuoteXML
                    Dim qqDiaImgInfo As QuickQuote.CommonObjects.QuickQuoteDiamondImageInfo = Nothing
                    qqDiaImgInfo = qqXML.GetDiamondImageInfoFromDatabase(h.PolicyId, h.PolicyImageNum, imgType:=QuickQuote.CommonObjects.QuickQuoteDiamondImageInfo.DiamondImageType.EndorsementQuote, useActiveFlag:=True, activeFlagToUse:=True, errorMessage:=errorMessage)
                    If qqDiaImgInfo IsNot Nothing Then
                        showLink = True
                    Else
                        showLink = False
                    End If
                Case Else
                    showLink = True
            End Select
        End If
        lblLink.Visible = showLink

        lblVerImgNum.Text = h.RenewalVersion & " - " & h.PolicyImageNum
        lblVerImgNum.ToolTip = "policyId " & h.PolicyId
        lblUser.Text = h.TransUser
        lblUser.ToolTip = h.TransUser
        lblTypeReason.Text = h.Description & " - " & h.TransReason
        lblTypeReason.ToolTip = h.Description & " - " & h.TransReason
        lblRemark.Text = h.TransRemark
        lblRemark.ToolTip = h.TransRemark
        lblTEff.Text = FormatDateTime(h.TEffDate, DateFormat.ShortDate)
        lblTExp.Text = FormatDateTime(h.TExpDate, DateFormat.ShortDate)
        lblWPrem.Text = FormatCurrency(h.PremiumWritten, 2)
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function
End Class