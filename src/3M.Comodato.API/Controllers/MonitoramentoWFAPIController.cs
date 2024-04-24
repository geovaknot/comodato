using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/MonitoramentoWFAPI")]
    [Authorize]
    public class MonitoramentoWFAPIController : BaseAPIController
    {
        /// <summary>
        /// Executa geração de relatório de equipamentos recolhidos
        /// </summary>
        ///<param name="TIPO"></param>
        ///<param name="DT_INICIAL"></param>
        ///<param name="DT_FINAL"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(MonitoramentoWFEntity monitoramentoWFEntity)
        {
            try
            {
                JObject JO = new JObject();
                JO.Add("STATUS", JsonConvert.SerializeObject(new MonitoramentoWFData().ObterLista(monitoramentoWFEntity), Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("CarregarMonitoramento")]
        public IHttpActionResult CarregarMonitoramento(string ini, string fim, string tipo)
        {
            DateTime inicio = Convert.ToDateTime( ini + " 00:00:00.000");
            DateTime final = Convert.ToDateTime(fim + " 23:59:59.999");

            MonitoramentoWFEntity monitoramento = new MonitoramentoWFEntity();
            monitoramento.DT_INICIAL = inicio;
            monitoramento.DT_FINAL = final;
            monitoramento.TIPO = tipo;
            DataTable dataTable = new MonitoramentoWFData().ObterLista(monitoramento);


            if (dataTable.Rows.Count < 1)
            {
                JObject JOo = new JObject();
                JOo.Add("Dados", JsonConvert.SerializeObject("<br /><font color='lightgray' size='3'>Não há dados</font>", Formatting.None));
                return Ok(JOo);
            }

            string html = "<table bgcolor='#FAFAFA' style='width:100%; border: 3px solid white;'>";
            try
            {
                //Para apresentar cabeçalho:
                html = html + "<tr bgcolor='white' style='width:100%; border: 3px solid white;'>";
                DataRow rowTitulo = dataTable.Rows[0];
                for (int j = 0; j < rowTitulo.Table.Columns.Count; j++)
                {
                    string cabecalho = rowTitulo.Table.Columns[j].ToString();

                    switch (cabecalho)
                    {
                        case "TP_PEDIDO":
                            cabecalho = "Tipo";
                            break;
                        //case "ST_STATUS_PEDIDO":
                        //    cabecalho = "Status(id)";
                        //    break;
                        case "DS_STATUS_NOME_REDUZ":
                            cabecalho = "Status";
                            break;
                        case "CD_GRUPOWF":
                            cabecalho = "Grupo";
                            break;
                        case "cnmNome":
                            cabecalho = "Responsável";
                            break;
                        case "TMA":
                            cabecalho = "Tempo Médio (hrs)";
                            break;
                        case "QTD_ATEND":
                            cabecalho = "Atendimentos";
                            break;
                    }

                    html = html + "<th>";

                    html = html + cabecalho;

                    html = html + "</th>";
                }

                html = html + "</tr>";

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    html = html + "<tr style='width:100%; border: 3px solid white;'>";

                    //html = html + "<td>" + i + "</td>";

                    DataRow row = dataTable.Rows[i];
                    for (int j = 0; j < row.Table.Columns.Count; j++)
                    {
                        string dado = dataTable.Rows[i][j].ToString();

                        if ((j == 0 || j == 1) && i > 0 && dado == dataTable.Rows[i - 1][j].ToString()) // || j == 2
                        {
                            dado = "";
                        }

                        if (dado == "0" || dado == "" || dado == null)
                        {
                            if (j == 4)
                                dado = "< 1";

                            if (j == 5)
                                dado = "";
                        }

                        if (j == 0 && dado == "E")
                        {
                            dado = "Envio";
                        } else if (j == 0 && dado == "D")
                        {
                            dado = "Devolução";
                        }                        

                        html = html + "<td style='padding:5px;'>";

                        html = html + dado;

                        html = html + "</td>";
                    }

                    html = html + "</tr>";
                }

                html = html + "</table>";
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject JO = new JObject();
            JO.Add("Dados", JsonConvert.SerializeObject(html, Formatting.None));
            return Ok(JO);
        }
    }
}