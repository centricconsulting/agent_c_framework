Imports IFM.PrimativeExtensions

Public Class ctl_AppPolicyholder
    Inherits VRControlBase

    Public ReadOnly Property PhoneTypeID() As String
        Get
            Return Me.ctlInsured.PhoneTypeID
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Visible Then
            Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.HiddenField1, "0")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMain.ClientID
            Me.ListAccordionDivId = Me.divMain.ClientID
            If Me.Quote IsNot Nothing Then
                ' 7-10-17 MGB I added this CASE statement because the PH control was not displaying on the app side for BOP & CAP
                ' 8-11-17 Added WCP -zshanks
                ' 9-6-17 Added WCP, CPR, CGL, and CPP - MGB
                Select Case Quote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal ', QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        Me.Visible = True
                        Exit Select
                    Case Else
                        If IFM.VR.Common.Helpers.AllLines.RequiredEmailHelper.IsRequiredEmailAvailable(Quote) = True Then
                            If Quote.Policyholder.PrimaryEmail.IsNullEmptyorWhitespace() = False AndAlso Quote.Policyholder2.PrimaryEmail.IsNullEmptyorWhitespace() = False Then
                                Me.Visible = False
                                Me.HiddenField1.Value = "false"
                            End If
                        Else
                            If Quote.Policyholder.PrimaryEmail.IsNullEmptyorWhitespace() = False OrElse Quote.Policyholder.PrimaryPhone.IsNullEmptyorWhitespace() = False Then
                                Me.Visible = False
                                Me.HiddenField1.Value = "false"
                            End If
                        End If
                        Exit Select
                End Select
            End If

        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If Me.Visible Then
            MyBase.ValidateControl(valArgs)
            Me.ValidateChildControls(valArgs)
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            Me.SaveChildControls()
        End If
        Return True
    End Function
End Class