Public Class VRHelpMe
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim helpFileUrl As String = ""

        If Not IsPostBack Then
            If Request.QueryString("p") IsNot Nothing Then ' page sender
                If Request.QueryString("s") IsNot Nothing Then ' section sender
                    ' have page and section info
                    Me.lblTest.Text = String.Format("Page: {0} </br> Section: {1}", Request.QueryString("p"), Request.QueryString("s"))

                    Select Case Request.QueryString("p").ToUpper()
                        Case "VR3CAP", "VR3CAPAPP", "VR3CGL", "VR3CGLAPP", "VR3CPP", "VR3CPPAPP", "VR3CPR", "VR3CPRAPP", "VR3BOP", "VR3BOPAPP", "VR3WCP", "VR3WCPAPP", "VR3Umbrella", "VR3UmbrellaAPP"
                            Select Case Request.QueryString("s").ToUpper()
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.location.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.Crime.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.InlandMarine.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Coverages.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Coverages.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.app.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.na.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                                Case Else
                                    helpFileUrl = ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")
                                    Exit Select
                            End Select
                        Case "VR3AUTO", "VR3AUTOAPP"
                            Select Case Request.QueryString("s").ToUpper()
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_Policyholders")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_Drivers")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_Vehicles")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_Coverages")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.uwQuestions.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_UW")
                                    Exit Select
                                Case Else
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_UW")
                                    Exit Select
                            End Select
                        Case "VR3HOME", "VR3HOMEAPP"
                            Select Case Request.QueryString("s").ToUpper()
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomePolicyholders") 'matt a  1-20-15
                                    Exit Select
                                Case "HEE"
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomeownersEndorsement")
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.property_.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomeProperty") 'matt a  1-20-15
                                    Exit Select
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages.ToString().ToUpper()
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomeCoverages") 'matt a  1-20-15
                                    Exit Select
                                Case "HOSPECLIMITS"
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomeSpecialLimits")
                                    Exit Select
                                Case "MLSPECLIMITS"
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_MobileSpecialLimits")
                                    Exit Select
                                Case "HB" 'added 1/10/18 for HOM Upgrade MLW
                                    helpFileUrl = ConfigurationManager.AppSettings("HOM_WhatsNewHomeBusinessLink")
                                    Exit Select
                                Case Else
                                    helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomePolicyholders") 'matt a  1-20-15
                                    Exit Select
                            End Select

                        Case "VR3DWELLINGFIRE", "VR3DWELLINGFIREAPP"
                            helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_DFRManual") 'matt a  5-31-16
                            Exit Select
                        Case "VR3FARM", "VR3FARMAPP" ' Matt A - 9-10-15
                            helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_FarmGuidelines")
                            Select Case Request.QueryString("s").ToUpper()
                                Case IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHolders.ToString().ToUpper()
                                    'helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomePolicyholders") 'matt a  1-20-15
                                    Exit Select
                                Case Else
                            End Select

                        Case Else
                            Exit Select
                    End Select
                Else
                    ' request quesrystring "s" is nothing
                    ' just have page info
                    Me.lblTest.Text = String.Format("Page: {0} </br> Section: none", Request.QueryString("p"))

                    Select Case Request.QueryString("p").ToUpper()
                        Case "VR3AUTOUWQUESTIONS"
                            helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_UW")
                            Exit Select
                        Case "MYVELOCIRATER"
                            helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_MyVR")
                            Exit Select
                        Case "VR3HOMEAPP", "VR3HOME"
                            helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_HomePolicyholders") 'matt a  1-20-15

                        Case Else
                            helpFileUrl = ConfigurationManager.AppSettings("VRHelpDoc_MyVR")
                            Exit Select
                    End Select
                End If
            Else
                ' no request querystring was sent
                Me.lblTest.Text = "Page: none </br> Section: none"
            End If

            lblTest.Text += "<p> If the help documents existed you would see them now.</p>"

            If String.IsNullOrWhiteSpace(helpFileUrl) = False Then
                ' load file
                Response.Redirect(helpFileUrl)
            End If

        End If
    End Sub

End Class