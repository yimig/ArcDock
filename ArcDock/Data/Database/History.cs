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

        public History()
        {
            conn = new SQLiteConnection(connString);
            OpenDatabase();
        }

        public void OpenDatabase()
        {
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "Create table if not exists history(id INTEGER primary key autoincrement,item_id int,template text,template_id text,template_name text,template_content text,print_type text,print_date datetime)";
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
    }
}
