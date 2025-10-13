using IFM.ControlFlags;
using IFM.VR.Common.Underwriting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(IFM.Web.Modules.DependencyInjectionModule),
                                     nameof(IFM.Web.Modules.DependencyInjectionModule.RegisterModule))]
namespace IFM.Web.Modules
{
    public class DependencyInjectionModule : IHttpModule
    {
        private HttpApplication _application;
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            _application = context;

            if( new IFM.VR.Flags.App.DependencyInjection().UseMEDI)
            {
                RegisterServices(_application.AddServiceCollection());
            }
        }

        protected void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMemoryCache()
                             .AddSingleton<IFeatureFlag,VR.Flags.LOB.PPA>()
                             .AddSingleton<IUnderwritingQuestionsService,UnderwritingQuestionsService>();

            AddLocalFileProvider(serviceCollection);
        }

        protected IServiceCollection AddLocalFileProvider(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IFileProvider>(new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory));
            return serviceCollection;
        }
        public static void RegisterModule()
        {
            HttpApplication.RegisterModule(typeof(DependencyInjectionModule));
        }
    }
}
