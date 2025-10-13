Public Class ctlPrintHistory
    Inherits VRControlBase

    Dim printForms As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
    Const SchedContrEquipName = "Scheduled Contractors Equipment"

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkViewSelection.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.HeaderCheckBox.ClientID, False)

        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#DocPrintDiv"").accordion({collapsible: false, heightStyle: ""content""});")
        _script.AddScriptLine("$("".DocPrintSection"").accordion({collapsible: false, heightStyle: ""content"", icons: false});")
        _script.AddVariableLine("var PrintHistoryPolicyImgNum = " + Me.Quote.PolicyImageNum + ";")


        'End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'Me.ctl_PF_ScheduledConstEquipment.Visible = False
        'Me.ctl_PF_ScheduledConstEquipment.Populate()

        PopulatePrintFormList()

        If IsQuoteReadOnly() AndAlso Quote.TransactionTypeId <> "3" AndAlso Quote.PolicyStatusCode <> QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.Pending Then
            chkShowAllImgs.Checked = SetShowAllIfOneImage()
        End If


        Me.rptDocPrint.DataSource = printForms
        Me.rptDocPrint.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'Populate()
        End If

    End Sub

    Public Overrides Function Save() As Boolean

        Return True

    End Function

    Private Sub Print_Click(sender As Object, e As EventArgs) Handles lnkViewSelection.Click
        If Me.Visible Then
            Dim selectedPrintForms As New Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
            Dim bytes As Byte()
            Dim errMsg As String = String.Empty

            If (printForms Is Nothing) Then
                PopulatePrintFormList()
            End If

            For Each ri As RepeaterItem In rptDocPrint.Items
                Dim chkSelected As CheckBox = ri.FindControl("CheckBox")
                Dim description As Label = ri.FindControl("FormDesc")
                Dim itemIndex = ri.ItemIndex

                ' Add any Non-Diamond Add-on items "Description" text to the list below to be excluded from the Diamond PDF
                '   creation process.
                'Dim AddOnNotInDiamondList As New List(Of String) From {SchedContrEquipName}

                If (chkSelected.Checked) Then
                    selectedPrintForms.Add(printForms(itemIndex))
                End If

                ''Only Checked Items and Items that are in diamond (no add-on list items)
                'If (chkSelected.Checked And Not AddOnNotInDiamondList.Contains(description.Text)) Then
                '    selectedPrintForms.Add(printForms(itemIndex))
                'End If

                'If (chkSelected.Checked And AddOnNotInDiamondList.Contains(description.Text)) Then
                '    'Me.VRScript.AddScriptLine("alert('Routing is complete.'); window.open = 'PF_ContractorsEquipment.aspx';")
                'End If

            Next

            If (selectedPrintForms.Count > 0) Then
                bytes = Common.PrintHistories.PrintHistories.GetPrintForm(selectedPrintForms, errMsg)

                If bytes IsNot Nothing Then
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("Documents{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
                    ' Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("Documents{0}.pdf", "BOP1005181")) 'WCP019163
                    Response.BinaryWrite(bytes)
                Else
                    'Me.VRScript.AddScriptLine("alert('Could not find Documents.');")
                    Me.VRScript.AddScriptLine("alert('" + errMsg + "');")
                End If
            End If


        End If
    End Sub

    Private Sub btnPolicyHistory_Click(sender As Object, e As EventArgs) Handles btnPolicyHistory.Click
        ' Send to Policy History
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub

    Private Function GetFormList()
        ' testing
        If Me.Quote IsNot Nothing Then
            If IsNumeric(Me.Quote.PolicyId) Then
                Dim policyID = Me.Quote.PolicyId   '1142040 '1143305
                Dim loginName As String = ""
                Dim loginPassword As String = ""

                If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername").ToString <> "" Then
                    loginName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginName = ConfigurationManager.AppSettings("QuickQuoteTestUsername").ToString
                End If
                If System.Web.HttpContext.Current.Session("DiamondPassword") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> "" Then
                    loginPassword = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginPassword = ConfigurationManager.AppSettings("QuickQuoteTestPassword").ToString
                End If

                printForms = Common.PrintHistories.PrintHistories.GetDiamondPrintHistory(policyID,
                                                                                            loginName,
                                                                                            loginPassword,
                                                                                            Common.PrintHistories.PrintHistories.PrintType.All,
                                                                                            "",
                                                                                            Common.PrintHistories.PrintHistories.PrintFormDescriptionEvaluationType.UseDefaultForPrintType) 'CAH Changed from 'OnlyUniqueFormDescriptions' to get complete list.
                Return printForms
            End If
        End If
        Return Nothing
    End Function

    Private Sub PopulatePrintFormList()
        printForms = GetFormList()

        Select Case Me.Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                Dim ContractorEquipPrint As New Diamond.Common.Objects.Printing.PrintForm
                If Me.Quote.ContractorsEquipmentScheduledItems IsNot Nothing AndAlso Quote.ContractorsEquipmentScheduledItems.Count > 0 Then
                    ContractorEquipPrint.Description = SchedContrEquipName
                    ContractorEquipPrint.FormNumber = "N/A"
                    printForms.Add(ContractorEquipPrint)
                End If
        End Select
    End Sub

    Private Function SetShowAllIfOneImage() As Boolean
        Dim ValueToTest = printForms?.First().PolicyImageNum
        If printForms IsNot Nothing Then
            For Each form As Diamond.Common.Objects.Printing.PrintForm In printForms
                If ValueToTest <> form?.PolicyImageNum Then
                    Return False
                End If
            Next
        End If
        Return True
    End Function



End Class