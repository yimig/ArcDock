using log4net;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArcDock.Data
{
    public class ExcelReader
    {
        private static ILog log = LogManager.GetLogger("ExcelReader");
        public static Dictionary<string,List<string>> ReadXlsx(string filePath)
        {
            var dict = new Dictionary<string,List<string>>();
            using(var fs = new FileStream(filePath,FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook wb = new XSSFWorkbook(fs);
                var sheet = wb.GetSheetAt(0);
                if (sheet != null && sheet.LastRowNum > 1)
                {
                    var headerRow = sheet.GetRow(0);
                    for(var i = 0;i< headerRow.LastCellNum; i++)
                    {
                        var list = new List<string>();
                        for(var j = 1; j< sheet.LastRowNum; j++)
                        {
                            try
                            {
                                list.Add(sheet.GetRow(j).GetCell(i).ToString());
                            } catch(Exception ex)
                            {
                                list.Add("");
                                MessageBox.Show("单元格" + (j + 1) + "行," + (i + 1) + "列解析失败："+ ex.Message);
                                log.Error("文件【" + filePath + "】中单元格" + (j + 1) + "行," + (i + 1) + "列解析失败：", ex);
                            }

                        }
                        dict.Add(headerRow.GetCell(i).ToString(), list);
                    }
                }
            }
            return dict;
        }
    }
}
