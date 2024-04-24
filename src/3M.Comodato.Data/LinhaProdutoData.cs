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
    public class LinhaProdutoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LinhaProdutoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(LinhaProdutoEntity LinhaProduto)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLinhaProdutoSelect");

                if (LinhaProduto.CD_LINHA_PRODUTO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int32, LinhaProduto.CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(LinhaProduto.DS_LINHA_PRODUTO))
                    _db.AddInParameter(dbCommand, "@p_DS_LINHA_PRODUTO", DbType.String, LinhaProduto.DS_LINHA_PRODUTO);

                if (LinhaProduto.VL_EXPECTATIVA_PADRAO != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_EXPECTATIVA_PADRAO", DbType.Decimal, LinhaProduto.VL_EXPECTATIVA_PADRAO);

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

        public bool Inserir(ref LinhaProdutoEntity LinhaProduto)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcLinhaProdutoInsert");

                _db.AddInParameter(dbCommand, "@p_DS_LINHA_PRODUTO", DbType.String, LinhaProduto.DS_LINHA_PRODUTO);
                _db.AddInParameter(dbCommand, "@p_VL_EXPECTATIVA_PADRAO", DbType.Decimal, LinhaProduto.VL_EXPECTATIVA_PADRAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, LinhaProduto.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                LinhaProduto.CD_LINHA_PRODUTO = Convert.ToInt32(_db.GetParameterValue(dbCommand, "@p_CD_LINHA_PRODUTO"));

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

        public void Excluir(LinhaProdutoEntity linhaProduto)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcLinhaProdutoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, linhaProduto.CD_LINHA_PRODUTO);

                if (linhaProduto.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, linhaProduto.nidUsuarioAtualizacao);

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

        public bool Alterar(LinhaProdutoEntity linhaProduto)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLinhaProdutoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, linhaProduto.CD_LINHA_PRODUTO);

                if (!string.IsNullOrEmpty(linhaProduto.DS_LINHA_PRODUTO))
                    _db.AddInParameter(dbCommand, "@p_DS_LINHA_PRODUTO", DbType.String, linhaProduto.DS_LINHA_PRODUTO);

                _db.AddInParameter(dbCommand, "@p_VL_EXPECTATIVA_PADRAO", DbType.Decimal, linhaProduto.VL_EXPECTATIVA_PADRAO);

                if (linhaProduto.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, linhaProduto.nidUsuarioAtualizacao);

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

        public IList<LinhaProdutoEntity> ObterListaLinhaProdutoSinc()
        {
            try
            {

                IList<LinhaProdutoEntity> listaLinhaProduto = new List<LinhaProdutoEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select lp.* from tb_Linha_produto lp ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        //cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            LinhaProdutoEntity linhaProduto = new LinhaProdutoEntity();
                            linhaProduto.CD_LINHA_PRODUTO = Convert.ToInt32(SDR["CD_LINHA_PRODUTO"].ToString());
                            linhaProduto.DS_LINHA_PRODUTO = Convert.ToString(SDR["DS_LINHA_PRODUTO"] is DBNull ? "" : SDR["DS_LINHA_PRODUTO"].ToString());
                            linhaProduto.VL_EXPECTATIVA_PADRAO = Convert.ToDecimal(SDR["VL_EXPECTATIVA_PADRAO"] is DBNull ? "0" : SDR["VL_EXPECTATIVA_PADRAO"].ToString());

                            listaLinhaProduto.Add(linhaProduto);
                        }
                        cnx.Close();
                        return listaLinhaProduto;
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
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
