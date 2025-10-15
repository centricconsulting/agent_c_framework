Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.Umbrella
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption

Public Class ctl_FUPPUP_CoverageLimits
    Inherits VRControlBase

#Region "Declarations"





#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(FUPPUP_divPolicyCoverageLimits.ClientID, hdnAccordGenInfo, "0")

        Me.VRScript.StopEventPropagation(Me.lnkSaveGeneralInfo.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearGeneralInfo.ClientID)

    End Sub

    Public Overrides Sub LoadStaticData()

        QQHelper.LoadStaticDataOptionsDropDown(ddlUmbrellaLimit, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteUmbrella, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.UmbrellaCoverageLimitId, SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(ddlUmbrellaUmUimLimit, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteUmbrella, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.UmbrellaUmUimLimitId, SortBy.None, Me.Quote.LobType)

    End Sub



    Public Overrides Sub Populate()
        LoadStaticData()

        If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then

            'If Me.SubQuoteFirst IsNot Nothing Then
            If String.IsNullOrWhiteSpace(SubQuoteFirst.UmbrellaCoverageLimitId) = False Then
                ddlUmbrellaLimit.SelectedValue = SubQuoteFirst.UmbrellaCoverageLimitId
            Else
                ddlUmbrellaLimit.SelectedValue = 56
            End If

            If String.IsNullOrWhiteSpace(SubQuoteFirst.UmbrellaUmUimLimitId) = False Then
                ddlUmbrellaUmUimLimit.SelectedValue = SubQuoteFirst.UmbrellaUmUimLimitId
            Else
                ddlUmbrellaUmUimLimit.SelectedValue = 0
            End If

            If String.IsNullOrWhiteSpace(SubQuoteFirst.UmbrellaSelfInsuredRetentionLimitId) = False Then
                If SubQuoteFirst.UmbrellaSelfInsuredRetentionLimitId = "286" Then
                    txtSelfInsuredRetention.Text = "$1,000"
                Else
                    txtSelfInsuredRetention.Text = "$250"
                End If
            Else
                If Me.IsCommercialPolicy() Then
                    txtSelfInsuredRetention.Text = "$1000"
                Else
                    txtSelfInsuredRetention.Text = "$250"
                End If
            End If
            'End If

        End If

        Exit Sub
    End Sub



    Public Overrides Function Save() As Boolean

        If Me.SubQuoteFirst IsNot Nothing Then

            SubQuoteFirst.UmbrellaCoverageLimitId = ddlUmbrellaLimit.SelectedValue
            SubQuoteFirst.UmbrellaUmUimLimitId = ddlUmbrellaUmUimLimit.SelectedValue

            If String.IsNullOrWhiteSpace(SubQuoteFirst.UmbrellaSelfInsuredRetentionLimitId) Then
                If Me.IsCommercialPolicy() Then
                    SubQuoteFirst.UmbrellaSelfInsuredRetentionLimitId = "286"
                Else
                    SubQuoteFirst.UmbrellaSelfInsuredRetentionLimitId = "165"
                End If
            End If

        End If

        'Loop Through all subquotes and add these values

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Policy Level Coverages"

        Dim valItems = PolicyLevelValidations.ValidatePolicyLevel(Me.Quote, valArgs.ValidationType)
        If valItems.Any() Then
            For Each v In valItems
                Select Case v.FieldId
                    Case PolicyLevelValidations.ddlUmbrellaUmUimLimit
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlUmbrellaUmUimLimit, v, 0, 0)
                End Select
            Next
        End If

    End Sub


#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkSaveGeneralInfo.Click ', lnkSaveEndorsements.Click
        Me.Save_FireSaveEvent()
        Populate()
    End Sub

    Protected Sub lnkClearGeneralInfo_Click(sender As Object, e As EventArgs) Handles lnkClearGeneralInfo.Click
        ClearControl()
        Populate()
    End Sub

    Public Overrides Sub ClearControl()
        SubQuoteFirst.UmbrellaCoverageLimitId = "-1"
        SubQuoteFirst.UmbrellaUmUimLimitId = "-1"
    End Sub

#End Region

End Class