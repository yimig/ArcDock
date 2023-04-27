using ArcDock.Data;
using ArcDock.Data.Json;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ArcDock
{
    /// <summary>
    /// 文件列名匹配对应窗口
    /// </summary>
    public partial class TableMatchWindow : Window, INotifyPropertyChanged
    {
        #region 字段属性和事件

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 文件列名对应配置项选项ID生成JSON时使用此配置
        /// </summary>
        private ConfigItem configItem;
        /// <summary>
        /// 文件列名对应配置项ID循环打印时使用此配置
        /// </summary>
        private Config config;
        private string filePath;
        /// <summary>
        /// 文件列名与配置ID的对应关系列表
        /// </summary>
        private ObservableCollection<TableTitleItem> tableTitleList;
        private List<string> displayIdList;
        /// <summary>
        /// 文件列名与配置ID的对应关系列表
        /// </summary>
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
        /// <summary>
        /// 数据文件路径
        /// </summary>
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
        /// <summary>
        /// 配置ID列表，作为下拉列表资源使用
        /// </summary>
        public List<string> DisplayIdList
        {
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
        /// <summary>
        /// 是否用户点击确定
        /// </summary>
        public bool IsCheck { get; set; }
        /// <summary>
        /// 文件对应结果
        /// </summary>
        public Dictionary<string, List<string>> Result { get; set; }
        /// <summary>
        /// 文件分析结果
        /// </summary>
        private Dictionary<string, List<string>> FileTable { get; set; }
        /// <summary>
        /// 当前页面的日志控制对象
        /// </summary>
        private static ILog log = LogManager.GetLogger("TableMatchWindow");

        #endregion

        #region 初始化

        /// <summary>
        /// 文件列名匹配对应窗口
        /// </summary>
        public TableMatchWindow()
        {
            IsCheck = false;
            DisplayIdList = new List<string>();
            TableTitleList = new ObservableCollection<TableTitleItem>();
            this.DataContext = this;
            InitializeComponent();

        }

        /// <summary>
        /// 文件列名对应配置项选项ID生成JSON时使用此配置
        /// </summary>
        /// <param name="configItem"></param>
        public TableMatchWindow(ConfigItem configItem) : this()
        {
            this.configItem = configItem;
        }

        /// <summary>
        /// 文件列名对应配置项ID循环打印时使用此配置
        /// </summary>
        /// <param name="config"></param>
        public TableMatchWindow(Config config) : this()
        {
            this.config = config;
        }

        /// <summary>
        /// 初始化显示列表
        /// </summary>
        private void InitTable()
        {
            if (configItem != null)
            {
                InsertTableValue(configItem.Option.ToList());
            }
            else
            {
                InsertTableValue(config.ConfigItemList.Select(i => i.Id).ToList());
            }

        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 选择文件并分析
        /// </summary>
        private void SelectFile()
        {
            var fileDialog = new OpenFileDialog() { Filter = "Excel Document(.xlsx)|*.xlsx" };
            if (fileDialog.ShowDialog() == true)
            {
                FilePath = fileDialog.FileName;
                try
                {
                    FileTable = ExcelReader.ReadXlsx(FilePath);
                    InitTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取文件：" + FilePath + "失败：" + ex.Message);
                    log.Error("读取文件：" + FilePath + "失败", ex);
                }
            }
        }

        /// <summary>
        /// 清空显示列表
        /// </summary>
        private void ClearTable()
        {
            TableTitleList.Clear();
        }

        /// <summary>
        /// 显示列表添加值
        /// </summary>
        /// <param name="idList">文件列</param>
        private void InsertTableValue(List<string> idList)
        {
            ClearTable();
            DisplayIdList = FileTable.Keys.ToList();
            if (DisplayIdList.Count() > 0)
            {
                for (var i = 0; i < idList.Count(); i++)
                {
                    if (i < DisplayIdList.Count())
                    {
                        TableTitleList.Add(new TableTitleItem(DisplayIdList.ToArray()[i], idList[i]));
                    }
                    else
                    {
                        TableTitleList.Add(new TableTitleItem(DisplayIdList[DisplayIdList.Count() - 1], idList[i]));
                    }
                }
            }

        }

        /// <summary>
        /// 检查填写内容
        /// </summary>
        /// <returns>是否检查通过</returns>
        private bool CheckResult()
        {
            var list = TableTitleList.Select(i => i.Id).ToList();
            var distinctList = list.Distinct().ToList();
            if (list.Count() > distinctList.Count())
            {
                for (var i = 0; i < list.Count(); i++)
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

        /// <summary>
        /// 生成对应结果
        /// </summary>
        private void GetResult()
        {
            Result = new Dictionary<string, List<string>>();
            foreach (var item in TableTitleList)
            {
                Result.Add(item.Id, FileTable[item.FileTitle]);
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 用户选择确定按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (CheckResult())
            {
                IsCheck = true;
                GetResult();
                this.Close();
            }
        }

        /// <summary>
        /// 用户按下选择文件按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSelectFile_Click(object sender, RoutedEventArgs e)
        {
            SelectFile();
        }

        #endregion
    }
}
