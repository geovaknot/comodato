using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace _3M.Comodato.Data
{
    public class EstoqueMoviData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public EstoqueMoviData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterListaMovimento(EstoqueMoviEntity filtroEstoque)
        {
            return this.ObterListaMovimento(filtroEstoque, DateTime.MinValue, DateTime.MinValue);
        }
        public DataTable ObterListaMovimento(EstoqueMoviEntity filtroEstoque, DateTime periodoInicio, DateTime periodoFim)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoqueMoviPermissaoSelect");

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();


                if (filtroEstoque.ESTOQUE_ORIGEM.ID_ESTOQUE > 0)
                    _db.AddInParameter(dbCommand, "@p_nidEstoque", DbType.Int64, filtroEstoque.ESTOQUE_ORIGEM.ID_ESTOQUE);

                if (!string.IsNullOrEmpty(filtroEstoque.Peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_cd_peca", DbType.String, filtroEstoque.Peca.CD_PECA);

                if (!string.IsNullOrEmpty(filtroEstoque.TP_MOVIMENTACAO.CD_TP_MOVIMENTACAO))
                    _db.AddInParameter(dbCommand, "@p_cd_tp_movimentacao", DbType.String, filtroEstoque.TP_MOVIMENTACAO.CD_TP_MOVIMENTACAO);


                if (periodoInicio != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_dt_inicio", DbType.Date, periodoInicio);

                if (periodoFim != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_dt_final", DbType.Date, periodoFim);

                if (filtroEstoque.ESTOQUE_ORIGEM.ID_USU_RESPONSAVEL > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, filtroEstoque.ESTOQUE_ORIGEM.ID_USU_RESPONSAVEL);

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

        public DataTable ObterListaTipoMovimento()
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTpEstoqueMoviSelect");

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

        public bool Inserir(ref EstoqueMoviEntity estoqueMovi)
        {
            return Inserir(ref estoqueMovi, null);
        }
        public bool Inserir(ref EstoqueMoviEntity estoqueMovi, SqlTransaction transaction)
        {
            bool retorno = false;

            try
            {
                SqlConnection sqlConnection = GetConnection(transaction);
                using (SqlCommand command = sqlConnection.CreateCommand())
                {
                    if (transaction != null)
                        command.Transaction = transaction;

                    command.CommandText = "prcEstoqueMoviInsert";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@p_CD_TP_MOVIMENTACAO", estoqueMovi.TP_MOVIMENTACAO.CD_TP_MOVIMENTACAO);

                    if (estoqueMovi.ID_OS.HasValue)
                        command.Parameters.AddWithValue("@p_ID_OS", estoqueMovi.ID_OS.Value);

                    command.Parameters.AddWithValue("@p_DT_MOVIMENTACAO", estoqueMovi.DT_MOVIMENTACAO);

                    if (estoqueMovi.ESTOQUE_DESTINO.ID_ESTOQUE != 0)
                        command.Parameters.AddWithValue("@p_ID_ESTOQUE", estoqueMovi.ESTOQUE_DESTINO.ID_ESTOQUE);

                    command.Parameters.AddWithValue("@p_CD_PECA", estoqueMovi.Peca.CD_PECA);
                    command.Parameters.AddWithValue("@p_QT_PECA", estoqueMovi.QT_PECA);
                    command.Parameters.AddWithValue("@p_ID_USU_MOVI", estoqueMovi.USU_MOVI.nidUsuario);

                    if (estoqueMovi.ESTOQUE_ORIGEM.ID_ESTOQUE != 0)
                        command.Parameters.AddWithValue("@p_ID_ESTOQUE_ORIGEM", estoqueMovi.ESTOQUE_ORIGEM.ID_ESTOQUE);

                    command.Parameters.AddWithValue("@p_TP_ENTRADA_SAIDA", estoqueMovi.TP_ENTRADA_SAIDA);

                    if (estoqueMovi.cliente.CD_CLIENTE > 0)
                        command.Parameters.AddWithValue("@p_CD_CLIENTE", estoqueMovi.cliente.CD_CLIENTE);

                    command.Parameters.AddWithValue("@p_nidUsuarioAtualizacao", estoqueMovi.nidUsuarioAtualizacao);
                    command.Parameters.Add(new SqlParameter() { Direction = ParameterDirection.Output, ParameterName = "@p_ID_ESTOQUE_MOVI", DbType = DbType.Int64 });

                    command.ExecuteNonQuery();

                    estoqueMovi.ID_ESTOQUE_MOVI = Convert.ToInt64(command.Parameters["@p_ID_ESTOQUE_MOVI"].Value);
                }

                CloseConnection(transaction, sqlConnection);

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

        private static void CloseConnection(SqlTransaction transaction, SqlConnection sqlConnection)
        {
            if (transaction == null)
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
            }
        }

        private SqlConnection GetConnection(SqlTransaction transaction)
        {
            SqlConnection sqlConnection;
            if (transaction == null)
            {
                sqlConnection = new SqlConnection(_db.ConnectionString);
                sqlConnection.Open();
            }
            else
            {
                sqlConnection = transaction.Connection;
            }

            return sqlConnection;
        }


        /// <summary>
        /// Obtem lista de movimentações de um estoque de um estoque de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<EstoqueMoviSinc> ObterListaEstoqueMoviSinc(Int64 idUsuario)
        {
            try
            {
                IList<EstoqueMoviSinc> listaEstoqueMovi = new List<EstoqueMoviSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select em.* from tbestoquemovi em " +
                                         " inner join tbestoque e ON e.ID_ESTOQUE = em.ID_ESTOQUE " +
                                         " inner join tb_tecnico t ON e.cd_tecnico = t.cd_tecnico " +
                                         " WHERE(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) AND DT_MOVIMENTACAO > GETDATE() -1 "; //+
                                         //" AND DT_MOVIMENTACAO > GETDATE() - Convert(int, (select cvlParametro as QTD from tbParametro where ccdParametro = 'qtdDiasRetroativos')) ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            EstoqueMoviSinc estoqueMovi = new EstoqueMoviSinc();
                            estoqueMovi.ID_ESTOQUE_MOVI = Convert.ToInt64(SDR["ID_ESTOQUE_MOVI"].ToString());
                            estoqueMovi.CD_TP_MOVIMENTACAO = Convert.ToChar(SDR["CD_TP_MOVIMENTACAO"] is DBNull ? " " : SDR["CD_TP_MOVIMENTACAO"].ToString());
                            estoqueMovi.ID_OS = Convert.ToInt64(SDR["ID_OS"] is DBNull ? 0 : SDR["ID_OS"]);
                            estoqueMovi.DT_MOVIMENTACAO = Convert.ToDateTime(SDR["DT_MOVIMENTACAO"] is DBNull ? "01/01/2000" : SDR["DT_MOVIMENTACAO"]);
                            estoqueMovi.ID_ESTOQUE = Convert.ToInt64(SDR["ID_ESTOQUE"] is DBNull ? 0 : SDR["ID_ESTOQUE"]);
                            estoqueMovi.CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString());
                            estoqueMovi.QT_PECA = Convert.ToInt64(SDR["QT_PECA"] is DBNull ? 0 : SDR["QT_PECA"]);
                            estoqueMovi.ID_USU_MOVI = Convert.ToInt64(SDR["ID_USU_MOVI"] is DBNull ? 0 : SDR["ID_USU_MOVI"]);
                            estoqueMovi.ID_ESTOQUE_ORIGEM = Convert.ToInt64(SDR["ID_ESTOQUE_ORIGEM"] is DBNull ? 0 : SDR["ID_ESTOQUE_ORIGEM"]);
                            estoqueMovi.TP_ENTRADA_SAIDA = Convert.ToChar(SDR["TP_ENTRADA_SAIDA"] is DBNull ? " " : SDR["TP_ENTRADA_SAIDA"].ToString());
                            estoqueMovi.CD_CLIENTE = Convert.ToInt64(SDR["CD_CLIENTE"] is DBNull ? 0 : SDR["CD_CLIENTE"]);
                            estoqueMovi.nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);
                            estoqueMovi.dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]);
                            listaEstoqueMovi.Add(estoqueMovi);
                        }
                        cnx.Close();
                        return listaEstoqueMovi;
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
