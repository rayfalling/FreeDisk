using System;
using System.IO;
using System.Web.UI;
using Services.AES;
using Services.User;

namespace FreeDiskWeb
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoadImg(object sender, EventArgs e)
        {
            UserHelper user = new UserHelper();
            if (user.Exists(UserName.Text))
            {
                string imgaddress = "images/" + user.Username + "_avtar.png";
                if (File.Exists(imgaddress)) headimg.ImageUrl = imgaddress;
                else
                {
                    user.GetModel(UserName.Text);
                    user.HeadImage.Save("images/" + user.Username + "_avtar.png");
                    headimg.ImageUrl = imgaddress;
                }
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            UserHelper user = new UserHelper();
            user.GetModel(UserName.Text);
            string pasewordaes = UserMethods.AesPassword(PassWord.Text);
            if (UserName.Text == user.Username && pasewordaes == user.Password)
            {
                //todo 完成页面跳转以及加载文件列表
            }
        }
    }
}