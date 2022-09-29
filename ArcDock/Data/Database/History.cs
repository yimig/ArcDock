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
    /// <summary>
    /// 历史记录操作类
    /// </summary>
    public class History
    {
        #region 字段和属性

        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string connString = ConfigurationManager.ConnectionStrings["history"].ConnectionString;

        /// <summary>
        /// 数据库连接
        /// </summary>
        private SQLiteConnection conn;

        /// <summary>
        /// 每页最大条目数
        /// </summary>
        private const int PAGE_RANGE = 100;

        /// <summary>
        /// 根据每页最大条目数得出的最大页码
        /// </summary>
        public int MaxPage { get; set; }

        #endregion

        #region 初始化

        public History()
        {
            conn = new SQLiteConnection(connString);
            OpenDatabase();
            //InitMaxPageNum();
            InitSortMaxPageNum();
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
                    "Create table if not exists history(item_id int,template text,template_id text,template_name text,template_content text,print_type text,print_date datetime)";
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region 数据库操作

        /// <summary>
        /// 获取条目总数
        /// </summary>
        /// <returns>条目总数</returns>
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

        /// <summary>
        /// 初始化最大页码
        /// </summary>
        private void InitMaxPageNum()
        {
            var item_count = 0;
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select case when max(item_id) is null then 0 else max(item_id) end max_id from history;";
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    item_count = sr.GetInt32(0);
                }
                sr.Close();
            }

            if (item_count == 0)
            {
                MaxPage = 1;
                return;
            }
            MaxPage = item_count % PAGE_RANGE == 0 ? (item_count / PAGE_RANGE) : (item_count / PAGE_RANGE) + 1;
        }

        /// <summary>
        /// 初始化重排整理的最大页码
        /// </summary>
        private void InitSortMaxPageNum()
        {
            var item_count = 0;
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select max(item_id) from history;";
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    item_count = sr.GetInt32(0);
                }
                sr.Close();
            }

            if (item_count == 0)
            {
                MaxPage = 1;
                return;
            }
            MaxPage = item_count % PAGE_RANGE == 0 ? (item_count / PAGE_RANGE) : (item_count / PAGE_RANGE) + 1;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="itemId">项目ID</param>
        /// <param name="tempFileName">模板文件名</param>
        /// <param name="tempId">预留值ID</param>
        /// <param name="tempName">预留值友好名称</param>
        /// <param name="tempContent">预留值填充内容</param>
        /// <param name="printType">打印类型</param>
        /// <param name="printDate">打印日期</param>
        private void InsertData(int itemId, string tempFileName, string tempId, string tempName, string tempContent, string printType, DateTime printDate)
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

        /// <summary>
        /// 删除指定日期后的数据
        /// </summary>
        /// <param name="limitedDate">限制日期</param>
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

        /// <summary>
        /// 获取全部内容
        /// </summary>
        /// <param name="startRow">开始行数</param>
        /// <param name="range">获取行的数量</param>
        /// <returns>数据库查询值</returns>
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

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <param name="contentQuery">查询预留值填充内容</param>
        /// <returns>查询结果</returns>
        private List<DataResult> SelectData(string contentQuery)
        {
            var resList = new List<DataResult>();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select item_id,template,template_id,template_name,template_content,print_type,print_date from history where template_content like @content order by ROWID desc;";
                cmd.Parameters.Add("content", DbType.String).Value = "%" + contentQuery + "%";
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

        /// <summary>
        /// 获取重排整理的内容
        /// </summary>
        /// <param name="startRow">开始行数</param>
        /// <param name="range">获取行的数量</param>
        /// <returns></returns>
        private List<SortResult> SelectSortData(int startRow, int range)
        {
            var resList = new List<SortResult>();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select item_id," +
                                  "       group_concat(patient_no, '') patient_no," +
                                  "       group_concat(patient_name, '') patient_name," +
                                  "       group_concat(medicament_name, '') medicament_name," +
                                  "       group_concat(medicament_num, '') medicament_num," +
                                  "       group_concat(patient_dept, '') patient_dept," +
                                  "       group_concat(patient_bed, '') patient_bed," +
                                  "       min(print_date) print_date " +
                                  "from (select item_id," +
                                  "             case template_id when 'patient_name' then template_content else '' end 'patient_name'," +
                                  "             case template_id when 'patient_no' then template_content else '' end 'patient_no'," +
                                  "             case template_id when 'patient_bed' then template_content else '' end 'patient_bed'," +
                                  "             case template_id when 'patient_dept' then template_content else '' end 'patient_dept'," +
                                  "             case template_id when 'medicament_name' then template_content else '' end 'medicament_name'," +
                                  "             case template_id when 'medicament_num' then template_content else '' end 'medicament_num'," +
                                  "             print_date" +
                                  "      from history)" +
                                  "group by item_id limit @start_row,@range;";
                cmd.Parameters.Add("start_row", DbType.Int32).Value = startRow;
                cmd.Parameters.Add("range", DbType.Int32).Value = range;
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    var res = new SortResult();
                    res.ItemId = sr.GetInt32(0);
                    res.PatientNo = sr.GetString(1);
                    res.PatientName = sr.GetString(2);
                    res.MedicamentName = sr.GetString(3);
                    res.MedicamentNum = sr.GetString(4);
                    res.PatientDept = sr.GetString(5);
                    res.PatientBed = sr.GetString(6);
                    res.RawPrintDate = sr.GetDateTime(7);
                    resList.Add(res);
                }

                sr.Close();
            }

            return resList;
        }

        /// <summary>
        /// 获取重排整理的查询结果
        /// </summary>
        /// <param name="contentQuery">查询预留值填充内容</param>
        /// <returns>查询结果</returns>
        private List<SortResult> SelectSortData(string contentQuery)
        {
            var resList = new List<SortResult>();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select item_id," +
                                  "       group_concat(patient_no, '') patient_no," +
                                  "       group_concat(patient_name, '') patient_name," +
                                  "       group_concat(medicament_name, '') medicament_name," +
                                  "       group_concat(medicament_num, '') medicament_num," +
                                  "       group_concat(patient_dept, '') patient_dept," +
                                  "       group_concat(patient_bed, '') patient_bed," +
                                  "       min(print_date) print_date" +
                                  "from (select item_id," +
                                  "             case template_id when 'patient_name' then template_content else '' end 'patient_name'," +
                                  "             case template_id when 'patient_no' then template_content else '' end 'patient_no'," +
                                  "             case template_id when 'patient_bed' then template_content else '' end 'patient_bed'," +
                                  "             case template_id when 'patient_dept' then template_content else '' end 'patient_dept'," +
                                  "             case template_id when 'medicament_name' then template_content else '' end 'medicament_name'," +
                                  "             case template_id when 'medicament_num' then template_content else '' end 'medicament_num'," +
                                  "             print_date" +
                                  "      from history where template_content like @content)" +
                                  "group by item_id;";
                cmd.Parameters.Add("content", DbType.String).Value = "%" + contentQuery + "%";
                var sr = cmd.ExecuteReader();
                while (sr.Read())
                {
                    var res = new SortResult();
                    res.ItemId = sr.GetInt32(0);
                    res.PatientNo = sr.GetString(1);
                    res.PatientName = sr.GetString(2);
                    res.MedicamentName = sr.GetString(3);
                    res.MedicamentNum = sr.GetString(4);
                    res.PatientDept = sr.GetString(5);
                    res.PatientBed = sr.GetString(6);
                    res.RawPrintDate = sr.GetDateTime(7);
                    resList.Add(res);
                }

                sr.Close();
            }

            return resList;
        }

        /// <summary>
        /// 获取一条完整的项目信息
        /// </summary>
        /// <param name="ItemId">项目ID</param>
        /// <returns>项目信息集合</returns>
        public List<DataResult> GetFullItemData(int ItemId)
        {
            var resList = new List<DataResult>();
            if (conn.State == ConnectionState.Open)
            {
                var cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText =
                    "select item_id,template,template_id,template_name,template_content,print_type,print_date from history where item_id = @item_id";
                cmd.Parameters.Add("item_id", DbType.Int32).Value = ItemId;
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

        #endregion

        #region 公共方法解耦

        /// <summary>
        /// 添加一条历史记录
        /// </summary>
        /// <param name="result">记录条目</param>
        /// <param name="printDate">打印日期</param>
        public void AddHistory(HistoryResult result, DateTime printDate)
        {
            var itemId = GetMaxItemID();
            foreach (var res in result.ResultItems)
            {
                InsertData(itemId, result.TemplateFileName, res.Id, res.Name, res.Content, result.PrintType, printDate);
            }
        }

        /// <summary>
        /// 删除超期的历史
        /// </summary>
        public void RemoveOutDateHistory()
        {
            var limitedDate = DateTime.Now - TimeSpan.FromDays(365);
            DeleteData(limitedDate);
        }

        /// <summary>
        /// 获取指定页码的历史内容
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <returns>历史内容集合</returns>
        public List<DataResult> GetPage(int pageNo)
        {
            var resList = new List<DataResult>();
            if (pageNo > 0 && pageNo <= MaxPage)
            {
                resList = SelectData((pageNo - 1) * PAGE_RANGE, PAGE_RANGE);
            }

            return resList;
        }

        /// <summary>
        /// 获取指定页码的重排整理的历史内容
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <returns>历史内容集合</returns>
        public List<SortResult> GetSortPage(int pageNo)
        {
            var resList = new List<SortResult>();
            if (pageNo > 0 && pageNo <= MaxPage)
            {
                resList = SelectSortData((pageNo - 1) * PAGE_RANGE, PAGE_RANGE);
            }

            return resList;
        }

        /// <summary>
        /// 搜索历史
        /// </summary>
        /// <param name="contentQuery">关键字</param>
        /// <returns>历史内容集合</returns>
        public List<DataResult> GetQueryResult(string contentQuery)
        {
            return SelectData(contentQuery);
        }

        /// <summary>
        /// 搜索重排整理的历史记录
        /// </summary>
        /// <param name="contentQuery">关键字</param>
        /// <returns>历史内容集合</returns>

        public List<SortResult> GetSortQueryResult(string contentQuery)
        {
            return SelectSortData(contentQuery);
        }

        #endregion

        /* 重排序SQL
         * select item_id,
           group_concat(patient_no, '') patient_no,
           group_concat(patient_name, '') patient_name,
           group_concat(medicament_name, '') medicament_name,
           group_concat(medicament_num, '') medicament_num,
           group_concat(patient_dept, '') patient_dept,
           group_concat(patient_bed, '') patient_bed,
           min(print_date) print_date
           from (select item_id,
           case template_id when 'patient_name' then template_content else '' end 'patient_name',
           case template_id when 'patient_no' then template_content else '' end 'patient_no',
           case template_id when 'patient_bed' then template_content else '' end 'patient_bed',
           case template_id when 'patient_dept' then template_content else '' end 'patient_dept',
           case template_id when 'medicament_name' then template_content else '' end 'medicament_name',
           case template_id when 'medicament_num' then template_content else '' end 'medicament_num',
           print_date
           from history)
           group by item_id;
         */

    }
}
