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
    public class StatusAtivoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public StatusAtivoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref StatusAtivoEntity statusAtivoEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcStatusAtivoInsert");

                // _db.AddInParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.String, statusAtivo.CD_STATUS_ATIVO);
                _db.AddInParameter(dbCommand, "@p_DS_STATUS_ATIVO", DbType.String, statusAtivoEntity.DS_STATUS_ATIVO);

                if (statusAtivoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, statusAtivoEntity.nidUsuarioAtualizacao);
                }
                _db.AddOutParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.Int64, 18);
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

        public void Excluir(StatusAtivoEntity statusAtivo)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcStatusAtivoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.String, statusAtivo.CD_STATUS_ATIVO);

                if (statusAtivo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, statusAtivo.nidUsuarioAtualizacao);

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

        public bool Alterar(StatusAtivoEntity statusAtivo)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusAtivoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.String, statusAtivo.CD_STATUS_ATIVO);

                _db.AddInParameter(dbCommand, "@p_DS_STATUS_ATIVO", DbType.String, statusAtivo.DS_STATUS_ATIVO);


                if (statusAtivo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, statusAtivo.nidUsuarioAtualizacao);


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

        public DataTable ObterLista(StatusAtivoEntity statusAtivo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusAtivoSelect");

                if (statusAtivo.CD_STATUS_ATIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.Int64, statusAtivo.CD_STATUS_ATIVO);

                if (!string.IsNullOrEmpty(statusAtivo.DS_STATUS_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_DS_STATUS_ATIVO", DbType.String, statusAtivo.DS_STATUS_ATIVO);

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
