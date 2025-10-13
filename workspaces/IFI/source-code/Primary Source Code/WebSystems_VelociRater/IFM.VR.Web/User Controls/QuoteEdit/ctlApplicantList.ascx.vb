Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Public Class ctlApplicantList
    Inherits VRControlBase

    Public Property ActiveApplicantPane As String
        Get
            Return Me.hiddenActiveAccordIndex.Value
        End Get
        Set(value As String)
            Me.hiddenActiveAccordIndex.Value = value
        End Set
    End Property

    Public Property ShowActionButtons As Boolean
        Get
            Return Me.divActionButtons.Visible
        End Get
        Set(value As Boolean)
            Me.divActionButtons.Visible = value
        End Set
    End Property

    Public ReadOnly Property CanAddApplicant As Boolean
        Get
            'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
            'Return If(Me.Quote IsNot Nothing AndAlso Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name.TypeId = "2" And (Me.Quote IsNot Nothing AndAlso Me.Quote.Applicants Is Nothing Or (Me.Quote IsNot Nothing AndAlso Me.Quote.Applicants IsNot Nothing AndAlso Me.Quote.Applicants.Count < 2)), True, False)
            Return If(Me.Quote IsNot Nothing AndAlso Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name.TypeId = "2" And (Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants Is Nothing Or (Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.Count < 2)), True, False)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divApplicantListAccord.ClientID
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddVariableLine("function ShowApplicant(index){SetActiveAccordionIndex('" + ListAccordionDivId + "',index);}") ' here to respond to treeview applicant clicks
        Me.VRScript.CreateAccordion(ListAccordionDivId, Me.hiddenActiveAccordIndex, "0")
        Me.VRScript.AddVariableLine(String.Format("var applicantsAccordId = '{0}';", ListAccordionDivId))
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal Then
                If Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing Then
                    If Me.Quote.Policyholder.Name.TypeId <> "2" Then
                        Me.Visible = False
                        Return ' no need to go any further
                    Else
                        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                            Me.Visible = True 'for switching between Personal Liability and Commercial Liability on the Policy Level Coverages page
                        End If
                    End If
                End If
            End If
        End If

        'If Not IsCommercialPolicy Then
        '    Me.Visible = False
        '    Return
        'End If

        Me.Repeater1.DataSource = Nothing
        If Me.Quote IsNot Nothing Then
            'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
            'Me.Repeater1.DataSource = Me.Quote.Applicants
            Me.Repeater1.DataSource = Me.GoverningStateQuote.Applicants
            Me.Repeater1.DataBind()
            Me.bnAddApplicant.Visible = CanAddApplicant
            Me.FindChildVrControls() ' finds the just added controls do to the binding
            Dim index As Int32 = 0
            For Each child In Me.ChildVrControls
                If TypeOf child Is ctlApplicant Then
                    Dim c As ctlApplicant = child
                    c.ApplicantIndex = index
                    c.Populate()
                    index += 1
                End If
            Next

            'Me.btnSubmit.Visible = Me.Quote.Drivers IsNot Nothing AndAlso Me.Quote.Drivers.Any()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Applicant List"
        Me.ValidateChildControls(valArgs)

        Dim valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common.ApplicantListValidator.ValidateApplicantList(Me.Quote, Me.DefaultValidationType)
        For Each v In valItems
            Me.ValidationHelper.AddError(v.Message)
        Next

    End Sub

    Public Overrides Function Save() As Boolean
        For Each c In Me.ChildVrControls
            c.Save()
        Next
        Return True
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnSaveAndGotoNextPage.Click
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(Me.DefaultValidationType)))
        If sender Is Me.btnSaveAndGotoNextPage Then
            ' goto next page
        End If
    End Sub

    Protected Sub bnAddApplicant_Click(sender As Object, e As EventArgs) Handles bnAddApplicant.Click
        AddApplicant()
    End Sub

    Public Sub AddApplicant()
        If Me.Quote IsNot Nothing AndAlso CanAddApplicant Then
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, True, New VRValidationArgs(Me.DefaultValidationType)))
            'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
            'Me.Quote.Applicants.AddNew()
            Me.GoverningStateQuote.Applicants.AddNew()
            Me.Save_FireSaveEvent(False)
            Me.Populate()
            'Updated 9/18/18 for multi state MLW - Quote to GoverningStateQuote
            'hiddenActiveAccordIndex.Value = (Me.Quote.Applicants.Count - 1).ToString()
            hiddenActiveAccordIndex.Value = (Me.GoverningStateQuote.Applicants.Count - 1).ToString()
        End If
    End Sub
End Class