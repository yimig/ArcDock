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
    /// HTML结构化文本
    /// </summary>
    public class StructuredText
    {
        #region 属性字段和事件
        /// <summary>
        /// 文本块集合，默认顺序就是组装顺序
        /// </summary>
        private List<StructuredTextItem> textList;

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

        #endregion

        #region 初始化

        /// <summary>
        /// 获取和设置预留值内容
        /// </summary>
        /// <param name="id">预留值ID</param>
        /// <returns>预留值内容</returns>
        public string this[string id]
        {
            get => GetTemplateContent(id);
            set => SetTemplateContent(id, value);
        }

        /// <summary>
        /// 初始化一个结构化文档
        /// </summary>
        /// <param name="configItems">配置项</param>
        public StructuredText(List<ConfigItem> configItems)
        {
            textList = new List<StructuredTextItem>();
            this.configItems = configItems;
        }

        /// <summary>
        /// 初始化文档的同时载入文档
        /// </summary>
        /// <param name="configItems">配置项</param>
        /// <param name="text">文档内容</param>
        public StructuredText(List<ConfigItem> configItems, string text)
        {
            textList = new List<StructuredTextItem>();
            this.configItems = configItems;
            SetFullText(text);
        }

        public override string ToString()
        {
            return GetFullText();
        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 向结构化文档追加一段纯文本
        /// </summary>
        /// <param name="text">文本内容</param>
        private void AddText(string text)
        {
            this.textList.Add(new StructuredTextItem(text));
        }

        /// <summary>
        /// 向结构化文档追加一个预留值模板
        /// </summary>
        /// <param name="id">预留值ID</param>
        /// <param name="text">预留值内容</param>
        private void AddTemplate(string id, string text)
        {
            this.textList.Add(new StructuredTextItem(text, TextPieceType.Template) { Id = id });
        }

        /// <summary>
        /// 获取结构化文档中预留值的内容
        /// </summary>
        /// <param name="id">预留值ID</param>
        /// <returns>预留值内容</returns>
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

        /// <summary>
        /// 设置结构化文本中预留值的内容
        /// </summary>
        /// <param name="id">预留值ID</param>
        /// <param name="content">预留值内容</param>
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

        /// <summary>
        /// 以文本形式获取整个文档
        /// </summary>
        /// <returns>文档文本</returns>
        private string GetFullText()
        {
            string res = "";
            foreach (var piece in textList)
            {
                res += piece.Content;
            }

            return res;
        }

        /// <summary>
        /// 载入并分析文本，将文本结构化裁切为文本块
        /// </summary>
        /// <param name="text">源文本</param>
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
