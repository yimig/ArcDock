using ArcDock.Data;
using ArcDock.Data.Json;
using ArcDock.Properties;
using log4net;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace ArcDock
{
    /// <summary>
    /// Interaction logic for TableMatchWindow.xaml
    /// </summary>
    public partial class TableMatchWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private ConfigItem configItem;
        private string filePath;
        private ObservableCollection<TableTitleItem> tableTitleList;
        private List<string> displayIdList;
        public ObservableCollection<TableTitleItem> TableTitleList
        {
            get => tableTitleList;
            set
            {
                tableTitleList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TableTitleList"));
                }
            }
        }
        public string FilePath 
        { 
            get => filePath; 
            set
            {
                filePath = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FilePath"));
                }
            }
        }
        public List<string> DisplayFileTitleList {
            get => displayIdList; 
            set
            {
                displayIdList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayJsonIdList"));
                }
            } 
        }
        public bool IsCheck { get; set; }
        public string ResultJson { get; set; }
        private Dictionary<string, List<string>> FileTable { get; set; }
        private static ILog log = LogManager.GetLogger("TableMatchWindow");

        public TableMatchWindow(ConfigItem configItem)
        {
            this.configItem = configItem;
            IsCheck = false;
            DisplayFileTitleList = new List<string>();
            TableTitleList = new ObservableCollection<TableTitleItem>();
            this.DataContext = this;
            InitializeComponent();

        }

        private void SelectFile()
        {
            var fileDialog = new OpenFileDialog() { Filter = "Excel Document(.xlsx)|*.xlsx" };
            if(fileDialog.ShowDialog() == true)
            {
                FilePath = fileDialog.FileName;
                try
                {
                    FileTable = ExcelReader.ReadXlsx(FilePath);
                    InitTable();
                } catch (Exception ex)
                {
                    MessageBox.Show("读取文件：" + FilePath + "失败：" + ex.Message);
                    log.Error("读取文件："+ FilePath + "失败",ex);
                }
            }
        }
        
        private void ClearTable()
        {
            TableTitleList.Clear();
        }

        private void InitTable()
        {
            ClearTable();
            DisplayFileTitleList = FileTable.Keys.ToList();
            if(DisplayFileTitleList.Count()>0)
            {
                for (var i = 0; i < configItem.Option.Count(); i++)
                {
                    if (i < DisplayFileTitleList.Count())
                    {
                        TableTitleList.Add(new TableTitleItem(DisplayFileTitleList.ToArray()[i], configItem.Option[i]));
                    }
                    else
                    {
                        TableTitleList.Add(new TableTitleItem(DisplayFileTitleList[DisplayFileTitleList.Count() - 1], configItem.Option[i]));
                    }
                }
            }

        }

        private bool CheckResult()
        {
            var list = TableTitleList.Select(i => i.Id).ToList();
            var distinctList = list.Distinct().ToList();
            if (list.Count() > distinctList.Count())
            {
                for(var i = 0; i < list.Count(); i++)
                {
                    if (i >= distinctList.Count() || list[i] != distinctList[i])
                    {
                        MessageBox.Show("不可使相同的文件列名对应相同的Json ID，请检查以下重复Json ID项：" + list[i], "Json ID重复");
                        break;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }

        }

        private void GenerateJson()
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var item in TableTitleList)
            {
                result.Add(item.Id, FileTable[item.FileTitle]);
            }
            ResultJson = JsonConvert.SerializeObject(result);
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if(CheckResult())
            {
                IsCheck = true;
                GenerateJson();
                this.Close();
            }
        }

        private void tbSelectFile_Click(object sender, RoutedEventArgs e)
        {
            SelectFile();
        }
    }
}
