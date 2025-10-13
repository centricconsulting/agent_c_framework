Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Workflow

Public Class MyVR
    Inherits BasePage

    Private Const staffUserNameIndicationText As String = " (IFM_STAFF)"
    Public Shared ReadOnly connQQ As String = System.Configuration.ConfigurationManager.AppSettings("connQQ")

    Public Function GetSelectedAgentUserName() As String
        If Me.ddAgent.SelectedValue.Contains(staffUserNameIndicationText) Then
            ' remove
            Return Me.ddAgent.SelectedValue.Replace(staffUserNameIndicationText, "")
        End If
        Return Me.ddAgent.SelectedValue
    End Function

    Public Function GetAgencyId() As Int32
        Return DirectCast(Me.Page.Master, VelociRater).AgencyID
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim masterPage As VelociRater = DirectCast(Me.Page.Master, VR.Web.VelociRater)
        AddHandler masterPage.AgencyIdChanged, AddressOf AgencyChanged

        If Not IsPostBack Then
            If DirectCast(Me.Page.Master, VelociRater).IsStaff Then
                Me.divShowArchived.Visible = True
            Else
                Me.divShowArchived.Visible = False
            End If
            InitME()
        End If
    End Sub

    Sub InitME()
        LoadUserNameDropDown()
        LoadStatusDropDown()
        LoadLobDropDown()
    End Sub

    Sub AgencyChanged()
        InitME()
    End Sub

    Private Function FormatAgentUsername(ByVal username As String, ByVal isStaffUser As String) As String
        If isStaffUser Then
            username = String.Format("{0}{1}", username, staffUserNameIndicationText)
        End If
        Return username
    End Function

    Public Sub LoadUserNameDropDown()

        Try
            Dim startingSelectedVal As String = Me.ddAgent.SelectedValue

            If startingSelectedVal = "-1" And DirectCast(Me.Page.Master, VelociRater).IsStaff = False Then
                startingSelectedVal = Session("DiamondUsername")
            End If

            Me.ddAgent.Items.Clear()
            Me.ddAgent.Items.Add(New ListItem("All Users", ""))

            Using sql As New SQLselectObject(connQQ)
                sql.queryOrStoredProc = "usp_SavedQuotes_GetAgencyAvailableAgents"

                Dim masterPage As VelociRater = DirectCast(Me.Page.Master, VR.Web.VelociRater)
                Dim parms As New ArrayList()
                If masterPage.IsStaff And masterPage.AgencyID < 0 Then
                    parms.Add(New System.Data.SqlClient.SqlParameter("@AgencyID", Nothing))
                Else
                    parms.Add(New System.Data.SqlClient.SqlParameter("@AgencyID", masterPage.AgencyID))
                End If
                Dim supportedLobList_Text As String = ""
                For Each Pair In Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                    supportedLobList_Text += Pair.Value.ToString() + ","
                Next
                parms.Add(New System.Data.SqlClient.SqlParameter("@lobList", supportedLobList_Text.Trim(",")))
                sql.parameters = parms

                Using reader As System.Data.SqlClient.SqlDataReader = sql.GetDataReader()
                    If Not sql.hasError Then
                        If reader.HasRows Then
                            While reader.Read()
                                Dim isStaffUser As Boolean = CBool(reader.GetInt32(1))
                                Me.ddAgent.Items.Add(New ListItem(FormatAgentUsername(reader.GetString(0), isStaffUser), reader.GetString(0)))
                            End While
                        End If
                    End If
                End Using
            End Using

            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAgent, startingSelectedVal)
        Catch ex As Exception
#If DEBUG Then
            Debugger.Break()
#End If
            If IFM.VR.Web.Helpers.WebHelper_Personal.IsTesting() Then
                MessageBoxVRPers.Show(ex.Message, Response, ScriptManager.GetCurrent(Me.Page), Me)
            Else
                MessageBoxVRPers.Show("Could not load username drop down.", Response, ScriptManager.GetCurrent(Me.Page), Me)
            End If
        End Try

    End Sub

    Public Sub LoadStatusDropDown()
        Me.ddStatus.Items.Clear()

        ' The val does not mean anything - we will match on TEXT :(
        Me.ddStatus.Items.Add(New ListItem("All Statuses", ""))
        Dim index As Int32 = 1
        For Each status In VR.Common.QuoteSearch.QuoteSearch.FriendlyStatuses
            Dim isStaff As Boolean = DirectCast(Me.Page.Master, VelociRater).IsStaff
            If isStaff = False And status.Key.ToLower().Trim() = "archive" Then
                Continue For
            End If
            Dim csv As String = ""
            For Each Val As Int32 In status.Value
                csv += Val.ToString() + ","
            Next
            Me.ddStatus.Items.Add(New ListItem(status.Key, csv.Trim(",")))
            index += 1
        Next
    End Sub

    Public Sub LoadLobDropDown()
        Me.ddLob.Items.Clear()
        If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Count() > 1 Then
            Me.ddLob.Items.Add(New ListItem("All LOBs", ""))
        End If

        Dim personalLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Personal)
        Dim commLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial)
        Dim farmLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Farm)

        If personalLines.Any() Then
            Dim hI As New ListItem("All Personal Lines", IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Personal)
            Me.ddLob.Items.Add(hI)
            For Each lob In personalLines
                Dim i As New ListItem(lob.Key, lob.Value)
                Me.ddLob.Items.Add(i)
            Next
        End If

        If commLines.Any() Then
            Dim hI As New ListItem("All Commercial Lines", IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Commercial)
            Me.ddLob.Items.Add(hI)
            For Each lob In commLines
                Dim i As New ListItem(lob.Key, lob.Value)
                Me.ddLob.Items.Add(i)
            Next
        End If

        If farmLines.Any() Then
            Dim hI As New ListItem("All Farm Lines", IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Farm)
            Me.ddLob.Items.Add(hI)
            For Each lob In farmLines
                Dim i As New ListItem(lob.Key, lob.Value)
                Me.ddLob.Items.Add(i)
            Next
        End If

        'For Each lob In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
        '    Me.ddLob.Items.Add(New ListItem(lob.Key, lob.Value))
        'Next
    End Sub

    Public Overrides Sub HandleStartUpWorkFlowSelection(workflow As Workflow.WorkflowSection)

    End Sub
End Class