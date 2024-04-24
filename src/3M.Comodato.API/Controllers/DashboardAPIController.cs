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
    [RoutePrefix("api/DashboardAPI")]
    [Authorize]
    public class DashboardAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("ObterBoxEquipamentoComodatoLocado")]
        public IHttpActionResult ObterBoxEquipamentoComodatoLocado(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null, Int64? CD_TIPO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxEquipamentoComodatoLocado(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO, CD_TIPO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxClienteAtivo")]
        public IHttpActionResult ObterBoxClienteAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxClienteAtivo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxEquipamentoEnviado")]
        public IHttpActionResult ObterBoxEquipamentoEnviado(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTALDevolucao = 0;
            Int64 TOTALEnviado = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxEquipamentoEnviado(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        TOTALDevolucao = Convert.ToInt64("0" + dataTableReader["TOTALDevolucao"].ToString());
                        TOTALEnviado = Convert.ToInt64("0" + dataTableReader["TOTALEnviado"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTALDevolucao", JsonConvert.SerializeObject(TOTALDevolucao, Formatting.None));
            jObject.Add("TOTALEnviado", JsonConvert.SerializeObject(TOTALEnviado, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxProjEnvioEquip")]
        public IHttpActionResult ObterBoxProjEnvioEquip(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            string PERCENTUAL_GRAFICO_REALIZADO = "0";
            string PERCENTUAL_GRAFICO_GAP = "0";
            string PERCENTUAL_GRAFICO_RESTANTE = "0";
            string TIPO_GAP = string.Empty;

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxProjEnvioEquip(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
                        PERCENTUAL_GRAFICO_REALIZADO = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_REALIZADO"]).ToString("N0");
                        PERCENTUAL_GRAFICO_GAP = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_GAP"]).ToString("N0");

                        if ((dataTableReader["PERCENTUAL_GRAFICO_RESTANTE"]) != DBNull.Value)
                        {
                            if(dataTableReader["PERCENTUAL_GRAFICO_RESTANTE"].ToString().Contains("-"))
                                PERCENTUAL_GRAFICO_RESTANTE = Convert.ToDecimal("0").ToString("N0");
                            else
                                PERCENTUAL_GRAFICO_RESTANTE = Convert.ToDecimal(dataTableReader["PERCENTUAL_GRAFICO_RESTANTE"]).ToString("N0");
                        }

                        TIPO_GAP = dataTableReader["TIPO_GAP"].ToString();
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            jObject.Add("PERCENTUAL_GRAFICO_REALIZADO", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_REALIZADO, Formatting.None));
            jObject.Add("PERCENTUAL_GRAFICO_GAP", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_GAP, Formatting.None));
            jObject.Add("PERCENTUAL_GRAFICO_RESTANTE", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_RESTANTE, Formatting.None));
            jObject.Add("TIPO_GAP", JsonConvert.SerializeObject(TIPO_GAP, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxPecaEnviada")]
        public IHttpActionResult ObterBoxPecaEnviada(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            decimal TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxPecaEnviada(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        var valor = dataTableReader["TOTAL"].ToString();
                        if (valor == null || valor == "")
                        {
                            TOTAL = 0;
                        }
                        else
                            TOTAL = Convert.ToDecimal(dataTableReader["TOTAL"]);
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxAtendimento")]
        public IHttpActionResult ObterBoxAtendimento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            DateTime? vigenciaINICIAL = null;
            DateTime? vigenciaFINAL = null;
            string Atendimento = "0%";
            string TotalPeriodos = "0";
            string PeriodosRealizados = "0 0%";
            string PeriodosPlanejados = "0 0%";
            string VisitasRealizadas = "0";
            string OSRealizadas = "0";
            string ClientesAtendidos = "0";
            string Tecnicos = "0";
            string Vigencia = string.Empty;
            string ClientesPerdidos = "0";
            string ValorPecaEnviada = "R$0,00";
            string ValorMetaPecaRecupEnviado = "R$0,00";
            string ValorPecaEnviada3M1 = "R$0,00";
            string ValorPecaEnviadaRec = "R$0,00";
            string ValorPecaRecuperadaMes = "R$0,00";

            Int64 TOTAL = 0;
            string PERCENTUAL_GRAFICO_REALIZADO = "0";
            string PERCENTUAL_GRAFICO_GAP = "0";
            string PERCENTUAL_GRAFICO_RESTANTE = "0";
            string TIPO_GAP = string.Empty;

            try
            {
                // Vigência
                try
                {
                    vigenciaINICIAL = Convert.ToDateTime(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.vigenciaINICIAL));
                }
                catch { }
                try
                {
                    vigenciaFINAL = Convert.ToDateTime(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.vigenciaFINAL));
                }
                catch { }

                if (vigenciaINICIAL == null)
                    vigenciaINICIAL = Convert.ToDateTime("01/01/" + DateTime.Now.Year.ToString());
                if (vigenciaFINAL == null)
                    vigenciaFINAL = Convert.ToDateTime("31/12/" + DateTime.Now.Year.ToString());

                Vigencia = Convert.ToDateTime(vigenciaINICIAL).ToString("dd/MM/yyyy") + " à " + Convert.ToDateTime(vigenciaFINAL).ToString("dd/MM/yyyy");

                // Periodos
                DataTableReader dataTableReader = new DashboardData().ObterBoxPeriodo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        Atendimento = dataTableReader["TIPO_GAP"].ToString() + Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_GAP"]).ToString("N0") + "%";
                        TotalPeriodos = Convert.ToInt64("0" + dataTableReader["TOTAL"]).ToString("N0");
                        PeriodosRealizados = Convert.ToDecimal("0" + dataTableReader["TOTAL_VISITA_REALIZADO"]).ToString("N0") + " " + Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_REALIZADO"]).ToString("N0") + "%";
                        PeriodosPlanejados = Convert.ToDecimal("0" + dataTableReader["TOTAL_VISITA_PLANEJADO"]).ToString("N0") + " " + Convert.ToDecimal("0" + (Convert.ToDecimal(dataTableReader["PERCENTUAL_GRAFICO_REALIZADO"]) + Convert.ToDecimal(dataTableReader["PERCENTUAL_GRAFICO_GAP"]))).ToString("N0") + "%";

                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"]);
                        PERCENTUAL_GRAFICO_REALIZADO = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_REALIZADO"]).ToString("N0");
                        PERCENTUAL_GRAFICO_GAP = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_GAP"]).ToString("N0");
                        if (dataTableReader["PERCENTUAL_GRAFICO_RESTANTE"].ToString().Contains("-"))
                        {
                            PERCENTUAL_GRAFICO_RESTANTE = Convert.ToDecimal("0").ToString("N0");
                        }
                        else
                            PERCENTUAL_GRAFICO_RESTANTE = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_RESTANTE"]).ToString("N0");
                        TIPO_GAP = dataTableReader["TIPO_GAP"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                // Visitas e OS realizadas, Técnicos e Clientes Perdidos, Valor Peça Enviadas Item
                dataTableReader = new DashboardData().ObterBoxAtendimento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        VisitasRealizadas = dataTableReader["TOTAL_Visitas_Realizadas"].ToString();
                        OSRealizadas = dataTableReader["TOTAL_OS_Realizadas"].ToString();
                        Tecnicos = dataTableReader["TOTAL_Tecnicos"].ToString();
                        ClientesPerdidos = dataTableReader["TOTAL_Clientes_Perdidos"].ToString();
                        ValorPecaEnviada = Convert.ToDecimal(dataTableReader["VALOR_Peca_Enviado_Item"]).ToString("C2");
                        ValorMetaPecaRecupEnviado = Convert.ToDecimal(dataTableReader["VALOR_MetaPecaRecupEnviado"]).ToString("C2");
                        ValorPecaEnviada3M1 = Convert.ToDecimal(dataTableReader["VALOR_Peca_Enviado_3M1"]).ToString("C2");
                        ValorPecaEnviadaRec = Convert.ToDecimal(dataTableReader["VALOR_Peca_Enviado_Recuperado"]).ToString("C2");
                        ValorPecaRecuperadaMes = Convert.ToDecimal(dataTableReader["VALOR_Peca_Recuperada_Mes"]).ToString("C2");
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("Atendimento", JsonConvert.SerializeObject(Atendimento, Formatting.None));
            jObject.Add("TotalPeriodos", JsonConvert.SerializeObject(TotalPeriodos, Formatting.None));
            jObject.Add("PeriodosRealizados", JsonConvert.SerializeObject(PeriodosRealizados, Formatting.None));
            jObject.Add("PeriodosPlanejados", JsonConvert.SerializeObject(PeriodosPlanejados, Formatting.None));
            jObject.Add("VisitasRealizadas", JsonConvert.SerializeObject(VisitasRealizadas, Formatting.None));
            jObject.Add("OSRealizadas", JsonConvert.SerializeObject(OSRealizadas, Formatting.None));
            jObject.Add("ClientesAtendidos", JsonConvert.SerializeObject(ClientesAtendidos, Formatting.None));
            jObject.Add("Tecnicos", JsonConvert.SerializeObject(Tecnicos, Formatting.None));
            jObject.Add("Vigencia", JsonConvert.SerializeObject(Vigencia, Formatting.None));
            jObject.Add("ClientesPerdidos", JsonConvert.SerializeObject(ClientesPerdidos, Formatting.None));
            jObject.Add("ValorPecaEnviada", JsonConvert.SerializeObject(ValorPecaEnviada, Formatting.None));
            jObject.Add("ValorMetaPecaRecupEnviado", JsonConvert.SerializeObject(ValorMetaPecaRecupEnviado, Formatting.None));
            jObject.Add("ValorPecaEnviada3M1", JsonConvert.SerializeObject(ValorPecaEnviada3M1, Formatting.None));
            jObject.Add("ValorPecaEnviadaRec", JsonConvert.SerializeObject(ValorPecaEnviadaRec, Formatting.None));
            jObject.Add("ValorPecaRecuperadaMes", JsonConvert.SerializeObject(ValorPecaRecuperadaMes, Formatting.None));

            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));
            jObject.Add("PeriodosTitulo", JsonConvert.SerializeObject("Períodos (" + TIPO_GAP + PERCENTUAL_GRAFICO_GAP + "%)", Formatting.None));

            jObject.Add("PERCENTUAL_GRAFICO_REALIZADO", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_REALIZADO, Formatting.None));
            jObject.Add("PERCENTUAL_GRAFICO_GAP", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_GAP, Formatting.None));
            jObject.Add("PERCENTUAL_GRAFICO_RESTANTE", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_RESTANTE, Formatting.None));
            jObject.Add("TIPO_GAP", JsonConvert.SerializeObject(TIPO_GAP, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxSolicitacaoAtendimentoPendente")]
        public IHttpActionResult ObterBoxSolicitacaoAtendimentoPendente(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxSolicitacaoAtendimentoPendente(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxAtendimentoAndamento")]
        public IHttpActionResult ObterBoxAtendimentoAndamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxAtendimentoAndamento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxAtendimentoVisitasAndamento")]
        public IHttpActionResult ObterBoxAtendimentoVisitasAndamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxAtendimentoVisitasAndamento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxUnitizacao")]
        public IHttpActionResult ObterBoxUnitizacao(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxUnitizacao(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64(dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxDistribuicaoKAT")]
        public IHttpActionResult ObterBoxDistribuicaoKAT(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            string TOTALA = string.Empty;
            string TOTALB = string.Empty;
            string TOTALC = string.Empty;

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxDistribuicaoKAT(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        decimal CALCULO = 0;

                        if (Convert.ToInt64(dataTableReader["TOTAL_KAT"]) > 0)
                        {
                            CALCULO = (Convert.ToDecimal(dataTableReader["TOTAL"]) * 100) / Convert.ToDecimal(dataTableReader["TOTAL_KAT"]);
                        }

                        if (dataTableReader["DS_CLASSIFICACAO_KAT"].ToString() == "A")
                            TOTALA = dataTableReader["DS_CLASSIFICACAO_KAT"].ToString() + " " + CALCULO.ToString("N2") + "%"; // / " + "0%";
                        else if (dataTableReader["DS_CLASSIFICACAO_KAT"].ToString() == "B")
                            TOTALB = dataTableReader["DS_CLASSIFICACAO_KAT"].ToString() + " " + CALCULO.ToString("N2") + "%"; // / " + "0%";
                        else
                            TOTALC = dataTableReader["DS_CLASSIFICACAO_KAT"].ToString() + " " + CALCULO.ToString("N2") + "%"; // / " + "0%";
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTALA", JsonConvert.SerializeObject(TOTALA, Formatting.None));
            jObject.Add("TOTALB", JsonConvert.SerializeObject(TOTALB, Formatting.None));
            jObject.Add("TOTALC", JsonConvert.SerializeObject(TOTALC, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxAtivoEnviadoNaoInstalado")]
        public IHttpActionResult ObterBoxAtivoEnviadoNaoInstalado(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTALEnviados = 0;
            Int64 TOTALNaoInstalados = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxAtivoEnviadoNaoInstalado(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        TOTALEnviados = Convert.ToInt64("0" + dataTableReader["TOTALEnviados"].ToString());
                        TOTALNaoInstalados = Convert.ToInt64("0" + dataTableReader["TOTALNaoInstalados"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTALEnviados", JsonConvert.SerializeObject(TOTALEnviados, Formatting.None));
            jObject.Add("TOTALNaoInstalados", JsonConvert.SerializeObject(TOTALNaoInstalados, Formatting.None));

            return Ok(jObject);
        }

        //[HttpPost]
        //[Route("ObterBoxExcecaoAtendimento")]
        //public IHttpActionResult ObterBoxExcecaoAtendimento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        //{
        //    Int64 TOTAL = 0;
        //    try
        //    {
        //        DataTableReader dataTableReader = new DashboardData().ObterBoxExcecaoAtendimento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

        //        if (dataTableReader.HasRows)
        //        {
        //            if (dataTableReader.Read())
        //                TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
        //        }

        //        if (dataTableReader != null)
        //        {
        //            dataTableReader.Dispose();
        //            dataTableReader = null;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        return BadRequest(ex.Message);
        //    }

        //    JObject jObject = new JObject();
        //    jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

        //    return Ok(jObject);
        //}

        //[HttpPost]
        //[Route("ObterBoxAtendimentoAreaTecnica")]
        //public IHttpActionResult ObterBoxAtendimentoAreaTecnica(Int64? CD_CLIENTE = null)
        //{
        //    string Atendimento = "0%";
        //    string TotalPeriodos = "0";
        //    string Tecnicos = "0";
        //    string PeriodosRealizados = "0 0%";
        //    string PeriodosPlanejados = "0 0%";

        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        return BadRequest(ex.Message);
        //    }

        //    JObject jObject = new JObject();
        //    jObject.Add("Atendimento", JsonConvert.SerializeObject(Atendimento, Formatting.None));
        //    jObject.Add("TotalPeriodos", JsonConvert.SerializeObject(TotalPeriodos, Formatting.None));
        //    jObject.Add("Tecnicos", JsonConvert.SerializeObject(Tecnicos, Formatting.None));
        //    jObject.Add("PeriodosRealizados", JsonConvert.SerializeObject(PeriodosRealizados, Formatting.None));
        //    jObject.Add("PeriodosPlanejados", JsonConvert.SerializeObject(PeriodosPlanejados, Formatting.None));

        //    return Ok(jObject);
        //}

        [HttpPost]
        [Route("ObterBoxFechadorAtivo")]
        public IHttpActionResult ObterBoxFechadorAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxFechadorAtivo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxIdentificadorAtivo")]
        public IHttpActionResult ObterBoxIdentificadorAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxIdentificadorAtivo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString());
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxPeriodo")]
        public IHttpActionResult ObterBoxPeriodo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            Int64 TOTAL = 0;
            string PERCENTUAL_GRAFICO_REALIZADO = "0";
            string PERCENTUAL_GRAFICO_GAP = "0";
            string PERCENTUAL_GRAFICO_RESTANTE = "0";
            string TIPO_GAP = string.Empty;
            string PeriodosTitulo = string.Empty;
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxPeriodo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        TOTAL = Convert.ToInt64("0" + dataTableReader["TOTAL"]);
                        PERCENTUAL_GRAFICO_REALIZADO = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_REALIZADO"]).ToString("N0");
                        PERCENTUAL_GRAFICO_GAP = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_GAP"]).ToString("N0");
                        if(dataTableReader["PERCENTUAL_GRAFICO_RESTANTE"].ToString().Contains("-"))
                            PERCENTUAL_GRAFICO_RESTANTE = Convert.ToDecimal("0").ToString("N0");
                        else
                            PERCENTUAL_GRAFICO_RESTANTE = Convert.ToDecimal("0" + dataTableReader["PERCENTUAL_GRAFICO_RESTANTE"]).ToString("N0");
                        TIPO_GAP = dataTableReader["TIPO_GAP"].ToString();
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));
            jObject.Add("PeriodosTitulo", JsonConvert.SerializeObject("Períodos (" + TIPO_GAP + PERCENTUAL_GRAFICO_GAP + "%)", Formatting.None));

            jObject.Add("PERCENTUAL_GRAFICO_REALIZADO", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_REALIZADO, Formatting.None));
            jObject.Add("PERCENTUAL_GRAFICO_GAP", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_GAP, Formatting.None));
            jObject.Add("PERCENTUAL_GRAFICO_RESTANTE", JsonConvert.SerializeObject(PERCENTUAL_GRAFICO_RESTANTE, Formatting.None));
            jObject.Add("TIPO_GAP", JsonConvert.SerializeObject(TIPO_GAP, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxVenda")]
        public IHttpActionResult ObterBoxVenda(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            decimal TOTALAnoAtual = 0;
            decimal TOTALAnoAnterior = 0;
            string VendasTitulo = "Vendas " + DateTime.Now.Year.ToString();
            string VendasTituloAnoAnterior = "Gap " + DateTime.Now.AddYears(-1).Year.ToString();
            Int32 PercentualAnoAnterior = 0;
            string TOTAL = "0";
            string CSS = "badge badge-pill badge-light";
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxVenda(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (Convert.ToInt64("0" + dataTableReader["ANO"].ToString()) == DateTime.Now.Year)
                            TOTALAnoAtual = Convert.ToDecimal("0" + dataTableReader["TOT_VENDAS"].ToString());
                        else
                            TOTALAnoAnterior = Convert.ToDecimal("0" + dataTableReader["TOT_VENDAS"].ToString());
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                //        if (TOTALAnoAnterior == 0)
                //        {
                //            PercentualAnoAnterior = 0;
                //        }
                //
                //        else
                //        {
                //            decimal diferenca = TOTALAnoAtual - TOTALAnoAnterior;
                //            if (TOTALAnoAtual > 0)
                //            {
                //                PercentualAnoAnterior = Convert.ToInt64((100 * diferenca) / TotalAnoAtual); //.ToString("N0");
                //            }
                //        }
                if (TOTALAnoAnterior > 0)
                    PercentualAnoAnterior = Convert.ToInt32((TOTALAnoAtual / TOTALAnoAnterior) * 100);

                if (PercentualAnoAnterior >= 90 && PercentualAnoAnterior < 100)
                    CSS = "badge badge-pill badge-warning";
                else if (PercentualAnoAnterior >= 100)
                    CSS = "badge badge-pill badge-success";
                else
                    CSS = "badge badge-pill badge-danger";


                //       if (Convert.ToInt64(PercentualAnoAnterior) <= -5 || TOTALAnoAtual == 0)
                //           CSS = "badge badge-pill badge-danger";
                //       else if (Convert.ToInt64(PercentualAnoAnterior) >= 5)
                //           CSS = "badge badge-pill badge-success";
                //       else
                //           CSS = "badge badge-pill badge-warning";

                if (TOTALAnoAtual > 1000000)
                    TOTAL = Convert.ToDecimal(TOTALAnoAtual / 1000000).ToString("N0") + "MI";
                else if (TOTALAnoAtual > 1000)
                    TOTAL = Convert.ToDecimal(TOTALAnoAtual / 1000).ToString("N0") + "K";
                else
                    TOTAL = TOTALAnoAtual.ToString("N2");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));
            jObject.Add("VendasTitulo", JsonConvert.SerializeObject(VendasTitulo, Formatting.None));
            jObject.Add("VendasTituloAnoAnterior", JsonConvert.SerializeObject(VendasTituloAnoAnterior, Formatting.None));
            if (TOTALAnoAtual > 0)
                jObject.Add("PercentualAnoAnterior", JsonConvert.SerializeObject(PercentualAnoAnterior.ToString() + "%", Formatting.None));
            else
            {
                jObject.Add("PercentualAnoAnterior", JsonConvert.SerializeObject("--", Formatting.None));
            }
            jObject.Add("CSS", JsonConvert.SerializeObject(CSS, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxVendedor")]
        public IHttpActionResult ObterBoxVendedor(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            string ClientesAtendidos = "0";
            string Equipamentos = "0";
            string Depreciacao12Meses = "R$0,00";
            string CustoManutencao = "0";
            string Investimento = "R$0,00";
            string QtdeEquipamentosSemConsumo = "0";
            //string QtdeClientesEmprestimo = "0";

            string SolicitacaoEnvio = "0";
            string AnaliseMarketing = "0";
            string AnaliseAreaTecnica = "0";
            string AnalisePlanejamento = "0";
            string EnviadoCliente = "0";
            string Instalado = "0";

            string SolicitacaoRetirada = "0";
            string AnaliseLogistica = "0";
            string ComTransportadora = "0";
            string EmAgendamentoTMS = "0";
            string Devolvido3M = "0";

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxVendedor(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        Equipamentos = Convert.ToInt64(dataTableReader["TOTAL_Equipamentos"]).ToString("N0");
                        Depreciacao12Meses = Convert.ToDecimal(dataTableReader["TOTAL_Depreciacao"]).ToString("C2");
                        CustoManutencao = Convert.ToDecimal(dataTableReader["TOTAL_Custo_Manutencao"]).ToString("C2");
                        QtdeEquipamentosSemConsumo = Convert.ToInt64(dataTableReader["TOTAL_Clientes_Sem_Consumo"]).ToString("N0");
                        Investimento = Convert.ToDecimal(dataTableReader["TOTAL_Investimento"]).ToString("C2");

                        SolicitacaoEnvio = Convert.ToInt64(dataTableReader["TOTAL_Solicitacao_Envio"]).ToString("N0");
                        AnaliseMarketing = Convert.ToInt64(dataTableReader["TOTAL_Analise_Marketing"]).ToString("N0");
                        AnaliseAreaTecnica = Convert.ToInt64(dataTableReader["TOTAL_Analise_Area_Tecnica"]).ToString("N0");
                        AnalisePlanejamento = Convert.ToInt64(dataTableReader["TOTAL_Analise_Planejamento"]).ToString("N0");
                        EnviadoCliente = Convert.ToInt64(dataTableReader["TOTAL_Enviado_Cliente"]).ToString("N0");
                        Instalado = Convert.ToInt64(dataTableReader["TOTAL_Instalado"]).ToString("N0");

                        SolicitacaoRetirada = Convert.ToInt64(dataTableReader["TOTAL_Solicitacao_Retirada"]).ToString("N0");
                        AnaliseLogistica = Convert.ToInt64(dataTableReader["TOTAL_Analise_Logistica"]).ToString("N0");
                        ComTransportadora = Convert.ToInt64(dataTableReader["TOTAL_Com_Transportadora"]).ToString("N0");
                        EmAgendamentoTMS = Convert.ToInt64(dataTableReader["TOTAL_Em_Agendamento_TMS"]).ToString("N0");
                        Devolvido3M = Convert.ToInt64(dataTableReader["TOTAL_Devolvido_3M"]).ToString("N0");
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                // Clientes Atendidos igual ao (Clientes Ativos)
                dataTableReader = new DashboardData().ObterBoxClienteAtivo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        ClientesAtendidos = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString()).ToString();
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("ClientesAtendidos", JsonConvert.SerializeObject(ClientesAtendidos, Formatting.None));
            jObject.Add("Equipamentos", JsonConvert.SerializeObject(Equipamentos, Formatting.None));
            jObject.Add("Depreciacao12Meses", JsonConvert.SerializeObject(Depreciacao12Meses, Formatting.None));
            jObject.Add("CustoManutencao", JsonConvert.SerializeObject(CustoManutencao, Formatting.None));
            jObject.Add("Investimento", JsonConvert.SerializeObject(Investimento, Formatting.None));
            jObject.Add("QtdeEquipamentosSemConsumo", JsonConvert.SerializeObject(QtdeEquipamentosSemConsumo, Formatting.None));
            //jObject.Add("QtdeClientesEmprestimo", JsonConvert.SerializeObject(QtdeClientesEmprestimo, Formatting.None));

            jObject.Add("SolicitacaoEnvio", JsonConvert.SerializeObject(SolicitacaoEnvio, Formatting.None));
            jObject.Add("AnaliseMarketing", JsonConvert.SerializeObject(AnaliseMarketing, Formatting.None));
            jObject.Add("AnaliseAreaTecnica", JsonConvert.SerializeObject(AnaliseAreaTecnica, Formatting.None));
            jObject.Add("AnalisePlanejamento", JsonConvert.SerializeObject(AnalisePlanejamento, Formatting.None));
            jObject.Add("EnviadoCliente", JsonConvert.SerializeObject(EnviadoCliente, Formatting.None));
            jObject.Add("Instalado", JsonConvert.SerializeObject(Instalado, Formatting.None));

            jObject.Add("SolicitacaoRetirada", JsonConvert.SerializeObject(SolicitacaoRetirada, Formatting.None));
            jObject.Add("AnaliseLogistica", JsonConvert.SerializeObject(AnaliseLogistica, Formatting.None));
            jObject.Add("ComTransportadora", JsonConvert.SerializeObject(ComTransportadora, Formatting.None));
            jObject.Add("EmAgendamentoTMS", JsonConvert.SerializeObject(EmAgendamentoTMS, Formatting.None));
            jObject.Add("Devolvido3M", JsonConvert.SerializeObject(Devolvido3M, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxPecaTrocada")]
        public IHttpActionResult ObterBoxPecaTrocada(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            string TOTAL = "0";
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxPecaTrocada(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToInt64(dataTableReader["TOTAL"]).ToString("N0");
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxPesquisaSatisfacao")]
        public IHttpActionResult ObterBoxPesquisaSatisfacao(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            string TOTAL = "0";
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxPesquisaSatisfacao(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        TOTAL = Convert.ToDecimal(dataTableReader["TOTAL"]).ToString("N2");
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("TOTAL", JsonConvert.SerializeObject(TOTAL, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxLinhaProduto")]
        public IHttpActionResult ObterBoxLinhaProduto(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            //string Depreciacao12Meses = "0";
            //string CustoManutencao = "0";
            string ClientesAtendidos = "0";
            string Investimento = "0";
            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxLinhaProduto(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        //Depreciacao12Meses = Convert.ToDecimal("0" + dataTableReader["Depreciacao12Meses"]).ToString("C2");
                        //CustoManutencao = Convert.ToDecimal("0" + dataTableReader["CustoManutencao"]).ToString("C2");
                        Investimento = Convert.ToDecimal("0" + dataTableReader["TOTAL_Investimento"]).ToString("C2");
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                // Clientes Atendidos igual ao (Clientes Ativos)
                dataTableReader = new DashboardData().ObterBoxClienteAtivo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        ClientesAtendidos = Convert.ToInt64("0" + dataTableReader["TOTAL"].ToString()).ToString();
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            //jObject.Add("Depreciacao12Meses", JsonConvert.SerializeObject(Depreciacao12Meses, Formatting.None));
            //jObject.Add("CustoManutencao", JsonConvert.SerializeObject(CustoManutencao, Formatting.None));
            jObject.Add("ClientesAtendidos", JsonConvert.SerializeObject(ClientesAtendidos, Formatting.None));
            jObject.Add("Investimento", JsonConvert.SerializeObject(Investimento, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterBoxEnvioEquipamento")]
        public IHttpActionResult ObterBoxEnvioEquipamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            //Int64 SolicitacoesEnvio = 0;
            //Int64 EnvioRealizado = 0;
            //Int64 EnvioPendente = 0;
            //Int64 EmCompra = 0;

            JObject jObject = new JObject();

            try
            {
                DataTableReader dataTableReader = new DashboardData().ObterBoxEnvioEquipamento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    int item = 1;
                    while (dataTableReader.Read())
                    {
                        string ITEM = dataTableReader["DS_STATUS_NOME_REDUZ"].ToString();
                        Int64 TOTAL = Convert.ToInt64(dataTableReader["TOTAL"]);

                        jObject.Add("EnvioEquipamentoITEM_" + item.ToString(), JsonConvert.SerializeObject(ITEM, Formatting.None));
                        jObject.Add("EnvioEquipamentoTOTAL_" + item.ToString(), JsonConvert.SerializeObject(TOTAL, Formatting.None));

                        item++;
                    }
                    //if (dataTableReader.Read())
                    //{
                    //    SolicitacoesEnvio = Convert.ToInt64(dataTableReader["TOTAL_Solicitacoes_Envio"]);
                    //    EnvioRealizado = Convert.ToInt64(dataTableReader["TOTAL_Envio_Realizado"]);
                    //    EnvioPendente = Convert.ToInt64(dataTableReader["TOTAL_Envio_Pendente"]);
                    //    EmCompra = Convert.ToInt64(dataTableReader["TOTAL_Em_Compra"]);
                    //}
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
                return BadRequest(ex.Message);
            }

            //JObject jObject = new JObject();
            //jObject.Add("SolicitacoesEnvio", JsonConvert.SerializeObject(SolicitacoesEnvio, Formatting.None));
            //jObject.Add("EnvioRealizado", JsonConvert.SerializeObject(EnvioRealizado, Formatting.None));
            //jObject.Add("EnvioPendente", JsonConvert.SerializeObject(EnvioPendente, Formatting.None));
            //jObject.Add("EmCompra", JsonConvert.SerializeObject(EmCompra, Formatting.None));

            return Ok(jObject);
        }

        [HttpPost]
        [Route("ObterGraficoVolumeVenda")]
        public IHttpActionResult ObterGraficoVolumeVenda(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaVolumeVenda> listasVolumesVendas = new List<Models.ListaVolumeVenda>();
            List<Models.ListaVolumeVenda> listaAtualizada = new List<Models.ListaVolumeVenda>();

            try
            {
                Models.ListaVolumeVenda listaVolumeVenda = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoVolumeVenda(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaVolumeVenda = new Models.ListaVolumeVenda();
                        listaVolumeVenda.linhaProduto.CD_LINHA_PRODUTO = Convert.ToInt32(dataTableReader["CD_LINHA_PRODUTO"]);
                        listaVolumeVenda.linhaProduto.DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString();
                        listaVolumeVenda.TOT_VENDAS = Convert.ToDecimal(dataTableReader["TOT_VENDAS"]);
                        listaVolumeVenda.QT_VENDAS_CV = Convert.ToDecimal(dataTableReader["QT_VENDAS_CV"]);
                        listaVolumeVenda.QT_VENDAS = Convert.ToDecimal(dataTableReader["QT_VENDAS"]);
                        listaVolumeVenda.DEPCOM = Convert.ToDecimal(dataTableReader["DEPCOM"]);
                        listaVolumeVenda.LESAFO = Convert.ToDecimal(dataTableReader["LESAFO"]);

                        listasVolumesVendas.Add(listaVolumeVenda);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                decimal TotalGeralVendas = listasVolumesVendas.Sum(x => x.TOT_VENDAS);

                for (int a = 0; a < listasVolumesVendas.Count; a++)
                {
                    Int32 TOT_VENDAS_PERC = 0;
                    if (listasVolumesVendas[a].TOT_VENDAS > 0)
                        TOT_VENDAS_PERC = Convert.ToInt32((listasVolumesVendas[a].TOT_VENDAS * 100) / TotalGeralVendas);

                    listaVolumeVenda = new Models.ListaVolumeVenda();
                    listaVolumeVenda = listasVolumesVendas[a];
                    listaVolumeVenda.TOT_VENDAS_PERC = TOT_VENDAS_PERC;
                    listaAtualizada.Add(listaVolumeVenda);

                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("VOLUMEVENDA", JsonConvert.SerializeObject(listaAtualizada, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoVenda")]
        public IHttpActionResult ObterGraficoVenda(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaVenda> listasVendas = new List<Models.ListaVenda>();

            try
            {
                Models.ListaVenda listaVenda = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoVenda(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaVenda = new Models.ListaVenda();
                        listaVenda.CD_MES = Convert.ToInt32(dataTableReader["CD_MES"]);
                        listaVenda.DS_MES = dataTableReader["DS_MES"].ToString();
                        listaVenda.TOT_VENDAS = Convert.ToDecimal(Convert.ToDecimal(Convert.ToDecimal(dataTableReader["TOT_VENDAS"]) / 1000).ToString("N2"));
                        listaVenda.QT_VENDAS_CV = Convert.ToDecimal(dataTableReader["QT_VENDAS_CV"]) / 1000;
                        listaVenda.QT_VENDAS = Convert.ToDecimal(dataTableReader["QT_VENDAS"]) / 1000;
                        listaVenda.DEPCOM = Convert.ToDecimal(dataTableReader["DEPCOM"]);
                        listaVenda.LESAFO = Convert.ToDecimal(dataTableReader["LESAFO"]);

                        listasVendas.Add(listaVenda);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("VENDA", JsonConvert.SerializeObject(listasVendas, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoFamiliaModelo")]
        public IHttpActionResult ObterGraficoFamiliaModelo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaFamiliaModelo> listasFamiliaModelo = new List<Models.ListaFamiliaModelo>();

            try
            {
                Models.ListaFamiliaModelo listaFamiliaModelo = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoFamiliaModelo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaFamiliaModelo = new Models.ListaFamiliaModelo();
                        listaFamiliaModelo.grupoModelo.CD_GRUPO_MODELO = dataTableReader["cd_grupoModelo"].ToString();
                        listaFamiliaModelo.grupoModelo.DS_GRUPO_MODELO = dataTableReader["ds_grupoModelo"].ToString();
                        listaFamiliaModelo.TOTAL = Convert.ToInt64(dataTableReader["TOTAL"]);

                        listasFamiliaModelo.Add(listaFamiliaModelo);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("FAMILAMODELO", JsonConvert.SerializeObject(listasFamiliaModelo, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoValorPecaEnviadaMes")]
        public IHttpActionResult ObterGraficoValorPecaEnviadaMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            int divisor = 0;
            decimal ValorEnvioMensalPecas = 0;
            decimal ValorMETA = 0;
            List<Models.ListaValorPecaEnviadaMes> listasValorPecaEnviadaMes = new List<Models.ListaValorPecaEnviadaMes>();

            try
            {
                divisor = 12;
                ValorEnvioMensalPecas = Convert.ToDecimal("0" + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ValorEnvioMensalPecas));
                ValorMETA = ValorEnvioMensalPecas / divisor;

                Models.ListaValorPecaEnviadaMes listaValorPecaEnviadaMes = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoValorPecaEnviadaMes(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaValorPecaEnviadaMes = new Models.ListaValorPecaEnviadaMes();
                        listaValorPecaEnviadaMes.MES = Convert.ToInt32(dataTableReader["MES"]);
                        switch (listaValorPecaEnviadaMes.MES)
                        {
                            case 1:
                                listaValorPecaEnviadaMes.cdsMes = "JAN";
                                break;
                            case 2:
                                listaValorPecaEnviadaMes.cdsMes = "FEV";
                                break;
                            case 3:
                                listaValorPecaEnviadaMes.cdsMes = "MAR";
                                break;
                            case 4:
                                listaValorPecaEnviadaMes.cdsMes = "ABR";
                                break;
                            case 5:
                                listaValorPecaEnviadaMes.cdsMes = "MAI";
                                break;
                            case 6:
                                listaValorPecaEnviadaMes.cdsMes = "JUN";
                                break;
                            case 7:
                                listaValorPecaEnviadaMes.cdsMes = "JUL";
                                break;
                            case 8:
                                listaValorPecaEnviadaMes.cdsMes = "AGO";
                                break;
                            case 9:
                                listaValorPecaEnviadaMes.cdsMes = "SET";
                                break;
                            case 10:
                                listaValorPecaEnviadaMes.cdsMes = "OUT";
                                break;
                            case 11:
                                listaValorPecaEnviadaMes.cdsMes = "NOV";
                                break;
                            case 12:
                                listaValorPecaEnviadaMes.cdsMes = "DEZ";
                                break;
                        }
                        listaValorPecaEnviadaMes.TOTAL_3M1 = Convert.ToDecimal("0" + dataTableReader["TOTAL_3M1"]);
                        listaValorPecaEnviadaMes.TOTAL_3M2 = Convert.ToDecimal("0" + dataTableReader["TOTAL_3M2"]);
                        listaValorPecaEnviadaMes.TOTAL_3M3 = Convert.ToDecimal("0" + dataTableReader["TOTAL_3M3"]);
                        listaValorPecaEnviadaMes.TOTAL_3M4 = Convert.ToDecimal("0" + dataTableReader["TOTAL_3M4"]);

                        listaValorPecaEnviadaMes.TOTAL_METAS = Decimal.Round(ValorMETA, 2);

                        // Ajusta o cálculo para o próximo registro
                        divisor--;
                        if (divisor > 0)
                        {
                            ValorEnvioMensalPecas = ValorEnvioMensalPecas - (listaValorPecaEnviadaMes.TOTAL_3M1 + listaValorPecaEnviadaMes.TOTAL_3M2);
                            ValorMETA = ValorEnvioMensalPecas / divisor;
                        }

                        listasValorPecaEnviadaMes.Add(listaValorPecaEnviadaMes);
                    }
                }

                //for (int a = divisor; a > 0; a--)
                //{
                //    listasValorPecaEnviadaMes.Add(new Models.ListaValorPecaEnviadaMes
                //    {
                //        TOTAL_METAS = ValorMETA,
                //    });
                //    ValorMETA = ValorEnvioMensalPecas / a;
                //}

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("VALORPECAS", JsonConvert.SerializeObject(listasValorPecaEnviadaMes, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoTotalAtivo")]
        public IHttpActionResult ObterGraficoTotalAtivo(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaTotalAtivo> listaTotalAtivos = new List<Models.ListaTotalAtivo>();

            try
            {
                Models.ListaTotalAtivo listaVenda = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoTotalAtivo(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaVenda = new Models.ListaTotalAtivo();
                        listaVenda.ANO = Convert.ToInt32(dataTableReader["ANO"]);
                        listaVenda.TOTAL = Convert.ToDecimal(dataTableReader["TOTAL"]);

                        listaTotalAtivos.Add(listaVenda);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("ATIVO", JsonConvert.SerializeObject(listaTotalAtivos, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoAtendimento")]
        public IHttpActionResult ObterGraficoAtendimento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaAtendimento> listasAtendimentos = new List<Models.ListaAtendimento>();

            try
            {
                Models.ListaAtendimento listaAtendimento = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoAtendimento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaAtendimento = new Models.ListaAtendimento();
                        listaAtendimento.TITULO = dataTableReader["TITULO"].ToString();
                        listaAtendimento.TOTAL = Convert.ToInt64(dataTableReader["TOTAL"]);

                        listasAtendimentos.Add(listaAtendimento);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("ATENDIMENTO", JsonConvert.SerializeObject(listasAtendimentos, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoTrocaPecaMes")]
        public IHttpActionResult ObterGraficoTrocaPecaMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaTrocaPeca> listasTrocaPecas = new List<Models.ListaTrocaPeca>();

            try
            {
                Models.ListaTrocaPeca listaTrocaPeca = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoTrocaPecaMes(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaTrocaPeca = new Models.ListaTrocaPeca();
                        listaTrocaPeca.DS_MES = dataTableReader["DS_MES"].ToString();
                        listaTrocaPeca.TOTAL = Convert.ToInt64(dataTableReader["TOTAL"]);
                        listaTrocaPeca.QT_HORAS = Convert.ToInt64(dataTableReader["QT_HORAS"]);

                        listasTrocaPecas.Add(listaTrocaPeca);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("PECA", JsonConvert.SerializeObject(listasTrocaPecas, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoAtendimentoTecnicoRegional")]
        public IHttpActionResult ObterGraficoAtendimentoTecnicoRegional(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaAtendimentoRegional> listasAtendimentosRegionais = new List<Models.ListaAtendimentoRegional>();

            try
            {
                Models.ListaAtendimentoRegional listaAtendimentoRegional = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoAtendimentoTecnicoRegional(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaAtendimentoRegional = new Models.ListaAtendimentoRegional();
                        listaAtendimentoRegional.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                        //listaAtendimentoRegional.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        listaAtendimentoRegional.tecnico.NM_REDUZIDO = dataTableReader["NM_REDUZIDO"].ToString();
                        listaAtendimentoRegional.TOTAL_VISITA_REALIZADO = Convert.ToInt64(dataTableReader["TOTAL_VISITA_REALIZADO"]);
                        listaAtendimentoRegional.TOTAL_VISITA_PLANEJADO = Convert.ToInt64(dataTableReader["TOTAL_VISITA_PLANEJADO"]);

                        listasAtendimentosRegionais.Add(listaAtendimentoRegional);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("ATENDIMENTO", JsonConvert.SerializeObject(listasAtendimentosRegionais, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoPeriodoRealizadoMes")]
        public IHttpActionResult ObterGraficoPeriodoRealizadoMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaPeriodoRealizado> listaPeriodosRealizados = new List<Models.ListaPeriodoRealizado>();
            Int64 TOTAL_KAT = 0;
            try
            {
                Models.ListaPeriodoRealizado listaPeriodoRealizado = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoPeriodoRealizadoMes(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaPeriodoRealizado = new Models.ListaPeriodoRealizado();
                        listaPeriodoRealizado.DS_MES = dataTableReader["DS_MES"].ToString();
                        listaPeriodoRealizado.TOTAL = Convert.ToInt64(dataTableReader["QT_PERIODOS"]);
                        TOTAL_KAT = Convert.ToInt64(dataTableReader["TOTAL_KAT"]);

                        listaPeriodosRealizados.Add(listaPeriodoRealizado);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                // Define a META inicial
                Int64 META = TOTAL_KAT / 12;
                int A = 0;

                foreach (var x in listaPeriodosRealizados)
                {
                    if (A > 0)
                    {
                        // Se o atingido ficar abaixo da meta
                        if (META > x.TOTAL)
                        {
                            META = META + ((META - x.TOTAL) / (12 - A));
                        }
                        // Se o atingido ficar acima da meta
                        else
                        {
                            META = META - ((META - x.TOTAL) / (12 - A));
                        }
                    }
                    x.META = META;
                    A++;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("PERIODO", JsonConvert.SerializeObject(listaPeriodosRealizados, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoTipoEnvioEquipamento")]
        public IHttpActionResult ObterGraficoTipoEnvioEquipamento(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaTipoSolicitacao> listaTiposSolicitacao = new List<Models.ListaTipoSolicitacao>();

            try
            {
                Models.ListaTipoSolicitacao listTipoSolicitacao = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoTipoEnvioEquipamento(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listTipoSolicitacao = new Models.ListaTipoSolicitacao();
                        listTipoSolicitacao.wfTipoSolicitacao.ID_TIPO_SOLICITACAO = Convert.ToInt64(dataTableReader["ID_TIPO_SOLICITACAO"]);
                        listTipoSolicitacao.wfTipoSolicitacao.DS_TIPO_SOLICITACAO = dataTableReader["DS_TIPO_SOLICITACAO"].ToString();
                        listTipoSolicitacao.TOTAL = Convert.ToInt64(dataTableReader["TOTAL"]);

                        listaTiposSolicitacao.Add(listTipoSolicitacao);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("TIPOSOLICITACAO", JsonConvert.SerializeObject(listaTiposSolicitacao, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoEnvioEquipamentoLinhaProd")]
        public IHttpActionResult ObterGraficoEnvioEquipamentoLinhaProd(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaEnvioEquipamentoLinhaProduto> ListaEnvioEquipamentosLinhasProdutos = new List<Models.ListaEnvioEquipamentoLinhaProduto>();

            try
            {
                Models.ListaEnvioEquipamentoLinhaProduto listaEnvioEquipamentoLinhaProduto = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoEnvioEquipamentoLinhaProd(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaEnvioEquipamentoLinhaProduto = new Models.ListaEnvioEquipamentoLinhaProduto();
                        listaEnvioEquipamentoLinhaProduto.CD_MES = Convert.ToInt32(dataTableReader["CD_MES"]);
                        listaEnvioEquipamentoLinhaProduto.DS_MES = dataTableReader["DS_MES"].ToString();
                        //listaEnvioEquipamentoLinhaProduto.TOTAL_UNITIZACAO = Convert.ToInt64("0" + dataTableReader["TOTAL_UNITIZACAO"].ToString());
                        listaEnvioEquipamentoLinhaProduto.TOTAL_IDENTIFICACAO = Convert.ToInt64("0" + dataTableReader["TOTAL_IDENTIFICACAO"].ToString());
                        listaEnvioEquipamentoLinhaProduto.TOTAL_FECHADOR = Convert.ToInt64("0" + dataTableReader["TOTAL_FECHADOR"].ToString());

                        ListaEnvioEquipamentosLinhasProdutos.Add(listaEnvioEquipamentoLinhaProduto);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("LISTAENVIO", JsonConvert.SerializeObject(ListaEnvioEquipamentosLinhasProdutos, Formatting.None));
            return Ok(pecasJO);
        }

        [HttpPost]
        [Route("ObterGraficoMaquinaEnviadaDevolvidaMes")]
        public IHttpActionResult ObterGraficoMaquinaEnviadaDevolvidaMes(string CD_GRUPO, string CLIENTE, string CD_MODELO, string TECNICO, Int64? nidUsuario = null, Int64? CD_VENDEDOR = null, Int64? CD_LINHA_PRODUTO = null)
        {
            List<Models.ListaMaquinaEnviadaDevolvidaMes> listaMaquinasEnviadasDevolvidasMes = new List<Models.ListaMaquinaEnviadaDevolvidaMes>();

            try
            {
                Models.ListaMaquinaEnviadaDevolvidaMes listaMaquinaEnviadaDevolvidaMes = null;

                DataTableReader dataTableReader = new DashboardData().ObterGraficoMaquinaEnviadaDevolvidaMes(CD_GRUPO, CLIENTE, CD_MODELO, TECNICO, nidUsuario, CD_VENDEDOR, CD_LINHA_PRODUTO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        listaMaquinaEnviadaDevolvidaMes = new Models.ListaMaquinaEnviadaDevolvidaMes();
                        listaMaquinaEnviadaDevolvidaMes.CD_MES = Convert.ToInt32(dataTableReader["CD_MES"]);
                        listaMaquinaEnviadaDevolvidaMes.DS_MES = dataTableReader["DS_MES"].ToString();
                        listaMaquinaEnviadaDevolvidaMes.TOTAL_ENVIO = Convert.ToInt64("0" + dataTableReader["TOTAL_ENVIO"].ToString());
                        listaMaquinaEnviadaDevolvidaMes.TOTAL_DEVOLUCAO = Convert.ToInt64("0" + dataTableReader["TOTAL_DEVOLUCAO"].ToString());

                        listaMaquinasEnviadasDevolvidasMes.Add(listaMaquinaEnviadaDevolvidaMes);
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
                return BadRequest(ex.Message);
            }

            JObject pecasJO = new JObject();
            pecasJO.Add("LISTAMAQUINA", JsonConvert.SerializeObject(listaMaquinasEnviadasDevolvidasMes, Formatting.None));
            return Ok(pecasJO);
        }
    }
}