using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcDock.Data.Json;
using ArcDock.Tools;

namespace ArcDock.Data
{
    public class SearchDataItem
    {
        private string text, pyHead;
        public string Text { get=>text; }
        public string PYHead { get=>pyHead; }
        public List<ExecutionItem> ExecutionItems { get; set; }

        public SearchDataItem(string text)
        {
            this.text = text;
            pyHead = PinyinHelpers.GetFirstSpell(text);
        }

        public SearchDataItem(string text,List<ExecutionItem> executionItems)
        {
            this.text = text;
            pyHead = PinyinHelpers.GetFirstSpell(text);
            this.ExecutionItems = executionItems;
        }


    }
}
