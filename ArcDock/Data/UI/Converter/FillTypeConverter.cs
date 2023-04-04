using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArcDock.Data.UI.Converter
{
    internal class FillTypeConverter : IValueConverter
    {
        private Dictionary<int, string> fillTypeDict = new Dictionary<int, string>()
        {
            {0,"无自动补全"},
            {1,"单框文本补全" },
            {2,"多框联动补全" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return fillTypeDict[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reverseDict = fillTypeDict.ToDictionary(x => x.Value, x => x.Key);
            return reverseDict[value.ToString()];
        }
    }
}
