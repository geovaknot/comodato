using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/AtivoAPI")]
    [Authorize]
    public class AtivoAPIController : BaseAPIController
    {

        /// <summary>
        /// Obter lista de ATIVOS que o tecnico atende (para o sincronismo Mobile)
        /// </summary>
        /// <param name="idUsuario">Id do usuario que esta navegando</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObterListaAtivoFixoSinc")]
        public IHttpActionResult ObterListaAtivoFixoSinc(Int64 idUsuario)
        {
            IList<Entity.AtivoFixoSinc> listaAtivoFixo = new List<Entity.AtivoFixoSinc>();
            try
            {
                //ativoClienteEntity.cliente.CD_CLIENTE = null;
                AtivoFixoData ativoFixoData = new AtivoFixoData();
                listaAtivoFixo = ativoFixoData.ObterListaAtivoFixoSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("ATIVO_FIXO", JsonConvert.SerializeObject(listaAtivoFixo, Formatting.None));
                JO.Add("ATIVO_FIXO", JArray.FromObject(listaAtivoFixo));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("ObterListaAtivoCliente")]
        public IHttpActionResult ObterListaAtivoCliente(Int32 CD_Cliente, bool? SomenteATIVOSsemDTDEVOLUCAO = false)
        {
            List<Models.ListaAtivoCliente> listaAtivosClientes = new List<Models.ListaAtivoCliente>();

            try
            {
                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();
                ativoClienteEntity.cliente.CD_CLIENTE = CD_Cliente;
                DataTableReader dataTableReader = new AtivoClienteData().ObterListaEquipamentoAlocado(ativoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaAtivoCliente ativoCliente = new Models.ListaAtivoCliente
                        {
                            CD_CLIENTE = dataTableReader["CD_CLIENTE"].ToString(),
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            cdsPrograma = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                            cdsTipo = dataTableReader["DS_TIPO"].ToString() + " " + dataTableReader["TX_TIPO"].ToString(),
                            DS_ATIVO_FIXO = dataTableReader["DS_ATIVO_FIXO"].ToString(),
                            NR_NOTAFISCAL = dataTableReader["NR_NOTAFISCAL"].ToString(),
                            CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                            DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                        };

                        if (dataTableReader["DT_INCLUSAO"] != DBNull.Value)
                            ativoCliente.DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSAO"]); //.ToString("dd/MM/yyyy");
                        if (dataTableReader["DT_NOTAFISCAL"] != DBNull.Value)
                            ativoCliente.DT_NOTAFISCAL = Convert.ToDateTime(dataTableReader["DT_NOTAFISCAL"]); //.ToString("dd/MM/yyyy");
                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                            ativoCliente.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]); //.ToString("dd/MM/yyyy");
                        if(dataTableReader["DT_ULTIMA_MANUTENCAO"] != DBNull.Value)
                            ativoCliente.DT_ULTIMA_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_ULTIMA_MANUTENCAO"]).ToString("dd/MM/yyyy HH:mm");

                        if (dataTableReader["DS_MODELO"] != DBNull.Value)
                            ativoCliente.DS_MODELO = dataTableReader["DS_MODELO"].ToString();

                        if (SomenteATIVOSsemDTDEVOLUCAO == true)
                        {
                            if (dataTableReader["DT_DEVOLUCAO"] == DBNull.Value)
                                listaAtivosClientes.Add(ativoCliente);
                        }
                        else
                            listaAtivosClientes.Add(ativoCliente);

                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return BadRequest(ex.Message);
            }

            //return Request.CreateResponse(HttpStatusCode.OK, new { listaAtivosClientes = listaAtivosClientes });
            JObject JO = new JObject();
            JO.Add("listaAtivosClientes", JsonConvert.SerializeObject(listaAtivosClientes, Formatting.None));
            return Ok(JO);

        }

        [HttpPost]
        [Route("ObterListaAtivoClienteNaoAtendidos")]
        public IHttpActionResult ObterListaAtivoClienteNaoAtendidos(Int32 CD_Cliente, bool? SomenteATIVOSsemDTDEVOLUCAO = false, Int64? ID_VISITA = null)
        {
            List<Models.ListaAtivoCliente> listaAtivosClientes = new List<Models.ListaAtivoCliente>();

            try
            {
                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();
                ativoClienteEntity.cliente.CD_CLIENTE = CD_Cliente;
                DataTableReader dataTableReader = new AtivoClienteData().ObterListaEquipamentoAlocado(ativoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaAtivoCliente ativoCliente = new Models.ListaAtivoCliente
                        {
                            CD_CLIENTE = dataTableReader["CD_CLIENTE"].ToString(),
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            cdsPrograma = dataTableReader["DS_LINHA_PRODUTO"].ToString(),
                            cdsTipo = dataTableReader["DS_TIPO"].ToString() + " " + dataTableReader["TX_TIPO"].ToString(),
                            DS_ATIVO_FIXO = dataTableReader["DS_ATIVO_FIXO"].ToString(),
                            NR_NOTAFISCAL = dataTableReader["NR_NOTAFISCAL"].ToString(),
                            CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                            DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                        };

                        if (dataTableReader["DT_INCLUSAO"] != DBNull.Value)
                            ativoCliente.DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSAO"]); //.ToString("dd/MM/yyyy");
                        if (dataTableReader["DT_NOTAFISCAL"] != DBNull.Value)
                            ativoCliente.DT_NOTAFISCAL = Convert.ToDateTime(dataTableReader["DT_NOTAFISCAL"]); //.ToString("dd/MM/yyyy");
                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                            ativoCliente.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]); //.ToString("dd/MM/yyyy");
                        if (dataTableReader["DT_ULTIMA_MANUTENCAO"] != DBNull.Value)
                            ativoCliente.DT_ULTIMA_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_ULTIMA_MANUTENCAO"]).ToString("dd/MM/yyyy");

                        bool adicionarAtivosNaoAtendidos = true;

                        if (ID_VISITA != null)
                        {
                            OSEntity osEntity = new OSEntity();
                            osEntity.visita.ID_VISITA = Convert.ToInt64(ID_VISITA);
                            osEntity.ativoFixo.CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString();
                            DataTableReader dtReader = new OSData().ObterLista(osEntity).CreateDataReader();

                            if (dtReader.HasRows)
                            {
                                while (dtReader.Read())
                                {
                                    // Somente status Cancelada permite ser novamente utilizado o ativo
                                    if (Convert.ToInt64(dtReader["ST_TP_STATUS_VISITA_OS"]) == Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOS.Cancelada))
                                        adicionarAtivosNaoAtendidos = true;
                                    else
                                        adicionarAtivosNaoAtendidos = false;
                                }
                            }

                            if (dtReader != null)
                            {
                                dtReader.Dispose();
                                dtReader = null;
                            }
                        }

                        if (SomenteATIVOSsemDTDEVOLUCAO == true)
                        {
                            if (dataTableReader["DT_DEVOLUCAO"] == DBNull.Value)
                            {
                                if (adicionarAtivosNaoAtendidos == true)
                                    listaAtivosClientes.Add(ativoCliente);
                            }

                        }
                        else
                        {
                            if (adicionarAtivosNaoAtendidos == true)
                                listaAtivosClientes.Add(ativoCliente);
                        }

                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return BadRequest(ex.Message);
            }

            //return Request.CreateResponse(HttpStatusCode.OK, new { listaAtivosClientes = listaAtivosClientes });
            JObject JO = new JObject();
            JO.Add("listaAtivosClientes", JsonConvert.SerializeObject(listaAtivosClientes, Formatting.None));
            return Ok(JO);

        }

        //ObterListaComboAtivosRecolhidos
        [HttpPost]
        [Route("ObterListaComboAtivosRecolhidos")]
        public IHttpActionResult ObterListaComboAtivosRecolhidos(Int32 CD_Cliente)
        {
            List<Models.ListaAtivoCliente> listaAtivosClientes = new List<Models.ListaAtivoCliente>();

            try
            {
                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();
                if (CD_Cliente != 0)
                    ativoClienteEntity.cliente.CD_CLIENTE = CD_Cliente;

                DataTableReader dataTableReader = new AtivoClienteData().ObterListaComboAtivosRecolhidos(ativoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.ListaAtivoCliente ativoCliente = new Models.ListaAtivoCliente
                        {
                            CD_CLIENTE = dataTableReader["CD_CLIENTE"].ToString(),
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString(),
                            DS_ATIVO_FIXO = dataTableReader["DS_ATIVO_FIXO"].ToString()
                        };

                        listaAtivosClientes.Add(ativoCliente);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return BadRequest(ex.Message);
            }

            //return Request.CreateResponse(HttpStatusCode.OK, new { listaAtivosClientes = listaAtivosClientes });
            JObject JO = new JObject();
            JO.Add("listaAtivosClientes", JsonConvert.SerializeObject(listaAtivosClientes, Formatting.None));
            return Ok(JO);
        }

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(AtivoFixoEntity AtivoFixo)
        {
            if (AtivoFixo == null)
                AtivoFixo = new AtivoFixoEntity();
            AtivoFixo.FL_STATUS = true;
            List<AtivoFixoEntity> listaAtivosFixos = new List<AtivoFixoEntity>();

            try
            {
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(AtivoFixo).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        AtivoFixoEntity ativoFixo = new AtivoFixoEntity
                        {
                            CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                            DS_ATIVO_FIXO = dataTableReader["DS_ATIVO_FIXO"].ToString(),
                            modelo = new ModeloEntity()
                            {
                                CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                            },
                            DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSÃO"]),
                            TX_ANO_MAQUINA = dataTableReader["TX_ANO_MÁQUINA"].ToString(),
                            statusAtivo = new StatusAtivoEntity()
                            {
                                CD_STATUS_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_STATUS_ATIVO"]),
                                DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString()
                            },
                            situacaoAtivo = new SituacaoAtivoEntity()
                            {
                                CD_SITUACAO_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_SITUACAO_ATIVO"]),
                                DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString()
                            },
                            TX_TIPO = dataTableReader["TX_TIPO"].ToString(),
                            linhaProduto = new LinhaProdutoEntity()
                            {
                                CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"]),
                                DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString()
                            },
                            FL_STATUS = Convert.ToBoolean(dataTableReader["FL_STATUS"])
                        };

                        if (dataTableReader["DT_INVENTARIO"] != DBNull.Value)
                            ativoFixo.DT_INVENTARIO = Convert.ToDateTime(dataTableReader["DT_INVENTARIO"]); //.ToString("dd/MM/yyyy");


                        if (dataTableReader["DT_INCLUSÃO"] != DBNull.Value)
                            ativoFixo.DT_INCLUSAO = Convert.ToDateTime(dataTableReader["DT_INCLUSÃO"]);

                        if (dataTableReader["DT_FIM_GARANTIA"] != DBNull.Value)
                            ativoFixo.DT_FIM_GARANTIA = Convert.ToDateTime(dataTableReader["DT_FIM_GARANTIA"]);

                        if (dataTableReader["DT_MANUTENCAO"] != DBNull.Value)
                            ativoFixo.DT_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_MANUTENCAO"]);

                        if (dataTableReader["DT_FIM_MANUTENCAO"] != DBNull.Value)
                            ativoFixo.DT_FIM_MANUTENCAO = Convert.ToDateTime(dataTableReader["DT_FIM_MANUTENCAO"]);

                        listaAtivosFixos.Add(ativoFixo);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return BadRequest(ex.Message);
            }

            //return Request.CreateResponse(HttpStatusCode.OK, new { listaAtivosClientes = listaAtivosClientes });
            JObject JO = new JObject();
            JO.Add("listaAtivosFixos", JsonConvert.SerializeObject(listaAtivosFixos, Formatting.None));
            return Ok(JO);

        }

    }
}