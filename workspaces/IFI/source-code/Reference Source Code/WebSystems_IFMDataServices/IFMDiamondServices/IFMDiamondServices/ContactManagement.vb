Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.ContactManagement
    Public Module ContactManagement
        Public Function Import(Contacts As DCO.InsCollection(Of DCO.ContactManagement.Contact),
                               Optional ByRef e As System.Exception = Nothing,
                               Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCE.ContactManagement.ContactImportResult
            Dim res As New DCSM.ContactManagementService.Import.Response
            Dim req As New DCSM.ContactManagementService.Import.Request

            With req.RequestData
                .Contacts = Contacts
            End With

            If IFMS.ContactManagement.Import(res, req, e, dv) Then
                Return res.ResponseData.Result
            End If
            Return Nothing
        End Function

        Public Function Load(ContactId As Integer,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ContactManagement.Contact
            Dim res As New DCSM.ContactManagementService.Load.Response
            Dim req As New DCSM.ContactManagementService.Load.Request

            With req.RequestData
                .ContactId = ContactId
            End With

            If IFMS.ContactManagement.Load(res, req, e, dv) Then
                Return res.ResponseData.Contact
            End If
            Return Nothing
        End Function

        Public Function PurgeContacts(ContactStatusId As Integer,
                                      PurgeDate As Date,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As Integer
            Dim res As New DCSM.ContactManagementService.PurgeContacts.Response
            Dim req As New DCSM.ContactManagementService.PurgeContacts.Request

            With req.RequestData
                .ContactStatusTypeId = ContactStatusId
                .PurgeDate = PurgeDate
            End With

            If IFMS.ContactManagement.PurgeContacts(res, req, e, dv) Then
                Return res.ResponseData.NumberOfContactsPurged
            End If
            Return Nothing
        End Function

        Public Function Save(Contact As DCO.ContactManagement.Contact,
                             Optional ByRef e As System.Exception = Nothing,
                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.ContactManagement.Contact
            Dim res As New DCSM.ContactManagementService.Save.Response
            Dim req As New DCSM.ContactManagementService.Save.Request

            With req.RequestData
                .Contact = Contact
            End With

            If IFMS.ContactManagement.Save(res, req, e, dv) Then
                Return res.ResponseData.Contact
            End If
            Return Nothing
        End Function

        Public Function SearchByAddress(City As String,
                                        County As String,
                                        HouseNumber As String,
                                        NameAddressSourceId As Integer,
                                        StateId As Integer,
                                        StreetName As String,
                                        Zip As String,
                                        Optional ByRef e As System.Exception = Nothing,
                                        Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.ContactManagementService.SearchByAddress.Response
            Dim req As New DCSM.ContactManagementService.SearchByAddress.Request

            With req.RequestData
                .City = City
                .County = County
                .HouseNumber = HouseNumber
                .NameAddressSourceId = NameAddressSourceId
                .StateId = StateId
                .StreetName = StreetName
                .Zip = Zip
            End With

            IFMS.ContactManagement.SearchByAddress(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByName(FirstName As String,
                                     LastName As String,
                                     NameAddressSourceId As Integer,
                                     Optional ByRef e As System.Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.ContactManagementService.SearchByName.Response
            Dim req As New DCSM.ContactManagementService.SearchByName.Request

            With req.RequestData
                .FirstName = FirstName
                .LastName = LastName
                .NameAddressSourceId = NameAddressSourceId
            End With

            IFMS.ContactManagement.SearchByName(res, req, e, dv)

            Return Nothing
        End Function

        Public Function SearchByPreferredCustomerNumber(NameAddressSourceId As Integer,
                                                        PreferedCustomer As String,
                                                        Optional ByRef e As System.Exception = Nothing,
                                                        Optional ByRef dv As DCO.DiamondValidation = Nothing)
            Dim res As New DCSM.ContactManagementService.SearchByPreferredCustomerNumber.Response
            Dim req As New DCSM.ContactManagementService.SearchByPreferredCustomerNumber.Request

            With req.RequestData
                .NameAddressSourceId = NameAddressSourceId
                .PreferedCustomer = PreferedCustomer
            End With

            IFMS.ContactManagement.SearchByPreferredCustomerNumber(res, req, e, dv)

            Return Nothing
        End Function
    End Module
End Namespace