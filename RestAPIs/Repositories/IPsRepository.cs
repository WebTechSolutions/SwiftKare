using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using Microsoft.ApplicationBlocks.Data;
using RestAPIs.Extensions;
using RestAPIs.Interfaces;
using RestAPIs.Models;

namespace RestAPIs.Repositories
{
    // ReSharper disable once InconsistentNaming
    public class IPsRepository : IRepository<IPModel>
    {
        public IPsRepository()
        {
            ConnectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }

        public string ConnectionString { get; set; }

        public List<IPModel> GetList()
        {
            var dataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "GetAuthorizedIPs");
            if (dataSet.Tables.Count <= 0) return null;
            if (dataSet.Tables[0].Rows.Count <= 0) return null;
            var ipList = dataSet.Tables[0].ToList<IPModel>();
            return ipList;
        }

        public IPModel Find(object ip)
        {
            var listParam = new List<SqlParameter>
            {
                new SqlParameter("@IP", ip.ToString())
            };
            var dataSet = SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "GetAuthorizedIPs",
                listParam.ToArray());
            if (dataSet.Tables.Count <= 0) return null;
            if (dataSet.Tables[0].Rows.Count <= 0) return null;
            var objModel = dataSet.Tables[0].ToList<IPModel>().FirstOrDefault();
            return objModel;
        }

        public bool Exists(object ip)
        {
            return Find(ip) != null;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public bool Save(IPModel t)
        {
            throw new NotImplementedException();
        }
    }
}