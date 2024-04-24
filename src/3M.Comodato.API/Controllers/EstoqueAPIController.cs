using _3M.Comodato.Data;
using _3M.Comodato.Entity;
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
    [RoutePrefix("api/EstoqueAPI")]
    [Authorize]
    public class EstoqueAPIController : BaseAPIController
    {
        //#region Operações CRUD - Estoque

        //[AcceptVerbs("Get", "Post")]
        //[Route("ObterLista")]
        //public HttpResponseMessage ObterLista(EstoqueEntity filtroEstoque)
        //{
        //    if (null == filtroEstoque)
        //        filtroEstoque = new EstoqueEntity();

        //    EstoqueData estoqueData = new EstoqueData();
        //    using (DataTable result = estoqueData.ObterLista(filtroEstoque))
        //    {
        //        var listaEstoque = from dr in result.Rows.Cast<DataRow>()
        //                           select
        //                           new EstoqueEntity()
        //                           {
        //                               ID_ESTOQUE = Convert.ToInt64(dr["ID_ESTOQUE"]),
        //                               CD_ESTOQUE = dr["CD_ESTOQUE"].ToString(),
        //                               DS_ESTOQUE = dr["DS_ESTOQUE"].ToString(),
        //                               ID_USU_RESPONSAVEL = Convert.ToInt64(dr["ID_USU_RESPONSAVEL"]),
        //                               CD_TECNICO = dr["CD_TECNICO"].ToString(),
        //                               TP_ESTOQUE = dr["TP_ESTOQUE_TEC_3M"].ToString(),
        //                               dtmDataHoraAtualizacao = Convert.ToDateTime(dr["DT_CRIACAO"]),
        //                               bidAtivo = dr["FL_ATIVO"].ToString().Equals("S")
        //                           };

        //        return Request.CreateResponse(HttpStatusCode.OK, listaEstoque);
        //    }
        //}

        //[HttpPost]
        //[Route("Adicionar")]
        //public HttpResponseMessage Adicionar(EstoqueEntity estoque)
        //{
        //    try
        //    {
        //        EstoqueData estoqueData = new EstoqueData();

        //        bool sucesso = estoqueData.Inserir(ref estoque);
        //        return Request.CreateResponse(HttpStatusCode.Created, estoque);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        //[HttpPut]
        //[Route("Alterar")]
        //public HttpResponseMessage Alterar(EstoqueEntity estoque)
        //{
        //    try
        //    {
        //        EstoqueData estoqueData = new EstoqueData();
        //        bool sucesso = estoqueData.Alterar(estoque);
        //        if (sucesso)
        //            return Request.CreateResponse(HttpStatusCode.OK, estoque);
        //        else
        //            return Request.CreateResponse(HttpStatusCode.NotModified);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        //#endregion

        [HttpGet]
        [Route("ObterListaEstoqueSinc")]
        public IHttpActionResult ObterListaEstoqueSinc()
        {
            IList<Entity.EstoqueSinc> listaEstoque = new List<Entity.EstoqueSinc>();
            try
            {
                //ativoClienteEntity.cliente.CD_CLIENTE = null;
                EstoqueData estoqueData = new EstoqueData();
                listaEstoque = estoqueData.ObterListaEstoqueSinc();

                JObject JO = new JObject();
                //JO.Add("ESTOQUE", JsonConvert.SerializeObject(listaEstoque, Formatting.None));
                JO.Add("ESTOQUE", JArray.FromObject(listaEstoque));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("ObterListaEstoque")]
        public IHttpActionResult ObterListaEstoque(EstoqueEntity estoque)
        {
            IList<EstoqueEntity> listaEstoque = new List<EstoqueEntity>();
            try
            {
                EstoqueData estoqueData = new EstoqueData();
                if (estoque.ID_USU_RESPONSAVEL == 0)
                    throw new ArgumentNullException("estoque.ID_USU_RESPONSAVEL");

                using (DataTable dtEstoqueUsuario = estoqueData.ObterListaUsuario(estoque))
                {
                    listaEstoque = (from dr in dtEstoqueUsuario.Rows.Cast<DataRow>()
                                    select new EstoqueEntity()
                                    {
                                        ID_ESTOQUE = Convert.ToInt64(dr["ID_ESTOQUE"]),
                                        CD_ESTOQUE = dr["CD_ESTOQUE"].ToString(),
                                        DS_ESTOQUE = dr["DS_ESTOQUE"].ToString(),
                                        //ID_USU_RESPONSAVEL = Convert.ToInt64(dr["ID_USU_RESPONSAVEL"]),
                                        //CD_TECNICO = dr["CD_TECNICO"].ToString(),
                                        TP_ESTOQUE_TEC_3M = dr["TP_ESTOQUE_TEC_3M"].ToString(),
                                        dtmDataHoraAtualizacao = Convert.ToDateTime(dr["DT_CRIACAO"]),
                                        bidAtivo = dr["FL_ATIVO"].ToString().Equals("S")
                                    }).ToList();
                }

                //Ordenação por Cód Estoque
                //listaEstoque.OrderBy(f => f.ID_ESTOQUE);
                //listaEstoque.ToList();

                var listaEstoqueOrdenada = listaEstoque.AsEnumerable().OrderBy(t => t.CD_ESTOQUE).ToList();

                JObject JO = new JObject();
                JO.Add("ESTOQUE", JsonConvert.SerializeObject(listaEstoqueOrdenada, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(Int64 ID_ESTOQUE)
        {
            EstoqueEntity estoqueEntity = new EstoqueEntity();

            try
            {
                estoqueEntity.ID_ESTOQUE = ID_ESTOQUE;
                DataTableReader dataTableReader = new EstoqueData().ObterLista(estoqueEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        estoqueEntity.ID_ESTOQUE = Convert.ToInt64("0" + dataTableReader["ID_ESTOQUE"]);
                        estoqueEntity.CD_ESTOQUE = dataTableReader["ID_ESTOQUE"].ToString();
                        estoqueEntity.DS_ESTOQUE = dataTableReader["DS_ESTOQUE"].ToString();
                        estoqueEntity.ID_USU_RESPONSAVEL = Convert.ToInt64("0" + dataTableReader["ID_USU_RESPONSAVEL"]);
                        estoqueEntity.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]);
                        estoqueEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                        estoqueEntity.TP_ESTOQUE_TEC_3M = dataTableReader["TP_ESTOQUE_TEC_3M"].ToString();
                        estoqueEntity.FL_ATIVO = dataTableReader["FL_ATIVO"].ToString();
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

            return Request.CreateResponse(HttpStatusCode.OK, new { estoque = estoqueEntity });
        }

        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(EstoqueEntity estoqueEntity)
        {
            List<EstoqueEntity> estoques = new List<EstoqueEntity>();

            try
            {
                if (estoqueEntity == null)
                    estoqueEntity = new EstoqueEntity();

                DataTableReader dataTableReader = new EstoqueData().ObterLista(estoqueEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        estoqueEntity = new EstoqueEntity()
                        {
                            ID_ESTOQUE = Convert.ToInt64("0" + dataTableReader["ID_ESTOQUE"]),
                            CD_ESTOQUE = dataTableReader["CD_ESTOQUE"].ToString(),
                            DS_ESTOQUE = dataTableReader["DS_ESTOQUE"].ToString(),
                            ID_USU_RESPONSAVEL = Convert.ToInt64("0" + dataTableReader["ID_USU_RESPONSAVEL"]),
                            DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]),
                            tecnico = new TecnicoEntity()
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString()
                            },
                            TP_ESTOQUE_TEC_3M = dataTableReader["TP_ESTOQUE_TEC_3M"].ToString(),
                            FL_ATIVO = dataTableReader["FL_ATIVO"].ToString()
                        };

                        estoques.Add(estoqueEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { estoques = estoques });
        }

    }
}