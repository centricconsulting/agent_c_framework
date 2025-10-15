Public Class ctlCommercial_DocPrint
    Inherits VRControlBase

    Dim printForms As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
    Const SchedContrEquipName = "Scheduled Contractors Equipment"

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'If Me.Quote IsNot Nothing Then
        'Dim LOB As String = Me.Quote.LobType
        'Me.VRScript.AddVariableLine("var ctlCommercial_DocPrint_LOB='" + IFM.VR.Common.Helpers.QuickQuoteObjectHelper.GetAbbreviatedLOBPrefix(LOB) + "';")
        'Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID, False)

        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#DocPrintDiv"").accordion({collapsible: false, heightStyle: ""content""});")
        _script.AddScriptLine("$("".DocPrintSection"").accordion({collapsible: false, heightStyle: ""content""});")

        'End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'Me.ctl_PF_ScheduledConstEquipment.Visible = False
        'Me.ctl_PF_ScheduledConstEquipment.Populate()

        PopulatePrintFormList()

        Me.rptDocPrint.DataSource = printForms
        Me.rptDocPrint.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean

        Return True

    End Function

    Private Sub Print_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        If Me.Visible Then
            Dim selectedPrintForms As New Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
            Dim bytes As Byte()

            If (printForms Is Nothing) Then
                PopulatePrintFormList()
            End If

            For Each ri As RepeaterItem In rptDocPrint.Items
                Dim chkSelected As CheckBox = CType(ri.FindControl("CheckBox"), CheckBox)
                Dim description As Label = CType(ri.FindControl("Description"), Label)
                Dim itemIndex = ri.ItemIndex

                ' Add any Non-Diamond Add-on items "Description" text to the list below to be excluded from the Diamond PDF
                '   creation process.
                Dim AddOnNotInDiamondList As New List(Of String) From {SchedContrEquipName}

                'Only Checked Items and Items that are in diamond (no add-on list items)
                If (chkSelected.Checked And Not AddOnNotInDiamondList.Contains(description.Text)) Then
                    selectedPrintForms.Add(printForms(itemIndex))
                End If

                If (chkSelected.Checked And AddOnNotInDiamondList.Contains(description.Text)) Then
                    'Me.VRScript.AddScriptLine("alert('Routing is complete.'); window.open = 'PF_ContractorsEquipment.aspx';")
                End If

            Next

            If (selectedPrintForms.Count > 0) Then
                bytes = Common.PrintHistories.PrintHistories.GetPrintForm(selectedPrintForms)

                If bytes IsNot Nothing Then
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("Documents{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
                    ' Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("Documents{0}.pdf", "BOP1005181")) 'WCP019163
                    Response.BinaryWrite(bytes)
                Else
                    Me.VRScript.AddScriptLine("alert('Could not find Documents.');")
                End If
            End If


        End If
    End Sub

    Private Sub Cancel_Click(sender As Object, e As EventArgs) Handles btnReturn.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary, "0")
    End Sub

    Private Function GetFormList() As List(Of Diamond.Common.Objects.Printing.PrintForm)
        ' testing
        If Me.Quote IsNot Nothing Then
            If IsNumeric(Me.Quote.PolicyId) Then
                Dim policyID = CInt(Me.Quote.PolicyId)   '1142040 '1143305
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

                If IsQuoteEndorsement() Then
                    ' Endorsements - get all print forms including decs
                    printForms = Common.PrintHistories.PrintHistories.GetDiamondPrintHistory(policyID,
                                                                                            loginName,
                                                                                            loginPassword,
                                                                                            Common.PrintHistories.PrintHistories.PrintType.All,
                                                                                            "",
                                                                                            Common.PrintHistories.PrintHistories.PrintFormDescriptionEvaluationType.OnlyUniqueFormDescriptions)
                Else
                    ' New Business - get all print forms excluding decs
                    printForms = Common.PrintHistories.PrintHistories.GetDiamondPrintHistory(policyID,
                                                                                            loginName,
                                                                                            loginPassword,
                                                                                            Common.PrintHistories.PrintHistories.PrintType.AllExceptDeclarations,
                                                                                            "",
                                                                                            Common.PrintHistories.PrintHistories.PrintFormDescriptionEvaluationType.OnlyUniqueFormDescriptions)
                End If
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

End Class