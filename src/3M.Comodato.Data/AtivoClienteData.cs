using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class AtivoClienteData
    {
        readonly Database _db;
        DbCommand dbCommand;
        internal DbTransaction _transaction;
        public AtivoClienteData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref AtivoClienteEntity AtivoCliente, ref string Mensagem)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivoClienteInsert");

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoCliente.nidUsuarioAtualizacao);

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, AtivoCliente.cliente.CD_CLIENTE);

                if (AtivoCliente.ativoFixo.CD_ATIVO_FIXO == null)
                    AtivoCliente.ativoFixo.CD_ATIVO_FIXO = String.Empty;

                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, AtivoCliente.ativoFixo.CD_ATIVO_FIXO);
                _db.AddInParameter(dbCommand, "@p_DT_NOTAFISCAL", DbType.DateTime, AtivoCliente.DT_NOTAFISCAL);
                _db.AddInParameter(dbCommand, "@p_NR_NOTAFISCAL", DbType.Int64, AtivoCliente.NR_NOTAFISCAL);
                _db.AddInParameter(dbCommand, "@p_DT_DEVOLUCAO", DbType.DateTime, AtivoCliente.DT_DEVOLUCAO);
                _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.String, AtivoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO);
                _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, AtivoCliente.TX_OBS);
                _db.AddInParameter(dbCommand, "@p_CD_RAZAO", DbType.Int32, AtivoCliente.razaoComodato.CD_RAZAO_COMODATO);
                _db.AddInParameter(dbCommand, "@p_CD_TIPO", DbType.Int32, AtivoCliente.tipo.CD_TIPO);
                _db.AddInParameter(dbCommand, "@p_VL_ALUGUEL", DbType.Decimal, AtivoCliente.VL_ALUGUEL);
                _db.AddInParameter(dbCommand, "@p_TX_TERMOPGTO", DbType.String, AtivoCliente.TX_TERMOPGTO);
                _db.AddInParameter(dbCommand, "@p_QTD_MESES_LOCACAO", DbType.Int32, AtivoCliente.QTD_MESES_LOCACAO);
                _db.AddInParameter(dbCommand, "@p_DT_FIM_GARANTIA_REFORMA", DbType.DateTime, AtivoCliente.DT_FIM_GARANTIA_REFORMA);
                

                if (!string.IsNullOrEmpty(AtivoCliente.DS_ARQUIVO_FOTO))
                    _db.AddInParameter(dbCommand, "@p_DS_ARQUIVO_FOTO", DbType.String, AtivoCliente.DS_ARQUIVO_FOTO);

                if (!string.IsNullOrEmpty(AtivoCliente.DS_ARQUIVO_FOTO2))
                    _db.AddInParameter(dbCommand, "@p_DS_ARQUIVO_FOTO2", DbType.String, AtivoCliente.DS_ARQUIVO_FOTO2);

                _db.AddOutParameter(dbCommand, "@p_ID_ATIVO_CLIENTE", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_MENSAGEM", DbType.String, 100);

                _db.ExecuteNonQuery(dbCommand);

                AtivoCliente.ID_ATIVO_CLIENTE = Convert.ToInt64("0" + _db.GetParameterValue(dbCommand, "@p_ID_ATIVO_CLIENTE"));
                Mensagem = Convert.ToString(_db.GetParameterValue(dbCommand, "@p_MENSAGEM"));

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

        public void Excluir(AtivoClienteEntity AtivoCliente)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAtivoClienteDelete");

                _db.AddInParameter(dbCommand, "@p_ID_ATIVO_CLIENTE", DbType.String, AtivoCliente.ID_ATIVO_CLIENTE);

                if (AtivoCliente.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoCliente.nidUsuarioAtualizacao);

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

        public bool Alterar(AtivoClienteEntity AtivoCliente)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivoClienteUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_ATIVO_CLIENTE", DbType.String, AtivoCliente.ID_ATIVO_CLIENTE);

                if (AtivoCliente.cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, AtivoCliente.cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(AtivoCliente.ativoFixo.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, AtivoCliente.ativoFixo.CD_ATIVO_FIXO);

                if (AtivoCliente.DT_NOTAFISCAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_NOTAFISCAL", DbType.DateTime, AtivoCliente.DT_NOTAFISCAL);

                if (AtivoCliente.NR_NOTAFISCAL != 0)
                    _db.AddInParameter(dbCommand, "@p_NR_NOTAFISCAL", DbType.Int64, AtivoCliente.NR_NOTAFISCAL);

                if (AtivoCliente.DT_DEVOLUCAO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DEVOLUCAO", DbType.DateTime, AtivoCliente.DT_DEVOLUCAO);

                if (AtivoCliente.DT_DEVOLUCAO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FIM_GARANTIA_REFORMA", DbType.DateTime, AtivoCliente.DT_FIM_GARANTIA_REFORMA);
                

                if (!string.IsNullOrEmpty(AtivoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO))
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.String, AtivoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO);

                _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, AtivoCliente.TX_OBS);

                if (AtivoCliente.razaoComodato.CD_RAZAO_COMODATO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_RAZAO", DbType.Int32, AtivoCliente.razaoComodato.CD_RAZAO_COMODATO);

                if (AtivoCliente.tipo.CD_TIPO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO", DbType.Int32, AtivoCliente.tipo.CD_TIPO);

                _db.AddInParameter(dbCommand, "@p_VL_ALUGUEL", DbType.Decimal, AtivoCliente.VL_ALUGUEL);

                _db.AddInParameter(dbCommand, "@p_TX_TERMOPGTO", DbType.String, AtivoCliente.TX_TERMOPGTO);

                _db.AddInParameter(dbCommand, "@p_QTD_MESES_LOCACAO", DbType.Int32, AtivoCliente.QTD_MESES_LOCACAO);

                if (!string.IsNullOrEmpty(AtivoCliente.DS_ARQUIVO_FOTO))
                    _db.AddInParameter(dbCommand, "@p_DS_ARQUIVO_FOTO", DbType.String, AtivoCliente.DS_ARQUIVO_FOTO);

                if (!string.IsNullOrEmpty(AtivoCliente.DS_ARQUIVO_FOTO2))
                    _db.AddInParameter(dbCommand, "@p_DS_ARQUIVO_FOTO2", DbType.String, AtivoCliente.DS_ARQUIVO_FOTO2);

                if (AtivoCliente.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, AtivoCliente.nidUsuarioAtualizacao);

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

        public void AtualizarStatusDevolucao(long CD_CLIENTE, string CD_ATIVOS, char ST_DEV, long nidUsuarioAtualizacao)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivoClienteUpdateStatusDevolucao");

                if (CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);

                if (!string.IsNullOrEmpty(CD_ATIVOS))
                    _db.AddInParameter(dbCommand, "@p_listaAtivos", DbType.String, CD_ATIVOS);

                _db.AddInParameter(dbCommand, "@p_ST_DEVOLUCAO", DbType.String, ST_DEV);

                if (nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);

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

        public DataTable ObterListaFaturamento(DadosFaturamentoEntity faturamentoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcObterFaturamentoSelect");

                if (faturamentoEntity.ID > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID", DbType.Int64, faturamentoEntity.ID);
                }

                
                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                
                dataTable = dataSet.Tables[0];

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return dataTable;
        }

        public DataTable InativarFaturamento(DadosFaturamentoEntity faturamentoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcInativarFaturamento");

                if (faturamentoEntity.ID > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID", DbType.Int64, faturamentoEntity.ID);
                }
                if(faturamentoEntity.Ativo == false)
                {
                    _db.AddInParameter(dbCommand, "@p_Ativo", DbType.Boolean, faturamentoEntity.Ativo);
                }


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return dataTable;
        }

        public IList<AtivoClienteSinc> ObterListaAtivoClienteSinc(Int64 idUsuario)
        {
            try
            {
                IList<AtivoClienteSinc> listaAtivoCliente = new List<AtivoClienteSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                          @"select TB_ATIVO_CLIENTE.*, 
                                                   tb_tipo.ds_tipo,
	                                               (select coalesce(Max(osp.DT_DATA_OS), null)
	                                                  from tbOSPadrao osp
    	                                             where osp.CD_ATIVO_FIXO = tb_ativo_cliente.CD_ATIVO_FIXO
		                                               and osp.CD_CLIENTE = tb_ativo_cliente.CD_CLIENTE
	                                               ) AS DT_ULTIMA_MANUTENCAO
                                              from TB_ATIVO_CLIENTE 
                                              left join tb_tipo 
                                                on tb_ativo_cliente.cd_tipo = tb_tipo.cd_tipo 
                                             where CD_ATIVO_FIXO IN( 
                                                                    select a.CD_ATIVO_FIXO  
                                                                      from tb_ativo_fixo a 
                                                                     INNER JOIN TB_ATIVO_CLIENTE c ON a.CD_ATIVO_FIXO = c.CD_ATIVO_FIXO 
                                                                     INNER JOIN TB_TECNICO_CLIENTE tc ON tc.CD_CLIENTE = c.CD_CLIENTE
                                                                     WHERE tc.CD_TECNICO IN  
                                                                           (select CD_TECNICO from TB_TECNICO t 
                                                                             INNER JOIN tbUsuario u 
								                                                ON u.nidUsuario = t.ID_USUARIO AND t.ID_USUARIO = @ID_USUARIO) 
                                                                           )   
                                               and ( DT_DEVOLUCAO is null AND CD_MOTIVO_DEVOLUCAO is null)";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;


                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            AtivoClienteSinc ativoCliente = new AtivoClienteSinc();
                            ativoCliente.ID_ATIVO_CLIENTE = Convert.ToInt32(SDR["ID_ATIVO_CLIENTE"].ToString());
                            ativoCliente.CD_CLIENTE = Convert.ToInt32(SDR["CD_CLIENTE"].ToString());
                            ativoCliente.CD_ATIVO_FIXO = Convert.ToString( SDR["CD_ATIVO_FIXO"] is DBNull ? "" : SDR["CD_ATIVO_FIXO"].ToString()) ;
                            ativoCliente.CD_ATIVO_FIXO = ativoCliente.CD_ATIVO_FIXO.ToUpper();
                            ativoCliente.DT_NOTAFISCAL = Convert.ToDateTime( SDR["DT_NOTAFISCAL"] is DBNull ? "01/01/2000": SDR["DT_NOTAFISCAL"].ToString());
                            ativoCliente.NR_NOTAFISCAL = Convert.ToInt64(SDR["NR_NOTAFISCAL"] is DBNull ? 0 : SDR["NR_NOTAFISCAL"]);
                            ativoCliente.DT_DEVOLUCAO = Convert.ToDateTime(SDR["DT_DEVOLUCAO"] is DBNull ? null : SDR["DT_DEVOLUCAO"].ToString());
                            ativoCliente.CD_MOTIVO_DEVOLUCAO = SDR["CD_MOTIVO_DEVOLUCAO"] is DBNull ? null : SDR["CD_MOTIVO_DEVOLUCAO"].ToString();
                            ativoCliente.TX_OBS = ""; // SDR["TX_OBS"].ToString();
                            ativoCliente.CD_RAZAO = Convert.ToInt32(SDR["CD_RAZAO"] is DBNull? 0 : SDR["CD_RAZAO"]);
                            ativoCliente.CD_TIPO = Convert.ToInt32(SDR["CD_TIPO"] is DBNull ? "0" : SDR["CD_TIPO"].ToString());
                            ativoCliente.DS_TIPO = Convert.ToString(SDR["DS_TIPO"] is DBNull ? "" : SDR["DS_TIPO"].ToString());
                            ativoCliente.VL_ALUGUEL = 0;  //Convert.ToDecimal(SDR["VL_ALUGUEL"] is DBNull ? 0 : SDR["VL_ALUGUEL"]);
                            ativoCliente.TX_TERMOPGTO = ""; // Convert.ToString( SDR["TX_TERMOPGTO"] is DBNull ? "" : SDR["TX_TERMOPGTO"]);

                            if (string.IsNullOrWhiteSpace(SDR["DT_ULTIMA_MANUTENCAO"].ToString()))
                                ativoCliente.DT_ULTIMA_MANUTENCAO = null;
                            else
                                ativoCliente.DT_ULTIMA_MANUTENCAO = Convert.ToDateTime(SDR["DT_ULTIMA_MANUTENCAO"].ToString());

                            listaAtivoCliente.Add(ativoCliente);
                        }
                        cnx.Close();
                        return listaAtivoCliente;
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
        
        public DataTable ObterLista(AtivoClienteEntity AtivoCliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivoClienteSelect");

                if (AtivoCliente.ID_ATIVO_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_ATIVO_CLIENTE", DbType.String, AtivoCliente.ID_ATIVO_CLIENTE);

                if (AtivoCliente.cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, AtivoCliente.cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(AtivoCliente.ativoFixo.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, AtivoCliente.ativoFixo.CD_ATIVO_FIXO);

                if (AtivoCliente.DT_NOTAFISCAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_NOTAFISCAL", DbType.DateTime, AtivoCliente.DT_NOTAFISCAL);

                if (AtivoCliente.NR_NOTAFISCAL != 0)
                    _db.AddInParameter(dbCommand, "@p_NR_NOTAFISCAL", DbType.Int64, AtivoCliente.NR_NOTAFISCAL);

                if (AtivoCliente.DT_DEVOLUCAO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DEVOLUCAO", DbType.DateTime, AtivoCliente.DT_DEVOLUCAO);

                if (!string.IsNullOrEmpty(AtivoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO))
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.String, AtivoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO);

                if (!string.IsNullOrEmpty(AtivoCliente.TX_OBS))
                    _db.AddInParameter(dbCommand, "@p_TX_OBS", DbType.String, AtivoCliente.TX_OBS);

                if (AtivoCliente.razaoComodato.CD_RAZAO_COMODATO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_RAZAO", DbType.Int32, AtivoCliente.razaoComodato.CD_RAZAO_COMODATO);

                if (AtivoCliente.tipo.CD_TIPO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO", DbType.Int32, AtivoCliente.tipo.CD_TIPO);

                if (AtivoCliente.VL_ALUGUEL != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALUGUEL", DbType.Decimal, AtivoCliente.VL_ALUGUEL);

                if (!string.IsNullOrEmpty(AtivoCliente.TX_TERMOPGTO))
                    _db.AddInParameter(dbCommand, "@p_TX_TERMOPGTO", DbType.String, AtivoCliente.TX_TERMOPGTO);

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

        public DataTable ObterListaEquipamentoAlocado(AtivoClienteEntity AtivoCliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivoClienteSelectEquipamentoAlocado");

                if (AtivoCliente.cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, AtivoCliente.cliente.CD_CLIENTE);

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

        public DataTable ObterListaComboAtivosRecolhidos(AtivoClienteEntity AtivoCliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtivosRecolhidosSelect");

                if (AtivoCliente.cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, AtivoCliente.cliente.CD_CLIENTE);

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

        public DataTable ObterRelatorioAtivosClientesDI()
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptAtivosClientesDISelect");

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

        public bool ExcluirArquivoFoto(string idAtivo, string arquivo)
        {
            bool blnOK = false;

            using (SqlConnection cnx = new SqlConnection(_db.CreateConnection().ConnectionString))
            {
                cnx.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = " UPDATE [dbo].[TB_ATIVO_CLIENTE] SET " + arquivo.ToString() + " = '' WHERE ID_ATIVO_CLIENTE = @idAtivo";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@idAtivo", System.Data.SqlDbType.BigInt).Value = Convert.ToUInt32(idAtivo);

                        cmd.Connection = cnx;
                        cmd.ExecuteNonQuery();
                        cnx.Close();
                        blnOK = true;
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
                finally
                {
                    cnx.Close();
                    cnx.Dispose();
                }
                return blnOK;
            }
        }
    }
}
