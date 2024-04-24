using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class DashboardData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public DashboardData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterBoxEquipamentoComodatoLocado(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null, Int64? CD_TIPO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxEquipamentoComodatoLocadoSelect");
                connection = _db.CreateConnection();
                dbCommand.Connection = connection;

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                _db.AddInParameter(dbCommand, "@p_CD_TIPO", DbType.Int64, CD_TIPO);
                

                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxClienteAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxClienteAtivoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxEquipamentoEnviado(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxEquipamentoEnviadoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxProjEnvioEquip(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxProjEnvioEquipSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxPecaEnviada(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxPecaEnviadaSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaCliente(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashListaClienteSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaModelo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashListaModeloSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaEquipamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            //DataTable tabela2 = new DataTable();
            //List<DataTable> Tabelas = new List<DataTable>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashListaEquipamentoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                //tabela2 = dataSet.Tables[1];
                //Tabelas.Add(dataTable);
                //Tabelas.Add(tabela2);

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterManutencaoHsPc(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable tabela1 = new DataTable();
            DataTable tabela2 = new DataTable();
            List<DataTable> Tabelas = new List<DataTable>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashInfoManutencao");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                tabela1 = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return tabela1;
        }

        
        public DataTable ObterListaEquipamentoWorkFlow(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashListaEquipamentoWorkFlowSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaHistorico(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashListaHistoricoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaHistoricoValores(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashListaHistoricoValoresSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaTecnico(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashListaTecnicoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxSolicitacaoAtendimentoPendente(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxSolicitacaoAtendimentoPendenteSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxAtendimentoAndamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxAtendimentoAndamentoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxAtendimentoVisitasAndamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxAtendimentoAndamentoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxUnitizacao(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxUnitizadorAtivoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxDistribuicaoKAT(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxDistribuicaoKATSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxAtivoEnviadoNaoInstalado(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxAtivoEnviadoNaoInstaladoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        //public DataTable ObterBoxExcecaoAtendimento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        //{

        //    DbConnection connection = null;
        //    DataSet dataSet = new DataSet();
        //    DataTable dataTable = new DataTable();

        //    try
        //    {
        //        dbCommand = _db.GetStoredProcCommand("prcDashBoxExcecaoAtendimentoSelect");

        //        if (!string.IsNullOrEmpty(CD_GRUPO))
        //            _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

        //        if (!string.IsNullOrEmpty(CLIENTE))
        //            _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

        //        if (!string.IsNullOrEmpty(CD_MODELO))
        //            _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

        //        if (CD_LINHA_PRODUTO != null)
        //            _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

        //        if (!string.IsNullOrEmpty(TECNICO))
        //            _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

        //        if (nidUsuario != null)
        //            _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

        //        if (CD_VENDEDOR != null)
        //            _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

        //        connection = _db.CreateConnection();
        //        dbCommand.Connection = connection;
        //        connection.Open();

        //        dbCommand.CommandTimeout = 2000;

        //        dataSet = _db.ExecuteDataSet(dbCommand);
        //        dataTable = dataSet.Tables[0];

        //    }
        //    catch (System.Data.SqlClient.SqlException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            connection.Close();
        //        }
        //    }
        //    return dataTable;
        //}

        public DataTable ObterBoxFechadorAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxFechadorAtivoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.String, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxIdentificadorAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxIdentificadorAtivoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxPeriodo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxPeriodoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxVenda(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxVendaSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxVendedor(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxVendedorSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxPecaTrocada(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxPecaTrocadaSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxPesquisaSatisfacao(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxPesquisaSatisfacaoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxLinhaProduto(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxLinhaProdutoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxAtendimento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxAtendimentoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterBoxEnvioEquipamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashBoxEnvioEquipamentoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoVolumeVenda(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoVolumeVendaSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoVenda(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoVendaSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoFamiliaModelo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoFamiliaModeloSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoValorPecaEnviadaMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoValorPecaEnviadaMesSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoTotalAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoTotalAtivoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoAtendimento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoAtendimentoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoTrocaPecaMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoTrocaPecaMesSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoAtendimentoTecnicoRegional(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoAtendimentoTecnicoRegionalSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoPeriodoRealizadoMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoPeriodoRealizadoMesSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoTipoEnvioEquipamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoTipoEnvioEquipamentoSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoEnvioEquipamentoLinhaProd(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoEnvioEquipamentoLinhaProdSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterGraficoMaquinaEnviadaDevolvidaMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDashGraficoMaquinaEnviadaDevolvidaMesSelect");

                if (!string.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!string.IsNullOrEmpty(CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CLIENTE", DbType.String, CLIENTE);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, CD_MODELO);

                if (CD_LINHA_PRODUTO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TECNICO", DbType.String, TECNICO);

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);

                if (CD_VENDEDOR != null)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, CD_VENDEDOR);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 2000;

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }
    }
}
