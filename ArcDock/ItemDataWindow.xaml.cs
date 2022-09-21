using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ArcDock.Data.Database;

namespace ArcDock
{
    /// <summary>
    /// ItemDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ItemDataWindow : Window
    {
        private List<DataResult> dataResults;
        private bool isSingleItem = true;
        private Color color1 = Color.FromRgb(193, 215, 229);
        private Color color2 = Color.FromRgb(159,180,206);

        public ItemDataWindow(List<DataResult> data)
        {
            InitializeComponent();
            dataResults = data;
            InitControls();
        }

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

        private StackPanel GetItemStack(string title,string content)
        {
            var sp = new StackPanel();
            sp.Background = new SolidColorBrush(GetItemColor());
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(new TextBlock()
            {
                Text = title + ":",
                Margin = new Thickness(20,5,10,5),
                FontSize = 18,
                FontWeight = FontWeights.Bold
            });
            sp.Children.Add(new TextBox()
            {
                IsReadOnly = true,
                Background = new SolidColorBrush(Color.FromArgb(0,255,255,255)),
                BorderThickness = new Thickness(0),
                Text = content,
                Margin = new Thickness(5),
                FontSize = 18
            });
            return sp;
        }

        private Color GetItemColor()
        {
            var resColor = Color.FromRgb(0,0,0);
            if (isSingleItem) resColor = color1;
            else resColor = color2;
            isSingleItem = !isSingleItem;
            return resColor;
        }
    }
}
