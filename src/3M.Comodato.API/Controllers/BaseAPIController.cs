using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "GET, PUT, POST, DELETE, HEAD")]
    public class BaseAPIController : ApiController
    {
        /// <summary>
        /// Aplicação das Regras de Agenda de Atendimento / Visita Técnica
        /// </summary>
        /// <param name="ID_VISITA"></param>
        /// <param name="nidUsuarioAtualizacao"></param>
        public void AjustaRegrasVisitaOS(Int64 ID_VISITA, Int64 nidUsuarioAtualizacao, ref string Mensagem)
        {
            bool ExisteOSPendente = false;

            // Se há uma OS Pendente o Status da Visita deve ser Pendente
            OSEntity _osEntity = new OSEntity();
            _osEntity.visita.ID_VISITA = ID_VISITA;
            _osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Pendente);
            DataTableReader dataTableReader = new OSData().ObterLista(_osEntity).CreateDataReader();

            if (dataTableReader.HasRows)
            {
                if (dataTableReader.Read())
                {
                    ExisteOSPendente = true;

                    // Busca a Visita
                    VisitaEntity visitaEntity = new VisitaEntity();
                    visitaEntity.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
                    DataTableReader dtTableReader = new VisitaData().ObterLista(visitaEntity).CreateDataReader();

                    if (dtTableReader.HasRows)
                    {
                        if (dtTableReader.Read())
                        {
                            // Se a visita já estiver Pendente, não precisa mudar seu status novamente para Pendente
                            if (Convert.ToInt32(dtTableReader["ST_TP_STATUS_VISITA_OS"]) != Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Pendente))
                            {
                                // Atualiza a Visita para Pendente
                                visitaEntity.DT_DATA_VISITA = Convert.ToDateTime(dtTableReader["DT_DATA_VISITA"]);
                                visitaEntity.cliente.CD_CLIENTE = Convert.ToInt32(dtTableReader["CD_CLIENTE"]);
                                visitaEntity.tecnico.CD_TECNICO = dtTableReader["CD_TECNICO"].ToString();
                                visitaEntity.DS_OBSERVACAO = dtTableReader["DS_OBSERVACAO"].ToString();
                                visitaEntity.DS_NOME_RESPONSAVEL = dtTableReader["DS_NOME_RESPONSAVEL"].ToString();
                                visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Pendente);
                                visitaEntity.nidUsuarioAtualizacao = nidUsuarioAtualizacao;
                                if (dtTableReader["FL_ENVIO_EMAIL_PESQ"] != DBNull.Value)
                                {
                                    visitaEntity.FL_ENVIO_EMAIL_PESQ = dtTableReader["FL_ENVIO_EMAIL_PESQ"].ToString();
                                }

                                new VisitaData().Alterar(visitaEntity);

                                Mensagem = "Existem uma ou mais OS Pendente!<br/>O Status da Visita foi alterado automaticamente para <strong>PENDENTE</strong>...";
                            }
                        }
                    }

                    if (dtTableReader != null)
                    {
                        dtTableReader.Dispose();
                        dtTableReader = null;
                    }
                }
            }

            if (dataTableReader != null)
            {
                dataTableReader.Dispose();
                dataTableReader = null;
            }

            if (ExisteOSPendente == false)
            {
                // Se há uma OS Aberta o Status da Visita deve ser Aberta
                _osEntity = new OSEntity();
                _osEntity.visita.ID_VISITA = ID_VISITA;
                _osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOS.Aberta);
                dataTableReader = new OSData().ObterLista(_osEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        // Busca a Visita
                        VisitaEntity visitaEntity = new VisitaEntity();
                        visitaEntity.ID_VISITA = Convert.ToInt64(dataTableReader["ID_VISITA"]);
                        DataTableReader dtTableReader = new VisitaData().ObterLista(visitaEntity).CreateDataReader();

                        if (dtTableReader.HasRows)
                        {
                            if (dtTableReader.Read())
                            {
                                // Se a visita já estiver Aberta, não precisa mudar seu status novamente para Aberta
                                if (Convert.ToInt32(dtTableReader["ST_TP_STATUS_VISITA_OS"]) != Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Aberta))
                                {
                                    // Atualiza a Visita para Aberta
                                    visitaEntity.DT_DATA_VISITA = Convert.ToDateTime(dtTableReader["DT_DATA_VISITA"]);
                                    visitaEntity.cliente.CD_CLIENTE = Convert.ToInt32(dtTableReader["CD_CLIENTE"]);
                                    visitaEntity.tecnico.CD_TECNICO = dtTableReader["CD_TECNICO"].ToString();
                                    visitaEntity.DS_OBSERVACAO = dtTableReader["DS_OBSERVACAO"].ToString();
                                    visitaEntity.DS_NOME_RESPONSAVEL = dtTableReader["DS_NOME_RESPONSAVEL"].ToString();
                                    visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisita.Aberta);
                                    visitaEntity.nidUsuarioAtualizacao = nidUsuarioAtualizacao;
                                    if (dtTableReader["FL_ENVIO_EMAIL_PESQ"] != DBNull.Value)
                                    {
                                        visitaEntity.FL_ENVIO_EMAIL_PESQ = dtTableReader["FL_ENVIO_EMAIL_PESQ"].ToString();
                                    }

                                    new VisitaData().Alterar(visitaEntity);

                                    Mensagem = "Existem uma ou mais OS Aberta!<br/>O Status da Visita foi alterado automaticamente para <strong>ABERTA</strong>...";
                                }
                            }
                        }

                        if (dtTableReader != null)
                        {
                            dtTableReader.Dispose();
                            dtTableReader = null;
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

            }
        }

        public string SerializarJsonRetornoView(object objeto, string camposNecessarios)
        {
            var arr = camposNecessarios.Split(',');
            if(arr == null || arr.Length == 0)
            {
                return String.Empty;
            }

            return JsonConvert.SerializeObject(objeto, Formatting.None, new JsonSerializerSettings() { ContractResolver = new RetornarCampos(arr) });
        }

        protected JObject JObjectMessage(bool success, string msg)
        {
            JObject JO = new JObject();

            JO.Add("SUCESSO", success);
            JO.Add("MENSAGEM", msg);

            return JO;
        }

        protected IList<T> OrderList<T>(IList<T> lst, string orderby, string ordertype)
        {
            if (string.IsNullOrWhiteSpace(orderby))
                return lst;
            else
            {
                var propertyInfo = typeof(VisitaPadraoEntity).GetProperty(orderby);

                if (string.IsNullOrWhiteSpace(ordertype) || ordertype.ToLower() == "asc")
                    return lst.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                else
                    return lst.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
            }
        }

        protected bool ValidarPermiteIncluirOsSalvar(OSPadraoEntity osPadraoEntity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(osPadraoEntity.HR_INICIO) && string.IsNullOrWhiteSpace(osPadraoEntity.HR_FIM))
                    return false;

                if (ControlesUtility.Utilidade.VerificarExisteOsDataHoraSemCompararOsInformada(osPadraoEntity))
                {
                    return true;
                }
                else
                {
                    VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();
                    visitaPadraoEntity.Tecnico.CD_TECNICO = osPadraoEntity.Tecnico.CD_TECNICO;
                    visitaPadraoEntity.DT_DATA_VISITA = osPadraoEntity.DT_DATA_OS;
                    visitaPadraoEntity.HR_INICIO = osPadraoEntity.HR_INICIO;
                    visitaPadraoEntity.HR_FIM = osPadraoEntity.HR_FIM;

                    if (ControlesUtility.Utilidade.VerificarExisteVisitaDataHora(visitaPadraoEntity))
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        protected bool ValidarPermiteIncluirVisitaSalvar(VisitaPadraoEntity visitaPadraoEntity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(visitaPadraoEntity.HR_INICIO) && string.IsNullOrWhiteSpace(visitaPadraoEntity.HR_FIM))
                    return false;

                if (ControlesUtility.Utilidade.VerificarExisteVisitaDataHoraSemCompararVisitaInformada(visitaPadraoEntity))
                {
                    return true;
                }
                else
                {
                    OSPadraoEntity osPadraoEntity = new OSPadraoEntity();
                    osPadraoEntity.Tecnico.CD_TECNICO = visitaPadraoEntity.Tecnico.CD_TECNICO;
                    osPadraoEntity.DT_DATA_OS = visitaPadraoEntity.DT_DATA_VISITA;
                    osPadraoEntity.HR_INICIO = visitaPadraoEntity.HR_INICIO;
                    osPadraoEntity.HR_FIM = visitaPadraoEntity.HR_FIM;

                    if (ControlesUtility.Utilidade.VerificarExisteOSDataHora(osPadraoEntity))
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }
    }
}
