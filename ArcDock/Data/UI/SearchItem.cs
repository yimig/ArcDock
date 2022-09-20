using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ArcDock.Data.UI
{
    public class SearchItem: DockPanel
    {
        private TextBlock textBlock;

        public string Text
        {
            get => textBlock.Text;
            set => textBlock.Text = value;
        }

        public SearchItem(string text)
        {
            this.textBlock = new TextBlock() { Text = text };
            DockPanel.SetDock(textBlock, Dock.Left);
            this.Children.Add(textBlock);
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
