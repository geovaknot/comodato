using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class ConsumivelData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public ConsumivelData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref ConsumivelEntity Consumivel)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcConsumivelInsert");

                _db.AddInParameter(dbCommand, "@p_CD_CONSUMIVEL", DbType.String, Consumivel.CD_CONSUMIVEL);
                _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int32, Consumivel.linhaProduto.CD_LINHA_PRODUTO);
                _db.AddInParameter(dbCommand, "@p_DS_CONSUMIVEL", DbType.String, Consumivel.DS_CONSUMIVEL);
                _db.AddInParameter(dbCommand, "@p_TX_UNIDADE", DbType.String, Consumivel.TX_UNIDADE);
                _db.AddInParameter(dbCommand, "@p_CD_COMMODITY", DbType.String, Consumivel.CD_COMMODITY);
                _db.AddInParameter(dbCommand, "@p_DS_COMMODITY", DbType.String, Consumivel.DS_COMMODITY);
                _db.AddInParameter(dbCommand, "@p_CD_MAJOR", DbType.String, Consumivel.CD_MAJOR);
                _db.AddInParameter(dbCommand, "@p_DS_MAJOR", DbType.String, Consumivel.DS_MAJOR);
                _db.AddInParameter(dbCommand, "@p_CD_FAMILY", DbType.String, Consumivel.CD_FAMILY);
                _db.AddInParameter(dbCommand, "@p_DS_FAMILY", DbType.String, Consumivel.DS_FAMILY);
                _db.AddInParameter(dbCommand, "@p_CD_SUB_FAMILY", DbType.String, Consumivel.CD_SUB_FAMILY);
                _db.AddInParameter(dbCommand, "@p_DS_SUB_FAMILY", DbType.String, Consumivel.DS_SUB_FAMILY);
                _db.AddInParameter(dbCommand, "@p_TX_UNIDADE_CONV", DbType.String, Consumivel.TX_UNIDADE_CONV);
                _db.AddInParameter(dbCommand, "@p_BPCS", DbType.Boolean, Consumivel.BPCS);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Consumivel.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

                retorno = true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public void Excluir(ConsumivelEntity Consumivel)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcConsumivelDelete");

                _db.AddInParameter(dbCommand, "@p_CD_CONSUMIVEL", DbType.String, Consumivel.CD_CONSUMIVEL);

                if (Consumivel.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Consumivel.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Alterar(ConsumivelEntity Consumivel)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcConsumivelUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_CONSUMIVEL", DbType.String, Consumivel.CD_CONSUMIVEL);
                _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int32, Consumivel.linhaProduto.CD_LINHA_PRODUTO);
                _db.AddInParameter(dbCommand, "@p_DS_CONSUMIVEL", DbType.String, Consumivel.DS_CONSUMIVEL);
                _db.AddInParameter(dbCommand, "@p_TX_UNIDADE", DbType.String, Consumivel.TX_UNIDADE);
                _db.AddInParameter(dbCommand, "@p_CD_COMMODITY", DbType.String, Consumivel.CD_COMMODITY);
                _db.AddInParameter(dbCommand, "@p_DS_COMMODITY", DbType.String, Consumivel.DS_COMMODITY);
                _db.AddInParameter(dbCommand, "@p_CD_MAJOR", DbType.String, Consumivel.CD_MAJOR);
                _db.AddInParameter(dbCommand, "@p_DS_MAJOR", DbType.String, Consumivel.DS_MAJOR);
                _db.AddInParameter(dbCommand, "@p_CD_FAMILY", DbType.String, Consumivel.CD_FAMILY);
                _db.AddInParameter(dbCommand, "@p_DS_FAMILY", DbType.String, Consumivel.DS_FAMILY);
                _db.AddInParameter(dbCommand, "@p_CD_SUB_FAMILY", DbType.String, Consumivel.CD_SUB_FAMILY);
                _db.AddInParameter(dbCommand, "@p_DS_SUB_FAMILY", DbType.String, Consumivel.DS_SUB_FAMILY);
                _db.AddInParameter(dbCommand, "@p_TX_UNIDADE_CONV", DbType.String, Consumivel.TX_UNIDADE_CONV);
                _db.AddInParameter(dbCommand, "@p_ST_ATIVO", DbType.Boolean, Consumivel.ST_ATIVO);
                if (Consumivel.BPCS != null)
                    _db.AddInParameter(dbCommand, "@p_BPCS", DbType.Boolean, Consumivel.BPCS);

                if (Consumivel.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Consumivel.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return blnOK;
        }

        public DataTable ObterLista(ConsumivelEntity Consumivel)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcConsumivelSelect");

                if (!string.IsNullOrEmpty(Consumivel.CD_CONSUMIVEL))
                    _db.AddInParameter(dbCommand, "@p_CD_CONSUMIVEL", DbType.String, Consumivel.CD_CONSUMIVEL);

                if (Consumivel.linhaProduto.CD_LINHA_PRODUTO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int32, Consumivel.linhaProduto.CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(Consumivel.CD_CONSUMIVEL))
                    _db.AddInParameter(dbCommand, "@p_DS_CONSUMIVEL", DbType.String, Consumivel.DS_CONSUMIVEL);

                if (!string.IsNullOrEmpty(Consumivel.TX_UNIDADE))
                    _db.AddInParameter(dbCommand, "@p_TX_UNIDADE", DbType.String, Consumivel.TX_UNIDADE);

                if (!string.IsNullOrEmpty(Consumivel.CD_COMMODITY))
                    _db.AddInParameter(dbCommand, "@p_CD_COMMODITY", DbType.String, Consumivel.CD_COMMODITY);

                if (!string.IsNullOrEmpty(Consumivel.DS_COMMODITY))
                    _db.AddInParameter(dbCommand, "@p_DS_COMMODITY", DbType.String, Consumivel.DS_COMMODITY);

                if (!string.IsNullOrEmpty(Consumivel.CD_MAJOR))
                    _db.AddInParameter(dbCommand, "@p_CD_MAJOR", DbType.String, Consumivel.CD_MAJOR);

                if (!string.IsNullOrEmpty(Consumivel.DS_MAJOR))
                    _db.AddInParameter(dbCommand, "@p_DS_MAJOR", DbType.String, Consumivel.DS_MAJOR);

                if (!string.IsNullOrEmpty(Consumivel.CD_FAMILY))
                    _db.AddInParameter(dbCommand, "@p_CD_FAMILY", DbType.String, Consumivel.CD_FAMILY);

                if (!string.IsNullOrEmpty(Consumivel.DS_FAMILY))
                    _db.AddInParameter(dbCommand, "@p_DS_FAMILY", DbType.String, Consumivel.DS_FAMILY);

                if (!string.IsNullOrEmpty(Consumivel.CD_SUB_FAMILY))
                    _db.AddInParameter(dbCommand, "@p_CD_SUB_FAMILY", DbType.String, Consumivel.CD_SUB_FAMILY);

                if (!string.IsNullOrEmpty(Consumivel.DS_SUB_FAMILY))
                    _db.AddInParameter(dbCommand, "@p_DS_SUB_FAMILY", DbType.String, Consumivel.DS_SUB_FAMILY);

                if (Consumivel.BPCS != null)
                    _db.AddInParameter(dbCommand, "@p_BPCS", DbType.Boolean, Consumivel.BPCS);

                //if (Consumivel.nidUsuarioAtualizacao > 0)
                //    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Consumivel.nidUsuarioAtualizacao);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

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

        public DataTable ObterListaBPCS(ConsumivelEntity Consumivel)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcConsumivelSelectBPCS");

                _db.AddInParameter(dbCommand, "@p_CD_CONSUMIVEL", DbType.String, Consumivel.CD_CONSUMIVEL);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

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
