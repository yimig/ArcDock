using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace ArcDock.Data
{
    /// <summary>
    /// Python辅助类
    /// </summary>
    public class PythonEnvironment
    {
        private static ScriptEngine pyEngine;
        private static string code;

        /// <summary>
        /// 获得一个Python解释器实例
        /// </summary>
        /// <returns></returns>
        public static ScriptEngine GetPythonEngine()
        {
            if (pyEngine == null) new PythonEnvironment();
            return pyEngine;
        }

        /// <summary>
        /// 读取存储的Python代码
        /// </summary>
        /// <returns></returns>
        public static string GetPythonCode()
        {
            if (code == null) new PythonEnvironment();
            return code;
        }

        /// <summary>
        /// 运行Python进行字符串处理
        /// </summary>
        /// <param name="code">Python代码</param>
        /// <param name="source">原始字符串</param>
        /// <returns>结果字典</returns>
        public static Dictionary<string, string> RunPython(string code, string source)
        {
            var resDict = new Dictionary<string, string>();
            var scope = pyEngine.CreateScope();
            scope.SetVariable("source", source);
            scope.SetVariable("result", resDict);
            var script = pyEngine.CreateScriptSourceFromString(code);
            script.Execute(scope);
            return scope.GetVariable("result") as Dictionary<string, string>;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string connString = ConfigurationManager.ConnectionStrings["history"].ConnectionString;

        /// <summary>
        /// 数据库连接
        /// </summary>
        private SQLiteConnection conn;

        /// <summary>
        /// 连接数据库以获得Python代码
        /// </summary>
        public PythonEnvironment()
        {
            conn = new SQLiteConnection(connString);
            PythonEnvironment.pyEngine = Python.CreateEngine();
            OpenDatabase();
            PythonEnvironment.code = GetCode();
        }

        /// <summary>
        /// 获得数据库存储的Python代码
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 更新数据库存储的Python代码
        /// </summary>
        /// <param name="code"></param>
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

        /// <summary>
        /// 手动关闭数据库连接
        /// </summary>
        public void CloseDatabase()
        {
            conn.Close();
        }

    }
}
