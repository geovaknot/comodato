using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class ClienteData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public ClienteData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref ClienteEntity Cliente)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteInsert");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, Cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, Cliente.grupo.CD_GRUPO);
                _db.AddInParameter(dbCommand, "@p_CD_RAC", DbType.String, Cliente.CD_RAC);
                _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int32, Cliente.vendedor.CD_VENDEDOR);
                _db.AddInParameter(dbCommand, "@p_NR_CNPJ", DbType.String, Cliente.NR_CNPJ);
                _db.AddInParameter(dbCommand, "@p_NM_CLIENTE", DbType.String, Cliente.NM_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, Cliente.EN_ENDERECO);
                _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, Cliente.EN_BAIRRO);
                _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, Cliente.EN_CIDADE);
                _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, Cliente.EN_ESTADO);
                _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, Cliente.EN_CEP);
                _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, Cliente.regiao.CD_REGIAO);
                _db.AddInParameter(dbCommand, "@p_CD_FILIAL", DbType.String, Cliente.CD_FILIAL);
                _db.AddInParameter(dbCommand, "@p_CD_ABC", DbType.String, Cliente.CD_ABC);
                _db.AddInParameter(dbCommand, "@p_CL_CLIENTE", DbType.String, Cliente.CL_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, Cliente.TX_TELEFONE);
                _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, Cliente.TX_FAX);
                _db.AddInParameter(dbCommand, "@p_DT_DESATIVACAO", DbType.DateTime, Cliente.DT_DESATIVACAO);
                _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Int32, Cliente.executivo.CD_EXECUTIVO);
                _db.AddInParameter(dbCommand, "@p_BPCS", DbType.Boolean, Cliente.BPCS);
                _db.AddInParameter(dbCommand, "@p_QT_PERIODO", DbType.Int32, Cliente.QT_PERIODO);
                _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, Cliente.TX_EMAIL);
                _db.AddInParameter(dbCommand, "@p_FL_PESQ_SATISF", DbType.String, Cliente.FL_PESQ_SATISF);
                _db.AddInParameter(dbCommand, "@p_ID_SEGMENTO", DbType.Int64, Cliente.Segmento.ID_SEGMENTO);
                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.String, Cliente.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_FL_KAT_FIXO", DbType.Boolean, Cliente.FL_KAT_FIXO);
                _db.AddInParameter(dbCommand, "@p_DS_CLASSIFICACAO_KAT", DbType.String, Cliente.DS_CLASSIFICACAO_KAT);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Cliente.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_ID_USUARIO_TECNICOREGIONAL", DbType.Int64, Cliente.UsuarioTecnicoRegional.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_CD_BCPS", DbType.String, Cliente.CD_BCPS);
                _db.AddInParameter(dbCommand, "@p_FL_AtivaPlanoZero", DbType.String, Cliente.FL_AtivaPlanoZero);
                _db.AddInParameter(dbCommand, "@p_QTD_PeriodoPlanoZero", DbType.String, Cliente.QTD_PeriodoPlanoZero);
                _db.AddInParameter(dbCommand, "@p_EmailsInfo", DbType.String, Cliente.EmailsInfo);

                //_db.AddOutParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                //Cliente.CD_CLIENTE = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_CD_CLIENTE"));

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

        public void Excluir(ClienteEntity Cliente)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteDelete");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);

                if (Cliente.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Cliente.nidUsuarioAtualizacao);
                }

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

        public bool AlterarFlagEnvioPesquisa(ClienteEntity Cliente)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClientePesqSatisfUpdate");
                if (Cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.FL_PESQ_SATISF))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_PESQ_SATISF", DbType.String, Cliente.FL_PESQ_SATISF);
                }

                if (Cliente.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Cliente.nidUsuarioAtualizacao);
                }

                _db.ExecuteNonQuery(dbCommand);

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

        public bool Alterar(ClienteEntity Cliente)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);

                _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, Cliente.grupo.CD_GRUPO);

                if (!string.IsNullOrEmpty(Cliente.CD_RAC))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_RAC", DbType.String, Cliente.CD_RAC);
                }

                if (Cliente.vendedor.CD_VENDEDOR > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int32, Cliente.vendedor.CD_VENDEDOR);
                }

                if (!string.IsNullOrEmpty(Cliente.NR_CNPJ))
                {
                    _db.AddInParameter(dbCommand, "@p_NR_CNPJ", DbType.String, Cliente.NR_CNPJ);
                }

                if (!string.IsNullOrEmpty(Cliente.NM_CLIENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_NM_CLIENTE", DbType.String, Cliente.NM_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_ENDERECO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, Cliente.EN_ENDERECO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_BAIRRO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, Cliente.EN_BAIRRO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_CIDADE))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, Cliente.EN_CIDADE);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_ESTADO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, Cliente.EN_ESTADO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_CEP))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, Cliente.EN_CEP);
                }

                if (!string.IsNullOrEmpty(Cliente.regiao.CD_REGIAO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, Cliente.regiao.CD_REGIAO);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_FILIAL))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_FILIAL", DbType.String, Cliente.CD_FILIAL);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_ABC))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ABC", DbType.String, Cliente.CD_ABC);
                }

                if (!string.IsNullOrEmpty(Cliente.CL_CLIENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_CL_CLIENTE", DbType.String, Cliente.CL_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_TELEFONE))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, Cliente.TX_TELEFONE);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_FAX))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, Cliente.TX_FAX);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_EMAIL))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, Cliente.TX_EMAIL);
                }

                if (Cliente.usuario.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.String, Cliente.usuario.nidUsuario);
                }

                _db.AddInParameter(dbCommand, "@p_DT_DESATIVACAO", DbType.DateTime, Cliente.DT_DESATIVACAO);

                _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Int32, Cliente.executivo.CD_EXECUTIVO);

                if (Cliente.BPCS != null)
                {
                    _db.AddInParameter(dbCommand, "@p_BPCS", DbType.Boolean, Cliente.BPCS);
                }

                if (Cliente.EmailsInfo != null)
                {
                    _db.AddInParameter(dbCommand, "@p_EmailsInfo", DbType.String, Cliente.EmailsInfo);
                }

                _db.AddInParameter(dbCommand, "@p_QT_PERIODO", DbType.Int32, Cliente.QT_PERIODO);

                if (!string.IsNullOrEmpty(Cliente.FL_PESQ_SATISF))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_PESQ_SATISF", DbType.String, Cliente.FL_PESQ_SATISF);
                }

                if (Cliente.Segmento.ID_SEGMENTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_SEGMENTO", DbType.Int64, Cliente.Segmento.ID_SEGMENTO);
                }

                if (Cliente.FL_KAT_FIXO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_FL_KAT_FIXO", DbType.Boolean, Cliente.FL_KAT_FIXO);
                }

                if (!string.IsNullOrEmpty(Cliente.DS_CLASSIFICACAO_KAT))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_CLASSIFICACAO_KAT", DbType.String, Cliente.DS_CLASSIFICACAO_KAT);
                }

                if (Cliente.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Cliente.nidUsuarioAtualizacao);
                }

                if (Cliente.UsuarioTecnicoRegional.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_TECNICOREGIONAL", DbType.Int64, Cliente.UsuarioTecnicoRegional.nidUsuario);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_BCPS))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_BCPS", DbType.String, Cliente.CD_BCPS);
                }

                if (!string.IsNullOrEmpty(Cliente.FL_AtivaPlanoZero))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_AtivaPlanoZero", DbType.String, Cliente.FL_AtivaPlanoZero);
                }

                if (Cliente.QTD_PeriodoPlanoZero != null)
                {
                    _db.AddInParameter(dbCommand, "@p_QTD_PeriodoPlanoZero", DbType.Int32, Cliente.QTD_PeriodoPlanoZero);
                }

                

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

        public ClienteEntity ObterPorId(long id)
        {
            return this.ObterListaEntity(new ClienteEntity() { CD_CLIENTE = id }).FirstOrDefault();
        }

        public List<ClienteEntity> ObterListaEntity(ClienteEntity cliente)
        {
            var lista = new List<ClienteEntity>();
            var reader = this.ObterLista(cliente, null).CreateDataReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    lista.Add(MontarObjeto(reader));
                }
            }

            if (reader != null)
            {
                reader.Dispose();
            }

            return lista;
        }
 


        public DataTable ObterLista(ClienteEntity Cliente, Int64? nidUsuario = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteSelect");

                if (nidUsuario != null)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);
                }

                if (Cliente.CD_CLIENTE != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.grupo.CD_GRUPO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, Cliente.grupo.CD_GRUPO);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_RAC))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_RAC", DbType.String, Cliente.CD_RAC);
                }

                if (Cliente.vendedor.CD_VENDEDOR != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int32, Cliente.vendedor.CD_VENDEDOR);
                }

                if (!string.IsNullOrEmpty(Cliente.NR_CNPJ))
                {
                    _db.AddInParameter(dbCommand, "@p_NR_CNPJ", DbType.String, Cliente.NR_CNPJ);
                }

                if (!string.IsNullOrEmpty(Cliente.NM_CLIENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_NM_CLIENTE", DbType.String, Cliente.NM_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_ENDERECO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, Cliente.EN_ENDERECO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_BAIRRO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, Cliente.EN_BAIRRO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_CIDADE))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, Cliente.EN_CIDADE);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_ESTADO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, Cliente.EN_ESTADO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_CEP))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, Cliente.EN_CEP);
                }

                if (!string.IsNullOrEmpty(Cliente.regiao.CD_REGIAO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, Cliente.regiao.CD_REGIAO);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_FILIAL))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_FILIAL", DbType.String, Cliente.CD_FILIAL);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_ABC))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ABC", DbType.String, Cliente.CD_ABC);
                }

                if (!string.IsNullOrEmpty(Cliente.CL_CLIENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_CL_CLIENTE", DbType.String, Cliente.CL_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_TELEFONE))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, Cliente.TX_TELEFONE);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_FAX))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, Cliente.TX_FAX);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_EMAIL))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, Cliente.TX_EMAIL);
                }

                if (Cliente.DT_DESATIVACAO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_DESATIVACAO", DbType.DateTime, Cliente.DT_DESATIVACAO);
                }

                if (Cliente.executivo.CD_EXECUTIVO != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Int32, Cliente.executivo.CD_EXECUTIVO);
                }

                if (Cliente.BPCS != null)
                {
                    _db.AddInParameter(dbCommand, "@p_BPCS", DbType.Boolean, Cliente.BPCS);
                }

                if (Cliente.QT_PERIODO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QT_PERIODO", DbType.Int32, Cliente.QT_PERIODO);
                }

                if (!String.IsNullOrEmpty(Cliente.FL_PESQ_SATISF))
                {
                    _db.AddInParameter(dbCommand, "@p_QT_PERIODO", DbType.String, Cliente.FL_PESQ_SATISF);
                }

                if (Cliente.usuario.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.String, Cliente.usuario.nidUsuario);
                }

                if (Cliente.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Cliente.nidUsuarioAtualizacao);
                }

                if (Cliente.Segmento.ID_SEGMENTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_SEGMENTO", DbType.Int64, Cliente.Segmento.ID_SEGMENTO);
                }

                if (!string.IsNullOrEmpty(Cliente.Segmento.DS_SEGMENTO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_SEGMENTO", DbType.Int64, Cliente.Segmento.DS_SEGMENTO);
                }

                if (Cliente.usuario.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioCliente", DbType.Int64, Cliente.usuario.nidUsuario);
                }

                if (Cliente.FL_KAT_FIXO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_FL_KAT_FIXO", DbType.Boolean, Cliente.FL_KAT_FIXO);
                }

                if (!string.IsNullOrEmpty(Cliente.DS_CLASSIFICACAO_KAT))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_CLASSIFICACAO_KAT", DbType.String, Cliente.DS_CLASSIFICACAO_KAT);
                }

                /*
                if (Cliente.UsuarioTecnicoRegional.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@ID_USUARIO_TECNICOREGIONAL", DbType.Int64, Cliente.UsuarioTecnicoRegional.nidUsuario);
                }
                */

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

        public DataTable ObterListaCampoCliente(ClienteEntity Cliente, Int64? nidUsuario = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteCampoSelect");

                if (nidUsuario != null)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);
                }

                if (Cliente.CD_CLIENTE != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.grupo.CD_GRUPO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, Cliente.grupo.CD_GRUPO);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_RAC))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_RAC", DbType.String, Cliente.CD_RAC);
                }

                if (Cliente.vendedor.CD_VENDEDOR != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int32, Cliente.vendedor.CD_VENDEDOR);
                }

                if (!string.IsNullOrEmpty(Cliente.NR_CNPJ))
                {
                    _db.AddInParameter(dbCommand, "@p_NR_CNPJ", DbType.String, Cliente.NR_CNPJ);
                }

                if (!string.IsNullOrEmpty(Cliente.NM_CLIENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_NM_CLIENTE", DbType.String, Cliente.NM_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_ENDERECO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, Cliente.EN_ENDERECO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_BAIRRO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, Cliente.EN_BAIRRO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_CIDADE))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, Cliente.EN_CIDADE);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_ESTADO))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, Cliente.EN_ESTADO);
                }

                if (!string.IsNullOrEmpty(Cliente.EN_CEP))
                {
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, Cliente.EN_CEP);
                }

                if (!string.IsNullOrEmpty(Cliente.regiao.CD_REGIAO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, Cliente.regiao.CD_REGIAO);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_FILIAL))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_FILIAL", DbType.String, Cliente.CD_FILIAL);
                }

                if (!string.IsNullOrEmpty(Cliente.CD_ABC))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ABC", DbType.String, Cliente.CD_ABC);
                }

                if (!string.IsNullOrEmpty(Cliente.CL_CLIENTE))
                {
                    _db.AddInParameter(dbCommand, "@p_CL_CLIENTE", DbType.String, Cliente.CL_CLIENTE);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_TELEFONE))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, Cliente.TX_TELEFONE);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_FAX))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, Cliente.TX_FAX);
                }

                if (!string.IsNullOrEmpty(Cliente.TX_EMAIL))
                {
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, Cliente.TX_EMAIL);
                }

                if (Cliente.DT_DESATIVACAO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_DT_DESATIVACAO", DbType.DateTime, Cliente.DT_DESATIVACAO);
                }

                if (Cliente.executivo.CD_EXECUTIVO != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Int32, Cliente.executivo.CD_EXECUTIVO);
                }

                if (Cliente.BPCS != null)
                {
                    _db.AddInParameter(dbCommand, "@p_BPCS", DbType.Boolean, Cliente.BPCS);
                }

                if (Cliente.QT_PERIODO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QT_PERIODO", DbType.Int32, Cliente.QT_PERIODO);
                }

                if (!String.IsNullOrEmpty(Cliente.FL_PESQ_SATISF))
                {
                    _db.AddInParameter(dbCommand, "@p_QT_PERIODO", DbType.String, Cliente.FL_PESQ_SATISF);
                }

                if (Cliente.usuario.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.String, Cliente.usuario.nidUsuario);
                }

                if (Cliente.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Cliente.nidUsuarioAtualizacao);
                }

                if (Cliente.Segmento.ID_SEGMENTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_SEGMENTO", DbType.Int64, Cliente.Segmento.ID_SEGMENTO);
                }

                if (!string.IsNullOrEmpty(Cliente.Segmento.DS_SEGMENTO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_SEGMENTO", DbType.Int64, Cliente.Segmento.DS_SEGMENTO);
                }

                if (Cliente.usuario.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioCliente", DbType.Int64, Cliente.usuario.nidUsuario);
                }

                if (Cliente.FL_KAT_FIXO != null)
                {
                    _db.AddInParameter(dbCommand, "@p_FL_KAT_FIXO", DbType.Boolean, Cliente.FL_KAT_FIXO);
                }

                if (!string.IsNullOrEmpty(Cliente.DS_CLASSIFICACAO_KAT))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_CLASSIFICACAO_KAT", DbType.String, Cliente.DS_CLASSIFICACAO_KAT);
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

        public DataTable ObterListaCombo(Int64? nidUsuario = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteComboSelect");

                if (nidUsuario != null)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, nidUsuario);
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


        /// <summary>
        /// Obtem lista para o sincronismo Mobile, trazendo apenas a relação de clientes do tecnico
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public IList<ClienteSinc> ObterListaClienteSinc(Int64 idUsuario)
        {
            try
            {

                IList<ClienteSinc> listaCliente = new List<ClienteSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                          @"select c.*,
	                                               cast(coalesce((case when c.QT_PERIODO > 0 and (select count(osp.ID_OS) 
		                                                                                            from tbOSPadrao osp
		                                                                                           where osp.CD_CLIENTE = c.CD_CLIENTE 
			                                                                                         and osp.ST_STATUS_OS = 3
												                                                 ) > 0
						                                               then (select cast(count(osp.ID_OS) as float)
			                                                                   from tbOSPadrao osp
			                                                                  where osp.CD_CLIENTE = c.CD_CLIENTE and osp.ST_STATUS_OS = 3
								                                            ) * 100 / cast(c.QT_PERIODO as float)
		                                                               end), 0) as decimal(10, 2)) AS PERCENTUAL_REALIZADO
                                             from tb_cliente c 
                                            inner join tb_tecnico_cliente tc 
                                               on tc.CD_CLIENTE = c.CD_CLIENTE 
                                            inner join tb_tecnico t 
                                               on t.CD_TECNICO = tc.cd_tecnico 
                                            where c.dt_desativacao is null 
                                              and (t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO)";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            ClienteSinc cliente = new ClienteSinc();
                            cliente.CD_CLIENTE = Convert.ToInt32(SDR["CD_CLIENTE"].ToString());
                            cliente.CD_GRUPO = Convert.ToString(SDR["CD_GRUPO"] is DBNull ? "" : SDR["CD_GRUPO"].ToString());
                            cliente.CD_RAC = Convert.ToString(SDR["CD_RAC"] is DBNull ? "" : SDR["CD_RAC"].ToString());
                            cliente.CD_VENDEDOR = Convert.ToInt32(SDR["CD_VENDEDOR"] is DBNull ? 0 : SDR["CD_VENDEDOR"]);
                            cliente.NR_CNPJ = Convert.ToString(SDR["NR_CNPJ"] is DBNull ? "" : SDR["NR_CNPJ"]);
                            cliente.NM_CLIENTE = Convert.ToString(SDR["NM_CLIENTE"] is DBNull ? "" : SDR["NM_CLIENTE"]);
                            cliente.EN_ENDERECO = Convert.ToString(SDR["EN_ENDERECO"] is DBNull ? "" : SDR["EN_ENDERECO"]);
                            cliente.EN_BAIRRO = Convert.ToString(SDR["EN_BAIRRO"] is DBNull ? "" : SDR["EN_BAIRRO"]);
                            cliente.EN_CIDADE = Convert.ToString(SDR["EN_CIDADE"] is DBNull ? "" : SDR["EN_CIDADE"]);
                            cliente.EN_ESTADO = Convert.ToString(SDR["EN_ESTADO"] is DBNull ? "" : SDR["EN_ESTADO"]);
                            cliente.EN_CEP = Convert.ToString(SDR["EN_CEP"] is DBNull ? "" : SDR["EN_CEP"]);
                            cliente.CD_REGIAO = Convert.ToString(SDR["CD_REGIAO"] is DBNull ? "" : SDR["CD_REGIAO"]);
                            cliente.CD_FILIAL = Convert.ToString(SDR["CD_FILIAL"] is DBNull ? "" : SDR["CD_FILIAL"]);
                            cliente.TX_TELEFONE = Convert.ToString(SDR["TX_TELEFONE"] is DBNull ? "" : SDR["TX_TELEFONE"]);
                            cliente.TX_FAX = "";
                            cliente.DT_DESATIVACAO = Convert.ToDateTime(SDR["DT_DESATIVACAO"] is DBNull ? null : SDR["DT_DESATIVACAO"]);
                            cliente.CD_EXECUTIVO = Convert.ToInt32(SDR["CD_EXECUTIVO"] is DBNull ? 0 : SDR["CD_EXECUTIVO"]);
                            cliente.BPCS = Convert.ToBoolean(false);
                            cliente.QT_PERIODO = Convert.ToInt32(SDR["QT_PERIODO"] is DBNull ? 0 : SDR["QT_PERIODO"]);
                            cliente.TX_EMAIL = Convert.ToString(SDR["TX_EMAIL"] is DBNull ? "" : SDR["TX_EMAIL"]);
                            cliente.FL_PESQ_SATISF = Convert.ToString(SDR["FL_PESQ_SATISF"] is DBNull ? "" : SDR["FL_PESQ_SATISF"]);
                            cliente.PERCENTUAL_REALIZADO = Convert.ToDecimal(SDR["PERCENTUAL_REALIZADO"] is DBNull ? 0 : SDR["PERCENTUAL_REALIZADO"]);
                            listaCliente.Add(cliente);
                        }
                        cnx.Close();
                        return listaCliente;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ObterListaBPCS(ClienteEntity Cliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteSelectBPCS");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);

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

        public DataTable ObterListaDetalhes(ClienteEntity Cliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteDetalheSelect");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);

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

        public DataTable ObterListaDetalhesManutencao(ClienteEntity Cliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteDetalheManutencaoSelect");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);

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

        public DataTable ObterListaQtdeEquipamentos(ClienteEntity Cliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcClienteQtdeEquipamentosSelect");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Cliente.CD_CLIENTE);

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

        public DataTable CalcularKAT(int CD_CLIENTE)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcCalcularKAT");

                if (CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();
                dbCommand.CommandTimeout = 420;

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

        public bool GerarRelatorioKAT()
        {

            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcGerarRelatorioKAT");
                //dbCommand.CommandTimeout = 420;
                dbCommand.CommandTimeout = 0;

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

        public DataTable ObterListaKAT()
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioKATSelect");

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();
                //dbCommand.CommandTimeout = 420;

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

        public DataSet ObterRelatorioKAT()
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioKATSelect");

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                //dataTable = dataSet.Tables[0];
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
            return dataSet;
        }

        private ClienteEntity MontarObjeto(DataTableReader reader)
        {
            var clienteEntity = new ClienteEntity();
            clienteEntity.CD_CLIENTE = Convert.ToInt32(reader["CD_CLIENTE"]);
            clienteEntity.grupo.CD_GRUPO = reader["CD_GRUPO"].ToString();
            clienteEntity.grupo.DS_GRUPO = reader["DS_GRUPO"].ToString();
            clienteEntity.CD_RAC = reader["CD_RAC"].ToString();
            clienteEntity.vendedor.CD_VENDEDOR = Convert.ToInt64("0" + reader["CD_VENDEDOR"]);
            clienteEntity.vendedor.NM_VENDEDOR = reader["NM_VENDEDOR"].ToString();
            clienteEntity.NR_CNPJ = reader["NR_CNPJ"].ToString();
            //clienteEntity.NM_CLIENTE = reader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(reader["CD_CLIENTE"]).ToString() + ") " + reader["EN_CIDADE"].ToString() + " - " + reader["EN_ESTADO"].ToString();
            clienteEntity.NM_CLIENTE = reader["NM_CLIENTE"].ToString();
            clienteEntity.EN_ENDERECO = reader["EN_ENDERECO"].ToString();
            clienteEntity.EN_BAIRRO = reader["EN_BAIRRO"].ToString();
            clienteEntity.EN_CIDADE = reader["EN_CIDADE"].ToString();
            clienteEntity.EN_ESTADO = reader["EN_ESTADO"].ToString();
            clienteEntity.EN_CEP = reader["EN_CEP"].ToString();
            clienteEntity.regiao.CD_REGIAO = reader["CD_REGIAO"].ToString();
            clienteEntity.regiao.DS_REGIAO = reader["DS_REGIAO"].ToString();
            clienteEntity.CD_ABC = reader["CD_ABC"].ToString();
            clienteEntity.CD_FILIAL = reader["CD_FILIAL"].ToString();
            clienteEntity.CL_CLIENTE = reader["CL_CLIENTE"].ToString();
            clienteEntity.TX_TELEFONE = reader["TX_TELEFONE"].ToString();
            clienteEntity.TX_FAX = reader["TX_FAX"].ToString();
            clienteEntity.executivo.CD_EXECUTIVO = Convert.ToInt64("0" + reader["CD_EXECUTIVO"]);
            clienteEntity.executivo.NM_EXECUTIVO = reader["NM_EXECUTIVO"].ToString();
            clienteEntity.QT_PERIODO = Convert.ToInt32("0" + reader["QT_PERIODO"]);
            clienteEntity.FL_KAT_FIXO = Convert.ToBoolean(reader["FL_KAT_FIXO"]);

            if (reader["TX_EMAIL"] != DBNull.Value)
            {
                clienteEntity.TX_EMAIL = reader["TX_EMAIL"].ToString();
            }

            if (reader["DT_DESATIVACAO"] != DBNull.Value)
            {
                clienteEntity.DT_DESATIVACAO = Convert.ToDateTime(reader["DT_DESATIVACAO"]);
            }

            if (reader["FL_PESQ_SATISF"] != DBNull.Value)
            {
                clienteEntity.FL_PESQ_SATISF = reader["FL_PESQ_SATISF"].ToString();
            }

            if (reader["ID_SEGMENTO"] != DBNull.Value)
            {
                clienteEntity.Segmento.ID_SEGMENTO = Convert.ToInt64(reader["ID_SEGMENTO"]);
            }
            if (reader["DS_SEGMENTO"] != DBNull.Value)
            {
                clienteEntity.Segmento.DS_SEGMENTO = reader["DS_SEGMENTO"].ToString();
            }

            return clienteEntity;
        }

    }
}
