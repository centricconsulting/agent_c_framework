Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Configuration.ConfigurationManager

Public Class ctl_Comm_NewQuoteForClient
    Inherits VRControlBase

    Public Property lobList As Dictionary(Of String, String) = Nothing
#Region "Declarations"

#End Region

#Region "Methods and Functions"

    Protected Function CheckNewLook(lob As QuickQuoteObject.QuickQuoteLobType, url As String) As Boolean
        If url IsNot Nothing AndAlso Request("quoteid") IsNot Nothing Then
            Dim qq As New QuickQuote.CommonObjects.QuickQuoteObject
            Dim qqhelper = New QuickQuoteHelperClass
            Dim QQxml = New QuickQuoteXML
            Dim qId As String = ""
            Dim errMsg As String = ""

            qq.AgencyId = Me.Quote.AgencyId
            qq.AgencyCode = Me.Quote.AgencyCode
            qq.Client = qqhelper.CloneObject(Me.Quote.Client)
            qq.StateId = Me.Quote.StateId


            qq.LobType = lob
            'QQxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qId, errMsg) 'removed 12/16/2022; now handled below w/ QuoteSaveHelpers.SaveQuote call

            'If qqhelper.IsPositiveIntegerString(qId) = True AndAlso String.IsNullOrWhiteSpace(errMsg) = True Then
            'updated 12/16/2022
            If Common.QuoteSave.QuoteSaveHelpers.SaveQuote(qId, qq, errMsg, saveType:=QuickQuoteXML.QuickQuoteSaveType.Quote) = True AndAlso qqhelper.IsPositiveIntegerString(qId) = True Then
                Dim performSaveAfterNameConversion As Boolean = False
                Select Case qq.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        If qq?.Policyholder?.Name?.TypeId <> "2" Then
                            If qq?.Policyholder?.Name?.CommercialName1 = "" Then
                                SetCommercialNameFromPersonal(qq)
                            End If
                            ClearPersonalInfo(qq)
                            performSaveAfterNameConversion = True
                        End If
                End Select
                If performSaveAfterNameConversion Then
                    Common.QuoteSave.QuoteSaveHelpers.SaveQuote(qId, qq, errMsg, saveType:=QuickQuoteXML.QuickQuoteSaveType.Quote)
                End If

                'success; take user to edit page that corresponds to LOB
                Response.Redirect(url & "?quoteid=" & qId)
            Else
                'save failed; show error message
                Throw New Exception("Error saving quote: " & errMsg)
            End If
        End If
        Return False
    End Function

    Private Sub ClearPersonalInfo(qq As QuickQuoteObject)
        If qq IsNot Nothing AndAlso qq.Policyholder IsNot Nothing AndAlso qq.Policyholder.Name IsNot Nothing Then
            qq.Policyholder.Name.FirstName = "" 'Otherwise gives validation error "Must be a commercial name but you have a personal name"
            qq.Policyholder.Name.MiddleName = ""
            qq.Policyholder.Name.LastName = ""
            qq.Policyholder.Name.SexId = "" 'Otherwise gives validation error "Invalid Gender"
        End If
    End Sub

    Private Sub SetCommercialNameFromPersonal(qq As QuickQuoteObject)
        If qq IsNot Nothing AndAlso qq.Policyholder IsNot Nothing AndAlso qq.Policyholder.Name IsNot Nothing AndAlso IsNullEmptyorWhitespace(qq.Policyholder.Name.DisplayName) = False Then
            qq.Policyholder.Name.CommercialName1 = qq.Policyholder.Name.DisplayName
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Not Me.IsPostBack Then
            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    lobList = New Dictionary(Of String, String) From {
                        {"Farm FAR", "FAR"},
                        {"Farm Commercial Automobile CAP", "CAP"}
                    }
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto,
                     QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    lobList = New Dictionary(Of String, String) From {
                        {"Businessowners BOP", "BOP"},
                        {"Workers Compensation WCP", "WCP"},
                        {"General Liability CGL", "CGL"},
                        {"Commercial Automobile CAP", "CAP"},
                        {"Commercial Property CPR", "CPR"},
                        {"Commercial Package CPP", "CPP"},
                        {"Farm FAR", "FAR"},
                        {"Farm Commercial Automobile CAP", "CAP"}
                    }
                Case Else
                    lobList = New Dictionary(Of String, String) From {
                        {"Businessowners BOP", "BOP"},
                        {"Workers Compensation WCP", "WCP"},
                        {"General Liability CGL", "CGL"},
                        {"Commercial Automobile CAP", "CAP"},
                        {"Commercial Property CPR", "CPR"},
                        {"Commercial Package CPP", "CPP"}
                    }
            End Select

            If Me.GoverningStateQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                Dim hasWCP As Boolean = False
                For Each i In lobList
                    If i.Value = "WCP" Then
                        hasWCP = True
                        Exit For
                    End If
                Next
                If hasWCP Then
                    Dim keyToRemove As String = lobList.First(Function(kvp) kvp.Value = "WCP").Key
                    lobList.Remove(keyToRemove)
                End If
            End If
            rblLOBList.DataSource = lobList
            rblLOBList.DataBind()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub



#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Dim msg As String = Nothing
        Dim chosenLOB = rblLOBList.SelectedValue

        Select Case chosenLOB.ToUpper()
            Case "BOP"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.CommercialBOP, AppSettings("QuickQuote_BOP_Quote_NewLook"))
                Exit Select
            Case "WCP"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, AppSettings("QuickQuote_WCP_Quote_NewLook"))
                Exit Select
            Case "CGL"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, AppSettings("QuickQuote_CGL_Quote_NewLook"))
                Exit Select
            Case "CAP"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.CommercialAuto, AppSettings("QuickQuote_CAP_Quote_NewLook"))
                Exit Select
            Case "CPR"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.CommercialProperty, AppSettings("QuickQuote_CPR_Quote_NewLook"))
                Exit Select
            Case "CPP"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.CommercialPackage, AppSettings("QuickQuote_CPP_Quote_NewLook"))
                Exit Select
            Case "FAR"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.Farm, "VR3Farm.aspx")
            Case Else
                Throw New Exception("Unknown LOB!")
        End Select

    End Sub

#End Region




End Class