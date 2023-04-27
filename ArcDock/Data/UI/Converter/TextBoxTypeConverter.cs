using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ArcDock.Data.UI.Converter
{
    /// <summary>
    /// 模板配置文件中文本框类型的转换器
    /// </summary>
    public class TextBoxTypeConverter : IValueConverter
    {
        /// <summary>
        /// 类型ID与友好名称对应字典
        /// </summary>
        private Dictionary<string, string> textBoxTypeDict = new Dictionary<string, string>()
        {
            {"input","普通文本框"},
            {"richinput","多行文本框" },
            {"autoinput","智能文本框" },
            {"json", "JSON对象" }
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
            return textBoxTypeDict[value.ToString()];
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
            var reverseDict = textBoxTypeDict.ToDictionary(x => x.Value, x => x.Key);
            return reverseDict[value.ToString()];
        }
    }
}
