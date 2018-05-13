using System;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace AdoDataLayer
{
    public static class DbManagerFactory
    {
        public static DataProvider Validate(this DataProvider value)
        {
            if(!Enum.IsDefined(typeof(DataProvider), value)) throw new InvalidEnumArgumentException("DataProivder must be defined");

            return value;
        }

        public static IDbConnection GetConnection(DataProvider providerType)
        {
            IDbConnection iDbConnection;
            switch (providerType.Validate())
            {
                case DataProvider.SqlServer:
                    iDbConnection = new SqlConnection();
                    break;
                case DataProvider.OleDb:
                    iDbConnection = new OleDbConnection();
                    break;
                case DataProvider.Odbc:
                    iDbConnection = new OdbcConnection();
                    break;
                case DataProvider.Oracle:
                    throw new NotImplementedException();
                default:
                    return null;
            }
            return iDbConnection;
        }

        public static IDbCommand GetCommand(DataProvider providerType)
        {
            switch (providerType.Validate())
            {
                case DataProvider.SqlServer:
                    return new SqlCommand();
                case DataProvider.OleDb:
                    return new OleDbCommand();
                case DataProvider.Odbc:
                    return new OdbcCommand();
                case DataProvider.Oracle:
                    throw new NotImplementedException();
                default:
                    return null;
            }
        }

        public static IDbDataAdapter GetDataAdapter(DataProvider providerType)
        {
            switch (providerType.Validate())
            {
                case DataProvider.SqlServer:
                    return new SqlDataAdapter();
                case DataProvider.OleDb:
                    return new OleDbDataAdapter();
                case DataProvider.Odbc:
                    return new OdbcDataAdapter();
                case DataProvider.Oracle:
                    throw new NotImplementedException();
                default:
                    return null;
            }
        }

        public static IDbTransaction GetTransaction(IDbConnection connection)
        {
            if(connection == null) throw new ArgumentNullException("Connection cannot be null");
            IDbTransaction iDbTransaction = connection.BeginTransaction();
            return iDbTransaction;
        }

        public static IDataParameter GetParameter(DataProvider providerType)
        {
            IDataParameter iDataParameter = null;
            switch (providerType.Validate())
            {
                case DataProvider.SqlServer:
                    iDataParameter = new SqlParameter();
                    break;
                case DataProvider.OleDb:
                    iDataParameter = new OleDbParameter();
                    break;
                case DataProvider.Odbc:
                    iDataParameter = new OdbcParameter();
                    break;
                case DataProvider.Oracle:
                    throw new NotImplementedException();
            }
            return iDataParameter;
        }
    }
}
