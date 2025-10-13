Imports IFM.PrimativeExtensions

Public Class ctl_AttachmentUpload
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divVrUpload", Me.HiddenField1, "0")
        Me.VRScript.AddScriptLine("AttachmentUpload.QueryForattachedFiles();", True)
        Me.VRScript.CreateJSStringArrayFromList(IFM.VR.Common.Helpers.FileUploadHelper.VrFileUploadAcceptableExtensionList, "fileUploadAcceptableTypes", True)

        Dim workFlowManager = Me.FindFirstVRParentOfType(Of VRMasterControlBase)
        If workFlowManager.IsNotNull Then
            Me.btnReturntoWorkflow.Text = "Return to {0}".FormatIFM(workFlowManager.PriorWorkflowName)
        Else
            Me.btnReturntoWorkflow.Text = "Return to Prior Section"
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Me.Visible = Request.IsLocal
        'Me.lblHeader.Text = "Support Document Upload (Prototype - only shows on test)"


    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Protected Sub btnReturntoWorkflow_Click(sender As Object, e As EventArgs) Handles btnReturntoWorkflow.Click
        Dim workFlowManager = Me.FindFirstVRParentOfType(Of VRMasterControlBase)
        If workFlowManager.IsNotNull Then
            workFlowManager.ReturnToPriorWorkflow()
        End If
    End Sub
End Class