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
    [RoutePrefix("api/TecnicoAPI")]
    [Authorize]
    public class TecnicoAPIController : BaseAPIController
    {
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(string CD_TECNICO)
        {
            TecnicoEntity tecnicoEntity  = new TecnicoEntity();

            try
            {
                

                tecnicoEntity.CD_TECNICO = CD_TECNICO;
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        CarregarTecnicoEntity(dataTableReader, tecnicoEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tecnico = tecnicoEntity });
        }

        [HttpGet]
        [Route("ObterViaUsuario")]
        public HttpResponseMessage ObterViaUsuario(Int64 ID_USUARIO)
        {
            TecnicoEntity tecnicoEntity = new TecnicoEntity();

            try
            {
                tecnicoEntity.usuario.nidUsuario = ID_USUARIO;
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        CarregarTecnicoEntity(dataTableReader, tecnicoEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tecnico = tecnicoEntity });
        }

        /// <summary>
        /// Consulta de Técnicos
        /// </summary>
        /// <param name="tecnicoEntity"></param>
        /// <returns>List<TecnicoEntity> tecnicos</returns>
        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(TecnicoEntity tecnicoEntity)
        {
            List<TecnicoEntity> tecnicos = new List<TecnicoEntity>();

            try
            {
                if (tecnicoEntity == null)
                    tecnicoEntity = new TecnicoEntity();

                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tecnicoEntity = new TecnicoEntity();

                        CarregarTecnicoEntity(dataTableReader, tecnicoEntity);

                        tecnicos.Add(tecnicoEntity);
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

            //return Request.CreateResponse(HttpStatusCode.OK, new { tecnicos = tecnicos });
            JObject tecnicosJO = new JObject();
            tecnicosJO.Add("tecnicos", JsonConvert.SerializeObject(tecnicos, Formatting.None));
            return Ok(tecnicosJO);

        }

        /// <summary>
        /// Consulta de Técnicos ATIVOS (FL_ATIVO = S)
        /// </summary>
        /// <param name="tecnicoEntity"></param>
        /// <returns>List<TecnicoEntity> tecnicos</returns>
        [HttpPost]
        [Route("ObterListaAtivos")]
        public IHttpActionResult ObterListaAtivos(TecnicoEntity tecnicoEntity)
        {
            List<TecnicoEntity> tecnicos = new List<TecnicoEntity>();

            try
            {
                if (tecnicoEntity == null)
                    tecnicoEntity = new TecnicoEntity();

                tecnicoEntity.FL_ATIVO = "S";
                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tecnicoEntity = new TecnicoEntity();

                        CarregarTecnicoEntity(dataTableReader, tecnicoEntity);

                        tecnicos.Add(tecnicoEntity);
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

            //return Request.CreateResponse(HttpStatusCode.OK, new { tecnicos = tecnicos });
            JObject tecnicosJO = new JObject();
            tecnicosJO.Add("tecnicos", JsonConvert.SerializeObject(tecnicos, Formatting.None));
            return Ok(tecnicosJO);
        }

        /// <summary>
        /// Consulta os Técnicos via cliente
        /// </summary>
        /// <param name="CD_CLIENTE"></param>
        /// <returns>List<TecnicoClienteEntity> tecnicos</returns>
        [HttpGet]
        [Route("ObterListaByEscala")]
        public HttpResponseMessage ObterListaByEscala(int CD_CLIENTE)
        {
            List<TecnicoClienteEntity> tecnicos = new List<TecnicoClienteEntity>();

            try
            {
                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                DataTableReader dataTableReader = new TecnicoData().ObterListaEscala(CD_CLIENTE).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        TecnicoClienteEntity tecnicoClienteEntity = new TecnicoClienteEntity();
                        tecnicoClienteEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                        tecnicoClienteEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        tecnicos.Add(tecnicoClienteEntity);
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

            return Request.CreateResponse(HttpStatusCode.OK, new { tecnicos = tecnicos });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para TecnicoEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="tecnicoEntity"></param>
        protected void CarregarTecnicoEntity(DataTableReader dataTableReader, TecnicoEntity tecnicoEntity)
        {
            tecnicoEntity.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
            tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
            tecnicoEntity.EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString();
            tecnicoEntity.EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString();
            tecnicoEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
            tecnicoEntity.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();
            tecnicoEntity.EN_CEP = dataTableReader["EN_CEP"].ToString();
            tecnicoEntity.TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString();
            tecnicoEntity.TX_FAX = dataTableReader["TX_FAX"].ToString();
            tecnicoEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
            tecnicoEntity.TP_TECNICO = dataTableReader["TP_TECNICO"].ToString();
            tecnicoEntity.VL_CUSTO_HORA = Convert.ToDecimal("0" + dataTableReader["VL_CUSTO_HORA"]);
            tecnicoEntity.FL_ATIVO = dataTableReader["FL_ATIVO"].ToString();
            tecnicoEntity.usuarioCoordenador.nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO_COORDENADOR"]);
            tecnicoEntity.usuarioCoordenador.cnmNome = dataTableReader["cnmNomeCoordenador"].ToString() + " (" + dataTableReader["cdsLoginCoordenador"].ToString() + ")";
            tecnicoEntity.usuario.nidUsuario = Convert.ToInt64("0" + dataTableReader["ID_USUARIO"]);
            tecnicoEntity.usuario.cnmNome = dataTableReader["cnmNome"].ToString() + " (" + dataTableReader["cdsLogin"].ToString() + ")";
            tecnicoEntity.empresa.CD_Empresa = Convert.ToInt64("0" + dataTableReader["CD_Empresa"]);
            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
        }


        [HttpGet]
        [Route("ObterListaTecnicoSinc")]
        public IHttpActionResult ObterListaTecnicoSinc()
        {
            IList<TecnicoSinc> listaTecnico = new List<TecnicoSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                TecnicoData tecnicoData = new TecnicoData();
                listaTecnico = tecnicoData.ObterListaTecnicoSinc();

                JObject JO = new JObject();
                //JO.Add("TECNICO", JsonConvert.SerializeObject(listaTecnico, Formatting.None));
                JO.Add("TECNICO", JArray.FromObject(listaTecnico));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("ObterListaTecnicoCampo")]
        public IHttpActionResult ObterListaTecnicoCampo()
        {
            
            List<TecnicoEntity> listaTecnico = new List<TecnicoEntity>();
            TecnicoEntity tecnicoEntity = new TecnicoEntity();
            try
            {
                if (tecnicoEntity == null)
                    tecnicoEntity = new TecnicoEntity();
                tecnicoEntity.FL_ATIVO = "S";

                DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tecnicoEntity = new TecnicoEntity();

                        CarregarTecnicoEntity(dataTableReader, tecnicoEntity);

                        listaTecnico.Add(tecnicoEntity);
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


            JObject JO = new JObject();
         
            JO.Add("listaTecnico", JsonConvert.SerializeObject(listaTecnico, Formatting.None));
            return Ok(JO);

    

        }

    }
}