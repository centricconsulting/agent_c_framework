using System;
#if DEBUG
using System.Diagnostics;
#endif
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insuresoft.DiamondServices
{
    public class ServiceCall<Req, Res, ReqData> : IDisposable
    {
        public delegate Res pMethod(Req Request);
        private Req Request { get; set; }
        public ReqData RequestData { get; private set; }
        private readonly pMethod ProxyMethod;
        private readonly Diamond.Common.Services.Proxies.ProxyBase Proxy;
        private IFMResponse<Res> LastResponse;

        public ServiceCall(Diamond.Common.Services.Proxies.ProxyBase proxy, pMethod proxyMethod)
        {
            // create request object
            Request = (Req)Activator.CreateInstance(typeof(Req));
            RequestData = (ReqData)((dynamic)Request).RequestData; // I know it is going to have a request data
            this.Proxy = proxy;
            this.ProxyMethod = proxyMethod;
        }

        public IFMResponse<Res> Invoke()
        {
            var response = new IFMResponse<Res>();
            try
            {
                response.DiamondResponse = ProxyMethod(Request);
                if (LastResponse != null)
                    LastResponse.Dispose();
                LastResponse = response;
                if (response.DiamondResponse != null)
                    response.dv = ((dynamic)response.DiamondResponse).DiamondValidation; // I know it returns a response that always has .DiamondValidation because it is in the response base class                

            }
            catch (Exception err)
            {


                response.ex = err;
                if (Request != null && Request.ToString() != "Diamond.Common.Services.Messages.SecurityService.GetSignedOnUser.Request")
                {
#if DEBUG
                    Debugger.Break();
#else
                IFM.IFMErrorLogging.LogException<Req>(err,"",Request);
#endif
                }

            }
            return response;
        }

        //public async Task<IFMResponse<Res>> InvokeAsync()
        //{
        //    return await new Task<IFMResponse<Res>>(()=> Invoke());
        //}



        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Proxy != null)
                    {
                        Proxy.Dispose(); // we newed this up so we know it is not null                        
                    }
                    if (LastResponse != null)
                        ((dynamic)LastResponse).Dispose();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }

    }


    public class IFMResponse<Res> : IDisposable
    {
        public Exception ex { get; set; }
        public Diamond.Common.Objects.DiamondValidation dv { get; set; }
        public Res DiamondResponse { get; set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // TODO: dispose managed state (managed objects).
                if (disposing && DiamondResponse != null && IsIDisposable(typeof(Res)))
                    ((dynamic)DiamondResponse).Dispose();

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        bool IsIDisposable(Type t)
        {
            return t.GetInterfaces().Contains(typeof(IDisposable));
        }

    }


}
