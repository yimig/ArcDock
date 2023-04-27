using ArcDock.Data.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ArcDock.Data.UI.Converter
{
    /// <summary>
    /// 模板配置中配置ID与友好名称的转换器
    /// </summary>
    public class OptionIndexConverter : IValueConverter
    {
        /// <summary>
        /// 配置ID转换为友好名称，需要在parameter中传入config对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">需要转换的config对象</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var optionIndexDict = (parameter as Config).ConfigItemList.ToDictionary(i => i.Id, i => i.Name);
            return optionIndexDict[value.ToString()];
        }

        /// <summary>
        /// 友好名称转换为配置ID，需要在parameter中传入config对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">需要转换的config对象</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var reverseDict = (parameter as Config).ConfigItemList.ToDictionary(x => x.Name, x => x.Id);
            return reverseDict[value.ToString()];
        }
    }
}
