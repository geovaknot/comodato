using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class DepreciacaoData
    {
        readonly Database _db;
        DbCommand dbCommand;
        public DepreciacaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref DepreciacaoEntity Depreciacao)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcDepreciacaoInsert");

                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, Depreciacao.CD_ATIVO_FIXO);
                _db.AddInParameter(dbCommand, "@p_NR_MESES", DbType.Int64, Depreciacao.NR_MESES);
                _db.AddInParameter(dbCommand, "@p_DT_INICIO_DEPREC", DbType.DateTime, Depreciacao.DT_INICIO_DEPREC);
                _db.AddInParameter(dbCommand, "@p_VL_CUSTO_ATIVO", DbType.Decimal, Depreciacao.VL_CUSTO_ATIVO);
                _db.AddInParameter(dbCommand, "@p_VL_DEPREC_TOTAL", DbType.Decimal, Depreciacao.VL_DEPREC_TOTAL);
                _db.AddInParameter(dbCommand, "@p_VL_DEPREC_ULT_MES", DbType.Decimal, Depreciacao.VL_DEPREC_ULT_MES);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Depreciacao.nidUsuarioAtualizacao);

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

        public void Excluir(DepreciacaoEntity Depreciacao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcDepreciacaoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, Depreciacao.CD_ATIVO_FIXO);

                if (Depreciacao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Depreciacao.nidUsuarioAtualizacao);

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

        public bool Alterar(DepreciacaoEntity Depreciacao)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDepreciacaoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, Depreciacao.CD_ATIVO_FIXO);

                if (Depreciacao.NR_MESES != 0)
                    _db.AddInParameter(dbCommand, "@p_NR_MESES", DbType.Int64, Depreciacao.NR_MESES);

                if (Depreciacao.DT_INICIO_DEPREC != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIO_DEPREC", DbType.DateTime, Depreciacao.DT_INICIO_DEPREC);

                //if (Depreciacao.VL_CUSTO_ATIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_CUSTO_ATIVO", DbType.Decimal, Depreciacao.VL_CUSTO_ATIVO);

                //if (Depreciacao.VL_DEPREC_TOTAL != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_DEPREC_TOTAL", DbType.Decimal, Depreciacao.VL_DEPREC_TOTAL);

                //if (Depreciacao.VL_DEPREC_ULT_MES != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_DEPREC_ULT_MES", DbType.Decimal, Depreciacao.VL_DEPREC_ULT_MES);

                if (Depreciacao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Depreciacao.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(DepreciacaoEntity Depreciacao)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDepreciacaoSelect");

                if (!string.IsNullOrEmpty(Depreciacao.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, Depreciacao.CD_ATIVO_FIXO);

                if (Depreciacao.NR_MESES != 0)
                    _db.AddInParameter(dbCommand, "@p_NR_MESES", DbType.Int64, Depreciacao.NR_MESES);

                if (Depreciacao.DT_INICIO_DEPREC != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIO_DEPREC", DbType.DateTime, Depreciacao.DT_INICIO_DEPREC);

                if (Depreciacao.VL_CUSTO_ATIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_CUSTO_ATIVO", DbType.Decimal, Depreciacao.VL_CUSTO_ATIVO);

                if (Depreciacao.VL_DEPREC_TOTAL != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_DEPREC_TOTAL", DbType.Decimal, Depreciacao.VL_DEPREC_TOTAL);

                if (Depreciacao.VL_DEPREC_ULT_MES != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_DEPREC_ULT_MES", DbType.Decimal, Depreciacao.VL_DEPREC_ULT_MES);

                if (Depreciacao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Depreciacao.nidUsuarioAtualizacao);

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
