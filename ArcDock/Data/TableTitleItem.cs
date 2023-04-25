using ArcDock.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data
{
    public class TableTitleItem: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string fileTitle, id;
        public string FileTitle {
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

        public string Id { 
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
