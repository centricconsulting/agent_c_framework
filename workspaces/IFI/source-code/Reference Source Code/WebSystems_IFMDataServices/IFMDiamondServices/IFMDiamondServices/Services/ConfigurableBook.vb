'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSC = Diamond.Common.Services.Messages.ConfigurableBookService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.ConfigurableBook
    Public Module ConfigurableBook
        Public Function CreateNewVersion(ByRef res As DCSC.CreateNewVersion.Response,
                                         ByRef req As DCSC.CreateNewVersion.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CreateNewVersion
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function DeleteAnalysis(ByRef res As DCSC.DeleteAnalysis.Response,
                                         ByRef req As DCSC.DeleteAnalysis.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DeleteAnalysis
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExtractPoliciesToRate(ByRef res As DCSC.ExtractPoliciesToRate.Response,
                                         ByRef req As DCSC.ExtractPoliciesToRate.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExtractPoliciesToRate
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExtractPoliciesToTestRateVersions(ByRef res As DCSC.ExtractPoliciesToTestRateVersions.Response,
                                         ByRef req As DCSC.ExtractPoliciesToTestRateVersions.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExtractPoliciesToTestRateVersions
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExtractPolicy(ByRef res As DCSC.ExtractPolicy.Response,
                                         ByRef req As DCSC.ExtractPolicy.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExtractPolicy
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function ExtractPolicyForm(ByRef res As DCSC.ExtractPolicyForm.Response,
                                         ByRef req As DCSC.ExtractPolicyForm.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ExtractPolicyForm
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Load(ByRef res As DCSC.Load.Response,
                                         ByRef req As DCSC.Load.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Load
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAnalysis(ByRef res As DCSC.LoadAnalysis.Response,
                                         ByRef req As DCSC.LoadAnalysis.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadAnalysis
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadAvailableVersions(ByRef res As DCSC.LoadAvailableVersions.Response,
                                         ByRef req As DCSC.LoadAvailableVersions.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadAvailableVersions
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function PerformanceTestStub(ByRef res As DCSC.PerformanceTestStub.Response,
                                         ByRef req As DCSC.PerformanceTestStub.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.PerformanceTestStub
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function RatePolicy(ByRef res As DCSC.RatePolicy.Response,
                                         ByRef req As DCSC.RatePolicy.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.RatePolicy
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Save(ByRef res As DCSC.Save.Response,
                                         ByRef req As DCSC.Save.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Save
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveAnalysis(ByRef res As DCSC.SaveAnalysis.Response,
                                         ByRef req As DCSC.SaveAnalysis.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveAnalysis
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function SaveNew(ByRef res As DCSC.SaveNew.Response,
                                         ByRef req As DCSC.SaveNew.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveNew
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function TestBook(ByRef res As DCSC.TestBook.Response,
                                         ByRef req As DCSC.TestBook.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.TestBook
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function Validate(ByRef res As DCSC.ValidateBook.Response,
                                         ByRef req As DCSC.ValidateBook.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ConfigurableBookService.ConfigurableBookServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.Validate
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace