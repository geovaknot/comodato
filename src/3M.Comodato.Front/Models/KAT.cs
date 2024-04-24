using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Controllers;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace _3M.Comodato.Front.Models
{
    public class KAT
    {
        public int Periodos { get; set; }
        public decimal QtdPeriodosRealizados { get; set; }

        public KAT ObterKatPorCliente(int clienteId)
        {
            //var kats = new List<ListaAgendaAtendimento>();
            var Kat = new KAT();
            try
            {
                //var tecnicos = new List<TecnicoClienteEntity>();
                ////var cliente = new ClienteData().ObterKatPorCliente(clienteId);
                //var cliente = new ClienteData().ObterPorId(clienteId);
                //Kat.Periodos = cliente.QT_PERIODO;

                //var dataTableReader = new TecnicoClienteData().ObterLista(new TecnicoClienteEntity()

                //{
                //    cliente = new ClienteEntity()
                //    {
                //        CD_CLIENTE = clienteId
                //    },
                //    CD_ORDEM = 1
                //}).CreateDataReader();

                //if (dataTableReader.HasRows)
                //{
                //    while (dataTableReader.Read())
                //    {
                //        var tecnicoClienteEntity = new TecnicoClienteEntity();
                //        tecnicoClienteEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                //        tecnicoClienteEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                //        tecnicoClienteEntity.cliente.CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]);
                //        tecnicoClienteEntity.cliente.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString();
                //        tecnicoClienteEntity.CD_ORDEM = Convert.ToInt32("0" + dataTableReader["CD_ORDEM"]);
                //        tecnicos.Add(tecnicoClienteEntity);
                //    }
                //}

                //if (tecnicos != null && tecnicos.Count() > 0)
                //{
                //    var obj = new AgendaController().ObterListaAgendaAtendimento(new AgendaEntity()
                //    {
                //        cliente = new ClienteEntity()
                //        {
                //            CD_CLIENTE = clienteId,
                //        },
                //        tecnico = new TecnicoEntity()
                //        {
                //            CD_TECNICO = tecnicos[0].tecnico.CD_TECNICO
                //        }
                //    }, null, null, null);


                //    Kat.QtdPeriodosRealizados = obj.Sum(x => x.QT_PERIODO_REALIZADO);
                //}

                DateTime vigenciaINICIAL = DateTime.MinValue;
                DateTime vigenciaFINAL = DateTime.MinValue;
                // Vigência
                try
                {
                    vigenciaINICIAL = Convert.ToDateTime(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.vigenciaINICIAL));
                }
                catch { }
                try
                {
                    vigenciaFINAL = Convert.ToDateTime(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.vigenciaFINAL) + " 23:59:59");
                }
                catch { }

                if (vigenciaINICIAL == DateTime.MinValue)
                    vigenciaINICIAL = Convert.ToDateTime("01/01/" + DateTime.Now.Year.ToString());
                if (vigenciaFINAL == DateTime.MinValue)
                    vigenciaFINAL = Convert.ToDateTime("31/12/" + DateTime.Now.Year.ToString() + " 23:59:59");

                //var dataTableReader = new KatTecnicoData().ObterRelatorio(vigenciaINICIAL, vigenciaFINAL, null, clienteId.ToString()).Tables[0].CreateDataReader();
                var dataTableReader = new KatTecnicoData().ObterByVisitaCliTec(vigenciaINICIAL, vigenciaFINAL, null, clienteId,0).Tables[0].CreateDataReader();

                
                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (dataTableReader["KatAno"] != DBNull.Value)
                        {
                            Kat.Periodos = Convert.ToInt32(dataTableReader["KatAno"]);
                        }
                        if (dataTableReader["KatRealizado"] != DBNull.Value)
                        {
                            Kat.QtdPeriodosRealizados += Convert.ToDecimal(dataTableReader["KatRealizado"]);
                        }
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
            }

            return Kat;
        }
    }
}