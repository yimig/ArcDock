using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data.Json
{
    public class FlowTable: IEnumerable
    {
        private string title;
        private List<string> content;

        public string Title
        {
            get => title;
        }

        public string this[int index]
        {
            get
            {
                return content[index];
            }
        }

        public FlowTable(List<string> data, bool isFirstLineHeader)
        {
            this.content = new List<string>();
            if (isFirstLineHeader)
            {
                this.content = this.content.Concat(data).ToList();
            } 
            else {
                this.title = data[0];
                this.content = this.content.Concat(data.Skip(1)).ToList();
            }
        }

        public FlowTable(string flowStr)
        {
            this.content = new List<string>();
            var bodyStr = flowStr.Split('@');
            this.title = bodyStr[0];
            this.content = this.content.Concat(bodyStr[1].Split('|')).ToList();
        }

        public override string ToString()
        {
            var res = this.title + '@';
            foreach(string data in this.content)
            {
                res += data + '|';
            }
            return res.Last() == '|' ? res.Remove(res.Length - 1, 1) : res;
        }

        public IEnumerator GetEnumerator()
        {
            return this.content.GetEnumerator();
        }
    }
}
