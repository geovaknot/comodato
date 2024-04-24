$().ready(function () {
    LimparScrollWorkflow();
    // DefinirExibicaoAtual();
    CarregarSituacaoStatus();
    DefinirFluxoStatusRR();
    CarregarGRIDMensagem();
   // DefinirEdicaoCampos();
    DefiniMascara();


    //alert($("#HabilitaImprimir").val());

    if ($("#HabilitaImprimir").val() == "True") {
        $("#btnImprimirRelatorio").prop('disabled', false);
    }
    else {
        $("#btnImprimirRelatorio").prop('disabled', true);
    }
    //OcultarCampo($('#btnSalvarRascunho'));
    $("#btnSalvarSubmeter").prop('value', 'Salvar');

    var ST_STATUS_RR = parseInt($('#ST_STATUS_RR').val());


    $("#NM_Fornecedor").blur(function () {
        SalvarInformacaoAdicional("Fornecedor");
    });


    $("#Custo_Peca").blur(function () {
        var Custo_Peca = $("#Custo_Peca").val();
        SalvarInformacaoAdicional("custoPeca");
    });

    $("#Vl_Mao_Obra").blur(function () {
        var Vl_Mao_Obra = $("#Vl_Mao_Obra").val();
        SalvarInformacaoAdicional("vlMaodeObra");
    });

    $("#TEMPO_ATENDIMENTO").blur(function () {

        SalvarInformacaoAdicional("tempo");
    });

    $("#VL_Hora_Atendimento").blur(function () {

        SalvarInformacaoAdicional("tempo");
    });

    $("#VL_Minuto_Atendimento").blur(function () {

        SalvarInformacaoAdicional("tempo");
    });

   

});

$("#NM_Fornecedor").blur(function () {
    SalvarInformacaoAdicional("Fornecedor");
});



$('#DT_RETIRADA_AGENDADA-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_RETIRADA_REALIZADA-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_PROGRAMADA_TMS-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_DEVOLUCAO_3M-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_DEVOLUCAO_PLANEJAMENTO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#btnNovoHistorico').click(function () {
    $("#acompanhamentoReclamacao_rrComent_DS_COMENT").val('');

    $('#HistoricoModal').modal({
        show: true
    })
});

$('#btnSalvarHistoricoModal').click(function () {
    var URL = URLAPI + "RRComentAPI/Incluir";
    var ID_RELATORIO_RECLAMACAO = $("#ID_RELATORIO_RECLAMACAO").val();
    //var ID_RR_COMMENT =  $("#acompanhamentoReclamacao_rrComent_ID_RR_COMMENT").val();
    var DS_COMENT = $("#acompanhamentoReclamacao_rrComent_DS_COMENT").val();

    DS_COMENT = $.trim(DS_COMENT);

    if (DS_COMENT == "" || DS_COMENT.length == 0) {
        return;
    }

    var rrComentEntity = {
        relatorioReclamacao: {
            ID_RELATORIO_RECLAMACAO: ID_RELATORIO_RECLAMACAO,
        },
        DS_COMENT: DS_COMENT,
        usuario: {
            nidUsuario: nidUsuario,
            cnmNome: cnmNome
        }
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(rrComentEntity),
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            //Alerta("Aviso", MensagemGravacaoSucesso);
            CarregarGRIDMensagem();
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }
    });

});

$("#acompanhamentoReclamacao.RRStatus.ST_STATUS_RR").change(function () {

    var ST_STATUS_RR = parseInt($("#acompanhamentoReclamacao_RRStatus_ST_STATUS_RR").val());
    OcultarCampo($('#validaSTSTATUSPEDIDO'));

    if ($("#acompanhamentoReclamacao_RRStatus_ST_STATUS_RR").val() == "")
        return;
   
});

$("#CD_GRUPO_RESPONS").change(function () {
    var CD_GRUPOWF = $("#CD_GRUPO_RESPONS").val();

    OcultarCampo($('#validaIDGRUPORESPONS'));
    OcultarCampo($('#validaIDUSUARIORESPONS'));

    if (CD_GRUPOWF == "") {
        return;
    }

    var URL = URLAPI + "WfGrupoUsuAPI/ObterLista";

    var wfGrupoUsuEntity = {
        grupoWf: {
            CD_GRUPOWF: CD_GRUPOWF
        }
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(wfGrupoUsuEntity),
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            if (res.grupos != null) {
                LoadGrupoResponsaveis(res.grupos);
            }
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }
    });

    AlterarGrupoResponsavel();
});

$("#ID_USUARIO_RESPONS").change(function () {
    OcultarCampo($('#validaIDUSUARIORESPONS'));

    AlterarGrupoResponsavel();
});


$("#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO").blur(function () {
    OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
});


//Fazer
$("#btnAcao").click(function () {

    //var URL = URLAPI + "WorkflowAPI/AlterarStatus";
    var ST_STATUS_RR = $("#acompanhamentoReclamacao_RRStatus_ST_STATUS_RR").val();
    var ID_RELATORIO_RECLAMACAO = $("#ID_WF_PEDIDO_EQUIP").val();
    OcultarCampo($('#validaSTSTATUSPEDIDO'));

    if (ST_STATUS_RR == "" || ST_STATUS_RR == "0" || ST_STATUS_RR == 0) {
        ExibirCampo($('#validaSTSTATUSPEDIDO'));
        return;
    }
    btnAcaoConfirmada();
});


//Fazer
function btnAcaoConfirmada() {
    var ST_STATUS_RR = $("#acompanhamentoReclamacao_RRStatus_ST_STATUS_RR").val();
    var ID_RELATORIO_RECLAMACAO = $("#ID_RELATORIO_RECLAMACAO").val();

    var URL = URLAPI + "RelatorioReclamacaoAPI/AtualizarRelatorioReclamacao";

    var relatorioReclamacaoEntity = {
        ID_RELATORIO_RECLAMACAO: ID_RELATORIO_RECLAMACAO,
        ST_STATUS_RR: ST_STATUS_RR,
        ID_USUARIO_RESPONS: nidUsuario
    };
    

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(relatorioReclamacaoEntity),
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#ST_STATUS_RR").val(ST_STATUS_RR);
        
            AlertaRedirect("Aviso", MensagemGravacaoSucesso, "window.location = '../RelatorioReclamacao';");
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }
    });

}


$('#btnImprimirRelatorio').click(function () {


    var status = $("#acompanhamentoReclamacao_RRStatus_ST_STATUS_RR").val();
    var ID_RELATORIO_RECLAMACAO = $("#ID_RELATORIO_RECLAMACAO").val();
   

    if (status == 'null' || status == null)
        status = '';

    if (ID_RELATORIO_RECLAMACAO == 'null' || ID_RELATORIO_RECLAMACAO == null)
        ID_RELATORIO_RECLAMACAO = '';

    var URL = URLCriptografarChave + "?Conteudo=" +
        status + "|" + ID_RELATORIO_RECLAMACAO;

    $.ajax({
        url: URL,
        processData: true,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            if (res.idKey != null && res.idKey != '') {
                window.open(URLSite + '/RelatorioReclamacaoCompras.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }

    });

});

function AlterarGrupoResponsavel() {
    var URL = URLAPI + "WorkflowAPI/AlterarGrupoResponsavel";
    //var ST_STATUS_PEDIDO = $("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val();
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    //var CD_Empresa = $('#acompanhamentoPedidoDevolucao_empresa_CD_Empresa').val();
    //var DT_RETIRADA_AGENDADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA').val();
    //var DT_RETIRADA_REALIZADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA').val();
    //var DT_PROGRAMADA_TMS = $('#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS').val();
    //var DT_DEVOLUCAO_3M = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M').val();
    //var DT_DEVOLUCAO_PLANEJAMENTO = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO').val();
    var ID_USUARIO_RESPONS = $('#ID_USUARIO_RESPONS').val();
    var CD_GRUPO_RESPONS = $('#CD_GRUPO_RESPONS').val().trim();

    var WfPedidoEquipEntity = {
        ID_WF_PEDIDO_EQUIP: ID_WF_PEDIDO_EQUIP,
        //ST_STATUS_PEDIDO: ST_STATUS_PEDIDO,
        //CD_Empresa: CD_Empresa,
        //DT_RETIRADA_AGENDADA_Formatada: DT_RETIRADA_AGENDADA,
        //DT_RETIRADA_REALIZADA_Formatada: DT_RETIRADA_REALIZADA,
        //DT_PROGRAMADA_TMS_Formatada: DT_PROGRAMADA_TMS,
        //DT_DEVOLUCAO_3M_Formatada: DT_DEVOLUCAO_3M,
        //DT_DEVOLUCAO_PLANEJAMENTO_Formatada: DT_DEVOLUCAO_PLANEJAMENTO,
        ID_USU_ULT_ATU: nidUsuario,
        ID_USUARIO_RESPONS: ID_USUARIO_RESPONS,
        CD_GRUPO_RESPONS: CD_GRUPO_RESPONS,
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(WfPedidoEquipEntity),
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            //$("#ST_STATUS_PEDIDO").val(ST_STATUS_PEDIDO);
            ////Alerta("Aviso", MensagemGravacaoSucesso);
            ////DefinirExibicaoAtual();
            //CarregarSituacaoStatus();
            //CarregarGRIDMensagem();
            //DefinirFluxoStatusPedido();
            //AlertaRedirect("Aviso", MensagemGravacaoSucesso, "window.location = '../Workflow';");
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }
    });

}

function LoadGrupoResponsaveis(gruposJO) {
    var posicionado = false;
    var grupos = JSON.parse(gruposJO);

    LimparCombo($("#ID_USUARIO_RESPONS"));

    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#ID_USUARIO_RESPONS"), grupos[i].usuario.nidUsuario, grupos[i].usuario.cnmNome);

        if (grupos.length == 1) {
            $('#ID_USUARIO_RESPONS').val(grupos[i].usuario.nidUsuario).trigger('change');
            posicionado = true;
        }
    }

    var hidID_USUARIO_RESPONS = $("#hidID_USUARIO_RESPONS").val();

    if (posicionado == false && hidID_USUARIO_RESPONS != 0) {
        $('#ID_USUARIO_RESPONS').val(hidID_USUARIO_RESPONS).trigger('change');
    }
}

//Defini os campos que serão exibidos
function DefinirEdicaoCampos() {
   

    var perfil = ($("#PerfilAnalista").val());

    

}


function DefiniMascara() {
    $('#Custo_Peca').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });
    $('#Vl_Mao_Obra').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });
    $('#Custo_Total').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });
    $("#VL_Hora_Atendimento").mask('00')
    $("#VL_Minuto_Atendimento").mask('00');
}
//Defini os status que irão aparecer no combo
function DefinirFluxoStatusRR() {
  //  alert($("#HabilitaCampo").val());
    if ($("#HabilitaCampo").val() =="True") {
        var ST_STATUS_RR = parseInt($("#ST_STATUS_RR").val());
        var ID_GRUPO_RESPONS = parseInt($("#ID_USUARIO_RESPONS").val());
        var statusCarregar = '';

        LimparCombo($("#acompanhamentoReclamacao_RRStatus_ST_STATUS_RR"));

        if (ST_STATUS_RR == TecnicoRegional) {
            statusCarregar = Novo + ',' + AnaliseTecnica + ',' + Reprovado;

        } else if (ST_STATUS_RR == AnaliseTecnica) {
            statusCarregar = TecnicoRegional + ',' + EmCompras + ',' + Reprovado;
        } else if (ST_STATUS_RR == EmCompras) {
            statusCarregar = Finalizado + ',' + EnviadoTecnicoCampo + ',' + Reprovado;
        } else if (ST_STATUS_RR == EnviadoTecnicoCampo) {
            statusCarregar = Finalizado;
        }
        else {
            return;
        }


        var URL = URLAPI + "RRStatusAPI/ObterListaStatus?statusCarregar=" + statusCarregar + "&ID_GRUPOWF=" + ID_GRUPO_RESPONS;
        //  var URL = URLAPI + "RRStatusAPI/ObterListaStatus?statusCarregar=" + statusCarregar;

        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            async: false,
            contentType: "application/json",
            //headers: { "Authorization": "Basic " + localStorage.token },
            //data: null,
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                if (res.tiposStatusRR != null) {
                    LoadStatusRR(res.tiposStatusRR);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });
    }
}

//Carrega a grid de historico de mensagem
function CarregarGRIDMensagem() {
    var ID_RELATORIO_RECLAMACAO = $("#ID_RELATORIO_RECLAMACAO").val();

    if (ID_RELATORIO_RECLAMACAO == "" || ID_RELATORIO_RECLAMACAO == "0" || ID_RELATORIO_RECLAMACAO == 0) {
        return;
    }

    var URL = URLObterListaMensagem + "?ID_RELATORIO_RECLAMACAO=" + ID_RELATORIO_RECLAMACAO;

    $.ajax({
        url: URL,
        processData: true,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            if (res.Status == "Success") {
                $('#gridmvcMensagem').html(res.Html);
            }
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }

    });

}

//Monta o combo de Status
function LoadStatusRR(tpStatusRR) {
    for (i = 0; i < tpStatusRR.length; i++) {
        MontarCombo($("#acompanhamentoReclamacao_RRStatus_ST_STATUS_RR"), tpStatusRR[i].ST_STATUS_RR, tpStatusRR[i].DS_TRANSICAO);
    }
}

//Define edicao dos campos dependendo do perfil
function DefinirExibicaoCampos() {


}
function CarregarGrupoResponsavel() {
    var ST_STATUS_PEDIDO = $("#ST_STATUS_PEDIDO").val();

    LimparCombo($("#CD_GRUPO_RESPONS"));

    var URL = URLAPI + "WfGrupoAPI/ObterListaByStatusPedido?ST_STATUS_PEDIDO=" + ST_STATUS_PEDIDO;

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            if (res.grupos != null) {
                LoadGrupos(res.grupos);
            }
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}


function SalvarInformacaoAdicional(tipo) {

    var Nome_Fornecedor = "";
    var TEMPO_ATENDIMENTO = "";
    var Custo_Peca = "";
    var Vl_Mao_Obra = "";
    var VL_Hora_Atendimento = "0";
    var VL_Minuto_Atendimento = "0";

    if(tipo =="Fornecedor")
        Nome_Fornecedor = $("#NM_Fornecedor").val();

    if (tipo == "tempo") {

        // recuperar o valor da Hora
        //recuperar o valor do Minuto
         VL_Hora_Atendimento = $("#VL_Hora_Atendimento").val();
         VL_Minuto_Atendimento = $("#VL_Minuto_Atendimento").val();

        TEMPO_ATENDIMENTO = $("#TEMPO_ATENDIMENTO").val();
    }
    if (tipo == "custoPeca") {
        Custo_Peca = FormatarValorJson($('#Custo_Peca').val());
    }

    if (tipo == "vlMaodeObra")
        Vl_Mao_Obra = FormatarValorJson($("#Vl_Mao_Obra").val());

    var ID_RELATORIO_RECLAMACAO = $("#ID_RELATORIO_RECLAMACAO").val();
    var ST_STATUS_RR = parseInt($('#ST_STATUS_RR').val());

    var URL = URLAPI + "RelatorioReclamacaoAPI/AtualizarRelatorioReclamacao";

    var relatorioReclamacaoEntity = {
        ID_RELATORIO_RECLAMACAO: ID_RELATORIO_RECLAMACAO,
        NM_Fornecedor: Nome_Fornecedor,
        Custo_Peca: Custo_Peca,
        ValorMaoDeObra: Vl_Mao_Obra,
        TEMPO_ATENDIMENTO: TEMPO_ATENDIMENTO,
        VL_Hora_Atendimento : VL_Hora_Atendimento,
        VL_Minuto_Atendimento : VL_Minuto_Atendimento,
        ID_USUARIO_RESPONS: nidUsuario,
        ST_STATUS_RR: ST_STATUS_RR
    };


    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(relatorioReclamacaoEntity),
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
           // Alerta("AVISO", res.responseText);
            //if (res.grupos != null) {
            //    LoadGrupos(res.grupos);
            //}
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });


}


function LoadGrupos(gruposJO) {
    var posicionado = false;
    var grupos = JSON.parse(gruposJO);

    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#CD_GRUPO_RESPONS"), grupos[i].CD_GRUPOWF, grupos[i].DS_GRUPOWF);

        if (grupos.length == 1) {
            $('#CD_GRUPO_RESPONS').val(grupos[i].CD_GRUPOWF).trigger('change');
            posicionado = true;
        }
    }

    var hidCD_GRUPO_RESPONS = $("#hidCD_GRUPO_RESPONS").val();

    if (posicionado == false && hidCD_GRUPO_RESPONS != "") {
        $('#CD_GRUPO_RESPONS').val(hidCD_GRUPO_RESPONS).trigger('change');
    }
}
