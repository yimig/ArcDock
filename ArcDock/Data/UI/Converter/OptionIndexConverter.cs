using ArcDock.Data.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArcDock.Data.UI.Converter
{
    public class OptionIndexConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var optionIndexDict = (parameter as Config).ConfigItemList.ToDictionary(i => i.Id, i => i.Name);
            return optionIndexDict[value.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reverseDict = (parameter as Config).ConfigItemList.ToDictionary(x => x.Name, x => x.Id);
            return reverseDict[value.ToString()];
            //var reverseDict = textBoxTypeDict.ToDictionary(x => x.Value, x => x.Key);
            //return reverseDict[value.ToString()];
        }
    }
}
