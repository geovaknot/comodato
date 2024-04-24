using _3M.Comodato.API.Models;
using _3M.Comodato.Data;
//using System.Web.Mvc;
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
    [RoutePrefix("api/EstoquePecaAPI")]
    public class EstoquePecaAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("ObterCliente")]
        [Authorize]
        public HttpResponseMessage ObterCliente(string CD_CLIENTE, string CD_PECA, string CD_MODELO)
        {
            EstoquePecaEntity estoquePecaEntity = new EstoquePecaEntity();

            bool encontrado = false;

            try
            {
                EstoqueEntity estoqueCliente = new EstoqueEntity();
                estoqueCliente.Cliente.CD_CLIENTE = Convert.ToInt64(CD_CLIENTE);

                DataTable dtEstoqueCliente = new EstoqueData().ObterLista(estoqueCliente);
                if (dtEstoqueCliente.Rows.Count > 0)
                {
                    EstoquePecaEntity estoquePeca = new EstoquePecaEntity
                    {
                        ID_ESTOQUE_PECA = Convert.ToInt64(dtEstoqueCliente.Rows[0]["ID_ESTOQUE"])
                    };

                    if (!string.IsNullOrWhiteSpace(CD_PECA))
                        estoquePecaEntity.peca.CD_PECA = CD_PECA;

                    DataTableReader dataTableReader = new EstoquePecaData().ObterListaCliente(Convert.ToInt64(CD_CLIENTE), CD_PECA, CD_MODELO).CreateDataReader();
                    estoquePecaEntity.estoque = null;

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            estoquePecaEntity.ID_ESTOQUE_PECA = Convert.ToInt64(dataTableReader["ID_ESTOQUE_PECA"]);
                            estoquePecaEntity.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
                            estoquePecaEntity.peca.DS_PECA = dataTableReader["DS_PECA"].ToString();
                            estoquePecaEntity.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
                            estoquePecaEntity.QT_PECA_ATUAL = Convert.ToDecimal(dataTableReader["QT_PECA_ATUAL"]);
                            estoquePecaEntity.QT_PECA_MIN = Convert.ToDecimal(dataTableReader["QT_PECA_MIN"]);

                            if (dataTableReader["DT_ULT_MOVIM"] != DBNull.Value)
                                estoquePecaEntity.DT_ULT_MOVIM = Convert.ToDateTime(dataTableReader["DT_ULT_MOVIM"]);

                            encontrado = true;
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    if (encontrado == false)
                    {
                        PecaEntity pecaEntity = new PecaEntity();
                        pecaEntity.CD_PECA = CD_PECA;
                        dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                estoquePecaEntity.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
                                estoquePecaEntity.peca.DS_PECA = dataTableReader["DS_PECA"].ToString();
                                estoquePecaEntity.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
                            }
                        }

                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { estoquePeca = estoquePecaEntity });
        }


        [HttpGet]
        [Route("ObterEstoquePecaCliente")]
        public HttpResponseMessage ObterEstoquePecaCliente(string CD_CLIENTE, string CD_PECA)
        {
            List<EstoquePecaBase> estoquePecas = new List<EstoquePecaBase>();

            try
            {
                EstoqueEntity estoqueCliente = new EstoqueEntity();
                estoqueCliente.Cliente.CD_CLIENTE = Convert.ToInt64(CD_CLIENTE);

                DataTable dtEstoqueCliente = new EstoqueData().ObterLista(estoqueCliente);

                foreach (DataRow itemEstoque in dtEstoqueCliente.Rows)
                {
                    EstoquePecaEntity estoquePeca = new EstoquePecaEntity
                    {
                        ID_ESTOQUE_PECA = Convert.ToInt64(itemEstoque["ID_ESTOQUE"])
                    };

                    if (!string.IsNullOrWhiteSpace(CD_PECA))
                        estoquePeca.peca.CD_PECA = CD_PECA;

                    DataTableReader dataTableReader = new EstoquePecaData().ObterLista(estoquePeca).CreateDataReader();

                    try
                    {
                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                estoquePecas.Add(new EstoquePecaBase
                                {
                                    ID_ESTOQUE = Convert.ToInt64(dataTableReader["ID_ESTOQUE"]),
                                    ID_ESTOQUE_PECA = Convert.ToInt64(dataTableReader["ID_ESTOQUE_PECA"]),
                                    CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                    DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                    TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                                    QT_PECA_ATUAL = Convert.ToDecimal(dataTableReader["QT_PECA_ATUAL"])
                                });
                            }
                        }
                    }
                    finally
                    {
                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { estoquePeca = estoquePecas });
        }

        [HttpGet]
        [Route("ObterTecnico")]
        public HttpResponseMessage ObterTecnico(string CD_TECNICO, string CD_PECA, string CD_MODELO = null)
        {
            EstoquePecaEntity estoquePecaEntity = new EstoquePecaEntity();
            bool encontrado = false;

            try
            {
                DataTableReader dataTableReader = new EstoquePecaData().ObterListaTecnico(CD_TECNICO, CD_PECA, CD_MODELO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarEstoquePecaEntity(dataTableReader, estoquePecaEntity);
                        encontrado = true;
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (encontrado == false)
                {
                    PecaEntity pecaEntity = new PecaEntity();
                    pecaEntity.CD_PECA = CD_PECA;
                    //dataTableReader = new PecaData().ObterLista(pecaEntity, CD_TECNICO).CreateDataReader();
                    //Teste Proc sem QTD_RECEBIDA_NAO_APROVADA
                    dataTableReader = new PecaData().ObterListaNew(pecaEntity, CD_TECNICO).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            estoquePecaEntity.peca.CD_PECA = dataTableReader["CD_PECA"].ToString().ToUpper();
                            estoquePecaEntity.peca.DS_PECA = dataTableReader["DS_PECA"].ToString();
                            estoquePecaEntity.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
                            estoquePecaEntity.QTD_RECEBIDA_NAO_APROVADA = 0;
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { estoquePeca = estoquePecaEntity });
        }

        [HttpGet]
        [Route("ObterPecasTecnico")]
        public HttpResponseMessage ObterPecasTecnico(string CD_TECNICO, string CD_GRUPO_MODELO)
        {
            try
            {
                List<Models.EstoquePeca> listaPlanoZero = new List<Models.EstoquePeca>();

                EstoquePecaEntity ativoFixoEntity = new EstoquePecaEntity();
                DataTableReader dataTableReader = new EstoquePecaData().ObterListaTecnicoPecas(CD_TECNICO, CD_GRUPO_MODELO).CreateDataReader();
                
                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.EstoquePeca planoZero = new Models.EstoquePeca();
                        

                        if (dataTableReader["QT_PECA_ATUAL"] != DBNull.Value)
                        {
                            planoZero.QT_PECA_ATUAL = dataTableReader["QT_PECA_ATUAL"].ToString();
                        }
                        
                        planoZero.peca = new Models.Peca();
                        if (dataTableReader["CD_PECA"] != DBNull.Value)
                        {
                            planoZero.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
                        }
                        if (dataTableReader["DS_PECA"] != DBNull.Value)
                        {
                            planoZero.peca.DS_PECA = dataTableReader["DS_PECA"].ToString();
                        }

                        listaPlanoZero.Add(planoZero);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { PlanoZero = listaPlanoZero });
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("ObterPecasCliente")]
        public HttpResponseMessage ObterPecasCliente(string CD_CLIENTE, string CD_GRUPO_MODELO)
        {
            try
            {
                List<Models.EstoquePeca> listaPlanoZero = new List<Models.EstoquePeca>();

                EstoquePecaEntity ativoFixoEntity = new EstoquePecaEntity();
                DataTableReader dataTableReader = new EstoquePecaData().ObterListaClientePecas(Convert.ToInt64(CD_CLIENTE), CD_GRUPO_MODELO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.EstoquePeca planoZero = new Models.EstoquePeca();


                        if (dataTableReader["QT_PECA_ATUAL"] != DBNull.Value)
                        {
                            planoZero.QT_PECA_ATUAL = dataTableReader["QT_PECA_ATUAL"].ToString();
                        }

                        planoZero.peca = new Models.Peca();
                        if (dataTableReader["CD_PECA"] != DBNull.Value)
                        {
                            planoZero.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
                        }
                        if (dataTableReader["DS_PECA"] != DBNull.Value)
                        {
                            planoZero.peca.DS_PECA = dataTableReader["DS_PECA"].ToString();
                        }

                        listaPlanoZero.Add(planoZero);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { PlanoZero = listaPlanoZero });
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            
        }

        [HttpGet]
        [Route("ObterQtdEstoqueCliente")]
        public HttpResponseMessage ObterQtdEstoqueCliente(string CD_CLIENTE, string CD_PECA)
        {
            EstoquePecaSinc estoquePecaEntity = new EstoquePecaSinc();
            bool encontrado = false;

            try
            {
                EstoqueData estoque = new EstoqueData();
                EstoquePecaData estoquePeca = new EstoquePecaData();

                EstoqueEntity est = new EstoqueEntity();

                EstoqueSinc estoqueCli = estoque.ObterListaEstoqueSinc().Where(x => x.CD_CLIENTE == CD_CLIENTE).FirstOrDefault();

                if (estoqueCli != null)
                {
                    encontrado = true;
                    estoquePecaEntity = estoquePeca.ObterListaEstoquePecaSincPorID(estoqueCli.ID_ESTOQUE).Where
                    (x => x.CD_PECA.ToUpper() == CD_PECA.ToUpper()).FirstOrDefault();

                }
                else
                {
                    encontrado = false;
                }

                
                
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { estoquePeca = estoquePecaEntity, ExisteEstoque = encontrado });
        }

        [HttpGet]
        [Route("ObterEstoquePecaTecnico")]
        public HttpResponseMessage ObterEstoquePecaTecnico(string CD_TECNICO, string CD_PECA)
        {
            List<EstoquePecaBase> estoquePecas = new List<EstoquePecaBase>();

            try
            {
                DataTableReader dataTableReader = new EstoquePecaData().ObterListaTecnico(CD_TECNICO, CD_PECA).CreateDataReader();

                try
                {
                    if (dataTableReader.HasRows)
                    {
                        while (dataTableReader.Read())
                        {
                            estoquePecas.Add(new EstoquePecaBase
                            {
                                ID_ESTOQUE = Convert.ToInt64(dataTableReader["ID_ESTOQUE"]),
                                ID_ESTOQUE_PECA = Convert.ToInt64(dataTableReader["ID_ESTOQUE_PECA"]),
                                CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                                QT_PECA_ATUAL = Convert.ToDecimal(dataTableReader["QT_PECA_ATUAL"]),
                                QTD_RECEBIDA_NAO_APROVADA = Convert.ToDecimal(dataTableReader["QTD_REC_NAO_APROV"].ToString())
                            });
                        }
                    }
                }
                finally
                {
                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { estoquePeca = estoquePecas });
        }

        [HttpPost]
        [Route("ObterListaTecnico")]
        public HttpResponseMessage ObterListaTecnico(string CD_TECNICO)
        {
            List<EstoquePecaEntity> listaEstoquePecas = new List<EstoquePecaEntity>();
            EstoquePecaEntity estoquePecaEntity = null;

            try
            {
                DataTableReader dataTableReader = new EstoquePecaData().ObterListaTecnico(CD_TECNICO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        estoquePecaEntity = new EstoquePecaEntity();
                        CarregarEstoquePecaEntity(dataTableReader, estoquePecaEntity);
                        listaEstoquePecas.Add(estoquePecaEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { listaEstoquePecas = listaEstoquePecas });
        }

        /// <summary>
        /// Obtém a quantidade atual de peças em estoque
        /// </summary>
        /// <param name="nidEstoque">Id do Estoque</param>
        /// <param name="ccdPeca">Código da peça</param>
        /// <returns>JObject QTD_PECAS do tipo decimal</returns>
        [HttpPost]
        [Route("ObterQuantidadeEmEstoque")]
        public IHttpActionResult ObterQuantidadeEmEstoque(long nidEstoque, string ccdPeca)
        {
            try
            {
                if (nidEstoque == 0)
                {
                    throw new ArgumentException("Código do Estoque inválido ou não informado!");
                }

                if (string.IsNullOrEmpty(ccdPeca))
                {
                    throw new ArgumentException("Código da Peça inválida ou não informada!");
                }

                EstoquePecaEntity filtro = new EstoquePecaEntity();
                filtro.estoque.ID_ESTOQUE = nidEstoque;
                filtro.peca.CD_PECA = ccdPeca;

                decimal quantidadePecasEmEstoque = 0;
                string quantidadePecasEmEstoque_Formatado = string.Empty;
                string unidade = string.Empty;
                string formatadorDecimais = string.Empty;

                EstoquePecaData data = new EstoquePecaData();
                using (DataTable dtEstoque = data.ObterLista(filtro))
                {
                    if (dtEstoque.Rows.Count > 0)
                    {
                        quantidadePecasEmEstoque = Convert.ToDecimal(dtEstoque.Rows[0]["QT_PECA_ATUAL"]);
                        unidade = dtEstoque.Rows[0]["TX_UNIDADE"].ToString();

                        //if (unidade == "MT")
                        //{
                        //    formatadorDecimais = "N3";
                        //}
                        //else
                        //{
                            formatadorDecimais = "N0";
                        //}

                        quantidadePecasEmEstoque_Formatado = Convert.ToDecimal(dtEstoque.Rows[0]["QT_PECA_ATUAL"]).ToString(formatadorDecimais);
                    }
                }

                JObject JO = new JObject();
                JO.Add("QTD_PECAS", JsonConvert.SerializeObject(quantidadePecasEmEstoque, Formatting.None));
                JO.Add("QTD_PECAS_FORMATADO", JsonConvert.SerializeObject(quantidadePecasEmEstoque_Formatado, Formatting.None));
                JO.Add("UNIDADE", JsonConvert.SerializeObject(unidade, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Realiza a consulta do estoque de acordo com o código da peça e os estoques informados 
        /// </summary>
        /// <param name="estoquePecaEntity">
        ///     Peca.CD_PECA = ccdPeca
        ///     Estoque.TP_ESTOQUE = Estoque separado por ','
        /// </param>
        /// <returns>
        /// Lista do tipo EstoquePecaEntity [EstoquePecas]
        /// Lista com Quantidade por Tipo de Estoque [TP_ESTOQUE_QTD_PECA] TP_ESTOQUE_TEC_3M, QT_PECA
        /// </returns>
        [HttpPost]
        [Route("ListarQuantidadePorTipo")]
        public IHttpActionResult ListarQuantidadePorTipo(EstoquePecaEntity estoquePecaEntity)
        {
            try
            {
                JObject JO = new JObject();

                List<Models.EstoquePeca> listaPecas = new List<Models.EstoquePeca>();

                if (estoquePecaEntity.estoque.TP_ESTOQUE_TEC_3M.Contains(","))
                {
                    estoquePecaEntity.estoque.TP_ESTOQUE_TEC_3M = string.Join(",", estoquePecaEntity.estoque.TP_ESTOQUE_TEC_3M);
                }

                EstoquePecaData data = new EstoquePecaData();
                using (DataTable dtEstoque = data.ObterLista(estoquePecaEntity))
                {
                    //Lista de Peças de acordo com Filtros
                    listaPecas = (from p in dtEstoque.Rows.Cast<DataRow>()
                                  select new Models.EstoquePeca()
                                  {
                                      ID_ESTOQUE_PECA = Convert.ToInt64(p["ID_ESTOQUE_PECA"]),
                                      QT_PECA_ATUAL = Convert.ToDecimal(p["QT_PECA_ATUAL"]).ToString("N2"),
                                      QT_PECA_MIN = Convert.ToDecimal(p["QT_PECA_MIN"]).ToString("N2"),

                                      estoque = new Models.Estoque()
                                      {
                                          ID_ESTOQUE = Convert.ToInt64(p["ID_ESTOQUE"]),
                                          TP_ESTOQUE_TEC_3M = p["TP_ESTOQUE_TEC_3M"].ToString()
                                      },

                                      peca = new Models.Peca()
                                      {
                                          CD_PECA = p["CD_PECA"].ToString()
                                      }
                                  }).ToList();
                    //JO.Add("ESTOQUE_PECA", JsonConvert.SerializeObject(listaPecas, Formatting.None));


                    //Quantidade por Tipo de Estoque
                    var listaPecasEstoque = from p in listaPecas
                                            group p by p.estoque.TP_ESTOQUE_TEC_3M into g
                                            select new { TP_ESTOQUE_TEC_3M = g.Key, QT_PECA = Convert.ToDecimal(g.Sum(x => Convert.ToDecimal(x.QT_PECA_ATUAL))).ToString("N0") };
                    JO.Add("PECAS_ESTOQUE", JsonConvert.SerializeObject(listaPecasEstoque, Formatting.None));
                }

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ObterListaPecasPorEstoque")]
        public IHttpActionResult ObterListaPecasPorEstoque(long nidEstoque, long nidUsuario)
        {
            IList<PecaEntity> listaPecas = null;
            try
            {
                EstoquePecaData data = new EstoquePecaData();
                using (DataTable dtEstoque = data.ObterListaPorEstoque(nidEstoque, string.Empty, nidUsuario))
                {
                    listaPecas = (from p in dtEstoque.Rows.Cast<DataRow>()
                                  select new PecaEntity()
                                  {
                                      CD_PECA = p["CD_PECA"].ToString(),
                                      DS_PECA = p["DS_PECA"].ToString(),
                                      TX_UNIDADE = p["TX_UNIDADE"].ToString(),
                                      VL_PECA = Convert.ToDecimal(p["VL_PECA"]),
                                      TP_PECA = p["TP_PECA"].ToString(),
                                      FL_ATIVO_PECA = p["FL_ATIVO_PECA"].ToString()
                                  }).ToList();
                }

                JObject JO = new JObject();
                JO.Add("PECA", JsonConvert.SerializeObject(listaPecas, Formatting.None));
                return Ok(JO);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        protected void CarregarEstoquePecaEntity(DataTableReader dataTableReader, EstoquePecaEntity estoquePecaEntity)
        {
            estoquePecaEntity.estoque.ID_ESTOQUE = Convert.ToInt64(dataTableReader["ID_ESTOQUE"]);
            estoquePecaEntity.estoque.CD_ESTOQUE = dataTableReader["CD_ESTOQUE"].ToString();
            estoquePecaEntity.estoque.DS_ESTOQUE = dataTableReader["DS_ESTOQUE"].ToString();
            estoquePecaEntity.estoque.ID_USU_RESPONSAVEL = Convert.ToInt64(dataTableReader["ID_USU_RESPONSAVEL"]);
            estoquePecaEntity.estoque.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]);
            estoquePecaEntity.estoque.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            estoquePecaEntity.estoque.TP_ESTOQUE_TEC_3M = dataTableReader["TP_ESTOQUE_TEC_3M"].ToString();
            estoquePecaEntity.estoque.FL_ATIVO = dataTableReader["FL_ATIVO"].ToString();

            estoquePecaEntity.ID_ESTOQUE_PECA = Convert.ToInt64(dataTableReader["ID_ESTOQUE_PECA"]);
            estoquePecaEntity.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
            estoquePecaEntity.peca.DS_PECA = dataTableReader["DS_PECA"].ToString();
            estoquePecaEntity.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
            estoquePecaEntity.QT_PECA_ATUAL = Convert.ToDecimal(dataTableReader["QT_PECA_ATUAL"]);
            estoquePecaEntity.QT_PECA_MIN = Convert.ToDecimal(dataTableReader["QT_PECA_MIN"]);
            estoquePecaEntity.QTD_RECEBIDA_NAO_APROVADA = Convert.ToDecimal(dataTableReader["QTD_REC_NAO_APROV"].ToString());
            if (dataTableReader["DT_ULT_MOVIM"] != DBNull.Value)
            {
                estoquePecaEntity.DT_ULT_MOVIM = Convert.ToDateTime(dataTableReader["DT_ULT_MOVIM"]);
            }

            if (dataTableReader["DT_MOVIMENTACAO_AJUSTE_SAIDA"] != DBNull.Value)
            {
                estoquePecaEntity.DT_MOVIMENTACAO_AJUSTE_SAIDA = Convert.ToDateTime(dataTableReader["DT_MOVIMENTACAO_AJUSTE_SAIDA"]).ToString("dd/MM/yyyy");
            }
        }

        [HttpGet]
        [Route("ObterListaEstoquePecaSinc")]
        public IHttpActionResult ObterListaEstoquePecaSinc(Int64 idUsuario)
        {
            IList<EstoquePecaSinc> listaEstoquePeca = new List<EstoquePecaSinc>();
            try
            {
                EstoquePecaData estoquePecaData = new EstoquePecaData();
                listaEstoquePeca = estoquePecaData.ObterListaEstoquePecaSinc(idUsuario);

                JObject JO = new JObject
                {
                    { "ESTOQUE_PECA", JArray.FromObject(listaEstoquePeca) }
                };
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


    }
}