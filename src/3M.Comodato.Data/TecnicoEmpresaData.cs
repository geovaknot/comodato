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
    public class TecnicoEmpresaData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TecnicoEmpresaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(TecnicoEmpresaEntity tecnicoEmpresa)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoEmpresaSelect");

                if (!string.IsNullOrEmpty(tecnicoEmpresa.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, tecnicoEmpresa.CD_TECNICO);

                if (tecnicoEmpresa.CD_EMPRESA != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Empresa", DbType.Int32, tecnicoEmpresa.CD_EMPRESA);

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

        public bool Inserir(ref TecnicoEmpresaEntity tecnicoEmpresaEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTecnicoEmpresaInsert");

                if (!string.IsNullOrEmpty(tecnicoEmpresaEntity.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, tecnicoEmpresaEntity.CD_TECNICO);

                if (tecnicoEmpresaEntity.CD_EMPRESA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Empresa", DbType.String, tecnicoEmpresaEntity.CD_EMPRESA);

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

        public void Excluir(TecnicoEmpresaEntity tecnicoEmpresaEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTecnicoEmpresaDelete");

                _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, tecnicoEmpresaEntity.CD_TECNICO);

                if (tecnicoEmpresaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoEmpresaEntity.nidUsuarioAtualizacao);

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
