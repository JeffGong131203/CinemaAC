using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ACAdapter;
using System.Data.SqlClient;

namespace ExtractDataService
{
    public partial class Service1 : ServiceBase
    {
        private string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\DataFile.txt";

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service start...");

            System.Timers.Timer t = new System.Timers.Timer();
            t.Enabled = true;
            t.Interval = double.Parse(System.Configuration.ConfigurationManager.AppSettings["DataInt"]);

            t.Elapsed += T_Elapsed;
        }


        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                GetData();

                InsertDataToDB();
            }
            catch(Exception ex)
            {
                WriteLog("Error:" + ex.Message);
            }
        }

        protected override void OnStop()
        {
            WriteLog("Service stop...");
        }

        private void GetData()
        {
            string ip = System.Configuration.ConfigurationManager.AppSettings["ip"];

            ACAdapter.ACAdapter acd = new ACAdapter.ACAdapter(ip, filePath);

            WriteLog("GetData...");

            int ret = acd.GetState();

            WriteLog("Return Value:" + ret.ToString());
        }

        private void InsertDataToDB()
        {
            DataTable dt = new DataTable();
            dt.TableName = "tb_ACData";
            dt.Columns.Add("ID", Type.GetType("System.Guid"));
            dt.Columns.Add("UpdateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("DType");
            dt.Columns.Add("DID");
            dt.Columns.Add("Col1");
            dt.Columns.Add("Col2");
            dt.Columns.Add("Col3");
            dt.Columns.Add("Col4");
            dt.Columns.Add("Col5");
            dt.Columns.Add("Col6");
            dt.Columns.Add("Col7");
            dt.Columns.Add("Col8");
            dt.Columns.Add("Col9");
            dt.Columns.Add("Col10");

            string strFile = string.Empty;

            //判断是否存在 
            if (System.IO.File.Exists(filePath))
            {
                //读取text 
                strFile = System.IO.File.ReadAllText(filePath, Encoding.GetEncoding("gb2312"));
            }

            if (!string.IsNullOrEmpty(strFile))
            {
                string[] strData = strFile.Split(";".ToCharArray());

                foreach (string sinData in strData)
                {
                    if (!string.IsNullOrEmpty(sinData))
                    {
                        string[] dData = sinData.Split(":".ToCharArray());

                        if (dData.Length > 1)
                        {
                            DataRow dr = dt.NewRow();
                            dr["ID"] = Guid.NewGuid();
                            dr["UpdateTime"] = DateTime.Now;
                            dr["DType"] = dData[0].Substring(0, 1);
                            dr["DID"] = dData[0].Substring(1);

                            string[] sData = dData[1].Split(",".ToCharArray());

                            for (int i = 0; i < sData.Length; i++)
                            {
                                dr["Col" + (i + 1).ToString()] = sData[i];
                            }

                            dt.Rows.Add(dr);
                        }
                    }

                }
            }

            if(dt.Rows.Count>0)
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["SqlConn"]))
                {
                    conn.Open();

                    using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
                    {
                        //一次批量的插入的数据量
                        sqlBC.BatchSize = 1000;
                        //超时之前操作完成所允许的秒数，如果超时则事务不会提交 ，数据将回滚，所有已复制的行都会从目标表中移除
                        sqlBC.BulkCopyTimeout = 600;

                        //設定 NotifyAfter 属性，以便在每插入10000 条数据时，呼叫相应事件。  
                        sqlBC.NotifyAfter = 10000;
                        sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

                        //设置要批量写入的表
                        sqlBC.DestinationTableName = dt.TableName;

                        //自定义的datatable和数据库的字段进行对应
                        //sqlBC.ColumnMappings.Add("id", "tel");
                        //sqlBC.ColumnMappings.Add("name", "neirong");

                        //批量写入
                        sqlBC.WriteToServer(dt);
                    }
                }
            }
        }

        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            //MyLog.WriteLog(string.Format("{0} rows has instered", e.RowsCopied));
        }

        private void WriteLog(string logText)
        {
            using (StreamWriter sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + DateTime.Now.ToString("yyyyMM") + ".txt", true, Encoding.Default))
            {
                sw.WriteLine(DateTime.Now.ToString() + ":" + logText);
            }
        }
    }
}
