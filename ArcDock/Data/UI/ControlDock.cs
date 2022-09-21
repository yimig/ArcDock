using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ArcDock.Data.Json;
using CefSharp;
using CefSharp.Wpf;

namespace ArcDock.Data.UI
{
    public class ControlDock: DockPanel
    {
        public List<CustomArea> CustomAreas;
        private StackPanel textStack;
        private StackPanel inputStack;

        public ControlDock()
        {
            CustomAreas = new List<CustomArea>();
            textStack = new StackPanel();
            DockPanel.SetDock(textStack,Dock.Left);
            inputStack = new StackPanel();
            this.Children.Add(textStack);
            this.Children.Add(inputStack);
        }

        public void AddArea(CustomArea ca)
        {
            textStack.Children.Add(ca.Label);
            inputStack.Children.Add(ca.InputControl);
            CustomAreas.Add(ca);
        }

        public void ClearChildren()
        {
            CustomAreas.Clear();
            textStack.Children.Clear();
            inputStack.Children.Clear();
        }
    }
}
