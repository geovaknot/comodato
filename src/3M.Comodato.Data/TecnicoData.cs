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
    public class TecnicoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TecnicoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref TecnicoEntity tecnicoEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTecnicoInsert");

                if (!string.IsNullOrEmpty(tecnicoEntity.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, tecnicoEntity.CD_TECNICO);

                if (!string.IsNullOrEmpty(tecnicoEntity.NM_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_NM_TECNICO", DbType.String, tecnicoEntity.NM_TECNICO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_ENDERECO))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, tecnicoEntity.EN_ENDERECO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_BAIRRO))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, tecnicoEntity.EN_BAIRRO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_CIDADE))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, tecnicoEntity.EN_CIDADE);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_ESTADO))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, tecnicoEntity.EN_ESTADO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_CEP))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, tecnicoEntity.EN_CEP);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_TELEFONE))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, tecnicoEntity.TX_TELEFONE);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_FAX))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, tecnicoEntity.TX_FAX);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_EMAIL))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, tecnicoEntity.TX_EMAIL);

                if (!string.IsNullOrEmpty(tecnicoEntity.TP_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TP_TECNICO", DbType.String, tecnicoEntity.TP_TECNICO);

                if (tecnicoEntity.VL_CUSTO_HORA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_CUSTO_HORA", DbType.Decimal, tecnicoEntity.VL_CUSTO_HORA);

                if (!string.IsNullOrEmpty(tecnicoEntity.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, tecnicoEntity.FL_ATIVO);

                if (tecnicoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoEntity.nidUsuarioAtualizacao);

                if (tecnicoEntity.usuarioCoordenador.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_COORDENADOR", DbType.Int64, tecnicoEntity.usuarioCoordenador.nidUsuario);

                if (tecnicoEntity.usuarioSupervisorTecnico.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_SUPERVISOR", DbType.Int64, tecnicoEntity.usuarioSupervisorTecnico.nidUsuario);

                if (tecnicoEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, tecnicoEntity.usuario.nidUsuario);

                if (tecnicoEntity.empresa.CD_Empresa > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_EMPRESA", DbType.Int64, tecnicoEntity.empresa.CD_Empresa);

                if (!string.IsNullOrEmpty(tecnicoEntity.FL_FERIAS))
                    _db.AddInParameter(dbCommand, "@p_FL_FERIAS", DbType.String, tecnicoEntity.FL_FERIAS);

                if (!string.IsNullOrEmpty(tecnicoEntity.NM_REDUZIDO))
                    _db.AddInParameter(dbCommand, "@p_NM_REDUZIDO", DbType.String, tecnicoEntity.NM_REDUZIDO);

                if (!string.IsNullOrEmpty(tecnicoEntity.CD_BCPS))
                    _db.AddInParameter(dbCommand, "@p_CD_BCPS", DbType.String, tecnicoEntity.CD_BCPS);

                // _db.AddOutParameter(dbCommand, "@p_CD_TECNICO", DbType.String, 6);

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

        public void Excluir(TecnicoEntity tecnicoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTecnicoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, tecnicoEntity.CD_TECNICO);

                if (tecnicoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(TecnicoEntity tecnicoEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, tecnicoEntity.CD_TECNICO);

                if (!string.IsNullOrEmpty(tecnicoEntity.NM_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_NM_TECNICO", DbType.String, tecnicoEntity.NM_TECNICO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_ENDERECO))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, tecnicoEntity.EN_ENDERECO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_BAIRRO))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, tecnicoEntity.EN_BAIRRO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_CIDADE))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, tecnicoEntity.EN_CIDADE);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_ESTADO))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, tecnicoEntity.EN_ESTADO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_CEP))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, tecnicoEntity.EN_CEP);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_TELEFONE))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, tecnicoEntity.TX_TELEFONE);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_FAX))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, tecnicoEntity.TX_FAX);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_EMAIL))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, tecnicoEntity.TX_EMAIL);

                if (!string.IsNullOrEmpty(tecnicoEntity.TP_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TP_TECNICO", DbType.String, tecnicoEntity.TP_TECNICO);

                if (tecnicoEntity.VL_CUSTO_HORA != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_CUSTO_HORA", DbType.Decimal, tecnicoEntity.VL_CUSTO_HORA);

                if (!string.IsNullOrEmpty(tecnicoEntity.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, tecnicoEntity.FL_ATIVO);

                if (tecnicoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoEntity.nidUsuarioAtualizacao);

                if (tecnicoEntity.usuarioCoordenador.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_COORDENADOR", DbType.Int64, tecnicoEntity.usuarioCoordenador.nidUsuario);

                if (tecnicoEntity.usuarioSupervisorTecnico.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_SUPERVISOR", DbType.Int64, tecnicoEntity.usuarioSupervisorTecnico.nidUsuario);

                if (tecnicoEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, tecnicoEntity.usuario.nidUsuario);

                if (tecnicoEntity.empresa.CD_Empresa > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_EMPRESA", DbType.Int64, tecnicoEntity.empresa.CD_Empresa);

                if (!string.IsNullOrEmpty(tecnicoEntity.FL_FERIAS))
                    _db.AddInParameter(dbCommand, "@p_FL_FERIAS", DbType.String, tecnicoEntity.FL_FERIAS);

                if (!string.IsNullOrEmpty(tecnicoEntity.NM_REDUZIDO))
                    _db.AddInParameter(dbCommand, "@p_NM_REDUZIDO", DbType.String, tecnicoEntity.NM_REDUZIDO);

                if (!string.IsNullOrEmpty(tecnicoEntity.CD_BCPS))
                    _db.AddInParameter(dbCommand, "@p_CD_BCPS", DbType.String, tecnicoEntity.CD_BCPS);

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

        public DataTable ObterLista(TecnicoEntity tecnicoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoSelect");

                if (!string.IsNullOrEmpty(tecnicoEntity.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, tecnicoEntity.CD_TECNICO);

                if (!string.IsNullOrEmpty(tecnicoEntity.NM_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_NM_TECNICO", DbType.String, tecnicoEntity.NM_TECNICO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_ENDERECO))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, tecnicoEntity.EN_ENDERECO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_BAIRRO))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, tecnicoEntity.EN_BAIRRO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_CIDADE))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, tecnicoEntity.EN_CIDADE);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_ESTADO))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, tecnicoEntity.EN_ESTADO);

                if (!string.IsNullOrEmpty(tecnicoEntity.EN_CEP))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, tecnicoEntity.EN_CEP);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_TELEFONE))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, tecnicoEntity.TX_TELEFONE);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_FAX))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, tecnicoEntity.TX_FAX);

                if (!string.IsNullOrEmpty(tecnicoEntity.TX_EMAIL))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, tecnicoEntity.TX_EMAIL);

                if (!string.IsNullOrEmpty(tecnicoEntity.TP_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_TP_TECNICO", DbType.String, tecnicoEntity.TP_TECNICO);

                if (tecnicoEntity.VL_CUSTO_HORA != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_CUSTO_HORA", DbType.Decimal, tecnicoEntity.VL_CUSTO_HORA);

                if (!string.IsNullOrEmpty(tecnicoEntity.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, tecnicoEntity.FL_ATIVO);

                if (tecnicoEntity.usuarioCoordenador.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_COORDENADOR", DbType.Int64, tecnicoEntity.usuarioCoordenador.nidUsuario);

                if (tecnicoEntity.usuarioSupervisorTecnico.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_SUPERVISOR", DbType.Int64, tecnicoEntity.usuarioSupervisorTecnico.nidUsuario);

                if (tecnicoEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, tecnicoEntity.usuario.nidUsuario);

                if (tecnicoEntity.empresa.CD_Empresa > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_EMPRESA", DbType.Int64, tecnicoEntity.empresa.CD_Empresa);

                if (!string.IsNullOrEmpty(tecnicoEntity.FL_FERIAS))
                    _db.AddInParameter(dbCommand, "@p_FL_FERIAS", DbType.String, tecnicoEntity.FL_FERIAS);

                if (!string.IsNullOrEmpty(tecnicoEntity.NM_REDUZIDO))
                    _db.AddInParameter(dbCommand, "@p_NM_REDUZIDO", DbType.String, tecnicoEntity.NM_REDUZIDO);

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

        public List<TecnicoEntity> ObterTecnico(string cdTecnico)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            List<TecnicoEntity> Lista = new List<TecnicoEntity>();


            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoSelect");

                if (!string.IsNullOrEmpty(cdTecnico))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, cdTecnico);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

                if (dataTable.Rows.Count > 0)
                {

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        var dto = new TecnicoEntity();
                        dto.CD_TECNICO = dr["CD_TECNICO"].ToString();
                        dto.VL_CUSTO_HORA = Convert.ToDecimal(dr["VL_CUSTO_HORA"]);
                        dto.usuario.nidUsuario = Convert.ToInt64(dr["ID_USUARIO"]);
                        dto.TX_TELEFONE = dr["TX_TELEFONE"].ToString();
                        Lista.Add(dto);
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
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return Lista;
        }

        public List<TecnicoSinc> ObterTecnicoOS(string cdTecnico)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            List<TecnicoSinc> Lista = new List<TecnicoSinc>();


            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoSelect");

                if (!string.IsNullOrEmpty(cdTecnico))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, cdTecnico);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

                if (dataTable.Rows.Count > 0)
                {

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        var dto = new TecnicoSinc();
                        dto.CD_TECNICO = dr["CD_TECNICO"].ToString();
                        dto.NM_TECNICO = dr["NM_TECNICO"].ToString();
                        dto.ID_USUARIO = Convert.ToInt64(dr["ID_USUARIO"]);
                        Lista.Add(dto);
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
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return Lista;
        }


        public DataTable ObterListaEscala(int CD_CLIENTE)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoSelectEscala");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, CD_CLIENTE);

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

        public bool TransferirCarteira(string CD_TECNICO_ORIGEM, string CD_TECNICO_DESTINO, Int64? nidUsuarioAtualizacao = null)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoTransferirCarteira");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO_ORIGEM", DbType.String, CD_TECNICO_ORIGEM);

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO_DESTINO", DbType.String, CD_TECNICO_DESTINO);

                if (nidUsuarioAtualizacao != null)
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
            return blnOK;

        }

        /// <summary>
        /// Ajustar dados quando um técnico entra de férias ou é desligado 
        /// </summary>
        /// <param name="CD_TECNICO"></param>
        /// <param name="nidUsuarioAtualizacao"></param>
        /// <returns></returns> 
        public bool AjustarAgendaDeFerias(string CD_TECNICO, Int64? nidUsuarioAtualizacao = null)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAjusteDeFerias");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                if (nidUsuarioAtualizacao != null)
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
            return blnOK;
        }


        /// <summary>
        /// Obtem lista de Tecnicos Ativos para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<TecnicoSinc> ObterListaTecnicoSinc()
        {
            try
            {
                IList<TecnicoSinc> listaTecnico = new List<TecnicoSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        //cmd.CommandText =
                        //                 " select t.* from tb_tecnico t where fl_ativo = 'S' ";
                        cmd.CommandText =
                                " select t.*, e.ID_ESTOQUE from tb_tecnico t left join tbEstoque e on e.cd_tecnico = t.cd_tecnico where t.fl_ativo = 'S' ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        //cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            TecnicoSinc tecnico = new TecnicoSinc();
                            tecnico.CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"].ToString());
                            tecnico.NM_TECNICO= Convert.ToString(SDR["NM_TECNICO"] is DBNull ? "" : SDR["NM_TECNICO"].ToString());
                            tecnico.EN_ENDERECO = Convert.ToString(SDR["EN_ENDERECO"] is DBNull ? "" : SDR["EN_ENDERECO"].ToString());
                            tecnico.EN_BAIRRO = Convert.ToString(SDR["EN_BAIRRO"] is DBNull ? "" : SDR["EN_BAIRRO"].ToString());
                            tecnico.EN_CIDADE = Convert.ToString(SDR["EN_CIDADE"] is DBNull ? "" : SDR["EN_CIDADE"].ToString());
                            tecnico.EN_ESTADO = Convert.ToString(SDR["EN_ESTADO"] is DBNull ? "" : SDR["EN_ESTADO"].ToString());
                            tecnico.EN_CEP = Convert.ToString(SDR["EN_CEP"] is DBNull ? "" : SDR["EN_CEP"].ToString());
                            tecnico.TX_TELEFONE = Convert.ToString(SDR["TX_TELEFONE"] is DBNull ? "" : SDR["TX_TELEFONE"].ToString());
                            tecnico.TX_FAX = Convert.ToString(SDR["TX_FAX"] is DBNull ? "" : SDR["TX_FAX"].ToString());
                            tecnico.TX_EMAIL = Convert.ToString(SDR["TX_EMAIL"] is DBNull ? "" : SDR["TX_EMAIL"].ToString());
                            tecnico.TP_TECNICO = Convert.ToString(SDR["TP_TECNICO"] is DBNull ? "" : SDR["TP_TECNICO"].ToString());
                            tecnico.VL_CUSTO_HORA = Convert.ToInt64(SDR["VL_CUSTO_HORA"] is DBNull ? 0 : SDR["VL_CUSTO_HORA"]);
                            tecnico.FL_ATIVO= Convert.ToString(SDR["FL_ATIVO"] is DBNull ? "" : SDR["FL_ATIVO"].ToString());
                            tecnico.ID_USUARIO_COORDENADOR= Convert.ToInt64(SDR["ID_USUARIO_COORDENADOR"] is DBNull ? "0" : SDR["ID_USUARIO_COORDENADOR"].ToString());
                            tecnico.ID_USUARIO= Convert.ToInt64(SDR["ID_USUARIO"] is DBNull ? "0" : SDR["ID_USUARIO"].ToString());
                            tecnico.ID_USUARIO_SUPERVISOR = Convert.ToInt64(SDR["ID_USUARIO_TECNICOREGIONAL"] is DBNull ? "0" : SDR["ID_USUARIO_TECNICOREGIONAL"].ToString());

                            tecnico.CD_EMPRESA= Convert.ToInt64(SDR["CD_EMPRESA"] is DBNull ? "0" : SDR["CD_EMPRESA"].ToString());

                            listaTecnico.Add(tecnico);
                        }
                        cnx.Close();
                        return listaTecnico;
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
