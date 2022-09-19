using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data
{
    public class TemplateResult
    {
        public string TemplateFileName { get; set; }
        public string PrintType { get; set; }
        public List<TemplateResultItem> ResultItems { get; set; }

        public TemplateResult()
        {
            ResultItems = new List<TemplateResultItem>();
        }

        public TemplateResult(string fileName,string printType)
        {
            ResultItems = new List<TemplateResultItem>();
            this.TemplateFileName = fileName;
            this.PrintType = printType;
        }
    }

    public class TemplateResultItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
}
