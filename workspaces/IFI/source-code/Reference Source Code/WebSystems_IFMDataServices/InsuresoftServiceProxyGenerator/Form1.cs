using InsuresoftServiceProxyGenerator.Generator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InsuresoftServiceProxyGenerator
{
    public partial class Form1 : Form
    {

        // Do Not Change this path because this program also has direct references to the dlls at this path
        // if you changed this folder but not the references then they would not match
        public const string AssemblyFolder = @"G:\ITStaff\Matt\Diamond assemblies\";

        string dllFilePath = AssemblyFolder + "Diamond.Common.Services.dll";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            DiamondServiceCollector dsc = new DiamondServiceCollector(this.dllFilePath);
            ProxyCodeGenerator pcg = new ProxyCodeGenerator(dsc);
            this.txtCode.Text = pcg.Code;
            this.lblAssemblyVersion.Text = $"Diamond Assembly Version: {dsc.AssemblyVersion}";

        }
    }
}
