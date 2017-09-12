using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ACAdapter;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace CinemaAC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string menuID)
        {
            if (!string.IsNullOrEmpty(menuID))
            {
                ViewBag.DType = menuID;
            }
            else
            {
                ViewBag.DType = "Err";
            }

            return View();

            //return Content(clientData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="switcher">S1_CHK1</param>
        /// <param name="setValue"></param>
        /// <returns></returns>
        public ActionResult SetState(string switcher, string setValue)
        {
            string path = Server.MapPath("~/ZClient/DataFile.txt");

            string ip = System.Configuration.ConfigurationManager.AppSettings["ip"];

            ACAdapter.ACAdapter acd = new ACAdapter.ACAdapter(ip, path);

            string dType = switcher.Substring(0, 1).ToUpper();
            int dID = int.Parse(switcher.Substring(1, switcher.IndexOf("_") - 1));
            string setType = switcher.Substring(switcher.IndexOf("_") + 1).ToUpper();

            switch (dType)
            {
                case "S":
                    if (setType == "CHK1") //风机状态
                    {
                        acd.SetS2(dID, Convert.ToSingle(bool.Parse(setValue)));
                    }

                    if (setType == "CHK2") //冷热状态
                    {
                        acd.SetS3(dID, Convert.ToSingle(bool.Parse(setValue)));
                    }

                    if (setType == "SEL") //设定温度
                    {
                        acd.SetS1(dID, Convert.ToSingle(setValue));
                    }

                    break;
                case "H":
                    if (setType == "SEL1") //设定温度
                    {
                        acd.SetH1(dID, Convert.ToSingle(setValue));
                    }

                    if (setType == "CHK") //温控开关
                    {
                        acd.SetH2(dID, Convert.ToSingle(bool.Parse(setValue)));
                    }

                    if (setType == "SEL2") //风扇状态
                    {
                        acd.SetH3(dID, Convert.ToSingle(setValue));
                    }

                    if (setType == "SEL3") //系统模式
                    {
                        acd.SetH4(dID, Convert.ToSingle(setValue));
                    }

                    break;
            }

            Thread.Sleep(5000);

            GetClinetFile();

            if (dType == "S")
            {
                return RedirectToAction("S", "Home");
            }
            else if (dType == "H")
            {
                return RedirectToAction("H", "Home");
            }

            return RedirectToAction("S", "Home");

        }

        //public ActionResult Cinema(string menuID)
        //{
        //    if (!string.IsNullOrEmpty(menuID))
        //    {
        //        string clientData = GetClinetFile();

        //        string[] strData = clientData.Split(";".ToCharArray());

        //        ArrayList arrS = new ArrayList();

        //        foreach (string s in strData)
        //        {
        //            if (!string.IsNullOrEmpty(s))
        //            {
        //                if (s.Substring(0, 1).ToUpper() == menuID.ToUpper())
        //                {
        //                    arrS.Add(s);
        //                }
        //            }
        //        }

        //        ViewBag.arrData = arrS;
        //        ViewBag.DType = menuID;

        //        return View(menuID);
        //    }
        //    else
        //    {
        //        return Content("Error MenuID");
        //    }
        //}

        public ActionResult S()
        {
            string clientData = GetClinetFile();

            string[] strData = clientData.Split(";".ToCharArray());

            ArrayList arrS = new ArrayList();

            foreach (string s in strData)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (s.Substring(0, 1).ToUpper() == "S")
                    {
                        arrS.Add(s);
                    }
                }
            }

            ViewBag.arrData = arrS;
            ViewBag.DType = "S";

            return View();

        }

        public ActionResult H()
        {
            string clientData = GetClinetFile();

            string[] strData = clientData.Split(";".ToCharArray());

            ArrayList arrS = new ArrayList();

            foreach (string s in strData)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (s.Substring(0, 1).ToUpper() == "H")
                    {
                        arrS.Add(s);
                    }
                }
            }

            ViewBag.arrData = arrS;
            ViewBag.DType = "H";

            return View();

        }

        public ActionResult ChartsD()
        {
            DataTable dt = new DataTable();
            DataTable dtDay = new DataTable();
            DataTable dtHour = new DataTable();

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["SqlConn"]))
            {
                conn.Open();

                string sql = "SELECT top 100 [UpdateTime],[DID],[Col1] FROM [ACCinema].[dbo].[tb_ACData] where dtype='D' order by [UpdateTime] desc,[DID]";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

                sql = "select top 10 Convert(varchar(100),[UpdateTime],23) as 'UpdateTime',max(Col2) as 'Col2' from [dbo].[tb_ACData] where DType='D' group by Convert(varchar(100),[UpdateTime],23) Order by Convert(varchar(100),[UpdateTime],23) desc";
                cmd = new SqlCommand(sql, conn);
                sda = new SqlDataAdapter(cmd);
                sda.Fill(dtDay);

                sql = "select top 100 Convert(varchar(13),[UpdateTime],120) as 'UpdateTime',max(Col2) as 'Col2' from [dbo].[tb_ACData] where DType='D' group by Convert(varchar(13),[UpdateTime],120) Order by Convert(varchar(13),[UpdateTime],120) desc";
                cmd = new SqlCommand(sql, conn);
                sda = new SqlDataAdapter(cmd);
                sda.Fill(dtHour);


            }

            //ViewBag.dData = dt;
            DataView dv = dt.DefaultView;
            DataTable dtDate = dv.ToTable(true, "UpdateTime");
            DataTable dtDid = dv.ToTable(true, "DID");

            string[] arrayX = new string[dtDate.Rows.Count];
            for (int i = 0; i < dtDate.Rows.Count; i++)
            {
                arrayX[i] = DateTime.Parse(dtDate.Rows[i][0].ToString()).ToString("yyyy-MM-dd hh:mm:ss");
            }
            Array.Reverse(arrayX);

            Dictionary<string, string[]> dicData = new Dictionary<string, string[]>();

            foreach (DataRow dr in dtDid.Rows)
            {
                string did = Convert.ToString(dr[0]);

                DataRow[] arrDr = dt.Select(string.Format("DID='{0}'", did));
                string[] arrayD = new string[arrDr.Length];
                for (int i = 0; i < arrDr.Length; i++)
                {
                    arrayD[i] = Convert.ToString(arrDr[i]["Col1"]);
                }
                Array.Reverse(arrayD);

                dicData.Add(did, arrayD);
            }

            ViewBag.arrayX = Newtonsoft.Json.JsonConvert.SerializeObject(arrayX);
            ViewBag.dicData = dicData;

            //Day Value
            string[] arrayXDay = new string[dtDay.Rows.Count-1];
            string[] arrayDayValue = new string[dtDay.Rows.Count-1];
            if (dtDay.Rows.Count > 1)
            {
                for (int i = 1; i < dtDay.Rows.Count; i++)
                {
                    arrayXDay[i-1] = Convert.ToString(dtDay.Rows[i][0]);
                    arrayDayValue[i-1] = Convert.ToString(Convert.ToDecimal(dtDay.Rows[i - 1][1]) - Convert.ToDecimal(dtDay.Rows[i][1]));
                }                     

                Array.Reverse(arrayXDay);
                Array.Reverse(arrayDayValue);
            }

            ViewBag.arrayXDay = Newtonsoft.Json.JsonConvert.SerializeObject(arrayXDay);
            ViewBag.arrayDayValue = Newtonsoft.Json.JsonConvert.SerializeObject(arrayDayValue);

            //Hour Value
            string[] arrayXHour = new string[dtHour.Rows.Count-1];
            string[] arrayHourValue = new string[dtHour.Rows.Count-1];
            if (dtHour.Rows.Count > 1)
            {
                for (int i = 1; i < dtHour.Rows.Count; i++)
                {
                    arrayXHour[i-1] = Convert.ToString(dtHour.Rows[i][0]);
                    arrayHourValue[i-1] = Convert.ToString(Convert.ToDecimal(dtHour.Rows[i - 1][1]) - Convert.ToDecimal(dtHour.Rows[i][1]));
                }
                Array.Reverse(arrayXHour);
                Array.Reverse(arrayHourValue);
            }

            ViewBag.arrayXHour = Newtonsoft.Json.JsonConvert.SerializeObject(arrayXHour);
            ViewBag.arrayHourValue = Newtonsoft.Json.JsonConvert.SerializeObject(arrayHourValue);

            //Get Interface File
            string clientData = GetClinetFile();

            string[] strData = clientData.Split(";".ToCharArray());

            ArrayList arrS = new ArrayList();

            foreach (string s in strData)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (s.Substring(0, 1).ToUpper() == "D")
                    {
                        arrS.Add(s);
                    }
                }
            }

            //string cv = arrS[0].ToString().Split(",".ToCharArray())[0];


            ViewBag.arrData = arrS;
            ViewBag.DType = "D";

            return View();
        }

        public ActionResult ChartsS()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["SqlConn"]))
            {
                conn.Open();

                string sql = "SELECT top 100 [UpdateTime],[DID],[Col4] FROM [ACCinema].[dbo].[tb_ACData] where dtype='S' order by [UpdateTime] desc,[DID]";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                sda.Fill(dt);
            }

            //ViewBag.dData = dt;
            DataView dv = dt.DefaultView;
            DataTable dtDate = dv.ToTable(true, "UpdateTime");
            DataTable dtDid = dv.ToTable(true, "DID");

            string[] arrayX = new string[dtDate.Rows.Count];
            for (int i = 0; i < dtDate.Rows.Count; i++)
            {
                arrayX[i] = DateTime.Parse(dtDate.Rows[i][0].ToString()).ToString("yyyy-MM-dd hh:mm:ss");
            }
            Array.Reverse(arrayX);

            Dictionary<string, string[]> dicData = new Dictionary<string, string[]>();

            foreach (DataRow dr in dtDid.Rows)
            {
                string did = Convert.ToString(dr[0]);

                DataRow[] arrDr = dt.Select(string.Format("DID='{0}'", did));
                string[] arrayD = new string[arrDr.Length];
                for (int i = 0; i < arrDr.Length; i++)
                {
                    arrayD[i] = Convert.ToString(arrDr[i]["Col4"]);
                }
                Array.Reverse(arrayD);

                dicData.Add(did, arrayD);
            }

            ViewBag.arrayX = Newtonsoft.Json.JsonConvert.SerializeObject(arrayX);
            ViewBag.dicData = dicData;

            return View();
        }

        public ActionResult ChartsH()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["SqlConn"]))
            {
                conn.Open();

                string sql = "SELECT top 100 [UpdateTime],[DID],[Col2] FROM [ACCinema].[dbo].[tb_ACData] where dtype='H' order by [UpdateTime] desc,[DID]";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                sda.Fill(dt);
            }

            //ViewBag.dData = dt;
            DataView dv = dt.DefaultView;
            DataTable dtDate = dv.ToTable(true, "UpdateTime");
            DataTable dtDid = dv.ToTable(true, "DID");

            string[] arrayX = new string[dtDate.Rows.Count];
            for (int i = 0; i < dtDate.Rows.Count; i++)
            {
                arrayX[i] = DateTime.Parse(dtDate.Rows[i][0].ToString()).ToString("yyyy-MM-dd hh:mm:ss");
            }
            Array.Reverse(arrayX);

            Dictionary<string, string[]> dicData = new Dictionary<string, string[]>();

            foreach (DataRow dr in dtDid.Rows)
            {
                string did = Convert.ToString(dr[0]);

                DataRow[] arrDr = dt.Select(string.Format("DID='{0}'", did));
                string[] arrayD = new string[arrDr.Length];
                for (int i = 0; i < arrDr.Length; i++)
                {
                    arrayD[i] = Convert.ToString(arrDr[i]["Col2"]);
                }
                Array.Reverse(arrayD);

                dicData.Add(did, arrayD);
            }

            ViewBag.arrayX = Newtonsoft.Json.JsonConvert.SerializeObject(arrayX);
            ViewBag.dicData = dicData;

            return View();
        }

        public ActionResult ChartsSH()
        {
            //Get Interface File
            string clientData = GetClinetFile();

            string[] strData = clientData.Split(";".ToCharArray());

            Dictionary<int, string[]> dicData = new Dictionary<int, string[]>();
            int i = 0;

            foreach (string s in strData)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (s.Substring(0, 2).ToUpper() == "S4" || s.Substring(0, 2).ToUpper() == "S5" || s.Substring(0, 2).ToUpper() == "S6")
                    {
                        string[] d = s.Split(",".ToCharArray());

                        dicData.Add(i, new string[] { d[0].Substring(0, d[0].IndexOf(":")), d[3] });

                        i++;
                    }

                    if (s.Substring(0, 3).ToUpper() == "H16" || s.Substring(0, 3).ToUpper() == "H17" || s.Substring(0, 3).ToUpper() == "H20" || s.Substring(0, 3).ToUpper() == "H21")
                    {
                        string[] d = s.Split(",".ToCharArray());

                        dicData.Add(i, new string[] { d[0].Substring(0, d[0].IndexOf(":")), d[1] });

                        i++;
                    }

                    if (s.Substring(0, 3).ToUpper() == "H18")
                    {
                        string[] d = s.Split(",".ToCharArray());

                        dicData.Add(i, new string[] { "大堂", d[1] });

                        i++;
                    }

                    if (s.Substring(0, 3).ToUpper() == "H19")
                    {
                        string[] d = s.Split(",".ToCharArray());

                        dicData.Add(i, new string[] { "办公室", d[1] });

                        i++;
                    }
                }

                
            }

            ViewBag.boxData = dicData;

            return View();
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}


        /// <summary>
        /// 获取接口数据
        /// </summary>
        /// <returns></returns>
        public string GetClinetFile()
        {
            string strFile = string.Empty;

            string path = Server.MapPath("~/ZClient/DataFile.txt");

            string ip = System.Configuration.ConfigurationManager.AppSettings["ip"];

            //GetState Data
            ACAdapter.ACAdapter acd = new ACAdapter.ACAdapter(ip, path);
            acd.GetState();

            //判断是否存在 
            if (System.IO.File.Exists(path))
            {
                //读取text 
                strFile = System.IO.File.ReadAllText(path, Encoding.GetEncoding("gb2312"));
            }

            //Thread.Sleep(3000);

            return strFile;
        }
    }
}