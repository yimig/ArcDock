using System.ComponentModel;

namespace ArcDock.Data
{
    /// <summary>
    /// 读取文件列名与配置ID的对应关系
    /// </summary>
    public class TableTitleItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string fileTitle, id;

        /// <summary>
        /// 文件列名
        /// </summary>
        public string FileTitle
        {
            get => fileTitle;
            set
            {
                fileTitle = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FileTitle"));
                }
            }
        }

        /// <summary>
        /// 配置ID
        /// </summary>
        public string Id
        {
            get => id;
            set
            {
                id = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Id"));
                }
            }
        }

        public TableTitleItem(string fileTitle, string id)
        {
            FileTitle = fileTitle;
            Id = id;
        }
    }
}
