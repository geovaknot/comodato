using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class AtivoFixoData
    {
        readonly Database _db;
        DbCommand dbCommand;
        public AtivoFixoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref AtivoFixoEntity AtivoFixo)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAtivoFixoInsert");

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoFixo.nidUsuarioAtualizacao);

                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, AtivoFixo.CD_ATIVO_FIXO.ToUpper());
                _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, AtivoFixo.modelo.CD_MODELO);
                _db.AddInParameter(dbCommand, "@p_DT_INCLUSAO", DbType.DateTime, AtivoFixo.DT_INCLUSAO);
                _db.AddInParameter(dbCommand, "@p_TX_ANO_MAQUINA", DbType.String, AtivoFixo.TX_ANO_MAQUINA);
                _db.AddInParameter(dbCommand, "@p_DT_INVENTARIO", DbType.DateTime, AtivoFixo.DT_INVENTARIO);
                _db.AddInParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.Int64, AtivoFixo.statusAtivo.CD_STATUS_ATIVO);
                _db.AddInParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.Int64, AtivoFixo.situacaoAtivo.CD_SITUACAO_ATIVO);
                _db.AddInParameter(dbCommand, "@p_TX_TIPO", DbType.String, AtivoFixo.TX_TIPO);
                _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, AtivoFixo.linhaProduto.CD_LINHA_PRODUTO);
                _db.AddInParameter(dbCommand, "@p_DT_FIM_GARANTIA", DbType.DateTime, AtivoFixo.DT_FIM_GARANTIA);
                _db.AddInParameter(dbCommand, "@p_DT_MANUTENCAO", DbType.DateTime, AtivoFixo.DT_MANUTENCAO);
                _db.AddInParameter(dbCommand, "@p_DT_FIM_MANUTENCAO", DbType.DateTime, AtivoFixo.DT_FIM_MANUTENCAO);
                _db.AddInParameter(dbCommand, "@p_DS_MOTIVO", DbType.String, AtivoFixo.DS_MOTIVO);


                //_db.AddInParameter(dbCommand, "@p_FL_STATUS", DbType.Boolean, AtivoFixo.FL_STATUS);

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

        public void Excluir(AtivoFixoEntity AtivoFixo)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAtivoFixoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, AtivoFixo.CD_ATIVO_FIXO);

                if (AtivoFixo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoFixo.nidUsuarioAtualizacao);

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

        public bool Alterar(AtivoFixoEntity AtivoFixo)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivoFixoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, AtivoFixo.CD_ATIVO_FIXO);

                if (!string.IsNullOrEmpty(AtivoFixo.modelo.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, AtivoFixo.modelo.CD_MODELO);

                if (AtivoFixo.DT_INCLUSAO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INCLUSAO", DbType.DateTime, AtivoFixo.DT_INCLUSAO);

                if (!string.IsNullOrEmpty(AtivoFixo.TX_ANO_MAQUINA))
                    _db.AddInParameter(dbCommand, "@p_TX_ANO_MAQUINA", DbType.String, AtivoFixo.TX_ANO_MAQUINA);

                if (AtivoFixo.DT_INVENTARIO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INVENTARIO", DbType.DateTime, AtivoFixo.DT_INVENTARIO);

                if (AtivoFixo.statusAtivo.CD_STATUS_ATIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.Int64, AtivoFixo.statusAtivo.CD_STATUS_ATIVO);

                if (AtivoFixo.situacaoAtivo.CD_SITUACAO_ATIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.Int64, AtivoFixo.situacaoAtivo.CD_SITUACAO_ATIVO);

                if (!string.IsNullOrEmpty(AtivoFixo.TX_TIPO))
                    _db.AddInParameter(dbCommand, "@p_TX_TIPO", DbType.String, AtivoFixo.TX_TIPO);

                if (AtivoFixo.linhaProduto.CD_LINHA_PRODUTO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, AtivoFixo.linhaProduto.CD_LINHA_PRODUTO);

                _db.AddInParameter(dbCommand, "@p_FL_STATUS", DbType.Boolean, AtivoFixo.FL_STATUS);

                if (AtivoFixo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoFixo.nidUsuarioAtualizacao);


                if (AtivoFixo.DT_FIM_GARANTIA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FIM_GARANTIA", DbType.DateTime, AtivoFixo.DT_FIM_GARANTIA);
                if (AtivoFixo.DT_MANUTENCAO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_MANUTENCAO", DbType.DateTime, AtivoFixo.DT_MANUTENCAO);
                if (AtivoFixo.DT_FIM_MANUTENCAO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FIM_MANUTENCAO", DbType.DateTime, AtivoFixo.DT_FIM_MANUTENCAO);

                if (!string.IsNullOrEmpty(AtivoFixo.DS_MOTIVO))
                    _db.AddInParameter(dbCommand, "@p_DS_MOTIVO", DbType.String, AtivoFixo.DS_MOTIVO);

                //if (AtivoFixo.D != nul)
                //    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoFixo.nidUsuarioAtualizacao);


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

        public DataTable ObterLista(AtivoFixoEntity AtivoFixo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivoFixoSelect");

                if (!string.IsNullOrEmpty(AtivoFixo.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, AtivoFixo.CD_ATIVO_FIXO);

                if (!string.IsNullOrEmpty(AtivoFixo.modelo.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, AtivoFixo.modelo.CD_MODELO);

                if (AtivoFixo.DT_INCLUSAO > DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_DT_INCLUSAO", DbType.DateTime, AtivoFixo.DT_INCLUSAO);

                if (!string.IsNullOrEmpty(AtivoFixo.TX_ANO_MAQUINA))
                    _db.AddInParameter(dbCommand, "@p_TX_ANO_MAQUINA", DbType.String, AtivoFixo.TX_ANO_MAQUINA);

                if (AtivoFixo.DT_INVENTARIO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INVENTARIO", DbType.DateTime, AtivoFixo.DT_INVENTARIO);

                if (AtivoFixo.statusAtivo.CD_STATUS_ATIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_STATUS_ATIVO", DbType.Int64, AtivoFixo.statusAtivo.CD_STATUS_ATIVO);

                if (AtivoFixo.situacaoAtivo.CD_SITUACAO_ATIVO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.Int64, AtivoFixo.situacaoAtivo.CD_SITUACAO_ATIVO);

                if (!string.IsNullOrEmpty(AtivoFixo.TX_TIPO))
                    _db.AddInParameter(dbCommand, "@p_TX_TIPO", DbType.String, AtivoFixo.TX_TIPO);

                if (AtivoFixo.linhaProduto.CD_LINHA_PRODUTO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, AtivoFixo.linhaProduto.CD_LINHA_PRODUTO);

                if (AtivoFixo.FL_STATUS != null)
                    _db.AddInParameter(dbCommand, "@p_FL_STATUS", DbType.Int64, AtivoFixo.FL_STATUS);

                if (AtivoFixo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoFixo.nidUsuarioAtualizacao);

                if (AtivoFixo.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, AtivoFixo.cliente.CD_CLIENTE);


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

        public IList<AtivoFixoSinc> ObterListaAtivoFixoSinc(Int64 idUsuario)
        {
            try
            {

                IList<AtivoFixoSinc> listaAtivoFixo = new List<AtivoFixoSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = 
                                           " select a.*, "+
                                           " (select top 1 DT_DATA_OS from tbOSPadrao where CD_ATIVO_FIXO = a.CD_ATIVO_FIXO order by DT_DATA_OS desc) as Data_ultimaOS, " +
                                           " (select top 1 DT_DATA_OS from tbOSPadrao where CD_ATIVO_FIXO = a.CD_ATIVO_FIXO order by DT_DATA_OS desc) as last_order " +
                                           " from tb_ativo_fixo a " +
                                           " INNER JOIN TB_ATIVO_CLIENTE c ON a.CD_ATIVO_FIXO = c.CD_ATIVO_FIXO " +
                                           " and   DT_DEVOLUCAO is null " +
                                           " INNER JOIN TB_TECNICO_CLIENTE tc ON tc.CD_CLIENTE = c.CD_CLIENTE  "+
                                           " WHERE tc.CD_TECNICO IN " +
                                           " (select CD_TECNICO from TB_TECNICO t " +
                                           " INNER JOIN tbUsuario u ON u.nidUsuario = t.ID_USUARIO AND t.ID_USUARIO = @ID_USUARIO) "+
                                           " order by Data_ultimaOS asc ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            AtivoFixoSinc ativoFixo = new AtivoFixoSinc();
                            ativoFixo.CD_ATIVO_FIXO = Convert.ToString(SDR["CD_ATIVO_FIXO"].ToString());
                            ativoFixo.CD_ATIVO_FIXO = ativoFixo.CD_ATIVO_FIXO.ToUpper();
                            ativoFixo.CD_MODELO = Convert.ToString(SDR["CD_MODELO"].ToString());
                            ativoFixo.DT_INCLUSAO = Convert.ToDateTime(SDR["DT_INCLUSÃO"] is DBNull ? "01/01/2000" : SDR["DT_INCLUSÃO"].ToString());
                            ativoFixo.DT_FIM_GARANTIA = Convert.ToDateTime(SDR["DT_FIM_GARANTIA"] is DBNull ? "01/01/2000" : SDR["DT_FIM_GARANTIA"].ToString());
                            ativoFixo.DT_MANUTENCAO = Convert.ToDateTime(SDR["DT_MANUTENCAO"] is DBNull ? "01/01/2000" : SDR["DT_MANUTENCAO"].ToString());
                            ativoFixo.DT_FIM_MANUTENCAO = Convert.ToDateTime(SDR["DT_FIM_MANUTENCAO"] is DBNull ? "01/01/2000" : SDR["DT_FIM_MANUTENCAO"].ToString());
                            ativoFixo.TX_ANO_MAQUINA = Convert.ToString(SDR["TX_ANO_MÁQUINA"] is DBNull ? "" : SDR["TX_ANO_MÁQUINA"].ToString());
                            ativoFixo.DT_INVENTARIO = Convert.ToDateTime(SDR["DT_INVENTARIO"] is DBNull ? "01/01/2000" : SDR["DT_INVENTARIO"].ToString());
                            ativoFixo.CD_STATUS_ATIVO = Convert.ToInt32(SDR["CD_STATUS_ATIVO"] is DBNull ? 0 : SDR["CD_STATUS_ATIVO"]);
                            ativoFixo.CD_SITUACAO_ATIVO = Convert.ToInt32(SDR["CD_SITUACAO_ATIVO"] is DBNull ? 0 : SDR["CD_SITUACAO_ATIVO"]);
                            ativoFixo.TX_TIPO = Convert.ToString(SDR["TX_TIPO"] is DBNull ? "" : SDR["TX_TIPO"].ToString());
                            ativoFixo.CD_LINHA_PRODUTO = Convert.ToInt32(SDR["CD_LINHA_PRODUTO"] is DBNull ? 0 : SDR["CD_LINHA_PRODUTO"]);
                            ativoFixo.FL_STATUS = Convert.ToBoolean(SDR["FL_STATUS"] is DBNull ? false : SDR["FL_STATUS"]);
                            ativoFixo.Data_ultimaOS = Convert.ToDateTime(SDR["Data_ultimaOS"] is DBNull ? "01/01/2000" : SDR["Data_ultimaOS"].ToString());
                            ativoFixo.last_order = Convert.ToDateTime(SDR["last_order"] is DBNull ? "01/01/2000" : SDR["last_order"].ToString());
                            ativoFixo.dtmDataHoraAtualizacao = Convert.ToDateTime("01/01/2022");
                            listaAtivoFixo.Add(ativoFixo);
                        }
                        cnx.Close();
                        return listaAtivoFixo;
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
