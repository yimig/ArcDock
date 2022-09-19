using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data
{
    public class DataResult
    {
        private DateTime rawDateTime;
        public DateTime RawDatetime
        {
            set => rawDateTime = value;
        }

        public int ItemId { get; set; }

        public string TemplateFileName { get; set; }

        public string TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string TemplateContent { get; set; }

        public string PrintType { get; set; }

        public string PrintDate => rawDateTime.ToString();
    }
}
