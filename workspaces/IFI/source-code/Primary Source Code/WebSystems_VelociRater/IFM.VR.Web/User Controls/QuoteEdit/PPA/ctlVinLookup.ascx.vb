Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.PrimativeExtensions

Public Class ctlVinLookup
    Inherits VRControlBase

    Public ReadOnly Property MyVehicle As QuickQuoteVehicle
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Vehicles.GetItemAtIndex(Me.VehicleIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Property VehicleIndex As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = -1
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value            
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If IFM.VR.Common.Helpers.PPA.VehicleLookupPopupHelper.IsVehicleLookupPopupAvailable(Quote) Then
            Me.VRScript.CreateAccordion(Me.divSearchVinLookup.ClientID, Me.hiddenSearchVinLookup, "0")

            Dim isNewRAPALookupAvailable As String = IFM.VR.Common.Helpers.PPA.VinLookup.IsNewModelISORAPALookupTypeAvailable(QQHelper.IntegerForString(Quote.VersionId), QQHelper.DateForString(Quote.EffectiveDate), If(Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, False, True))
            Dim isNewSymbolsAvailable As String = IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(Quote)

            If isNewRAPALookupAvailable Then
                Me.VRScript.CreateJSBinding(Me.ddSearchYear, ctlPageStartupScript.JsEventType.onchange, "GetMakeList('ddSearchYear');GetModelList('ddSearchYear');", True)
                Me.VRScript.CreateJSBinding(Me.ddSearchMake, ctlPageStartupScript.JsEventType.onchange, "GetModelList('ddSearchMake');", True)
            
                Dim vehicleNum As String = "0"
                If MyVehicle IsNot Nothing Then
                    vehicleNum = MyVehicle.VehicleNum
                End If
                Dim isNewBusiness As String = "True"
                If IsQuoteEndorsement() Then
                    isNewBusiness = "False"
                End If
                Me.VRScript.CreateJSBinding(btnSearchVinLookup, ctlPageStartupScript.JsEventType.onclick, "SetupVinYMMSearch(""" & Me.txtSearchVinNumber.ClientID & """,""" & Me.ddSearchMake.ClientID & """,""" & Me.ddSearchModel.ClientID & """,""" & Me.ddSearchYear.ClientID & """,""" & Me.ddSearchModel.ClientID & """,""" & Me.divSearchVinLookup.ClientID & """,""" & Me.divSearchVinLookupContents.ClientID & """,""" & HiddenParentLookupWasFired.ClientID & """,""" & hdnVehicleIndex.ClientID & """,""" & Quote.VersionId & """,""" & Quote.PolicyId & """,""" & Quote.PolicyImageNum & """,""" & vehicleNum & """,""" & isNewBusiness & """,""" & If(Quote.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote, Quote.TransactionEffectiveDate, Quote.EffectiveDate) & """,""" & isNewRAPALookupAvailable & """,""" & isNewSymbolsAvailable & """); $(""#" + HiddenSearchLookupWasFired.ClientID + """).val('1'); return false;")
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        If IFM.VR.Common.Helpers.PPA.VehicleLookupPopupHelper.IsVehicleLookupPopupAvailable(Quote) Then
            Dim hdnYear = hdnSearchYear.Value.Trim()

            Me.ddSearchYear.Items.Clear()
            Me.ddSearchYear.Items.Add(New ListItem("", ""))
            Dim minYear As Integer = 1981
            Dim maxYear = Date.Today.Year + 1

            While (maxYear >= minYear)
                Me.ddSearchYear.Items.Add(New ListItem(maxYear.ToString(), maxYear.ToString()))
                maxYear -= 1
            End While

            Me.ddSearchYear.ClearSelection()
            If Not String.IsNullOrWhiteSpace(hdnYear) Then
                For Each item As ListItem In Me.ddSearchYear.Items
                    If item.Value = hdnYear Then
                        item.Selected = True
                    End If
                Next
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

End Class