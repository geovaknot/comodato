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
    public class StatusContratoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public StatusContratoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref StatusContratoEntity statusContratoEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcStatusContratoInsert");

                _db.AddInParameter(dbCommand, "@p_CD_STATUS_CONTRATO", DbType.String, statusContratoEntity.CD_STATUS_CONTRATO);
                _db.AddInParameter(dbCommand, "@p_DS_STATUS_CONTRATO", DbType.String, statusContratoEntity.DS_STATUS_CONTRATO);

                if (statusContratoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, statusContratoEntity.nidUsuarioAtualizacao);
                }

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

        public void Excluir(StatusContratoEntity statusContrato)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcStatusContratoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_STATUS_CONTRATO", DbType.String, statusContrato.CD_STATUS_CONTRATO);

                if (statusContrato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, statusContrato.nidUsuarioAtualizacao);

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

        public bool Alterar(StatusContratoEntity statusContrato)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusContratoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_STATUS_CONTRATO", DbType.String, statusContrato.CD_STATUS_CONTRATO);

                _db.AddInParameter(dbCommand, "@p_DS_STATUS_CONTRATO", DbType.String, statusContrato.DS_STATUS_CONTRATO);


                if (statusContrato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, statusContrato.nidUsuarioAtualizacao);


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

        public DataTable ObterLista(StatusContratoEntity statusContrato)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusContratoSelect");

                if (!string.IsNullOrEmpty(statusContrato.CD_STATUS_CONTRATO)) 
                    _db.AddInParameter(dbCommand, "@p_CD_STATUS_CONTRATO", DbType.String, statusContrato.CD_STATUS_CONTRATO);

                if (!string.IsNullOrEmpty(statusContrato.DS_STATUS_CONTRATO))
                    _db.AddInParameter(dbCommand, "@p_DS_STATUS_CONTRATO", DbType.String, statusContrato.DS_STATUS_CONTRATO);

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
