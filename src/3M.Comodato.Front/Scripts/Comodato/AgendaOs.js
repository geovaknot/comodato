var casasDecimais = 0;

jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    OcultarCampo($('#validaTpStatusVisitaOS'));
    OcultarCampo($('#validaCDAtivoFixo'));

    OcultarCampo($('#validaDT_DATA_LOG_OS'));
    OcultarCampo($('#validaDT_DATA_LOG_OS_HORA'));

    OcultarCampo($('#validaid_item_pedido_Peca'));
    OcultarValidacoesPeca();

    OcultarCampo($('#validaid_item_pedido_Pendencia'));
    OcultarValidacoesPendencia();

    $('#DT_DATA_LOG_OS').mask('00/00/0000');
    $('#DT_DATA_LOG_OS_HORA').mask('00:00:00');

    var ID_AGENDA = $("#agenda_ID_AGENDA").val();

    if (ID_AGENDA == "" || ID_AGENDA == "0" || ID_AGENDA == 0) {
        AlertaRedirect("Aviso", "Agenda inválida ou não informada!", "window.history.back();");
        return;
    }

    carregarComboAtivosCliente(false);
    carregarDados();
    carregarGridMVCPeca();
    carregarGridMVCPendencia();
});

$('#DT_DATA_LOG_OS-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});
 
function carregarDados() {
    var ID_AGENDA = $("#agenda_ID_AGENDA").val();
    var ID_OS = $("#ID_OS").val();

    if (ID_AGENDA == "" || ID_AGENDA == "0" || ID_AGENDA == 0) {
        AlertaRedirect("Aviso", "Agenda inválida ou não informada!", "window.history.back();");
        return;
    }

    carregarAgenda(ID_AGENDA);
    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0) {
        carregarOSINICIAR(ID_OS);
    } else {
        carregarOS(ID_OS);
    }
}

function carregarComboAtivosCliente(NaoAtendidos) {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var ID_VISITA = $("#visita_ID_VISITA").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLAPI + "AtivoAPI/";
    if (NaoAtendidos == false)
        URL = URL + "ObterListaAtivoCliente?CD_Cliente=" + CD_CLIENTE + "&SomenteATIVOSsemDTDEVOLUCAO=true";
    else
        URL = URL + "ObterListaAtivoClienteNaoAtendidos?CD_Cliente=" + CD_CLIENTE + "&SomenteATIVOSsemDTDEVOLUCAO=true&ID_VISITA=" + ID_VISITA;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.listaAtivosClientes != null) {
                LoadAtivosCliente(res.listaAtivosClientes);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadAtivosCliente(listaAtivosClientesJO) {
    LimparCombo($("#ativoFixo_CD_ATIVO_FIXO"));

    var listaAtivosClientes = JSON.parse(listaAtivosClientesJO);

    for (i = 0; i < listaAtivosClientes.length; i++) {
        MontarCombo($("#ativoFixo_CD_ATIVO_FIXO"), listaAtivosClientes[i].CD_ATIVO_FIXO, listaAtivosClientes[i].DS_ATIVO_FIXO + ' - ' + listaAtivosClientes[i].DT_ULTIMA_MANUTENCAO);
    }
}

function carregarAgenda(ID_AGENDA) {
    var URL = URLAPI + "AgendaAPI/Obter?ID_AGENDA=" + ID_AGENDA;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.agenda != null) {
                LoadAgenda(res.agenda);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadAgenda(agenda) {
    var NM_CLIENTE = agenda.cliente.NM_CLIENTE + " (" + agenda.cliente.CD_CLIENTE + ") " + agenda.cliente.EN_CIDADE + " - " + agenda.cliente.EN_ESTADO;
    $("#cliente_NM_CLIENTE").val(NM_CLIENTE);

    $("#tecnico_NM_TECNICO").val(agenda.tecnico.NM_TECNICO);
    $("#tecnico_empresa_NM_Empresa").val(agenda.tecnico.empresa.NM_Empresa);
}

function carregarOSINICIAR(ID_OS) {
    var URL = URLAPI + "OSAPI/Incluir";
    var ID_VISITA = $("#visita_ID_VISITA").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        AlertaRedirect("Aviso", "Visita inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var osEntity = {
        ID_OS: 0,
        DT_DATA_ABERTURA: new Date($.now()),
        tpStatusVisitaOS: {
            ST_TP_STATUS_VISITA_OS: statusNova,
        },
        ativoFixo: {
            CD_ATIVO_FIXO: null,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        visita: {
            ID_VISITA: ID_VISITA,
        },
        TP_MANUTENCAO: statusTpManutencaoPreventia,
        DS_OBSERVACAO: $("#DS_OBSERVACAO").val(),
        DS_NOME_RESPONSAVEL: $("#DS_NOME_RESPONSAVEL").val(),
        nidUsuarioAtualizacao: nidUsuario
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(osEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.ID_OS != null) {
                carregarOS(res.ID_OS);
            }

        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function carregarOS(ID_OS) {
    var URL = URLAPI + "OSAPI/Obter?ID_OS=" + ID_OS;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res != null) {
                LoadOS(res);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadOS(res) {
    if (res.OS != null) {
        $("#ID_OS").val(res.OS.ID_OS);
        $("#ativoFixo_CD_ATIVO_FIXO").val(res.OS.ativoFixo.CD_ATIVO_FIXO);
        if (res.OS.ativoFixo.CD_ATIVO_FIXO != '' && res.OS.ativoFixo.CD_ATIVO_FIXO != null)
            $("#ativoFixo_CD_ATIVO_FIXO").prop("disabled", true);
        else
            carregarComboAtivosCliente(true);
        $("#tpStatusVisitaOSAtual_ST_TP_STATUS_VISITA_OS").val(res.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
        $("#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS").val(res.OS.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS);
        $("#DS_OBSERVACAO").val(res.OS.DS_OBSERVACAO);
    }

    var classColorStatus = "";

    if (res.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusNova) {
        classColorStatus = "col-12 alert alert-success";
    }
    else if (res.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusAberta) {
        classColorStatus = "col-12 alert alert-success";
    }
    else if (res.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusFinalizada) {
        classColorStatus = "col-12 alert alert-primary";
    }
    else if (res.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusPausada) {
        classColorStatus = "col-12 alert alert-warning";
    }
    else if (res.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusPendente) {
        classColorStatus = "col-12 alert alert-warning";
    }
    else if (res.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusCancelada) {
        classColorStatus = "col-12 alert alert-danger";
    }
    else {
        classColorStatus = "col-12 alert alert-info";
    }

    $('#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').removeClass();
    $('#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').addClass(classColorStatus);

    if (res.preenchimentoOS != null) {
        $("#ID_OS_Formatado").val('Preenchimento OS: #' + res.preenchimentoOS.ID_OS_Formatado);
        $("#DT_DATA_ABERTURA").val(res.preenchimentoOS.DT_DATA_ABERTURA);
        $("#HR_INICIO").val(res.preenchimentoOS.HR_INICIO);
        $("#DT_DATA_ABERTURA_FIM").val(res.preenchimentoOS.DT_DATA_ABERTURA_FIM);
        $("#HR_FIM").val(res.preenchimentoOS.HR_FIM);
        $("#HR_TOTAL").val(res.preenchimentoOS.HR_TOTAL);
    }
    definirFluxoStatusOS();
    carregarHistorico();
}

function definirFluxoStatusOS() {
    var ST_TP_STATUS_VISITA_OS = parseInt($("#tpStatusVisitaOSAtual_ST_TP_STATUS_VISITA_OS").val());
    var statusCarregar = '';

    LimparCombo($("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS"));

    if (ST_TP_STATUS_VISITA_OS == statusNova) {
        statusCarregar = statusAberta;
    } else if (ST_TP_STATUS_VISITA_OS == statusAberta) {
        statusCarregar = statusPausada + ',' + statusFinalizada + ',' + statusCancelada + ',' + statusPendente;
    } else if (ST_TP_STATUS_VISITA_OS == statusPausada) {
        statusCarregar = statusAberta;
    } else if (ST_TP_STATUS_VISITA_OS == statusFinalizada) {
        statusCarregar = statusCancelada + ',' + statusAberta + ',' + statusPendente;
        //statusCarregar = statusCancelada + ',' + statusConfirmada + ',' + statusAberta + ',' + statusPendente;
    } else if (ST_TP_STATUS_VISITA_OS == statusPendente) {
        statusCarregar = statusCancelada + ',' + statusFinalizada + ',' + statusAberta;
    } else {
        return;
    }

    var URL = URLAPI + "TpStatusVisitaOSAPI/ObterListaStatus?statusCarregar=" + statusCarregar + "&FL_STATUS_OS=S";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tiposStatusVisitaOS != null) {
                LoadTpStatusVisitaOS(res.tiposStatusVisitaOS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadTpStatusVisitaOS(tiposStatusVisitaOS) {
    for (i = 0; i < tiposStatusVisitaOS.length; i++) {
        MontarCombo($("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS"), tiposStatusVisitaOS[i].ST_TP_STATUS_VISITA_OS, tiposStatusVisitaOS[i].DS_TP_STATUS_VISITA_OS_ACAO);
    }
}

$('#btnHistorico').click(function () {
    carregarHistorico();
});

function carregarHistorico() {
    var ID_OS = $("#ID_OS").val();

    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0) {
        ID_OS = -1;
    }

    var URL = URLObterListaHistoricoVisitaOS + "?ID_OS=" + ID_OS;

    $.ajax({
        url: URL,
        processData: true,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.Status == "Success") {
                $('#gridmvcHistoricoOS').html(res.Html);
                $('#HR_TOTAL').val(res.TempoGastoTOTAL);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

$('#btnVoltar').click(function () {
    window.location = '../Agenda/EditarVisita?idKey=' + $("#idKeyInicial").val();
});

$('#btnGravar').click(function () {
    var URL = URLAPI + "OSAPI/Alterar";
    var ID_OS = $("#ID_OS").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_VISITA = $("#visita_ID_VISITA").val();
    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO option:selected").val();
    var TP_MANUTENCAO = $("#TP_MANUTENCAO option:selected").val();

    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0) {
        AlertaRedirect("Aviso", "OS inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        AlertaRedirect("Aviso", "Visita inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_ATIVO_FIXO == "" || CD_ATIVO_FIXO == "0" || CD_ATIVO_FIXO == 0) {
        ExibirCampo($('#validaCDAtivoFixo'));
        return;
    }

    var osEntity = {
        ID_OS: ID_OS,
        ativoFixo: {
            CD_ATIVO_FIXO: CD_ATIVO_FIXO,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        visita: {
            ID_VISITA: ID_VISITA,
        },
        TP_MANUTENCAO: TP_MANUTENCAO,
        DS_OBSERVACAO: $("#DS_OBSERVACAO").val(),
        nidUsuarioAtualizacao: nidUsuario
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(osEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != null && res.MENSAGEM != '')
                Alerta("Aviso", res.MENSAGEM);
            else
                Alerta("Aviso", MensagemGravacaoSucesso);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

});

$('#btnAcao').click(function () {
    var ID_OS = $("#ID_OS").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_VISITA = $("#visita_ID_VISITA").val();
    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO option:selected").val();
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS option:selected").val();

    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0) {
        AlertaRedirect("Aviso", "OS inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        AlertaRedirect("Aviso", "Visita inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_ATIVO_FIXO == "" || CD_ATIVO_FIXO == "0" || CD_ATIVO_FIXO == 0) {
        ExibirCampo($('#validaCDAtivoFixo'));
        return;
    }
    else if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0) {
        ExibirCampo($('#validaTpStatusVisitaOS'));
        return;
    }

    ConfirmarSimNao('Aviso', 'Confirma ação na OS?', 'btnAcaoConfirmada()');
});

function btnAcaoConfirmada() {
    var URL = URLAPI + "OSAPI/Alterar";
    var ID_OS = $("#ID_OS").val();
    
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_VISITA = $("#visita_ID_VISITA").val();
    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO option:selected").val();
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS option:selected").val();
    var TP_MANUTENCAO = $("#TP_MANUTENCAO option:selected").val();

    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0) {
        AlertaRedirect("Aviso", "OS inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        AlertaRedirect("Aviso", "Visita inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_ATIVO_FIXO == "" || CD_ATIVO_FIXO == "0" || CD_ATIVO_FIXO == 0) {
        ExibirCampo($('#validaCDAtivoFixo'));
        return;
    }
    else if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0) {
        ExibirCampo($('#validaTpStatusVisitaOS'));
        return;
    }

    var osEntity = {
        ID_OS: ID_OS,
        DT_DATA_ABERTURA: new Date($.now()),
        ativoFixo: {
            CD_ATIVO_FIXO: CD_ATIVO_FIXO,
        },
        tpStatusVisitaOS: {
            ST_TP_STATUS_VISITA_OS: ST_TP_STATUS_VISITA_OS,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        visita: {
            ID_VISITA: ID_VISITA,
        },
        TP_MANUTENCAO: TP_MANUTENCAO,
        DS_OBSERVACAO: $("#DS_OBSERVACAO").val(),
        nidUsuarioAtualizacao: nidUsuario
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(osEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != null && res.MENSAGEM != '') 
                Alerta("Aviso", res.MENSAGEM);
            else
                carregarOS(res.ID_OS);
            //else 
            //    Alerta("Aviso", MensagemGravacaoSucesso);
            
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

$("#ativoFixo_CD_ATIVO_FIXO").change(function () {
    OcultarCampo($('#validaCDAtivoFixo'));
});

$("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS").change(function () {
    OcultarCampo($('#validaTpStatusVisitaOS'));
});

$('#btnNovaPeca').click(function () {
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOSAtual_ST_TP_STATUS_VISITA_OS").val();

    if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0) {
        ST_TP_STATUS_VISITA_OS = "0";
    }

    if (parseInt(ST_TP_STATUS_VISITA_OS) != statusAberta) {
        Alerta("Aviso", "Só é permitido adicionar <strong>Nova Peça</strong> para OS com status <strong>ABERTA</strong>!");
        return false;
    }

    carregarComboPeca($("#pecaOS_peca_CD_PECA"));

    $("#pecaOS_ID_PECA_OS").val(0);
    $("#pecaOS_CD_TP_ESTOQUE_CLI_TEC").val(statusTpEstoqueUtilizadoIntermediario);
    $("#pecaOS_peca_QTD_ESTOQUE").val(0);
    $("#pecaOS_QT_PECA").val('');
    $("#pecaOS_peca_TX_UNIDADE").val('');
    $("#pecaOS_DS_OBSERVACAO").val('');

    OcultarCampo($('#validaid_item_pedido_Peca'));
    OcultarValidacoesPeca();
});

$('#btnNovaPendencia').click(function () {
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOSAtual_ST_TP_STATUS_VISITA_OS").val();

    if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0) {
        ST_TP_STATUS_VISITA_OS = "0";
    }

    if (parseInt(ST_TP_STATUS_VISITA_OS) != statusAberta) {
        Alerta("Aviso", "Só é permitido adicionar <strong>Nova Pendência</strong> para OS com status <strong>ABERTA</strong>!");
        return false;
    }

    carregarComboPeca($("#pendenciaOS_peca_CD_PECA"));

    $("#pendenciaOS_ID_PENDENCIA_OS").val(0);
    $("#pendenciaOS_ST_STATUS_PENDENCIA").val(statusTpPendenciaPendente);
    $("#pendenciaOS_ST_TP_PENDENCIA").val(statusPendenciaPeca);
    $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").val(statusTpEstoqueUtilizadoIntermediario);
    $("#pendenciaOS_peca_QTD_ESTOQUE").val(0);
    $("#pendenciaOS_QT_PECA").val('');
    $("#pendenciaOS_peca_TX_UNIDADE").val('');
    $("#pendenciaOS_DS_DESCRICAO").val('');

    var ID_OS_Formatado = $("#ID_OS_Formatado").val();
    ID_OS_Formatado = ID_OS_Formatado.replace('Preenchimento OS: ', '');

    $("#pendenciaOS_OS_ID_OS").val($("#ID_OS").val());
    $("#pendenciaOS_OS_ID_OS_Formatado").val(ID_OS_Formatado);

    $("#pendenciaOS_OS_DT_DATA_ABERTURA").val($("#DT_DATA_ABERTURA").val());
    $("#pendenciaOS_OS_tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS").val($("#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS").val());

    definirCorStatusOSPendencia(ST_TP_STATUS_VISITA_OS);

    OcultarCampo($('#validaid_item_pedido_Pendencia'));
    OcultarValidacoesPendencia();

    $("#pendenciaOS_peca_CD_PECA").prop("disabled", false);
    $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").prop("disabled", false);
    $("#pendenciaOS_QT_PECA").prop("disabled", false);

});

function definirCorStatusOSPendencia(ST_TP_STATUS_VISITA_OS) {
    var classColorStatus = "";

    if (ST_TP_STATUS_VISITA_OS == statusNova) {
        classColorStatus = "col-12 alert alert-success";
    }
    else if (ST_TP_STATUS_VISITA_OS == statusAberta) {
        classColorStatus = "col-12 alert alert-success";
    }
    else if (ST_TP_STATUS_VISITA_OS == statusFinalizada) {
        classColorStatus = "col-12 alert alert-primary";
    }
    else if (ST_TP_STATUS_VISITA_OS == statusConfirmada) {
        classColorStatus = "col-12 alert alert-primary";
    }
    else if (ST_TP_STATUS_VISITA_OS == statusPausada) {
        classColorStatus = "col-12 alert alert-danger";
    }
    else if (ST_TP_STATUS_VISITA_OS == statusPendente) {
        classColorStatus = "col-12 alert alert-danger";
    }
    else if (ST_TP_STATUS_VISITA_OS == statusCancelada) {
        classColorStatus = "col-12 alert alert-secondary";
    }
    else {
        classColorStatus = "col-12 alert alert-info";
    }

    $('#pendenciaOS_OS_tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').removeClass();
    $('#pendenciaOS_OS_tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').addClass(classColorStatus);

}

$('#btnSalvarPecaModal').click(function () {
    var URL = URLAPI + "PecaOSAPI/Incluir";
    var ID_PECA_OS = $("#pecaOS_ID_PECA_OS").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_PECA = $("#pecaOS_peca_CD_PECA option:selected").val();
    var QT_PECA;
    if (casasDecimais > 0)
        QT_PECA = $("#pecaOS_QT_PECA").maskMoney('unmasked')[0];
    else
        QT_PECA = $("#pecaOS_QT_PECA").val();

    //if (ID_PECA_OS == "" || ID_PECA_OS == "0" || ID_PECA_OS == 0) {
    //    URL = URL + "Incluir";
    //}
    //else {
    //    URL = URL + "Alterar";
    //}

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        ExibirCampo($('#validaid_item_pedido_Peca'));
        return false;
    }
    else if (QT_PECA == "" || QT_PECA == "0" || QT_PECA == 0) {
        ExibirCampo($('#validaQTPECA_Peca'));
        return false;
    }
    else if (parseFloat(QT_PECA) <= 0) {
        ExibirCampo($('#validaQTPECARange_Peca'));
        return false;
    }
    else if (parseFloat(QT_PECA) > 999999999) {
        ExibirCampo($('#validaQTPECARangeMax_Peca'));
        return false;
    }

    var pecaOSDetalhamentoEntity = {
        ID_PECA_OS: ID_PECA_OS,
        OS: {
            ID_OS: $("#ID_OS").val(),
        },
        peca: {
            CD_PECA: CD_PECA,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        QT_PECA_Formatado: QT_PECA,
        CD_TP_ESTOQUE_CLI_TEC: $("#pecaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val(),
        nidUsuarioAtualizacao: nidUsuario,
        DS_OBSERVACAO: $("#pecaOS_DS_OBSERVACAO").val()
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pecaOSDetalhamentoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != '' || res.MENSAGEM != '')
                Alerta("Aviso", res.MENSAGEM);
            else {
                Alerta("Aviso", MensagemGravacaoSucesso);
                carregarGridMVCPeca();
               
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

});

$('#btnSalvarPendenciaModal').click(function () {
    var URL = URLAPI + "PendenciaOSAPI/";
    var ID_PENDENCIA_OS = $("#pendenciaOS_ID_PENDENCIA_OS").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_PECA = $("#pendenciaOS_peca_CD_PECA option:selected").val();
    var ST_TP_PENDENCIA = $('#pendenciaOS_ST_TP_PENDENCIA option:selected').val();
    var CD_TP_ESTOQUE_CLI_TEC = $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val();
    var QT_PECA;

    if (ID_PENDENCIA_OS == "" || ID_PENDENCIA_OS == "0" || ID_PENDENCIA_OS == 0) {
        URL = URL + "Incluir";
    }
    else {
        URL = URL + "Alterar";
    }

    if (ST_TP_PENDENCIA == 'P') {
        if (casasDecimais > 0)
            QT_PECA = $("#pendenciaOS_QT_PECA").maskMoney('unmasked')[0];
        else
            QT_PECA = $("#pendenciaOS_QT_PECA").val();

        if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
            ExibirCampo($('#validaid_item_pedido_Pendencia'));
            return false;
        }
        else if (QT_PECA == "" || QT_PECA == "0" || QT_PECA == 0) {
            ExibirCampo($('#validaQTPECA_Pendencia'));
            return false;
        }
        else if (parseFloat(QT_PECA) <= 0) {
            ExibirCampo($('#validaQTPECARange_Pendencia'));
            return false;
        }
        else if (parseFloat(QT_PECA) > 999999999) {
            ExibirCampo($('#validaQTPECARangeMax_Pendencia'));
            return false;
        }
    }
    else {
        CD_PECA = null;
        CD_TP_ESTOQUE_CLI_TEC = null;
        QT_PECA = null;
    }

    var pendenciaOSEntity = {
        ID_PENDENCIA_OS: ID_PENDENCIA_OS,
        os: {
            ID_OS: $("#pendenciaOS_OS_ID_OS").val(),
        },
        DT_ABERTURA_Formatado: $("#pendenciaOS_OS_DT_DATA_ABERTURA").val(),
        DS_DESCRICAO: $("#pendenciaOS_DS_DESCRICAO").val(),
        peca: {
            CD_PECA: CD_PECA,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        QT_PECA_Formatado: QT_PECA,
        ST_STATUS_PENDENCIA: $("#pendenciaOS_ST_STATUS_PENDENCIA option:selected").val(),
        ST_TP_PENDENCIA: $("#pendenciaOS_ST_TP_PENDENCIA option:selected").val(),
        CD_TP_ESTOQUE_CLI_TEC: CD_TP_ESTOQUE_CLI_TEC,
        nidUsuarioAtualizacao: nidUsuario
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pendenciaOSEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemGravacaoSucesso);
            carregarGridMVCPendencia();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

});

function carregarComboPeca(Obj) {
    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO option:selected").val();
    var URL = URLAPI + "PlanoZeroAPI/ObterListaModelo?CD_ATIVO_FIXO=" + CD_ATIVO_FIXO;
    //var URL = URLAPI + "PecaAPI/ObterListaAtivos";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            //if (res.PECA != null) {
            //    LoadPecas(Obj, res.PECA);
            //}
            if (res.PlanoZero != null) {
                LoadPecas(Obj, res.PlanoZero);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

//function LoadPecas(Obj, PecaJO) {
//    LimparCombo(Obj);

//    var listaPecas = JSON.parse(PecaJO);

//    for (i = 0; i < listaPecas.length; i++) {
//        MontarCombo(Obj, listaPecas[i].CD_PECA, listaPecas[i].DS_PECA);
//    }
//}

function LoadPecas(Obj, listaPecas) {
    LimparCombo(Obj);

    for (i = 0; i < listaPecas.length; i++) {
        MontarCombo(Obj, listaPecas[i].PecaModel.CD_PECA, listaPecas[i].PecaModel.DS_PECA);
    }
}

$("#pecaOS_peca_CD_PECA").change(function () {
    CarregarEstoquePeca();
});

$("#pecaOS_CD_TP_ESTOQUE_CLI_TEC").change(function () {
    CarregarEstoquePeca();
});

function CarregarEstoquePeca() {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_PECA = $("#pecaOS_peca_CD_PECA option:selected").val();
    var CD_TP_ESTOQUE_CLI_TEC = $("#pecaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val();

    OcultarCampo($('#validaid_item_pedido_Peca'));

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        $("#pecaOS_peca_QTD_ESTOQUE").val(0);
        $("#pecaOS_peca_TX_UNIDADE").val('');
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLAPI;
    if (CD_TP_ESTOQUE_CLI_TEC == 'C') {
        URL = URL + "EstoquePecaAPI/ObterCliente?CD_CLIENTE=" + $("#cliente_CD_CLIENTE").val() + "&CD_PECA=" + CD_PECA;
    }
    else {
        URL = URL + "EstoquePecaAPI/ObterTecnico?CD_TECNICO=" + CD_TECNICO + "&CD_PECA=" + CD_PECA;
    }

    //var token = sessionStorage.getItem("token");
    
    $.ajax({
        type: 'GET',
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
        complete: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.estoquePeca != null) {
                $("#pecaOS_peca_QTD_ESTOQUE").val(res.estoquePeca.QT_PECA_ATUAL);
                $("#pecaOS_peca_TX_UNIDADE").val(res.estoquePeca.peca.TX_UNIDADE);
                
                if (res.estoquePeca.peca.TX_UNIDADE == 'MT') {
                    $('#pecaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 3, allowZero: false });
                    casasDecimais = 3;
                }
                else {
                    $('#pecaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
                    casasDecimais = 0;
                }
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

    //if (CD_TP_ESTOQUE_CLI_TEC != statusTpEstoqueUtilizadoIntermediario) {
    //    $("#pecaOS_peca_QTD_ESTOQUE").val(0);
    //}
}

$('#pendenciaOS_ST_TP_PENDENCIA').change(function () {
    var ST_TP_PENDENCIA = $('#pendenciaOS_ST_TP_PENDENCIA option:selected').val();

    if (ST_TP_PENDENCIA == 'P')
        ExibirCampo($('#tipoPeca'));
    else
        OcultarCampo($('#tipoPeca'));
});

$("#pendenciaOS_peca_CD_PECA").change(function () {
    CarregarEstoquePecaPendencia();
});

$("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").change(function () {
    CarregarEstoquePecaPendencia();
});

function CarregarEstoquePecaPendencia() {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_PECA = $("#pendenciaOS_peca_CD_PECA option:selected").val();
    var CD_TP_ESTOQUE_CLI_TEC = $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val();

    OcultarCampo($('#validaid_item_pedido_Pendencia'));

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        $("#pendenciaOS_peca_QTD_ESTOQUE").val(0);
        $("#pendenciaOS_peca_TX_UNIDADE").val('');
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLAPI + "EstoquePecaAPI/ObterTecnico?CD_TECNICO=" + CD_TECNICO + "&CD_PECA=" + CD_PECA;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.estoquePeca != null) {
                $("#pendenciaOS_peca_QTD_ESTOQUE").val(res.estoquePeca.QT_PECA_ATUAL)
                $("#pendenciaOS_peca_TX_UNIDADE").val(res.estoquePeca.peca.TX_UNIDADE);

                if (res.estoquePeca.peca.TX_UNIDADE == 'MT') {
                    $('#pendenciaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 3, allowZero: false });
                    casasDecimais = 3;
                }
                else {
                    $('#pendenciaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
                    casasDecimais = 0;
                }

            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

    if (CD_TP_ESTOQUE_CLI_TEC != statusTpEstoqueUtilizadoIntermediario) {
        $("#pendenciaOS_peca_QTD_ESTOQUE").val(0);
    }
}

$("#pecaOS_QT_PECA").blur(function () {
    OcultarValidacoesPeca();
});

$("#pendenciaOS_QT_PECA").blur(function () {
    OcultarValidacoesPendencia();
});

$("#pecaOS_QT_PECA").keypress(function () {
    OcultarValidacoesPeca();
});

$("#pendenciaOS_QT_PECA").keypress(function () {
    OcultarValidacoesPendencia();
});

function carregarGridMVCPendencia() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var ID_OS = $("#ID_OS").val();
    var URL = URLObterListaPendenciaOS + "?CD_CLIENTE=" + CD_CLIENTE + "&ID_OS=" + ID_OS;

    $.ajax({
        type: 'GET',
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
            if (res.Status == "Success") {
                $('#gridmvcPendenciaOS').html(res.Html);
            }
            //if (res.listaTecnicoXClienteEscalas != null) {
            //    LoadTecnicoXClienteEscalas(res.listaTecnicoXClienteEscalas);
            //}
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function carregarGridMVCPeca() {
    var ID_OS = $("#ID_OS").val();
    var URL = URLObterListaPecaOS + "?ID_OS=" + ID_OS;

    $.ajax({
        type: 'GET',
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
            if (res.Status == "Success") {
                $('#gridmvcPecaOS').html(res.Html);
            }
            //if (res.listaTecnicoXClienteEscalas != null) {
            //    LoadTecnicoXClienteEscalas(res.listaTecnicoXClienteEscalas);
            //}
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function ExcluirPendenciaOS(ID_PENDENCIA_OS) {
    ConfirmarSimNao('Aviso', 'Confirma a <strong>FINALIZAÇÃO</strong> da Pendência?', 'ExcluirPendenciaOSConfirmada(' + ID_PENDENCIA_OS + ')');
}

function ExcluirPendenciaOSConfirmada(ID_PENDENCIA_OS) {
    var URL = URLAPI + "PendenciaOSAPI/Finalizar";

    if (ID_PENDENCIA_OS == "" || ID_PENDENCIA_OS == "0" || ID_PENDENCIA_OS == 0) {
        Alerta("Aviso", "Pendência inválida ou não informada!");
        return;
    }

    var pendenciaOSEntity = {
        ID_PENDENCIA_OS: ID_PENDENCIA_OS,
        nidUsuarioAtualizacao: nidUsuario
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pendenciaOSEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", 'Pendência finalizada com sucesso!');
            carregarGridMVCPendencia();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

function EditarPendenciaOS(ID_PENDENCIA_OS) {
    var URL = URLAPI + "PendenciaOSAPI/Obter?ID_PENDENCIA_OS=" + ID_PENDENCIA_OS;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.pendenciaOS != null) {
                LoadPendenciaOS(res);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadPendenciaOS(res) {
    carregarComboPeca($("#pendenciaOS_peca_CD_PECA"));

    if (res.pendenciaOS != null) {
        $("#pendenciaOS_ID_PENDENCIA_OS").val(res.pendenciaOS.ID_PENDENCIA_OS);
        $("#pendenciaOS_ST_STATUS_PENDENCIA").val(res.pendenciaOS.ST_STATUS_PENDENCIA);
        $("#pendenciaOS_ST_TP_PENDENCIA").val(res.pendenciaOS.ST_TP_PENDENCIA).trigger('change');
        $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").val(res.pendenciaOS.CD_TP_ESTOQUE_CLI_TEC);
        $("#pendenciaOS_peca_CD_PECA").val(res.pendenciaOS.peca.CD_PECA);
        $("#pendenciaOS_DS_DESCRICAO").val(res.pendenciaOS.DS_DESCRICAO);
        $("#pendenciaOS_OS_ID_OS").val(res.pendenciaOS.OS.ID_OS);
        $("#pendenciaOS_OS_tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS").val(res.pendenciaOS.OS.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS);
    }

    if (res.pendenciaOSModel != null) {
        $("#pendenciaOS_OS_ID_OS_Formatado").val(res.pendenciaOSModel.OS.ID_OS_Formatado);
        $("#pendenciaOS_OS_DT_DATA_ABERTURA").val(res.pendenciaOSModel.OS.DT_DATA_ABERTURA);
        $("#pendenciaOS_QT_PECA").val(res.pendenciaOSModel.QT_PECA);
    }

    definirCorStatusOSPendencia(res.pendenciaOS.OS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
    CarregarEstoquePecaPendencia();

    $("#pendenciaOS_peca_CD_PECA").prop("disabled", true);
    $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").prop("disabled", true);
    $("#pendenciaOS_QT_PECA").prop("disabled", true);

    $('#PendenciaModal').modal('show');
}

function OcultarValidacoesPeca() {
    OcultarCampo($('#validaQTPECA_Peca'));
    OcultarCampo($('#validaQTPECARange_Peca'));
    OcultarCampo($('#validaQTPECARangeMax_Peca'));
}

function OcultarValidacoesPendencia() {
    OcultarCampo($('#validaQTPECA_Pendencia'));
    OcultarCampo($('#validaQTPECARange_Pendencia'));
    OcultarCampo($('#validaQTPECARangeMax_Pendencia'));
}

function ExcluirPecaOS(ID_PECA_OS) {
    ConfirmarSimNao('Aviso', 'Confirma a <strong>EXCLUSÃO</strong> da Peça?', 'ExcluirPecaOSConfirmada(' + ID_PECA_OS + ')');
}

function ExcluirPecaOSConfirmada(ID_PECA_OS) {
    var URL = URLAPI + "PecaOSAPI/Excluir";
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (ID_PECA_OS == "" || ID_PECA_OS == "0" || ID_PECA_OS == 0) {
        Alerta("Aviso", "Peça inválida ou não informada!");
        return;
    }

    var pecaOSDetalhamentoEntity = {
        ID_PECA_OS: ID_PECA_OS,
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        nidUsuarioAtualizacao: nidUsuario
    };
    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pecaOSDetalhamentoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemExclusaoSucesso);
            carregarGridMVCPeca();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });


}

function AlterarLogStatusOS(ID_LOG_STATUS_OS) {
    var URL = URLAPI + "LogStatusOSAPI/Obter?ID_LOG_STATUS_OS=" + ID_LOG_STATUS_OS;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            if (res != null) {
                LoadLogStatusOS(res);
            }
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadLogStatusOS(res) {
    $('#ST_TP_STATUS_VISITA_OS_LOG').val(res.logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
    $('#DS_TP_STATUS_VISITA_OS').val(res.logStatusOS.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS);
    $('#ID_LOG_STATUS_OS').val(res.logStatusOS.ID_LOG_STATUS_OS);
    $('#DT_DATA_LOG_OS').val(res.logStatusOS.DT_DATA_LOG_OS_Formatado);
    $('#DT_DATA_LOG_OS_HORA').val(res.logStatusOS.DT_DATA_LOG_OS_HORA_Formatado);

    OcultarCampo($('#validaDT_DATA_LOG_OS'));
    OcultarCampo($('#validaDT_DATA_LOG_OS_HORA'));
}

$('#btnFecharModalHistoricoOS').click(function () {
    LimparCamposModalHistoricoOS();
});

function LimparCamposModalHistoricoOS() {
    $('#DS_TP_STATUS_VISITA_OS').val('');
    $('#ID_LOG_STATUS_OS').val('');
    $('#DT_DATA_LOG_OS').val('');
    $('#DT_DATA_LOG_OS_HORA').val('');

    OcultarCampo($('#validaDT_DATA_LOG_OS'));
    OcultarCampo($('#validaDT_DATA_LOG_OS_HORA'));
}

$('#btnGravarLogOS').click(function () {
    var URL = URLAPI + "LogStatusOSAPI/Alterar";
    var ID_OS = $("#ID_OS").val();
    var ST_TP_STATUS_VISITA_OS_LOG = $('#ST_TP_STATUS_VISITA_OS_LOG').val();
    var DS_TP_STATUS_VISITA_OS = $('#DS_TP_STATUS_VISITA_OS').val();
    var ID_LOG_STATUS_OS = $('#ID_LOG_STATUS_OS').val();
    var DT_DATA_LOG_OS = $('#DT_DATA_LOG_OS').val();
    var DT_DATA_LOG_OS_HORA = $('#DT_DATA_LOG_OS_HORA').val();

    if (ID_LOG_STATUS_OS == "" || ID_LOG_STATUS_OS == "0" || ID_LOG_STATUS_OS == 0) {
        Alerta("Aviso", "Nenhum Histórico selecionado para alteração!");
        return false;
    }
    else if (DT_DATA_LOG_OS == "") {
        ExibirCampo($('#validaDT_DATA_LOG_OS'));
        return false;
    }
    else if (DT_DATA_LOG_OS_HORA == "") {
        ExibirCampo($('#validaDT_DATA_LOG_OS_HORA'));
        return false;
    }

    var logStatusOS = {
        os: {
            ID_OS: ID_OS
        },
        tpStatusVisitaOS: {
            ST_TP_STATUS_VISITA_OS: ST_TP_STATUS_VISITA_OS_LOG
        },
        ID_LOG_STATUS_OS: ID_LOG_STATUS_OS,
        DT_DATA_LOG_OS_Formatado: DT_DATA_LOG_OS,
        DT_DATA_LOG_OS_HORA_Formatado: DT_DATA_LOG_OS_HORA,
        nidUsuarioAtualizacao: nidUsuario
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(logStatusOS),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != null && res.MENSAGEM != '') {
                Alerta("Aviso", res.MENSAGEM);
                return false;
            }
            else {
                //carregarHistorico();
                carregarOS(ID_OS);
                LimparCamposModalHistoricoOS();
                Alerta("Aviso", MensagemGravacaoSucesso);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

});

$("#DT_DATA_LOG_OS").blur(function () {
    OcultarCampo($('#validaDT_DATA_LOG_OS'));
});

$("#DT_DATA_LOG_OS").keypress(function () {
    OcultarCampo($('#validaDT_DATA_LOG_OS'));
});

$("#DT_DATA_LOG_OS_HORA").blur(function () {
    OcultarCampo($('#validaDT_DATA_LOG_OS_HORA'));
});

$("#DT_DATA_LOG_OS_HORA").keypress(function () {
    OcultarCampo($('#validaDT_DATA_LOG_OS_HORA'));
});
