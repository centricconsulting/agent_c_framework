Imports Microsoft.VisualBasic
Imports QuickQuote.CommonObjects
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports System.Data.SqlClient
Imports System.Configuration

Namespace QuickQuote.CommonMethods
    ''' <summary>
    ''' class used for common third party report methods
    ''' </summary>
    ''' <remarks>used w/ third party report ordering/retrieving</remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyReportHelperClass 'added 12/11/2014

        Dim qqHelper As New QuickQuoteHelperClass

        'initial code taken from QuickQuoteHelperClass
        'added 12/5/2014 for CLUE specs (bug #s 3160/3638)
        Public Sub InsertThirdPartyReportLogRecord(ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "")
            errorMsg = ""
            sqlErrorMsg = ""

            If thirdPartyReportLogRecord IsNot Nothing Then
                If thirdPartyReportLogRecord.thirdPartyReportTypeId > 0 AndAlso thirdPartyReportLogRecord.policyId > 0 AndAlso thirdPartyReportLogRecord.policyImageNum > 0 Then
                    Using sqlEO As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                        sqlEO.queryOrStoredProc = "usp_Insert_ThirdPartyReportLog"
                        'finished code 12/8/2014
                        '@thirdPartyReportTypeId int,
                        '@policyId int,
                        '@policyImageNum int,
                        '@unitNum int = NULL,
                        '@name1First varchar(50) = NULL,
                        '@name1Middle varchar(50) = NULL,
                        '@name1Last varchar(50) = NULL,
                        '@name1DOB date = NULL,
                        '@name1SSN varchar(20) = NULL, 
                        '@name2First varchar(50) = NULL,
                        '@name2Middle varchar(50) = NULL,
                        '@name2Last varchar(50) = NULL,
                        '@name2DOB date = NULL,
                        '@name2SSN varchar(20) = NULL, 
                        '@addressStreetNum varchar(20) = NULL,
                        '@addressStreetName varchar(100) = NULL,
                        '@addressApartmentNum varchar(20) = NULL,
                        '@addressCity varchar(100) = NULL,
                        '@addressState varchar(20) = NULL,
                        '@addressZip varchar(10) = NULL,
                        '@thirdPartyReportLogId int output
                        '12/18/2014 - changed db structure and params
                        '@thirdPartyReportTypeId int,
                        '@policyNumber varchar(12) = NULL,
                        '@policyId int,
                        '@policyImageNum int,
                        '@unitNum int = NULL,
                        '@quoteId int = NULL,
                        '@agencyId int = NULL,
                        '@agencyCode varchar(10) = NULL,
                        '@userId int = NULL,
                        '@thirdPartyReportLogId int output

                        sqlEO.inputParameters = New ArrayList
                        sqlEO.inputParameters.Add(New SqlParameter("@thirdPartyReportTypeId", thirdPartyReportLogRecord.thirdPartyReportTypeId))
                        If thirdPartyReportLogRecord.policyNumber <> "" Then 'new 12/18/2014
                            sqlEO.inputParameters.Add(New SqlParameter("@policyNumber", thirdPartyReportLogRecord.policyNumber))
                        End If
                        sqlEO.inputParameters.Add(New SqlParameter("@policyId", thirdPartyReportLogRecord.policyId))
                        sqlEO.inputParameters.Add(New SqlParameter("@policyImageNum", thirdPartyReportLogRecord.policyImageNum))
                        sqlEO.inputParameters.Add(New SqlParameter("@unitNum", thirdPartyReportLogRecord.unitNum))
                        'If thirdPartyReportLogRecord.name1First <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name1First", thirdPartyReportLogRecord.name1First))
                        'End If
                        'If thirdPartyReportLogRecord.name1Middle <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name1Middle", thirdPartyReportLogRecord.name1Middle))
                        'End If
                        'If thirdPartyReportLogRecord.name1Last <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name1Last", thirdPartyReportLogRecord.name1Last))
                        'End If
                        'If thirdPartyReportLogRecord.name1DOB <> "" AndAlso IsDate(thirdPartyReportLogRecord.name1DOB) = True Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name1DOB", CDate(thirdPartyReportLogRecord.name1DOB)))
                        'End If
                        'If thirdPartyReportLogRecord.name1SSN <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name1SSN", thirdPartyReportLogRecord.name1SSN))
                        'End If
                        'If thirdPartyReportLogRecord.name2First <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name2First", thirdPartyReportLogRecord.name2First))
                        'End If
                        'If thirdPartyReportLogRecord.name2Middle <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name2Middle", thirdPartyReportLogRecord.name2Middle))
                        'End If
                        'If thirdPartyReportLogRecord.name2Last <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name2Last", thirdPartyReportLogRecord.name2Last))
                        'End If
                        'If thirdPartyReportLogRecord.name2DOB <> "" AndAlso IsDate(thirdPartyReportLogRecord.name2DOB) = True Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name2DOB", CDate(thirdPartyReportLogRecord.name2DOB)))
                        'End If
                        'If thirdPartyReportLogRecord.name2SSN <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@name2SSN", thirdPartyReportLogRecord.name2SSN))
                        'End If
                        'If thirdPartyReportLogRecord.addressStreetNum <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@addressStreetNum", thirdPartyReportLogRecord.addressStreetNum))
                        'End If
                        'If thirdPartyReportLogRecord.addressStreetName <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@addressStreetName", thirdPartyReportLogRecord.addressStreetName))
                        'End If
                        'If thirdPartyReportLogRecord.addressApartmentNum <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@addressApartmentNum", thirdPartyReportLogRecord.addressApartmentNum))
                        'End If
                        'If thirdPartyReportLogRecord.addressCity <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@addressCity", thirdPartyReportLogRecord.addressCity))
                        'End If
                        'If thirdPartyReportLogRecord.addressState <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@addressState", thirdPartyReportLogRecord.addressState))
                        'End If
                        'If thirdPartyReportLogRecord.addressZip <> "" Then
                        '    sqlEO.inputParameters.Add(New SqlParameter("@addressZip", thirdPartyReportLogRecord.addressZip))
                        'End If
                        If thirdPartyReportLogRecord.quoteId > 0 Then 'new 12/19/2014
                            sqlEO.inputParameters.Add(New SqlParameter("@quoteId", thirdPartyReportLogRecord.quoteId))
                        End If
                        If thirdPartyReportLogRecord.agencyId > 0 Then 'new 12/18/2014
                            sqlEO.inputParameters.Add(New SqlParameter("@agencyId", thirdPartyReportLogRecord.agencyId))
                        End If
                        If thirdPartyReportLogRecord.agencyCode <> "" Then 'new 12/18/2014
                            sqlEO.inputParameters.Add(New SqlParameter("@agencyCode", thirdPartyReportLogRecord.agencyCode))
                        End If
                        If thirdPartyReportLogRecord.userId > 0 Then 'new 12/18/2014
                            sqlEO.inputParameters.Add(New SqlParameter("@userId", thirdPartyReportLogRecord.userId))
                        End If

                        sqlEO.outputParameter = New SqlParameter("@thirdPartyReportLogId", Data.SqlDbType.Int)

                        sqlEO.ExecuteStatement()

                        'If sqlEO.rowsAffected = 0 OrElse sqlEO.hasError = True Then
                        If sqlEO.hasError = True Then
                            'error
                            errorMsg = "error inserting third party report log record into database"
                            sqlErrorMsg = sqlEO.errorMsg
                        Else
                            thirdPartyReportLogRecord.thirdPartyReportLogId = sqlEO.outputParameter.Value
                            thirdPartyReportLogRecord.inserted = Date.Now.ToString
                        End If
                    End Using

                    'added 12/22/2014 to insert entities and links
                    If thirdPartyReportLogRecord.thirdPartyReportLogId > 0 Then
                        If thirdPartyReportLogRecord.thirdPartyReportEntities IsNot Nothing AndAlso thirdPartyReportLogRecord.thirdPartyReportEntities.Count > 0 Then
                            For Each e As QuickQuoteThirdPartyReportEntity In thirdPartyReportLogRecord.thirdPartyReportEntities
                                If e IsNot Nothing Then
                                    Dim entityInsertErrorMsg As String = ""
                                    Dim entityInsertSqlErrorMsg As String = ""
                                    InsertThirdPartyReportEntity(e, entityInsertErrorMsg, entityInsertSqlErrorMsg)
                                    If e.thirdPartyReportEntityId > 0 Then
                                        Dim linkInsertErrorMsg As String = ""
                                        Dim linkInsertSqlErrorMsg As String = ""
                                        Dim thirdPartyReportLogEntityLinkId As Integer = 0
                                        InsertThirdPartyReportLogEntityLink(thirdPartyReportLogRecord.thirdPartyReportLogId, e.thirdPartyReportEntityId, linkInsertErrorMsg, e.thirdPartyReportEntityTypeId, linkInsertSqlErrorMsg, thirdPartyReportLogEntityLinkId)
                                        If thirdPartyReportLogEntityLinkId > 0 Then
                                            'successful insert
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    End If
                Else
                    errorMsg = "please provide a valid thirdPartyReportTypeId, policyId, and policyImageNum"
                End If
            Else
                errorMsg = "please provide thirdPartyReportLogRecord"
            End If
        End Sub
        'Public Function GetThirdPartyReportLogRecord(ByVal thirdPartyReportTypeId As Integer, ByVal policyId As Integer, ByVal policyImageNum As Integer, ByVal unitNum As Integer, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "", Optional ByVal useUnitNum As Boolean = True) As QuickQuoteThirdPartyReportLogRecord
        'updated 12/18/2014 for new policyNumber param; may not use; 12/19/2014 added usePolicyNumber optional param and quoteId params
        Public Function GetThirdPartyReportLogRecord(ByVal thirdPartyReportTypeId As Integer, ByVal policyNumber As String, ByVal policyId As Integer, ByVal policyImageNum As Integer, ByVal unitNum As Integer, ByVal quoteId As Integer, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "", Optional ByVal useUnitNum As Boolean = True, Optional ByVal usePolicyNumber As Boolean = False, Optional useQuoteId As Boolean = False) As QuickQuoteThirdPartyReportLogRecord
            Dim tprlr As QuickQuoteThirdPartyReportLogRecord = Nothing
            errorMsg = ""
            sqlErrorMsg = ""

            'If thirdPartyReportTypeId > 0 AndAlso policyId > 0 Then '12/9/2014 - no longer validating policyImageNum
            'updated 12/18/2014 for policyNumber or policyId
            If thirdPartyReportTypeId > 0 AndAlso (policyNumber <> "" OrElse policyId > 0) Then
                Using sqlSO As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                    sqlSO.queryOrStoredProc = "usp_Get_ThirdPartyReportLog"
                    'finished code 12/8/2014
                    '@thirdPartyReportTypeId int = NULL,
                    '@policyId int,
                    '@policyImageNum int,
                    '@unitNum int = NULL,
                    '@justTop1 bit = 1
                    '12/18/2014 - changed db structure and params
                    '@thirdPartyReportTypeId int = NULL,
                    '@policyNumber varchar(12) = NULL,
                    '@policyId int = NULL,
                    '@policyImageNum int = NULL,
                    '@unitNum int = NULL,
                    '@quoteId int = NULL,
                    '@justTop1 bit = 1

                    sqlSO.parameters = New ArrayList
                    sqlSO.parameters.Add(New SqlParameter("@thirdPartyReportTypeId", thirdPartyReportTypeId))
                    'If policyNumber <> "" Then 'new 12/18/2014; updated 12/19/2014 to use new optional param instead of checking for non-empty string
                    If usePolicyNumber = True Then
                        sqlSO.parameters.Add(New SqlParameter("@policyNumber", policyNumber))
                    End If
                    If policyId > 0 Then 'optional as-of 12/18/2014
                        sqlSO.parameters.Add(New SqlParameter("@policyId", policyId))
                    End If
                    If policyImageNum > 0 Then '12/9/2014 - now optional
                        sqlSO.parameters.Add(New SqlParameter("@policyImageNum", policyImageNum))
                    End If
                    If useUnitNum = True Then
                        sqlSO.parameters.Add(New SqlParameter("@unitNum", unitNum))
                    End If
                    If useQuoteId = True Then 'added 12/19/2014
                        sqlSO.parameters.Add(New SqlParameter("@quoteId", quoteId))
                    End If

                    Dim dr As SqlDataReader = sqlSO.GetDataReader
                    If dr IsNot Nothing AndAlso dr.HasRows = True Then
                        dr.Read()
                        tprlr = New QuickQuoteThirdPartyReportLogRecord
                        With tprlr
                            .thirdPartyReportLogId = qqHelper.IntegerForString(dr.Item("thirdPartyReportLogId").ToString.Trim) '12/18/2014 note: could use IntegerForString function like is done in other spots (instead of assuming it's an int in db)... updated 12/18/2014 from dr.Item("thirdPartyReportLogId")
                            .thirdPartyReportTypeId = qqHelper.IntegerForString(dr.Item("thirdPartyReportTypeId").ToString.Trim) '12/18/2014 note: could use IntegerForString function like is done in other spots (instead of assuming it's an int in db)... updated 12/18/2014 from dr.Item("thirdPartyReportTypeId")
                            '.thirdPartyReportTypeDesc = dr.Item("thirdPartyReportTypeDesc").ToString.Trim
                            .policyNumber = dr.Item("policyNumber").ToString.Trim 'new 12/18/2014
                            .policyId = qqHelper.IntegerForString(dr.Item("policyId").ToString.Trim) '12/18/2014 note: could use IntegerForString function like is done in other spots (instead of assuming it's an int in db)... updated 12/18/2014 from dr.Item("policyId")
                            .policyImageNum = qqHelper.IntegerForString(dr.Item("policyImageNum").ToString.Trim) '12/18/2014 note: could use IntegerForString function like is done in other spots (instead of assuming it's an int in db)... updated 12/18/2014 from dr.Item("policyImageNum")
                            .unitNum = qqHelper.IntegerForString(dr.Item("unitNum").ToString.Trim) '12/18/2014 note: could use IntegerForString function like is done in other spots (instead of assuming it's an int in db)... updated 12/18/2014 from dr.Item("unitNum")
                            '.name1First = dr.Item("name1First").ToString.Trim
                            '.name1Middle = dr.Item("name1Middle").ToString.Trim
                            '.name1Last = dr.Item("name1Last").ToString.Trim
                            'If dr.Item("name1DOB").ToString.Trim <> "" AndAlso IsDate(dr.Item("name1DOB").ToString.Trim) = True Then
                            '    .name1DOB = dr.Item("name1DOB").ToString.Trim
                            'End If
                            '.name1SSN = dr.Item("name1SSN").ToString.Trim
                            '.name2First = dr.Item("name2First").ToString.Trim
                            '.name2Middle = dr.Item("name2Middle").ToString.Trim
                            '.name2Last = dr.Item("name2Last").ToString.Trim
                            'If dr.Item("name2DOB").ToString.Trim <> "" AndAlso IsDate(dr.Item("name2DOB").ToString.Trim) = True Then
                            '    .name2DOB = dr.Item("name2DOB").ToString.Trim
                            'End If
                            '.name2SSN = dr.Item("name2SSN").ToString.Trim
                            '.addressStreetNum = dr.Item("addressStreetNum").ToString.Trim
                            '.addressStreetName = dr.Item("addressStreetName").ToString.Trim
                            '.addressApartmentNum = dr.Item("addressApartmentNum").ToString.Trim
                            '.addressCity = dr.Item("addressCity").ToString.Trim
                            '.addressState = dr.Item("addressState").ToString.Trim
                            '.addressZip = dr.Item("addressZip").ToString.Trim
                            .quoteId = qqHelper.IntegerForString(dr.Item("quoteId").ToString.Trim) 'new 12/19/2014
                            .agencyId = qqHelper.IntegerForString(dr.Item("agencyId").ToString.Trim) 'new 12/18/2014; note: could use IntegerForString function like is done in other spots (instead of assuming it's an int in db)... updated 12/18/2014 from dr.Item("agencyId")
                            .agencyCode = dr.Item("agencyCode").ToString.Trim 'new 12/18/2014
                            .userId = qqHelper.IntegerForString(dr.Item("userId").ToString.Trim) 'new 12/18/2014; note: could use IntegerForString function like is done in other spots (instead of assuming it's an int in db)... updated 12/18/2014 from dr.Item("userId")
                            .inserted = dr.Item("inserted").ToString.Trim
                        End With

                    ElseIf sqlSO.hasError = True Then
                        errorMsg = "error getting third party report log record from database"
                        sqlErrorMsg = sqlSO.errorMsg
                    Else
                        errorMsg = "no third party report log record found"
                    End If
                End Using

                'added 12/23/2014; can also verify that tprlr IsNot Nothing, but link method will check too
                Dim linkErrorMsg As String = ""
                Dim linkSqlErrorMsg As String = ""
                GetThirdPartyReportLogEntityLinks(tprlr, linkErrorMsg, linkSqlErrorMsg)
            Else
                'errorMsg = "please provide a valid thirdPartyReportTypeId, policyId, and policyImageNum"
                'updated 12/18/2014
                errorMsg = "please provide a valid thirdPartyReportTypeId, along with policyNumber or policyId"
            End If

            Return tprlr
        End Function
        'added 12/8/2014
        Enum ThirdPartyReportType 'see ThirdPartyReportType table on QuickQuote database
            None = 0
            AutoPrefill = 1 'Auto Prefill in db
            CreditAuto = 2 'Credit Auto in db
            CreditProperty = 3 'Credit Property in db
            MVR = 4
            CLUEAuto = 5 'CLUE Auto in db
            CLUEProperty = 6 'CLUE Property in db
        End Enum
        Enum ThirdPartyReportTypeCategory 'added 4/8/2019; no table in QuickQuote database
            None = 0
            AutoPrefill = 1
            Credit = 2
            MVR = 3
            CLUE = 4
        End Enum
        'Public Function LoadThirdPartyReportLogRecordFromQuickQuoteObject(ByVal reportType As ThirdPartyReportType, ByVal qq As QuickQuoteObject, ByRef errorMsg As String, Optional ByVal qqDriver As QuickQuoteDriver = Nothing, Optional ByVal qqApplicant As QuickQuoteApplicant = Nothing, Optional ByVal activeNum As Integer = 0) As QuickQuoteThirdPartyReportLogRecord
        'updated for optional byref params
        'Public Function LoadThirdPartyReportLogRecordFromQuickQuoteObject(ByVal reportType As ThirdPartyReportType, ByVal qq As QuickQuoteObject, ByRef errorMsg As String, Optional ByVal qqDriver As QuickQuoteDriver = Nothing, Optional ByVal qqApplicant As QuickQuoteApplicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByRef policyId As Integer = 0, Optional ByRef policyImageNum As Integer = 0, Optional ByRef unitNum As Integer = 0) As QuickQuoteThirdPartyReportLogRecord
        'updated 12/19/2014 for policyNumber and quoteId optional params; also changed byref params to byval... to make sure nothing is changed from the calling side
        Public Function LoadThirdPartyReportLogRecordFromQuickQuoteObject(ByVal reportType As ThirdPartyReportType, ByVal qq As QuickQuoteObject, ByRef errorMsg As String, Optional ByVal qqDriver As QuickQuoteDriver = Nothing, Optional ByVal qqApplicant As QuickQuoteApplicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal unitNum As Integer = 0, Optional ByVal policyNumber As String = "", Optional ByVal quoteId As Integer = 0) As QuickQuoteThirdPartyReportLogRecord
            Dim tprlr As QuickQuoteThirdPartyReportLogRecord = Nothing
            errorMsg = ""

            If reportType <> Nothing AndAlso reportType <> ThirdPartyReportType.None Then
                If qq IsNot Nothing Then
                    ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord_QuickQuoteObject(qq, policyId, policyImageNum) 'added 12/9/2014
                    tprlr = New QuickQuoteThirdPartyReportLogRecord
                    tprlr.policyId = policyId 'added 12/9/2014
                    tprlr.policyImageNum = policyImageNum 'added 12/9/2014
                    tprlr.thirdPartyReportTypeId = CInt(reportType) 'added 12/9/2014
                    tprlr.agencyCode = qq.AgencyCode 'added 12/22/2014
                    If qq.AgencyId <> "" AndAlso IsNumeric(qq.AgencyId) = True Then 'added 12/22/2014
                        tprlr.agencyId = CInt(qq.AgencyId)
                    End If
                    tprlr.policyNumber = policyNumber 'added 12/23/2014
                    tprlr.quoteId = quoteId 'added 12/23/2014
                    tprlr.userId = qqHelper.IntegerForString(helper.DiamondUserId) 'added 12/23/2014
                    Select Case reportType
                        Case ThirdPartyReportType.AutoPrefill 'policy
                            'use client name/address
                            'If qq.Client IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName1(qq.Client.Name, tprlr)
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteAddress(qq.Client.Address, tprlr)
                            'ElseIf qq.Policyholder IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName1(qq.Policyholder.Name, tprlr)
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteAddress(qq.Policyholder.Address, tprlr)
                            'End If
                            'updated 12/19/2014
                            Dim e As New QuickQuoteThirdPartyReportEntity
                            e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Client)
                            e.nameSaved = True
                            e.addressSaved = True
                            If qq.Client IsNot Nothing Then
                                ConvertQuickQuoteNameToThirdPartyReportName(qq.Client.Name, e.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(qq.Client.Address, e.thirdPartyReportAddress)
                            ElseIf qq.Policyholder IsNot Nothing Then 'may not use... or may save both client and ph1
                                ConvertQuickQuoteNameToThirdPartyReportName(qq.Policyholder.Name, e.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(qq.Policyholder.Address, e.thirdPartyReportAddress)
                                e.isPolicyholder1 = True 'added 12/23/2014
                            End If
                            If tprlr.thirdPartyReportEntities Is Nothing Then
                                tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e)
                        Case ThirdPartyReportType.CreditAuto 'person; ph relationshipTypeId (8 or 5)
                            'use ph drivers
                            'Dim ph1Driver As QuickQuoteDriver = Nothing
                            'Dim ph2Driver As QuickQuoteDriver = Nothing
                            'Dim loadErrorMsg As String = ""
                            'LoadPolicyholderDriversFromQuickQuoteObject(qq, ph1Driver, ph2Driver, loadErrorMsg)
                            'If ph1Driver IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName1(ph1Driver.Name, tprlr)
                            'End If
                            'If ph2Driver IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName2(ph2Driver.Name, tprlr)
                            'End If

                            If qqDriver IsNot Nothing Then

                            ElseIf activeNum > 0 Then
                                qqDriver = qqHelper.QuickQuoteDriverForActiveNum(qq, activeNum)
                            ElseIf unitNum > 0 Then 'added 12/9/2014
                                qqDriver = qqHelper.QuickQuoteDriverForIdValueNum(qq, unitNum)
                            End If
                            If qqDriver IsNot Nothing Then 'added 12/9/2014
                                If qqDriver.HasValidDriverNum = True Then
                                    tprlr.unitNum = CInt(qqDriver.DriverNum)
                                ElseIf unitNum > 0 Then
                                    tprlr.unitNum = unitNum
                                End If

                                'added 12/11/2014; removed 12/19/2014... may eventually use; started using 12/23/2014
                                Dim isPolicyholder1 As Boolean = False
                                Dim isPolicyholder2 As Boolean = False
                                Dim isPolicyholder As Boolean = qqHelper.IsQuickQuoteDriverPolicyholder(qqDriver, isPolicyholder1, isPolicyholder2)

                                'added 12/10/2014
                                'LoadThirdPartyReportLogRecordFromQuickQuoteName1(qqDriver.Name, tprlr)
                                'LoadThirdPartyReportLogRecordFromQuickQuoteAddress(qqDriver.Address, tprlr)
                                'updated 12/19/2014
                                Dim e As New QuickQuoteThirdPartyReportEntity
                                e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Driver) 'may need to use ph driver type, but would need to check driver relationship to tell which one
                                e.nameSaved = True
                                e.addressSaved = True
                                e.isPolicyholder1 = isPolicyholder1 'added 12/23/2014
                                e.isPolicyholder2 = isPolicyholder2 'added 12/23/2014
                                ConvertQuickQuoteNameToThirdPartyReportName(qqDriver.Name, e.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(qqDriver.Address, e.thirdPartyReportAddress)
                                If tprlr.thirdPartyReportEntities Is Nothing Then
                                    tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                                End If
                                tprlr.thirdPartyReportEntities.Add(e)
                            End If
                        Case ThirdPartyReportType.CreditProperty 'person; ph relationshipTypeId (8 or 5)
                            'use ph applicants
                            'Dim ph1Applicant As QuickQuoteApplicant = Nothing
                            'Dim ph2Applicant As QuickQuoteApplicant = Nothing
                            'Dim loadErrorMsg As String = ""
                            'LoadPolicyholderApplicantsFromQuickQuoteObject(qq, ph1Applicant, ph2Applicant, loadErrorMsg)
                            'If ph1Applicant IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName1(ph1Applicant.Name, tprlr)
                            'End If
                            'If ph2Applicant IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName2(ph2Applicant.Name, tprlr)
                            'End If

                            If qqApplicant IsNot Nothing Then

                            ElseIf activeNum > 0 Then
                                qqApplicant = qqHelper.QuickQuoteApplicantForActiveNum(qq, activeNum)
                            ElseIf unitNum > 0 Then 'added 12/9/2014
                                qqApplicant = qqHelper.QuickQuoteApplicantForIdValueNum(qq, unitNum)
                            End If
                            If qqApplicant IsNot Nothing Then 'added 12/9/2014
                                If qqApplicant.HasValidApplicantNum = True Then
                                    tprlr.unitNum = CInt(qqApplicant.ApplicantNum)
                                ElseIf unitNum > 0 Then
                                    tprlr.unitNum = unitNum
                                End If

                                'added 12/11/2014; removed 12/19/2014... may eventually use; started using 12/23/2014
                                Dim isPolicyholder1 As Boolean = False
                                Dim isPolicyholder2 As Boolean = False
                                Dim isPolicyholder As Boolean = qqHelper.IsQuickQuoteApplicantPolicyholder(qqApplicant, isPolicyholder1, isPolicyholder2)

                                'added 12/10/2014
                                'LoadThirdPartyReportLogRecordFromQuickQuoteName1(qqApplicant.Name, tprlr)
                                'LoadThirdPartyReportLogRecordFromQuickQuoteAddress(qqApplicant.Address, tprlr)
                                'updated 12/19/2014
                                Dim e As New QuickQuoteThirdPartyReportEntity
                                e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Applicant) 'may need to use ph applicant type, but would need to check applicant relationship to tell which one
                                e.nameSaved = True
                                e.addressSaved = True
                                e.isPolicyholder1 = isPolicyholder1 'added 12/23/2014
                                e.isPolicyholder2 = isPolicyholder2 'added 12/23/2014
                                ConvertQuickQuoteNameToThirdPartyReportName(qqApplicant.Name, e.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(qqApplicant.Address, e.thirdPartyReportAddress)
                                If tprlr.thirdPartyReportEntities Is Nothing Then
                                    tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                                End If
                                tprlr.thirdPartyReportEntities.Add(e)
                            End If
                        Case ThirdPartyReportType.MVR 'person; rated driverExcludeTypeId (1)
                            'use drivers
                            If qqDriver IsNot Nothing Then

                            ElseIf activeNum > 0 Then
                                qqDriver = qqHelper.QuickQuoteDriverForActiveNum(qq, activeNum)
                            ElseIf unitNum > 0 Then 'added 12/9/2014
                                qqDriver = qqHelper.QuickQuoteDriverForIdValueNum(qq, unitNum)
                            End If
                            If qqDriver IsNot Nothing Then 'added 12/9/2014
                                If qqDriver.HasValidDriverNum = True Then
                                    tprlr.unitNum = CInt(qqDriver.DriverNum)
                                ElseIf unitNum > 0 Then
                                    tprlr.unitNum = unitNum
                                End If

                                'added 12/11/2014; removed 12/19/2014... may eventually use; started using 12/23/2014
                                Dim isPolicyholder1 As Boolean = False
                                Dim isPolicyholder2 As Boolean = False
                                Dim isPolicyholder As Boolean = qqHelper.IsQuickQuoteDriverPolicyholder(qqDriver, isPolicyholder1, isPolicyholder2)

                                'added 12/10/2014
                                'LoadThirdPartyReportLogRecordFromQuickQuoteName1(qqDriver.Name, tprlr)
                                'LoadThirdPartyReportLogRecordFromQuickQuoteAddress(qqDriver.Address, tprlr)
                                'updated 12/19/2014
                                Dim e As New QuickQuoteThirdPartyReportEntity
                                e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Driver) 'may need to use ph driver type, but would need to check driver relationship to tell which one
                                e.nameSaved = True
                                e.addressSaved = True
                                e.isPolicyholder1 = isPolicyholder1 'added 12/23/2014
                                e.isPolicyholder2 = isPolicyholder2 'added 12/23/2014
                                ConvertQuickQuoteNameToThirdPartyReportName(qqDriver.Name, e.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(qqDriver.Address, e.thirdPartyReportAddress)
                                If tprlr.thirdPartyReportEntities Is Nothing Then
                                    tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                                End If
                                tprlr.thirdPartyReportEntities.Add(e)
                            End If
                        Case ThirdPartyReportType.CLUEAuto 'policy
                            'use ph drivers; may need to use actual ph address instead of address on ph driver
                            Dim ph1Driver As QuickQuoteDriver = Nothing
                            Dim ph2Driver As QuickQuoteDriver = Nothing
                            Dim loadErrorMsg As String = ""
                            qqHelper.LoadPolicyholderDriversFromQuickQuoteObject(qq, ph1Driver, ph2Driver, loadErrorMsg)
                            'If ph1Driver IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName1(ph1Driver.Name, tprlr)
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteAddress(ph1Driver.Address, tprlr)
                            'End If
                            'If ph2Driver IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName2(ph2Driver.Name, tprlr)
                            'End If
                            'updated 12/19/2014
                            Dim e1 As New QuickQuoteThirdPartyReportEntity
                            e1.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder1Driver)
                            e1.nameSaved = True
                            e1.addressSaved = True
                            e1.isPolicyholder1 = True 'added 12/23/2014
                            If ph1Driver IsNot Nothing Then
                                ConvertQuickQuoteNameToThirdPartyReportName(ph1Driver.Name, e1.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(ph1Driver.Address, e1.thirdPartyReportAddress)
                            End If
                            If tprlr.thirdPartyReportEntities Is Nothing Then
                                tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e1)

                            Dim e2 As New QuickQuoteThirdPartyReportEntity
                            e2.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder2Driver)
                            e2.nameSaved = True
                            e2.addressSaved = True
                            e2.isPolicyholder2 = True 'added 12/23/2014
                            If ph2Driver IsNot Nothing Then
                                ConvertQuickQuoteNameToThirdPartyReportName(ph2Driver.Name, e2.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(ph2Driver.Address, e2.thirdPartyReportAddress)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e2)
                        Case ThirdPartyReportType.CLUEProperty 'policy
                            'use ph applicants and loc address
                            '12/11/2014 note: currently need to use the following entities to match ThirdPartyReportEntityValidationTypeCombos_CLUEProperty validation: Policyholder1 (for name fields), Policyholder2 (for name fields), and Location (for address fields); 12/18/2014 - added new entity types for ph applicants and drivers
                            Dim ph1Applicant As QuickQuoteApplicant = Nothing
                            Dim ph2Applicant As QuickQuoteApplicant = Nothing
                            Dim loadErrorMsg As String = ""
                            qqHelper.LoadPolicyholderApplicantsFromQuickQuoteObject(qq, ph1Applicant, ph2Applicant, loadErrorMsg)
                            'If ph1Applicant IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName1(ph1Applicant.Name, tprlr)
                            'End If
                            'If ph2Applicant IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteName2(ph2Applicant.Name, tprlr)
                            'End If
                            'If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                            '    LoadThirdPartyReportLogRecordFromQuickQuoteAddress(qq.Locations(0).Address, tprlr)
                            'End If
                            'updated 12/19/2014
                            Dim e1 As New QuickQuoteThirdPartyReportEntity
                            e1.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder1Applicant)
                            e1.nameSaved = True
                            e1.addressSaved = True
                            e1.isPolicyholder1 = True 'added 12/23/2014
                            If ph1Applicant IsNot Nothing Then
                                ConvertQuickQuoteNameToThirdPartyReportName(ph1Applicant.Name, e1.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(ph1Applicant.Address, e1.thirdPartyReportAddress)
                            End If
                            If tprlr.thirdPartyReportEntities Is Nothing Then
                                tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e1)

                            Dim e2 As New QuickQuoteThirdPartyReportEntity
                            e2.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder2Applicant)
                            e2.nameSaved = True
                            e2.addressSaved = True
                            e2.isPolicyholder2 = True 'added 12/23/2014
                            If ph2Applicant IsNot Nothing Then
                                ConvertQuickQuoteNameToThirdPartyReportName(ph2Applicant.Name, e2.thirdPartyReportName)
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(ph2Applicant.Address, e2.thirdPartyReportAddress)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e2)

                            Dim e3 As New QuickQuoteThirdPartyReportEntity
                            e3.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Location)
                            e3.nameSaved = False
                            e3.addressSaved = True
                            'If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                            '    ConvertQuickQuoteAddressToThirdPartyReportAddress(qq.Locations(0).Address, e3.thirdPartyReportAddress)
                            'End If
                            'updated 9/17/2018
                            Dim qqLocations As List(Of QuickQuoteLocation) = qqHelper.AllQuickQuoteLocations(qq, level:=helper.MultiStateLevel.AllStates)
                            If qqLocations IsNot Nothing AndAlso qqLocations.Count > 0 Then
                                ConvertQuickQuoteAddressToThirdPartyReportAddress(qqLocations(0).Address, e3.thirdPartyReportAddress)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e3)
                    End Select
                Else
                    errorMsg = "please provide a valid QuickQuoteObject"
                End If
            Else
                errorMsg = "please provide a valid ThirdPartyReportType"
            End If

            Return tprlr
        End Function
        'Public Function LoadThirdPartyReportLogRecordFromDiamondImage(ByVal reportType As ThirdPartyReportType, ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef errorMsg As String, Optional ByVal diaDriver As Diamond.Common.Objects.Policy.Driver = Nothing, Optional ByVal diaApplicant As Diamond.Common.Objects.Policy.Applicant = Nothing, Optional ByVal activeNum As Integer = 0) As QuickQuoteThirdPartyReportLogRecord
        'updated for optional byref params
        'Public Function LoadThirdPartyReportLogRecordFromDiamondImage(ByVal reportType As ThirdPartyReportType, ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef errorMsg As String, Optional ByVal diaDriver As Diamond.Common.Objects.Policy.Driver = Nothing, Optional ByVal diaApplicant As Diamond.Common.Objects.Policy.Applicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByRef policyId As Integer = 0, Optional ByRef policyImageNum As Integer = 0, Optional ByRef unitNum As Integer = 0) As QuickQuoteThirdPartyReportLogRecord
        'updated 12/19/2014 for policyNumber and quoteId optional params; also changed byref params to byval... to make sure nothing is changed from the calling side
        Public Function LoadThirdPartyReportLogRecordFromDiamondImage(ByVal reportType As ThirdPartyReportType, ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef errorMsg As String, Optional ByVal diaDriver As Diamond.Common.Objects.Policy.Driver = Nothing, Optional ByVal diaApplicant As Diamond.Common.Objects.Policy.Applicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal unitNum As Integer = 0, Optional ByVal policyNumber As String = "", Optional ByVal quoteId As Integer = 0) As QuickQuoteThirdPartyReportLogRecord
            Dim tprlr As QuickQuoteThirdPartyReportLogRecord = Nothing
            errorMsg = ""

            If reportType <> Nothing AndAlso reportType <> ThirdPartyReportType.None Then
                If diaImage IsNot Nothing Then
                    ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord_DiamondImage(diaImage, policyId, policyImageNum) 'added 12/9/2014
                    tprlr = New QuickQuoteThirdPartyReportLogRecord
                    tprlr.policyId = policyId 'added 12/9/2014
                    tprlr.policyImageNum = policyImageNum 'added 12/9/2014
                    tprlr.thirdPartyReportTypeId = CInt(reportType) 'added 12/9/2014
                    If diaImage.Agency IsNot Nothing Then 'added 12/22/2014
                        tprlr.agencyCode = diaImage.Agency.Code
                    End If
                    tprlr.agencyId = diaImage.AgencyId 'added 12/22/2014
                    tprlr.policyNumber = policyNumber 'added 12/23/2014
                    tprlr.quoteId = quoteId 'added 12/23/2014
                    tprlr.userId = qqHelper.IntegerForString(helper.DiamondUserId) 'added 12/23/2014
                    Select Case reportType
                        Case ThirdPartyReportType.AutoPrefill 'policy
                            'use client name/address
                            'If diaImage.Policy IsNot Nothing AndAlso diaImage.Policy.Client IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromDiamondImageName1(diaImage.Policy.Client.Name, tprlr)
                            '    LoadThirdPartyReportLogRecordFromDiamondImageAddress(diaImage.Policy.Client.Address, tprlr)
                            'ElseIf diaImage.PolicyHolder IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromDiamondImageName1(diaImage.PolicyHolder.Name, tprlr)
                            '    LoadThirdPartyReportLogRecordFromDiamondImageAddress(diaImage.PolicyHolder.Address, tprlr)
                            'End If
                            'updated 12/19/2014
                            Dim e As New QuickQuoteThirdPartyReportEntity
                            e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Client)
                            e.nameSaved = True
                            e.addressSaved = True
                            If diaImage.Policy IsNot Nothing AndAlso diaImage.Policy.Client IsNot Nothing Then
                                ConvertDiamondImageNameToThirdPartyReportName(diaImage.Policy.Client.Name, e.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(diaImage.Policy.Client.Address, e.thirdPartyReportAddress)
                            ElseIf diaImage.PolicyHolder IsNot Nothing Then 'may not use... or may save both client and ph1
                                ConvertDiamondImageNameToThirdPartyReportName(diaImage.PolicyHolder.Name, e.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(diaImage.PolicyHolder.Address, e.thirdPartyReportAddress)
                                e.isPolicyholder1 = True 'added 12/23/2014
                            End If
                            If tprlr.thirdPartyReportEntities Is Nothing Then
                                tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e)
                        Case ThirdPartyReportType.CreditAuto 'person; ph relationshipTypeId (8 or 5)
                            'use ph drivers
                            If diaDriver IsNot Nothing Then

                            ElseIf activeNum > 0 Then
                                diaDriver = qqHelper.DiamondDriverForActiveNum(diaImage, activeNum)
                            ElseIf unitNum > 0 Then 'added 12/9/2014
                                diaDriver = qqHelper.DiamondDriverForIdValueNum(diaImage, unitNum)
                            End If
                            If diaDriver IsNot Nothing Then 'added 12/9/2014
                                If helper.IsValidDiamondNum(diaDriver.DriverNum) = True Then
                                    tprlr.unitNum = diaDriver.DriverNum.Id
                                ElseIf unitNum > 0 Then
                                    tprlr.unitNum = unitNum
                                End If

                                'added 12/11/2014; removed 12/19/2014... may eventually use; started using 12/23/2014
                                Dim isPolicyholder1 As Boolean = False
                                Dim isPolicyholder2 As Boolean = False
                                Dim isPolicyholder As Boolean = qqHelper.IsDiamondDriverPolicyholder(diaDriver, isPolicyholder1, isPolicyholder2)

                                'added 12/10/2014
                                'LoadThirdPartyReportLogRecordFromDiamondImageName1(diaDriver.Name, tprlr)
                                'LoadThirdPartyReportLogRecordFromDiamondImageAddress(diaDriver.Address, tprlr)
                                'updated 12/19/2014
                                Dim e As New QuickQuoteThirdPartyReportEntity
                                e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Driver) 'may need to use ph driver type, but would need to check driver relationship to tell which one
                                e.nameSaved = True
                                e.addressSaved = True
                                e.isPolicyholder1 = isPolicyholder1 'added 12/23/2014
                                e.isPolicyholder2 = isPolicyholder2 'added 12/23/2014
                                ConvertDiamondImageNameToThirdPartyReportName(diaDriver.Name, e.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(diaDriver.Address, e.thirdPartyReportAddress)
                                If tprlr.thirdPartyReportEntities Is Nothing Then
                                    tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                                End If
                                tprlr.thirdPartyReportEntities.Add(e)
                            End If
                        Case ThirdPartyReportType.CreditProperty 'person; ph relationshipTypeId (8 or 5)
                            'use ph applicants
                            If diaApplicant IsNot Nothing Then

                            ElseIf activeNum > 0 Then
                                diaApplicant = qqHelper.DiamondApplicantForActiveNum(diaImage, activeNum)
                            ElseIf unitNum > 0 Then 'added 12/9/2014
                                diaApplicant = qqHelper.DiamondApplicantForIdValueNum(diaImage, unitNum)
                            End If
                            If diaApplicant IsNot Nothing Then 'added 12/9/2014
                                If helper.IsValidDiamondNum(diaApplicant.ApplicantNum) = True Then
                                    tprlr.unitNum = diaApplicant.ApplicantNum.Id
                                ElseIf unitNum > 0 Then
                                    tprlr.unitNum = unitNum
                                End If

                                'added 12/11/2014; removed 12/19/2014... may eventually use; started using 12/23/2014
                                Dim isPolicyholder1 As Boolean = False
                                Dim isPolicyholder2 As Boolean = False
                                Dim isPolicyholder As Boolean = qqHelper.IsDiamondApplicantPolicyholder(diaApplicant, isPolicyholder1, isPolicyholder2)

                                'added 12/10/2014
                                'LoadThirdPartyReportLogRecordFromDiamondImageName1(diaApplicant.Name, tprlr)
                                'LoadThirdPartyReportLogRecordFromDiamondImageAddress(diaApplicant.Address, tprlr)
                                'updated 12/19/2014
                                Dim e As New QuickQuoteThirdPartyReportEntity
                                e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Applicant) 'may need to use ph applicant type, but would need to check applicant relationship to tell which one
                                e.nameSaved = True
                                e.addressSaved = True
                                e.isPolicyholder1 = isPolicyholder1 'added 12/23/2014
                                e.isPolicyholder2 = isPolicyholder2 'added 12/23/2014
                                ConvertDiamondImageNameToThirdPartyReportName(diaApplicant.Name, e.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(diaApplicant.Address, e.thirdPartyReportAddress)
                                If tprlr.thirdPartyReportEntities Is Nothing Then
                                    tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                                End If
                                tprlr.thirdPartyReportEntities.Add(e)
                            End If
                        Case ThirdPartyReportType.MVR 'person; rated driverExcludeTypeId (1)
                            'use drivers
                            If diaDriver IsNot Nothing Then

                            ElseIf activeNum > 0 Then
                                diaDriver = qqHelper.DiamondDriverForActiveNum(diaImage, activeNum)
                            ElseIf unitNum > 0 Then 'added 12/9/2014
                                diaDriver = qqHelper.DiamondDriverForIdValueNum(diaImage, unitNum)
                            End If
                            If diaDriver IsNot Nothing Then 'added 12/9/2014
                                If helper.IsValidDiamondNum(diaDriver.DriverNum) = True Then
                                    tprlr.unitNum = diaDriver.DriverNum.Id
                                ElseIf unitNum > 0 Then
                                    tprlr.unitNum = unitNum
                                End If

                                'added 12/11/2014; removed 12/19/2014... may eventually use; started using 12/23/2014
                                Dim isPolicyholder1 As Boolean = False
                                Dim isPolicyholder2 As Boolean = False
                                Dim isPolicyholder As Boolean = qqHelper.IsDiamondDriverPolicyholder(diaDriver, isPolicyholder1, isPolicyholder2)

                                'added 12/10/2014
                                'LoadThirdPartyReportLogRecordFromDiamondImageName1(diaDriver.Name, tprlr)
                                'LoadThirdPartyReportLogRecordFromDiamondImageAddress(diaDriver.Address, tprlr)
                                'updated 12/19/2014
                                Dim e As New QuickQuoteThirdPartyReportEntity
                                e.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Driver) 'may need to use ph driver type, but would need to check driver relationship to tell which one
                                e.nameSaved = True
                                e.addressSaved = True
                                e.isPolicyholder1 = isPolicyholder1 'added 12/23/2014
                                e.isPolicyholder2 = isPolicyholder2 'added 12/23/2014
                                ConvertDiamondImageNameToThirdPartyReportName(diaDriver.Name, e.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(diaDriver.Address, e.thirdPartyReportAddress)
                                If tprlr.thirdPartyReportEntities Is Nothing Then
                                    tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                                End If
                                tprlr.thirdPartyReportEntities.Add(e)
                            End If
                        Case ThirdPartyReportType.CLUEAuto 'policy
                            'use ph drivers; may need to use actual ph address instead of address on ph driver
                            Dim ph1Driver As Diamond.Common.Objects.Policy.Driver = Nothing
                            Dim ph2Driver As Diamond.Common.Objects.Policy.Driver = Nothing
                            Dim loadErrorMsg As String = ""
                            qqHelper.LoadPolicyholderDriversFromDiamondImage(diaImage, ph1Driver, ph2Driver, loadErrorMsg)
                            'If ph1Driver IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromDiamondImageName1(ph1Driver.Name, tprlr)
                            '    LoadThirdPartyReportLogRecordFromDiamondImageAddress(ph1Driver.Address, tprlr)
                            'End If
                            'If ph2Driver IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromDiamondImageName2(ph2Driver.Name, tprlr)
                            'End If
                            'updated 12/19/2014
                            Dim e1 As New QuickQuoteThirdPartyReportEntity
                            e1.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder1Driver)
                            e1.nameSaved = True
                            e1.addressSaved = True
                            e1.isPolicyholder1 = True 'added 12/23/2014
                            If ph1Driver IsNot Nothing Then
                                ConvertDiamondImageNameToThirdPartyReportName(ph1Driver.Name, e1.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(ph1Driver.Address, e1.thirdPartyReportAddress)
                            End If
                            If tprlr.thirdPartyReportEntities Is Nothing Then
                                tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e1)

                            Dim e2 As New QuickQuoteThirdPartyReportEntity
                            e2.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder2Driver)
                            e2.nameSaved = True
                            e2.addressSaved = True
                            e2.isPolicyholder2 = True 'added 12/23/2014
                            If ph2Driver IsNot Nothing Then
                                ConvertDiamondImageNameToThirdPartyReportName(ph2Driver.Name, e2.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(ph2Driver.Address, e2.thirdPartyReportAddress)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e2)
                        Case ThirdPartyReportType.CLUEProperty 'policy
                            'use ph applicants and loc address
                            '12/11/2014 note: currently need to use the following entities to match ThirdPartyReportEntityValidationTypeCombos_CLUEProperty validation: Policyholder1 (for name fields), Policyholder2 (for name fields), and Location (for address fields)
                            Dim ph1Applicant As Diamond.Common.Objects.Policy.Applicant = Nothing
                            Dim ph2Applicant As Diamond.Common.Objects.Policy.Applicant = Nothing
                            Dim loadErrorMsg As String = ""
                            qqHelper.LoadPolicyholderApplicantsFromDiamondImage(diaImage, ph1Applicant, ph2Applicant, loadErrorMsg)
                            'If ph1Applicant IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromDiamondImageName1(ph1Applicant.Name, tprlr)
                            'End If
                            'If ph2Applicant IsNot Nothing Then
                            '    LoadThirdPartyReportLogRecordFromDiamondImageName2(ph2Applicant.Name, tprlr)
                            'End If
                            'Dim rl As Diamond.Common.Objects.Policy.RiskLevel = qqHelper.DiamondRiskLevelForImage(diaImage)
                            'If rl IsNot Nothing Then
                            '    If rl.Locations IsNot Nothing AndAlso rl.Locations.Count > 0 Then
                            '        LoadThirdPartyReportLogRecordFromDiamondImageAddress(rl.Locations(0).Address, tprlr)
                            '    End If
                            'End If
                            'updated 12/19/2014
                            Dim e1 As New QuickQuoteThirdPartyReportEntity
                            e1.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder1Applicant)
                            e1.nameSaved = True
                            e1.addressSaved = True
                            e1.isPolicyholder1 = True 'added 12/23/2014
                            If ph1Applicant IsNot Nothing Then
                                ConvertDiamondImageNameToThirdPartyReportName(ph1Applicant.Name, e1.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(ph1Applicant.Address, e1.thirdPartyReportAddress)
                            End If
                            If tprlr.thirdPartyReportEntities Is Nothing Then
                                tprlr.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e1)

                            Dim e2 As New QuickQuoteThirdPartyReportEntity
                            e2.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Policyholder2Applicant)
                            e2.nameSaved = True
                            e2.addressSaved = True
                            e2.isPolicyholder2 = True 'added 12/23/2014
                            If ph2Applicant IsNot Nothing Then
                                ConvertDiamondImageNameToThirdPartyReportName(ph2Applicant.Name, e2.thirdPartyReportName)
                                ConvertDiamondImageAddressToThirdPartyReportAddress(ph2Applicant.Address, e2.thirdPartyReportAddress)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e2)

                            Dim e3 As New QuickQuoteThirdPartyReportEntity
                            e3.thirdPartyReportEntityTypeId = CInt(ThirdPartyReportEntityType.Location)
                            e3.nameSaved = False
                            e3.addressSaved = True
                            'Dim rl As Diamond.Common.Objects.Policy.RiskLevel = qqHelper.DiamondRiskLevelForImage(diaImage)
                            'If rl IsNot Nothing Then
                            '    If rl.Locations IsNot Nothing AndAlso rl.Locations.Count > 0 Then
                            '        ConvertDiamondImageAddressToThirdPartyReportAddress(rl.Locations(0).Address, e3.thirdPartyReportAddress)
                            '    End If
                            'End If
                            'updated 9/17/2018
                            Dim diaLocations As List(Of Diamond.Common.Objects.Policy.Location) = qqHelper.AllDiamondLocations(diaImage, level:=helper.MultiStateLevel.AllStates)
                            If diaLocations IsNot Nothing AndAlso diaLocations.Count > 0 Then
                                ConvertDiamondImageAddressToThirdPartyReportAddress(diaLocations(0).Address, e3.thirdPartyReportAddress)
                            End If
                            tprlr.thirdPartyReportEntities.Add(e3)
                    End Select
                Else
                    errorMsg = "please provide a valid Diamond image"
                End If
            Else
                errorMsg = "please provide a valid ThirdPartyReportType"
            End If

            Return tprlr
        End Function
        '12/19/2014 - removed methods no longer needed or replaced w/ new logic and db structure
        'Public Sub LoadThirdPartyReportLogRecordFromQuickQuoteName1(ByVal qqName As QuickQuoteName, ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord)
        '    If qqName IsNot Nothing Then
        '        If thirdPartyReportLogRecord Is Nothing Then
        '            thirdPartyReportLogRecord = New QuickQuoteThirdPartyReportLogRecord
        '        End If
        '        With thirdPartyReportLogRecord
        '            SetNameFieldsForQuickQuoteThirdPartyReportLogRecord(qqName, .name1First, .name1Middle, .name1Last, .name1DOB, .name1SSN)
        '        End With
        '    End If
        'End Sub
        'Public Sub LoadThirdPartyReportLogRecordFromQuickQuoteName2(ByVal qqName As QuickQuoteName, ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord)
        '    If qqName IsNot Nothing Then
        '        If thirdPartyReportLogRecord Is Nothing Then
        '            thirdPartyReportLogRecord = New QuickQuoteThirdPartyReportLogRecord
        '        End If
        '        With thirdPartyReportLogRecord
        '            SetNameFieldsForQuickQuoteThirdPartyReportLogRecord(qqName, .name2First, .name2Middle, .name2Last, .name2DOB, .name2SSN)
        '        End With
        '    End If
        'End Sub
        'Public Sub LoadThirdPartyReportLogRecordFromQuickQuoteAddress(ByVal qqAddress As QuickQuoteAddress, ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord)
        '    If qqAddress IsNot Nothing Then
        '        If thirdPartyReportLogRecord Is Nothing Then
        '            thirdPartyReportLogRecord = New QuickQuoteThirdPartyReportLogRecord
        '        End If
        '        With thirdPartyReportLogRecord
        '            '.addressStreetNum = qqAddress.HouseNum
        '            '.addressStreetName = qqAddress.StreetName
        '            '.addressApartmentNum = qqAddress.ApartmentNumber
        '            '.addressCity = qqAddress.City
        '            '.addressState = qqAddress.State
        '            '.addressZip = qqAddress.Zip
        '            SetAddressFieldsForQuickQuoteThirdPartyReportLogRecord(qqAddress, .addressStreetNum, .addressStreetName, .addressApartmentNum, .addressCity, .addressState, .addressZip)
        '        End With
        '    End If
        'End Sub
        'Public Sub LoadThirdPartyReportLogRecordFromDiamondImageName1(ByVal diaName As Diamond.Common.Objects.Name, ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord)
        '    If diaName IsNot Nothing Then
        '        If thirdPartyReportLogRecord Is Nothing Then
        '            thirdPartyReportLogRecord = New QuickQuoteThirdPartyReportLogRecord
        '        End If
        '        With thirdPartyReportLogRecord
        '            SetNameFieldsForDiamondImageThirdPartyReportLogRecord(diaName, .name1First, .name1Middle, .name1Last, .name1DOB, .name1SSN)
        '        End With
        '    End If
        'End Sub
        'Public Sub LoadThirdPartyReportLogRecordFromDiamondImageName2(ByVal diaName As Diamond.Common.Objects.Name, ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord)
        '    If diaName IsNot Nothing Then
        '        If thirdPartyReportLogRecord Is Nothing Then
        '            thirdPartyReportLogRecord = New QuickQuoteThirdPartyReportLogRecord
        '        End If
        '        With thirdPartyReportLogRecord
        '            SetNameFieldsForDiamondImageThirdPartyReportLogRecord(diaName, .name2First, .name2Middle, .name2Last, .name2DOB, .name2SSN)
        '        End With
        '    End If
        'End Sub
        'Public Sub LoadThirdPartyReportLogRecordFromDiamondImageAddress(ByVal diaAddress As Diamond.Common.Objects.Address, ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord)
        '    If diaAddress IsNot Nothing Then
        '        If thirdPartyReportLogRecord Is Nothing Then
        '            thirdPartyReportLogRecord = New QuickQuoteThirdPartyReportLogRecord
        '        End If
        '        With thirdPartyReportLogRecord
        '            SetAddressFieldsForDiamondImageThirdPartyReportLogRecord(diaAddress, .addressStreetNum, .addressStreetName, .addressApartmentNum, .addressCity, .addressState, .addressZip)
        '        End With
        '    End If
        'End Sub
        'Public Sub SetNameFieldsForQuickQuoteThirdPartyReportLogRecord(ByVal qqName As QuickQuoteName, ByRef first As String, ByRef middle As String, ByRef last As String, ByRef dob As String, ByRef ssn As String)
        '    first = ""
        '    middle = ""
        '    last = ""
        '    dob = ""
        '    ssn = ""

        '    If qqName IsNot Nothing Then
        '        With qqName
        '            first = .FirstName
        '            middle = .MiddleName
        '            last = .LastName
        '            dob = .BirthDate
        '            ssn = .TaxNumber
        '        End With
        '    End If
        'End Sub
        'Public Sub SetAddressFieldsForQuickQuoteThirdPartyReportLogRecord(ByVal qqAddress As QuickQuoteAddress, ByRef streetNum As String, ByRef streetName As String, ByRef aptNum As String, ByRef city As String, ByRef state As String, ByRef zip As String)
        '    streetNum = ""
        '    streetName = ""
        '    aptNum = ""
        '    city = ""
        '    state = ""
        '    zip = ""

        '    If qqAddress IsNot Nothing Then
        '        With qqAddress
        '            streetNum = .HouseNum
        '            streetName = .StreetName
        '            aptNum = .ApartmentNumber
        '            city = .City
        '            state = .State
        '            zip = .Zip
        '        End With
        '    End If
        'End Sub
        'Public Sub SetNameFieldsForDiamondImageThirdPartyReportLogRecord(ByVal diaName As Diamond.Common.Objects.Name, ByRef first As String, ByRef middle As String, ByRef last As String, ByRef dob As String, ByRef ssn As String)
        '    first = ""
        '    middle = ""
        '    last = ""
        '    dob = ""
        '    ssn = ""

        '    If diaName IsNot Nothing Then
        '        With diaName
        '            first = .FirstName
        '            middle = .MiddleName
        '            last = .LastName
        '            If .BirthDate <> Nothing AndAlso IsDate(.BirthDate.ToString) Then
        '                dob = .BirthDate.ToString
        '            End If
        '            ssn = .TaxNumber
        '        End With
        '    End If
        'End Sub
        'Public Sub SetAddressFieldsForDiamondImageThirdPartyReportLogRecord(ByVal diaAddress As Diamond.Common.Objects.Address, ByRef streetNum As String, ByRef streetName As String, ByRef aptNum As String, ByRef city As String, ByRef state As String, ByRef zip As String)
        '    streetNum = ""
        '    streetName = ""
        '    aptNum = ""
        '    city = ""
        '    state = ""
        '    zip = ""

        '    If diaAddress IsNot Nothing Then
        '        With diaAddress
        '            streetNum = .HouseNumber
        '            streetName = .StreetName
        '            aptNum = .ApartmentNumber
        '            city = .City
        '            'state = GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, .StateId.ToString)
        '            state = .StateName 'haven't used this before, but is ReadOnly so I'd assume it's looking it up based on the StateId
        '            zip = .Zip
        '        End With
        '    End If
        'End Sub
        'added 12/19/2014 to go along w/ latest logic and db structure
        Public Sub ConvertQuickQuoteNameToThirdPartyReportName(ByVal qqName As QuickQuoteName, ByRef tprName As QuickQuoteThirdPartyReportName, Optional ByVal resetIfNothing As Boolean = False)
            If qqName IsNot Nothing Then
                If tprName Is Nothing Then
                    tprName = New QuickQuoteThirdPartyReportName
                End If
                With tprName
                    .first = qqName.FirstName
                    .middle = qqName.MiddleName
                    .last = qqName.LastName
                    .DOB = qqName.BirthDate
                    .SSN = qqName.TaxNumber
                End With
            Else
                If resetIfNothing = True AndAlso tprName IsNot Nothing Then
                    tprName = Nothing
                End If
            End If
        End Sub
        Public Sub ConvertQuickQuoteAddressToThirdPartyReportAddress(ByVal qqAddress As QuickQuoteAddress, ByRef tprAddress As QuickQuoteThirdPartyReportAddress, Optional ByVal resetIfNothing As Boolean = False)
            If qqAddress IsNot Nothing Then
                If tprAddress Is Nothing Then
                    tprAddress = New QuickQuoteThirdPartyReportAddress
                End If
                With tprAddress
                    .streetNum = qqAddress.HouseNum
                    .streetName = qqAddress.StreetName
                    .apartmentNum = qqAddress.ApartmentNumber
                    .city = qqAddress.City
                    .state = qqAddress.State
                    .zip = qqAddress.Zip
                End With
            Else
                If resetIfNothing = True AndAlso tprAddress IsNot Nothing Then
                    tprAddress = Nothing
                End If
            End If
        End Sub
        Public Sub ConvertDiamondImageNameToThirdPartyReportName(ByVal diaName As Diamond.Common.Objects.Name, ByRef tprName As QuickQuoteThirdPartyReportName, Optional ByVal resetIfNothing As Boolean = False)
            If diaName IsNot Nothing Then
                If tprName Is Nothing Then
                    tprName = New QuickQuoteThirdPartyReportName
                End If
                With tprName
                    .first = diaName.FirstName
                    .middle = diaName.MiddleName
                    .last = diaName.LastName
                    If diaName.BirthDate <> Nothing AndAlso IsDate(diaName.BirthDate.ToString) Then
                        .DOB = diaName.BirthDate.ToString
                    Else
                        .DOB = ""
                    End If
                    .SSN = diaName.TaxNumber
                End With
            Else
                If resetIfNothing = True AndAlso tprName IsNot Nothing Then
                    tprName = Nothing
                End If
            End If
        End Sub
        Public Sub ConvertDiamondImageAddressToThirdPartyReportAddress(ByVal diaAddress As Diamond.Common.Objects.Address, ByRef tprAddress As QuickQuoteThirdPartyReportAddress, Optional ByVal resetIfNothing As Boolean = False)
            If diaAddress IsNot Nothing Then
                If tprAddress Is Nothing Then
                    tprAddress = New QuickQuoteThirdPartyReportAddress
                End If
                With tprAddress
                    .streetNum = diaAddress.HouseNumber
                    .streetName = diaAddress.StreetName
                    .apartmentNum = diaAddress.ApartmentNumber
                    .city = diaAddress.City
                    .state = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, diaAddress.StateId.ToString)
                    '.state = diaAddress.StateName 'haven't used this before, but is ReadOnly so I'd assume it's looking it up based on the StateId; 12/23/2014 note: didn't work
                    .zip = diaAddress.Zip
                End With
            Else
                If resetIfNothing = True AndAlso tprAddress IsNot Nothing Then
                    tprAddress = Nothing
                End If
            End If
        End Sub
        Enum ThirdPartyValidationType
            None = 0
            'Name1First = 1
            'Name1Middle = 2
            'Name1Last = 3
            'Name1DOB = 4
            'Name1SSN = 5
            'Name2First = 6
            'Name2Middle = 7
            'Name2Last = 8
            'Name2DOB = 9
            'Name2SSN = 10
            'AddressStreetNum = 11
            'AddressStreetName = 12
            'AddressApartmentNum = 13
            'AddressCity = 14
            'AddressState = 15
            'AddressZip = 16
            'replaced values 12/11/2014... to go w/ combo of entity type and validationType
            NameFirst = 1
            NameMiddle = 2
            NameLast = 3
            NameDOB = 4
            NameSSN = 5
            AddressStreetNum = 6
            AddressStreetName = 7
            AddressApartmentNum = 8
            AddressCity = 9
            AddressState = 10
            AddressZip = 11
        End Enum
        Enum ThirdPartyReportEntityType 'added 12/11/2014; see ThirdPartyReportEntityType table on QuickQuote database
            None = 0
            Policyholder1 = 1 'Policyholder 1 in db
            Policyholder2 = 2 'Policyholder 2 in db
            Client = 3 'may change to Client1 so we can also have Client2 (Client.Name2)
            Driver = 4
            Applicant = 5
            Location = 6
            'added more 12/18/2014
            Policyholder1Applicant = 7 'Policyholder 1 Applicant in db
            Policyholder2Applicant = 8 'Policyholder 2 Applicant in db
            Policyholder1Driver = 9 'Policyholder 1 Driver in db
            Policyholder2Driver = 10 'Policyholder 2 Driver in db
        End Enum
        Enum ThirdPartyComparisonType
            Neither = 0
            Main = 1
            Compare = 2
            Both = 3
        End Enum
        'Public Function HasThirdPartyDataChange(ByVal tprlr As QuickQuoteThirdPartyReportLogRecord, ByVal tprlrCompare As QuickQuoteThirdPartyReportLogRecord, ByVal validationType As ThirdPartyValidationType, Optional ByRef invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither) As Boolean
        '    Dim hasChange As Boolean = False

        '    If tprlr IsNot Nothing AndAlso tprlrCompare IsNot Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Neither
        '    ElseIf tprlr Is Nothing AndAlso tprlrCompare Is Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Both
        '    ElseIf tprlr Is Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Main
        '    ElseIf tprlrCompare Is Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Compare
        '    End If

        '    If validationType <> Nothing AndAlso validationType <> ThirdPartyValidationType.None Then
        '        If invalidObjectComparisonType = ThirdPartyComparisonType.Neither Then
        '            Select Case validationType
        '                Case ThirdPartyValidationType.Name1First
        '                    If UCase(tprlr.name1First) <> UCase(tprlrCompare.name1First) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name1Middle
        '                    If UCase(tprlr.name1Middle) <> UCase(tprlrCompare.name1Middle) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name1Last
        '                    If UCase(tprlr.name1Last) <> UCase(tprlrCompare.name1Last) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name1DOB
        '                    Dim validDate1 As Boolean = False
        '                    Dim validDate2 As Boolean = False
        '                    If tprlr.name1DOB <> "" AndAlso IsDate(tprlr.name1DOB) = True Then
        '                        validDate1 = True
        '                    End If
        '                    If tprlrCompare.name1DOB <> "" AndAlso IsDate(tprlrCompare.name1DOB) = True Then
        '                        validDate2 = True
        '                    End If
        '                    If validDate1 = True AndAlso validDate2 = True Then
        '                        If CDate(tprlr.name1DOB) <> CDate(tprlrCompare.name1DOB) Then
        '                            hasChange = True
        '                        End If
        '                    ElseIf validDate1 = True OrElse validDate2 = True Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name1SSN
        '                    If UCase(Replace(tprlr.name1SSN, "-", "")) <> UCase(Replace(tprlrCompare.name1SSN, "-", "")) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name2First
        '                    If UCase(tprlr.name2First) <> UCase(tprlrCompare.name2First) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name2Middle
        '                    If UCase(tprlr.name2Middle) <> UCase(tprlrCompare.name2Middle) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name2Last
        '                    If UCase(tprlr.name2Last) <> UCase(tprlrCompare.name2Last) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name2DOB
        '                    Dim validDate1 As Boolean = False
        '                    Dim validDate2 As Boolean = False
        '                    If tprlr.name2DOB <> "" AndAlso IsDate(tprlr.name2DOB) = True Then
        '                        validDate1 = True
        '                    End If
        '                    If tprlrCompare.name2DOB <> "" AndAlso IsDate(tprlrCompare.name2DOB) = True Then
        '                        validDate2 = True
        '                    End If
        '                    If validDate1 = True AndAlso validDate2 = True Then
        '                        If CDate(tprlr.name2DOB) <> CDate(tprlrCompare.name2DOB) Then
        '                            hasChange = True
        '                        End If
        '                    ElseIf validDate1 = True OrElse validDate2 = True Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.Name2SSN
        '                    If UCase(Replace(tprlr.name2SSN, "-", "")) <> UCase(Replace(tprlrCompare.name2SSN, "-", "")) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.AddressStreetNum
        '                    If UCase(tprlr.addressStreetNum) <> UCase(tprlrCompare.addressStreetNum) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.AddressStreetName
        '                    If UCase(tprlr.addressStreetName) <> UCase(tprlrCompare.addressStreetName) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.AddressApartmentNum
        '                    If UCase(tprlr.addressApartmentNum) <> UCase(tprlrCompare.addressApartmentNum) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.AddressCity
        '                    If UCase(tprlr.addressCity) <> UCase(tprlrCompare.addressCity) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.AddressState
        '                    If UCase(tprlr.addressState) <> UCase(tprlrCompare.addressState) Then
        '                        hasChange = True
        '                    End If
        '                Case ThirdPartyValidationType.AddressZip
        '                    If UCase(Replace(tprlr.addressZip, "-", "")) <> UCase(Replace(tprlrCompare.addressZip, "-", "")) Then
        '                        hasChange = True
        '                    End If
        '            End Select
        '        ElseIf invalidObjectComparisonType = ThirdPartyComparisonType.Main OrElse invalidObjectComparisonType = ThirdPartyComparisonType.Compare Then 'may not use
        '            hasChange = True
        '        End If
        '    End If

        '    Return hasChange
        'End Function
        'Public Function HasThirdPartyDataChange(ByVal tprlr As QuickQuoteThirdPartyReportLogRecord, ByVal tprlrCompare As QuickQuoteThirdPartyReportLogRecord, ByVal validationTypes As List(Of ThirdPartyValidationType), Optional ByRef invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither) As Boolean
        '    Dim hasChange As Boolean = False

        '    If tprlr IsNot Nothing AndAlso tprlrCompare IsNot Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Neither
        '    ElseIf tprlr Is Nothing AndAlso tprlrCompare Is Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Both
        '    ElseIf tprlr Is Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Main
        '    ElseIf tprlrCompare Is Nothing Then
        '        invalidObjectComparisonType = ThirdPartyComparisonType.Compare
        '    End If

        '    If validationTypes IsNot Nothing AndAlso validationTypes.Count > 0 Then
        '        If invalidObjectComparisonType = ThirdPartyComparisonType.Neither Then
        '            For Each vt As ThirdPartyValidationType In validationTypes
        '                hasChange = HasThirdPartyDataChange(tprlr, tprlrCompare, vt)
        '                If hasChange = True Then
        '                    Exit For
        '                End If
        '            Next
        '        ElseIf invalidObjectComparisonType = ThirdPartyComparisonType.Main OrElse invalidObjectComparisonType = ThirdPartyComparisonType.Compare Then 'may not use
        '            hasChange = True
        '        End If
        '    End If

        '    Return hasChange
        'End Function
        'Public Function ThirdPartyValidationTypes_Prefill() As List(Of ThirdPartyValidationType)
        '    Dim validationTypes As List(Of ThirdPartyValidationType) = Nothing

        '    Return validationTypes
        'End Function
        'Public Function ThirdPartyValidationTypes_CreditAuto() As List(Of ThirdPartyValidationType)
        '    Dim validationTypes As List(Of ThirdPartyValidationType) = Nothing

        '    '12/10/2014 note: method used while rating already sees if last name and dob is different between existing image and what's currently on the QQ object
        '    'validationTypes = New List(Of ThirdPartyValidationType)
        '    'validationTypes.Add(ThirdPartyValidationType.Name1Last)
        '    'validationTypes.Add(ThirdPartyValidationType.Name1DOB)

        '    Return validationTypes
        'End Function
        'Public Function ThirdPartyValidationTypes_CreditProperty() As List(Of ThirdPartyValidationType)
        '    Dim validationTypes As List(Of ThirdPartyValidationType) = Nothing

        '    '12/10/2014 note: method used while rating already sees if last name and dob is different between existing image and what's currently on the QQ object
        '    'validationTypes = New List(Of ThirdPartyValidationType)
        '    'validationTypes.Add(ThirdPartyValidationType.Name1Last)
        '    'validationTypes.Add(ThirdPartyValidationType.Name1DOB)

        '    Return validationTypes
        'End Function
        'Public Function ThirdPartyValidationTypes_MVR() As List(Of ThirdPartyValidationType)
        '    Dim validationTypes As List(Of ThirdPartyValidationType) = Nothing

        '    Return validationTypes
        'End Function
        'Public Function ThirdPartyValidationTypes_CLUEAuto() As List(Of ThirdPartyValidationType)
        '    Dim validationTypes As List(Of ThirdPartyValidationType) = Nothing

        '    Return validationTypes
        'End Function
        'Public Function ThirdPartyValidationTypes_CLUEProperty() As List(Of ThirdPartyValidationType)
        '    Dim validationTypes As List(Of ThirdPartyValidationType) = Nothing

        '    validationTypes = New List(Of ThirdPartyValidationType)
        '    validationTypes.Add(ThirdPartyValidationType.Name1Last)
        '    validationTypes.Add(ThirdPartyValidationType.Name1DOB)
        '    validationTypes.Add(ThirdPartyValidationType.Name1SSN)
        '    validationTypes.Add(ThirdPartyValidationType.Name2Last)
        '    validationTypes.Add(ThirdPartyValidationType.Name2DOB)
        '    validationTypes.Add(ThirdPartyValidationType.Name2SSN)
        '    validationTypes.Add(ThirdPartyValidationType.AddressStreetNum)
        '    validationTypes.Add(ThirdPartyValidationType.AddressStreetName)
        '    validationTypes.Add(ThirdPartyValidationType.AddressCity)
        '    validationTypes.Add(ThirdPartyValidationType.AddressZip)

        '    Return validationTypes
        'End Function
        ''added 12/9/2014
        'Public Function ThirdPartyValidationTypes(ByVal reportType As ThirdPartyReportType) As List(Of ThirdPartyValidationType)
        '    Dim validationTypes As List(Of ThirdPartyValidationType) = Nothing

        '    Select Case reportType
        '        Case ThirdPartyReportType.AutoPrefill
        '            validationTypes = ThirdPartyValidationTypes_Prefill()
        '        Case ThirdPartyReportType.CreditAuto
        '            validationTypes = ThirdPartyValidationTypes_CreditAuto()
        '        Case ThirdPartyReportType.CreditProperty
        '            validationTypes = ThirdPartyValidationTypes_CreditProperty()
        '        Case ThirdPartyReportType.MVR
        '            validationTypes = ThirdPartyValidationTypes_MVR()
        '        Case ThirdPartyReportType.CLUEAuto
        '            validationTypes = ThirdPartyValidationTypes_CLUEAuto()
        '        Case ThirdPartyReportType.CLUEProperty
        '            validationTypes = ThirdPartyValidationTypes_CLUEProperty()
        '    End Select

        '    Return validationTypes
        'End Function
        '12/11/2014 - created new methods to coincide w/ entity/validationType combo
        Public Function HasThirdPartyDataChange(ByVal tprlr As QuickQuoteThirdPartyReportLogRecord, ByVal tprlrCompare As QuickQuoteThirdPartyReportLogRecord, ByVal combo As QuickQuoteThirdPartyReportEntityValidationTypeCombo, Optional ByRef invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither) As Boolean
            Dim hasChange As Boolean = False

            If tprlr IsNot Nothing AndAlso tprlrCompare IsNot Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Neither
            ElseIf tprlr Is Nothing AndAlso tprlrCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Both
            ElseIf tprlr Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Main
            ElseIf tprlrCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Compare
            End If

            If combo IsNot Nothing AndAlso combo.entityType <> Nothing AndAlso combo.entityType <> ThirdPartyReportEntityType.None AndAlso combo.validationType <> Nothing AndAlso combo.validationType <> ThirdPartyValidationType.None Then
                If invalidObjectComparisonType = ThirdPartyComparisonType.Neither Then
                    Dim entityMain As QuickQuoteThirdPartyReportEntity = ThirdPartyReportEntityForType(tprlr.thirdPartyReportEntities, combo.entityType)
                    Dim entityCompare As QuickQuoteThirdPartyReportEntity = ThirdPartyReportEntityForType(tprlrCompare.thirdPartyReportEntities, combo.entityType)

                    'If entityMain IsNot Nothing AndAlso entityCompare IsNot Nothing Then 'won't validate for now... since method will already handle
                    hasChange = HasThirdPartyDataChange(entityMain, entityCompare, combo.validationType)
                    'End If

                ElseIf invalidObjectComparisonType = ThirdPartyComparisonType.Main OrElse invalidObjectComparisonType = ThirdPartyComparisonType.Compare Then 'may not use
                    'hasChange = True 'removed 12/11/2014
                End If
            End If

            Return hasChange
        End Function
        Public Function HasThirdPartyDataChange(ByVal tprlr As QuickQuoteThirdPartyReportLogRecord, ByVal tprlrCompare As QuickQuoteThirdPartyReportLogRecord, ByVal combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo), Optional ByRef invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither) As Boolean
            Dim hasChange As Boolean = False

            If tprlr IsNot Nothing AndAlso tprlrCompare IsNot Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Neither
            ElseIf tprlr Is Nothing AndAlso tprlrCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Both
            ElseIf tprlr Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Main
            ElseIf tprlrCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Compare
            End If

            If combos IsNot Nothing AndAlso combos.Count > 0 Then
                If invalidObjectComparisonType = ThirdPartyComparisonType.Neither Then
                    For Each c As QuickQuoteThirdPartyReportEntityValidationTypeCombo In combos
                        hasChange = HasThirdPartyDataChange(tprlr, tprlrCompare, c)
                        If hasChange = True Then
                            Exit For
                        End If
                    Next
                ElseIf invalidObjectComparisonType = ThirdPartyComparisonType.Main OrElse invalidObjectComparisonType = ThirdPartyComparisonType.Compare Then 'may not use
                    'hasChange = True 'removed 12/11/2014
                End If
            End If

            Return hasChange
        End Function
        Public Function HasThirdPartyDataChange(ByVal entityMain As QuickQuoteThirdPartyReportEntity, ByVal entityCompare As QuickQuoteThirdPartyReportEntity, ByVal validationType As ThirdPartyValidationType, Optional ByRef invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither) As Boolean 'added 12/11/2014
            Dim hasChange As Boolean = False

            If entityMain IsNot Nothing AndAlso entityCompare IsNot Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Neither
            ElseIf entityMain Is Nothing AndAlso entityCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Both
            ElseIf entityMain Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Main
            ElseIf entityCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Compare
            End If

            If validationType <> Nothing AndAlso validationType <> ThirdPartyValidationType.None Then
                If invalidObjectComparisonType = ThirdPartyComparisonType.Neither Then
                    Select Case validationType
                        Case ThirdPartyValidationType.NameFirst, ThirdPartyValidationType.NameMiddle, ThirdPartyValidationType.NameLast, ThirdPartyValidationType.NameDOB, ThirdPartyValidationType.NameSSN
                            'hasChange = HasThirdPartyEntityNameChange(entityMain.thirdPartyReportName, entityCompare.thirdPartyReportName, validationType)
                            'updated 12/24/2014 to use nameSaved in case the object's not instantiated
                            Dim nameMain As QuickQuoteThirdPartyReportName = entityMain.thirdPartyReportName
                            If nameMain Is Nothing AndAlso entityMain.nameSaved = True Then
                                nameMain = New QuickQuoteThirdPartyReportName
                            End If
                            Dim nameCompare As QuickQuoteThirdPartyReportName = entityCompare.thirdPartyReportName
                            If nameCompare Is Nothing AndAlso entityCompare.nameSaved = True Then
                                nameCompare = New QuickQuoteThirdPartyReportName
                            End If
                            hasChange = HasThirdPartyEntityNameChange(nameMain, nameCompare, validationType)
                        Case ThirdPartyValidationType.AddressStreetNum, ThirdPartyValidationType.AddressStreetName, ThirdPartyValidationType.AddressApartmentNum, ThirdPartyValidationType.AddressCity, ThirdPartyValidationType.AddressState, ThirdPartyValidationType.AddressZip
                            'hasChange = HasThirdPartyEntityAddressChange(entityMain.thirdPartyReportAddress, entityCompare.thirdPartyReportAddress, validationType)
                            'updated 12/24/2014 to use addressSaved in case the object's not instantiated
                            Dim addressMain As QuickQuoteThirdPartyReportAddress = entityMain.thirdPartyReportAddress
                            If addressMain Is Nothing AndAlso entityMain.addressSaved = True Then
                                addressMain = New QuickQuoteThirdPartyReportAddress
                            End If
                            Dim addressCompare As QuickQuoteThirdPartyReportAddress = entityCompare.thirdPartyReportAddress
                            If addressCompare Is Nothing AndAlso entityCompare.addressSaved = True Then
                                addressCompare = New QuickQuoteThirdPartyReportAddress
                            End If
                            hasChange = HasThirdPartyEntityAddressChange(addressMain, addressCompare, validationType)
                    End Select
                ElseIf invalidObjectComparisonType = ThirdPartyComparisonType.Main OrElse invalidObjectComparisonType = ThirdPartyComparisonType.Compare Then 'may not use
                    'hasChange = True 'removed 12/11/2014
                End If
            End If

            Return hasChange
        End Function
        Public Function HasThirdPartyEntityNameChange(ByVal nameMain As QuickQuoteThirdPartyReportName, ByVal nameCompare As QuickQuoteThirdPartyReportName, ByVal validationType As ThirdPartyValidationType, Optional ByRef invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither) As Boolean 'added 12/11/2014
            Dim hasChange As Boolean = False

            If nameMain IsNot Nothing AndAlso nameCompare IsNot Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Neither
            ElseIf nameMain Is Nothing AndAlso nameCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Both
            ElseIf nameMain Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Main
            ElseIf nameCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Compare
            End If

            If validationType <> Nothing AndAlso validationType <> ThirdPartyValidationType.None Then
                If invalidObjectComparisonType = ThirdPartyComparisonType.Neither Then
                    Select Case validationType
                        Case ThirdPartyValidationType.NameFirst
                            If UCase(nameMain.first) <> UCase(nameCompare.first) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.NameMiddle
                            If UCase(nameMain.middle) <> UCase(nameCompare.middle) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.NameLast
                            If UCase(nameMain.last) <> UCase(nameCompare.last) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.NameDOB
                            Dim validDate1 As Boolean = False
                            Dim validDate2 As Boolean = False
                            If nameMain.DOB <> "" AndAlso IsDate(nameMain.DOB) = True Then
                                validDate1 = True
                            End If
                            If nameCompare.DOB <> "" AndAlso IsDate(nameCompare.DOB) = True Then
                                validDate2 = True
                            End If
                            If validDate1 = True AndAlso validDate2 = True Then
                                If CDate(nameMain.DOB) <> CDate(nameCompare.DOB) Then
                                    hasChange = True
                                End If
                            ElseIf validDate1 = True OrElse validDate2 = True Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.NameSSN
                            If UCase(Replace(nameMain.SSN, "-", "")) <> UCase(Replace(nameCompare.SSN, "-", "")) Then
                                hasChange = True
                            End If
                    End Select
                ElseIf invalidObjectComparisonType = ThirdPartyComparisonType.Main OrElse invalidObjectComparisonType = ThirdPartyComparisonType.Compare Then 'may not use
                    'hasChange = True 'removed 12/11/2014
                End If
            End If

            Return hasChange
        End Function
        Public Function HasThirdPartyEntityAddressChange(ByVal addressMain As QuickQuoteThirdPartyReportAddress, ByVal addressCompare As QuickQuoteThirdPartyReportAddress, ByVal validationType As ThirdPartyValidationType, Optional ByRef invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither) As Boolean 'added 12/11/2014
            Dim hasChange As Boolean = False

            If addressMain IsNot Nothing AndAlso addressCompare IsNot Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Neither
            ElseIf addressMain Is Nothing AndAlso addressCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Both
            ElseIf addressMain Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Main
            ElseIf addressCompare Is Nothing Then
                invalidObjectComparisonType = ThirdPartyComparisonType.Compare
            End If

            If validationType <> Nothing AndAlso validationType <> ThirdPartyValidationType.None Then
                If invalidObjectComparisonType = ThirdPartyComparisonType.Neither Then
                    Select Case validationType
                        Case ThirdPartyValidationType.AddressStreetNum
                            If UCase(addressMain.streetNum) <> UCase(addressCompare.streetNum) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.AddressStreetName
                            If UCase(addressMain.streetName) <> UCase(addressCompare.streetName) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.AddressApartmentNum
                            If UCase(addressMain.apartmentNum) <> UCase(addressCompare.apartmentNum) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.AddressCity
                            If UCase(addressMain.city) <> UCase(addressCompare.city) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.AddressState
                            If UCase(addressMain.state) <> UCase(addressCompare.state) Then
                                hasChange = True
                            End If
                        Case ThirdPartyValidationType.AddressZip
                            If UCase(Replace(addressMain.zip, "-", "")) <> UCase(Replace(addressCompare.zip, "-", "")) Then
                                hasChange = True
                            End If
                    End Select
                ElseIf invalidObjectComparisonType = ThirdPartyComparisonType.Main OrElse invalidObjectComparisonType = ThirdPartyComparisonType.Compare Then 'may not use
                    'hasChange = True 'removed 12/11/2014
                End If
            End If

            Return hasChange
        End Function
        Public Function ThirdPartyReportEntityValidationTypeCombos_Prefill() As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = Nothing

            Return combos
        End Function
        Public Function ThirdPartyReportEntityValidationTypeCombos_CreditAuto() As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = Nothing

            '12/10/2014 note: method used while rating already sees if last name and dob is different between existing image and what's currently on the QQ object
            'validationTypes = New List(Of ThirdPartyValidationType)
            'validationTypes.Add(ThirdPartyValidationType.Name1Last)
            'validationTypes.Add(ThirdPartyValidationType.Name1DOB)

            'may need to incorporate PH, but will just use Driver for now
            'combos = New List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Driver, .validationType = ThirdPartyValidationType.NameLast})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Driver, .validationType = ThirdPartyValidationType.NameDOB})

            Return combos
        End Function
        Public Function ThirdPartyReportEntityValidationTypeCombos_CreditProperty() As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = Nothing

            '12/10/2014 note: method used while rating already sees if last name and dob is different between existing image and what's currently on the QQ object
            'validationTypes = New List(Of ThirdPartyValidationType)
            'validationTypes.Add(ThirdPartyValidationType.Name1Last)
            'validationTypes.Add(ThirdPartyValidationType.Name1DOB)

            'may need to incorporate PH, but will just use Applicant for now
            'combos = New List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.NameLast})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.NameDOB})

            Return combos
        End Function
        Public Function ThirdPartyReportEntityValidationTypeCombos_MVR() As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = Nothing

            Return combos
        End Function
        Public Function ThirdPartyReportEntityValidationTypeCombos_CLUEAuto() As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = Nothing

            'added 12/23/2014 so it's similar to CLUEProp (except Loc Address)
            combos = New List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) 'added this line 12/30/2014
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1Driver, .validationType = ThirdPartyValidationType.NameLast})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1Driver, .validationType = ThirdPartyValidationType.NameDOB})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1Driver, .validationType = ThirdPartyValidationType.NameSSN})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2Driver, .validationType = ThirdPartyValidationType.NameLast})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2Driver, .validationType = ThirdPartyValidationType.NameDOB})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2Driver, .validationType = ThirdPartyValidationType.NameSSN})

            Return combos
        End Function
        Public Function ThirdPartyReportEntityValidationTypeCombos_CLUEProperty() As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = Nothing

            combos = New List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            'validationTypes.Add(ThirdPartyValidationType.Name1Last)
            'validationTypes.Add(ThirdPartyValidationType.Name1DOB)
            'validationTypes.Add(ThirdPartyValidationType.Name1SSN)
            'validationTypes.Add(ThirdPartyValidationType.Name2Last)
            'validationTypes.Add(ThirdPartyValidationType.Name2DOB)
            'validationTypes.Add(ThirdPartyValidationType.Name2SSN)
            'validationTypes.Add(ThirdPartyValidationType.AddressStreetNum)
            'validationTypes.Add(ThirdPartyValidationType.AddressStreetName)
            'validationTypes.Add(ThirdPartyValidationType.AddressCity)
            'validationTypes.Add(ThirdPartyValidationType.AddressZip)

            'may need to add entity types for PH Applicant or Driver... Policyholder1Applicant, Policyholder2Applicant, Policyholder1Driver, Policyholder2Driver
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.NameLast})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.NameDOB})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.NameSSN})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.AddressStreetNum})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.AddressStreetName})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.AddressCity})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Applicant, .validationType = ThirdPartyValidationType.AddressZip})
            'will just use PH types for now
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1, .validationType = ThirdPartyValidationType.NameLast})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1, .validationType = ThirdPartyValidationType.NameDOB})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1, .validationType = ThirdPartyValidationType.NameSSN})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2, .validationType = ThirdPartyValidationType.NameLast})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2, .validationType = ThirdPartyValidationType.NameDOB})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2, .validationType = ThirdPartyValidationType.NameSSN})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressStreetNum})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressStreetName})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressCity})
            'combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressZip})
            'updated 12/18/2014 for new ph applicant types
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1Applicant, .validationType = ThirdPartyValidationType.NameLast})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1Applicant, .validationType = ThirdPartyValidationType.NameDOB})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder1Applicant, .validationType = ThirdPartyValidationType.NameSSN})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2Applicant, .validationType = ThirdPartyValidationType.NameLast})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2Applicant, .validationType = ThirdPartyValidationType.NameDOB})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Policyholder2Applicant, .validationType = ThirdPartyValidationType.NameSSN})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressStreetNum})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressStreetName})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressCity})
            combos.Add(New QuickQuoteThirdPartyReportEntityValidationTypeCombo With {.entityType = ThirdPartyReportEntityType.Location, .validationType = ThirdPartyValidationType.AddressZip})

            Return combos
        End Function
        Public Function ThirdPartyReportEntityValidationTypeCombos(ByVal reportType As ThirdPartyReportType) As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo)
            Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = Nothing

            Select Case reportType
                Case ThirdPartyReportType.AutoPrefill
                    combos = ThirdPartyReportEntityValidationTypeCombos_Prefill()
                Case ThirdPartyReportType.CreditAuto
                    combos = ThirdPartyReportEntityValidationTypeCombos_CreditAuto()
                Case ThirdPartyReportType.CreditProperty
                    combos = ThirdPartyReportEntityValidationTypeCombos_CreditProperty()
                Case ThirdPartyReportType.MVR
                    combos = ThirdPartyReportEntityValidationTypeCombos_MVR()
                Case ThirdPartyReportType.CLUEAuto
                    combos = ThirdPartyReportEntityValidationTypeCombos_CLUEAuto()
                Case ThirdPartyReportType.CLUEProperty
                    combos = ThirdPartyReportEntityValidationTypeCombos_CLUEProperty()
            End Select

            Return combos
        End Function
        'Public Function NeedsToReorderReportBasedOnValidation_QuickQuoteObject(ByVal reportType As ThirdPartyReportType, ByVal qq As QuickQuoteObject, ByRef currentThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByVal qqDriver As QuickQuoteDriver = Nothing, Optional ByVal qqApplicant As QuickQuoteApplicant = Nothing, Optional ByVal activeNum As Integer = 0) As Boolean
        'updated for optional byref params; 12/10/2014 - changed byref params to byval... to make sure nothing is changed from the calling side
        'Public Function NeedsToReorderReportBasedOnValidation_QuickQuoteObject(ByVal reportType As ThirdPartyReportType, ByVal qq As QuickQuoteObject, ByRef currentThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByVal qqDriver As QuickQuoteDriver = Nothing, Optional ByVal qqApplicant As QuickQuoteApplicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal unitNum As Integer = 0) As Boolean
        'updated 12/19/2014 for policyNumber and quoteId optional params
        Public Function NeedsToReorderReportBasedOnValidation_QuickQuoteObject(ByVal reportType As ThirdPartyReportType, ByVal qq As QuickQuoteObject, ByRef currentThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByVal qqDriver As QuickQuoteDriver = Nothing, Optional ByVal qqApplicant As QuickQuoteApplicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal unitNum As Integer = 0, Optional ByVal policyNumber As String = "", Optional ByVal quoteId As Integer = 0) As Boolean
            Dim needsToReorder As Boolean = False
            currentThirdPartyReportLogRecord = Nothing

            If reportType <> Nothing AndAlso reportType <> ThirdPartyReportType.None Then
                'Dim validationTypes As List(Of ThirdPartyValidationType) = ThirdPartyValidationTypes(reportType)
                Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = ThirdPartyReportEntityValidationTypeCombos(reportType) 'updated validation logic 12/11/2014
                'If validationTypes IsNot Nothing AndAlso validationTypes.Count > 0 Then
                'updated validation logic 12/11/2014
                If combos IsNot Nothing AndAlso combos.Count > 0 Then
                    If qq IsNot Nothing Then
                        ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord_QuickQuoteObject(qq, policyId, policyImageNum)
                        'If qq.PolicyId <> "" AndAlso IsNumeric(qq.PolicyId) = True AndAlso CInt(qq.PolicyId) > 0 Then
                        If policyId > 0 Then
                            'Dim polId As Integer = 0
                            'Dim polImageNum As Integer = 0
                            'Dim unitNum As Integer = 0
                            Dim useUnitNum As Boolean = True
                            Dim usePolicyNumber As Boolean = False 'added 12/19/2014
                            Dim useQuoteId As Boolean = False 'added 12/19/2014

                            'polId = CInt(qq.PolicyId)
                            'If qq.PolicyImageNum <> "" AndAlso IsNumeric(qq.PolicyImageNum) = True AndAlso CInt(qq.PolicyImageNum) > 0 Then
                            '    polImageNum = CInt(qq.PolicyImageNum) 'may not use
                            'End If
                            Select Case reportType
                                Case ThirdPartyReportType.AutoPrefill, ThirdPartyReportType.CLUEAuto, ThirdPartyReportType.CLUEProperty
                                    useUnitNum = False
                            End Select

                            'currentThirdPartyReportLogRecord = LoadThirdPartyReportLogRecordFromQuickQuoteObject(reportType, qq, errorMsg, qqDriver, qqApplicant, activeNum, policyId, policyImageNum, unitNum)
                            'updated 12/19/2014 for new params
                            currentThirdPartyReportLogRecord = LoadThirdPartyReportLogRecordFromQuickQuoteObject(reportType, qq, errorMsg, qqDriver, qqApplicant, activeNum, policyId, policyImageNum, unitNum, policyNumber, quoteId)
                            If currentThirdPartyReportLogRecord IsNot Nothing Then
                                If currentThirdPartyReportLogRecord.unitNum > 0 Then
                                    unitNum = currentThirdPartyReportLogRecord.unitNum
                                End If
                                Dim existingThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord = Nothing
                                Dim sqlErrorMsg As String = ""

                                'existingThirdPartyReportLogRecord = GetThirdPartyReportLogRecord(CInt(reportType), polId, polImageNum, unitNum, errorMsg, sqlErrorMsg, useUnitNum)
                                '12/9/2014 note: may need to send 0 for policyImageNum here if we want it to just search on policyId... so it doesn't use results from ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord
                                'existingThirdPartyReportLogRecord = GetThirdPartyReportLogRecord(CInt(reportType), policyId, policyImageNum, unitNum, errorMsg, sqlErrorMsg, useUnitNum)
                                'updated 12/19/2014 for new params
                                existingThirdPartyReportLogRecord = GetThirdPartyReportLogRecord(CInt(reportType), policyNumber, policyId, policyImageNum, unitNum, quoteId, errorMsg, sqlErrorMsg, useUnitNum, usePolicyNumber, useQuoteId)
                                If existingThirdPartyReportLogRecord IsNot Nothing Then
                                    'compare current w/ existing
                                    Dim invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither
                                    'needsToReorder = HasThirdPartyDataChange(currentThirdPartyReportLogRecord, existingThirdPartyReportLogRecord, validationTypes, invalidObjectComparisonType)
                                    'updated validation logic 12/11/2014
                                    needsToReorder = HasThirdPartyDataChange(currentThirdPartyReportLogRecord, existingThirdPartyReportLogRecord, combos, invalidObjectComparisonType)
                                Else
                                    If sqlErrorMsg <> "" Then
                                        'error getting existing record from db
                                    Else
                                        'no existing record in db
                                    End If
                                End If
                            Else
                                If errorMsg = "" Then
                                    errorMsg = "unable to load current third party report log record."
                                End If
                            End If
                        Else
                            'errorMsg = "invalid policyId on QuickQuote object"
                            errorMsg = "invalid policyId"
                        End If
                    Else
                        errorMsg = "invalid QuickQuote Object"
                    End If
                Else
                    'no validation types; 12/11/2014 note: now combos
                End If
            Else
                errorMsg = "invalid report type"
            End If

            Return needsToReorder
        End Function
        'Public Function NeedsToReorderReportBasedOnValidation_DiamondImage(ByVal reportType As ThirdPartyReportType, ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef currentThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByVal diaDriver As Diamond.Common.Objects.Policy.Driver = Nothing, Optional ByVal diaApplicant As Diamond.Common.Objects.Policy.Applicant = Nothing, Optional ByVal activeNum As Integer = 0) As Boolean
        'updated for optional byref params; 12/10/2014 - changed byref params to byval... to make sure nothing is changed from the calling side
        'Public Function NeedsToReorderReportBasedOnValidation_DiamondImage(ByVal reportType As ThirdPartyReportType, ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef currentThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByVal diaDriver As Diamond.Common.Objects.Policy.Driver = Nothing, Optional ByVal diaApplicant As Diamond.Common.Objects.Policy.Applicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal unitNum As Integer = 0) As Boolean
        'updated 12/19/2014 for policyNumber and quoteId optional params
        Public Function NeedsToReorderReportBasedOnValidation_DiamondImage(ByVal reportType As ThirdPartyReportType, ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef currentThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByVal diaDriver As Diamond.Common.Objects.Policy.Driver = Nothing, Optional ByVal diaApplicant As Diamond.Common.Objects.Policy.Applicant = Nothing, Optional ByVal activeNum As Integer = 0, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal unitNum As Integer = 0, Optional ByVal policyNumber As String = "", Optional ByVal quoteId As Integer = 0) As Boolean
            Dim needsToReorder As Boolean = False
            currentThirdPartyReportLogRecord = Nothing

            If reportType <> Nothing AndAlso reportType <> ThirdPartyReportType.None Then
                'Dim validationTypes As List(Of ThirdPartyValidationType) = ThirdPartyValidationTypes(reportType)
                Dim combos As List(Of QuickQuoteThirdPartyReportEntityValidationTypeCombo) = ThirdPartyReportEntityValidationTypeCombos(reportType) 'updated validation logic 12/11/2014
                'If validationTypes IsNot Nothing AndAlso validationTypes.Count > 0 Then
                'updated validation logic 12/11/2014
                If combos IsNot Nothing AndAlso combos.Count > 0 Then
                    If diaImage IsNot Nothing Then
                        ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord_DiamondImage(diaImage, policyId, policyImageNum)
                        'If diaImage.PolicyId > 0 Then
                        If policyId > 0 Then
                            'Dim polId As Integer = 0
                            'Dim polImageNum As Integer = 0
                            'Dim unitNum As Integer = 0
                            Dim useUnitNum As Boolean = True
                            Dim usePolicyNumber As Boolean = False 'added 12/19/2014
                            Dim useQuoteId As Boolean = False 'added 12/19/2014

                            'polId = diaImage.PolicyId
                            'If diaImage.PolicyImageNum > 0 Then
                            '    polImageNum = diaImage.PolicyImageNum 'may not use
                            'End If
                            Select Case reportType
                                Case ThirdPartyReportType.AutoPrefill, ThirdPartyReportType.CLUEAuto, ThirdPartyReportType.CLUEProperty
                                    useUnitNum = False
                            End Select

                            'currentThirdPartyReportLogRecord = LoadThirdPartyReportLogRecordFromDiamondImage(reportType, diaImage, errorMsg, diaDriver, diaApplicant, activeNum, policyId, policyImageNum, unitNum)
                            'updated 12/19/2014 for new params
                            currentThirdPartyReportLogRecord = LoadThirdPartyReportLogRecordFromDiamondImage(reportType, diaImage, errorMsg, diaDriver, diaApplicant, activeNum, policyId, policyImageNum, unitNum, policyNumber, quoteId)
                            If currentThirdPartyReportLogRecord IsNot Nothing Then
                                If currentThirdPartyReportLogRecord.unitNum > 0 Then
                                    unitNum = currentThirdPartyReportLogRecord.unitNum
                                End If
                                Dim existingThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord = Nothing
                                Dim sqlErrorMsg As String = ""

                                'existingThirdPartyReportLogRecord = GetThirdPartyReportLogRecord(CInt(reportType), polId, polImageNum, unitNum, errorMsg, sqlErrorMsg, useUnitNum)
                                '12/9/2014 note: may need to send 0 for policyImageNum here if we want it to just search on policyId... so it doesn't use results from ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord
                                'existingThirdPartyReportLogRecord = GetThirdPartyReportLogRecord(CInt(reportType), policyId, policyImageNum, unitNum, errorMsg, sqlErrorMsg, useUnitNum)
                                'updated 12/19/2014 for new params
                                existingThirdPartyReportLogRecord = GetThirdPartyReportLogRecord(CInt(reportType), policyNumber, policyId, policyImageNum, unitNum, quoteId, errorMsg, sqlErrorMsg, useUnitNum, usePolicyNumber, useQuoteId)
                                If existingThirdPartyReportLogRecord IsNot Nothing Then
                                    'compare current w/ existing
                                    Dim invalidObjectComparisonType As ThirdPartyComparisonType = ThirdPartyComparisonType.Neither
                                    'needsToReorder = HasThirdPartyDataChange(currentThirdPartyReportLogRecord, existingThirdPartyReportLogRecord, validationTypes, invalidObjectComparisonType)
                                    'updated validation logic 12/11/2014
                                    needsToReorder = HasThirdPartyDataChange(currentThirdPartyReportLogRecord, existingThirdPartyReportLogRecord, combos, invalidObjectComparisonType)
                                Else
                                    If sqlErrorMsg <> "" Then
                                        'error getting existing record from db
                                    Else
                                        'no existing record in db
                                    End If
                                End If
                            Else
                                If errorMsg = "" Then
                                    errorMsg = "unable to load current third party report log record."
                                End If
                            End If
                        Else
                            'errorMsg = "invalid policyId on Diamond image"
                            errorMsg = "invalid policyId"
                        End If
                    Else
                        errorMsg = "invalid Diamond image"
                    End If
                Else
                    'no validation types; 12/11/2014 note: now combos
                End If
            Else
                errorMsg = "invalid report type"
            End If

            Return needsToReorder
        End Function
        Public Sub SetPolicyIdAndImageNumberForThirdPartyReportLogRecord_QuickQuoteObject(ByVal qq As QuickQuoteObject, ByRef policyId As Integer, ByRef policyImageNum As Integer)
            policyId = 0
            policyImageNum = 0

            If qq IsNot Nothing Then
                If qq.PolicyId <> "" AndAlso IsNumeric(qq.PolicyId) = True AndAlso CInt(qq.PolicyId) > 0 Then
                    policyId = CInt(qq.PolicyId)
                    If qq.PolicyImageNum <> "" AndAlso IsNumeric(qq.PolicyImageNum) = True AndAlso CInt(qq.PolicyImageNum) > 0 Then
                        policyImageNum = CInt(qq.PolicyImageNum) 'may not use
                    End If
                End If
            End If
        End Sub
        Public Sub SetPolicyIdAndImageNumberForThirdPartyReportLogRecord_DiamondImage(ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef policyId As Integer, ByRef policyImageNum As Integer)
            policyId = 0
            policyImageNum = 0

            If diaImage IsNot Nothing Then
                If diaImage.PolicyId > 0 Then
                    policyId = diaImage.PolicyId
                    If diaImage.PolicyImageNum > 0 Then
                        policyImageNum = diaImage.PolicyImageNum 'may not use
                    End If
                End If
            End If
        End Sub
        Public Sub ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord_QuickQuoteObject(ByVal qq As QuickQuoteObject, ByRef policyId As Integer, ByRef policyImageNum As Integer)
            If policyId > 0 Then
                'can validate; can also set policyImageNum to 0 from here if needed

            Else
                SetPolicyIdAndImageNumberForThirdPartyReportLogRecord_QuickQuoteObject(qq, policyId, policyImageNum)
            End If
        End Sub
        Public Sub ValidatePolicyIdAndImageNumberForThirdPartyReportLogRecord_DiamondImage(ByVal diaImage As Diamond.Common.Objects.Policy.Image, ByRef policyId As Integer, ByRef policyImageNum As Integer)
            If policyId > 0 Then
                'can validate; can also set policyImageNum to 0 from here if needed

            Else
                SetPolicyIdAndImageNumberForThirdPartyReportLogRecord_DiamondImage(diaImage, policyId, policyImageNum)
            End If
        End Sub
        'added 12/10/2014
        Public Function QuickQuoteThirdPartyReportLogRecordForUnitNum(ByVal qqThirdPartyReportLogRecords As List(Of QuickQuoteThirdPartyReportLogRecord), ByVal unitNum As Integer) As QuickQuoteThirdPartyReportLogRecord
            Dim qqThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord = Nothing

            'If unitNum > 0 AndAlso qqThirdPartyReportLogRecords IsNot Nothing AndAlso qqThirdPartyReportLogRecords.Count > 0 Then
            'not going to validate unitNum since it could be 0
            If qqThirdPartyReportLogRecords IsNot Nothing AndAlso qqThirdPartyReportLogRecords.Count > 0 Then
                For Each tprlr As QuickQuoteThirdPartyReportLogRecord In qqThirdPartyReportLogRecords
                    If tprlr.unitNum = unitNum Then
                        qqThirdPartyReportLogRecord = tprlr
                        Exit For
                    End If
                Next
            End If

            Return qqThirdPartyReportLogRecord
        End Function
        Public Function HasQuickQuoteThirdPartyReportLogRecordForUnitNum(ByVal qqThirdPartyReportLogRecords As List(Of QuickQuoteThirdPartyReportLogRecord), ByVal unitNum As Integer) As Boolean
            Dim hasRecord As Boolean = False

            Dim qqThirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord = QuickQuoteThirdPartyReportLogRecordForUnitNum(qqThirdPartyReportLogRecords, unitNum)
            If qqThirdPartyReportLogRecord IsNot Nothing Then
                hasRecord = True
            End If

            Return hasRecord
        End Function
        'added 12/11/2014
        Public Sub InsertThirdPartyReportEntity(ByRef thirdPartyReportEntity As QuickQuoteThirdPartyReportEntity, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "")
            errorMsg = ""
            sqlErrorMsg = ""

            If thirdPartyReportEntity IsNot Nothing Then
                If thirdPartyReportEntity.thirdPartyReportEntityTypeId > 0 Then

                    'updated 12/18/2014 to insert name and address records
                    If thirdPartyReportEntity.thirdPartyReportName IsNot Nothing Then
                        Dim nameInsertError As String = ""
                        Dim nameInsertErrorSql As String = ""
                        InsertThirdPartyReportName(thirdPartyReportEntity.thirdPartyReportName, nameInsertError, nameInsertErrorSql)
                        If nameInsertErrorSql <> "" Then
                            thirdPartyReportEntity.nameSaved = False
                        ElseIf nameInsertError = "" AndAlso thirdPartyReportEntity.thirdPartyReportName.thirdPartyReportNameId > 0 Then
                            thirdPartyReportEntity.nameSaved = True
                        End If
                    End If
                    If thirdPartyReportEntity.thirdPartyReportAddress IsNot Nothing Then
                        Dim addressInsertError As String = ""
                        Dim addressInsertErrorSql As String = ""
                        InsertThirdPartyReportAddress(thirdPartyReportEntity.thirdPartyReportAddress, addressInsertError, addressInsertErrorSql)
                        If addressInsertErrorSql <> "" Then
                            thirdPartyReportEntity.addressSaved = False
                        ElseIf addressInsertError = "" AndAlso thirdPartyReportEntity.thirdPartyReportAddress.thirdPartyReportAddressId > 0 Then
                            thirdPartyReportEntity.addressSaved = True
                        End If
                    End If

                    Using sqlEO As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                        sqlEO.queryOrStoredProc = "usp_Insert_ThirdPartyReportEntity"
                        '@thirdPartyReportEntityTypeId int,
                        '@thirdPartyReportNameId int = NULL,
                        '@thirdPartyReportAddressId int = NULL,
                        '@thirdPartyReportEntityId int output

                        sqlEO.inputParameters = New ArrayList
                        sqlEO.inputParameters.Add(New SqlParameter("@thirdPartyReportEntityTypeId", thirdPartyReportEntity.thirdPartyReportEntityTypeId))
                        If thirdPartyReportEntity.thirdPartyReportName IsNot Nothing Then 'may need to verify the nameId too
                            sqlEO.inputParameters.Add(New SqlParameter("@thirdPartyReportNameId", thirdPartyReportEntity.thirdPartyReportName.thirdPartyReportNameId))
                        End If
                        If thirdPartyReportEntity.thirdPartyReportAddress IsNot Nothing Then 'may need to verify the addressId too
                            sqlEO.inputParameters.Add(New SqlParameter("@thirdPartyReportAddressId", thirdPartyReportEntity.thirdPartyReportAddress.thirdPartyReportAddressId))
                        End If
                        '12/18/2014 - added new params
                        sqlEO.inputParameters.Add(New SqlParameter("@nameSaved", qqHelper.BooleanToInt(thirdPartyReportEntity.nameSaved)))
                        sqlEO.inputParameters.Add(New SqlParameter("@addressSaved", qqHelper.BooleanToInt(thirdPartyReportEntity.addressSaved)))
                        '12/18/2014 - added new params
                        sqlEO.inputParameters.Add(New SqlParameter("@isPolicyholder1", qqHelper.BooleanToInt(thirdPartyReportEntity.isPolicyholder1)))
                        sqlEO.inputParameters.Add(New SqlParameter("@isPolicyholder2", qqHelper.BooleanToInt(thirdPartyReportEntity.isPolicyholder2)))

                        sqlEO.outputParameter = New SqlParameter("@thirdPartyReportEntityId", Data.SqlDbType.Int)

                        sqlEO.ExecuteStatement()

                        'If sqlEO.rowsAffected = 0 OrElse sqlEO.hasError = True Then
                        If sqlEO.hasError = True Then
                            'error
                            errorMsg = "error inserting third party report entity into database"
                            sqlErrorMsg = sqlEO.errorMsg
                        Else
                            thirdPartyReportEntity.thirdPartyReportEntityId = sqlEO.outputParameter.Value
                            thirdPartyReportEntity.inserted = Date.Now.ToString
                        End If
                    End Using
                Else
                    errorMsg = "please provide a valid thirdPartyReportEntityTypeId"
                End If
            Else
                errorMsg = "please provide thirdPartyReportEntity"
            End If
        End Sub
        Public Sub InsertThirdPartyReportName(ByRef thirdPartyReportName As QuickQuoteThirdPartyReportName, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "")
            errorMsg = ""
            sqlErrorMsg = ""

            If thirdPartyReportName IsNot Nothing Then
                Using sqlEO As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                    sqlEO.queryOrStoredProc = "usp_Insert_ThirdPartyReportName"
                    '@first varchar(50) = NULL,
                    '@middle varchar(50) = NULL,
                    '@last varchar(50) = NULL,
                    '@DOB date = NULL,
                    '@SSN varchar(20) = NULL,
                    '@thirdPartyReportNameId int output

                    sqlEO.inputParameters = New ArrayList
                    If thirdPartyReportName.first <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@first", thirdPartyReportName.first))
                    End If
                    If thirdPartyReportName.middle <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@middle", thirdPartyReportName.middle))
                    End If
                    If thirdPartyReportName.last <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@last", thirdPartyReportName.last))
                    End If
                    If thirdPartyReportName.DOB <> "" AndAlso IsDate(thirdPartyReportName.DOB) = True Then
                        sqlEO.inputParameters.Add(New SqlParameter("@DOB", CDate(thirdPartyReportName.DOB)))
                    End If
                    If thirdPartyReportName.SSN <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@SSN", thirdPartyReportName.SSN))
                    End If

                    sqlEO.outputParameter = New SqlParameter("@thirdPartyReportNameId", Data.SqlDbType.Int)

                    sqlEO.ExecuteStatement()

                    'If sqlEO.rowsAffected = 0 OrElse sqlEO.hasError = True Then
                    If sqlEO.hasError = True Then
                        'error
                        errorMsg = "error inserting third party report name into database"
                        sqlErrorMsg = sqlEO.errorMsg
                    Else
                        thirdPartyReportName.thirdPartyReportNameId = sqlEO.outputParameter.Value
                        thirdPartyReportName.inserted = Date.Now.ToString
                    End If
                End Using
            Else
                errorMsg = "please provide thirdPartyReportName"
            End If
        End Sub
        Public Sub InsertThirdPartyReportAddress(ByRef thirdPartyReportAddress As QuickQuoteThirdPartyReportAddress, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "")
            errorMsg = ""
            sqlErrorMsg = ""

            If thirdPartyReportAddress IsNot Nothing Then
                Using sqlEO As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                    sqlEO.queryOrStoredProc = "usp_Insert_ThirdPartyReportAddress"
                    '@streetNum varchar(20) = NULL,
                    '@streetName varchar(100) = NULL,
                    '@apartmentNum varchar(20) = NULL,
                    '@city varchar(100) = NULL,
                    '@state varchar(20) = NULL,
                    '@zip varchar(10) = NULL,
                    '@thirdPartyReportAddressId int output

                    sqlEO.inputParameters = New ArrayList
                    If thirdPartyReportAddress.streetNum <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@streetNum", thirdPartyReportAddress.streetNum))
                    End If
                    If thirdPartyReportAddress.streetName <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@streetName", thirdPartyReportAddress.streetName))
                    End If
                    If thirdPartyReportAddress.apartmentNum <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@apartmentNum", thirdPartyReportAddress.apartmentNum))
                    End If
                    If thirdPartyReportAddress.city <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@city", thirdPartyReportAddress.city))
                    End If
                    If thirdPartyReportAddress.state <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@state", thirdPartyReportAddress.state))
                    End If
                    If thirdPartyReportAddress.zip <> "" Then
                        sqlEO.inputParameters.Add(New SqlParameter("@zip", thirdPartyReportAddress.zip))
                    End If

                    sqlEO.outputParameter = New SqlParameter("@thirdPartyReportAddressId", Data.SqlDbType.Int)

                    sqlEO.ExecuteStatement()

                    'If sqlEO.rowsAffected = 0 OrElse sqlEO.hasError = True Then
                    If sqlEO.hasError = True Then
                        'error
                        errorMsg = "error inserting third party report address into database"
                        sqlErrorMsg = sqlEO.errorMsg
                    Else
                        thirdPartyReportAddress.thirdPartyReportAddressId = sqlEO.outputParameter.Value
                        thirdPartyReportAddress.inserted = Date.Now.ToString
                    End If
                End Using
            Else
                errorMsg = "please provide thirdPartyReportAddress"
            End If
        End Sub
        'added 12/11/2014; could add optional param for FirstOrLast enum type... if last, would remove Exit For statement so it keeps looping... shouldn't be needed since there should only be 1 active link for each entity type
        Public Function ThirdPartyReportEntityForType(ByVal entities As List(Of QuickQuoteThirdPartyReportEntity), ByVal entityType As ThirdPartyReportEntityType) As QuickQuoteThirdPartyReportEntity
            Dim e As QuickQuoteThirdPartyReportEntity = Nothing

            If entityType <> Nothing AndAlso entityType <> ThirdPartyReportEntityType.None Then
                If entities IsNot Nothing AndAlso entities.Count > 0 Then
                    For Each ent As QuickQuoteThirdPartyReportEntity In entities
                        If ent.thirdPartyReportEntityTypeId = CInt(entityType) Then
                            e = ent
                            Exit For
                        End If
                    Next
                End If
            End If

            Return e
        End Function
        'added 12/12/2014
        'Public Sub InsertThirdPartyReportLogEntityLink(ByVal thirdPartyReportLogId As Integer, ByVal thirdPartyReportEntityId As Integer, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "", Optional ByRef thirdPartyReportLogEntityLinkId As Integer = 0)
        'updated 12/18/2014 for optional thirdPartyReportEntityTypeId param
        Public Sub InsertThirdPartyReportLogEntityLink(ByVal thirdPartyReportLogId As Integer, ByVal thirdPartyReportEntityId As Integer, ByRef errorMsg As String, Optional ByVal thirdPartyReportEntityTypeId As Integer = 0, Optional ByRef sqlErrorMsg As String = "", Optional ByRef thirdPartyReportLogEntityLinkId As Integer = 0)
            errorMsg = ""
            sqlErrorMsg = ""
            thirdPartyReportLogEntityLinkId = 0

            If thirdPartyReportLogId > 0 AndAlso thirdPartyReportEntityId > 0 Then
                Using sqlEO As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                    sqlEO.queryOrStoredProc = "usp_Insert_ThirdPartyReportLogEntityLink" 'not created yet; will also have active flag (defaulted to True) and updated timestamp... on insert, any other links for same entitytypeid should be deactivated; created EOD 12/12/2014
                    '@thirdPartyReportLogId int,
                    '@thirdPartyReportEntityId int, '12/12/2014 note: may also need thirdPartyReportEntityTypeId in table so sp can use that to deactivate any other links for that type
                    '@thirdPartyReportLogEntityLinkId int output

                    sqlEO.inputParameters = New ArrayList
                    sqlEO.inputParameters.Add(New SqlParameter("@thirdPartyReportLogId", thirdPartyReportLogId))
                    sqlEO.inputParameters.Add(New SqlParameter("@thirdPartyReportEntityId", thirdPartyReportEntityId))
                    If thirdPartyReportEntityTypeId > 0 Then 'added 12/18/2014
                        sqlEO.inputParameters.Add(New SqlParameter("@thirdPartyReportEntityTypeId", thirdPartyReportEntityTypeId))
                    End If

                    sqlEO.outputParameter = New SqlParameter("@thirdPartyReportLogEntityLinkId", Data.SqlDbType.Int)

                    sqlEO.ExecuteStatement()

                    'If sqlEO.rowsAffected = 0 OrElse sqlEO.hasError = True Then
                    If sqlEO.hasError = True Then
                        'error
                        errorMsg = "error inserting third party report log entity link into database"
                        sqlErrorMsg = sqlEO.errorMsg
                    Else
                        thirdPartyReportLogEntityLinkId = sqlEO.outputParameter.Value
                        'inserted = Date.Now.ToString
                    End If
                End Using
            Else
                errorMsg = "please provide a valid thirdPartyReportLogId and thirdPartyReportEntityId"
            End If
        End Sub
        'added 12/18/2014
        Public Sub GetThirdPartyReportLogEntityLinks(ByRef thirdPartyReportLogRecord As QuickQuoteThirdPartyReportLogRecord, ByRef errorMsg As String, Optional ByRef sqlErrorMsg As String = "")
            errorMsg = ""
            sqlErrorMsg = ""

            If thirdPartyReportLogRecord IsNot Nothing Then
                If thirdPartyReportLogRecord.thirdPartyReportLogId > 0 Then
                    Using sqlSO As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                        sqlSO.queryOrStoredProc = "usp_Get_ThirdPartyReportLogEntityLinks"
                        '@thirdPartyReportLogId int

                        sqlSO.parameter = New SqlParameter("@thirdPartyReportLogId", thirdPartyReportLogRecord.thirdPartyReportLogId)

                        Dim dr As SqlDataReader = sqlSO.GetDataReader
                        If dr IsNot Nothing AndAlso dr.HasRows = True Then
                            While dr.Read
                                Dim e As New QuickQuoteThirdPartyReportEntity
                                With e
                                    e.thirdPartyReportEntityId = qqHelper.IntegerForString(dr.Item("thirdPartyReportEntityId").ToString.Trim)
                                    e.thirdPartyReportEntityTypeId = qqHelper.IntegerForString(dr.Item("thirdPartyReportEntityTypeId").ToString.Trim)
                                    'e.inserted = dr.Item("inserted").ToString.Trim 'this is inserted field for Link table
                                    e.inserted = dr.Item("entityInserted").ToString.Trim
                                    e.nameSaved = qqHelper.BitToBoolean(dr.Item("nameSaved").ToString.Trim)
                                    e.addressSaved = qqHelper.BitToBoolean(dr.Item("addressSaved").ToString.Trim)
                                    e.isPolicyholder1 = qqHelper.BitToBoolean(dr.Item("isPolicyholder1").ToString.Trim) 'added 12/23/2014
                                    e.isPolicyholder2 = qqHelper.BitToBoolean(dr.Item("isPolicyholder2").ToString.Trim) 'added 12/23/2014
                                    Dim nameId As Integer = qqHelper.IntegerForString(dr.Item("thirdPartyReportNameId").ToString.Trim)
                                    Dim addressId As Integer = qqHelper.IntegerForString(dr.Item("thirdPartyReportAddressId").ToString.Trim)
                                    If nameId > 0 Then
                                        e.nameSaved = True
                                    End If
                                    If addressId > 0 Then
                                        e.addressSaved = True
                                    End If
                                    If e.nameSaved = True Then
                                        e.thirdPartyReportName = New QuickQuoteThirdPartyReportName
                                        With e.thirdPartyReportName
                                            .thirdPartyReportNameId = nameId
                                            .first = dr.Item("first").ToString.Trim
                                            .middle = dr.Item("middle").ToString.Trim
                                            .last = dr.Item("last").ToString.Trim
                                            .DOB = dr.Item("DOB").ToString.Trim
                                            .SSN = dr.Item("SSN").ToString.Trim
                                            .inserted = dr.Item("nameInserted").ToString.Trim
                                        End With
                                    End If
                                    If e.addressSaved = True Then
                                        e.thirdPartyReportAddress = New QuickQuoteThirdPartyReportAddress
                                        With e.thirdPartyReportAddress
                                            .thirdPartyReportAddressId = addressId
                                            .streetNum = dr.Item("streetNum").ToString.Trim
                                            .streetName = dr.Item("streetName").ToString.Trim
                                            .apartmentNum = dr.Item("apartmentNum").ToString.Trim
                                            .city = dr.Item("city").ToString.Trim
                                            .state = dr.Item("state").ToString.Trim
                                            .zip = dr.Item("zip").ToString.Trim
                                            .inserted = dr.Item("addressInserted").ToString.Trim
                                        End With
                                    End If
                                End With
                                If thirdPartyReportLogRecord.thirdPartyReportEntities Is Nothing Then
                                    thirdPartyReportLogRecord.thirdPartyReportEntities = New List(Of QuickQuoteThirdPartyReportEntity)
                                End If
                                thirdPartyReportLogRecord.thirdPartyReportEntities.Add(e)
                            End While
                        ElseIf sqlSO.hasError = True Then
                            errorMsg = "error getting third party report log entity links from database"
                            sqlErrorMsg = sqlSO.errorMsg
                        Else
                            errorMsg = "no third party report log entity links found"
                        End If
                    End Using
                Else
                    errorMsg = "invalid thirdPartyReportLogId for thirdPartyReportLogRecord"
                End If
            Else
                errorMsg = "please provide a valid thirdPartyReportLogRecord"
            End If
        End Sub
        'added 8/6/2015 to centralize logic
        'Updated 02/10/2021 for CAP Endorsements Task 52973 MLW
        Public Shared Function IsOkayToOrderCredit(ByVal lobType As QuickQuoteObject.QuickQuoteLobType) As Boolean
            'Dim isOkay As Boolean = False

            'Select Case lobType
            '    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.Farm
            '        isOkay = True
            'End Select

            'Return isOkay
            Return IsOkayToOrderCredit_WithTranType(lobType)
        End Function
        'Added 02/10/2021 for CAP Endorsements Task 52973 MLW
        Public Shared Function IsOkayToOrderCredit_WithTranType(ByVal lobType As QuickQuoteObject.QuickQuoteLobType, Optional ByVal tranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None) As Boolean
            Dim isOkay As Boolean = False

            Select Case lobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.Farm
                    isOkay = True
            End Select

            Return isOkay
        End Function
        'added 4/23/2019
        'Updated 02/10/2021 for CAP Endorsements Task 52973 MLW
        Public Shared Function IsOkayToOrderMvr(ByVal lobType As QuickQuoteObject.QuickQuoteLobType) As Boolean
            'Dim isOkay As Boolean = False

            'Select Case lobType
            '    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
            '        isOkay = True
            'End Select

            'Return isOkay
            Return IsOkayToOrderMvr_WithTranType(lobType)
        End Function

        'Added 2/10/2021 for CAP Endorsements Task 52973
        Public Shared Function IsOkayToOrderMvr_WithTranType(ByVal lobType As QuickQuoteObject.QuickQuoteLobType, Optional ByVal tranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None) As Boolean
            Dim isOkay As Boolean = False

            Select Case lobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    isOkay = True
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    If tranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        isOkay = True
                    End If
            End Select

            Return isOkay
        End Function
        'Updated 02/10/2021 for CAP Endorsements Task 52973 MLW
        Public Shared Function IsOkayToOrderClue(ByVal lobType As QuickQuoteObject.QuickQuoteLobType) As Boolean
            'Dim isOkay As Boolean = False

            'Select Case lobType
            '    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
            '        isOkay = True
            'End Select

            'Return isOkay
            Return IsOkayToOrderClue_WithTranType(lobType)
        End Function
        'Added 02/10/2021 for CAP Endorsements Task 52973 MLW
        Public Shared Function IsOkayToOrderClue_WithTranType(ByVal lobType As QuickQuoteObject.QuickQuoteLobType, Optional ByVal tranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None) As Boolean
            Dim isOkay As Boolean = False

            Select Case lobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    isOkay = True
            End Select

            Return isOkay
        End Function
        'Updated 02/10/2021 for CAP Endorsements Task 52973 MLW
        Public Shared Function IsOkayToOrderCreditMvrOrClue(ByVal lobType As QuickQuoteObject.QuickQuoteLobType) As Boolean
            'Dim isOkay As Boolean = False

            'If IsOkayToOrderCredit(lobType) = True OrElse IsOkayToOrderMvr(lobType) = True OrElse IsOkayToOrderClue(lobType) = True Then
            '    isOkay = True
            'End If

            'Return isOkay
            Return IsOkayToOrderCreditMvrOrClue_WithTranType(lobType)
        End Function
        'Added 02/10/2021 for CAP Endorsements Task 52973 MLW
        Public Shared Function IsOkayToOrderCreditMvrOrClue_WithTranType(ByVal lobType As QuickQuoteObject.QuickQuoteLobType, Optional ByVal tranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None) As Boolean
            Dim isOkay As Boolean = False

            If IsOkayToOrderCredit_WithTranType(lobType, tranType:=tranType) = True OrElse IsOkayToOrderMvr_WithTranType(lobType, tranType:=tranType) = True OrElse IsOkayToOrderClue_WithTranType(lobType, tranType:=tranType) = True Then
                isOkay = True
            End If

            Return isOkay
        End Function

        'added 5/13/2021
        Public Shared Function StringForThirdPartyReportTypeCategory(ByVal reportTypeCat As ThirdPartyReportTypeCategory, Optional ByVal allowNone As Boolean = False) As String
            Dim strCat As String = ""

            If System.Enum.IsDefined(GetType(ThirdPartyReportTypeCategory), reportTypeCat) = True AndAlso (allowNone = True OrElse reportTypeCat <> ThirdPartyReportTypeCategory.None) Then
                strCat = System.Enum.GetName(GetType(ThirdPartyReportTypeCategory), reportTypeCat)
            End If

            Return strCat
        End Function
    End Class
End Namespace
