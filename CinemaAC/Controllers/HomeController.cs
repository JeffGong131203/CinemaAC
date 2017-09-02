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


            return Cinema(dType);
        }

        public ActionResult Cinema(string menuID)
        {
            if (!string.IsNullOrEmpty(menuID))
            {
                string clientData = GetClinetFile();

                string[] strData = clientData.Split(";".ToCharArray());

                ArrayList arrS = new ArrayList();

                foreach (string s in strData)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (s.Substring(0, 1).ToUpper() == menuID.ToUpper())
                        {
                            arrS.Add(s);
                        }
                    }
                }

                ViewBag.arrData = arrS;
                ViewBag.DType = menuID;

                return View(menuID);
            }
            else
            {
                return Content("Error MenuID");
            }
        }

        public ActionResult ChartsD()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["SqlConn"]))
            {
                conn.Open();

                string sql = "SELECT top 100 [UpdateTime],[DID],[Col1] FROM [ACCinema].[dbo].[tb_ACData] where dtype='D' order by [UpdateTime] desc,[DID]";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                sda.Fill(dt);
            }

            //ViewBag.dData = dt;
            DataView dv = dt.DefaultView;
            DataTable dtDate = dv.ToTable(true, "UpdateTime");
            DataTable dtDid = dv.ToTable(true, "DID");

            string[] arrayX = new string[dtDate.Rows.Count];
            for(int i=0;i<dtDate.Rows.Count;i++)
            {
                arrayX[i] = DateTime.Parse(dtDate.Rows[i][0].ToString()).ToString("yyyy-MM-dd hh:mm:ss");
            }
            Array.Reverse(arrayX);

            Dictionary<string, string[]> dicData = new Dictionary<string, string[]>();

            foreach(DataRow dr in dtDid.Rows)
            {
                string did = Convert.ToString(dr[0]);

                DataRow[] arrDr = dt.Select(string.Format("DID='{0}'", did));
                string[] arrayD = new string[arrDr.Length];
                for (int i= 0;i<arrDr.Length;i++)
                {
                    arrayD[i] = Convert.ToString(arrDr[i]["Col1"]);
                }
                Array.Reverse(arrayD);

                dicData.Add(did, arrayD);
            }

            ViewBag.arrayX = Newtonsoft.Json.JsonConvert.SerializeObject(arrayX);
            ViewBag.dicData = dicData;

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
            //ACAdapter.ACAdapter acd = new ACAdapter.ACAdapter(ip, path);
            //acd.GetState();

            //判断是否存在 
            if (System.IO.File.Exists(path))
            {
                //读取text 
                strFile = System.IO.File.ReadAllText(path, Encoding.GetEncoding("gb2312"));
            }

            return strFile;
        }
    }
}