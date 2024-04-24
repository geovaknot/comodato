using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class TpEstoqueMoviData
    {
        readonly Database _db;
        //DbCommand dbCommand;

        public TpEstoqueMoviData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        /// <summary>
        /// Obtem lista de Tipos de Movimentações de um estoque
        /// </summary>
        /// <returns></returns>  
        public IList<TpEstoqueMoviSinc> ObterListaTpEstoqueMoviSinc()
        {
            try
            {
                IList<TpEstoqueMoviSinc> listaTpEstoqueMovi = new List<TpEstoqueMoviSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select em.* from tbtpestoquemovi em ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            TpEstoqueMoviSinc tpEstoqueMovi = new TpEstoqueMoviSinc();
                            tpEstoqueMovi.ID_TP_ESTOQUE_MOVI= Convert.ToInt64(SDR["ID_TP_ESTOQUE_MOVI"].ToString());
                            tpEstoqueMovi.CD_TP_MOVIMENTACAO= Convert.ToString(SDR["CD_TP_MOVIMENTACAO"] is DBNull ? " " : SDR["CD_TP_MOVIMENTACAO"].ToString());
                            tpEstoqueMovi.DS_TP_MOVIMENTACAO= Convert.ToString(SDR["DS_TP_MOVIMENTACAO"] is DBNull ? "" : SDR["DS_TP_MOVIMENTACAO"].ToString());
                            tpEstoqueMovi.DS_NOME_REDUZ= Convert.ToString(SDR["DS_NOME_REDUZ"] is DBNull ? "" : SDR["DS_NOME_REDUZ"].ToString());
                            listaTpEstoqueMovi.Add(tpEstoqueMovi);
                        }
                        cnx.Close();
                        return listaTpEstoqueMovi;
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
