Imports QuickQuote.CommonObjects 'added 5/19/2017
Imports QuickQuote.CommonMethods 'added 5/19/201

Partial Class DiamondQuoteIRPM_QQ
    Inherits System.Web.UI.Page

    Dim quickQuote As QuickQuoteObject
    Dim QQxml As New QuickQuoteXML
    Dim qqHelper As New QuickQuoteHelperClass
    Dim errMsg As String = ""

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True 'added 8/21/2012
        If Page.IsPostBack = False Then
            'added new js stuff 8/21/2012
            Me.btnCancel.Attributes.Add("onclick", "btnSubmit_Click(this, 'Cancelling...');") 'for disable button and server-side logic
            Me.btnRateAndSave.Attributes.Add("onclick", "btnSubmit_Click(this, 'Rating/Saving...');") 'for disable button and server-side logic

            If Request.QueryString("QuoteId") IsNot Nothing AndAlso Request.QueryString("QuoteId").ToString <> "" AndAlso IsNumeric(Request.QueryString("QuoteId").ToString) = True Then
                Me.lblQuoteId.Text = Request.QueryString("QuoteId").ToString

                'added 8/9/2012
                Me.lblRedirectPage.Text = ""
                If Request.UrlReferrer IsNot Nothing AndAlso Request.UrlReferrer.ToString <> "" AndAlso UCase(Request.UrlReferrer.ToString).Contains("QUOTESUMMARY") Then
                    Me.lblRedirectPage.Text = ConfigurationManager.AppSettings("QuickQuote_QuoteSummary").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                End If

                'QQxml.GetQuoteForSaveType(Me.lblQuoteId.Text, QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, errMsg)
                'changed 8/9/2012
                QQxml.GetQuoteForSaveType(Me.lblQuoteId.Text, QuickQuoteXML.QuickQuoteSaveType.IRPM, quickQuote, errMsg)
                If errMsg <> "" Then
                    ShowError(errMsg & "  You're being redirected to the Saved Quotes page.", True)
                ElseIf quickQuote Is Nothing Then
                    ShowError("There was a problem loading this quote's information.  You're being redirected to the Saved Quotes page.", True)
                Else
                    'load initial values
                    Me.txtIRPM_MgmtCoop.Text = quickQuote.IRPM_ManagementCooperation
                    Me.txtIRPM_MgmtCoopDesc.Text = quickQuote.IRPM_ManagementCooperationDesc
                    Me.txtIRPM_Loc.Text = quickQuote.IRPM_Location
                    Me.txtIRPM_LocDesc.Text = quickQuote.IRPM_LocationDesc
                    Me.txtIRPM_BuildFeat.Text = quickQuote.IRPM_BuildingFeatures
                    Me.txtIRPM_BuildFeatDesc.Text = quickQuote.IRPM_BuildingFeaturesDesc
                    Me.txtIRPM_Premises.Text = quickQuote.IRPM_Premises
                    Me.txtIRPM_PremisesDesc.Text = quickQuote.IRPM_PremisesDesc
                    Me.txtIRPM_Equipment.Text = quickQuote.IRPM_Equipment
                    Me.txtIRPM_EquipmentDesc.Text = quickQuote.IRPM_EquipmentDesc
                    Me.txtIRPM_Emp.Text = quickQuote.IRPM_Employees
                    Me.txtIRPM_EmpDesc.Text = quickQuote.IRPM_EmployeesDesc
                    Me.txtIRPM_Prot.Text = quickQuote.IRPM_Protection
                    Me.txtIRPM_ProtDesc.Text = quickQuote.IRPM_ProtectionDesc
                    Me.txtIRPM_CatHaz.Text = quickQuote.IRPM_CatostrophicHazards
                    Me.txtIRPM_CatHazDesc.Text = quickQuote.IRPM_CatostrophicHazardsDesc
                    Me.txtIRPM_MgmtExp.Text = quickQuote.IRPM_ManagementExperience
                    Me.txtIRPM_MgmtExpDesc.Text = quickQuote.IRPM_ManagementExperienceDesc
                    Me.txtIRPM_MedFac.Text = quickQuote.IRPM_MedicalFacilities
                    Me.txtIRPM_MedFacDesc.Text = quickQuote.IRPM_MedicalFacilitiesDesc
                    Me.txtIRPM_ClsPec.Text = quickQuote.IRPM_ClassificationPeculiarities
                    Me.txtIRPM_ClsPecDesc.Text = quickQuote.IRPM_ClassificationPeculiaritiesDesc
                    'added 8/28/2012 for GL
                    Me.txtIRPM_GL_MgmtCoop.Text = quickQuote.IRPM_GL_ManagementCooperation
                    Me.txtIRPM_GL_MgmtCoopDesc.Text = quickQuote.IRPM_GL_ManagementCooperationDesc
                    Me.txtIRPM_GL_Loc.Text = quickQuote.IRPM_GL_Location
                    Me.txtIRPM_GL_LocDesc.Text = quickQuote.IRPM_GL_LocationDesc
                    Me.txtIRPM_GL_Premises.Text = quickQuote.IRPM_GL_Premises
                    Me.txtIRPM_GL_PremisesDesc.Text = quickQuote.IRPM_GL_PremisesDesc
                    Me.txtIRPM_GL_Equipment.Text = quickQuote.IRPM_GL_Equipment
                    Me.txtIRPM_GL_EquipmentDesc.Text = quickQuote.IRPM_GL_EquipmentDesc
                    Me.txtIRPM_GL_Emp.Text = quickQuote.IRPM_GL_Employees
                    Me.txtIRPM_GL_EmpDesc.Text = quickQuote.IRPM_GL_EmployeesDesc
                    Me.txtIRPM_GL_ClsPec.Text = quickQuote.IRPM_GL_ClassificationPeculiarities
                    Me.txtIRPM_GL_ClsPecDesc.Text = quickQuote.IRPM_GL_ClassificationPeculiaritiesDesc
                    'added 10/3/2012 for CAP
                    Me.txtIRPM_CAP_Mgmt.Text = quickQuote.IRPM_CAP_Management
                    Me.txtIRPM_CAP_MgmtDesc.Text = quickQuote.IRPM_CAP_ManagementDesc
                    Me.txtIRPM_CAP_Emp.Text = quickQuote.IRPM_CAP_Employees
                    Me.txtIRPM_CAP_EmpDesc.Text = quickQuote.IRPM_CAP_EmployeesDesc
                    Me.txtIRPM_CAP_Equipment.Text = quickQuote.IRPM_CAP_Equipment
                    Me.txtIRPM_CAP_EquipmentDesc.Text = quickQuote.IRPM_CAP_EquipmentDesc
                    Me.txtIRPM_CAP_SafetyOrganization.Text = quickQuote.IRPM_CAP_SafetyOrganization
                    Me.txtIRPM_CAP_SafetyOrganizationDesc.Text = quickQuote.IRPM_CAP_SafetyOrganizationDesc
                    'added 10/17/2012 for CPR
                    Me.txtIRPM_CPR_Mgmt.Text = quickQuote.IRPM_CPR_Management
                    Me.txtIRPM_CPR_MgmtDesc.Text = quickQuote.IRPM_CPR_ManagementDesc
                    Me.txtIRPM_CPR_Loc.Text = quickQuote.IRPM_Location
                    Me.txtIRPM_CPR_LocDesc.Text = quickQuote.IRPM_LocationDesc
                    Me.txtIRPM_CPR_BuildFeat.Text = quickQuote.IRPM_BuildingFeatures
                    Me.txtIRPM_CPR_BuildFeatDesc.Text = quickQuote.IRPM_BuildingFeaturesDesc
                    Me.txtIRPM_CPR_PremisesAndEquipment.Text = quickQuote.IRPM_CPR_PremisesAndEquipment
                    Me.txtIRPM_CPR_PremisesAndEquipmentDesc.Text = quickQuote.IRPM_CPR_PremisesAndEquipmentDesc

                    'added 9/6/2012 to default rows to invisible
                    Me.IrpmMgmtCoopRow.Visible = False
                    Me.IrpmLocationRow.Visible = False
                    Me.IrpmBuildingFeaturesRow.Visible = False
                    Me.IrpmPremisesRow.Visible = False
                    Me.IrpmEquipmentRow.Visible = False
                    Me.IrpmEmployeesRow.Visible = False
                    Me.IrpmProtectionRow.Visible = False
                    Me.IrpmCatHazRow.Visible = False
                    Me.IrpmMgmtExpRow.Visible = False
                    Me.IrpmMedFacRow.Visible = False
                    Me.IrpmClsPecRow.Visible = False
                    Me.Irpm_GL_MgmtCoopRow.Visible = False
                    Me.Irpm_GL_LocationRow.Visible = False
                    Me.Irpm_GL_PremisesRow.Visible = False
                    Me.Irpm_GL_EquipmentRow.Visible = False
                    Me.Irpm_GL_EmployeesRow.Visible = False
                    Me.Irpm_GL_ClsPecRow.Visible = False
                    Me.Percent25MessageRow.Visible = False
                    Me.Percent10MessageRow.Visible = False
                    Me.btnRateAndSave.Enabled = True 'added 9/6/2012
                    'updated for CAP 10/3/2012
                    Me.Irpm_CAP_MgmtRow.Visible = False
                    Me.Irpm_CAP_EmployeesRow.Visible = False
                    Me.Irpm_CAP_EquipmentRow.Visible = False
                    Me.Irpm_CAP_SafetyOrganizationRow.Visible = False
                    'updated for CPR 10/17/2012
                    Me.Irpm_CPR_MgmtRow.Visible = False
                    Me.Irpm_CPR_LocationRow.Visible = False
                    Me.Irpm_CPR_BuildingFeaturesRow.Visible = False
                    Me.Irpm_CPR_PremisesAndEquipmentRow.Visible = False
                    'added for CPP 11/20/2012
                    Me.CPP_Percent25MessageRow.Visible = False
                    Me.CPP_CPR_HeaderRow.Visible = False
                    Me.CPP_CGL_HeaderRow.Visible = False

                    If quickQuote.LobType <> Nothing AndAlso quickQuote.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                        Select Case quickQuote.LobType
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                                Me.IrpmMgmtCoopRow.Visible = True
                                Me.IrpmLocationRow.Visible = True
                                Me.IrpmBuildingFeaturesRow.Visible = True
                                Me.IrpmPremisesRow.Visible = True
                                'Me.IrpmEquipmentRow.Visible = False
                                Me.IrpmEmployeesRow.Visible = True
                                Me.IrpmProtectionRow.Visible = True
                                Me.IrpmCatHazRow.Visible = True
                                Me.IrpmMgmtExpRow.Visible = True
                                'Me.IrpmMedFacRow.Visible = False
                                'Me.IrpmClsPecRow.Visible = False
                                'Me.Irpm_GL_MgmtCoopRow.Visible = False
                                'Me.Irpm_GL_LocationRow.Visible = False
                                'Me.Irpm_GL_PremisesRow.Visible = False
                                'Me.Irpm_GL_EquipmentRow.Visible = False
                                'Me.Irpm_GL_EmployeesRow.Visible = False
                                'Me.Irpm_GL_ClsPecRow.Visible = False
                                Me.Percent25MessageRow.Visible = True
                                'Me.Percent10MessageRow.Visible = False
                            Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                                Me.IrpmMgmtCoopRow.Visible = True
                                'Me.IrpmLocationRow.Visible = False
                                'Me.IrpmBuildingFeaturesRow.Visible = False
                                Me.IrpmPremisesRow.Visible = True
                                Me.IrpmEquipmentRow.Visible = True
                                Me.IrpmEmployeesRow.Visible = True
                                'Me.IrpmProtectionRow.Visible = False
                                'Me.IrpmCatHazRow.Visible = False
                                'Me.IrpmMgmtExpRow.Visible = False
                                Me.IrpmMedFacRow.Visible = True
                                Me.IrpmClsPecRow.Visible = True
                                'Me.Irpm_GL_MgmtCoopRow.Visible = False
                                'Me.Irpm_GL_LocationRow.Visible = False
                                'Me.Irpm_GL_PremisesRow.Visible = False
                                'Me.Irpm_GL_EquipmentRow.Visible = False
                                'Me.Irpm_GL_EmployeesRow.Visible = False
                                'Me.Irpm_GL_ClsPecRow.Visible = False
                                'Me.Percent25MessageRow.Visible = False
                                Me.Percent10MessageRow.Visible = True
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                                'Me.IrpmMgmtCoopRow.Visible = False
                                'Me.IrpmLocationRow.Visible = False
                                'Me.IrpmBuildingFeaturesRow.Visible = False
                                'Me.IrpmPremisesRow.Visible = False
                                'Me.IrpmEquipmentRow.Visible = False
                                'Me.IrpmEmployeesRow.Visible = False
                                'Me.IrpmProtectionRow.Visible = False
                                'Me.IrpmCatHazRow.Visible = False
                                'Me.IrpmMgmtExpRow.Visible = False
                                'Me.IrpmMedFacRow.Visible = False
                                'Me.IrpmClsPecRow.Visible = False
                                Me.Irpm_GL_MgmtCoopRow.Visible = True
                                Me.Irpm_GL_LocationRow.Visible = True
                                Me.Irpm_GL_PremisesRow.Visible = True
                                Me.Irpm_GL_EquipmentRow.Visible = True
                                Me.Irpm_GL_EmployeesRow.Visible = True
                                Me.Irpm_GL_ClsPecRow.Visible = True
                               Me.Percent25MessageRow.Visible = True
                                'Me.Percent10MessageRow.Visible = False
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto 'added 10/3/2012
                                Me.Irpm_CAP_MgmtRow.Visible = True
                                Me.Irpm_CAP_EmployeesRow.Visible = True
                                Me.Irpm_CAP_EquipmentRow.Visible = True
                                Me.Irpm_CAP_SafetyOrganizationRow.Visible = True
                                Me.Percent25MessageRow.Visible = True
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                                Me.Irpm_CPR_MgmtRow.Visible = True
                                Me.Irpm_CPR_LocationRow.Visible = True
                                Me.Irpm_CPR_BuildingFeaturesRow.Visible = True
                                Me.Irpm_CPR_PremisesAndEquipmentRow.Visible = True
                                Me.IrpmEmployeesRow.Visible = True
                                Me.IrpmProtectionRow.Visible = True
                                Me.Percent25MessageRow.Visible = True
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage 'added 11/20/2012
                                Me.CPP_Percent25MessageRow.Visible = True
                                Me.CPP_CPR_HeaderRow.Visible = True
                                Me.CPP_CGL_HeaderRow.Visible = True
                                'GL
                                Me.Irpm_GL_MgmtCoopRow.Visible = True
                                Me.Irpm_GL_LocationRow.Visible = True
                                Me.Irpm_GL_PremisesRow.Visible = True
                                Me.Irpm_GL_EquipmentRow.Visible = True
                                Me.Irpm_GL_EmployeesRow.Visible = True
                                Me.Irpm_GL_ClsPecRow.Visible = True
                                'Prop
                                Me.Irpm_CPR_MgmtRow.Visible = True
                                Me.Irpm_CPR_LocationRow.Visible = True
                                Me.Irpm_CPR_BuildingFeaturesRow.Visible = True
                                Me.Irpm_CPR_PremisesAndEquipmentRow.Visible = True
                                Me.IrpmEmployeesRow.Visible = True
                                Me.IrpmProtectionRow.Visible = True
                            Case Else
                                Me.btnRateAndSave.Enabled = False 'added 9/6/2012
                        End Select
                    Else
                        Me.btnRateAndSave.Enabled = False 'added 9/6/2012
                    End If

                    ViewState.Add("QuickQuoteObject", quickQuote)
                    'added 10/8/2012 for CAP (IRPM validation)
                    If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso (quickQuote.Vehicles Is Nothing OrElse quickQuote.Vehicles.Count < 2) Then
                        Me.btnRateAndSave.Enabled = False
                        ShowError("You cannot edit CAP IRPM factors unless there are at least 2 vehicles.", True)
                    End If
                End If
            Else
                ShowError("A valid parameter for QuoteId was not sent thru the querystring.  You're being redirected to the Saved Quotes page.", True)
            End If
        End If
    End Sub

    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = ConfigurationManager.AppSettings("QuickQuote_SavedQuotes").ToString
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Protected Sub btnRateAndSave_Click(sender As Object, e As System.EventArgs) Handles btnRateAndSave.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        errMsg = ""

        'added 8/23/2012 but not being used to validate yet (UW is going to handle by creating Rating warnings)
        Dim CreditDebitTotal As String = "0"
        Dim CreditDebitFactor As String = "1"
        'added 11/20/2012 for CPP
        Dim CPP_CPR_CreditDebitTotal As String = "0"
        Dim CPP_CPR_CreditDebitFactor As String = "1"
        Dim CPP_CGL_CreditDebitTotal As String = "0"
        Dim CPP_CGL_CreditDebitFactor As String = "1"

        If Me.IrpmMgmtCoopRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_MgmtCoop.Text) = False OrElse CDbl(Me.txtIRPM_MgmtCoop.Text) < 0.9 OrElse CDbl(Me.txtIRPM_MgmtCoop.Text) > 1.1 Then
                errMsg = "Management/Cooperation factor must be a numeric value between 0.9 and 1.1"
            ElseIf CDbl(Me.txtIRPM_MgmtCoop.Text) <> 1 AndAlso Me.txtIRPM_MgmtCoopDesc.Text = "" Then
                errMsg = "Please provide a remark for Management/Cooperation"
            ElseIf CDbl(Me.txtIRPM_MgmtCoop.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_MgmtCoop.Text))
            End If
        End If
        If Me.Irpm_CPR_MgmtRow.Visible = True Then 'added 10/17/2012 for CPR
            If IsNumeric(Me.txtIRPM_CPR_Mgmt.Text) = False OrElse CDbl(Me.txtIRPM_CPR_Mgmt.Text) < 0.82 OrElse CDbl(Me.txtIRPM_CPR_Mgmt.Text) > 1.18 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = "Property "
                End If
                errMsg = addText & "Management factor must be a numeric value between 0.82 and 1.18"
            ElseIf CDbl(Me.txtIRPM_CPR_Mgmt.Text) <> 1 AndAlso Me.txtIRPM_CPR_MgmtDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = " (Property)"
                End If
                errMsg = "Please provide a remark for Management" & addText
            ElseIf CDbl(Me.txtIRPM_CPR_Mgmt.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_Mgmt.Text))
                CPP_CPR_CreditDebitTotal = qqHelper.getSum(CPP_CPR_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_Mgmt.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.IrpmLocationRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_Loc.Text) = False OrElse CDbl(Me.txtIRPM_Loc.Text) < 0.9 OrElse CDbl(Me.txtIRPM_Loc.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Location factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_Loc.Text) <> 1 AndAlso Me.txtIRPM_LocDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Location", "; ")
            ElseIf CDbl(Me.txtIRPM_Loc.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_Loc.Text))
            End If
        End If
        If Me.Irpm_CPR_LocationRow.Visible = True Then 'added 10/17/2012 for CPR
            If IsNumeric(Me.txtIRPM_CPR_Loc.Text) = False OrElse CDbl(Me.txtIRPM_CPR_Loc.Text) < 0.82 OrElse CDbl(Me.txtIRPM_CPR_Loc.Text) > 1.18 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = "Property "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Location factor must be a numeric value between 0.82 and 1.18", "; ")
            ElseIf CDbl(Me.txtIRPM_CPR_Loc.Text) <> 1 AndAlso Me.txtIRPM_CPR_LocDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = " (Property)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Location" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_CPR_Loc.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_Loc.Text))
                CPP_CPR_CreditDebitTotal = qqHelper.getSum(CPP_CPR_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_Loc.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.IrpmBuildingFeaturesRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_BuildFeat.Text) = False OrElse CDbl(Me.txtIRPM_BuildFeat.Text) < 0.9 OrElse CDbl(Me.txtIRPM_BuildFeat.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Building Features factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_BuildFeat.Text) <> 1 AndAlso Me.txtIRPM_BuildFeatDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Building Features", "; ")
            ElseIf CDbl(Me.txtIRPM_BuildFeat.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_BuildFeat.Text))
            End If
        End If
        If Me.Irpm_CPR_BuildingFeaturesRow.Visible = True Then 'added 10/17/2012 for CPR
            If IsNumeric(Me.txtIRPM_CPR_BuildFeat.Text) = False OrElse CDbl(Me.txtIRPM_CPR_BuildFeat.Text) < 0.8 OrElse CDbl(Me.txtIRPM_CPR_BuildFeat.Text) > 1.2 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = "Property "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Building Features factor must be a numeric value between 0.8 and 1.2", "; ")
            ElseIf CDbl(Me.txtIRPM_CPR_BuildFeat.Text) <> 1 AndAlso Me.txtIRPM_CPR_BuildFeatDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = " (Property)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Building Features" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_CPR_BuildFeat.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_BuildFeat.Text))
                CPP_CPR_CreditDebitTotal = qqHelper.getSum(CPP_CPR_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_BuildFeat.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.IrpmPremisesRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_Premises.Text) = False OrElse CDbl(Me.txtIRPM_Premises.Text) < 0.9 OrElse CDbl(Me.txtIRPM_Premises.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Premises factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_Premises.Text) <> 1 AndAlso Me.txtIRPM_PremisesDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Premises", "; ")
            ElseIf CDbl(Me.txtIRPM_Premises.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_Premises.Text))
            End If
        End If
        If Me.IrpmEquipmentRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_Equipment.Text) = False OrElse CDbl(Me.txtIRPM_Equipment.Text) < 0.9 OrElse CDbl(Me.txtIRPM_Equipment.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Equipment factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_Equipment.Text) <> 1 AndAlso Me.txtIRPM_EquipmentDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Equipment", "; ")
            ElseIf CDbl(Me.txtIRPM_Equipment.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_Equipment.Text))
            End If
        End If
        If Me.Irpm_CPR_PremisesAndEquipmentRow.Visible = True Then 'added 10/17/2012 for CPR
            If IsNumeric(Me.txtIRPM_CPR_PremisesAndEquipment.Text) = False OrElse CDbl(Me.txtIRPM_CPR_PremisesAndEquipment.Text) < 0.8 OrElse CDbl(Me.txtIRPM_CPR_PremisesAndEquipment.Text) > 1.2 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = "Property "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Premises and Equipment factor must be a numeric value between 0.8 and 1.2", "; ")
            ElseIf CDbl(Me.txtIRPM_CPR_PremisesAndEquipment.Text) <> 1 AndAlso Me.txtIRPM_CPR_PremisesAndEquipmentDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CPR_HeaderRow.Visible = True Then
                    addText = " (Property)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Premises and Equipment" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_CPR_PremisesAndEquipment.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_PremisesAndEquipment.Text))
                CPP_CPR_CreditDebitTotal = qqHelper.getSum(CPP_CPR_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CPR_PremisesAndEquipment.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.IrpmEmployeesRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_Emp.Text) = False OrElse CDbl(Me.txtIRPM_Emp.Text) < 0.9 OrElse CDbl(Me.txtIRPM_Emp.Text) > 1.1 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "Property "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Employees factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_Emp.Text) <> 1 AndAlso Me.txtIRPM_EmpDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (Property)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Employees" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_Emp.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_Emp.Text))
                CPP_CPR_CreditDebitTotal = qqHelper.getSum(CPP_CPR_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_Emp.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.IrpmProtectionRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_Prot.Text) = False OrElse CDbl(Me.txtIRPM_Prot.Text) < 0.9 OrElse CDbl(Me.txtIRPM_Prot.Text) > 1.1 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "Property "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Protection factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_Prot.Text) <> 1 AndAlso Me.txtIRPM_ProtDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (Property)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Protection" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_Prot.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_Prot.Text))
                CPP_CPR_CreditDebitTotal = qqHelper.getSum(CPP_CPR_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_Prot.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.IrpmCatHazRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_CatHaz.Text) = False OrElse CDbl(Me.txtIRPM_CatHaz.Text) < 0.9 OrElse CDbl(Me.txtIRPM_CatHaz.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Catastrophic Hazards factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_CatHaz.Text) <> 1 AndAlso Me.txtIRPM_CatHazDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Catastrophic Hazards", "; ")
            ElseIf CDbl(Me.txtIRPM_CatHaz.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CatHaz.Text))
            End If
        End If
        If Me.IrpmMgmtExpRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_MgmtExp.Text) = False OrElse CDbl(Me.txtIRPM_MgmtExp.Text) < 0.9 OrElse CDbl(Me.txtIRPM_MgmtExp.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Management Experience factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_MgmtExp.Text) <> 1 AndAlso Me.txtIRPM_MgmtExpDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Management Experience", "; ")
            ElseIf CDbl(Me.txtIRPM_MgmtExp.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_MgmtExp.Text))
            End If
        End If
        If Me.IrpmMedFacRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_MedFac.Text) = False OrElse CDbl(Me.txtIRPM_MedFac.Text) < 0.9 OrElse CDbl(Me.txtIRPM_MedFac.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Medical Facilities factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_MedFac.Text) <> 1 AndAlso Me.txtIRPM_MedFacDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Medical Facilities", "; ")
            ElseIf CDbl(Me.txtIRPM_MedFac.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_MedFac.Text))
            End If
        End If
        If Me.IrpmClsPecRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_ClsPec.Text) = False OrElse CDbl(Me.txtIRPM_ClsPec.Text) < 0.9 OrElse CDbl(Me.txtIRPM_ClsPec.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Classification Peculiarities factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_ClsPec.Text) <> 1 AndAlso Me.txtIRPM_ClsPecDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Classification Peculiarities", "; ")
            ElseIf CDbl(Me.txtIRPM_ClsPec.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_ClsPec.Text))
            End If
        End If
        'added 8/28/2012 for GL
        If Me.Irpm_GL_MgmtCoopRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_GL_MgmtCoop.Text) = False OrElse CDbl(Me.txtIRPM_GL_MgmtCoop.Text) < 0.85 OrElse CDbl(Me.txtIRPM_GL_MgmtCoop.Text) > 1.15 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "General Liability "
                End If
                'errMsg = addText & "Management/Cooperation factor must be a numeric value between 0.85 and 1.15"
                'updated 11/20/2012 to append error since CPR could already have something
                errMsg = qqHelper.appendText(errMsg, addText & "Management/Cooperation factor must be a numeric value between 0.85 and 1.15", "; ")
            ElseIf CDbl(Me.txtIRPM_GL_MgmtCoop.Text) <> 1 AndAlso Me.txtIRPM_GL_MgmtCoopDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (General Liability)"
                End If
                'errMsg = "Please provide a remark for Management/Cooperation" & addText
                'updated 11/20/2012 to append error since CPR could already have something
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Management/Cooperation" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_GL_MgmtCoop.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_MgmtCoop.Text))
                CPP_CGL_CreditDebitTotal = qqHelper.getSum(CPP_CGL_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_MgmtCoop.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.Irpm_GL_LocationRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_GL_Loc.Text) = False OrElse CDbl(Me.txtIRPM_GL_Loc.Text) < 0.9 OrElse CDbl(Me.txtIRPM_GL_Loc.Text) > 1.1 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "General Liability "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Location factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Loc.Text) <> 1 AndAlso Me.txtIRPM_GL_LocDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (General Liability)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Location" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Loc.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Loc.Text))
                CPP_CGL_CreditDebitTotal = qqHelper.getSum(CPP_CGL_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Loc.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.Irpm_GL_PremisesRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_GL_Premises.Text) = False OrElse CDbl(Me.txtIRPM_GL_Premises.Text) < 0.85 OrElse CDbl(Me.txtIRPM_GL_Premises.Text) > 1.15 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "General Liability "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Premises factor must be a numeric value between 0.85 and 1.15", "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Premises.Text) <> 1 AndAlso Me.txtIRPM_GL_PremisesDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (General Liability)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Premises" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Premises.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Premises.Text))
                CPP_CGL_CreditDebitTotal = qqHelper.getSum(CPP_CGL_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Premises.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.Irpm_GL_EquipmentRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_GL_Equipment.Text) = False OrElse CDbl(Me.txtIRPM_GL_Equipment.Text) < 0.85 OrElse CDbl(Me.txtIRPM_GL_Equipment.Text) > 1.15 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "General Liability "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Equipment factor must be a numeric value between 0.85 and 1.15", "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Equipment.Text) <> 1 AndAlso Me.txtIRPM_GL_EquipmentDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (General Liability)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Equipment" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Equipment.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Equipment.Text))
                CPP_CGL_CreditDebitTotal = qqHelper.getSum(CPP_CGL_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Equipment.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.Irpm_GL_EmployeesRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_GL_Emp.Text) = False OrElse CDbl(Me.txtIRPM_GL_Emp.Text) < 0.85 OrElse CDbl(Me.txtIRPM_GL_Emp.Text) > 1.15 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "General Liability "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Employees factor must be a numeric value between 0.85 and 1.15", "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Emp.Text) <> 1 AndAlso Me.txtIRPM_GL_EmpDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (General Liability)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Employees" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_GL_Emp.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Emp.Text))
                CPP_CGL_CreditDebitTotal = qqHelper.getSum(CPP_CGL_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_Emp.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        If Me.Irpm_GL_ClsPecRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_GL_ClsPec.Text) = False OrElse CDbl(Me.txtIRPM_GL_ClsPec.Text) < 0.9 OrElse CDbl(Me.txtIRPM_GL_ClsPec.Text) > 1.1 Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = "General Liability "
                End If
                errMsg = qqHelper.appendText(errMsg, addText & "Classification Peculiarities factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_GL_ClsPec.Text) <> 1 AndAlso Me.txtIRPM_GL_ClsPecDesc.Text = "" Then
                Dim addText As String = "" 'added 11/20/2012 for CPP
                If Me.CPP_CGL_HeaderRow.Visible = True Then
                    addText = " (General Liability)"
                End If
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Classification Peculiarities" & addText, "; ")
            ElseIf CDbl(Me.txtIRPM_GL_ClsPec.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_ClsPec.Text))
                CPP_CGL_CreditDebitTotal = qqHelper.getSum(CPP_CGL_CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_GL_ClsPec.Text)) 'added 11/20/2012 for CPP
            End If
        End If
        'added 10/3/2012 for CAP
        If Me.Irpm_CAP_MgmtRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_CAP_Mgmt.Text) = False OrElse CDbl(Me.txtIRPM_CAP_Mgmt.Text) < 0.9 OrElse CDbl(Me.txtIRPM_CAP_Mgmt.Text) > 1.1 Then
                errMsg = "Management factor must be a numeric value between 0.9 and 1.1"
            ElseIf CDbl(Me.txtIRPM_CAP_Mgmt.Text) <> 1 AndAlso Me.txtIRPM_CAP_MgmtDesc.Text = "" Then
                errMsg = "Please provide a remark for Management"
            ElseIf CDbl(Me.txtIRPM_CAP_Mgmt.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CAP_Mgmt.Text))
            End If
        End If
        If Me.Irpm_CAP_EmployeesRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_CAP_Emp.Text) = False OrElse CDbl(Me.txtIRPM_CAP_Emp.Text) < 0.85 OrElse CDbl(Me.txtIRPM_CAP_Emp.Text) > 1.15 Then
                errMsg = qqHelper.appendText(errMsg, "Employees factor must be a numeric value between 0.85 and 1.15", "; ")
            ElseIf CDbl(Me.txtIRPM_CAP_Emp.Text) <> 1 AndAlso Me.txtIRPM_CAP_EmpDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Employees", "; ")
            ElseIf CDbl(Me.txtIRPM_CAP_Emp.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CAP_Emp.Text))
            End If
        End If
        If Me.Irpm_CAP_EquipmentRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_CAP_Equipment.Text) = False OrElse CDbl(Me.txtIRPM_CAP_Equipment.Text) < 0.85 OrElse CDbl(Me.txtIRPM_CAP_Equipment.Text) > 1.15 Then
                errMsg = qqHelper.appendText(errMsg, "Equipment factor must be a numeric value between 0.85 and 1.15", "; ")
            ElseIf CDbl(Me.txtIRPM_CAP_Equipment.Text) <> 1 AndAlso Me.txtIRPM_CAP_EquipmentDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Equipment", "; ")
            ElseIf CDbl(Me.txtIRPM_CAP_Equipment.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CAP_Equipment.Text))
            End If
        End If
        If Me.Irpm_CAP_SafetyOrganizationRow.Visible = True Then
            If IsNumeric(Me.txtIRPM_CAP_SafetyOrganization.Text) = False OrElse CDbl(Me.txtIRPM_CAP_SafetyOrganization.Text) < 0.9 OrElse CDbl(Me.txtIRPM_CAP_SafetyOrganization.Text) > 1.1 Then
                errMsg = qqHelper.appendText(errMsg, "Safety Organization factor must be a numeric value between 0.9 and 1.1", "; ")
            ElseIf CDbl(Me.txtIRPM_CAP_SafetyOrganization.Text) <> 1 AndAlso Me.txtIRPM_CAP_SafetyOrganizationDesc.Text = "" Then
                errMsg = qqHelper.appendText(errMsg, "Please provide a remark for Safety Organization", "; ")
            ElseIf CDbl(Me.txtIRPM_CAP_SafetyOrganization.Text) <> 1 Then
                CreditDebitTotal = qqHelper.getSum(CreditDebitTotal, qqHelper.getDiff("1", Me.txtIRPM_CAP_SafetyOrganization.Text))
            End If
        End If

        CreditDebitFactor = qqHelper.getDiff(CreditDebitFactor, CreditDebitTotal)
        'added 11/20/2012 for CPP
        CPP_CPR_CreditDebitFactor = qqHelper.getDiff(CPP_CPR_CreditDebitFactor, CPP_CPR_CreditDebitTotal)
        CPP_CGL_CreditDebitFactor = qqHelper.getDiff(CPP_CGL_CreditDebitFactor, CPP_CGL_CreditDebitTotal)

        'added 9/5/2012
        If errMsg = "" AndAlso ConfigurationManager.AppSettings("QuickQuote_Validate_IRPM_Factor") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Validate_IRPM_Factor").ToString) = "YES" Then
            'updated 11/20/2012 for CPP
            If Me.CPP_CPR_HeaderRow.Visible = True OrElse Me.CPP_CGL_HeaderRow.Visible = True Then
                'CPP
                If qqHelper.IsHomeOfficeStaffUser = True Then
                    'staff
                    If (Me.Percent25MessageRow.Visible = True OrElse Me.Percent10MessageRow.Visible = True OrElse Me.CPP_Percent25MessageRow.Visible = True) AndAlso CDbl(CPP_CPR_CreditDebitFactor) < 0.5 Then
                        errMsg = "Your overall Property factor cannot be less than 0.500; you're currently at " & CPP_CPR_CreditDebitFactor
                    End If
                    If (Me.Percent25MessageRow.Visible = True OrElse Me.Percent10MessageRow.Visible = True OrElse Me.CPP_Percent25MessageRow.Visible = True) AndAlso CDbl(CPP_CGL_CreditDebitFactor) < 0.5 Then
                        errMsg = qqHelper.appendText(errMsg, "Your overall General Liability factor cannot be less than 0.500; you're currently at " & CPP_CGL_CreditDebitFactor, "; ")
                    End If
                Else
                    'not staff
                    If (Me.Percent25MessageRow.Visible = True OrElse Me.CPP_Percent25MessageRow.Visible = True) AndAlso CDbl(CPP_CPR_CreditDebitFactor) < 0.75 Then
                        errMsg = "Your overall Property factor cannot be less than 0.750; you're currently at " & CPP_CPR_CreditDebitFactor
                        'ElseIf Me.Percent10MessageRow.Visible = True AndAlso CDbl(CreditDebitFactor) < 0.9 Then
                        '    errMsg = "Your overall factor cannot be less than 0.900; you're currently at " & CreditDebitFactor
                    End If
                    If (Me.Percent25MessageRow.Visible = True OrElse Me.CPP_Percent25MessageRow.Visible = True) AndAlso CDbl(CPP_CGL_CreditDebitFactor) < 0.75 Then
                        errMsg = qqHelper.appendText(errMsg, "Your overall General Liability factor cannot be less than 0.750; you're currently at " & CPP_CGL_CreditDebitFactor, "; ")
                        'ElseIf Me.Percent10MessageRow.Visible = True AndAlso CDbl(CreditDebitFactor) < 0.9 Then
                        '    errMsg = "Your overall factor cannot be less than 0.900; you're currently at " & CreditDebitFactor
                    End If
                End If
            Else
                'not CPP (use normal logic)
                If qqHelper.IsHomeOfficeStaffUser = True Then
                    'staff
                    If (Me.Percent25MessageRow.Visible = True OrElse Me.Percent10MessageRow.Visible = True) AndAlso CDbl(CreditDebitFactor) < 0.5 Then
                        errMsg = "Your overall factor cannot be less than 0.500; you're currently at " & CreditDebitFactor
                    End If
                Else
                    'not staff
                    If Me.Percent25MessageRow.Visible = True AndAlso CDbl(CreditDebitFactor) < 0.75 Then
                        errMsg = "Your overall factor cannot be less than 0.750; you're currently at " & CreditDebitFactor
                    ElseIf Me.Percent10MessageRow.Visible = True AndAlso CDbl(CreditDebitFactor) < 0.9 Then
                        errMsg = "Your overall factor cannot be less than 0.900; you're currently at " & CreditDebitFactor
                    End If
                End If
            End If
        End If

        'for testing 8/23/2012
        'If errMsg <> "" Then
        '    ShowError("Error:  " & errMsg)
        'Else
        '    ShowError("CreditDebitTotal:  " & CreditDebitTotal & "; factor:  " & CreditDebitFactor)
        'End If

        If errMsg = "" Then
            If ViewState("QuickQuoteObject") IsNot Nothing Then
                quickQuote = CType(ViewState("QuickQuoteObject"), QuickQuoteObject)

                quickQuote.IRPM_ManagementCooperation = Me.txtIRPM_MgmtCoop.Text
                quickQuote.IRPM_ManagementCooperationDesc = Me.txtIRPM_MgmtCoopDesc.Text
                quickQuote.IRPM_Location = Me.txtIRPM_Loc.Text
                quickQuote.IRPM_LocationDesc = Me.txtIRPM_LocDesc.Text
                quickQuote.IRPM_BuildingFeatures = Me.txtIRPM_BuildFeat.Text
                quickQuote.IRPM_BuildingFeaturesDesc = Me.txtIRPM_BuildFeatDesc.Text
                quickQuote.IRPM_Premises = Me.txtIRPM_Premises.Text
                quickQuote.IRPM_PremisesDesc = Me.txtIRPM_PremisesDesc.Text
                quickQuote.IRPM_Equipment = Me.txtIRPM_Equipment.Text
                quickQuote.IRPM_EquipmentDesc = Me.txtIRPM_EquipmentDesc.Text
                quickQuote.IRPM_Employees = Me.txtIRPM_Emp.Text
                quickQuote.IRPM_EmployeesDesc = Me.txtIRPM_EmpDesc.Text
                quickQuote.IRPM_Protection = Me.txtIRPM_Prot.Text
                quickQuote.IRPM_ProtectionDesc = Me.txtIRPM_ProtDesc.Text
                quickQuote.IRPM_CatostrophicHazards = Me.txtIRPM_CatHaz.Text
                quickQuote.IRPM_CatostrophicHazardsDesc = Me.txtIRPM_CatHazDesc.Text
                quickQuote.IRPM_ManagementExperience = Me.txtIRPM_MgmtExp.Text
                quickQuote.IRPM_ManagementExperienceDesc = Me.txtIRPM_MgmtExpDesc.Text
                quickQuote.IRPM_MedicalFacilities = Me.txtIRPM_MedFac.Text
                quickQuote.IRPM_MedicalFacilitiesDesc = Me.txtIRPM_MedFacDesc.Text
                quickQuote.IRPM_ClassificationPeculiarities = Me.txtIRPM_ClsPec.Text
                quickQuote.IRPM_ClassificationPeculiaritiesDesc = Me.txtIRPM_ClsPecDesc.Text
                'added 8/28/2012 for GL
                quickQuote.IRPM_GL_ManagementCooperation = Me.txtIRPM_GL_MgmtCoop.Text
                quickQuote.IRPM_GL_ManagementCooperationDesc = Me.txtIRPM_GL_MgmtCoopDesc.Text
                quickQuote.IRPM_GL_Location = Me.txtIRPM_GL_Loc.Text
                quickQuote.IRPM_GL_LocationDesc = Me.txtIRPM_GL_LocDesc.Text
                quickQuote.IRPM_GL_Premises = Me.txtIRPM_GL_Premises.Text
                quickQuote.IRPM_GL_PremisesDesc = Me.txtIRPM_GL_PremisesDesc.Text
                quickQuote.IRPM_GL_Equipment = Me.txtIRPM_GL_Equipment.Text
                quickQuote.IRPM_GL_EquipmentDesc = Me.txtIRPM_GL_EquipmentDesc.Text
                quickQuote.IRPM_GL_Employees = Me.txtIRPM_GL_Emp.Text
                quickQuote.IRPM_GL_EmployeesDesc = Me.txtIRPM_GL_EmpDesc.Text
                quickQuote.IRPM_GL_ClassificationPeculiarities = Me.txtIRPM_GL_ClsPec.Text
                quickQuote.IRPM_GL_ClassificationPeculiaritiesDesc = Me.txtIRPM_GL_ClsPecDesc.Text
                'added 10/3/2012 for CAP
                quickQuote.IRPM_CAP_Management = Me.txtIRPM_CAP_Mgmt.Text
                quickQuote.IRPM_CAP_ManagementDesc = Me.txtIRPM_CAP_MgmtDesc.Text
                quickQuote.IRPM_CAP_Employees = Me.txtIRPM_CAP_Emp.Text
                quickQuote.IRPM_CAP_EmployeesDesc = Me.txtIRPM_CAP_EmpDesc.Text
                quickQuote.IRPM_CAP_Equipment = Me.txtIRPM_CAP_Equipment.Text
                quickQuote.IRPM_CAP_EquipmentDesc = Me.txtIRPM_CAP_EquipmentDesc.Text
                quickQuote.IRPM_CAP_SafetyOrganization = Me.txtIRPM_CAP_SafetyOrganization.Text
                quickQuote.IRPM_CAP_SafetyOrganizationDesc = Me.txtIRPM_CAP_SafetyOrganizationDesc.Text
                'added 10/17/2012 for CPR
                quickQuote.IRPM_CPR_Management = Me.txtIRPM_CPR_Mgmt.Text
                quickQuote.IRPM_CPR_ManagementDesc = Me.txtIRPM_CPR_MgmtDesc.Text
                'If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then
                'updated 11/20/2012 for CPP
                If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    'to make sure these props are only overwritten for CPR
                    quickQuote.IRPM_Location = Me.txtIRPM_CPR_Loc.Text
                    quickQuote.IRPM_LocationDesc = Me.txtIRPM_CPR_LocDesc.Text
                    quickQuote.IRPM_BuildingFeatures = Me.txtIRPM_CPR_BuildFeat.Text
                    quickQuote.IRPM_BuildingFeaturesDesc = Me.txtIRPM_CPR_BuildFeatDesc.Text
                End If
                quickQuote.IRPM_CPR_PremisesAndEquipment = Me.txtIRPM_CPR_PremisesAndEquipment.Text
                quickQuote.IRPM_CPR_PremisesAndEquipmentDesc = Me.txtIRPM_CPR_PremisesAndEquipmentDesc.Text

                ViewState("QuickQuoteObject") = quickQuote

                'QQxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, Me.lblQuoteId.Text, errMsg)
                'changed 8/9/2012
                QQxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.IRPM, quickQuote, Me.lblQuoteId.Text, errMsg)
                If errMsg <> "" Then
                    ShowError(errMsg, True, Me.lblRedirectPage.Text)
                Else
                    'send onto Quote Summary
                    Response.Redirect(ConfigurationManager.AppSettings("QuickQuote_QuoteSummary").ToString & "?QuoteId=" & Me.lblQuoteId.Text)
                End If
            Else
                ShowError("There was a problem re-loading this quote's information.  You're being redirected to the Saved Quotes page.", True)
            End If
        Else
            errMsg = "The following validation errors were encountered:  " & errMsg
            ShowError(errMsg)
        End If
        
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As System.EventArgs) Handles btnCancel.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblRedirectPage.Text <> "" Then
            Response.Redirect(Me.lblRedirectPage.Text)
        Else
            Response.Redirect(ConfigurationManager.AppSettings("QuickQuote_SavedQuotes").ToString)
        End If
    End Sub
End Class
