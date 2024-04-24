using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace _3M.Comodato.Data
{
    public class ContratoData
    {
        readonly Database _db;
        DbCommand dbCommand;


        public ContratoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(ContratoEntity contrato)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcContratoSelect");

                if (contrato.ID_CONTRATO != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_CONTRATO", DbType.Int64, contrato.ID_CONTRATO);

                if (contrato.Cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, contrato.Cliente.CD_CLIENTE);

                if (contrato.DT_EMISSAO != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_DT_EMISSAO", DbType.String, contrato.DT_EMISSAO);

                if (contrato.DT_RECEBIMENTO.HasValue)
                    _db.AddInParameter(dbCommand, "@p_DT_RECEBIMENTO", DbType.String, contrato.DT_RECEBIMENTO.Value);

                if (contrato.DT_OK.HasValue)
                    _db.AddInParameter(dbCommand, "@p_DT_OK", DbType.String, contrato.DT_OK.Value);

                if (!string.IsNullOrEmpty(contrato.Status.CD_STATUS_CONTRATO))
                    _db.AddInParameter(dbCommand, "@p_CD_STATUS_CONTRATO", DbType.String, contrato.Status.CD_STATUS_CONTRATO);

                if (contrato.NR_CONTRATO != 0)
                    _db.AddInParameter(dbCommand, "@p_NR_CONTRATO", DbType.String, contrato.NR_CONTRATO);

                if (!string.IsNullOrEmpty(contrato.TX_OBS))
                    _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, contrato.TX_OBS);

                if (!string.IsNullOrEmpty(contrato.TX_CONTRATO_TIPO))
                    _db.AddInParameter(dbCommand, "@p_TX_CONTRATO_TIPO", DbType.String, contrato.TX_CONTRATO_TIPO);

                if (!string.IsNullOrEmpty(contrato.DS_CLAUSULAS))
                    _db.AddInParameter(dbCommand, "@p_DS_CLAUSULAS", DbType.String, contrato.DS_CLAUSULAS);

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

        public bool Inserir(ref ContratoEntity contrato)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcContratoInsert");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, contrato.Cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_DT_EMISSAO", DbType.Date, contrato.DT_EMISSAO);
                _db.AddInParameter(dbCommand, "@p_DT_RECEBIMENTO", DbType.Date, contrato.DT_RECEBIMENTO);
                _db.AddInParameter(dbCommand, "@p_DT_OK", DbType.Date, contrato.DT_OK);
                _db.AddInParameter(dbCommand, "@p_CD_STATUS_CONTRATO", DbType.String, contrato.Status.CD_STATUS_CONTRATO);
                _db.AddInParameter(dbCommand, "@p_NR_CONTRATO", DbType.Decimal, contrato.NR_CONTRATO);
                _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, contrato.TX_OBS);
                _db.AddInParameter(dbCommand, "@p_TX_CONTRATO_TIPO", DbType.String, contrato.TX_CONTRATO_TIPO);
                _db.AddInParameter(dbCommand, "@p_DS_CLAUSULAS", DbType.String, contrato.DS_CLAUSULAS);

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Decimal, contrato.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_ID_CONTRATO", DbType.Decimal, 18);

                _db.ExecuteNonQuery(dbCommand);

                contrato.ID_CONTRATO = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_CONTRATO"));

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

        public void Excluir(ContratoEntity contrato)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcContratoDelete");

                _db.AddInParameter(dbCommand, "@p_ID_CONTRATO", DbType.Int64, contrato.ID_CONTRATO);

                if (contrato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, contrato.nidUsuarioAtualizacao);

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

        public bool Alterar(ContratoEntity contrato)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcContratoUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_CONTRATO", DbType.Int64, contrato.ID_CONTRATO);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, contrato.Cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_DT_EMISSAO", DbType.Date, contrato.DT_EMISSAO);

                if (contrato.DT_RECEBIMENTO.HasValue)
                    _db.AddInParameter(dbCommand, "@p_DT_RECEBIMENTO", DbType.Date, contrato.DT_RECEBIMENTO.Value);

                if (contrato.DT_OK.HasValue)
                    _db.AddInParameter(dbCommand, "@p_DT_OK", DbType.Date, contrato.DT_OK.Value);

                _db.AddInParameter(dbCommand, "@p_CD_STATUS_CONTRATO", DbType.String, contrato.Status.CD_STATUS_CONTRATO);
                _db.AddInParameter(dbCommand, "@p_NR_CONTRATO", DbType.Int64, contrato.NR_CONTRATO);
                _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, contrato.TX_OBS);
                _db.AddInParameter(dbCommand, "@p_DS_CLAUSULAS", DbType.String, contrato.DS_CLAUSULAS);
                _db.AddInParameter(dbCommand, "@p_TX_CONTRATO_TIPO", DbType.String, contrato.TX_CONTRATO_TIPO);


                if (contrato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, contrato.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);
                blnOK = true;

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
    }
}