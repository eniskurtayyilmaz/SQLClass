using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Data;

namespace SuperSarar.cls
{
    public class SqlDatabase : IDisposable
    {

        //public static IPAddress[] ip;
        public static string host_name;
        public static int userFK;

        
        ConnectionInfo info = new ConnectionInfo();

        #region Private Variables

        public static SqlConnection connection = null;
        private SqlConnectionStringBuilder connectionStrBuilder = null;
        private string connectionString = String.Empty;
        private bool disposed = false;
        //SqlCommand command = new SqlCommand();

        #endregion

        #region Class Constructors



        public string dataadi()
        {
            //string stcon = ConfigurationManager.AppSettings["Connection"].ToString();
            //int baslangic = stcon.IndexOf("database=");
            //stcon = stcon.Substring(baslangic + 9, stcon.Length - baslangic - 10);
            string sql = @"SELECT  * FROM   sys.databases AS d WHERE     (name = '" + info.DataBaseName + "')";
            DataSet ds = SqlUtil.SqlDatabaseDataset(sql);
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["name"].ToString() + "-" + ds.Tables[0].Rows[0]["database_id"] + "-" + ds.Tables[0].Rows[0]["create_date"];
            return "";

        }



        private string ReadConfig()
        {
            string str = "";
            try
            {
                //    str = decryptPassword(ConfigurationManager.AppSettings["Connection"].ToString());
                // str = ConfigurationManager.AppSettings["Connection"].ToString();
                str = "server=" + info.DataSource + ";uid=" + info.UserName + ";pwd=" + info.Password + ";database=" + info.DataBaseName + ";Max Pool Size=1024;Pooling=true;";
            }
            catch (Exception exX)
            {
                // str = @"server=UFUK\UFUK;uid=sa;pwd=123;database=sagliknetson1;";
                //throw;
            }
            return str;
        }

        //şifreleme yontemi

        public static RSACryptoServiceProvider rsa;
        public static void AssignParameter()
        {
            const int PROVIDER_RSA_FULL = 1;
            const string CONTAINER_NAME = "SpiderContainer";
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            rsa = new RSACryptoServiceProvider(cspParams);
        }
        public static string DecryptData(string data2Decrypt)
        {
            AssignParameter();
            byte[] getpassword = Convert.FromBase64String(data2Decrypt);
            byte[] plain = rsa.Decrypt(getpassword, false);
            return System.Text.Encoding.UTF8.GetString(plain);
        }

        public string sifrecoz()
        {

            /*
            OakLeaf.MM.Main.Managers.mmAppSettingsManager mng = new OakLeaf.MM.Main.Managers.mmAppSettingsManager();
            string retVal = mng.GetSectionSetting("databases", "RaporYazilim\\Connection");
            string[] parts = retVal.Split(';');
            string sifre = parts[2];
            sifre = sifre.Substring(4, sifre.Length - 4);
            string coz = "";
            try
            {
                coz = DecryptData(sifre);

            }
            catch (Exception ex)
            {

                coz = "1";
            }

            parts[2] = "pwd=" + coz;
            string don = "";
            foreach (string s in parts)
                don += s + ";";
            don = don.Substring(0, don.Length - 1);
            return don;
             */
            return "";
        }
        //
        public string[] baglantibilgi()
        {
            string str = this.ReadConfig();
            return str.Split(';');
        }
        public SqlDatabase()
        {
            connectionString = this.ReadConfig(); //Properties.Settings.Default["ConnectionString"].ToString();
            connectionString = "Data Source=" + info.DataSource + ";Initial Catalog=" + info.DataBaseName + ";User Id=" + info.UserName + ";Password=" + info.Password + ";";
            //connection = new SqlConnection(connectionString);

            try
            {
                connection = new SqlConnection(connectionString);
            }
            catch
            {
                connection = new SqlConnection(connectionString);
            }
            //string host_name = Dns.GetHostName();
            //IPAddress[] ip = Dns.GetHostAddresses(host_name);
            //host_name = Dns.GetHostName();
            //ip = Dns.GetHostAddresses(host_name);

            try
            {
                SqlConnection.ClearPool(connection);
                SqlConnection.ClearAllPools();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("hataaaa" + ex.Message);
            }
        }

        public SqlDatabase(int pnConnectTimeOut)
        {
            connectionString = this.ReadConfig(); //Properties.Settings.Default["ConnectionString"].ToString();

            //connection = new SqlConnection(connectionString);

            try
            {
                connection = new SqlConnection(connectionString);
            }
            catch
            {
                connection = new SqlConnection(connectionString);
            }
            
        }

        public SqlDatabase(string connectionStr)
        {
            connectionString = connectionStr;
            connection = new SqlConnection(connectionString);
        }

        public SqlDatabase(string dataSource, string databaseName)
        {
            connectionStrBuilder = new SqlConnectionStringBuilder();
            connectionStrBuilder.DataSource = dataSource;
            connectionStrBuilder.InitialCatalog = databaseName;
            connectionStrBuilder.IntegratedSecurity = true;
            connection = new SqlConnection(connectionStrBuilder.ConnectionString);
        }

        public SqlDatabase(string dataSource, string databaseName, string userName, string password)
        {
            connectionStrBuilder = new SqlConnectionStringBuilder();
            connectionStrBuilder.DataSource = dataSource;
            connectionStrBuilder.InitialCatalog = databaseName;
            connectionStrBuilder.UserID = userName;
            connectionStrBuilder.Password = password;
            connection = new SqlConnection(connectionStrBuilder.ConnectionString);
        }

        #endregion

        #region Destructors

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                    connection.Dispose();
                }
            }
            disposed = true;
        }

        ~SqlDatabase()
        {
            Dispose(false);
        }

        #endregion

        public SqlException LastException = null;
        public SqlTransaction currentTransaction = null;

        #region Public Functions

        /// <summary>
        /// Opens a connection to the server.
        /// Connection string has to be defined before
        /// </summary>
        /// <returns>True if Success</returns>
        public bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    if (connection.ConnectionString == "")
                        connection = new SqlConnection(connectionString);
                    connection.Open();
                    //command.CommandText = "Set DateFormat dmy";
                    //command.Connection = connection;
                    SqlCommand cmd = new SqlCommand("Set DateFormat dmy", connection);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                return true;
            }
            catch (SqlException ex)
            {
                LastException = ex;
                return false;
            }
        }

        /// <summary>
        /// Closes active connection
        /// </summary>
        public void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }

        public SqlTransaction BeginTransaction()
        {

            //if (connection.State != ConnectionState.Closed)
            //    currentTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            //return currentTransaction;

            if (connection.State != ConnectionState.Closed)
                currentTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            return currentTransaction;
        }

        public void CommitTransaction()
        {
            if (this.currentTransaction == null)
                currentTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            if (currentTransaction.Connection.State == ConnectionState.Open)
                currentTransaction.Commit();
            this.currentTransaction = null;
        }

        public void RollBackTransaction()
        {
            if (this.currentTransaction == null)
                currentTransaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            currentTransaction.Rollback();
            this.currentTransaction = null;
        }

        /// <summary>
        /// Executes the query and returns a full dataset
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataset(string commandText)
        {
            DataSet ds = new DataSet();
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            //command.ResetCommandTimeout();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(ds);
            dataAdapter.Dispose();
            command.Dispose();
            return ds;
        }

        public DataTable ExecuteDataTable(string commandText)
        {
            DataSet ds = new DataSet();
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            return command.ExecuteReader().GetSchemaTable();
        }

        //log dosyasına yeni kayıt ekliyor
        public void ExecutePrivateDataset(string sql)
        {
            /*
            if (userFK==0)
            {
                return ;
            }
            sql=sql.Trim();
            string substr=sql.Substring(0,10);
            bool select = substr.Contains("SELECT") || substr.Contains("Select") || substr.Contains("select");
            bool insert = substr.Contains("INSERT") || substr.Contains("Insert") || substr.Contains("insert");
            bool update = substr.Contains("UPDATE") || substr.Contains("Update") || substr.Contains("update");
            bool delete = substr.Contains("DELETE") || substr.Contains("Delete") || substr.Contains("delete");
            string state = "";
            if (select)
            {
                state = "select";
                return ;
            }
            if (insert) state = "insert";
            if (update) state = "update";
            if (delete) state = "delete";

            
            InsertUpdateUtil util = new InsertUpdateUtil();
            util.TableName = "log";
            util.PrimaryKey = "logPK";
            util.Columns.Add("userFK", userFK);
            DateTime dt= new Act().GetServerDate();
            util.Columns.Add("operation_date",dt);
            util.Columns.Add("ip", ip[0].ToString());
            util.Columns.Add("host_name", host_name);
            util.Columns.Add("sql_string", sql);
            util.Columns.Add("state", state);
            util.AddNew();
            /*
            DataSet ds = new DataSet();
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            this.OpenConnection();
            string str = this.ReadConfig(); //Properties.Settings.Default["ConnectionString"].ToString();
            SqlConnection cnn=new SqlConnection(str);
            cnn.Open();
            sql=sql.Replace("'","");
            
            string commandText = "insert into log (userFK,operation_date,ip,host_name,sql_string,params,state) values (" + userFK + ",'" + DateTime.Now + "','" + ip[0].ToString() + "','" +host_name+"','"+ sql + "','','"+state+"')";
            SqlCommand command = new SqlCommand(commandText, cnn);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(ds);
            dataAdapter.Dispose();
            command.Dispose();
            cnn.Dispose(); 
            return ds;
             */
        }

        /// <summary>
        /// Executes the query and returns a full dataset
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataset(string commandText, CommandType commandType)
        {
            DataSet ds = new DataSet();
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(ds);
            dataAdapter.Dispose();
            command.Dispose();
            return ds;
        }

        /// <summary>
        /// Executes the query and returns a full dataset
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataset(string commandText, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);

            }
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(ds);
            dataAdapter.Dispose();
            command.Dispose();
            return ds;
        }

        public DataSet ExecuteDataset(string commandText, Dictionary<string, SqlParameter> parameters)
        {
            DataSet ds = new DataSet();
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            foreach (string key in parameters.Keys)
            {
                command.Parameters.Add(parameters[key]);
            }
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(ds);
            dataAdapter.Dispose();
            command.Dispose();
            return ds;
        }


        /// <summary>
        /// Executes the query and returns a full dataset
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataset(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(ds);
            dataAdapter.Dispose();
            command.Dispose();
            return ds;
        }

        /// <summary>
        /// Executes the query and returns an open SqlDataReader
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string commandText)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            SqlDataReader reader = command.ExecuteReader();
            command.Dispose();
            return reader;
        }

        /// <summary>
        /// Executes the query and returns an open SqlDataReader
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteReader(string commandText, CommandType commandType)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            SqlDataReader reader = command.ExecuteReader();
            command.Dispose();
            return reader;
        }

        /// <summary>
        /// Executes the query and returns an open SqlDataReader
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            SqlDataReader reader = command.ExecuteReader();
            command.Dispose();
            return reader;
        }

        /// <summary>
        /// Executes the query and returns an open SqlDataReader
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="commandType">Command Type</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns></returns>

        public SqlConnection baglanti()
        {
            SqlConnection _con = new SqlConnection();
            _con = connection;
            return _con;
        }

        public SqlDataReader ExecuteReader(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            SqlDataReader reader = command.ExecuteReader();
            command.Dispose();
            return reader;
        }

        /// <summary>
        /// Executes the query and returns the count of effected rows by the query (eg. update clause)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <returns>Number Of Rows Effected</returns>
        public int ExecuteNoneQuery(string commandText)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            int rowsEffected = command.ExecuteNonQuery();
            command.Dispose();
            // this.CloseConnection();
            return rowsEffected;
        }

        /// <summary>
        /// Executes the query and returns the count of effected rows by the query (eg. update clause)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="commandType">Command Type</param>
        /// <returns>Number Of Rows Effected</returns>
        public int ExecuteNoneQuery(string commandText, CommandType commandType)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            int rowsEffected = command.ExecuteNonQuery();
            command.Dispose();
            return rowsEffected;
        }

        /// <summary>
        /// Executes the query and returns the count of effected rows by the query (eg. update clause)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>Number Of Rows Effected</returns>
        public int ExecuteNoneQuery(string commandText, params SqlParameter[] parameters)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);

            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;

            int rowsEffected = command.ExecuteNonQuery();
            command.Dispose();
            return rowsEffected;
        }

        /// <summary>
        /// Executes the query and returns the count of effected rows by the query (eg. update clause)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="commandType">Command Type</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>Number Of Rows Effected</returns>
        public int ExecuteNoneQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            int rowsEffected = command.ExecuteNonQuery();
            command.Dispose();
            return rowsEffected;
        }

        /// <summary>
        /// Executes the query and returs the first column of the first row of the result set. Additional rows and columns are ignored. (eg. SUM, COUNT)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <returns>The first columns of the first row in the result set</returns>
        public object ExecuteScalar(string commandText)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            object firstCol = command.ExecuteScalar();
            command.Dispose();
            return firstCol;
        }

        /// <summary>
        /// Executes the query and returs the first column of the first row of the result set. Additional rows and columns are ignored. (eg. SUM, COUNT)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="commandType">Command Type</param>
        /// <returns>The first columns of the first row in the result set</returns>
        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            object firstCol = command.ExecuteScalar();
            command.Dispose();
            return firstCol;
        }

        /// <summary>
        /// Executes the query and returs the first column of the first row of the result set. Additional rows and columns are ignored. (eg. SUM, COUNT)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>The first columns of the first row in the result set</returns>
        public object ExecuteScalar(string commandText, params SqlParameter[] parameters)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            object firstCol = command.ExecuteScalar();
            command.Dispose();
            return firstCol;
        }

        /// <summary>
        /// Executes the query and returs the first column of the first row of the result set. Additional rows and columns are ignored. (eg. SUM, COUNT)
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <param name="commandType">Command Type</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>The first columns of the first row in the result set</returns>
        public object ExecuteScalar(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            this.OpenConnection();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandType = commandType;
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            object firstCol = command.ExecuteScalar();
            command.Dispose();
            return firstCol;
        }



        /// <summary>
        /// Executes the query and returns the last inserted identity value
        /// </summary>
        /// <param name="commandText">Sql String</param>
        /// <returns>Last Inserted Identity Value in This Scope</returns>
        public int ExecuteAndGetIdentity(string commandText)
        {
            //Business.ABusinessObject a = new ABusinessObject();
            this.OpenConnection();
            commandText += " SELECT SCOPE_IDENTITY() ";
            SqlCommand command = new SqlCommand(commandText, connection);
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            int identityValue = (int)command.ExecuteScalar();
            //int identityValue =a.GetInt(command.ExecuteScalar());
            command.Dispose();
            return identityValue;
        }

        public int ExecuteAndGetIdentity(string commandText, params SqlParameter[] parameters)
        {
            this.OpenConnection();
            commandText += " SELECT SCOPE_IDENTITY() ";
            SqlCommand command = new SqlCommand(commandText, connection);
            foreach (SqlParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            if (this.currentTransaction != null)
                command.Transaction = currentTransaction;
            int identityValue = (int)command.ExecuteScalar();
            command.Dispose();
            return identityValue;
        }

        /// <summary>
        /// Inserts a record to the database.
        /// </summary>
        /// <param name="TableName">TableName of The record off which the data will be inserted</param>
        /// <param name="parameters">Dictionary object of string keys and SqlParameter Values</param>
        /// <returns>Identity Value Of The Inserted Record</returns>
        /// <example><![CDATA[ db.InsertRecord("Users", dictParameters ]]></example>
        public int InsertRecord(string TableName, Dictionary<string, SqlParameter> parameters)
        {
            int retVal = -1;
            SqlCommand command = new SqlCommand();
            string commandText = "";
            string valueClause = "";
            commandText = "Insert Into " + TableName + "(";

            foreach (string key in parameters.Keys)
            {
                commandText += key + ",";
                command.Parameters.Add(parameters[key]);
                valueClause += parameters[key].ParameterName + ",";
            }
            this.OpenConnection();
            commandText = commandText.Substring(0, commandText.Length - 1);
            valueClause = valueClause.Substring(0, valueClause.Length - 1);
            commandText = commandText + ") Values(" + valueClause + ")";

            command.CommandText = commandText + " SELECT SCOPE_IDENTITY() ";
            command.Connection = connection;
            if (currentTransaction != null)
                command.Transaction = currentTransaction;
            retVal = Convert.ToInt32((command.ExecuteScalar()));
            command.Dispose();
            return retVal;
        }

        public bool UpdateRecord(string TableName, Dictionary<string, SqlParameter> parameters, string primaryKeyField, int privaryKeyValue)
        {
            int retVal = -1;
            SqlCommand cmd = new SqlCommand();
            string commandText = "";
            commandText = "Update " + TableName + " Set ";

            foreach (string key in parameters.Keys)
            {
                if (parameters[key].Value == null)
                {
                    commandText += key + " = null ,";

                }
                else
                {
                    commandText += key + " = " + parameters[key].ParameterName + ",";

                    cmd.Parameters.Add(parameters[key]);
                }

            }
            commandText = commandText.Substring(0, commandText.Length - 1);
            commandText += " Where " + primaryKeyField + " = " + privaryKeyValue.ToString();

            this.OpenConnection();
            cmd.CommandText = commandText;
            cmd.Connection = connection;
            if (currentTransaction != null)
                cmd.Transaction = currentTransaction;
            retVal = cmd.ExecuteNonQuery();
            cmd.Dispose();
            if (retVal > -1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Primary Key is String 
        /// Added by Cihan Dilman 15.11.2007
        /// </summary>
        public bool UpdateRecord(string TableName, Dictionary<string, SqlParameter> parameters, string primaryKeyField, string privaryKeyValue)
        {
            int retVal = -1;
            SqlCommand command = new SqlCommand();
            string commandText = "";
            commandText = "Update " + TableName + " Set ";

            foreach (string key in parameters.Keys)
            {
                if (parameters[key].Value == null)
                {
                    commandText += key + " = null ,";

                }
                else
                {
                    commandText += key + " = " + parameters[key].ParameterName + ",";

                    command.Parameters.Add(parameters[key]);
                }
            }
            commandText = commandText.Substring(0, commandText.Length - 1);
            commandText += " Where  (" + primaryKeyField + " = '" + privaryKeyValue.Trim() + "')";
            this.OpenConnection();
            command.CommandText = commandText;
            command.Connection = connection;
            if (currentTransaction != null)
                command.Transaction = currentTransaction;
            retVal = command.ExecuteNonQuery();
            command.Dispose();
            if (retVal > -1)
                return true;
            else
                return false;

            string s = "";
        }


        #endregion
    }

    public static class SqlUtil
    {

        public static DataSet SqlDatabaseDataset(string sql)
        {
            SqlDatabase db = new SqlDatabase();
            db.OpenConnection();
            //log tutmak için eklendi sorun çıkarsa aşağıdaki satırı açıklama  satırına dönderirsiniz
            //db.ExecutePrivateDataset(sql);
            DataSet ds = db.ExecuteDataset(sql);
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            db.CloseConnection();
            db.Dispose();
            return ds;
        }

        public static DataSet SqlDatabaseDataset(string sql, int pnConnectTimeOut)
        {
            SqlDatabase db = new SqlDatabase();
            db.OpenConnection();
            //log tutmak için eklendi sorun çıkarsa aşağıdaki satırı açıklama  satırına dönderirsiniz
            //db.ExecutePrivateDataset(sql);
            
            DataSet ds = db.ExecuteDataset(sql);
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            db.CloseConnection();
            db.Dispose();
            return ds;
        }

        public static DataTable GetColumnNamesTable(String sql)
        {
            SqlDatabase db = new SqlDatabase();
            db.OpenConnection();
            DataTable dt = db.ExecuteDataTable(sql);
            db.CloseConnection();
            db.Dispose();
            return dt;
        }

        public static DataSet SqlDatabaseDataset(string sql, params SqlParameter[] parameters)
        {
            SqlDatabase db = new SqlDatabase();
            db.OpenConnection();
            //log tutmak için eklendi sorun çıkarsa aşağıdaki satırı açıklama  satırına dönderirsiniz
            //db.ExecutePrivateDataset(sql);
            DataSet ds = db.ExecuteDataset(sql, parameters);
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            db.CloseConnection();
            db.Dispose();
            return ds;

        }

        internal static long RowCount(string commandText, params SqlParameter[] parameters)
        {
            long cnt = 0;
            SqlDatabase db = new SqlDatabase();
            db.OpenConnection();
            DataSet ds = db.ExecuteDataset(commandText, parameters);
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            cnt = ds.Tables[0].Rows.Count;
            ds.Dispose();
            db.CloseConnection();
            db.Dispose();
            return cnt;
        }

        internal static long RowCount(string commandText)
        {
            long cnt = 0;
            SqlDatabase db = new SqlDatabase();
            db.OpenConnection();
            DataSet ds = db.ExecuteDataset(commandText);
            ds.Locale = System.Globalization.CultureInfo.CurrentCulture;
            cnt = ds.Tables[0].Rows.Count;
            ds.Dispose();
            db.CloseConnection();
            db.Dispose();
            return cnt;
        }
    }

    public class InsertUpdateUtil
    {
        //PriceApply clas = new PriceApply();
        private SqlDatabase db = new SqlDatabase();

        public InsertUpdateUtil()
        {
            db.OpenConnection();
        }

        public InsertUpdateUtil(string tableName)
        {
            this.TableName = tableName;
            db.OpenConnection();
        }

        public InsertUpdateUtil(string tableName, string primaryKey)
        {
            this.TableName = tableName;
            this.PrimaryKey = primaryKey;
            db.OpenConnection();
        }

        public InsertUpdateUtil(string tableName, string primaryKey, int primaryValue)
        {
            this.TableName = tableName;
            this.PrimaryKey = primaryKey;
            this.PrimaryValue = primaryValue;
            db.OpenConnection();
        }

        private string _TableName;
        public string TableName { get { return _TableName; } set { _TableName = value; } }

        private string _PrimaryKey;
        public string PrimaryKey { get { return _PrimaryKey; } set { _PrimaryKey = value; } }

        private Dictionary<string, object> _Columns = new Dictionary<string, object>();
        public Dictionary<string, object> Columns { get { return _Columns; } }

        private int _PrimaryValue;
        public int PrimaryValue { get { return _PrimaryValue; } set { _PrimaryValue = value; } }


        public void BeginTransaction()
        {
            db.BeginTransaction();
        }

        public void CommitTransaction()
        {
            db.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            db.RollBackTransaction();
        }

        public void OpenConnection()
        {
            db.OpenConnection();
        }

        public void CloseConnection()
        {
            db.CloseConnection();
        }

        /// <summary>
        /// Tabloya satır ekler
        /// </summary>
        /// <returns>Eklenen satırın Identity Column değeri (int) cast edilerek</returns>
        public int AddNew()
        {
            int retVal = -1;
            Dictionary<string, SqlParameter> sqlParams = new Dictionary<string, SqlParameter>();
            foreach (string key in this.Columns.Keys)
            {
                sqlParams.Add(key, new SqlParameter("@" + key, this.Columns[key]));
            }

            retVal = db.InsertRecord(this.TableName, sqlParams);
            return retVal;
        }

        /// <summary>
        /// Tablodaki satırı update eder. TableName, Columns, PrimaryKey ve PrimaryValue değerleri dolu olmalıdır.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            bool retVal = false;
            Dictionary<string, SqlParameter> sqlParams = new Dictionary<string, SqlParameter>();
            foreach (string key in this.Columns.Keys)
            {

                sqlParams.Add(key, new SqlParameter("@" + key, this.Columns[key]));
            }

            retVal = db.UpdateRecord(this.TableName, sqlParams, this.PrimaryKey, this.PrimaryValue);
            return retVal;
        }
        public void Delete()
        {
            this.Delete(this.PrimaryKey, this.PrimaryValue);
        }
        public void Delete(string tablename)
        {
            string commandText = "Delete from " + this.TableName;
            try
            {
                db.ExecuteNoneQuery(commandText);
            }
            catch (Exception ex)
            {
                ;
            }

        }

        public void Delete(string fieldName, object fieldValue)
        {
            string commandText = "Delete from " + this.TableName + " Where " + fieldName + " = @param1";
            SqlParameter param1 = new SqlParameter("@param1", fieldValue);
            try
            {
                db.ExecuteNoneQuery(commandText, param1);
            }
            catch (Exception ex)
            {
                ;
            }
        }

        public void Delete(Dictionary<string, object> dictFieldsVals)
        {
            string commandText = "Delete from " + this.TableName + " Where ";
            SqlParameter[] parameters = new SqlParameter[dictFieldsVals.Count];
            int index = 0;
            foreach (string field_name in dictFieldsVals.Keys)
            {
                parameters.SetValue(new SqlParameter(field_name, dictFieldsVals[field_name]), index);
                commandText += field_name + " = @" + parameters[index].ParameterName + " AND ";
                index++;
            }
            commandText = commandText.Substring(0, commandText.Length - 4);
            db.ExecuteDataset(commandText, parameters);
            try
            {
                if (TableName.Equals("act"))
                {
                    //object app_date = this.Columns["approval_date"];
                    //object pvpk = this.Columns["pvFK"];
                }
            }
            catch (Exception ex)
            {
                ;
            }
        }

        public int RecordCount(string commandText)
        {
            DataSet ds = db.ExecuteDataset(commandText);
            return ds.Tables[0].Rows.Count;
        }

        public int RecordCount(string commandText, params SqlParameter[] parameters)
        {
            DataSet ds = db.ExecuteDataset(commandText, parameters);
            return ds.Tables[0].Rows.Count;
        }

        public void ExecuteCommand(string commandText)
        {
            db.ExecuteNoneQuery(commandText);
        }

        public int ExecuteCommand(string commandText, params SqlParameter[] parameters)
        {
            return db.ExecuteNoneQuery(commandText, parameters);
        }

    }
}
