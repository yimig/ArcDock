using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ArcDock.Data
{
    public class PythonEnvironment
    {
        private static ScriptEngine pyEngine;
        private static string code;
        public static ScriptEngine GetPythonEngine()
        {
            if (pyEngine == null) new PythonEnvironment();
            return pyEngine;
        }
        public static string GetPythonCode()
        {
            if (code == null) new PythonEnvironment();
            return code;
        }

        public static Dictionary<string,string> RunPython(string code,string source)
        {
            var resDict = new Dictionary<string,string>();
            var scope = pyEngine.CreateScope();
            scope.SetVariable("source", source);
            scope.SetVariable("result", resDict);
            var script = pyEngine.CreateScriptSourceFromString(code);
            script.Execute(scope);
            return scope.GetVariable("result") as Dictionary<string,string>;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string connString = ConfigurationManager.ConnectionStrings["history"].ConnectionString;

        /// <summary>
        /// 数据库连接
        /// </summary>
        private SQLiteConnection conn;

        public PythonEnvironment()
        {
            conn = new SQLiteConnection(connString);
            PythonEnvironment.pyEngine = Python.CreateEngine();
            OpenDatabase();
            PythonEnvironment.code = GetCode();
        }

        private string GetCode()
        {
            var code = "";
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select * from code where id = 'string_analyze'";
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    code = sr.GetString(2);
                }
                sr.Close();
            }

            return code;
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        public void OpenDatabase()
        {
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS code(id varchar(255) primary key ,language varchar(255),code text);" +
                    "INSERT OR IGNORE INTO code(id,language,code) values ('string_analyze','python','');";
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateCode(string code)
        {
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "REPLACE INTO code(id,language,code) values ('string_analyze','python',@code);";
                cmd.Parameters.Add("code", DbType.String).Value = code;
                cmd.ExecuteNonQuery();
                PythonEnvironment.code = code;
            }
        }

        public void CloseDatabase()
        {
            conn.Close();
        }

    }
}
