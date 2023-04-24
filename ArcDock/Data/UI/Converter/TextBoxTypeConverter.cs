using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArcDock.Data.UI.Converter
{
    internal class TextBoxTypeConverter : IValueConverter
    {
        private Dictionary<string, string> textBoxTypeDict = new Dictionary<string, string>()
        {
            {"input","普通文本框"},
            {"richinput","多行文本框" },
            {"autoinput","智能文本框" },
            {"json", "JSON对象" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return textBoxTypeDict[value.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reverseDict = textBoxTypeDict.ToDictionary(x => x.Value, x => x.Key);
            return reverseDict[value.ToString()];
            //var reverseDict = textBoxTypeDict.ToDictionary(x => x.Value, x => x.Key);
            //return reverseDict[value.ToString()];
        }
    }
}
