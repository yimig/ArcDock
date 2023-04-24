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
        private string fileTitle, jsonId;
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

        public string JsonId { 
            get => jsonId; 
            set
            {
                jsonId = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("JsonId"));
                }
            } 
        }

        public TableTitleItem(string fileTitle, string jsonId)
        {
            FileTitle = fileTitle;
            JsonId = jsonId;
        }
    }
}
