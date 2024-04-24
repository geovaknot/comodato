using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace _3M.Comodato.Data
{
    public class EstoquePecaData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public EstoquePecaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterListaTecnico(string CD_TECNICO, string CD_PECA = null, string CD_MODELO = null)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoquePecaSelectTecnico");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                if (!string.IsNullOrEmpty(CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, CD_PECA);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, CD_MODELO);

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

        public DataTable ObterListaTecnicoPecas(string CD_TECNICO, string CD_GRUPO_MODELO = null, string CD_PECA = null)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoquePecaSelectTecnico");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                if (!string.IsNullOrEmpty(CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, CD_PECA);
                if (!string.IsNullOrEmpty(CD_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, CD_GRUPO_MODELO);

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
        public DataTable ObterListaClientePecas(Int64 CD_CLIENTE, string CD_GRUPO_MODELO = null, string CD_PECA = null)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoquePecaSelectCliente");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);

                if (!string.IsNullOrEmpty(CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, CD_PECA);
                if (!string.IsNullOrEmpty(CD_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, CD_GRUPO_MODELO);

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

        public DataTable ObterListaCliente(Int64 CD_CLIENTE, string CD_PECA = null, string CD_MODELO = null)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoquePecaSelectCliente");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);

                if (!string.IsNullOrEmpty(CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, CD_PECA);

                if (!string.IsNullOrEmpty(CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, CD_MODELO);

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

        public DataTable ObterListaPorEstoque(long nidEstoque, string ccdPeca, long nidUsuario)
        {
            return ObterListaPorEstoque(nidEstoque, ccdPeca, nidUsuario, null);
        }

        public DataTable ObterListaPorEstoque(long nidEstoque, string ccdPeca, long nidUsuario, SqlTransaction transaction)
        {
            //DbConnection connection = null;
            //DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                SqlConnection sqlConnection = GetConnection(transaction);
                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    if (transaction != null)
                        sqlCommand.Transaction = transaction;

                    sqlCommand.CommandText = "prcPecaEstoquePecaSelect";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    if (nidEstoque != 0)
                        sqlCommand.Parameters.AddWithValue("@p_ID_ESTOQUE", nidEstoque);

                    if (!string.IsNullOrEmpty(ccdPeca))
                        sqlCommand.Parameters.AddWithValue("@p_CD_PECA", ccdPeca);

                    if (nidUsuario != 0)
                        sqlCommand.Parameters.AddWithValue("@p_ID_USU_RESPONSAVEL", nidUsuario);

                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        sqlDataAdapter.Fill(dataTable);
                }

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;
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

        public DataTable ObterListaIntermediario(EstoquePecaEntity estoquePeca)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoquePecaIntermediarioSelect");

                if (estoquePeca.estoque.ID_ESTOQUE != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int32, estoquePeca.estoque.ID_ESTOQUE);

                if (!string.IsNullOrEmpty(estoquePeca.peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, estoquePeca.peca.CD_PECA);

                if (!string.IsNullOrEmpty(estoquePeca.peca.FL_ATIVO_PECA))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO_PECA", DbType.String, estoquePeca.peca.FL_ATIVO_PECA);

                if (estoquePeca.estoque.ID_USU_RESPONSAVEL != 0 && estoquePeca.estoque.ID_ESTOQUE == 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, estoquePeca.estoque.ID_USU_RESPONSAVEL);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (SqlException ex)
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

        public DataTable ObterLista(EstoquePecaEntity estoquePecaFiltro)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                if (null == estoquePecaFiltro)
                    estoquePecaFiltro = new EstoquePecaEntity();

                dbCommand = _db.GetStoredProcCommand("prcEstoquePecaSelect");

                if (estoquePecaFiltro.peca == null || !string.IsNullOrEmpty(estoquePecaFiltro.peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, estoquePecaFiltro.peca.CD_PECA);

                if (estoquePecaFiltro.estoque != null)
                {
                    if (estoquePecaFiltro.ID_ESTOQUE_PECA > 0)
                        _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE_PECA", DbType.Int64, estoquePecaFiltro.ID_ESTOQUE_PECA);

                    if (estoquePecaFiltro.estoque.ID_ESTOQUE > 0)
                        _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, estoquePecaFiltro.estoque.ID_ESTOQUE);

                    if (!string.IsNullOrEmpty(estoquePecaFiltro.estoque.TP_ESTOQUE_TEC_3M))
                        _db.AddInParameter(dbCommand, "@p_TP_ESTOQUE_TEC_3M", DbType.String, estoquePecaFiltro.estoque.TP_ESTOQUE_TEC_3M);

                    if (!string.IsNullOrEmpty(estoquePecaFiltro.estoque.FL_ATIVO))
                        _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoquePecaFiltro.estoque.FL_ATIVO);
                }


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

        public bool Inserir(ref EstoquePecaEntity estoquePeca)
        {
            return Inserir(ref estoquePeca);
        }
        public bool Inserir(ref EstoquePecaEntity estoquePeca, SqlTransaction transaction)
        {
            bool retorno = false;

            try
            {
                SqlConnection sqlConnection = GetConnection(transaction);

                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    if (transaction != null)
                        sqlCommand.Transaction = transaction;

                    sqlCommand.CommandText = "prcEstoquePecaInsert";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue("@p_CD_PECA", estoquePeca.peca.CD_PECA);
                    sqlCommand.Parameters.AddWithValue("@p_QT_PECA_ATUAL", estoquePeca.QT_PECA_ATUAL);
                    sqlCommand.Parameters.AddWithValue("@p_QT_PECA_MIN", estoquePeca.QT_PECA_MIN);
                    sqlCommand.Parameters.AddWithValue("@p_DT_ULT_MOVIM", estoquePeca.DT_ULT_MOVIM);

                    if (estoquePeca.estoque.ID_ESTOQUE != 0)
                        sqlCommand.Parameters.AddWithValue("@p_ID_ESTOQUE", estoquePeca.estoque.ID_ESTOQUE);

                    sqlCommand.Parameters.AddWithValue("@p_nidUsuarioAtualizacao", estoquePeca.nidUsuarioAtualizacao);
                    sqlCommand.Parameters.Add(new SqlParameter() { Direction = ParameterDirection.Output, ParameterName = "@p_ID_ESTOQUE_PECA", DbType = DbType.Int64 });

                    sqlCommand.ExecuteNonQuery();
                    estoquePeca.ID_ESTOQUE_PECA = Convert.ToInt64(sqlCommand.Parameters["@p_ID_ESTOQUE_PECA"].Value);
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

        public bool Alterar(EstoquePecaEntity estoquePeca)
        {
            return Alterar(estoquePeca, null);
        }

        public bool Alterar(EstoquePecaEntity estoquePeca, SqlTransaction transaction)
        {
            bool blnOK = false;
            try
            {
                SqlConnection sqlConnection = GetConnection(transaction);


                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    if (transaction != null)
                        sqlCommand.Transaction = transaction;

                    sqlCommand.CommandText = "prcEstoquePecaUpdate";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.AddWithValue("@p_ID_ESTOQUE_PECA", estoquePeca.ID_ESTOQUE_PECA);
                    sqlCommand.Parameters.AddWithValue("@p_CD_PECA", estoquePeca.peca.CD_PECA);
                    sqlCommand.Parameters.AddWithValue("@p_QT_PECA_ATUAL", estoquePeca.QT_PECA_ATUAL);
                    sqlCommand.Parameters.AddWithValue("@p_QT_PECA_MIN", estoquePeca.QT_PECA_MIN);
                    sqlCommand.Parameters.AddWithValue("@p_DT_ULT_MOVIM", estoquePeca.DT_ULT_MOVIM);

                    if (estoquePeca.estoque.ID_ESTOQUE != 0)
                        sqlCommand.Parameters.AddWithValue("@p_ID_ESTOQUE", estoquePeca.estoque.ID_ESTOQUE);

                    sqlCommand.Parameters.AddWithValue("@p_nidUsuarioAtualizacao", estoquePeca.nidUsuarioAtualizacao);

                    sqlCommand.ExecuteNonQuery();
                }
                CloseConnection(transaction, sqlConnection);

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

        /// <summary>
        /// Obtem lista de pecas de um estoque de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<EstoquePecaSinc> ObterListaEstoquePecaSinc(Int64 idUsuario)
        {
            try
            {
                IList<EstoquePecaSinc> listaEstoquePeca = new List<EstoquePecaSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = ObterSqlEstoquePecaSinc();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;
                        cmd.Connection = cnx;
                        cnx.Open();

                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            EstoquePecaSinc estoquePeca = new EstoquePecaSinc
                            {
                                ID_ESTOQUE_PECA = Convert.ToInt32(SDR["ID_ESTOQUE_PECA"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QT_PECA_ATUAL = Convert.ToInt64(SDR["QT_PECA_ATUAL"] is DBNull ? 0 : SDR["QT_PECA_ATUAL"]),
                                QT_PECA_DISPONIVEL = Convert.ToInt64(SDR["QT_PECA_ATUAL"] is DBNull ? 0 : SDR["QT_PECA_ATUAL"]),
                                QT_PECA_MIN = Convert.ToInt64(SDR["QT_PECA_MIN"] is DBNull ? 0 : SDR["QT_PECA_MIN"]),
                                QTD_RECEBIDA_NAO_APROVADA = Convert.ToInt64(SDR["QTD_REC_NAO_APROV"] is DBNull ? 0 : SDR["QTD_REC_NAO_APROV"]),
                                DT_ULT_MOVIM = Convert.ToDateTime(SDR["DT_ULT_MOVIM"] is DBNull ? null : SDR["DT_ULT_MOVIM"]),
                                ID_ESTOQUE = Convert.ToInt64(SDR["ID_ESTOQUE"] is DBNull ? 0 : SDR["ID_ESTOQUE"]),
                                CD_CLIENTE = Convert.ToInt64(SDR["CD_CLIENTE"] is DBNull ? 0 : SDR["CD_CLIENTE"])
                            };

                            listaEstoquePeca.Add(estoquePeca);
                        }
                        cnx.Close();
                        return listaEstoquePeca;
                    }
                }
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

        public IList<EstoquePecaSinc> ObterListaEstoquePecaSincPorID(Int64 idEstoque)
        {
            try
            {
                List<EstoquePecaSinc> listaEstoquePeca = new List<EstoquePecaSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        //cmd.CommandText = ObterSqlEstoquePecaSincId();
                        //cmd.CommandText = ObterSqlEstoquePecaSinc();
                        cmd.CommandText = "prcPecaEstoquePecaSelectPorID";
                        cmd.CommandType = CommandType.StoredProcedure;

                        
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@p_ID_ESTOQUE", idEstoque);
                        //cmd.Parameters.Add("@ID_ESTOQUE", SqlDbType.BigInt).Value = idEstoque;
                        cmd.Connection = cnx;
                        cnx.Open();

                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            EstoquePecaSinc estoquePeca = new EstoquePecaSinc
                            {
                                ID_ESTOQUE_PECA = Convert.ToInt32(SDR["ID_ESTOQUE_PECA"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QT_PECA_ATUAL = Convert.ToInt64(SDR["QT_PECA_ATUAL"] is DBNull ? 0 : SDR["QT_PECA_ATUAL"]),
                                QT_PECA_MIN = Convert.ToInt64(SDR["QT_PECA_MIN"] is DBNull ? 0 : SDR["QT_PECA_MIN"]),
                                DT_ULT_MOVIM = Convert.ToDateTime(SDR["DT_ULT_MOVIM"] is DBNull ? null : SDR["DT_ULT_MOVIM"]),
                                ID_ESTOQUE = Convert.ToInt64(SDR["ID_ESTOQUE"] is DBNull ? 0 : SDR["ID_ESTOQUE"])
                            };
                            listaEstoquePeca.Add(estoquePeca);
                        }
                        cnx.Close();
                        return listaEstoquePeca;
                    }
                }
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

        private string ObterSqlEstoquePecaSinc()
        {
            return $@" 
                             select ep.*, 
                                    null AS CD_CLIENTE,
                                    {ObterSqlQuantidadeAprovadaNaoRecebidaPecaEstoqueTecnico()}
                               from tbestoquepeca ep                       
                              inner join tbestoque e ON e.ID_ESTOQUE = ep.ID_ESTOQUE   
                              inner join tb_tecnico t ON e.cd_tecnico = t.cd_tecnico  
                              WHERE (t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) 
                             UNION  
                             select ep.*,
                                    E.CD_CLIENTE,
                                    {ObterSqlQuantidadeAprovadaNaoRecebidaPecaEstoqueTecnico()}
                               from tbestoquepeca ep                       
                              inner join tbestoque e ON e.ID_ESTOQUE = ep.ID_ESTOQUE                                            
                              WHERE e.TP_ESTOQUE_TEC_3M = 'CLI'
                                AND e.CD_CLIENTE IN                                         
                                        (select CD_CLIENTE from [dbo].[TB_TECNICO_CLIENTE]
                                          where CD_TECNICO IN 
                                                (
                                                  SELECT TOP 1 CD_TECNICO 
                                                    FROM TB_TECNICO 
                                                   WHERE ID_USUARIO =  @ID_USUARIO
                                                )
                                        )";
        }

        private string ObterSqlEstoquePecaSincId()
        {
            return $@" 
                             select ep.* 
                                    from tbestoquepeca ep                       
                              WHERE (ep.ID_ESTOQUE = @ID_ESTOQUE) 
                             ";
        }

        private string ObterSqlQuantidadeAprovadaNaoRecebidaPecaEstoqueTecnico()
        {
            return $@"(select COALESCE(sum(COALESCE(ped_item.QTD_APROVADA, 0)) - sum(COALESCE(ped_item.QTD_RECEBIDA, 0)), 0)
 		                 from TB_PEDIDO pedido
		                inner join TB_PEDIDO_PECA ped_item
     		               on pedido.ID_PEDIDO = ped_item.ID_PEDIDO
		                where pedido.CD_TECNICO = e.CD_TECNICO
			              and pedido.ID_STATUS_PEDIDO in(3, 5, 6) --Aprovado / Pendencia / Recebido com Pendência
			              and ped_item.CD_PECA = ep.CD_PECA
			              and ped_item.ST_STATUS_ITEM in(3, 5, 7) --Aprovado
		              ) AS QTD_REC_NAO_APROV";
        }

    }
}
