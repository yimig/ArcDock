using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using ArcDock.Data.Json;

namespace ArcDock.Data
{
    /// <summary>
    /// 结构化文本
    /// </summary>
    public class StructuredText
    {
        #region 属性字段和事件
        /// <summary>
        /// 文本块集合，默认顺序就是组装顺序
        /// </summary>
        private List<StructuredTextPiece> textList;
        /// <summary>
        /// 预留项配置
        /// </summary>
        private List<ConfigItem> configItems;
        /// <summary>
        /// 将文本结构化或文本化
        /// </summary>
        public string FullText
        {
            get => GetFullText();
            set => SetFullText(value);
        }

        public override string ToString()
        {
            return GetFullText();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 获取和设置模板内容
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <returns>模板内容</returns>
        public string this[string id]
        {
            get => GetTemplateContent(id);
            set => SetTemplateContent(id, value);
        }

        public StructuredText(List<ConfigItem> configItems)
        {
            textList = new List<StructuredTextPiece>();
            this.configItems = configItems;
        }

        public StructuredText(List<ConfigItem> configItems, string text)
        {
            textList = new List<StructuredTextPiece>();
            this.configItems = configItems;
            SetFullText(text);
        }

        private void AddText(string text)
        {
            this.textList.Add(new StructuredTextPiece(text));
        }

        private void AddTemplate(string id, string text)
        {
            this.textList.Add(new StructuredTextPiece(text, TextPieceType.Template) { Id = id });
        }

        #endregion

        #region 功能解耦

        private string GetTemplateContent(string id)
        {
            string res = "";
            try
            {
                res = textList.Find(piece => piece.Id.Equals(id)).Content;
            }
            catch (InvalidOperationException) { }

            return res;
        }

        private void SetTemplateContent(string id, string content)
        {
            try
            {
                foreach (var target in textList.FindAll(piece => piece.Id.Equals(id)))
                {
                    target.Content = content;
                }
            }
            catch (InvalidOperationException) { }
        }

        private string GetFullText()
        {
            string res = "";
            foreach (var piece in textList)
            {
                res += piece.Content;
            }

            return res;
        }

        private void SetFullText(string text)
        {
            foreach (var subText in text.Split(new string[] { "{{", "}}" }, StringSplitOptions.None))
            {
                try
                {
                    var template = configItems.Single(item => item.Id.Equals(subText.Trim()));
                    AddTemplate(template.Id, "");
                }
                catch (InvalidOperationException)
                {
                    AddText(subText);
                }

            }
        }

        #endregion

    }
}
