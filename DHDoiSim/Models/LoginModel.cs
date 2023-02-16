using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
//ThaiHN Thêm
using System.Net;
using System.Text;
using System.IO;
using System.Net.Http;
//using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace DHDoiSim.Models
{
    public class LoginModel
    {
        UserListDA Model = new UserListDA();
        DHDoiSimEntities db = new DHDoiSimEntities();

        [Display(Name = "Tài Khoản")]
        public string Username { get; set; }
        [Display(Name = "Mật Khẩu")]
        public string Password { get; set; }
        public bool IsValid(LoginModel model)
        {
            //ThaiHN Sửa
            //Thêm XTTT Tập đoàn ở đây
            var request = (HttpWebRequest)WebRequest.Create("http://10.10.41.18/ApiAuthenVnpt/LdapVnpt/AuthenVnpt");
            NetworkCredential credentials = new NetworkCredential("WebApiUsername", "WebApiPattword");
            request.Credentials = credentials;
            var postData = "UserName=" + model.Username;
            postData += "&PassWord=" + model.Password;
            postData += "&AllowOtp=false";
            postData += "&SoDienThoaiFromVnpt=false";
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //parse JSON 
            dynamic ketquaxacthuc = JObject.Parse(responseString);
            Boolean xacthuc_error;
            String thongbao;
            xacthuc_error = ketquaxacthuc.IsError;
            thongbao = ketquaxacthuc.Message;

            if (xacthuc_error == true)
            {
                //Response.Write("<script>alert('" + thongbao + "')</script>");
                return false;
            }
            //Kiểm tra nếu chưa có trong Student thì thêm vào
            //
            //bool add = Model.AddUserList(model.Username, model.Username, "abc123", "nam", model.Username, "01/01/2000");
            try
            {
                if (Convert.ToBoolean(db.UserLists.First(x => x.UserName == model.Username).ID_User))
                {
                    SetUserSession(db.UserLists.First(x => x.UserName == model.Username).ID_User);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.InnerException);
            }
            return false;
        }
        public void SetUserSession(int userID)
        {
            UserList user = db.UserLists.SingleOrDefault(x => x.ID_User == userID);
            HttpContext.Current.Session.Add(Common.UserSession.ISLOGIN, true);
            HttpContext.Current.Session.Add(Common.UserSession.ID, user.ID_User);
            HttpContext.Current.Session.Add(Common.UserSession.PERMISSION, user.ID_Permission);
            HttpContext.Current.Session.Add(Common.UserSession.USERNAME, user.UserName);
            HttpContext.Current.Session.Add(Common.UserSession.EMAIL, user.Email);
            HttpContext.Current.Session.Add(Common.UserSession.AVATAR, user.Avatar);
            HttpContext.Current.Session.Add(Common.UserSession.NAME, user.Name);
            HttpContext.Current.Session.Add(Common.UserSession.ID_PHONG, user.ID_Phong);
            HttpContext.Current.Session.Add(Common.UserSession.ID_TO, user.ID_To);
        }
    }
}