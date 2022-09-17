using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data
{
    public class StructuredTextPiece
    {
        public string Content { get; set; }
        public TextPieceType Type { get; set; }

        public string Id { get; set; }

        public StructuredTextPiece(string content)
        {
            this.Id = "";
            this.Content = content;
            this.Type = TextPieceType.Text;
        }

        public StructuredTextPiece(string content, TextPieceType type)
        {
            this.Id = "";
            this.Content = content;
            this.Type = type;
        }
    }

    public enum TextPieceType
    {
        Text,Template
    }
}
