using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ArcDock.Data.UI.Converter
{
    /// <summary>
    /// 模板配置文件中智能文本框类型的转换器
    /// </summary>
    internal class FillTypeConverter : IValueConverter
    {
        /// <summary>
        /// 类型ID与友好显示名称对应字典
        /// </summary>
        private Dictionary<int, string> fillTypeDict = new Dictionary<int, string>()
        {
            {0,"无自动补全"},
            {1,"单框文本补全" },
            {2,"多框联动补全" }
        };

        /// <summary>
        /// 类型ID转换为友好显示名称
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return fillTypeDict[(int)value];
        }

        /// <summary>
        /// 友好显示名称转换为类型ID
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reverseDict = fillTypeDict.ToDictionary(x => x.Value, x => x.Key);
            return reverseDict[value.ToString()];
        }
    }
}
