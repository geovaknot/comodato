using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/AtivoClienteAPI")]
    [Authorize]
    public class AtivoClienteAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("ObterListaAtivoClienteSinc")]
        public IHttpActionResult ObterListaAtivoClienteSinc(Int64 idUsuario)
        {
            IList<Entity.AtivoClienteSinc> listaAtivoCliente = new List<Entity.AtivoClienteSinc>();
            try
            {
                //ativoClienteEntity.cliente.CD_CLIENTE = null;
                AtivoClienteData ativoClienteData = new AtivoClienteData();
                listaAtivoCliente = ativoClienteData.ObterListaAtivoClienteSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("ATIVO_CLIENTE", JsonConvert.SerializeObject(listaAtivoCliente, Formatting.None));
                JO.Add("ATIVO_CLIENTE", JArray.FromObject(listaAtivoCliente));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ObterListaTipoServico")]
        public IHttpActionResult ObterListaTipoServico(Int64 codigoCliente)
        {
            try
            {
                bool clienteDI = false;

                SegmentoEntity segmento = null;
                ClienteData clienteData = new ClienteData();
                DataTable dtCliente = clienteData.ObterLista(new ClienteEntity() { CD_CLIENTE = codigoCliente });
                if (dtCliente.Rows.Count > 0)
                {
                    SegmentoData segmentoData = new SegmentoData();
                    segmento = segmentoData.ObterLista(new SegmentoEntity() { ID_SEGMENTO = Convert.ToInt64(dtCliente.Rows[0]["ID_SEGMENTO"]) }).FirstOrDefault();
                }

                clienteDI = segmento != null &&
                    segmento.DS_SEGMENTO_MIN == ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CodigoSegmentoDistribuidor);


                List<TipoEntity> tipos = new List<TipoEntity>();
                TipoEntity tipoEntity = new TipoEntity();
                DataTableReader dataTableReader = new TipoData().ObterLista(tipoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        TipoEntity tipo = new TipoEntity
                        {
                            CD_TIPO = Convert.ToInt64(dataTableReader["CD_TIPO"]),
                            DS_TIPO = dataTableReader["DS_TIPO"].ToString()
                        };

                        tipo.FL_SEGMENTO_DI = false;
                        if (dataTableReader["FL_SEGMENTO_DI"] != DBNull.Value)
                        {
                            tipo.FL_SEGMENTO_DI = (bool)dataTableReader["FL_SEGMENTO_DI"];
                        }

                        if (!clienteDI && tipo.FL_SEGMENTO_DI.Value)
                        {
                            continue;
                        }

                        tipos.Add(tipo);
                    }
                }


                JObject JO = new JObject();
                JO.Add("ATIVO_CLIENTE_TIPO_SERVICO", JsonConvert.SerializeObject(tipos, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(AtivoClienteEntity AtivoFixo)
        {
            if (AtivoFixo == null)
                AtivoFixo = new AtivoClienteEntity();
            AtivoFixo.bidAtivo = true;
            List<AtivoClienteEntity> listaAtivosFixos = new List<AtivoClienteEntity>();

            try
            {
                DataTableReader dataTableReader = new AtivoClienteData().ObterLista(AtivoFixo).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        AtivoClienteEntity ativoFixo = new AtivoClienteEntity
                        {

                            ID_ATIVO_CLIENTE = Convert.ToInt64(dataTableReader["ID_ATIVO_CLIENTE"]),

                            cliente = new ClienteEntity()
                            {
                                CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                            },
                            ativoFixo = new AtivoFixoEntity()
                            {

                                CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                                TX_ANO_MAQUINA = dataTableReader["TX_ANO_MAQUINA"].ToString(),
                                linhaProduto = new LinhaProdutoEntity
                                {
                                    CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"]),
                                    DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString()
                                },
                                modelo = new ModeloEntity
                                {
                                    CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                    DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                                },
                                situacaoAtivo = new SituacaoAtivoEntity
                                {
                                    CD_SITUACAO_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_SITUACAO_ATIVO"]),
                                    DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString()
                                },
                                statusAtivo = new StatusAtivoEntity
                                {
                                    CD_STATUS_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_STATUS_ATIVO"]),
                                    DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString()
                                }
                            },
                            NR_NOTAFISCAL = Convert.ToInt64("0" + dataTableReader["NR_NOTAFISCAL"]),
                            motivoDevolucao = new MotivoDevolucaoEntity
                            {
                                CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                                DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                            },
                            TX_OBS = dataTableReader["TX_OBS"].ToString(),
                            razaoComodato = new RazaoComodatoEntity
                            {
                                CD_RAZAO_COMODATO = Convert.ToInt64("0" + dataTableReader["CD_RAZAO"]),
                                DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString()
                            },
                            tipo = new TipoEntity
                            {
                                CD_TIPO = Convert.ToInt64("0" + dataTableReader["CD_TIPO"]),
                                DS_TIPO = dataTableReader["DS_TIPO"].ToString()
                            },
                            TX_TERMOPGTO = dataTableReader["TX_TERMOPGTO"].ToString()
                        };


                        if (dataTableReader["DT_NOTAFISCAL"] != DBNull.Value)
                        {
                            ativoFixo.DT_NOTAFISCAL = Convert.ToDateTime(dataTableReader["DT_NOTAFISCAL"]);
                        }

                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                        {
                            ativoFixo.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]);
                           
                        }

                        if (dataTableReader["VL_ALUGUEL"] != DBNull.Value)
                        {
                            ativoFixo.VL_ALUGUEL = Convert.ToDecimal(dataTableReader["VL_ALUGUEL"]);
                        }

                        if (dataTableReader["QTD_MESES_LOCACAO"] != DBNull.Value)
                        {
                            ativoFixo.QTD_MESES_LOCACAO = Convert.ToInt32(dataTableReader["QTD_MESES_LOCACAO"]);
                        }

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

        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(Int64 ID)
        {
            DadosFaturamentoEntity faturamentoEntity = new DadosFaturamentoEntity();
            Models.DadosFaturamento faturamentoModel = new Models.DadosFaturamento();
            DadosFaturamentoEntity faturamento = new DadosFaturamentoEntity();

            try
            {
                faturamentoEntity.ID = ID;
                DataTableReader dataTableReader = new AtivoClienteData().ObterListaFaturamento(faturamentoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarfaturamentoEntity(dataTableReader, faturamentoEntity);
                        CarregarfaturamentoModel(dataTableReader, faturamentoModel);
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { faturamentoEntity = faturamentoEntity, faturamento = faturamento });
        }

        protected void CarregarfaturamentoEntity(DataTableReader dataTableReader, DadosFaturamentoEntity faturamentoEntity)
        {
            faturamentoEntity.ID = Convert.ToInt64("0" + dataTableReader["ID"]);
            faturamentoEntity.AluguelApos3anos = Convert.ToDouble("0" + dataTableReader["AluguelApos3anos"]);
            faturamentoEntity.CD_Material = dataTableReader["CD_Material"].ToString();
            faturamentoEntity.DepartamentoVenda = dataTableReader["DepartamentoVenda"].ToString();
            faturamentoEntity.DT_UltimoFaturamento = Convert.ToDateTime(dataTableReader["DT_UltimoFaturamento"]);

        }

        protected void CarregarfaturamentoModel(DataTableReader dataTableReader, Models.DadosFaturamento faturamento)
        {
            faturamento.ID = Convert.ToInt64("0" + dataTableReader["ID"]);
            faturamento.AluguelApos3anos = Convert.ToDouble("0" + dataTableReader["AluguelApos3anos"]);
            faturamento.CD_Material = dataTableReader["CD_Material"].ToString();
            faturamento.DepartamentoVenda = dataTableReader["DepartamentoVenda"].ToString();
            faturamento.DT_UltimoFaturamento = Convert.ToDateTime(dataTableReader["DT_UltimoFaturamento"]);

        }
        [HttpGet]
        [Route("Inativar")]
        public HttpResponseMessage Inativar(Int64 ID)
        {
            DadosFaturamentoEntity faturamentoEntity = new DadosFaturamentoEntity();
            Models.DadosFaturamento faturamentoModel = new Models.DadosFaturamento();
            DadosFaturamentoEntity faturamento = new DadosFaturamentoEntity();

            try
            {
                faturamentoEntity.ID = ID;
                faturamentoEntity.Ativo = false;
                new AtivoClienteData().InativarFaturamento(faturamentoEntity);

                
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { faturamentoEntity = faturamentoEntity, faturamento = faturamento });
        }

    }
}


