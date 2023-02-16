using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DHDoiSim.Common;
namespace DHDoiSim.Models
{
    public class UserListDA
    {
        User user = new User();
        DHDoiSimEntities db = new DHDoiSimEntities();
        public void UpdateLastLogin()
        {
            var update = (from x in db.UserLists where x.ID_User == user.ID select x).Single();
            update.Last_Login = DateTime.Now;
            db.SaveChanges();
        }
        public void UpdateLastSeen(string name,string url)
        {
            var update = (from x in db.UserLists where x.ID_User == user.ID select x).Single();
            update.Last_Seen = name;
            update.Last_Seen_URL = url;
            db.SaveChanges();
        }
        public Dictionary<string, int> GetDashBoard()
        {
            var ListCount = new Dictionary<string, int>();
            int CountAdmin = db.UserLists.Count();
            ListCount.Add("CountAdmin", CountAdmin);
            return ListCount;
        }
        public List<UserList> GetUserLists()
        {
            return db.UserLists.ToList();
        }
        public UserList GetUserList(int id)
        {
            UserList UserList = new UserList();
            try
            {
                UserList = db.UserLists.SingleOrDefault(x => x.ID_User == id);
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            return UserList;
        }
        public bool AddUserList(string name ,string username, string password, string gender, string email, string birthday)
        {
            var UserList = new UserList();
            UserList.Name = name;
            UserList.UserName = username;
            UserList.Password = Common.Encryptor.MD5Hash(password);
            UserList.Gender = gender;
            UserList.Email = email;
            UserList.ID_Permission = 1;
            UserList.Avatar = "avatar-default.jpg";
            UserList.Birthday = Convert.ToDateTime(birthday);
            try
            {
                db.UserLists.Add(UserList);
                db.SaveChanges();
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
        public bool DeleteAdmin(int id)
        {
            try
            {
                var delete = (from x in db.UserLists where x.ID_User == id select x).Single();
                db.UserLists.Remove(delete);
                db.SaveChanges();
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
        public bool EditAdmin(int id_admin, string name, string username, string password, string gender, string email, string birthday)
        {
            try
            {
                var update = (from x in db.UserLists where x.ID_User == id_admin select x).Single();
                update.Name = name;
                update.UserName = username;
                update.Email = email;
                update.Gender = gender;
                update.Birthday = Convert.ToDateTime(birthday);
                if(password != null)
                    update.Password = Common.Encryptor.MD5Hash(password);
                db.SaveChanges();
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}