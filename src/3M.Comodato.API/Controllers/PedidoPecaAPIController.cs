using _3M.Comodato.Data;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using Newtonsoft.Json;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/PedidoPecaAPI")]
    [Authorize]
    public class PedidoPecaAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("ObterListaPedidoPecaSinc")]
        public IHttpActionResult ObterListaPedidoPecaSinc(Int64 idUsuario)
        {
            IList<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
            IList<PedidoPecaLogSinc> listaPedidoPecaLog = new List<PedidoPecaLogSinc>();

            try
            {
                PedidoPecaData pedidoPecaData = new PedidoPecaData();
                PedidoPecaLogData pedidoPecaLogData = new PedidoPecaLogData();
               
                listaPedidoPeca = pedidoPecaData.ObterListaPedidoPecaSinc(idUsuario);
                listaPedidoPecaLog = pedidoPecaLogData.ObterListaPedidoPecaLogPorId(idUsuario);
                
                JObject JO = new JObject
                {
                    { "PEDIDO_PECA", JArray.FromObject(listaPedidoPeca) },
                    { "PEDIDO_PECA_LOG", JArray.FromObject(listaPedidoPecaLog) }
                };

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Incluir")]
        public HttpResponseMessage Incluir(PedidoPecaEntity pedidoPecaEntity)
        {
            try
            {
                if (pedidoPecaEntity.VL_PECA == null)
                {
                    PecaEntity pecaEntity = new PecaEntity
                    {
                        CD_PECA = pedidoPecaEntity.peca.CD_PECA
                    };

                    DataTableReader dataTableReader = new PecaData().ObterListaNew(pecaEntity).CreateDataReader();

                    try
                    {
                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                if (dataTableReader["VL_PECA"] != DBNull.Value)
                                {
                                    pedidoPecaEntity.VL_PECA = Convert.ToDecimal(dataTableReader["VL_PECA"]);
                                }
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

                if (pedidoPecaEntity.TIPO_PECA == 0)
                    pedidoPecaEntity.TIPO_PECA = Convert.ToByte(ControlesUtility.Enumeradores.TipoPeca.PecaComItem);

                new PedidoPecaData().Inserir(ref pedidoPecaEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidoPecaEntity.ID_ITEM_PEDIDO, pedidoPecaEntity.TOKEN });
        }

        [HttpPost]
        [Route("Alterar")]
        public HttpResponseMessage Alterar(PedidoPecaEntity pedidoPecaEntity)
        {
            try
            {
                if (pedidoPecaEntity.QTD_ULTIMO_RECEBIMENTO != null && pedidoPecaEntity.QTD_ULTIMO_RECEBIMENTO != 0)
                {
                    if(pedidoPecaEntity.QTD_RECEBIDA == null || pedidoPecaEntity.QTD_RECEBIDA == 0) {
                        pedidoPecaEntity.QTD_RECEBIDA = pedidoPecaEntity.QTD_ULTIMO_RECEBIMENTO;
                    }
                        
                }
                if (pedidoPecaEntity.pedido.TP_Especial == "Especial")
                {
                    if (pedidoPecaEntity.VL_PECA == null || pedidoPecaEntity.VL_PECA == 0)
                    {
                        throw new Exception("Para pedido de peça do tipo Especial o valor da peça deve ser informado!");
                    }
                }
                new PedidoPecaData().Alterar(pedidoPecaEntity);

                //SL00035389
                if (pedidoPecaEntity.QTD_APROVADA == 0)
                {
                    string Mensagem = string.Empty;
                    PedidoPecaData pedidoPecaData = new PedidoPecaData();

                    PedidoEntity pedidoEntity = new PedidoEntity();
                    pedidoEntity.ID_PEDIDO = pedidoPecaEntity.pedido.ID_PEDIDO;
                    DataTableReader dataTableReader = new PedidoData().ObterLista(pedidoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {

                            CarregarPedidoEntity(dataTableReader, pedidoEntity);
                            
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    
                    if (pedidoEntity.TP_TIPO_PEDIDO == "C")
                    {
                        pedidoPecaData.Atualizar_Estoque_Tabela_Peca_CLIENTE(pedidoPecaEntity.ID_ITEM_PEDIDO, pedidoPecaEntity.peca.CD_PECA, pedidoEntity.cliente.CD_CLIENTE);
                    }
                    else
                    {
                        pedidoPecaData.Atualizar_Estoque_Tabela_Peca(pedidoPecaEntity.ID_ITEM_PEDIDO, pedidoPecaEntity.peca.CD_PECA, pedidoEntity.tecnico.CD_TECNICO);
                    }

                    IList<PedidoPecaSinc> pecasPedido = pedidoPecaData.ObterListaPedidoPecaPedido(pedidoPecaEntity.pedido.ID_PEDIDO);
                    
                    if (pecasPedido != null)
                    {
                        if (pecasPedido.Count == 1 && pedidoPecaEntity.ST_STATUS_ITEM == "4")
                        {
                            new PedidoPecaData().AtualizaStatusPedido(pedidoPecaEntity.pedido.ID_PEDIDO);

                            PedidoEntity pedidoEntityDepoisAtualizacao = new PedidoEntity();
                            pedidoEntityDepoisAtualizacao = ObterPedidoEntity(pedidoPecaEntity.pedido.ID_PEDIDO);

                            if (pedidoEntityDepoisAtualizacao.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Cancelado))
                            {
                                EnviarEmailPedido(pedidoEntityDepoisAtualizacao, "CANCELADO", ref Mensagem);

                            }
                        }
                        else if(pedidoPecaEntity.ST_STATUS_ITEM == "4")
                        {
                            bool AtualizaPedido = false;
                            if (pecasPedido != null)
                            {
                                char ST_PECA = '0';
                                char ST_PECA_AUX = '0';
                                int StatusIguais = 0;
                                int Ap = 0;
                                int C = 0;
                                int SomaApCancelado = 0;
                                int somabcpsCancelado = 0;
                                int bpcs = 0;
                                foreach (var peca in pecasPedido)
                                {
                                    if (ST_PECA == '0' && ST_PECA_AUX == '0')
                                    {
                                        ST_PECA = peca.ST_STATUS_ITEM;

                                    }
                                    if (ST_PECA != '0' && ST_PECA_AUX == '0')
                                    {
                                        ST_PECA_AUX = ST_PECA;
                                    }
                                    if (ST_PECA != '0' && ST_PECA_AUX != '0')
                                    {
                                        if (peca.ST_STATUS_ITEM == ST_PECA_AUX)
                                        {
                                            StatusIguais += 1;
                                        }
                                        if (peca.ST_STATUS_ITEM == '4')
                                        {
                                            C += 1;
                                        }
                                        if (peca.ST_STATUS_ITEM == '3')
                                        {
                                            Ap += 1;
                                        }
                                        if (peca.ST_STATUS_ITEM == '8')
                                        {
                                            bpcs += 1;
                                        }
                                    }



                                }
                                SomaApCancelado = Ap + C;
                                somabcpsCancelado = bpcs + C;

                                if (StatusIguais == pecasPedido.Count || SomaApCancelado == pecasPedido.Count)
                                {
                                    AtualizaPedido = true;
                                }
                                else if (StatusIguais == pecasPedido.Count || somabcpsCancelado == pecasPedido.Count)
                                {
                                    AtualizaPedido = true;
                                }
                                else
                                {
                                    AtualizaPedido = false;
                                }
                            }
                            if (AtualizaPedido == true)
                            {
                                //Atualiza Status do Pedido
                                pedidoPecaData.AtualizaStatusPedido(pedidoPecaEntity.pedido.ID_PEDIDO);
                            }

                            
                        }
                    }

                    //new PedidoPecaData().AtualizaStatusPedido(pedidoPecaEntity.pedido.ID_PEDIDO);

                        //SL00035666
                        //Envia email pedidos Cancelado e Cancelado Parcial
                        //PedidoEntity pedidoEntityDepoisAtualizacao = new PedidoEntity();
                        //pedidoEntityDepoisAtualizacao = ObterPedidoEntity(pedidoPecaEntity.pedido.ID_PEDIDO);

                        //if (pedidoEntityDepoisAtualizacao.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Cancelado))
                        //{
                        //    EnviarEmailPedido(pedidoEntityDepoisAtualizacao, "CANCELADO", ref Mensagem);

                        //}

                        //if (pedidoEntityDepoisAtualizacao.statusPedido.ID_STATUS_PEDIDO == Convert.ToInt64(ControlesUtility.Enumeradores.StatusPedido.Aprovado))
                        //{
                        //    EnviarEmailPedido(pedidoEntityDepoisAtualizacao, "APROVADO PARCIALMENTE", ref Mensagem);

                        //}

                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { QTD_APROVADA = pedidoPecaEntity.QTD_APROVADA });
        }

        [HttpPost]
        [Route("Excluir")]
        public HttpResponseMessage Excluir(PedidoPecaEntity pedidoPecaEntity)
        {
            try
            {
                new PedidoPecaData().Excluir(pedidoPecaEntity);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(Int64 ID_ITEM_PEDIDO)
        {
            PedidoPecaEntity pedidoPecaEntity = new PedidoPecaEntity();
            Models.PedidoPeca pedidoPecaModel = new Models.PedidoPeca();
            PedidoPecaEntity pedidoPeca = new PedidoPecaEntity();

            try
            {
                pedidoPecaEntity.ID_ITEM_PEDIDO = ID_ITEM_PEDIDO;
                DataTableReader dataTableReader = new PedidoPecaData().ObterLista(pedidoPecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarPedidoPecaEntity(dataTableReader, pedidoPecaEntity);
                        CarregarPedidoPecaModel(dataTableReader, pedidoPecaModel);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidoPeca = pedidoPecaEntity, pedidoPecaModel = pedidoPecaModel });
        }

        public HttpResponseMessage ObterPedidoBPCS(Int64 ID_ITEM_PEDIDO)
        {
            PedidoPecaEntity pedidoPecaEntity = new PedidoPecaEntity();
            Models.PedidoPeca pedidoPecaModel = new Models.PedidoPeca();
            PedidoPecaEntity pedidoPeca = new PedidoPecaEntity();

            try
            {
                pedidoPecaEntity.ID_ITEM_PEDIDO = ID_ITEM_PEDIDO;
                DataTableReader dataTableReader = new PedidoPecaData().ObterLista(pedidoPecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarPedidoPecaEntity(dataTableReader, pedidoPecaEntity);
                        CarregarPedidoPecaModel(dataTableReader, pedidoPecaModel);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidoPeca = pedidoPecaEntity, pedidoPecaModel = pedidoPecaModel });
        }

        

        [HttpGet]
        [Route("ObterPedidoPecaLog")]
        public HttpResponseMessage ObterPedidoPecaLog(Int64 ID_ITEM_PEDIDO)
        {
            List <PedidoPecaLogSinc> pedidoPecaLogEntity = new List<PedidoPecaLogSinc>();
            List<PedidoPecaLogSinc> pedidoPecaLogModel = new List<PedidoPecaLogSinc>();

            PedidoPecaLogData pedPecaLog = new PedidoPecaLogData();

            try
            {
                
                List<PedidoPecaLogSinc> listaPedidoPecaLog = pedPecaLog.ObterLista(ID_ITEM_PEDIDO).ToList();

                if (listaPedidoPecaLog != null)
                {
                    foreach (var item in listaPedidoPecaLog)
                    {
                        pedidoPecaLogEntity.Add(item);
                        pedidoPecaLogModel.Add(item);
                    }
                    
                }

                
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidoPecaLog = pedidoPecaLogEntity, pedidoPecaLogModel = pedidoPecaLogModel });
        }

        [HttpGet]
        [Route("ValidaPecaDuplicidadePedido")]
        public HttpResponseMessage ValidaPecaDuplicidadePedido(Int64 ID_PEDIDO, Int64 ID_ITEM_PEDIDO, string CD_PECA)
        {
            bool retorno = false;
            try
            {
                DataTableReader dataTableReader = new PedidoPecaData().BuscaPecaDuplicidadePedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        retorno = true;
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

            return Request.CreateResponse(HttpStatusCode.OK, new { retorno = retorno });
        }

        [HttpGet]
        [Route("GetImage")]
        [AllowAnonymous]
        public HttpResponseMessage GetImage(string ImgFileName)
        {
           
            var context = HttpContext.Current;
            string diretorio = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload) + ControlesUtility.Constantes.PastaFotosPecasSincronismo;

            string filePath = context.Server.MapPath("~\\" + diretorio +  ImgFileName);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            if (!System.IO.File.Exists(filePath))
            {
                response.Content = new StringContent("<!DOCTYPE html><html><head><meta charset=\"utf - 8\" /></head><body>" +
                    "<p>App Mobile n&atilde;o fez upload da imagem " + ImgFileName  + "</p>" +
                    "<p>A imagem n&atilde;o existe na pasta " + diretorio + " do servidor</p>" +
                    "</body></html>");
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                return response;

            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    try
                    {
                        fileStream.CopyTo(memoryStream);
                        Bitmap image = new Bitmap(1, 1);
                        image.Save(memoryStream, ImageFormat.Jpeg);

                        byte[] byteImage = memoryStream.ToArray();

                        MemoryStream ms = new MemoryStream(byteImage);
                        response.Content = new StreamContent(ms);
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                        return response;

                    }
                    catch (Exception)
                    {
                        response.Content = new StringContent("<!DOCTYPE html><html><head><meta charset=\"utf - 8\" /></head><body>" +
                            "<p>App Mobile fez upload inv&aacute;lido da imagem " + ImgFileName + "</p>" +
                            "<p>A imagem na pasta " + diretorio + " do servidor n&atilde;o &eacute; um jpeg v&aacute;lido ou est&aacute; corrompido.</p>" +
                            "</body></html>");
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                        return response;


                    }

                }
            }
        }


        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(PedidoPecaEntity pedidoPecaEntity)
        {
            List<PedidoPecaEntity> pedidoPecas = new List<PedidoPecaEntity>();

            try
            {
                if (pedidoPecaEntity == null)
                {
                    pedidoPecaEntity = new PedidoPecaEntity();
                }

                DataTableReader dataTableReader = new PedidoPecaData().ObterLista(pedidoPecaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        pedidoPecaEntity = new PedidoPecaEntity();

                        CarregarPedidoPecaEntity(dataTableReader, pedidoPecaEntity);

                        pedidoPecas.Add(pedidoPecaEntity);
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { pedidoPecas = pedidoPecas });
        }

        protected void CarregarPedidoPecaEntity(DataTableReader dataTableReader, PedidoPecaEntity pedidoPecaEntity)
        {
            pedidoPecaEntity.ID_ITEM_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_ITEM_PEDIDO"]);
            pedidoPecaEntity.pedido.ID_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_PEDIDO"]);
            pedidoPecaEntity.peca.CD_PECA = dataTableReader["CD_PECA"].ToString();
            pedidoPecaEntity.peca.DS_PECA = dataTableReader["DS_PECA"].ToString().ToUpper();
            pedidoPecaEntity.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
            if (dataTableReader["QTD_SOLICITADA"] != DBNull.Value)
            {
                pedidoPecaEntity.QTD_SOLICITADA = Convert.ToDecimal(dataTableReader["QTD_SOLICITADA"]);
            }

            if (dataTableReader["QTD_APROVADA"] != DBNull.Value)
            {
                pedidoPecaEntity.QTD_APROVADA = Convert.ToDecimal(dataTableReader["QTD_APROVADA"]);
            }

            if (dataTableReader["QTD_RECEBIDA"] != DBNull.Value)
            {
                pedidoPecaEntity.QTD_RECEBIDA = Convert.ToDecimal(dataTableReader["QTD_RECEBIDA"]);
            }

            pedidoPecaEntity.TX_APROVADO = dataTableReader["TX_APROVADO"].ToString();
            pedidoPecaEntity.NR_DOC_ORI = Convert.ToInt64("0" + dataTableReader["NR_DOC_ORI"]);
            pedidoPecaEntity.ST_STATUS_ITEM = dataTableReader["ST_STATUS_ITEM"].ToString();
            pedidoPecaEntity.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString();
            pedidoPecaEntity.DS_DIR_FOTO = dataTableReader["DS_DIR_FOTO"].ToString();
            pedidoPecaEntity.TOKEN = Convert.ToInt64(dataTableReader["TOKEN"]);
            pedidoPecaEntity.TIPO_PECA = Convert.ToByte(dataTableReader["TIPO_PECA"]);
            pedidoPecaEntity.DESCRICAO_PECA = dataTableReader["DESCRICAO_PECA"].ToString();

            if (dataTableReader["ID_ESTOQUE_DEBITO"] != DBNull.Value)
            {
                pedidoPecaEntity.estoque3M1.ID_ESTOQUE = Convert.ToInt64("0" + dataTableReader["ID_ESTOQUE_DEBITO"]);
            }

            if (dataTableReader["ID_ESTOQUE_DEBITO_3M2"] != DBNull.Value)
            {
                pedidoPecaEntity.estoque3M2.ID_ESTOQUE = Convert.ToInt64("0" + dataTableReader["ID_ESTOQUE_DEBITO_3M2"]);
            }


            if (dataTableReader["QTD_APROVADA_3M1"] != DBNull.Value)
            {
                pedidoPecaEntity.QTD_APROVADA_3M1 = Convert.ToDecimal(dataTableReader["QTD_APROVADA_3M1"]);
            }
            if (dataTableReader["QTD_APROVADA_3M2"] != DBNull.Value)
            {
                pedidoPecaEntity.QTD_APROVADA_3M2 = Convert.ToDecimal(dataTableReader["QTD_APROVADA_3M2"]);
            }

            if (dataTableReader["ID_LOTE_APROVACAO"] != DBNull.Value)
            {
                pedidoPecaEntity.ID_LOTE_APROVACAO = Convert.ToInt64(dataTableReader["ID_LOTE_APROVACAO"]);
            }

            if (dataTableReader["NR_LINHA"] != DBNull.Value)
            {
                pedidoPecaEntity.NR_LINHA = Convert.ToInt64(dataTableReader["NR_LINHA"]);
            }
        }

        protected void CarregarPedidoPecaModel(DataTableReader dataTableReader, Models.PedidoPeca pedidoPeca)
        {
            pedidoPeca.ID_ITEM_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_ITEM_PEDIDO"]);
            pedidoPeca.pedido.ID_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_PEDIDO"]);
            pedidoPeca.peca.CD_PECA = dataTableReader["CD_PECA"].ToString().ToUpper();
            pedidoPeca.peca.DS_PECA = dataTableReader["DS_PECA"].ToString();
            pedidoPeca.peca.TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString();
            
            string formatadorDecimais = "N0";

            if (dataTableReader["QTD_SOLICITADA"] != DBNull.Value)
            {
                pedidoPeca.QTD_SOLICITADA = Convert.ToDecimal(dataTableReader["QTD_SOLICITADA"]).ToString(formatadorDecimais);
            }

            if (dataTableReader["QTD_APROVADA"] != DBNull.Value)
            {
                pedidoPeca.QTD_APROVADA = Convert.ToDecimal(dataTableReader["QTD_APROVADA"]).ToString(formatadorDecimais);
            }

            if (dataTableReader["QTD_RECEBIDA"] != DBNull.Value)
            {
                pedidoPeca.QTD_RECEBIDA = Convert.ToDecimal(dataTableReader["QTD_RECEBIDA"]).ToString(formatadorDecimais);
            }

            pedidoPeca.TX_APROVADO = dataTableReader["TX_APROVADO"].ToString();
            pedidoPeca.NR_DOC_ORI = Convert.ToInt64("0" + dataTableReader["NR_DOC_ORI"]);
            pedidoPeca.ST_STATUS_ITEM = dataTableReader["ST_STATUS_ITEM"].ToString();
            pedidoPeca.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString();
            pedidoPeca.DS_DIR_FOTO = dataTableReader["DS_DIR_FOTO"].ToString();
            pedidoPeca.TOKEN = Convert.ToInt64(dataTableReader["TOKEN"]);
            pedidoPeca.TIPO_PECA = Convert.ToByte(dataTableReader["TIPO_PECA"]);
            pedidoPeca.DESCRICAO_PECA = dataTableReader["DESCRICAO_PECA"].ToString();

            if (dataTableReader["QT_PECA_ATUAL"] != DBNull.Value)
            {
                pedidoPeca.QT_PECA_ATUAL = Convert.ToDecimal(dataTableReader["QT_PECA_ATUAL"]).ToString(formatadorDecimais);
            }

            if (dataTableReader["QT_SUGERIDA_PZ"] != DBNull.Value)
            {
                pedidoPeca.QT_SUGERIDA_PZ = Convert.ToDecimal(dataTableReader["QT_SUGERIDA_PZ"]).ToString(formatadorDecimais);
            }
            else
            {
                pedidoPeca.QT_SUGERIDA_PZ = "0";
            }

            if (dataTableReader["ID_ESTOQUE_DEBITO"] != DBNull.Value)
            {
                pedidoPeca.estoque3M1.ID_ESTOQUE = Convert.ToInt64("0" + dataTableReader["ID_ESTOQUE_DEBITO"]);
            }

            if (dataTableReader["ID_ESTOQUE_DEBITO_3M2"] != DBNull.Value)
            {
                pedidoPeca.estoque3M2.ID_ESTOQUE = Convert.ToInt64("0" + dataTableReader["ID_ESTOQUE_DEBITO_3M2"]);
            }

            pedidoPeca.QTD_APROVADA_3M1 = "";
            if (dataTableReader["QTD_APROVADA_3M1"] != DBNull.Value)
            {
                pedidoPeca.QTD_APROVADA_3M1 = Convert.ToDecimal(dataTableReader["QTD_APROVADA_3M1"]).ToString(formatadorDecimais);
            }

            pedidoPeca.QTD_APROVADA_3M2 = "";
            if (dataTableReader["QTD_APROVADA_3M2"] != DBNull.Value)
            {
                pedidoPeca.QTD_APROVADA_3M2 = Convert.ToDecimal(dataTableReader["QTD_APROVADA_3M2"]).ToString(formatadorDecimais);
            }

            pedidoPeca.ID_LOTE_APROVACAO = "";
            if (dataTableReader["ID_LOTE_APROVACAO"] != DBNull.Value)
            {
                pedidoPeca.ID_LOTE_APROVACAO = Convert.ToInt64(dataTableReader["ID_LOTE_APROVACAO"]).ToString();
            }

            var estCDCLI = dataTableReader["QTD_ESTOQUE_CLIENTE"].ToString();

            if (dataTableReader["QTD_ESTOQUE_CLIENTE"] != DBNull.Value)
            {
                if(dataTableReader["QTD_ESTOQUE_CLIENTE"] == null || dataTableReader["QTD_ESTOQUE_CLIENTE"] == "")
                    pedidoPeca.QTD_ESTOQUE_CLIENTE = Convert.ToInt64("0");
                else
                    pedidoPeca.QTD_ESTOQUE_CLIENTE = Convert.ToInt64(dataTableReader["QTD_ESTOQUE_CLIENTE"]);
            }
            else
            {
                pedidoPeca.QTD_ESTOQUE_CLIENTE = 0;
            }
        }

        

        public PedidoEntity ObterPedidoEntity(Int64 ID_PEDIDO)
        {
            PedidoEntity pedidoEntity = new PedidoEntity();
            Models.ListaSolicitacaoPecas listaSolicitacaoPecas = new Models.ListaSolicitacaoPecas();

            try
            {
                pedidoEntity.ID_PEDIDO = ID_PEDIDO;
                DataTableReader dataTableReader = new PedidoData().ObterLista(pedidoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarPedidoEntity(dataTableReader, pedidoEntity);
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
            }

            return pedidoEntity;
        }


        protected void CarregarPedidoEntity(DataTableReader dataTableReader, PedidoEntity pedidoEntity)
        {
            pedidoEntity.ID_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_PEDIDO"]);
            pedidoEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            pedidoEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
            pedidoEntity.tecnico.empresa.CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]);
            pedidoEntity.tecnico.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
            pedidoEntity.NR_DOCUMENTO = Convert.ToInt64("0" + dataTableReader["NR_DOCUMENTO"]);
            pedidoEntity.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]);
            if (dataTableReader["DT_ENVIO"] != DBNull.Value)
            {
                pedidoEntity.DT_ENVIO = Convert.ToDateTime(dataTableReader["DT_ENVIO"]);
            }
            if (dataTableReader["DT_Aprovacao"] != DBNull.Value)
            {
                pedidoEntity.DT_Aprovacao = Convert.ToDateTime(dataTableReader["DT_Aprovacao"]);
            }
            if (dataTableReader["nidUsuario"] != DBNull.Value)
            {
                Int64 IdUser = Convert.ToInt64(dataTableReader["nidUsuario"].ToString());
                if (IdUser == 0 || IdUser == null)
                    pedidoEntity.UsuarioSolicitante = null;
                else
                {
                    var user = new UsuarioData().ObterListaUsuarioSinc(IdUser).FirstOrDefault(x => x.nidUsuario == IdUser);
                    pedidoEntity.UsuarioSolicitante = user.cnmNome;
                }
            }
            if (dataTableReader["nidUsuarioAprovador"] != DBNull.Value)
            {
                Int64 IdUser = Convert.ToInt64(dataTableReader["nidUsuarioAprovador"].ToString());
                if (IdUser == 0 || IdUser == null)
                    pedidoEntity.UsuarioAprovador = null;
                else
                {
                    var user = new UsuarioData().ObterListaUsuarioSinc(IdUser).FirstOrDefault(x => x.nidUsuario == IdUser);
                    pedidoEntity.UsuarioAprovador = user.cnmNome;
                }
            }
            if (dataTableReader["DT_RECEBIMENTO"] != DBNull.Value)
            {
                pedidoEntity.DT_RECEBIMENTO = Convert.ToDateTime(dataTableReader["DT_RECEBIMENTO"]);
            }

            pedidoEntity.TX_OBS = dataTableReader["TX_OBS"].ToString();
            pedidoEntity.PENDENTE = dataTableReader["PENDENTE"].ToString();
            pedidoEntity.NR_DOC_ORI = Convert.ToInt64("0" + dataTableReader["NR_DOC_ORI"]);
            pedidoEntity.statusPedido.ID_STATUS_PEDIDO = Convert.ToInt64("0" + dataTableReader["ID_STATUS_PEDIDO"]);
            pedidoEntity.statusPedido.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_PEDIDO"].ToString();
            pedidoEntity.TP_TIPO_PEDIDO = dataTableReader["TP_TIPO_PEDIDO"].ToString();
            pedidoEntity.cliente.CD_CLIENTE = Convert.ToInt32("0" + dataTableReader["CD_CLIENTE"]);
            pedidoEntity.CD_PEDIDO = Convert.ToInt64("0" + dataTableReader["CD_PEDIDO"]);
        }


        protected void EnviarEmailPedido(PedidoEntity pedidoEntity, string msgStatus, ref string Mensagem)
        {
            //Método Modelo envio de email
            try
            {
                string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsSolicitacaoPeca");

                if (string.IsNullOrEmpty(emailsSolicitacaoPeca))
                {
                    return;
                }

                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                tecnicoEntity.CD_TECNICO = pedidoEntity.tecnico.CD_TECNICO;
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                        tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                        tecnicoEntity.usuario.cdsEmail = "";
                        tecnicoEntity.usuarioCoordenador.cdsEmail = "";
                        if (dataTableReader["cdsEmail"] != DBNull.Value)
                        {
                            tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        }
                        if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                        {
                            tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                PedidoEntity _pedidoEntity = new PedidoEntity();
                _pedidoEntity.ID_PEDIDO = pedidoEntity.ID_PEDIDO;
                dataTableReader = new PedidoData().ObterLista(_pedidoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        pedidoEntity.DT_CRIACAO = Convert.ToDateTime(dataTableReader["DT_CRIACAO"]);
                        pedidoEntity.CD_PEDIDO = Convert.ToInt64(dataTableReader["CD_PEDIDO"]);
                        pedidoEntity.FL_EMERGENCIA = dataTableReader["FL_EMERGENCIA"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                string msgEmergencia = "Não";
                if (pedidoEntity.FL_EMERGENCIA == "S")
                {
                    msgEmergencia = "<strong>SIM</strong>";
                }

                // Envia a requisição de troca de senha por e-mail
                MailSender mailSender = new MailSender();

                string mailTo = emailsSolicitacaoPeca;
                if (msgStatus != "SOLICITADO")
                {
                    mailTo = "";
                    if (tecnicoEntity.usuario.cdsEmail != "")
                    {
                        mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                    }
                    if (tecnicoEntity.usuarioCoordenador.cdsEmail != "")
                    {
                        mailTo += tecnicoEntity.usuarioCoordenador.cdsEmail + ";";
                    }
                    //if (mailTo != "")
                    //{
                    //    mailTo = mailTo.Remove(mailTo.Length - 1);
                    //}
                }
                string mailSubject = "3M.Comodato - Pedido de Peça ( " + msgStatus + " )";
                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = null;

                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = string.Empty;
                string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");

                Conteudo += "<p>Um Pedido de Peças acaba de ser <strong>" + msgStatus + "</strong>!</p>";
                Conteudo += "<p>Segue dados do pedido:</p>";
                Conteudo += "<p>Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";
                Conteudo += "Pedido: <strong>" + Convert.ToInt64(pedidoEntity.CD_PEDIDO).ToString("000000") + "</strong><br/>";
                Conteudo += "Data: " + Convert.ToDateTime(pedidoEntity.DT_CRIACAO).ToString("dd/MM/yyyy") + "<br/>";
                Conteudo += "Status do pedido: " + msgStatus + "<br/>";
                Conteudo += "Tipo: <strong>";
                if (pedidoEntity.TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Value)
                {
                    Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[0].Key + "</strong><br/>";
                }
                else if (pedidoEntity.TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Value)
                {
                    Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[1].Key + "</strong><br/>";
                }
                else if (pedidoEntity.TP_TIPO_PEDIDO == ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Value)
                {
                    Conteudo += ControlesUtility.Dicionarios.TipoPedido().ToArray()[2].Key + "</strong><br/>";
                }
                Conteudo += "Emergência: " + msgEmergencia + "<br/></p>";
                Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB:</p>";
                Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                MensagemEmail.Replace("#Conteudo", Conteudo);
                mailMessage = MensagemEmail.ToString();

                mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                Mensagem = "Não foi possível enviar e-mail para este pedido!";
            }
        }

        [HttpPost]
        [Route("GravarPedidoPecaSinc")]
        public IHttpActionResult GravarPedidoPecaSinc(JObject JO, Int64 idUsuario)
        {
            SincronismoData sincData = new SincronismoData();
            Int64 id_log_sincronismo = 0;
            id_log_sincronismo = sincData.GravaLogSincronismo(idUsuario, Convert.ToString(JO));

            try
            {
                List<PedidoSinc> listaPedido = new List<PedidoSinc>();
                listaPedido = JsonConvert.DeserializeObject<List<PedidoSinc>>(JO["PEDIDO"] is null ? "" : JO["PEDIDO"].ToString());

                List<PedidoPecaSinc> listaPedidoPeca = new List<PedidoPecaSinc>();
                listaPedidoPeca = JsonConvert.DeserializeObject<List<PedidoPecaSinc>>(JO["PEDIDO_PECA"] is null ? "" : JO["PEDIDO_PECA"].ToString());

                List<PedidoPecaLogSinc> listaPedidoPecaLog = new List<PedidoPecaLogSinc>();
                listaPedidoPecaLog = JsonConvert.DeserializeObject<List<PedidoPecaLogSinc>>(JO["PEDIDO_PECA_LOG"] is null ? "" : JO["PEDIDO_PECA_LOG"].ToString());

                List<PedidoPecaSinc> listaPedidoPecaRecebimento = new List<PedidoPecaSinc>();
                listaPedidoPecaRecebimento = JsonConvert.DeserializeObject<List<PedidoPecaSinc>>(JO["PEDIDO_PECA_RECEBIMENTO"] is null ? "" : JO["PEDIDO_PECA_RECEBIMENTO"].ToString());

                SalvarImagemPecaPedidoEmDisco(listaPedidoPeca);

                sincData.GravarPedidoPecaSinc(idUsuario, ref listaPedido, listaPedidoPeca, listaPedidoPecaLog);

                if (listaPedidoPecaRecebimento?.Count > 0)
                    sincData.ProcessarRecebimentoPecaPedido(idUsuario, listaPedidoPecaRecebimento);

                string resultadoSincronismo = "sucesso";
                sincData.AtualizaLogSincronismo(id_log_sincronismo, resultadoSincronismo);

                return Ok(resultadoSincronismo);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                sincData.AtualizaLogSincronismo(id_log_sincronismo, "ERRO:" + ex.Message);
                return BadRequest(ex.Message);
            }

        }

        private void SalvarImagemPecaPedidoEmDisco(List<PedidoPecaSinc> listaPedidoPeca)
        {
            string diretorio = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload) + ControlesUtility.Constantes.PastaFotosPecasSincronismo;
            if (listaPedidoPeca != null)
                listaPedidoPeca.Where(c => !string.IsNullOrEmpty(c.DS_DIR_FOTO) && string.IsNullOrEmpty(c.ID_ITEM_PEDIDO.ToString())).ToList()
                    .ForEach(c => c.DS_DIR_FOTO = ControlesUtility.ImageUtility.SaveImage(diretorio, c.DS_DIR_FOTO));
        }

        private Boolean ValidarLotes(string lotes, Int64 ID_PEDIDO)
        {
            var pecas = lotes.Split(',').ToList();

            int Contador3M1 = 0;
            int Contador3M2 = 0;

            foreach (var pc in pecas)
            {
                if (pc != "")
                {
                    var itemPedido = new PedidoPecaData().ObterListaPedidoPecaPedido(ID_PEDIDO).Where(x => x.ID_ITEM_PEDIDO == Convert.ToInt64(pc)).FirstOrDefault();
                    if (itemPedido != null)
                    {
                        if (itemPedido.CD_PECA_REFERENCIA == "" || itemPedido.CD_PECA_REFERENCIA == null)
                        {
                            Contador3M1++;
                        }
                        else
                        {
                            Contador3M2++;
                        }
                    }
                }
                
            }

            if (Contador3M1 > 0 && Contador3M2 > 0)
                return false;
            else
                return true;
        }

        [HttpPost]
        [Route("InserirInformacoesPedidoPeca")]
        public HttpResponseMessage InserirInformacoesPedidoPeca(DadosPedidoEntity dadosPedidoEntity)
        {
            try
            {
                Int64 NumeroRemessa = 0;

                if (ValidarLotes(dadosPedidoEntity.pecasLote, Convert.ToInt64(dadosPedidoEntity.ID_PEDIDO)))
                {
                    Int64 PedidoId = 0;
                    if (dadosPedidoEntity.ID_PEDIDO != null)
                        PedidoId = Convert.ToInt64(dadosPedidoEntity.ID_PEDIDO);

                    var ListatotaldeDados = new PedidoPecaData().ObterListaDadosPedidoTotal(PedidoId).ToList();



                    if (ListatotaldeDados?.Count == 0)
                        NumeroRemessa = 1;
                    else
                    {
                        var dados = ListatotaldeDados.Last();

                        var idPedido = dados.ID_PEDIDO.Value.ToString();

                        if (idPedido.Length <= 5)
                        {
                            if (dados.NR_REMESSA >= 999)
                            {
                                throw new Exception("Não será possível fazer o envio dessa solicitação, pois excede a capacidade atual do sistema.");
                            }
                            else
                            {
                                NumeroRemessa = Convert.ToInt64(dados.NR_REMESSA) + 1;
                            }
                        }
                        else if(idPedido.Length == 6)
                        {
                            if (dados.NR_REMESSA >= 99)
                            {
                                throw new Exception("Não será possível fazer o envio dessa solicitação, pois excede a capacidade atual do sistema.");
                            }
                            else
                            {
                                NumeroRemessa = Convert.ToInt64(dados.NR_REMESSA) + 1;
                            }
                        }
                        else if (idPedido.Length == 7)
                        {
                            if (dados.NR_REMESSA >= 9)
                            {
                                throw new Exception("Não será possível fazer o envio dessa solicitação, pois excede a capacidade atual do sistema.");
                            }
                            else
                            {
                                NumeroRemessa = Convert.ToInt64(dados.NR_REMESSA) + 1;
                            }
                        }
                        else
                        {
                            throw new Exception("O número de controle do BPCS ultrapassará o limite de 10 casas.");
                        }
                        
                        
                    //    var dados1 = ListatotaldeDados.Where(x => x.NR_REMESSA == 1).ToList();
                    //    var dados2 = ListatotaldeDados.Where(x => x.NR_REMESSA == 2).ToList();
                    //    var dados3 = ListatotaldeDados.Where(x => x.NR_REMESSA == 3).ToList();
                    //    var dados4 = ListatotaldeDados.Where(x => x.NR_REMESSA == 4).ToList();
                    //    var dados5 = ListatotaldeDados.Where(x => x.NR_REMESSA == 5).ToList();
                    //    var dados6 = ListatotaldeDados.Where(x => x.NR_REMESSA == 6).ToList();
                    //    var dados7 = ListatotaldeDados.Where(x => x.NR_REMESSA == 7).ToList();
                    //    var dados8 = ListatotaldeDados.Where(x => x.NR_REMESSA == 8).ToList();
                    //    var dados9 = ListatotaldeDados.Where(x => x.NR_REMESSA == 9).ToList();
                    //    var dados10 = ListatotaldeDados.Where(x => x.NR_REMESSA == 10).ToList();

                    //    if (dados1?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados2?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados3?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados4?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados5?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados6?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados7?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados8?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados9?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;
                    //    if (dados10?.Count > 0)
                    //        NumeroRemessa = NumeroRemessa + 1;

                    //    NumeroRemessa++;
                    }

                    IList<PedidoPecaSinc> pecasPedidoSemItem = new PedidoPecaData().ObterListaPedidoPecaPedido(PedidoId);

                    var listaPecas = dadosPedidoEntity.pecasLote.Split(',').ToList();

                    foreach (var id_item_pedido in listaPecas)
                    {
                        if (id_item_pedido != "")
                        {
                            DadosPedidoEntity infoPedido = new DadosPedidoEntity();
                            var itemPedido = new PedidoPecaData().ObterListaPedidoPecaPedido(PedidoId).Where(x => x.ID_ITEM_PEDIDO == Convert.ToInt64(id_item_pedido)).FirstOrDefault();
                            infoPedido.CD_PECA = itemPedido.CD_PECA;
                            infoPedido.ID_PEDIDO = dadosPedidoEntity.ID_PEDIDO;
                            infoPedido.ID_ITEM_PEDIDO = itemPedido.ID_ITEM_PEDIDO;
                            infoPedido.DS_APROVADOR = dadosPedidoEntity.DS_APROVADOR;
                            infoPedido.DS_TELEFONE = dadosPedidoEntity.DS_TELEFONE.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                            infoPedido.DS_APROVADOR = dadosPedidoEntity.DS_APROVADOR;
                            infoPedido.RAMAL = dadosPedidoEntity.RAMAL;
                            infoPedido.RESP_CLIENTE = dadosPedidoEntity.RESP_CLIENTE;
                            infoPedido.VOLUME = dadosPedidoEntity.VOLUME;
                            infoPedido.PesoBruto = dadosPedidoEntity.PesoBruto;
                            infoPedido.PesoLiquido = dadosPedidoEntity.PesoLiquido;
                            infoPedido.NR_REMESSA = NumeroRemessa;

                            var dadosPedido = new PedidoPecaData().ObterListaDadosPedido(Convert.ToInt64(infoPedido.ID_ITEM_PEDIDO)).ToList();

                            if (dadosPedido.Count > 0)
                            {
                                throw new Exception($"Já existe registro para este item: {infoPedido.CD_PECA}");
                            }
                            else
                            {
                                new PedidoPecaData().InserirDadosPedido(ref infoPedido);

                                new PedidoPecaData().AtualizarEnvioRemessaBPCSPECA(infoPedido.ID_ITEM_PEDIDO);
                            }
                        }


                    }
                }
                else
                {
                    throw new Exception($"Não é possível enviar os dados do Est. 3M1 e 3M2 na mesma remessa de NF!");
                }
               
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
