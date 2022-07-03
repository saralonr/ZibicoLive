using PetaPoco;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZibicoLive.Entity
{
    public interface IRepository
    {
        bool Exists<T>();
        int GetCount<T>();
        T GetById<T>(int id);
        T GetById<T>(Guid id);
        T GetById<T>(string id);
        List<T> GetList<T>();
        List<T> GetList<T>(string where);
        List<T> GetList<T>(string where, object Params);
        object Insert<T>(T poco);
        int Update<T>(T poco);
        int Execute(string sql, object Params);
        int Delete<T>(T poco);
        int DeleteById<T>(int id);
        int DeleteById<T>(Guid id);
        int DeleteById<T>(string id);
        Page<T> PageList<T>(long page, long perPage, string query, object Params);
    }
}
