Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CIM
Public Class cov_FineArtsFloater_Item
    Inherits VRControlBase

    Public Property FineArtsFloaterCheckbox As CheckBox
        Get
            Dim ceBox As CheckBox = Parent.FindControl("chkFineArtsFloater")
            Return ceBox
        End Get
        Set(value As CheckBox)
            Dim ceBox As CheckBox = Parent.FindControl("chkFineArtsFloater")
            ceBox.Checked = value.Checked
        End Set
    End Property
    Public ReadOnly Property HasArtItems As Boolean
        Get
            Return (From b In Me.Buildings Where b.Building.FineArtsScheduledItems IsNot Nothing AndAlso b.Building.FineArtsScheduledItems.Any() Select b).Any()

        End Get
    End Property

    Public Event AddFineArtsPolicyLevelItems(ThisState As QuickQuoteObject)


    Private ReadOnly Property Buildings As List(Of CIM_Building)
        Get
            Dim list As New List(Of CIM_Building)
            If Quote IsNot Nothing Then
                If Quote.Locations IsNot Nothing Then
                    For locIndex As Int32 = 0 To Quote.Locations.Count - 1
                        Dim zip As String = Quote.Locations(locIndex).Address.Zip
                        If zip.Length > 5 Then
                            zip = zip.Substring(0, 5)
                        End If
                        Dim address As String = String.Format("{0} {1} {2} {3} {4} {5} {6}", Quote.Locations(locIndex).Address.HouseNum, Quote.Locations(locIndex).Address.StreetName, If(String.IsNullOrWhiteSpace(Quote.Locations(locIndex).Address.ApartmentNumber) = False, "Apt# " + Quote.Locations(locIndex).Address.ApartmentNumber, ""), Quote.Locations(locIndex).Address.POBox, Quote.Locations(locIndex).Address.City, Quote.Locations(locIndex).Address.State, zip).Replace("  ", " ").Trim()
                        If Quote.Locations(locIndex).Buildings IsNot Nothing Then
                            For bIndex As Int32 = 0 To Quote.Locations(locIndex).Buildings.Count - 1
                                list.Add(New CIM_Building(locIndex, bIndex, Quote.Locations(locIndex).Address.DisplayAddress, Quote.Locations(locIndex).Buildings(bIndex)))
                            Next
                        End If
                    Next
                End If
            End If
            Return list
        End Get
    End Property
    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then

            Me.faRepeater.DataSource = Buildings
            Me.faRepeater.DataBind()

        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandlers()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)

    End Sub


    Public Overrides Sub ClearControl()
        ClearChildControls()
    End Sub

    Private Sub faRepeater_Add(sender As Object, e As RepeaterItemEventArgs) Handles faRepeater.ItemDataBound
        Dim artControl As cov_FineArtsFloater_Item_Details = e.Item.FindControl("cov_FineArtsFloater_Item_Details")
        artControl.LocationIndex = DirectCast(e.Item.DataItem, CIM_Building).LocationIndex
        artControl.BuildingIndex = DirectCast(e.Item.DataItem, CIM_Building).BuildingIndex

        artControl.Path = artControl.ClientID

        'AddHandler artControl.AddFineArtsPolicyItems, AddressOf cov_FineArtsFloater_Item_Details_AddFineArtsPolicyItems

        artControl.Populate()
    End Sub

    'Private Sub cov_FineArtsFloater_Item_Details_AddFineArtsPolicyItems(ThisState As QuickQuoteObject) Handles cov_FineArtsFloater_Item_Details.AddFineArtsPolicyItems
    '    RaiseEvent AddFineArtsPolicyLevelItems(ThisState)
    'End Sub
    Private Sub cov_FineArtsFloater_Item_Details_AddFineArtsPolicyItems(ThisState As QuickQuoteObject)
        RaiseEvent AddFineArtsPolicyLevelItems(ThisState)
    End Sub

    Private Sub AddHandlers()
        For Each cntrl As RepeaterItem In Me.faRepeater.Items
            Dim artControl As cov_FineArtsFloater_Item_Details = cntrl.FindControl("cov_FineArtsFloater_Item_Details")
            AddHandler artControl.AddFineArtsPolicyItems, AddressOf cov_FineArtsFloater_Item_Details_AddFineArtsPolicyItems
        Next

    End Sub



End Class