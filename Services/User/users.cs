using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using Services.AES;
using Services.SQL;

namespace Services.User
{
    [Serializable]
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Bitmap HeadImage { get; set; }
        public string HeadImageAddress { get; set; }
        public int Group { get; set; }
    }
    public class UserHelper : Users
    {
        public static Users CreateUser(string username, string password, string headimage)
        {
            Users user = new Users
            {
                Username = username,
                Password = password,
                HeadImageAddress = headimage ?? "images/avtar.png"
            };
            return user;
        }
        public static Users CreateUser(string username, string password, Bitmap headimage)
        {
            Bitmap bitmap = new Bitmap("images/avtar.png");
            Users user = new Users
            {
                Username = username,
                Password = password,
                HeadImage = headimage ?? bitmap
            };
            return user;
        }
        public static int GetMaxId()
        {
            return SqlHelper.GetMaxId("Id", "Users");
        }
        public bool Exists(string username)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from [Users]");
            strSql.Append(" where Username=@Username ");
            SqlParameter[] parameters = {
                    new SqlParameter("@Username", SqlDbType.NVarChar,50)};
            parameters[0].Value = username;
            return SqlHelper.Exists(strSql.ToString(), parameters);
        }
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from [Users]");
            strSql.Append(" where Username=@Username ");
            SqlParameter[] parameters = {
                    new SqlParameter("@Username", SqlDbType.NVarChar,50)};
            parameters[0].Value = id;
            return SqlHelper.Exists(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Users] (");
            strSql.Append("Id,Username,UserPassword,headimage,UserGroup)");
            strSql.Append(" values (");
            strSql.Append("@id,@Username,@password,@headimage,@group)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@Id", SqlDbType.Int,4),
                    new SqlParameter("@username", SqlDbType.NVarChar,50),
                    new SqlParameter("@password", SqlDbType.NVarChar,50),
                    new SqlParameter("@headimage", SqlDbType.Image),
                    new SqlParameter("@group", SqlDbType.Int,4)};
            parameters[0].Value = Id;
            parameters[1].Value = Username;
            parameters[2].Value = Password;
            parameters[3].Value = HeadImage;
            parameters[4].Value = Group;
            object obj = SqlHelper.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            return Convert.ToInt32(obj);
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update [Users] set ");
            strSql.Append("Username=@Username,");
            strSql.Append("UserPassword=@password,");
            strSql.Append("headimage=@headimage");
            strSql.Append("UserGroup=@group");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters = {
                    new SqlParameter("@Id", SqlDbType.Int),
                    new SqlParameter("@username", SqlDbType.NVarChar,50),
                    new SqlParameter("@password", SqlDbType.NVarChar,50),
                    new SqlParameter("@headimage", SqlDbType.Image),
                    new SqlParameter("@group", SqlDbType.Int)};
            parameters[0].Value = Id;
            parameters[1].Value = Username;
            parameters[2].Value = Password;
            parameters[3].Value = HeadImage;
            parameters[4].Value = Group;
            int rows = SqlHelper.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from [Users] ");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters = {
                    new SqlParameter("@Id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            int rows = SqlHelper.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public void GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Id,Username,UserPassword,headimage,UserGroup ");
            strSql.Append(" FROM [Users] ");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters = {
                    new SqlParameter("@Id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            DataSet ds = SqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Id"] != null && ds.Tables[0].Rows[0]["Id"].ToString() != "")
                {
                    Id = int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
                }
                if (ds.Tables[0].Rows[0]["Username"] != null)
                {
                    Username = ds.Tables[0].Rows[0]["Username"].ToString();
                }
                if (ds.Tables[0].Rows[0]["UserPassword"] != null && ds.Tables[0].Rows[0]["UserPassword"].ToString() != "")
                {
                    Password = ds.Tables[0].Rows[0]["UserPassword"].ToString();
                }
                if (ds.Tables[0].Rows[0]["headimage"] != null)
                {
                    byte[] imageBytes = (byte[])ds.Tables[0].Rows[0]["headimage"];
                    MemoryStream ms = new MemoryStream(imageBytes);
                    Bitmap bmap = new Bitmap(ms);
                    HeadImage = bmap;
                }
                if (ds.Tables[0].Rows[0]["UserGroup"] != null && ds.Tables[0].Rows[0]["UserGroup"].ToString() != "")
                {
                    Id = int.Parse(ds.Tables[0].Rows[0]["UserGroup"].ToString());
                }
            }
        }

        public void GetModel(string username)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Id,Username,UserPassword,headimage,UserGroup ");
            strSql.Append(" FROM [Users] ");
            strSql.Append(" where Username=@Username ");
            SqlParameter[] parameters = {
                    new SqlParameter("@Username", SqlDbType.NVarChar,50)};
            parameters[0].Value = username;

            DataSet ds = SqlHelper.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Id"] != null && ds.Tables[0].Rows[0]["Id"].ToString() != "")
                {
                    Id = int.Parse(ds.Tables[0].Rows[0]["Id"].ToString());
                }
                if (ds.Tables[0].Rows[0]["Username"] != null)
                {
                    Username = ds.Tables[0].Rows[0]["Username"].ToString();
                }
                if (ds.Tables[0].Rows[0]["UserPassword"] != null && ds.Tables[0].Rows[0]["UserPassword"].ToString() != "")
                {
                    Password = ds.Tables[0].Rows[0]["UserPassword"].ToString();
                }
                if (ds.Tables[0].Rows[0]["headimage"] != null)
                {
                    byte[] imageBytes = (byte[])ds.Tables[0].Rows[0]["headimage"];
                    MemoryStream ms = new MemoryStream(imageBytes);
                    Bitmap bmap = new Bitmap(ms);
                    HeadImage = bmap;
                }
                if (ds.Tables[0].Rows[0]["UserGroup"] != null && ds.Tables[0].Rows[0]["UserGroup"].ToString() != "")
                {
                    Id = int.Parse(ds.Tables[0].Rows[0]["UserGroup"].ToString());
                }
            }
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM [Users] ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return SqlHelper.Query(strSql.ToString());
        }
    }

    public static class UserMethods
    {
        public static string AesPassword(string password)
        {
            return password.AesStr(Keys.Keyval, Keys.Ivval);
        }
    }
}
