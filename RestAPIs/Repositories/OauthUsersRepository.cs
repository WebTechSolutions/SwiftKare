using System;
using System.Collections.Generic;
using Microsoft.ApplicationBlocks.Data;
using RestAPIs.Extensions;
using RestAPIs.Interfaces;
using RestAPIs.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RestAPIs.Repositories
{
    public class OauthUsersRepository : IRepository<OauthUserModel>
    {
        public string ConnectionString { get; set; }

        private readonly string _userName;
        private readonly string _password;
        public OauthUsersRepository(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public bool Exists(object id)
        {
            throw new NotImplementedException();
        }

        public OauthUserModel Find(object id)
        {
            var listParam = new List<SqlParameter>
            {
                new SqlParameter("@userName", _userName),
                new SqlParameter("@password", _password)
            };
            var dataSet = new DataSet();
            try
            {
                dataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "GetApiAuthorizedUsers",
                listParam.ToArray());
            }
            catch (Exception)
            {
               // throw;
            }
            if (dataSet.Tables.Count <= 0) return null;
            if (dataSet.Tables[0].Rows.Count <= 0) return null;
            var objModel = dataSet.Tables[0].ToList<OauthUserModel>().FirstOrDefault();
            return objModel;
        }

        public List<OauthUserModel> GetList()
        {
            var dataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "GetApiAuthorizedUsers");
            if (dataSet.Tables.Count <= 0) return null;
            if (dataSet.Tables[0].Rows.Count <= 0) return null;
            var list = dataSet.Tables[0].ToList<OauthUserModel>();
            return list;
        }

        public bool Save(OauthUserModel t)
        {
            throw new NotImplementedException();
        }
    }
}