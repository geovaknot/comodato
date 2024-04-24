using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3M.Comodato.Entity;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace _3M.Comodato.Data
{
    public class StatusVisitaOSData
    {
        readonly Database _db;
        public StatusVisitaOSData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IList<StatusVisitaOSEntity> ObterListaStatusVisitaOS()
        {
            try
            {

                IList<StatusVisitaOSEntity> listaStatusVisitaOS = new List<StatusVisitaOSEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SELECT * FROM tbTPStatusVisitaOS ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        //cmd.Parameters.Add("@ID_ENDERECO", System.Data.SqlDbType.BigInt).Value = _IDAdress;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            StatusVisitaOSEntity statusVisita = new StatusVisitaOSEntity();
                            statusVisita.ID_TP_STATUS_VISITA_OS = Convert.ToInt32(SDR["ID_TP_STATUS_VISITA_OS"] is DBNull ? 0 : SDR["ID_TP_STATUS_VISITA_OS"]);
                            statusVisita.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(SDR["ST_TP_STATUS_VISITA_OS"] is DBNull ? 0 : SDR["ST_TP_STATUS_VISITA_OS"]);
                            statusVisita.DS_TP_STATUS_VISITA_OS = Convert.ToString(SDR["DS_TP_STATUS_VISITA_OS"] is DBNull ? "" : SDR["DS_TP_STATUS_VISITA_OS"].ToString());
                            statusVisita.FL_STATUS_OS = Convert.ToChar(SDR["FL_STATUS_OS"] is DBNull ? "" : SDR["FL_STATUS_OS"].ToString());
                            statusVisita.nidUsuarioAtualizacao = Convert.ToInt32(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);
                            statusVisita.dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"].ToString());
                            statusVisita.DS_TP_STATUS_VISITA_OS_ACAO = Convert.ToString(SDR["DS_TP_STATUS_VISITA_OS_ACAO"] is DBNull ? "" : SDR["DS_TP_STATUS_VISITA_OS_ACAO"].ToString());
                            listaStatusVisitaOS.Add(statusVisita);
                        }
                        cnx.Close();
                        return listaStatusVisitaOS;
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
