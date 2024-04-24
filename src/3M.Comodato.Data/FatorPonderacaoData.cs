using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class FatorPonderacaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public FatorPonderacaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref TB_PONDERACAO_pzEntity fator_Ponderacao)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFator_PonderacaoInsert");

                _db.AddInParameter(dbCommand, "@p_MIN_CLIENTES", DbType.Int64, fator_Ponderacao.MIN_CLIENTES);
                _db.AddInParameter(dbCommand, "@p_MAX_CLIENTES", DbType.Int64, fator_Ponderacao.MAX_CLIENTES);
                _db.AddInParameter(dbCommand, "@p_FATOR", DbType.Int32, fator_Ponderacao.FATOR);
                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, fator_Ponderacao.nidUsuarioAtualizacao);

                
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

        
        public bool Alterar(TB_PONDERACAO_pzEntity fator_Ponderacao)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFator_PonderacaoUpdate");

                _db.AddInParameter(dbCommand, "@p_Id", DbType.Int64, fator_Ponderacao.ID);
                _db.AddInParameter(dbCommand, "@p_MIN_CLIENTES", DbType.Int64, fator_Ponderacao.MIN_CLIENTES);
                _db.AddInParameter(dbCommand, "@p_MAX_CLIENTES", DbType.Int64, fator_Ponderacao.MAX_CLIENTES);
                _db.AddInParameter(dbCommand, "@p_FATOR", DbType.Int32, fator_Ponderacao.FATOR);
                _db.AddInParameter(dbCommand, "@p_Ativo", DbType.Boolean, fator_Ponderacao.Ativo);
                

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

        public List<TB_PONDERACAO_pzEntity> ObterListaEntity()
        {
            var lista = new List<TB_PONDERACAO_pzEntity>();
            var reader = this.ObterLista().CreateDataReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    lista.Add(MontarObjeto(reader));
                }
            }

            if (reader != null)
            {
                reader.Dispose();
            }

            return lista;
        }

        public DataTable ObterLista()
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFator_PonderacaoSelect");
                
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

        public DataTable ObterPonderacaoPorId(int Id)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFator_PonderacaoSelectPorId");

                _db.AddInParameter(dbCommand, "@p_Id", DbType.Int64, Id);

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

        public DataTable ObterPonderacao()
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFator_PonderacaoMinMaxSelect");

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

        public DataTable ObterPorId(int id)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFator_PonderacaoSelectPorId");

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

        private TB_PONDERACAO_pzEntity MontarObjeto(DataTableReader reader)
        {
            var obj = new TB_PONDERACAO_pzEntity();
            if (!Convert.IsDBNull(reader["ID"])) 
                obj.ID = Convert.ToInt32(reader["ID"].ToString());

            if (!Convert.IsDBNull(reader["MIN_CLIENTES"]))
                obj.MIN_CLIENTES = Convert.ToInt64(reader["MIN_CLIENTES"].ToString());

            if (!Convert.IsDBNull(reader["MAX_CLIENTES"]))
                obj.MAX_CLIENTES = Convert.ToInt64(reader["MAX_CLIENTES"].ToString());

            if (!Convert.IsDBNull(reader["FATOR"]))
                obj.FATOR = Convert.ToInt32(reader["FATOR"].ToString());

            if (!Convert.IsDBNull(reader["DataInclusao"]))
                obj.dtmDataHoraAtualizacao = Convert.ToDateTime(reader["DataInclusao"].ToString());

            if (!Convert.IsDBNull(reader["nidUsuario"]))
                obj.nidUsuarioAtualizacao = Convert.ToInt64(reader["nidUsuario"].ToString());

            if (!Convert.IsDBNull(reader["Ativo"]))
                obj.Ativo = Convert.ToBoolean(reader["Ativo"].ToString());
            
            return obj;
        }

        public void Excluir(TB_PONDERACAO_pzEntity empresa)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTB_PONDERACAODelete");

                _db.AddInParameter(dbCommand, "@p_ID", DbType.Int32, empresa.ID);

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

    }
}
