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
    [RoutePrefix("api/ModeloAPI")]
    [Authorize]
    public class ModeloAPIController : BaseAPIController
    {
        /// <summary>
        /// Obtem lista completa de Modelos Ativos
        /// </summary>
        /// <param name="modeloEntity"></param>
        /// <returns>List<ModeloEntity> modelos</returns>
        [HttpPost]
        [Route("ObterLista")]
        public IHttpActionResult ObterLista(ModeloEntity modeloEntity)
        {
            List<ModeloEntity> modelos = new List<ModeloEntity>();

            try
            {
                if (modeloEntity == null)
                    modeloEntity = new ModeloEntity();

                DataTableReader dataTableReader = new ModeloData().ObterLista(modeloEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        modeloEntity = new ModeloEntity();

                        CarregarModeloEntity(dataTableReader, modeloEntity);

                        modelos.Add(modeloEntity);
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

            JObject modelosJO = new JObject();
            modelosJO.Add("MODELO", JsonConvert.SerializeObject(modelos, Formatting.None));
            return Ok(modelosJO);

        }

        /// <summary>
        /// Transfere os dados do DataTableReader para ModeloEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="modeloEntity"></param>
        protected void CarregarModeloEntity(DataTableReader dataTableReader, ModeloEntity modeloEntity)
        {
            modeloEntity.CD_MODELO = Convert.ToString(dataTableReader["CD_MODELO"]);
            modeloEntity.DS_MODELO = Convert.ToString(dataTableReader["DS_MODELO"]);
            modeloEntity.CD_MOD_NR12 = Convert.ToString(dataTableReader["CD_MOD_NR12"]);
            modeloEntity.CD_GRUPO_MODELO = Convert.ToString(dataTableReader["CD_GRUPO_MODELO"]);

            if (dataTableReader["VL_COMPLEXIDADE_EQUIP"] != DBNull.Value)
                modeloEntity.VL_COMPLEXIDADE_EQUIP = Convert.ToDecimal(dataTableReader["VL_COMPLEXIDADE_EQUIP"]);

            if (dataTableReader["TP_EMPACOTAMENTO"] != DBNull.Value)
                modeloEntity.TP_EMPACOTAMENTO = Convert.ToString(dataTableReader["TP_EMPACOTAMENTO"]).Trim();

            if (dataTableReader["VL_COMP_MIN"] != DBNull.Value)
                modeloEntity.VL_COMP_MIN = Convert.ToDecimal(dataTableReader["VL_COMP_MIN"].ToString());

            if (dataTableReader["VL_COMP_MAX"] != DBNull.Value)
                modeloEntity.VL_COMP_MAX = Convert.ToDecimal(dataTableReader["VL_COMP_MAX"].ToString());

            if (dataTableReader["VL_LARG_MIN"] != DBNull.Value)
                modeloEntity.VL_LARG_MIN = Convert.ToDecimal(dataTableReader["VL_LARG_MIN"]);

            if (dataTableReader["VL_LARG_MAX"] != DBNull.Value)
                modeloEntity.VL_LARG_MAX = Convert.ToDecimal(dataTableReader["VL_LARG_MAX"]);

            if (dataTableReader["VL_ALTUR_MIN"] != DBNull.Value)
                modeloEntity.VL_ALTUR_MIN = Convert.ToDecimal(dataTableReader["VL_ALTUR_MIN"]);

            if (dataTableReader["VL_ALTUR_MAX"] != DBNull.Value)
                modeloEntity.VL_ALTUR_MAX = Convert.ToDecimal(dataTableReader["VL_ALTUR_MAX"]);

            if (dataTableReader["VL_LARG_CAIXA"] != DBNull.Value)
                modeloEntity.VL_LARG_CAIXA = Convert.ToDecimal(dataTableReader["VL_LARG_CAIXA"]);

            if (dataTableReader["VL_ALTUR_CAIXA"] != DBNull.Value)
                modeloEntity.VL_ALTUR_CAIXA = Convert.ToDecimal(dataTableReader["VL_ALTUR_CAIXA"]);

            if (dataTableReader["VL_COMP_CAIXA"] != DBNull.Value)
                modeloEntity.VL_COMP_CAIXA = Convert.ToDecimal(dataTableReader["VL_COMP_CAIXA"]);

            if (dataTableReader["VL_PESO_CUBADO"] != DBNull.Value)
                modeloEntity.VL_PESO_CUBADO = Convert.ToDecimal(dataTableReader["VL_PESO_CUBADO"]);

            if (dataTableReader["VL_PROJETADO"] != DBNull.Value)
                modeloEntity.VL_PROJETADO = Convert.ToDecimal(dataTableReader["VL_PROJETADO"]);

            if (dataTableReader["ID_CATEGORIA"] != DBNull.Value)
                modeloEntity.CATEGORIA.ID_CATEGORIA = Convert.ToInt64(dataTableReader["ID_CATEGORIA"]);

            if (dataTableReader["CD_LINHA_PRODUTO"] != DBNull.Value)
                modeloEntity.LINHA_PRODUTO.CD_LINHA_PRODUTO = Convert.ToInt32(dataTableReader["CD_LINHA_PRODUTO"]);

            if (dataTableReader["DS_CATEGORIA"] != DBNull.Value)
                modeloEntity.CATEGORIA.DS_CATEGORIA = dataTableReader["DS_CATEGORIA"].ToString();

            if (dataTableReader["DS_LINHA_PRODUTO"] != DBNull.Value)
                modeloEntity.LINHA_PRODUTO.DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString();
        }


        [HttpGet]
        [Route("ObterListaModeloSinc")]
        public IHttpActionResult ObterListaModeloSinc()
        {
            IList<ModeloSinc> listaModelo = new List<ModeloSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                ModeloData modeloData = new ModeloData();
                listaModelo = modeloData.ObterListaModeloSinc();

                JObject JO = new JObject();
                //JO.Add("MODELO", JsonConvert.SerializeObject(listaModelo, Formatting.None));
                JO.Add("MODELO", JArray.FromObject(listaModelo));
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
