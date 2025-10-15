Imports QuickQuote.CommonMethods 'added 5/19/2017

Partial Class DiamondPrintForm_QQ
    Inherits System.Web.UI.Page

    Dim qqHelper As New QuickQuoteHelperClass
    Dim policyId As String = ""
    Dim formBytes As Byte() = Nothing

    Enum PrintType
        All = 1
        JustWorksheet = 2
    End Enum

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            If Request.QueryString("PolicyId") IsNot Nothing AndAlso Request.QueryString("PolicyId").ToString <> "" Then
                policyId = Request.QueryString("PolicyId").ToString

                If Session("DiamondPrintFormBytes_" & policyId) IsNot Nothing Then
                    formBytes = CType(Session("DiamondPrintFormBytes_" & policyId), Byte())
                Else
                    If ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All").ToString) = "WORKSHEET" Then
                        GetDiamondPrintHistory(PrintType.JustWorksheet)
                    Else
                        GetDiamondPrintHistory()
                    End If
                End If

                If formBytes IsNot Nothing Then
                    Response.ContentType = "application/pdf"
                    Response.BinaryWrite(formBytes)
                    Response.Flush()

                    If Session("DiamondPrintFormBytes_" & policyId) IsNot Nothing Then
                        Session("DiamondPrintFormBytes_" & policyId) = Nothing
                    End If
                Else
                    Response.Write("No print form found. Please try again.")
                End If
            Else
                Response.Write("No print form parameters were found.  Please try again.")
            End If

        End If
    End Sub
    Private Sub GetDiamondPrintHistory(Optional ByVal pType As PrintType = PrintType.All)
        If policyId <> "" AndAlso IsNumeric(policyId) = True Then
            Using dia As New DiamondWebClass.DiamondPrinting
                Dim forms As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing '5/19/2017 - added initialization logic
                'forms = dia.getPrintFormsForPolicyId(dia.loginDiamond("PrintServices", "PrintServices"), 245276) 'policy id for QBOP010535 test on prodpatch
                'forms = dia.getPrintFormsForPolicyId(dia.loginDiamond("PrintServices", "PrintServices"), CInt(policyId))
                'updated 3/12/2013 to not send new credentials to Diamond
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
                If loginName <> "" AndAlso loginPassword <> "" Then
                    '5/7/2013 - doesn't need Try/Catch to prevent unhandled exception (since method in DiamondPrinting class already has it)
                    forms = dia.getPrintFormsForPolicyId(dia.loginDiamond(loginName, loginPassword), CInt(policyId))
                End If

                If forms IsNot Nothing Then
                    If pType = PrintType.JustWorksheet Then
                        Dim pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm) 'added 11/27/2012 for CPP (multiple worksheets; CPR and CGL)
                        For Each pf As Diamond.Common.Objects.Printing.PrintForm In forms
                            If UCase(pf.Description).Contains("WORKSHEET") Then
                                'formBytes = GetPrintForm(pf)
                                'Exit For
                                'updated 11/27/2012 for CPP
                                If pfs Is Nothing Then
                                    pfs = New Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
                                End If
                                pfs.Add(pf)
                            End If
                        Next
                        'added 11/27/2012 for CPP
                        If pfs IsNot Nothing Then
                            formBytes = GetPrintForm(pfs)
                        End If
                    Else
                        'all
                        Dim pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm) = Nothing '5/19/2017 - added initialization logic
                        For Each pf As Diamond.Common.Objects.Printing.PrintForm In forms
                            If pfs Is Nothing Then
                                pfs = New Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
                            End If
                            pfs.Add(pf)
                        Next
                        If pfs IsNot Nothing Then
                            formBytes = GetPrintForm(pfs)
                        End If
                    End If
                    'For Each pf As Diamond.Common.Objects.Printing.PrintForm In forms
                    '    ''If UCase(pf.Description).Contains("DECLA") = True Then
                    '    'Me.lblLogger.Text &= "<br><br>"
                    '    'Me.lblLogger.Text &= "TypeId=" & pf.FormCategoryTypeId
                    '    'Me.lblLogger.Text &= "<br>Type=" & pf.FormCategoryTypeDescription
                    '    'Me.lblLogger.Text &= "<br>Desc=" & pf.Description
                    '    'Me.lblLogger.Text &= "<br>PrintXMLid=" & pf.PrintXmlId
                    '    'Me.lblLogger.Text &= "<br>Form #=" & pf.FormNumber
                    '    'Me.lblLogger.Text &= "<br>Added=" & pf.AddedDate.ToString
                    '    ''End If

                    '    'to just get Worksheet
                    '    'If UCase(pf.Description).Contains("WORKSHEET") Then
                    '    '    Session("DiamondPrintFormBytes") = GetPrintForm(pf)
                    '    '    Exit For
                    '    'End If


                    'Next
                End If
            End Using
            If formBytes IsNot Nothing Then
                'Response.Redirect("DiamondPrintForm.aspx")
                'Me.PrintHistoryRow.Visible = True

                'Me.lblByteString.Text = qqHelper.StringFromBytes(CType(Session("DiamondPrintFormBytes"), Byte()))
                'Session("DiamondPrintFormBytes") = Nothing

                'using the querystring overloads max url length; may try POSTing to print page

            End If
        End If
    End Sub
    Private Function GetPrintForm(ByVal pf As Diamond.Common.Objects.Printing.PrintForm) As Byte()
        Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
        Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

        With reprintRequest.RequestData
            '.PolicyId = policyID
            '.PolicyImageNum = imageNum

            .PolicyId = pf.PolicyId
            .PolicyImageNum = pf.PolicyImageNum

            .PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
            .PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
        End With

        Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
            Try
                '5/7/2013 - enclosed in Try/Catch to prevent unhandled exception (so page would still work)
                reprintResponse = reprintProxy.ReprintJob(reprintRequest)
            Catch ex As Exception

            End Try
        End Using

        If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
            Return reprintResponse.ResponseData.Data
        Else
            If reprintResponse.DiamondValidation.HasErrors Then
                For Each diaVal As Diamond.Common.Objects.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                    If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                        'errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                    End If
                Next
            End If
            Return Nothing
        End If
    End Function
    Private Function GetPrintForm(ByVal pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)) As Byte()
        If pfs IsNot Nothing AndAlso pfs.Count > 0 Then
            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            With reprintRequest.RequestData
                .PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                For Each pf As Diamond.Common.Objects.Printing.PrintForm In pfs
                    .PolicyId = pf.PolicyId
                    .PolicyImageNum = pf.PolicyImageNum

                    .PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                Next
            End With

            Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                Try
                    '5/7/2013 - enclosed in Try/Catch to prevent unhandled exception (so page would still work)
                    reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                Catch ex As Exception

                End Try
            End Using

            If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                Return reprintResponse.ResponseData.Data
            Else
                If reprintResponse.DiamondValidation.HasErrors Then
                    For Each diaVal As Diamond.Common.Objects.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                        If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                            'errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                        End If
                    Next
                End If
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function
End Class
