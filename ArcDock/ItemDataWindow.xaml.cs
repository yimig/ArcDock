using ArcDock.Data.Database;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArcDock
{
    /// <summary>
    /// 历史记录详细信息窗口
    /// </summary>
    public partial class ItemDataWindow : Window
    {
        #region 属性和字段

        /// <summary>
        /// 查询结果集
        /// </summary>
        private List<DataResult> dataResults;

        /// <summary>
        /// 是否为单数项
        /// </summary>
        private bool isSingleItem = true;

        /// <summary>
        /// 单数项颜色
        /// </summary>
        private Color color1 = Color.FromRgb(193, 215, 229);

        /// <summary>
        /// 双数项颜色
        /// </summary>
        private Color color2 = Color.FromRgb(159, 180, 206);

        #endregion

        #region 初始化

        public ItemDataWindow(List<DataResult> data)
        {
            InitializeComponent();
            dataResults = data;
            InitControls();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitControls()
        {
            foreach (var result in dataResults)
            {
                StMain.Children.Add(GetItemStack(result.TemplateName, result.TemplateContent));
            }

            StMain.Children.Add(GetItemStack("模板名称", dataResults[0].TemplateFileName));
            StMain.Children.Add(GetItemStack("打印类型", dataResults[0].PrintType));
            StMain.Children.Add(GetItemStack("打印时间", dataResults[0].PrintDate));
        }

        /// <summary>
        /// 初始化结果项栈UI
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private StackPanel GetItemStack(string title, string content)
        {
            var sp = new StackPanel();
            sp.Background = new SolidColorBrush(GetItemColor());
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(new TextBlock()
            {
                Text = title + ":",
                Margin = new Thickness(20, 5, 10, 5),
                FontSize = 18,
                FontWeight = FontWeights.Bold
            });
            sp.Children.Add(new TextBox()
            {
                IsReadOnly = true,
                Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255)),
                BorderThickness = new Thickness(0),
                Text = content,
                Margin = new Thickness(5),
                FontSize = 18
            });
            return sp;
        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 获取结果项背景颜色
        /// </summary>
        /// <returns>目前背景色</returns>
        private Color GetItemColor()
        {
            var resColor = Color.FromRgb(0, 0, 0);
            if (isSingleItem) resColor = color1;
            else resColor = color2;
            isSingleItem = !isSingleItem;
            return resColor;
        }

        #endregion

    }
}
