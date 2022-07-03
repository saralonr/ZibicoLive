using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZibicoLive.Entity
{
    public sealed class BaseRepository : IRepository,IDisposable
    {
        private Database _db;
        public BaseRepository()
        {
            _db = new Database(DBCon.context);
        }
        public BaseRepository(Database db)
        {
            _db = db;
        }
        public bool Exists<T>()
        {
            return (GetCount<T>() == 0) ? false : true;
        }
        public List<T> Query<T>(string query, object param = null)
        {
            var result = _db.Query<T>(query, param).ToList();
            return result;
        }
        public int GetCount<T>()
        {
            var tableName = ((TableNameAttribute)typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0]).Value;
            var result = _db.Query<int>($"Select count(*) from {tableName}").FirstOrDefault();
            return result;
        }
        public T GetById<T>(int id)
        {
            var tableName = ((TableNameAttribute)typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0]).Value;
            var result = _db.Query<T>($"Select top 1 * from {tableName} where ID=@id", new { id = id }).FirstOrDefault();
            return result;
        }
        public T GetById<T>(Guid id)
        {
            var tableName = ((TableNameAttribute)typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0]).Value;
            var result = _db.Query<T>($"Select top 1 * from {tableName} where ID=@id", new { id = id }).FirstOrDefault();
            return result;
        }
        public T GetById<T>(string id)
        {
            var tableName = ((TableNameAttribute)typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0]).Value;
            var result = _db.Query<T>($"Select top 1 * from {tableName} where ID=@id", new { id = id }).FirstOrDefault();
            return result;
        }
        public List<T> GetList<T>()
        {
            var tableName = ((TableNameAttribute)typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0]).Value;
            var result = _db.Query<T>($"Select * from {tableName}").ToList();
            return result;
        }
        public List<T> GetList<T>(string where)
        {
            var tableName = ((TableNameAttribute)typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0]).Value;
            var result = _db.Query<T>($"Select * from {tableName} where {where}").ToList();
            return result;
        }
        public List<T> GetList<T>(string where, object Params)
        {
            var tableName = ((TableNameAttribute)typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true)[0]).Value;
            var result = _db.Query<T>($"Select * from {tableName} where {where}", Params).ToList();
            return result;
        }
        public object Insert<T>(T poco)
        {
            var result = _db.Insert(poco);
            return result;
        }
        public int Update<T>(T poco)
        {
            var result = _db.Update(poco);
            return result;
        }
        public int Execute(string sql,object Params)
        {
           return _db.Execute(sql, Params);
        }
        public int Delete<T>(T poco)
        {
            var result = _db.Delete(poco);
            return result;
        }
        public int DeleteById<T>(int id)
        {
            var primaryKey = ((PrimaryKeyAttribute)typeof(T).GetCustomAttributes(typeof(PrimaryKeyAttribute), true)[0]).Value;

            T poco = (T)Activator.CreateInstance(typeof(T));
            Type type = typeof(T);
            PropertyInfo prop = type.GetProperty(primaryKey);
            prop.SetValue(poco, id, null);
            var result = _db.Delete(poco);
            return result;
        }
        public int DeleteById<T>(Guid id)
        {
            var primaryKey = ((PrimaryKeyAttribute)typeof(T).GetCustomAttributes(typeof(PrimaryKeyAttribute), true)[0]).Value;

            T poco = (T)Activator.CreateInstance(typeof(T));
            Type type = typeof(T);
            PropertyInfo prop = type.GetProperty(primaryKey);
            prop.SetValue(poco, id, null);
            var result = _db.Delete(poco);
            return result;
        }
        public int DeleteById<T>(string id)
        {
            var primaryKey = ((PrimaryKeyAttribute)typeof(T).GetCustomAttributes(typeof(PrimaryKeyAttribute), true)[0]).Value;

            T poco = (T)Activator.CreateInstance(typeof(T));
            Type type = typeof(T);
            PropertyInfo prop = type.GetProperty(primaryKey);
            prop.SetValue(poco, id, null);
            var result = _db.Delete(poco);
            return result;
        }
        public Page<T> PageList<T>(long page, long perPage, string query, object Params)
        {
            var result = _db.Page<T>(page, perPage, query, Params);
            return result;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
