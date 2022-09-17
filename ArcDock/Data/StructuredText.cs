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
    public class StructuredText
    {
        private List<StructuredTextPiece> textList;
        private List<ConfigItem> configItems;

        public string FullText
        {
            get => GetFullText();
            set => SetFullText(value);
        }

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

        public StructuredText(List<ConfigItem> configItems,string text)
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
            this.textList.Add(new StructuredTextPiece(text,TextPieceType.Template){Id = id});
        }

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
            catch(InvalidOperationException) {}
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
                    var template = configItems.Single(item => item.Id.Equals(subText));
                    AddTemplate(template.Id, "");
                }
                catch (InvalidOperationException)
                {
                    AddText(subText);
                }

            }
        }

        public override string ToString()
        {
            return GetFullText();
        }
    }
}
