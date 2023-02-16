using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Globalization;

namespace DHDoiSim.Controllers
{
    [RoutePrefix("api/DHDoiSim_GetUrlImage")]
    //[RoutePrefix("api/DHDoiSim_UploadImage")]
    public class GetUrlImageController : ApiController
    {
        [System.Web.Http.HttpPost]
        [AllowAnonymous]
        //public HttpResponseMessage PostImage(string MaDuAn, string MaTram, string MaUnit, string LoaiImage, string Lat, string Lng, string TaiKhoan, string Email)
        public HttpResponseMessage PostImage(string IDSimPhieu, string position)
        {
            //string Lat = "", Lng = "";
            int WeekOfYear = 1;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                WeekOfYear = GetWeekNumber(DateTime.Now);
                var httpRequest = HttpContext.Current.Request;
                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        IList<string> AllowedFileExtensions = new List<string> { ".zip", ".rar", ".jpg", ".gif", ".png", ".trp", ".xls", ".xlsx", ".pdf" };
                        //IList<string> AllowedFileExtensions = new List<string> {".jpg", ".gif", ".png", ".trp", ".xls", ".xlsx", ".pdf" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();

                        //clsKetNoiSHVT cls = new clsKetNoiSHVT();
                        //DataTable dt = cls.LoadData("insert into PhotoImage ");
                        //string ID = dt.Rows[0]["ID"].ToString();

                        string location = "", locationDone = "";
                        string filename = postedFile.FileName;
                        filename = postedFile.FileName.Replace(" ", "");
                        //location = DateTime.Now.Second.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "_Lng" + Lng + "_Lat" + Lat + "_" + Email + "_" + MaDuAn + "_" + MaTram + "_" + MaUnit + "_" + LoaiImage + "_" + filename;
                        location = DateTime.Now.Second.ToString() + DateTime.Now.Minute.ToString() + IDSimPhieu + "_" + position + "_" + filename;
                        //var filePath = HttpContext.Current.Server.MapPath("~/Images/NTCSHT/" + location);                        
                        //locationDone = @"E:\MsHa\Programs\Nam 2021\ChuongTrinhVT_TapTrung_DoneNew\WebApplication1\Images\NTCSHT\" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() +"/"+ location;
                        locationDone = @"E:\MrThai\DHDoiSim\DHDoiSim\Images\" + WeekOfYear + "_" + DateTime.Now.Year.ToString() + "/" + location;
                        location = "/Images/" + WeekOfYear + "_" + DateTime.Now.Year.ToString() + "/" + location;

                        if (!Directory.Exists(locationDone))
                        {
                            Directory.CreateDirectory(@"E:\MrThai\DHDoiSim\DHDoiSim\Images\" + WeekOfYear + "_" + DateTime.Now.Year.ToString() + "/");

                        }
                        postedFile.SaveAs(locationDone);
                        var Message = string.Format("http://203.210.142.246:8003" + location);//địa chỉ web được đưa lên IIS

                        return Request.CreateErrorResponse(HttpStatusCode.Created, Message);

                    }
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception e)
            {
                //using (System.IO.StreamWriter file1 =
                //                 new System.IO.StreamWriter(@"E:\MsHa\Programs\Nam 2021\log.txt"))
                //{
                //    file1.WriteLine(e.Message);
                //}

                var res = string.Format("Error");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }
        static int GetWeekNumber(DateTime time)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
    }
}
