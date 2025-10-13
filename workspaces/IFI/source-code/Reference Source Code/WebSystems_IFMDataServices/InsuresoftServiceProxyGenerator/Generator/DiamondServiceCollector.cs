using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace InsuresoftServiceProxyGenerator.Generator
{
    class DiamondServiceCollector
    {
        readonly string dllFilePath = "";
        public string AssemblyVersion = "";
        public Dictionary<Type, List<MethodInfo>> TypeList = new Dictionary<Type, List<MethodInfo>>();
        public int TypeCount { get; set; }
        public int MethodCount { get; set; }

        public DiamondServiceCollector(string filePath)
        {
            this.dllFilePath = filePath;
            GetTypes();
        }

        private void GetTypes()
        {
            System.Reflection.Assembly myAssembly = Assembly.LoadFile(dllFilePath);
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(dllFilePath);
            this.AssemblyVersion = myFileVersionInfo.FileVersion;
            Type[] types = GetTypesInNamespace(myAssembly, "Diamond.Common.Services.Proxies");
            collectTypes(types);
        }

        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            var types = assembly.GetTypes();
            var tList = from t in types
                        where
                        t.Namespace.ToLower().StartsWith(nameSpace.ToLower()) &&
                        t.Name.ToLower().EndsWith("proxy") && t.Name.ToLower() != "proxy" && t.IsPublic
                        select t;
            return tList.ToArray();
        }

        private void collectTypes(Type[] types)
        {
            foreach (var i in types)
            {
                if (i.IsPublic)
                {
                    var mList = new List<MethodInfo>();
                    TypeList.Add(i, mList);
                    TypeCount += 1;
                    BuildMethodRequests(i, mList);
                }
            }
        }



        private void BuildMethodRequests(Type t, List<MethodInfo> methodList)
        {
            foreach (var m in from x in t.GetMethods() orderby x.Name select x)
            {
                if (m.IsPublic && m.IsGenericMethod == false && m.ReturnType.FullName.EndsWith("Response"))
                {
                    var parms = m.GetParameters();
                    if ((from p in parms where p.ParameterType.FullName == "System.IAsyncResult" select p).Any() == false)
                    {
                        if ((from p in parms where p.ParameterType.FullName.EndsWith("Request") select p).Any() == true)
                        {
                            methodList.Add(m);
                            MethodCount += 1;
                        }
                    }
                }
            }
        }



    }
}
