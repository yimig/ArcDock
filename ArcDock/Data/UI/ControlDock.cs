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

        public void ResetChildrenContent()
        {
            for (int i = 0; i < CustomAreas.Count; i++)
            {
                SetChildrenContentValue(i, CustomAreas[i].config.Default);
            }
        }

        public void SetChildrenContentValue(int index,string newVal)
        {
            if (CustomAreas[index].config.Type.Equals("input")) (CustomAreas[index] as InputArea).Content = newVal;
            else if (CustomAreas[index].config.Type.Equals("richinput")) (CustomAreas[index] as RichInputArea).Content = newVal;
            else if (CustomAreas[index].config.Type.Equals("autoinput")) (CustomAreas[index] as AutoInputArea).Content = newVal;
        }

        public void SetChildrenContentValue(string id, string newVal)
        {
            for (int i = 0; i < CustomAreas.Count; i++)
            {
                if (CustomAreas[i].Id == id)
                {
                    SetChildrenContentValue(i,newVal);
                    return;
                }
            }
        }
    }
}
