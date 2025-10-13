using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace InsuresoftServiceProxyGenerator.Generator
{
    class ProxyCodeGenerator
    {
        readonly StringBuilder GeneratedCode = new StringBuilder();
        readonly DiamondServiceCollector dsc;
        public const string MyNameSpace = "Insuresoft.DiamondServices";

        public string Code { get { return GeneratedCode.ToString(); } }

        public ProxyCodeGenerator(DiamondServiceCollector dsc)
        {
            this.dsc = dsc;
            GenerateCode();
        }

        void GenerateCode()
        {
            GeneratedCode.AppendLine($"namespace {MyNameSpace}" );
            GeneratedCode.AppendLine("{");

            foreach (var t in dsc.TypeList)
            {
                GeneratedCode.AppendLine(DiamondComments.GetCodeCommentForClass(t.Key.FullName));
                GeneratedCode.AppendLine($"public static class {(t.Key.Name.EndsWith("Proxy") == false ? t.Key.Name : t.Key.Name.Remove(t.Key.Name.Length - 5, 5))}");
                GeneratedCode.AppendLine("{");
                BuildMethodRequests(t.Key, t.Value);
                GeneratedCode.AppendLine("}");
            }

            GeneratedCode.AppendLine("}");

        }

        private void BuildMethodRequests(Type t, List<MethodInfo> mInfoList)
        {
            Func<string, string> GetGenericTypes = delegate (string name) {
                return $"{name}, {(name.Remove(name.Length - 7, 7))}Response,{name}Data";//,{(name.Remove(name.Length - 7, 7))}ResponseData";
            };

            foreach (var m in mInfoList)
            {
                var parms = m.GetParameters();
                GeneratedCode.AppendLine(DiamondComments.GetCodeCommentForMethod(t.FullName + "." + m.Name));
                GeneratedCode.AppendLine($"public static ServiceCall<{GetGenericTypes(parms[0].ParameterType.FullName)}>{m.Name}()");


                GeneratedCode.AppendLine("{");

                GeneratedCode.AppendLine($"var proxy = new global::{t.FullName}();");
                GeneratedCode.AppendLine($"var s = new ServiceCall<{GetGenericTypes(parms[0].ParameterType.FullName)}>(proxy,proxy.{m.Name});");

                GeneratedCode.AppendLine("return s;");
                GeneratedCode.AppendLine("}");// end method
            }
        }



    }
}
