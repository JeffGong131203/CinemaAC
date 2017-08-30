using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ACAdapter;

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

        public ActionResult SetState(string switcher,string setValue)
        {
            return Cinema("S");
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