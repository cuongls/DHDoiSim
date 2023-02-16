using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DHDoiSim.Models;
namespace DHDoiSim.Common
{
    public class User
    {
        public bool ISLOGIN { get; set; } = false;
        public int ID { get; set; }
        public int PERMISSION { get; set; }
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string AVATAR { get; set; }
        public string NAME { get; set; }
        public int ID_PHONG { get; set; }
        public int ID_TO { get; set; }

        public User ()
        {
            try
            {
                ISLOGIN = (bool)HttpContext.Current.Session[UserSession.ISLOGIN];
                ID = (int)HttpContext.Current.Session[UserSession.ID];
                PERMISSION = (int)HttpContext.Current.Session[UserSession.PERMISSION];
                USERNAME = (string)HttpContext.Current.Session[UserSession.USERNAME];
                EMAIL = (string)HttpContext.Current.Session[UserSession.EMAIL];
                AVATAR = (string)HttpContext.Current.Session[UserSession.AVATAR];
                NAME = (string)HttpContext.Current.Session[UserSession.NAME];
                ID_PHONG = (int)HttpContext.Current.Session[UserSession.ID_PHONG];
                ID_TO = (int)HttpContext.Current.Session[UserSession.ID_TO];
            }
            catch (Exception) { }
        }
        public bool IsLogin()
        {
            try
            {
                if (ISLOGIN)
                    return true;
                return false;
            } catch (Exception)
            {
                return false;
            }
        }
        public void Reset()
        {
            HttpContext.Current.Session.Clear();
        }
        //ThaiHN thêm - Quyền cập nhật dữ liệu
        public bool IsDataEntry()
        {
            try
            {
                if (ISLOGIN && (PERMISSION  == 2) || (PERMISSION == 1))
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //
        public bool IsAdmin()
        {
            try
            {
                if (ISLOGIN && PERMISSION == 1)
                    return true;
                return false;
            } catch (Exception)
            {
                return false;
            }
        }

    }
}