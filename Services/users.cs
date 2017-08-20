using System;
using System.Configuration;
using System.Drawing;
namespace Services
{
    [Serializable]
    public class Users
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public Image HeadImage { get; set; }
        public string HeadImageAddress { get; set; }
        public static Users CreateUser(string username,string password,string headimage)
        {
            Users user = new Users();
            user.Username = username;
            user.Password = password;
            if (headimage == null) user.HeadImageAddress = "images/avtar.png";
            else user.HeadImageAddress = headimage;
            return user;
        }
        public static Users CreateUser(string username, string password, Image headimage)
        {
            Users user = new Users();
            user.Username = username;
            user.Password = password;
            if (headimage == null) user.HeadImage = Image.FromFile("images/avtar.png");
            else user.HeadImage = headimage;
            return user;
        }
    }
}
