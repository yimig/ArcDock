using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;
using System.Data;

namespace ArcDock.Data.Database
{
    public class History
    {
        private static string connString = ConfigurationManager.ConnectionStrings["history"].ConnectionString;
        private SQLiteConnection conn;
        private const int PAGE_RANGE = 100;
        public int MaxPage { get; set; }

        public History()
        {
            conn = new SQLiteConnection(connString);
            OpenDatabase();
            InitMaxPageNum();
        }

        public void OpenDatabase()
        {
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "Create table if not exists history(item_id int,template text,template_id text,template_name text,template_content text,print_type text,print_date datetime)";
                cmd.ExecuteNonQuery();
            }
        }

        private int GetMaxItemID()
        {
            var id = 0;
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select case when max(item_id) is null then 0 else max(item_id) end max_id from history";
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    id = sr.GetInt32(0);
                }
                sr.Close();
            }

            return id + 1;
        }

        private void InitMaxPageNum()
        {
            var item_count = 0;
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select count(*) from history;";
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    item_count = sr.GetInt32(0);
                }
                sr.Close();
            }

            MaxPage = item_count == 0 ? 1 : (item_count / PAGE_RANGE) + 1;
        }

        private void InsertData(int itemId,string tempFileName,string tempId,string tempName,string tempContent,string printType,DateTime printDate)
        {
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "insert into history(item_id, template, template_id, template_name, template_content, print_type, print_date) values (@item_id, @template, @template_id, @template_name, @template_content, @print_type, @print_date);";
                cmd.Parameters.Add("item_id", DbType.Int32).Value = itemId;
                cmd.Parameters.Add("template", DbType.String).Value = tempFileName;
                cmd.Parameters.Add("template_id", DbType.String).Value = tempId;
                cmd.Parameters.Add("template_name", DbType.String).Value = tempName;
                cmd.Parameters.Add("template_content", DbType.String).Value = tempContent;
                cmd.Parameters.Add("print_type", DbType.String).Value = printType;
                cmd.Parameters.Add("print_date", DbType.DateTime).Value = printDate;
                cmd.ExecuteNonQuery();
            }
        }

        public void AddHistory(TemplateResult result, DateTime printDate)
        {
            var itemId = GetMaxItemID();
            foreach (var res in result.ResultItems)
            {
                InsertData(itemId,result.TemplateFileName,res.Id,res.Name,res.Content,result.PrintType,printDate);
            }
        }

        private void DeleteData(DateTime limitedDate)
        {
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "delete from history where print_date<@print_date";
                cmd.Parameters.Add("print_date", DbType.DateTime).Value = limitedDate;
                cmd.ExecuteNonQuery();
            }
        }

        public void RemoveOutDateHistory()
        {
            var limitedDate = DateTime.Now - TimeSpan.FromDays(365);
            DeleteData(limitedDate);
        }

        private List<DataResult> SelectData(int startRow, int range)
        {
            var resList = new List<DataResult>();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select item_id,template,template_id,template_name,template_content,print_type,print_date from history order by ROWID desc limit @start_row,@range;";
                cmd.Parameters.Add("start_row", DbType.Int32).Value = startRow;
                cmd.Parameters.Add("range", DbType.Int32).Value = range;
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    var res = new DataResult();
                    res.ItemId = sr.GetInt32(0);
                    res.TemplateFileName = sr.GetString(1);
                    res.TemplateId = sr.GetString(2);
                    res.TemplateName = sr.GetString(3);
                    res.TemplateContent = sr.GetString(4);
                    res.PrintType = sr.GetString(5);
                    res.RawDatetime = sr.GetDateTime(6);
                    resList.Add(res);
                }

                sr.Close();
            }

            return resList;
        }

        private List<DataResult> SelectData(string contentQuery)
        {
            var resList = new List<DataResult>();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select item_id,template,template_id,template_name,template_content,print_type,print_date from history where template_content like @content order by ROWID desc;";
                cmd.Parameters.Add("content", DbType.String).Value = "%"+contentQuery+"%";
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    var res = new DataResult();
                    res.ItemId = sr.GetInt32(0);
                    res.TemplateFileName = sr.GetString(1);
                    res.TemplateId = sr.GetString(2);
                    res.TemplateName = sr.GetString(3);
                    res.TemplateContent = sr.GetString(4);
                    res.PrintType = sr.GetString(5);
                    res.RawDatetime = sr.GetDateTime(6);
                    resList.Add(res);
                }
                sr.Close();
            }

            return resList;
        }

        public List<DataResult> GetPage(int pageNo)
        {
            var resList = new List<DataResult>();
            if (pageNo > 0 && pageNo <= MaxPage)
            {
                resList = SelectData((pageNo - 1) * PAGE_RANGE, PAGE_RANGE);
            }

            return resList;
        }

        public List<DataResult> GetQueryResult(string contentQuery)
        {
            return SelectData(contentQuery);
        }
    }
}
