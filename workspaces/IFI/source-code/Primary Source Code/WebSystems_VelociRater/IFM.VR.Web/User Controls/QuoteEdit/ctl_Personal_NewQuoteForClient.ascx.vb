Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Configuration.ConfigurationManager

Public Class ctl_Personal_NewQuoteForClient
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

            'updated 12/16/2022
            If Common.QuoteSave.QuoteSaveHelpers.SaveQuote(qId, qq, errMsg, saveType:=QuickQuoteXML.QuickQuoteSaveType.Quote) = True AndAlso qqhelper.IsPositiveIntegerString(qId) = True Then

                'success; take user to edit page that corresponds to LOB
                Response.Redirect(url & "?quoteid=" & qId)
            Else
                'save failed; show error message
                Throw New Exception("Error saving quote: " & errMsg)
            End If
        End If
        Return False
    End Function

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'If Not Me.IsPostBack Then
        Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    If Me.Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                        lobList = New Dictionary(Of String, String) From {
                            {"Homeowners HOM", "HOM"}
                        }
                    Else
                        lobList = New Dictionary(Of String, String) From {
                            {"Homeowners HOM", "HOM"},
                            {"Personal Umbrella PUP", "PUP"}
                        }
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    If Me.Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                        lobList = New Dictionary(Of String, String) From {
                            {"Personal Auto PPA", "PPA"}
                        }
                    Else
                        lobList = New Dictionary(Of String, String) From {
                            {"Personal Auto PPA", "PPA"},
                            {"Personal Umbrella PUP", "PUP"}
                        }
                    End If
            End Select

            rblLOBList.DataSource = lobList
            rblLOBList.DataBind()
        'End If
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
            Case "HOM"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.HomePersonal, AppSettings("QuickQuote_HOM_Input"))
            Case "PPA"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.AutoPersonal, AppSettings("QuickQuote_PPA_Input"))
            Case "PUP"
                CheckNewLook(QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal, AppSettings("QuickQuote_FUPPUP_Input"))

            Case Else
                Throw New Exception("Unknown LOB!")
        End Select

    End Sub

#End Region




End Class