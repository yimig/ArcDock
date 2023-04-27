using System.Collections.Generic;
using System.Windows.Controls;

namespace ArcDock.Data.UI
{
    /// <summary>
    /// 主页左侧控件流区域，根据模板填充控件
    /// </summary>
    public class ControlDock : DockPanel
    {
        /// <summary>
        /// 当前控件集合
        /// </summary>
        public List<CustomArea> CustomAreas;
        /// <summary>
        /// 控件标题区域
        /// </summary>
        private StackPanel textStack;
        /// <summary>
        /// 控件文本框区域
        /// </summary>
        private StackPanel inputStack;

        /// <summary>
        /// 初始化控件流区域
        /// </summary>
        public ControlDock()
        {
            CustomAreas = new List<CustomArea>();
            textStack = new StackPanel();
            DockPanel.SetDock(textStack, Dock.Left);
            inputStack = new StackPanel();
            this.Children.Add(textStack);
            this.Children.Add(inputStack);
        }

        /// <summary>
        /// 填充控件
        /// </summary>
        /// <param name="ca"></param>
        public void AddArea(CustomArea ca)
        {
            textStack.Children.Add(ca.Label);
            inputStack.Children.Add(ca.InputControl);
            CustomAreas.Add(ca);
        }

        /// <summary>
        /// 清空控件
        /// </summary>
        public void ClearChildren()
        {
            CustomAreas.Clear();
            textStack.Children.Clear();
            inputStack.Children.Clear();
        }

        /// <summary>
        /// 根据控件集合填充流面板
        /// </summary>
        public void ResetChildrenContent()
        {
            for (int i = 0; i < CustomAreas.Count; i++)
            {
                SetChildrenContentValue(i, CustomAreas[i].config.Default);
            }
        }

        /// <summary>
        /// 根据控件集合顺序填充控件内容
        /// </summary>
        /// <param name="index">控件所在集合CustomAreas位置</param>
        /// <param name="newVal">填充内容</param>
        public void SetChildrenContentValue(int index, string newVal)
        {
            if (CustomAreas[index].config.Type.Equals("input")) (CustomAreas[index] as InputArea).Content = newVal;
            else if (CustomAreas[index].config.Type.Equals("richinput")) (CustomAreas[index] as RichInputArea).Content = newVal;
            else if (CustomAreas[index].config.Type.Equals("autoinput")) (CustomAreas[index] as AutoInputArea).Content = newVal;
            else if (CustomAreas[index].config.Type.Equals("json")) (CustomAreas[index] as CodeInputArea).Content = newVal;
        }

        /// <summary>
        /// 根据配置ID填充控件内容
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <param name="newVal">填充内容</param>
        public void SetChildrenContentValue(string id, string newVal)
        {
            for (int i = 0; i < CustomAreas.Count; i++)
            {
                if (CustomAreas[i].Id == id)
                {
                    SetChildrenContentValue(i, newVal);
                    return;
                }
            }
        }
    }
}
