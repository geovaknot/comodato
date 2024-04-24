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
    public class ExecutivoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public ExecutivoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref ExecutivoEntity executivoEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcExecutivoInsert");

               
                _db.AddInParameter(dbCommand, "@p_NM_EXECUTIVO", DbType.String, executivoEntity.NM_EXECUTIVO);

                if (executivoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, executivoEntity.nidUsuarioAtualizacao);
                }
                _db.AddOutParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Int64, 3);
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

        public void Excluir(ExecutivoEntity executivoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcExecutivoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Int64, executivoEntity.CD_EXECUTIVO);

                if (executivoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, executivoEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(ExecutivoEntity executivoEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcExecutivoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.String, executivoEntity.CD_EXECUTIVO);

                _db.AddInParameter(dbCommand, "@p_NM_EXECUTIVO", DbType.String, executivoEntity.NM_EXECUTIVO);


                if (executivoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, executivoEntity.nidUsuarioAtualizacao);


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

        public DataTable ObterLista(ExecutivoEntity executivoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcExecutivoSelect");

                if (executivoEntity.CD_EXECUTIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Int64, executivoEntity.CD_EXECUTIVO);

                if (!string.IsNullOrEmpty(executivoEntity.NM_EXECUTIVO))
                    _db.AddInParameter(dbCommand, "@p_NM_EXECUTIVO", DbType.String, executivoEntity.NM_EXECUTIVO);

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
