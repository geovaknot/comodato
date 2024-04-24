using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/SQLQueryAPI")]
    [Authorize]
    public class SQLQueryAPIController : BaseAPIController
    {

        [HttpPost]
        [Route("ObterDados")]
        public IHttpActionResult ObterDados(string comandoSQL, string chave)
        {
            JObject jObject = new JObject();
            string HTML = string.Empty;

            if (chave != "JnFcW6TnQtV3K9XlkMdPJg==" ) //&& chave != "CJL5vD4Mer2VCayTPOrmiQ==")
            {
                jObject.Add("Dados", JsonConvert.SerializeObject("<span style='font-size: 10pt; color: red'><strong>ACESSO NEGADO!</strong></span>", Formatting.None));
                return Ok(jObject);
            }

            //if (chave == "JnFcW6TnQtV3K9XlkMdPJg==")
            //{
                if (comandoSQL.ToUpper().Contains("INSERT")
                    || comandoSQL.ToUpper().Contains("UPDATE")
                    || comandoSQL.ToUpper().Contains("DELETE")
                    || comandoSQL.ToUpper().Contains("DROP")
                    || comandoSQL.ToUpper().Contains("CREATE")
                    || comandoSQL.ToUpper().Contains("ALTER")
                    || comandoSQL.ToUpper().Contains("TRUNCATE")
                    || comandoSQL.ToUpper().Contains("#"))
                {
                    jObject.Add("Dados", JsonConvert.SerializeObject("<span style='font-size: 10pt; color: red'><strong>COMANDO NEGADO!</strong></span>", Formatting.None));
                    return Ok(jObject);
                }
            //}

            try
            {
                DataTable dataTable = new SQLQueryData().ObterDados(comandoSQL);

                if (dataTable.Rows.Count == 0)
                {
                    jObject.Add("Dados", JsonConvert.SerializeObject("<span style='font-size: 10pt; color: red'><strong>DADOS NÃO ENCONTRADOS!</strong></span>", Formatting.None));
                    return Ok(jObject);
                }

                HTML = "<table border='1' width='100%'>";
                HTML += "<tr>";
                DataRow rowTitulo = dataTable.Rows[0];
                for (int j = 0; j < rowTitulo.Table.Columns.Count; j++)
                {
                    HTML += "<td>";
                    HTML += rowTitulo.Table.Columns[j].ToString();
                    HTML += "</td>";
                }

                HTML += "</tr>";

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    HTML += "<tr>";

                    DataRow row = dataTable.Rows[i];
                    for (int j = 0; j < row.Table.Columns.Count; j++)
                    {
                        HTML += "<td>";

                        if (comandoSQL.ToUpper().Contains("SP_HELPTEXT"))
                            HTML += dataTable.Rows[i][j].ToString().Replace("\t", " ").Replace("\r\n", " ");
                        else
                            HTML += dataTable.Rows[i][j].ToString();

                        HTML += "</td>";
                    }

                    HTML += "</tr>";
                }
                HTML += "</table>";
            }
            catch (Exception ex)
            {
                if(ex.HResult == -2146233080)
                {
                    jObject.Add("Dados", JsonConvert.SerializeObject("<span style='font-size: 10pt; color: red'><strong>COMANDO EXECUTADO COM SUCESSO!</strong></span>", Formatting.None));
                }
                else
                {
                    LogUtility.LogarErro(ex);
                    jObject.Add("Dados", JsonConvert.SerializeObject("<span style='font-size: 10pt; color: red'><strong>" + ex.Message + "</strong></span>", Formatting.None));

                }
                //return BadRequest(ex.Message);
                return Ok(jObject);
            }

            jObject.Add("Dados", JsonConvert.SerializeObject(HTML, Formatting.None));
            return Ok(jObject);
        }
    }
}