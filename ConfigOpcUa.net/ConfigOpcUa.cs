using ConfigOpcUa.net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUa
{
    public class ConfigOpcUa : ConfigPtx.CfgPtx
    {
        private readonly ViewModel _viewModel;

        public ConfigOpcUa()
        {
            _viewModel = new ViewModel();
        }
        public override void CheckProject(string pName, string pErrName)
        {
        }

        public override byte[] CheCoLabel(int mod, string pLabel)
        {
            return null;
        }

        public override string CreatePort(System.Windows.Forms.IWin32Window hWnd)
        {
            return string.Empty;
        }

        public override void LoadConfig(string pName)
        {
        }

        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            MainWindow mainWindow = new MainWindow(_viewModel);
            mainWindow.ShowDialog();
        }

        public override void OS9Files(out string pFiles, string pName, int typ)
        {
            pFiles = string.Empty;
        }

        public override void StartupLines(out string pLinesW, out string pLines, string pName, int typ)
        {
            pLinesW = string.Empty;
            pLines = string.Empty;
        }

        public override void UnLoadConfig()
        {
        }
    }
}
