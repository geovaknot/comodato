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
    public class RazaoComodatoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public RazaoComodatoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref RazaoComodatoEntity razaoComodatoEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcRazaoComodatoInsert");

               // _db.AddInParameter(dbCommand, "@p_CD_RAZAO_COMODATO", DbType.Int64, razaoComodatoEntity.CD_RAZAO_COMODATO);
                _db.AddInParameter(dbCommand, "@p_DS_RAZAO_COMODATO", DbType.String, razaoComodatoEntity.DS_RAZAO_COMODATO);

                if (razaoComodatoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, razaoComodatoEntity.nidUsuarioAtualizacao);
                }
                _db.AddOutParameter(dbCommand, "@p_CD_RAZAO_COMODATO", DbType.Int64, 3);
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

        public void Excluir(RazaoComodatoEntity razaoComodatoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcRazaoComodatoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_RAZAO_COMODATO", DbType.Int64, razaoComodatoEntity.CD_RAZAO_COMODATO);

                if (razaoComodatoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, razaoComodatoEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(RazaoComodatoEntity razaoComodatoEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRazaoComodatoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_RAZAO_COMODATO", DbType.String, razaoComodatoEntity.CD_RAZAO_COMODATO);

                _db.AddInParameter(dbCommand, "@p_DS_RAZAO_COMODATO", DbType.String, razaoComodatoEntity.DS_RAZAO_COMODATO);


                if (razaoComodatoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, razaoComodatoEntity.nidUsuarioAtualizacao);


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

        public DataTable ObterLista(RazaoComodatoEntity razaoComodatoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRazaoComodatoSelect");

                if (razaoComodatoEntity.CD_RAZAO_COMODATO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_RAZAO_COMODATO", DbType.Int64, razaoComodatoEntity.CD_RAZAO_COMODATO);

                if (!string.IsNullOrEmpty(razaoComodatoEntity.DS_RAZAO_COMODATO))
                    _db.AddInParameter(dbCommand, "@p_DS_RAZAO_COMODATO", DbType.String, razaoComodatoEntity.DS_RAZAO_COMODATO);

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
