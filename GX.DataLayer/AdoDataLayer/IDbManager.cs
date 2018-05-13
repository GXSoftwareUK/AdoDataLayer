using System.Collections.Generic;
using System.Data;

namespace AdoDataLayer
{
    public interface IDbManager
    {
        IDbCommand Command { get; }
        IDbConnection Connection { get; }
        string ConnectionString { get; set; }
        IDataReader DataReader { get; set; }
        List<IDbDataParameter> ParameterList { get; }
        DataProvider ProviderType { get; set; }
        IDbTransaction Transaction { get; }

        void AddParameters(IDbDataParameter param);
        void BeginTransaction();
        void Close();
        void CloseReader();
        void CommitTransaction();
        DataSet ExecuteDataSet(CommandType commandType, string commandText);
        int ExecuteNonQuery(CommandType commandType, string commandText);
        IDataReader ExecuteReader(CommandType commandType, string commandText);
        object ExecuteScalar(CommandType commandType, string commandText);
        void Open();
    }
}