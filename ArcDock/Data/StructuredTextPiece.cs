using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data
{
    /// <summary>
    /// 文本块类
    /// </summary>
    public class StructuredTextPiece
    {
        /// <summary>
        /// 文本块内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 文本块类型
        /// </summary>
        public TextPieceType Type { get; set; }
        /// <summary>
        /// 文本块类型为Template时，Id为预留值ID
        /// 文本块类型为Text时，Id为预留值空串
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 以文本模式初始化文本块
        /// </summary>
        /// <param name="content">文本块内容</param>
        public StructuredTextPiece(string content)
        {
            this.Id = "";
            this.Content = content;
            this.Type = TextPieceType.Text;
        }

        /// <summary>
        /// 初始化文本块
        /// </summary>
        /// <param name="content">文本块内容</param>
        /// <param name="type">文本块类型</param>
        public StructuredTextPiece(string content, TextPieceType type)
        {
            this.Id = "";
            this.Content = content;
            this.Type = type;
        }
    }

    /// <summary>
    /// 文本块类型
    /// </summary>
    public enum TextPieceType
    {
        /// <summary>
        /// 文本类型
        /// </summary>
        Text,
        /// <summary>
        /// 模板类型
        /// </summary>
        Template
    }
}
