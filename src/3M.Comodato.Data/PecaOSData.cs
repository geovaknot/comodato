using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class PecaOSData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public PecaOSData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref PecaOSDetalhamentoEntity pecaOSTecnicoEntity, ref string Mensagem)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaOSInsert");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, pecaOSTecnicoEntity.OS.ID_OS);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pecaOSTecnicoEntity.peca.CD_PECA);
                //_db.AddInParameter(dbCommand, "@p_VL_VALOR_PECA", DbType.Decimal, pecaOSTecnicoEntity.VL_VALOR_PECA);
                _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, pecaOSTecnicoEntity.QT_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, pecaOSTecnicoEntity.CD_TP_ESTOQUE_CLI_TEC);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pecaOSTecnicoEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, pecaOSTecnicoEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pecaOSTecnicoEntity.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, pecaOSTecnicoEntity.TOKEN);
                _db.AddOutParameter(dbCommand, "@p_ID_PECA_OS", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_Mensagem", DbType.String, 8000);
                _db.AddOutParameter(dbCommand, "@p_TOKEN_GERADO", DbType.Int64, 18);
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, pecaOSTecnicoEntity.DS_OBSERVACAO);
                _db.ExecuteNonQuery(dbCommand);

                pecaOSTecnicoEntity.ID_PECA_OS = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_PECA_OS"));
                Mensagem = _db.GetParameterValue(dbCommand, "@p_Mensagem").ToString();
                pecaOSTecnicoEntity.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

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

        public void Excluir(PecaOSDetalhamentoEntity pecaOSTecnicoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPecaOSDelete");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, pecaOSTecnicoEntity.OS.ID_OS);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pecaOSTecnicoEntity.peca.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, pecaOSTecnicoEntity.QT_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, pecaOSTecnicoEntity.CD_TP_ESTOQUE_CLI_TEC);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pecaOSTecnicoEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, pecaOSTecnicoEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pecaOSTecnicoEntity.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_ID_PECA_OS", DbType.Int64, pecaOSTecnicoEntity.ID_PECA_OS);

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

        public DataTable ObterLista(PecaOSEntity pecaOS)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaOSSelect");

                if (pecaOS.ID_PECA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_PECA_OS", DbType.Int64, pecaOS.ID_PECA_OS);

                if (pecaOS.OS.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, pecaOS.OS.ID_OS);

                if (!string.IsNullOrEmpty(pecaOS.peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pecaOS.peca.CD_PECA);

                if (pecaOS.QT_PECA > 0)
                    _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, pecaOS.QT_PECA);

                if (!string.IsNullOrEmpty(pecaOS.CD_TP_ESTOQUE_CLI_TEC))
                    _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, pecaOS.CD_TP_ESTOQUE_CLI_TEC);

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


        /// <summary>
        /// Obtem lista de Pecas trocadas em Ordem de serviço de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<PecaOSSinc> ObterListaPecaOsSinc(Int64 idUsuario)
        {
            try
            {
                IList<PecaOSSinc> listaPecaOs = new List<PecaOSSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select po.*, o.DT_DATA_OS from tbPecaOS po "+

                                            "inner join tbOSPadrao o(nolock) on "+

                                               " o.ID_OS = po.ID_OS " +

                                            "inner join tb_tecnico t(nolock) ON " +

                                                "t.cd_tecnico = o.cd_tecnico " +

                                            " WHERE(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO)";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            PecaOSSinc pecaOS = new PecaOSSinc
                            {
                                ID_PECA_OS = Convert.ToInt64(SDR["ID_PECA_OS"].ToString()),
                                ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QT_PECA = Convert.ToInt32(SDR["QT_PECA"] is DBNull ? 0 : SDR["QT_PECA"]),
                                CD_TP_ESTOQUE_CLI_TEC = Convert.ToChar(SDR["CD_TP_ESTOQUE_CLI_TEC"] is DBNull ? " " : SDR["CD_TP_ESTOQUE_CLI_TEC"].ToString()),
                                nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]),
                                dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]),
                                DS_OBSERVACAO = SDR["DS_OBSERVACAO"].ToString() ?? "",
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString()),
                                DT_DATA_OS = Convert.ToDateTime(SDR["DT_DATA_OS"] is DBNull ? "01/01/2000" : SDR["DT_DATA_OS"])
                            };

                            listaPecaOs.Add(pecaOS);
                        }
                        cnx.Close();
                        return listaPecaOs;
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
        
        public List<PecaOSSinc> ObterListaPecaOsEmail(Int64 ID_OS)
        {
            DbConnection connection = null;
            List<PecaOSSinc> listaOS = new List<PecaOSSinc>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoPecaEmailSelect");

                if (ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int32, ID_OS);
                
                connection = _db.CreateConnection();
                dbCommand.Connection = connection;

                if (connection.State == ConnectionState.Open)
                    connection.Close();

                dbCommand.Connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                listaOS = mapOS(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return listaOS;
        }


        private List<PecaOSSinc> mapOS(DataTable dataTable)
        {
            List<PecaOSSinc> listaOS = new List<PecaOSSinc>();

            foreach (DataRow SDR in dataTable.Rows)
            {
                PecaOSSinc pecaOS = new PecaOSSinc
                {
                    ID_PECA_OS = Convert.ToInt64(SDR["ID_PECA_OS"].ToString()),
                    ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString()),
                    CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                    DS_PECA = Convert.ToString(SDR["DS_PECA"] is DBNull ? "" : SDR["DS_PECA"].ToString()),
                    QT_PECA = Convert.ToInt32(SDR["QT_PECA"] is DBNull ? 0 : SDR["QT_PECA"]),
                    CD_TP_ESTOQUE_CLI_TEC = Convert.ToChar(SDR["CD_TP_ESTOQUE_CLI_TEC"] is DBNull ? " " : SDR["CD_TP_ESTOQUE_CLI_TEC"].ToString()),
                    nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]),
                    dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]),
                    DS_OBSERVACAO = SDR["DS_OBSERVACAO"].ToString() ?? "",
                    TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString()),
                    DT_DATA_OS = Convert.ToDateTime(SDR["DT_DATA_OS"] is DBNull ? "01/01/2000" : SDR["DT_DATA_OS"])
                };
                listaOS.Add(pecaOS);
            }

            return listaOS;
        }


    }
}
