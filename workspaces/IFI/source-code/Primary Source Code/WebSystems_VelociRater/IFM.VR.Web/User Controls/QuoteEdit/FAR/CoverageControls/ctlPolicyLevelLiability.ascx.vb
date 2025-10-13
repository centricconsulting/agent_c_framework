Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
'Imports IFM.VR.Validation.ObjectValidation.AllLines

Public Class ctlPolicyLevelLiability
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_farmLocationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_farmLocationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_farmLocationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() AndAlso Me.Quote.Locations.Count > MyLocationIndex Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MainAccordionDivId = dvFarmPolicyLiabilityCoverage.ClientID
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenFarmLiabilityCoverage, "0")

        'save event
        VRScript.StopEventPropagation(lnkBtnSave.ClientID)

        'remove event
        VRScript.StopEventPropagation(lnkBtnClear.ClientID)

        Dim scriptCoverageType As String = "ToggleExtraData(""" + ddlLiabCovType.ClientID + """, """ + dvLiability.ClientID + """, """ + dvMedPay.ClientID + """, """ + dvEmpLiab.ClientID + """, """ + dvBusinessPursuits.ClientID +
            """, """ + dvFamilyMedPay.ClientID + """, """ + dvCustomFarming.ClientID + """, """ + dvFarmPollution.ClientID + """, """ + dvEPLI.ClientID + """, """ + dvAdditionalIns.ClientID + """, """ + dvIdentityFraud.ClientID +
            """, """ + dvPersLiab.ClientID + """);"
        ddlLiabCovType.Attributes.Add("onchange", scriptCoverageType)
    End Sub

    Public Overrides Sub LoadStaticData()
        If MyFarmLocation IsNot Nothing Then
            ddlLiabCovType.Items.Clear()

            MyFarmLocation.ProgramTypeId = "7"
            If MyFarmLocation.ProgramTypeId = "7" Then
                Dim newListItem As ListItem

                If Quote.Policyholder.Name.TypeId = "1" Then
                    newListItem = New ListItem("Personal Liab", "1")
                    ddlLiabCovType.Items.Add(newListItem)
                Else
                    newListItem = New ListItem("Comm Liab", "2")
                    ddlLiabCovType.Items.Add(newListItem)
                End If

                newListItem = New ListItem("None", "6")
                ddlLiabCovType.Items.Add(newListItem)
                dvLiabilityDropDown.Attributes.Add("style", "display:block;")
                dvLiabilityType.Attributes.Add("style", "display:none;")
                lblLiabCovType.Text = ""
            Else
                dvLiabilityDropDown.Attributes.Add("style", "display:none;")
                dvLiabilityType.Attributes.Add("style", "display:block;")
                lblLiabCovType.Text = "Not a Select-O-Matic"
            End If
        End If
    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function
End Class