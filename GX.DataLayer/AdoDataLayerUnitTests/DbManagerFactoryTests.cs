using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Moq;


namespace AdoDataLayer.Tests
{
    [TestClass()]
    public class DbManagerFactoryTests
    {
        private DataProvider _provider;
        private IDbManagerFactory _classUnderTest;

        [TestClass()]
        public class WhenSqlDataProviderNotDefined : DbManagerFactoryTests
        {
            [TestMethod]
            [ExpectedException(typeof(InvalidEnumArgumentException))]
            public void ThenThrowExcpetionGetConnectionTest()
            {
                //Arrange
                _provider = (DataProvider)1000;
                _classUnderTest = new DbManagerFactoryTesteable(_provider);
                //Act

                _classUnderTest.GetConnection();
                //Assert
                Assert.Fail("ExpectedException InvalidEnumArgumentException");
            }

            [TestMethod]
            [ExpectedException(typeof(InvalidEnumArgumentException))]
            public void ThenThrowExcpetionGetCommandTest()
            {
                //Arrange
                _provider = (DataProvider)1000;
                _classUnderTest = new DbManagerFactoryTesteable(_provider);
                //Act

                _classUnderTest.GetCommand();
                //Assert
                Assert.Fail("ExpectedException InvalidEnumArgumentException");
            }

            [TestMethod]
            [ExpectedException(typeof(InvalidEnumArgumentException))]
            public void ThenThrowExcpetionGetDataAdapterTest()
            {
                //Arrange
                _provider = (DataProvider)1000;
                _classUnderTest = new DbManagerFactoryTesteable(_provider);
                //Act

                _classUnderTest.GetDataAdapter();

                //Assert
                Assert.Fail("ExpectedException InvalidEnumArgumentException");
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ThenThrowExcpetionGetTransactionTest()
            {
                //Arrange
                _classUnderTest = new DbManagerFactoryTesteable(DataProvider.SqlServer);
                
                //Act

                _classUnderTest.GetTransaction();
                //Assert
                Assert.Fail("ExpectedException InvalidEnumArgumentException");
            }

            [TestMethod]
            [ExpectedException(typeof(InvalidEnumArgumentException))]
            public void ThenThrowExcpetionGetParameterTest()
            {
                //Arrange
                _provider = (DataProvider)1000;
                _classUnderTest = new DbManagerFactoryTesteable(_provider);
                //Act

                _classUnderTest.GetParameter();
                //Assert
                Assert.Fail("ExpectedException InvalidEnumArgumentException");
            }
        }

       [TestClass]
        public class WhenSqlDataProvider: DbManagerFactoryTests
        {
            public WhenSqlDataProvider()
            {
                _provider = DataProvider.SqlServer;
                _classUnderTest = new DbManagerFactoryTesteable(_provider);
            }

            [TestMethod]
            public void GetConnectionITest()
            {
                //Arrange
                _classUnderTest.GetConnection();

                //Act
                var result = _classUnderTest.Connection;
                
                //Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SqlConnection);
            }

            [TestMethod()]
            public void GetCommandTest()
            {
                //Arrange
                _classUnderTest.GetCommand();

                //Act
                var result = _classUnderTest.Command;

                //Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SqlCommand);
            }

            [TestMethod()]
            public void GetDataAdapterTest()
            {
                //Arrange
                _classUnderTest.GetDataAdapter();

                //Act
                var result = _classUnderTest.Adapter;

                //Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SqlDataAdapter);
            }

            [TestMethod()]
            public void GetTransactionTest()
            {
                //Arrange
                var transaction = new Mock<IDbTransaction>();
                var connection = new Mock<IDbConnection>();
                connection.Setup(t => t.BeginTransaction()).Returns((IDbTransaction)transaction.Object);

                _classUnderTest.Connection = connection.Object;
                _classUnderTest.GetTransaction();
                

                //Act
                var result = _classUnderTest.Transaction;

                //Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result.GetType().Name.Equals("IDbTransactionProxy", StringComparison.CurrentCultureIgnoreCase));
            }

            [TestMethod()]
            public void GetParameterTest()
            {
                //Arrange
                _classUnderTest.GetParameter();

                //Act
                var result = _classUnderTest.Parameter;

                //Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result is SqlParameter);
            }
        }
       
    }

    public interface IDbManagerFactory
    {
        void GetConnection();

        void GetCommand();

        void GetDataAdapter();

        void GetTransaction();

        void GetParameter();

        IDbConnection Connection { get; set; }

        IDbCommand Command { get; set; }

        IDbDataAdapter Adapter { get; set; }

        IDbTransaction Transaction { get; set; }

        IDataParameter Parameter { get; set; }
    }

    public class DbManagerFactoryTesteable : IDbManagerFactory
    {
        private readonly DataProvider _providerType;

        public virtual IDbConnection Connection { get; set; }

        public virtual IDbCommand Command { get; set; }

        public virtual IDbDataAdapter Adapter { get; set; }

        public IDbTransaction Transaction { get; set; }

        public virtual IDataParameter Parameter { get; set; }

        public DbManagerFactoryTesteable(DataProvider providerType)
        {
            _providerType = providerType;
        }

        public virtual void GetConnection()
        {
            Connection = DbManagerFactory.GetConnection(_providerType);
        }

        public virtual void GetCommand()
        {
            Command = DbManagerFactory.GetCommand(_providerType);
        }

        public virtual void GetDataAdapter()
        {
            Adapter = DbManagerFactory.GetDataAdapter(_providerType);
        }

        public virtual void GetTransaction()
        {
            Transaction = DbManagerFactory.GetTransaction((IDbConnection)Connection);
        }

        public virtual void GetParameter()
        {
            Parameter = DbManagerFactory.GetParameter(_providerType);
        }
    }
}