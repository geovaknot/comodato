using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/MovimentacaoPecaAPI")]
    [Authorize]
    public class MovimPecasAPIController : BaseAPIController
    {
        [AcceptVerbs("Get", "Post")]
        [Route("ObterLista")]
        public HttpResponseMessage ObterListaMovimentacao(EstoqueMoviEntity filtroEstoqueMovi)
        {
            EstoqueMoviData estoqueData = new EstoqueMoviData();
            using (DataTable result = estoqueData.ObterListaMovimento(null))
            {
                var listaEstoque = from dr in result.Rows.Cast<DataRow>()
                                   select
                                   new EstoqueMoviEntity();

                return Request.CreateResponse(HttpStatusCode.OK, listaEstoque);
            }
        }

        [HttpPost]
        [Route("MovimentarEstoque")]
        public IHttpActionResult MovimentarEstoque(EstoqueMoviEntity estoqueMovi)
        {

            try
            {
                #region Controle Entrada/Saída
                bool gerarEntrada = false;
                bool gerarSaida = false;

                switch (estoqueMovi.TP_MOVIMENTACAO.CD_TP_MOVIMENTACAO)
                {
                    case "1":
                    case "2":
                        gerarEntrada = gerarSaida = true;
                        break;
                    case "3":
                        gerarEntrada = true;
                        gerarSaida = false;
                        break;
                    case "4":
                        gerarEntrada = false;
                        gerarSaida = true;
                        break;
                }
                #endregion


                EstoquePecaData estoquePecaData = new EstoquePecaData();
                EstoqueMoviData estoqueMoviData = new EstoqueMoviData();

                #region Lista peças à movimentar
                List<EstoqueMoviEntity> estoqueMoviEntities = new List<EstoqueMoviEntity>();

                if (estoqueMovi.TP_MOVIMENTACAO.CD_TP_MOVIMENTACAO == "1")
                {
                    DataTable dtEstoquePeca = estoquePecaData.ObterListaPorEstoque(estoqueMovi.ESTOQUE_ORIGEM.ID_ESTOQUE, string.Empty, 0);
                    estoqueMoviEntities = (from ep in dtEstoquePeca.Rows.Cast<DataRow>()
                                           select new EstoqueMoviEntity()
                                           {
                                               Peca = new PecaEntity() { CD_PECA = ep["CD_PECA"].ToString() },
                                               QT_PECA = Convert.ToDecimal(ep["QT_PECA_ATUAL"]),
                                               TP_MOVIMENTACAO = estoqueMovi.TP_MOVIMENTACAO,
                                               ESTOQUE_ORIGEM = estoqueMovi.ESTOQUE_ORIGEM,
                                               ESTOQUE_DESTINO = estoqueMovi.ESTOQUE_DESTINO,
                                               DT_MOVIMENTACAO = DateTime.Now,
                                               USU_MOVI = new UsuarioEntity() { nidUsuario = estoqueMovi.USU_MOVI.nidUsuario },
                                               nidUsuarioAtualizacao = estoqueMovi.nidUsuarioAtualizacao
                                           }).ToList();
                }
                else
                {
                    estoqueMovi.DT_MOVIMENTACAO = DateTime.Now;
                    estoqueMoviEntities.Add(estoqueMovi);
                }
                #endregion
                using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                {
                    sqlConnection.Open();
                    using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
                    {
                        try
                        {
                            foreach (EstoqueMoviEntity movimentacao in estoqueMoviEntities)
                            {
                                EstoquePecaEntity pecaSaida = new EstoquePecaEntity();

                                if (gerarSaida)
                                {
                                    EstoqueMoviEntity saida = (EstoqueMoviEntity)movimentacao.Clone();
                                    saida.TP_ENTRADA_SAIDA = "S";
                                    saida.QT_PECA = saida.QT_PECA * (-1);
                                    estoqueMoviData.Inserir(ref saida, sqlTransaction);

                                    using (DataTable dtPecaEstoque = estoquePecaData.ObterListaPorEstoque(saida.ESTOQUE_ORIGEM.ID_ESTOQUE, saida.Peca.CD_PECA, saida.USU_MOVI.nidUsuario, sqlTransaction))
                                    {
                                        if (dtPecaEstoque.Rows.Count == 0)
                                            throw new Exception("Não há estoque origem suficiente");

                                        pecaSaida.ID_ESTOQUE_PECA = Convert.ToInt64(dtPecaEstoque.Rows[0]["ID_ESTOQUE_PECA"]);
                                        pecaSaida.peca = saida.Peca;
                                        pecaSaida.QT_PECA_ATUAL = Convert.ToDecimal(dtPecaEstoque.Rows[0]["QT_PECA_ATUAL"]) + saida.QT_PECA;
                                        pecaSaida.QT_PECA_MIN = Convert.ToDecimal(dtPecaEstoque.Rows[0]["QT_PECA_MIN"]);
                                        pecaSaida.estoque = saida.ESTOQUE_ORIGEM;
                                        pecaSaida.DT_ULT_MOVIM = DateTime.Now;

                                        if (pecaSaida.QT_PECA_ATUAL < 0)
                                            throw new Exception("Não há estoque origem suficiente");

                                        estoquePecaData.Alterar(pecaSaida, sqlTransaction);
                                    }
                                }

                                if (gerarEntrada)
                                {
                                    EstoqueMoviEntity entrada = (EstoqueMoviEntity)movimentacao.Clone();
                                    entrada.TP_ENTRADA_SAIDA = "E";
                                    estoqueMoviData.Inserir(ref entrada, sqlTransaction);

                                    using (DataTable dtPecaEstoque = estoquePecaData.ObterListaPorEstoque(entrada.ESTOQUE_DESTINO.ID_ESTOQUE, entrada.Peca.CD_PECA, entrada.USU_MOVI.nidUsuario, sqlTransaction))
                                    {
                                        EstoquePecaEntity pecaEntrada = new EstoquePecaEntity();
                                        pecaEntrada.peca = entrada.Peca;
                                        pecaEntrada.estoque = entrada.ESTOQUE_DESTINO;
                                        pecaEntrada.DT_ULT_MOVIM = DateTime.Now;

                                        if (dtPecaEstoque.Rows.Count == 0)
                                        {
                                            pecaEntrada.QT_PECA_ATUAL = entrada.QT_PECA;
                                            pecaEntrada.QT_PECA_MIN = 0;

                                            estoquePecaData.Inserir(ref pecaEntrada, sqlTransaction);
                                        }
                                        else
                                        {
                                            pecaEntrada.ID_ESTOQUE_PECA = Convert.ToInt64(dtPecaEstoque.Rows[0]["ID_ESTOQUE_PECA"]);
                                            pecaEntrada.QT_PECA_ATUAL = Convert.ToDecimal(dtPecaEstoque.Rows[0]["QT_PECA_ATUAL"]) + entrada.QT_PECA;
                                            pecaEntrada.QT_PECA_MIN = Convert.ToDecimal(dtPecaEstoque.Rows[0]["QT_PECA_MIN"]);

                                            estoquePecaData.Alterar(pecaEntrada, sqlTransaction);
                                        }
                                    }
                                }

                            }
                            sqlTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            sqlTransaction.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            if (sqlConnection.State != ConnectionState.Closed)
                                sqlConnection.Close();
                        }
                    }
                }

                JObject JO = new JObject();
                JO.Add("ESTOQUEMOVI", JsonConvert.SerializeObject(estoqueMovi, Formatting.None));
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