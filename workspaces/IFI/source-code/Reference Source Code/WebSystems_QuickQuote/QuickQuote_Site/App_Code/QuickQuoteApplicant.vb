Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store applicant information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteApplicant
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As Generic.List(Of QuickQuotePhone) 'no longer sending to Diamond as-of 12/22/2014; re-rate could have issues since image doesn't load them back in and our logic tries to add them new since it appears that nothing is there (doesn't get db key constraint error like BillingAddressee and Operator since the PK contains emailId/phoneId instead of emailTypeId/phoneTypeId)
        Private _Emails As Generic.List(Of QuickQuoteEmail) 'no longer sending to Diamond as-of 12/22/2014; re-rate could have issues since image doesn't load them back in and our logic tries to add them new since it appears that nothing is there (doesn't get db key constraint error like BillingAddressee and Operator since the PK contains emailId/phoneId instead of emailTypeId/phoneTypeId)
        'updated 8/2/2013 for Personal Lines
        Private _BusinessStartedDate As String
        Private _EducationTypeId As String 'may need matching EducationType variable/property
        Private _Employer As String
        Private _OccupationTypeId As String 'may need matching OccupationType variable/property
        Private _PurchaseDate As String
        Private _RelationshipTypeId As String 'may need matching RelationshipType variable/property
        'Private _CurrentResidenceTypeId As String 'may need matching CurrentResidenceType
        'Private _Owned As Boolean 'added 8/5/2013
        'Private _ResidenceInfoDetailTypeId As String 'may need matching ResidenceInfoDetailType
        'Private _YearsAtPreviousAddress As String
        'Private _YearsAtCurrentAddress As String
        Private _ResidenceInfo As QuickQuoteResidenceInfo 'added 8/5/2013
        Private _SelfEmployedInfo As String
        Private _SpouseEmployer As String
        Private _SpouseOccupationTypeId As String 'may need matching SpouseOccupationType variable/property
        Private _StandardIndustrialClassification As String
        Private _USCitizenTypeId As String 'may need matching USCitizenType variable/property
        Private _YearsWithCurrentEmployer As String
        Private _YearsWithPriorEmployer As String
        'added 3/3/2014
        Private _LossHistoryRecords As List(Of QuickQuoteLossHistoryRecord)

        Private _CanUseLossHistoryNumForLossHistoryReconciliation As Boolean 'added 4/23/2014
        Private _ApplicantNum As String 'added 4/24/2014
        Private _HasApplicantNameChanged As Boolean 'added 4/24/2014
        Private _HasLastNameChanged As Boolean 'added 7/28/2014 to match driver change from 5/12/2014
        Private _HasBirthDateChanged As Boolean 'added 7/28/2014 to match driver change from 5/12/2014

        Private _TieringInformation As QuickQuoteTieringInformation 'added 7/28/2014

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 9/15/2019
        Private _AddedDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String
        Private _AddedImageNum As String

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property

        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                'If _Name IsNot Nothing AndAlso _Name.NameAddressSourceId = "" Then
                '    _Name.NameAddressSourceId = "28" 'Applicant
                'End If
                SetObjectsParent(_Name)
            End Set
        End Property
        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property
        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A46}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{32BB5FB9-093F-42E5-8373-3CB836D45A46}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A47}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{32BB5FB9-093F-42E5-8373-3CB836D45A47}")
            End Set
        End Property
        Public Property BusinessStartedDate As String
            Get
                Return _BusinessStartedDate
            End Get
            Set(value As String)
                _BusinessStartedDate = value
                qqHelper.ConvertToShortDate(_BusinessStartedDate)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's EducationType table (0=N/A, 1=High School, 2=Tech, 3=Vocational, 4=College Graduate); not getting set right now in Diamond (not working for some reason)</remarks>
        Public Property EducationTypeId As String '0=N/A; 1=High School; 2=Tech; 3=Vocational; 4=College Graduate
            Get
                Return _EducationTypeId
            End Get
            Set(value As String)
                _EducationTypeId = value
            End Set
        End Property
        Public Property Employer As String
            Get
                Return _Employer
            End Get
            Set(value As String)
                _Employer = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's OccupationType table</remarks>
        Public Property OccupationTypeId As String 'OccupationType table: -1=[blank or empty string]; 0=N/A; 1=N/A; 2=Alcohol Distributor; 3=Bar/Restaurant Owner; 4=Bartender; etc.
            Get
                Return _OccupationTypeId
            End Get
            Set(value As String)
                _OccupationTypeId = value
            End Set
        End Property
        Public Property PurchaseDate As String
            Get
                Return _PurchaseDate
            End Get
            Set(value As String)
                _PurchaseDate = value
                qqHelper.ConvertToShortDate(_PurchaseDate)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's RelationshipType table</remarks>
        Public Property RelationshipTypeId As String '0=None; 8=Policyholder; 5=Policyholder #2; etc.
            Get
                Return _RelationshipTypeId
            End Get
            Set(value As String)
                _RelationshipTypeId = value
            End Set
        End Property
        'Public Property CurrentResidenceTypeId As String '
        '    Get
        '        Return _CurrentResidenceTypeId
        '    End Get
        '    Set(value As String)
        '        _CurrentResidenceTypeId = value
        '    End Set
        'End Property
        'Public Property Owned As Boolean
        '    Get
        '        Return _Owned
        '    End Get
        '    Set(value As Boolean)
        '        _Owned = value
        '    End Set
        'End Property
        'Public Property ResidenceInfoDetailTypeId As String '1=Previous1; 2=Previous2; 3=Previous3
        '    Get
        '        Return _ResidenceInfoDetailTypeId
        '    End Get
        '    Set(value As String)
        '        _ResidenceInfoDetailTypeId = value
        '    End Set
        'End Property
        'Public Property YearsAtPreviousAddress As String
        '    Get
        '        Return _YearsAtPreviousAddress
        '    End Get
        '    Set(value As String)
        '        _YearsAtPreviousAddress = value
        '    End Set
        'End Property
        'Public Property YearsAtCurrentAddress As String
        '    Get
        '        Return _YearsAtCurrentAddress
        '    End Get
        '    Set(value As String)
        '        _YearsAtCurrentAddress = value
        '    End Set
        'End Property
        Public Property ResidenceInfo As QuickQuoteResidenceInfo
            Get
                SetObjectsParent(_ResidenceInfo)
                Return _ResidenceInfo
            End Get
            Set(value As QuickQuoteResidenceInfo)
                _ResidenceInfo = value
                SetObjectsParent(_ResidenceInfo)
            End Set
        End Property
        Public Property SelfEmployedInfo As String
            Get
                Return _SelfEmployedInfo
            End Get
            Set(value As String)
                _SelfEmployedInfo = value
            End Set
        End Property
        Public Property SpouseEmployer As String
            Get
                Return _SpouseEmployer
            End Get
            Set(value As String)
                _SpouseEmployer = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's OccupationType table</remarks>
        Public Property SpouseOccupationTypeId As String 'OccupationType table: -1=[blank or empty string]; 0=N/A; 1=N/A; 2=Alcohol Distributor; 3=Bar/Restaurant Owner; 4=Bartender; etc.
            Get
                Return _SpouseOccupationTypeId
            End Get
            Set(value As String)
                _SpouseOccupationTypeId = value
            End Set
        End Property
        Public Property StandardIndustrialClassification As String
            Get
                Return _StandardIndustrialClassification
            End Get
            Set(value As String)
                _StandardIndustrialClassification = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's USCitizenType table (-1=Unanswered, 0=N/A, 1=Yes, 2=No)</remarks>
        Public Property USCitizenTypeId As String '-1=Unanswered; 0=N/A; 1=Yes; 2=No
            Get
                Return _USCitizenTypeId
            End Get
            Set(value As String)
                _USCitizenTypeId = value
            End Set
        End Property
        Public Property YearsWithCurrentEmployer As String
            Get
                Return _YearsWithCurrentEmployer
            End Get
            Set(value As String)
                _YearsWithCurrentEmployer = value
            End Set
        End Property
        Public Property YearsWithPriorEmployer As String
            Get
                Return _YearsWithPriorEmployer
            End Get
            Set(value As String)
                _YearsWithPriorEmployer = value
            End Set
        End Property
        'added 3/3/2014
        Public Property LossHistoryRecords As List(Of QuickQuoteLossHistoryRecord)
            Get
                SetParentOfListItems(_LossHistoryRecords, "{32BB5FB9-093F-42E5-8373-3CB836D45A48}")
                Return _LossHistoryRecords
            End Get
            Set(value As List(Of QuickQuoteLossHistoryRecord))
                _LossHistoryRecords = value
                SetParentOfListItems(_LossHistoryRecords, "{32BB5FB9-093F-42E5-8373-3CB836D45A48}")
            End Set
        End Property

        Public Property CanUseLossHistoryNumForLossHistoryReconciliation As Boolean 'added 4/23/2014
            Get
                Return _CanUseLossHistoryNumForLossHistoryReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLossHistoryNumForLossHistoryReconciliation = value
            End Set
        End Property
        Public Property ApplicantNum As String 'added 4/24/2014
            Get
                Return _ApplicantNum
            End Get
            Set(value As String)
                _ApplicantNum = value
            End Set
        End Property
        Public Property HasApplicantNameChanged As Boolean 'added 4/24/2014
            Get
                Return _HasApplicantNameChanged
            End Get
            Set(value As Boolean)
                _HasApplicantNameChanged = value
            End Set
        End Property
        Public Property HasLastNameChanged As Boolean 'added 7/28/2014 to match driver change from 5/12/2014
            Get
                Return _HasLastNameChanged
            End Get
            Set(value As Boolean)
                _HasLastNameChanged = value
            End Set
        End Property
        Public Property HasBirthDateChanged As Boolean 'added 7/28/2014 to match driver change from 5/12/2014
            Get
                Return _HasBirthDateChanged
            End Get
            Set(value As Boolean)
                _HasBirthDateChanged = value
            End Set
        End Property

        Public Property TieringInformation As QuickQuoteTieringInformation 'added 7/28/2014
            Get
                SetObjectsParent(_TieringInformation)
                Return _TieringInformation
            End Get
            Set(value As QuickQuoteTieringInformation)
                _TieringInformation = value
                SetObjectsParent(_TieringInformation)
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        'added 9/15/2019
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
            End Set
        End Property
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property
        Public Property AddedImageNum As String 'added 7/31/2019
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""

            _Name = New QuickQuoteName
            '_Name.NameAddressSourceId = "28" 'Applicant
            'updated 12/11/2013
            _Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Applicant")
            _Address = New QuickQuoteAddress
            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014
            _BusinessStartedDate = ""
            _EducationTypeId = ""
            _Employer = ""
            _OccupationTypeId = ""
            _PurchaseDate = ""
            _RelationshipTypeId = ""
            '_CurrentResidenceTypeId = ""
            '_Owned = False
            '_ResidenceInfoDetailTypeId = ""
            '_YearsAtPreviousAddress = ""
            '_YearsAtCurrentAddress = ""
            _ResidenceInfo = New QuickQuoteResidenceInfo
            _SelfEmployedInfo = ""
            _SpouseEmployer = ""
            _SpouseOccupationTypeId = ""
            _StandardIndustrialClassification = ""
            _USCitizenTypeId = ""
            _YearsWithCurrentEmployer = ""
            _YearsWithPriorEmployer = ""
            'added 3/3/2014
            '_LossHistoryRecords = New List(Of QuickQuoteLossHistoryRecord)
            _LossHistoryRecords = Nothing 'added 8/4/2014

            _CanUseLossHistoryNumForLossHistoryReconciliation = False 'added 4/23/2014
            _ApplicantNum = "" 'added 4/24/2014
            _HasApplicantNameChanged = False 'added 4/24/2014
            _HasLastNameChanged = False 'added 7/28/2014 to match driver change from 5/12/2014
            _HasBirthDateChanged = False 'added 7/28/2014 to match driver change from 5/12/2014

            _TieringInformation = New QuickQuoteTieringInformation 'added 7/28/2014

            _DetailStatusCode = "" 'added 5/15/2019

            'added 9/15/2019
            _AddedDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = ""
        End Sub
        'added 4/23/2014 for lossHistory reconciliation
        Public Sub ParseThruLossHistories()
            If _LossHistoryRecords IsNot Nothing AndAlso _LossHistoryRecords.Count > 0 Then
                For Each lh As QuickQuoteLossHistoryRecord In _LossHistoryRecords
                    '4/23/2014 note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseLossHistoryNumForLossHistoryReconciliation = False Then
                        If lh.HasValidLossHistoryNum = True Then
                            _CanUseLossHistoryNumForLossHistoryReconciliation = True
                            'Exit For 'removed 7/3/2014; needs to keep going so ParseThruLossHistoryDetails can be called for each one
                        End If
                    End If
                    lh.ParseThruLossHistoryDetails() 'added 7/3/2014
                Next
            End If
        End Sub
        Public Function HasValidApplicantNum() As Boolean 'added 4/24/2014 for reconciliation purposes
            'If _ApplicantNum <> "" AndAlso IsNumeric(_ApplicantNum) = True AndAlso CInt(_ApplicantNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ApplicantNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                If Me.Address IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayAddress: " & Me.Address.DisplayAddress, vbCrLf)
                End If
                If Me.RelationshipTypeId <> "" Then
                    Dim rel As String = ""
                    rel = "RelationshipTypeId: " & Me.RelationshipTypeId
                    Dim relType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteApplicant, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, Me.RelationshipTypeId)
                    If relType <> "" Then
                        rel &= " (" & relType & ")"
                    End If
                    str = qqHelper.appendText(str, rel, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If

                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _Phones IsNot Nothing Then
                        If _Phones.Count > 0 Then
                            For Each ph As QuickQuotePhone In _Phones
                                ph.Dispose()
                                ph = Nothing
                            Next
                            _Phones.Clear()
                        End If
                        _Phones = Nothing
                    End If
                    If _Emails IsNot Nothing Then
                        If _Emails.Count > 0 Then
                            For Each em As QuickQuoteEmail In _Emails
                                em.Dispose()
                                em = Nothing
                            Next
                            _Emails.Clear()
                        End If
                        _Emails = Nothing
                    End If
                    If _BusinessStartedDate IsNot Nothing Then
                        _BusinessStartedDate = Nothing
                    End If
                    If _EducationTypeId IsNot Nothing Then
                        _EducationTypeId = Nothing
                    End If
                    If _Employer IsNot Nothing Then
                        _Employer = Nothing
                    End If
                    If _OccupationTypeId IsNot Nothing Then
                        _OccupationTypeId = Nothing
                    End If
                    If _PurchaseDate IsNot Nothing Then
                        _PurchaseDate = Nothing
                    End If
                    If _RelationshipTypeId IsNot Nothing Then
                        _RelationshipTypeId = Nothing
                    End If
                    'If _CurrentResidenceTypeId IsNot Nothing Then
                    '    _CurrentResidenceTypeId = Nothing
                    'End If
                    'If _Owned <> Nothing Then
                    '    _Owned = Nothing
                    'End If
                    'If _ResidenceInfoDetailTypeId IsNot Nothing Then
                    '    _ResidenceInfoDetailTypeId = Nothing
                    'End If
                    'If _YearsAtPreviousAddress IsNot Nothing Then
                    '    _YearsAtPreviousAddress = Nothing
                    'End If
                    'If _YearsAtCurrentAddress IsNot Nothing Then
                    '    _YearsAtCurrentAddress = Nothing
                    'End If
                    If _ResidenceInfo IsNot Nothing Then
                        _ResidenceInfo.Dispose()
                        _ResidenceInfo = Nothing
                    End If
                    If _SelfEmployedInfo IsNot Nothing Then
                        _SelfEmployedInfo = Nothing
                    End If
                    If _SpouseEmployer IsNot Nothing Then
                        _SpouseEmployer = Nothing
                    End If
                    If _SpouseOccupationTypeId IsNot Nothing Then
                        _SpouseOccupationTypeId = Nothing
                    End If
                    If _StandardIndustrialClassification IsNot Nothing Then
                        _StandardIndustrialClassification = Nothing
                    End If
                    If _USCitizenTypeId IsNot Nothing Then
                        _USCitizenTypeId = Nothing
                    End If
                    If _YearsWithCurrentEmployer IsNot Nothing Then
                        _YearsWithCurrentEmployer = Nothing
                    End If
                    If _YearsWithPriorEmployer IsNot Nothing Then
                        _YearsWithPriorEmployer = Nothing
                    End If
                    'added 3/3/2014
                    If _LossHistoryRecords IsNot Nothing Then
                        If _LossHistoryRecords.Count > 0 Then
                            For Each lh As QuickQuoteLossHistoryRecord In _LossHistoryRecords
                                lh.Dispose()
                                lh = Nothing
                            Next
                            _LossHistoryRecords.Clear()
                        End If
                        _LossHistoryRecords = Nothing
                    End If

                    If _CanUseLossHistoryNumForLossHistoryReconciliation <> Nothing Then 'added 4/23/2014
                        _CanUseLossHistoryNumForLossHistoryReconciliation = Nothing
                    End If
                    If _ApplicantNum IsNot Nothing Then 'added 4/24/2014
                        _ApplicantNum = Nothing
                    End If
                    If _HasApplicantNameChanged <> Nothing Then 'added 4/24/2014
                        _HasApplicantNameChanged = Nothing
                    End If
                    _HasLastNameChanged = Nothing 'added 7/28/2014 to match driver change from 5/12/2014
                    _HasBirthDateChanged = Nothing 'added 7/28/2014 to match driver change from 5/12/2014

                    If _TieringInformation IsNot Nothing Then 'added 7/28/2014
                        _TieringInformation.Dispose()
                        _TieringInformation = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    'added 9/15/2019
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)
                    qqHelper.DisposeString(_AddedImageNum)

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
End Namespace
