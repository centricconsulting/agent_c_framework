Imports System.Web
Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to hold additional interest information
    ''' </summary>
    ''' <remarks>can be tied to vehicles, buildings, etc.</remarks>
    <Serializable()>
    Public Class QuickQuoteAdditionalInterest
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass
        Dim chc As New CommonHelperClass

        'removed private variables 4/1/2020
        'Private _PolicyId As String
        'Private _PolicyImageNum As String
        'Private _ATIMA As Boolean

        'Private _Name As QuickQuoteName
        'Private _Address As QuickQuoteAddress
        'Private _Phones As Generic.List(Of QuickQuotePhone)
        'Private _Emails As Generic.List(Of QuickQuoteEmail)

        ''AdditionalInterestList nodes
        'Private _GroupTypeId As String 'Diamond table AdditionalInterestGroupType
        'Private _ListId As String 'also at AdditionalInterest level
        'Private _AgencyId As String
        'Private _SingleEntry As Boolean
        'Private _StatusCode As String

        'Private _Num As String 'started using 4/29/2014 for reconciliation
        'Private _TypeId As String 'Diamond table AdditionalInterestType
        'Private _TypeName As AdditionalInterestType = AdditionalInterestType.NA
        'Private _BillTo As Boolean
        'Private _Description As String
        'Private _HasWaiverOfSubrogation As Boolean
        'Private _ISAOA As Boolean
        'Private _InterestInProperty As String
        'Private _LoanAmount As String
        'Private _LoanNumber As String
        'Private _Other As String
        'Private _TrustAgreementDate As String

        Private _NameAddressNum As Integer 'added 8/7/2012 for use w/ Generic.List(Of QuickQuoteGenericNameAddress)

        'removed private variables 4/1/2020
        'Private _HasAdditionalInterestListNameChanged As Boolean 'added 4/29/2014
        'Private _OverwriteAdditionalInterestListInfoForDiamondId As Boolean 'added 5/6/2014
        'Private _HasAdditionalInterestListIdChanged As Boolean 'added 5/6/2014

        'Private _DetailStatusCode As String 'added 5/15/2019; removed 4/1/2020

        'added 4/1/2020
        Private _Base As QuickQuoteAdditionalInterestBase
        Private _List As QuickQuoteAdditionalInterestList

        'added 5/13/2021
        Private _OriginalSourceAI As QuickQuoteAdditionalInterest = Nothing
        'added 5/20/2021
        Private _ListPosition As Integer = 0

        Public Enum AdditionalInterestType
            NA = 0
            ManagerLessorOfPremises = 1
            LeasedLand = 2
            LeasedEquipment = 3
            GrantorOfFranchise = 4
            StateOrPoliticalSubdivisionPermits = 5
            DesignatedPerson = 6
            Mortgagee = 7
            LossPayee = 8
            AdditionalInsured = 9
            Contract = 10
            SecondMortgagee = 11
            Other = 12
            Seller = 13
            AdditionalInterest = 14
            ThirdMortgagee = 15
            LienHolder = 16
            PropertyAdditionalInsured = 17
            PropertyJointOwner = 18
            PropertyCoTitleholder = 19
            PropertyLessor = 20
            LiabilityJointOwner = 21
            LiabilityCoTitleholder = 22
            LiabilityLessor = 23
            LiabilityOccupyingResidenceonPremises = 24
            LiabilityNonRelativeResident = 25
            CommercialLiabilityCoOwner = 26
            CommercialLiabilityControllingInterest = 27
            CommercialLiabilityMortgageeAssigneeOrReceiver = 28
            HBBLandlord = 29
            HBBControllingInterest = 30
            LiabilityAdditionalInsured31 = 31 'Duplicate in Diamond DB - Not sure which one is in use
            Lessor = 32
            CoTitleholder = 33
            NamedInsured = 34
            AdditionalNamedInsured = 35
            LiabilityAdditionalInsured36 = 36 'Duplicate in Diamond DB - Not sure which one is in use
            Landlords = 37
            ControllingInterest = 38
            NewlyAcquiredOrganization = 39
            TrustAndTrustee = 40
            LeasingCompany = 41
            FirstMortgagee = 42
            AdditionalInsuredCoOwners = 43
            AdditionalInsuredLifeEstate = 44
            Arrangement = 45
            AdditionalInsuredLivingTrustsAndTheirExecutorsAdministratorsOrTrustees = 46
            AdditionalInsuredLongTermContract = 47
            AdditionalInsuredNonResidentOfHousehold = 48
            AdditionalInsuredLandlord = 49
            ContractofSale = 50
            OtherMembersOfNamedInsuredHousehold = 51
            GrantorBeneficiary = 52
            FirstLienholder = 53
            SecondLienholder = 54
            AssigneeOrReceiver = 55
            AdditionalInsuredPartnersCorporateOfficersOrCoowners = 56
            AdditionalInsuredPersonsOrOrganizations = 57
            Employee = 58
            SoleProprietor = 59
            Partner = 60
            Officer = 61
            FarmOrAgriculturalWorker = 62
            DomesticOrHouseholdWorker = 63
            CA2001AdditionalInterest = 64
            LossPayableLossPayable = 65
            LossPayableLendersLossPayable = 66
            LossPayableContractOfSale = 67
            ContractSeller = 68
            CoDeedholder = 69
            AdditionalInsuredRevocableTrust = 70
            LossPayable = 71
            JointLossPayable = 72
            MortgageeOrAssigneeOrReceiver = 73
            LendersLossPayable = 74
            CA9961LossPayeeAudioVisualEquipment = 75
            CA9944LossPayable = 76
            AdditionalInsuredBuildingOwner = 77
            LossPayableBuildingOwner = 78
            Trust = 79
            Trustee = 80
            Relative = 81
        End Enum

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyId As String
            Get
                Return Base.PolicyId
            End Get
            Set(value As String)
                Base.PolicyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyImageNum As String
            Get
                Return Base.PolicyImageNum
            End Get
            Set(value As String)
                Base.PolicyImageNum = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ATIMA As Boolean
            Get
                Return Base.ATIMA
            End Get
            Set(value As Boolean)
                Base.ATIMA = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Name As QuickQuoteName
            Get
                'SetObjectsParent(_Name)
                Return List.Name
            End Get
            Set(value As QuickQuoteName)
                List.Name = value
                'If _Name IsNot Nothing AndAlso _Name.NameAddressSourceId = "" Then
                '    _Name.NameAddressSourceId = "12" 'Additional Interest
                '    SetObjectsParent(_Name)
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Address As QuickQuoteAddress
            Get
                'SetObjectsParent(_Address)
                Return List.Address
            End Get
            Set(value As QuickQuoteAddress)
                List.Address = value
                'SetObjectsParent(_Address)
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                'SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A41}")
                Return List.Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                List.Phones = value
                'SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A41}")
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                'SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A42}")
                Return List.Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                List.Emails = value
                'SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A42}")
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property GroupTypeId As String
            Get
                Return List.GroupTypeId
            End Get
            Set(value As String)
                List.GroupTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ListId As String
            Get
                Return List.ListId
            End Get
            Set(value As String)
                List.ListId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyId As String
            Get
                Return List.AgencyId
            End Get
            Set(value As String)
                List.AgencyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property SingleEntry As Boolean
            Get
                Return List.SingleEntry
            End Get
            Set(value As Boolean)
                List.SingleEntry = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property StatusCode As String
            Get
                Return List.StatusCode
            End Get
            Set(value As String)
                List.StatusCode = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Num As String 'started using 4/29/2014 for reconciliation
            Get
                Return Base.Num
            End Get
            Set(value As String)
                Base.Num = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's AdditionalInterestType table</remarks>
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TypeId As String '9/26/2013 note: AdditionalInterestType table
            Get
                Return Base.TypeId
            End Get
            Set(value As String)
                Base.TypeId = value
                'If chc.IsNumericString(_TypeId) AndAlso chc.NumericStringComparison(_TypeId, CommonHelperClass.ComparisonOperators.GreaterThan, 0) Then
                '    _TypeName = CInt(_TypeId)
                'Else
                '    _TypeName = AdditionalInterestType.NA
                'End If
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TypeName As AdditionalInterestType
            Get
                Return Base.TypeName
            End Get
            Set(value As AdditionalInterestType)
                Base.TypeName = value
                '_TypeId = _TypeName
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillTo As Boolean
            Get
                Return Base.BillTo
            End Get
            Set(value As Boolean)
                Base.BillTo = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Description As String
            Get
                Return Base.Description
            End Get
            Set(value As String)
                Base.Description = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HasWaiverOfSubrogation As Boolean
            Get
                Return Base.HasWaiverOfSubrogation
            End Get
            Set(value As Boolean)
                Base.HasWaiverOfSubrogation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ISAOA As Boolean
            Get
                Return Base.ISAOA
            End Get
            Set(value As Boolean)
                Base.ISAOA = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property InterestInProperty As String
            Get
                Return Base.InterestInProperty
            End Get
            Set(value As String)
                Base.InterestInProperty = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LoanAmount As String
            Get
                Return Base.LoanAmount
            End Get
            Set(value As String)
                Base.LoanAmount = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LoanNumber As String
            Get
                Return Base.LoanNumber
            End Get
            Set(value As String)
                Base.LoanNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Other As String
            Get
                Return Base.Other
            End Get
            Set(value As String)
                Base.Other = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TrustAgreementDate As String
            Get
                Return Base.TrustAgreementDate
            End Get
            Set(value As String)
                Base.TrustAgreementDate = value
                'qqHelper.ConvertToShortDate(_TrustAgreementDate)
            End Set
        End Property

        Public Property NameAddressNum As Integer
            Get
                Return _NameAddressNum
            End Get
            Set(value As Integer)
                _NameAddressNum = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HasAdditionalInterestListNameChanged As Boolean 'added 4/29/2014
            Get
                Return List.HasAdditionalInterestListNameChanged
            End Get
            Set(value As Boolean)
                List.HasAdditionalInterestListNameChanged = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property OverwriteAdditionalInterestListInfoForDiamondId As Boolean 'added 5/6/2014
            Get
                Return List.OverwriteAdditionalInterestListInfoForDiamondId
            End Get
            Set(value As Boolean)
                List.OverwriteAdditionalInterestListInfoForDiamondId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HasAdditionalInterestListIdChanged As Boolean 'added 5/6/2014
            Get
                Return List.HasAdditionalInterestListIdChanged
            End Get
            Set(value As Boolean)
                List.HasAdditionalInterestListIdChanged = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return Base.DetailStatusCode
            End Get
            Set(value As String)
                Base.DetailStatusCode = value
            End Set
        End Property

        'added 4/1/2020
        Public Property Base As QuickQuoteAdditionalInterestBase
            Get
                If _Base Is Nothing Then
                    _Base = New QuickQuoteAdditionalInterestBase
                End If
                SetObjectsParent(_Base)
                Return _Base
            End Get
            Set(value As QuickQuoteAdditionalInterestBase)
                _Base = value
                SetObjectsParent(_Base)
            End Set
        End Property
        Public Property List As QuickQuoteAdditionalInterestList
            Get
                If _List Is Nothing Then
                    _List = New QuickQuoteAdditionalInterestList
                End If
                SetObjectsParent(_List)
                Return _List
            End Get
            Set(value As QuickQuoteAdditionalInterestList)
                _List = value
                SetObjectsParent(_List)
            End Set
        End Property

        'added 12/23/2020
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AddedDate As String
            Get
                Return Base.AddedDate
            End Get
            Set(value As String)
                Base.AddedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AddedImageNum As String
            Get
                Return Base.AddedImageNum
            End Get
            Set(value As String)
                Base.AddedImageNum = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LastModifiedDate As String
            Get
                Return Base.LastModifiedDate
            End Get
            Set(value As String)
                Base.LastModifiedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PCAdded_Date As String
            Get
                Return Base.PCAdded_Date
            End Get
            Set(value As String)
                Base.PCAdded_Date = value
            End Set
        End Property

        'added 5/13/2021
        Public ReadOnly Property OriginalSourceAI As QuickQuoteAdditionalInterest
            Get
                Return _OriginalSourceAI
            End Get
        End Property


        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            'added 4/1/2020
            _Base = New QuickQuoteAdditionalInterestBase
            _List = New QuickQuoteAdditionalInterestList

            'removed private variables 4/1/2020
            '_PolicyId = ""
            '_PolicyImageNum = ""
            '_ATIMA = False

            '_Name = New QuickQuoteName
            ''_Name.NameAddressSourceId = "12" 'Additional Interest '5/12/2014 note: may need to update to use static data list... done
            '_Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Additional Interest")
            '_Address = New QuickQuoteAddress
            ''_Phones = New Generic.List(Of QuickQuotePhone)
            ''_Emails = New Generic.List(Of QuickQuoteEmail)

            '_GroupTypeId = "0" 'defaulted to 0 (Not Assigned) on 8/23/2012
            '_ListId = ""
            '_AgencyId = ""
            '_SingleEntry = True 'False'defaulted to True on 8/23/2012
            '_StatusCode = "1" 'defaulted to 1 on 8/23/2012

            '_Num = ""
            '_TypeId = ""
            '_BillTo = False
            '_Description = ""
            '_HasWaiverOfSubrogation = False
            '_ISAOA = False
            '_InterestInProperty = ""
            '_LoanAmount = ""
            '_LoanNumber = ""
            '_Other = ""
            '_TrustAgreementDate = ""

            _NameAddressNum = 0

            'removed private variables 4/1/2020
            '_HasAdditionalInterestListNameChanged = False 'added 4/29/2014
            '_OverwriteAdditionalInterestListInfoForDiamondId = False 'added 5/6/2014
            '_HasAdditionalInterestListIdChanged = False 'added 5/6/2014

            '_DetailStatusCode = "" 'added 5/15/2019; removed 4/1/2020

            '5/13/2021 note: no need to initialize _OriginalSourceAI since it's set to Nothing in the declaration
        End Sub
        Public Function HasValidAdditionalInterestListId() As Boolean 'added 4/27/2014
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_ListId)
            Return List.HasValidAdditionalInterestListId()
        End Function
        Public Function HasValidAdditionalInterestNum() As Boolean 'added 4/29/2014
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_Num)
            Return Base.HasValidAdditionalInterestNum()
        End Function
        Public Function HasInfo() As Boolean 'added 4/1/2020
            If Base.HasInfo() = True OrElse List.HasInfo() = True Then
                Return True
            Else
                Return False
            End If
        End Function

        'added 5/13/2021
        Public Sub Set_OriginalSourceAI(ByVal sourceAI As QuickQuoteAdditionalInterest)
            _OriginalSourceAI = sourceAI
        End Sub

        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                If Me.Address IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayAddress: " & Me.Address.DisplayAddress, vbCrLf)
                End If
                If Me.TypeId <> "" Then
                    Dim ai As String = ""
                    ai = "TypeId: " & Me.TypeId
                    Dim aiType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, Me.TypeId)
                    If aiType <> "" Then
                        ai &= " (" & aiType & ")"
                    End If
                    str = qqHelper.appendText(str, ai, vbCrLf)
                End If
                If Me.ListId <> "" Then
                    str = qqHelper.appendText(str, "ListId: " & Me.ListId, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'added 5/20/2021
        Protected Friend Sub Set_ListPosition(ByVal val As Integer)
            _ListPosition = val
        End Sub
        Protected Friend Function ListPosition() As Integer
            Return _ListPosition
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    'removed private variables 4/1/2020
                    'If _PolicyId IsNot Nothing Then
                    '    _PolicyId = Nothing
                    'End If
                    'If _PolicyImageNum IsNot Nothing Then
                    '    _PolicyImageNum = Nothing
                    'End If
                    'If _ATIMA <> Nothing Then
                    '    _ATIMA = Nothing
                    'End If

                    'If _Name IsNot Nothing Then
                    '    _Name.Dispose()
                    '    _Name = Nothing
                    'End If
                    'If _Address IsNot Nothing Then
                    '    _Address.Dispose()
                    '    _Address = Nothing
                    'End If
                    'If _Phones IsNot Nothing Then
                    '    If _Phones.Count > 0 Then
                    '        For Each ph As QuickQuotePhone In _Phones
                    '            ph.Dispose()
                    '            ph = Nothing
                    '        Next
                    '        _Phones.Clear()
                    '    End If
                    '    _Phones = Nothing
                    'End If
                    'If _Emails IsNot Nothing Then
                    '    If _Emails.Count > 0 Then
                    '        For Each em As QuickQuoteEmail In _Emails
                    '            em.Dispose()
                    '            em = Nothing
                    '        Next
                    '        _Emails.Clear()
                    '    End If
                    '    _Emails = Nothing
                    'End If

                    'If _GroupTypeId IsNot Nothing Then
                    '    _GroupTypeId = Nothing
                    'End If
                    'If _ListId IsNot Nothing Then
                    '    _ListId = Nothing
                    'End If
                    'If _AgencyId IsNot Nothing Then
                    '    _AgencyId = Nothing
                    'End If
                    'If _SingleEntry <> Nothing Then
                    '    _SingleEntry = Nothing
                    'End If
                    'If _StatusCode IsNot Nothing Then
                    '    _StatusCode = Nothing
                    'End If

                    'If _Num IsNot Nothing Then
                    '    _Num = Nothing
                    'End If
                    'If _TypeId IsNot Nothing Then
                    '    _TypeId = Nothing
                    'End If
                    'If _BillTo <> Nothing Then
                    '    _BillTo = Nothing
                    'End If
                    'If _Description IsNot Nothing Then
                    '    _Description = Nothing
                    'End If
                    'If _HasWaiverOfSubrogation <> Nothing Then
                    '    _HasWaiverOfSubrogation = Nothing
                    'End If
                    'If _ISAOA <> Nothing Then
                    '    _ISAOA = Nothing
                    'End If
                    'If _InterestInProperty IsNot Nothing Then
                    '    _InterestInProperty = Nothing
                    'End If
                    'If _LoanAmount IsNot Nothing Then
                    '    _LoanAmount = Nothing
                    'End If
                    'If _LoanNumber IsNot Nothing Then
                    '    _LoanNumber = Nothing
                    'End If
                    'If _Other IsNot Nothing Then
                    '    _Other = Nothing
                    'End If
                    'If _TrustAgreementDate IsNot Nothing Then
                    '    _TrustAgreementDate = Nothing
                    'End If

                    If _NameAddressNum <> Nothing Then
                        _NameAddressNum = Nothing
                    End If

                    'removed private variables 4/1/2020
                    'If _HasAdditionalInterestListNameChanged <> Nothing Then
                    '    _HasAdditionalInterestListNameChanged = Nothing
                    'End If
                    'If _OverwriteAdditionalInterestListInfoForDiamondId <> Nothing Then
                    '    _OverwriteAdditionalInterestListInfoForDiamondId = Nothing
                    'End If
                    'If _HasAdditionalInterestListIdChanged <> Nothing Then
                    '    _HasAdditionalInterestListIdChanged = Nothing
                    'End If

                    'qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019; removed 4/1/2020

                    'added 4/1/2020
                    If _Base IsNot Nothing Then
                        _Base.Dispose()
                        _Base = Nothing
                    End If
                    If _List IsNot Nothing Then
                        _List.Dispose()
                        _List = Nothing
                    End If

                    '5/13/2021 note: don't dispose of _OriginalSourceAI

                    MyBase.Dispose() 'added 8/4/2014
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

    'added 5/20/2021
    Public Class QuickQuoteAdditionalInterestComparer_ByListPositionOrCopiedNum
        Implements IComparer(Of QuickQuoteAdditionalInterest)

        Public Function Compare(x As QuickQuoteAdditionalInterest, y As QuickQuoteAdditionalInterest) As Integer Implements IComparer(Of QuickQuoteAdditionalInterest).Compare
            'Throw New NotImplementedException()
            If x Is Nothing AndAlso y Is Nothing Then
                Return 0
            ElseIf x Is Nothing Then
                Return -1
            ElseIf y Is Nothing Then
                Return 1
            Else
                Dim qqHelper As New QuickQuoteHelperClass
                Dim AdditionalInterestNum_X As Integer = qqHelper.IntegerForString(x.Num)
                Dim AdditionalInterestNum_Y As Integer = qqHelper.IntegerForString(y.Num)
                Dim maxInt As Integer = Integer.MaxValue 'will be used for ones that don't have a valid additionalInterestNum to keep them at the end of the list
                If AdditionalInterestNum_X <= 0 Then
                    AdditionalInterestNum_X = maxInt
                End If
                If AdditionalInterestNum_Y <= 0 Then
                    AdditionalInterestNum_Y = maxInt
                End If
                If x.OriginalSourceAI IsNot Nothing OrElse y.OriginalSourceAI IsNot Nothing Then
                    If x.OriginalSourceAI IsNot Nothing AndAlso x.OriginalSourceAI.HasValidAdditionalInterestNum() = True Then
                        AdditionalInterestNum_X = qqHelper.IntegerForString(x.OriginalSourceAI.Num)
                    End If
                    If y.OriginalSourceAI IsNot Nothing AndAlso y.OriginalSourceAI.HasValidAdditionalInterestNum() = True Then
                        AdditionalInterestNum_Y = qqHelper.IntegerForString(y.OriginalSourceAI.Num)
                    End If
                    If AdditionalInterestNum_X = AdditionalInterestNum_Y Then
                        If x.ListPosition = y.ListPosition Then
                            Return 0
                        Else
                            Return x.ListPosition.CompareTo(y.ListPosition)
                        End If
                    Else
                        Return AdditionalInterestNum_X.CompareTo(AdditionalInterestNum_Y)
                    End If
                Else
                    If x.ListPosition = y.ListPosition Then
                        If AdditionalInterestNum_X = AdditionalInterestNum_Y Then
                            Return 0
                        Else
                            Return AdditionalInterestNum_X.CompareTo(AdditionalInterestNum_Y)
                        End If
                    Else
                        Return x.ListPosition.CompareTo(y.ListPosition)
                    End If
                End If
            End If
        End Function
    End Class
    Public Class QuickQuoteVehicleAndAdditionalInterests
        Public Property Vehicle As QuickQuoteVehicle = Nothing
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
    End Class
    Public Class QuickQuoteLocationAndAdditionalInterests
        Public Property Location As QuickQuoteLocation = Nothing
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
    End Class
    Public Class QuickQuoteBuildingAndAdditionalInterests
        Public Property Building As QuickQuoteBuilding = Nothing
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
    End Class
    Public Class QuickQuoteBuildingWithParentLocationAndAdditionalInterests
        Public Property Location As QuickQuoteLocation = Nothing
        Public Property Building As QuickQuoteBuilding = Nothing
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
    End Class

    Public Class QuickQuoteInlandMarineContractorsCoverageAndAdditionalInterests
        Public Property Coverage As QuickQuoteContractorsEquipmentScheduledCoverage = Nothing
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
    End Class
    Public Class QuickQuoteAdditionalInterestRelatedResults
        Public Property TopLevelAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property VehiclesAndAdditionalInterests As List(Of QuickQuoteVehicleAndAdditionalInterests) = Nothing
        Public Property LocationsAndAdditionalInterests As List(Of QuickQuoteLocationAndAdditionalInterests) = Nothing
        Public Property BuildingsWithParentLocationAndAdditionalInterests As List(Of QuickQuoteBuildingWithParentLocationAndAdditionalInterests) = Nothing
        Public Property InlandMarineBuildersRiskAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property InlandMarineComputersAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property InlandMarineContractorsEquipmentAndAdditionalInterests As List(Of QuickQuoteInlandMarineContractorsCoverageAndAdditionalInterests) = Nothing
        Public Property InlandMarineFineArtsAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property InlandMarineMotorTruckCargoAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property InlandMarineOwnersCargoAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property InlandMarineScheduledPropertyFloaterAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property InlandMarineSignsAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing
        Public Property InlandMarineTransportationAdditionalInterests As List(Of QuickQuoteAdditionalInterest) = Nothing

    End Class
End Namespace
