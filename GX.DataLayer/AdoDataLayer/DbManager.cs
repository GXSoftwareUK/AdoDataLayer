using System;
using System.Collections.Generic;
using System.Data;

namespace AdoDataLayer
{
    public sealed class DbManager : IDisposable, IDbManager
    {
        private IDbCommand _idbCommand;

        public DbManager(DataProvider providerType)
        {
            ProviderType = providerType;
        }

        public DbManager(DataProvider providerType, string connectionString)
        {
            ProviderType = providerType;
            ConnectionString = connectionString;
        }

        public IDbConnection Connection { get; private set; }

        public IDataReader DataReader { get; set; }

        public DataProvider ProviderType { get; set; }

        public string ConnectionString { get; set; }

        public IDbCommand Command => _idbCommand;

        public IDbTransaction Transaction { get; private set; }

        public void Open()
        {
            Connection = DbManagerFactory.GetConnection(ProviderType);

            Connection.ConnectionString = ConnectionString;

            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            _idbCommand = DbManagerFactory.GetCommand(ProviderType);
        }

        public void Close()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }


        public void AddParameters(IDbDataParameter param)
        {
            ParameterList.Add(param);
        }

        public void BeginTransaction()
        {
            if (Transaction == null)
                Transaction = DbManagerFactory.GetTransaction(ProviderType);
            _idbCommand.Transaction = Transaction;
        }

        public void CommitTransaction()
        {
            Transaction?.Commit();
            Transaction = null;
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            _idbCommand = DbManagerFactory.GetCommand(ProviderType);

            _idbCommand.Connection = Connection;

            PrepareCommand(_idbCommand, Connection, Transaction,commandType,commandText, ParameterList);
            DataReader = _idbCommand.ExecuteReader();

            _idbCommand.Parameters.Clear();

            return DataReader;
        }

        public void CloseReader()
        {
            DataReader?.Close();
        }

        private static void AttachParameters(IDbCommand command, IEnumerable<IDbDataParameter> commandParameters)
        {
            foreach (var idbParameter in commandParameters)
            {
                if ((idbParameter.Direction == ParameterDirection.InputOutput) && (idbParameter.Value == null))
                    idbParameter.Value = DBNull.Value;
                
                command.Parameters.Add(idbParameter);
            }
        }

        private static void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText,IEnumerable<IDbDataParameter> commandParameters)
        {
            command.Connection = connection;
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (transaction != null)
                command.Transaction = transaction;
            
            if (commandParameters != null)
                AttachParameters(command, commandParameters);
            
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            _idbCommand = DbManagerFactory.GetCommand(ProviderType);

            PrepareCommand(_idbCommand, Connection, Transaction,commandType, commandText, ParameterList);

            var returnValue = _idbCommand.ExecuteNonQuery();

            _idbCommand.Parameters.Clear();

            return returnValue;
        }

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            _idbCommand = DbManagerFactory.GetCommand(ProviderType);

            PrepareCommand(_idbCommand, Connection, Transaction,commandType,commandText, ParameterList);

            var returnValue = _idbCommand.ExecuteScalar();

            _idbCommand.Parameters.Clear();

            return returnValue;
        }

        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            _idbCommand = DbManagerFactory.GetCommand(ProviderType);

            PrepareCommand(_idbCommand, Connection, Transaction,commandType,commandText, ParameterList);

            var dataAdapter = DbManagerFactory.GetDataAdapter(ProviderType);

            dataAdapter.SelectCommand = _idbCommand;

            var dataSet = new DataSet();

            dataAdapter.Fill(dataSet);

            _idbCommand.Parameters.Clear();

            return dataSet;
        }

        public List<IDbDataParameter> ParameterList => _parameterList ?? (_parameterList = new List<IDbDataParameter>());

        private List<IDbDataParameter> _parameterList;

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                Close();
                _idbCommand = null;
                Transaction = null;
                Connection = null;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _disposedValue = true;
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~DbManager()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
             GC.SuppressFinalize(this);
        }
        #endregion

    }
}