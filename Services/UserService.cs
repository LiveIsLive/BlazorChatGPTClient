using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.Services
{
	public class UserService
	{
		protected readonly LiteDB.ILiteDatabase Database;

		private LiteDB.ILiteCollection<Models.User> _Users = null!;
		protected LiteDB.ILiteCollection<Models.User> Users
		{
			get
			{
				if (this._Users == null)
					this._Users=this.Database.GetCollection<Models.User>();
				return this._Users;
			}
		}

		//static UserService()
		//{
		//	LiteDB.BsonMapper.Global.EnumAsInteger = true;
		//}

		public UserService(LiteDB.ILiteDatabase database)
		{
			Database = database;
		}

		protected string MD5(string s)
		{
			return System.BitConverter.ToString(System.Security.Cryptography.MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(s))).Replace("-", "");
		}

		public string? Check(string userName,string password)
		{
			Models.User user = this.Users.FindOne(u => u.UserName == userName);
			if (user == null)
				return "用户名不正确！";
			if (this.MD5(password) != user.Password)
				return "密码不正确！";
			return null;
		}

		public Models.User[] List()
		{
			return this.Users.Query().OrderBy(u => u.UserName).ToArray();
		}

		public string? Update(Models.User user)
		{
			if (user.Password.Length != 32)
				user.Password = this.MD5(user.Password);
			if (this.Users.Exists(u => u.UserId == user.UserId))
				this.Users.Update(user);
			this.Users.Insert(user);
			return null;
		}

		public bool Delete(long userId)
		{
			return this.Users.Delete(userId);
		}
	}
}