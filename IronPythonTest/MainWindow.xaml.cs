using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IronPythonTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string input, output;
        public string Input
        {
            get => input;
            set
            {
                input = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Input"));
                }
            }
        }
        public string Output
        {
            get => output;
            set
            {
                output = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Output"));
                }
            }
        }

        private void TbInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                Output = RunPython(TbInput.Text);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string RunPython(string code)
        {
            var pythonEngin = IronPython.Hosting.Python.CreateEngine();
            ScriptScope scope = pythonEngin.CreateScope();
            scope.SetVariable("test", 2);
            var pythonScripts = pythonEngin.CreateScriptSourceFromString(code);
            pythonScripts.Execute(scope);
            return Convert.ToString(scope.GetVariable("test"));
        }
    }
}
