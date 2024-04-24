jQuery(document).ready(function () {
    OcultarCampo($('#validaTpStatusVisitaOS'));

    OcultarCampo($('#validaDT_DATA_LOG_VISITA'));
    OcultarCampo($('#validaDT_DATA_LOG_VISITA_HORA'));

    $('#DT_DATA_LOG_VISITA').mask('00/00/0000');
    $('#DT_DATA_LOG_VISITA_HORA').mask('00:00:00');

    var ID_AGENDA = $("#agenda_ID_AGENDA").val();

    if (ID_AGENDA == "" || ID_AGENDA == "0" || ID_AGENDA == 0) {
        AlertaRedirect("Aviso", "Agenda inválida ou não informada!", "window.history.back();");
        return;
    }

    carregarDados();
});

$('#DT_DATA_LOG_VISITA-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

function carregarDados() {
    var ID_AGENDA = $("#agenda_ID_AGENDA").val();
    var ID_VISITA = $("#ID_VISITA").val();

    if (ID_AGENDA == "" || ID_AGENDA == "0" || ID_AGENDA == 0) {
        AlertaRedirect("Aviso", "Agenda inválida ou não informada!", "window.history.back();");
        return;
    }

    carregarAgenda(ID_AGENDA);
    if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        carregarVisitaINICIAR();
    } else {
        carregarVisita(ID_VISITA);
    }
}

function carregarAgenda(ID_AGENDA) {
    var URL = URLAPI + "AgendaAPI/Obter?ID_AGENDA=" + ID_AGENDA;

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

function carregarVisitaINICIAR() {
    var URL = URLAPI + "VisitaAPI/Incluir";
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var visitaEntity = {
        ID_VISITA: 0,
        DT_DATA_VISITA: new Date($.now()),
        tpStatusVisitaOS: {
            ST_TP_STATUS_VISITA_OS: statusNova,
        },
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        DS_OBSERVACAO: $("#DS_OBSERVACAO").val(),
        DS_NOME_RESPONSAVEL: $("#DS_NOME_RESPONSAVEL").val(),
        nidUsuarioAtualizacao: nidUsuario
    };


    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(visitaEntity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.ID_VISITA != null) {
                carregarVisita(res.ID_VISITA);
            }

        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function carregarVisita(ID_VISITA) {
    var URL = URLAPI + "VisitaAPI/Obter?ID_VISITA=" + ID_VISITA;

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
            if (res != null) {
                LoadVisita(res);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadVisita(res) {
    if (res.visita != null) {
        $("#ID_VISITA").val(res.visita.ID_VISITA);
        $("#tpStatusVisitaOSAtual_ST_TP_STATUS_VISITA_OS").val(res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
        $("#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS").val(res.visita.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS);
        $("#DS_OBSERVACAO").val(res.visita.DS_OBSERVACAO);
        $("#DS_NOME_RESPONSAVEL").val(res.visita.DS_NOME_RESPONSAVEL);
        $("#tecnico_NM_TECNICO").val(res.visita.tecnico.NM_TECNICO);

    }

    var classColorStatus = "";

    if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusNova) {
        classColorStatus = "col-12 alert alert-success";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusAberta) {
        classColorStatus = "col-12 alert alert-success";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusFinalizada) {
        classColorStatus = "col-12 alert alert-primary";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusConfirmada) {
        classColorStatus = "col-12 alert alert-primary";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusPausada) {
        classColorStatus = "col-12 alert alert-warning";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusPendente) {
        classColorStatus = "col-12 alert alert-warning";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusCancelada) {
        classColorStatus = "col-12 alert alert-danger";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusPortaria) {
        classColorStatus = "col-12 alert alert-warning";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusIntegracao) {
        classColorStatus = "col-12 alert alert-secondary";
    }
    else if (res.visita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusTreinamento) {
        classColorStatus = "col-12 alert alert-secondary";
    }
    else {
        classColorStatus = "col-12 alert alert-info";
    }

    $('#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').removeClass();
    $('#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').addClass(classColorStatus);

    if (res.visitaTecnica != null) {
        $("#DT_DATA_VISITA").val(res.visitaTecnica.DT_DATA_VISITA);
        $("#HR_INICIO").val(res.visitaTecnica.HR_INICIO);
        $("#DT_DATA_VISITA_FIM").val(res.visitaTecnica.DT_DATA_VISITA_FIM);
        $("#HR_FIM").val(res.visitaTecnica.HR_FIM);
        $("#HR_TOTAL").val(res.visitaTecnica.HR_TOTAL);
    }
    definirFluxoStatusVisita();
    carregarHistorico();
    remontarIdKey();
}

function definirFluxoStatusVisita() {
    var ST_TP_STATUS_VISITA_OS = parseInt($("#tpStatusVisitaOSAtual_ST_TP_STATUS_VISITA_OS").val());
    var statusCarregar = '';

    LimparCombo($("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS"));

    if (ST_TP_STATUS_VISITA_OS == statusNova) {
        statusCarregar = statusAberta + ',' + statusIntegracao + ',' + statusPortaria + ',' + statusTreinamento + ',' + statusConsultoria;
    } else if (ST_TP_STATUS_VISITA_OS == statusAberta) {
        statusCarregar = statusPausada + ',' + statusFinalizada + ',' + statusCancelada + ',' + statusPendente;
    } else if (ST_TP_STATUS_VISITA_OS == statusPortaria) {
        statusCarregar = statusAberta + ',' + statusIntegracao + ',' + statusCancelada;
    } else if (ST_TP_STATUS_VISITA_OS == statusIntegracao) {
        statusCarregar = statusAberta + ',' + statusFinalizada + ',' + statusCancelada;
    } else if (ST_TP_STATUS_VISITA_OS == statusPausada) {
        statusCarregar = statusAberta;
    } else if (ST_TP_STATUS_VISITA_OS == statusFinalizada) {
        statusCarregar = statusCancelada; //+ ',' + statusConfirmada;
    } else if (ST_TP_STATUS_VISITA_OS == statusPendente) {
        statusCarregar = statusCancelada + ',' + statusFinalizada + ',' + statusAberta;
    } else if (ST_TP_STATUS_VISITA_OS == statusTreinamento) {
        statusCarregar = statusAberta + ',' + statusFinalizada + ',' + statusCancelada;

    } else if (ST_TP_STATUS_VISITA_OS == statusConsultoria) {
        statusCarregar = statusAberta + ',' + statusFinalizada + ',' + statusCancelada;

    } else {
        return;
    }

    var URL = URLAPI + "TpStatusVisitaOSAPI/ObterListaStatus?statusCarregar=" + statusCarregar + "&FL_STATUS_OS=N";

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

$('#btnGravarSuperior').click(function () {
    Gravar();
});

$('#btnGravarInferior').click(function () {
    Gravar();
});

function Gravar() {
    var URL = URLAPI + "VisitaAPI/Alterar";
    var ID_VISITA = $("#ID_VISITA").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
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

    var visitaEntity = {
        ID_VISITA: ID_VISITA,
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        DS_OBSERVACAO: $("#DS_OBSERVACAO").val(),
        DS_NOME_RESPONSAVEL: $("#DS_NOME_RESPONSAVEL").val(),
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(visitaEntity),
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != null && res.MENSAGEM != '') {
                Alerta("Aviso", res.MENSAGEM);
                carregarVisita(ID_VISITA);
            }
            else
                Alerta("Aviso", MensagemGravacaoSucesso);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

$('#btnAcao').click(function (event) {
    event.preventDefault();
    var el = $(this);
    el.prop('disabled', true);

    var ID_VISITA = $("#ID_VISITA").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS option:selected").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        el.prop('disabled', false); 
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        el.prop('disabled', false); 
        return;
    }
    else if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        AlertaRedirect("Aviso", "Visita inválida ou não informada!", "window.history.back();");
        el.prop('disabled', false); 
        return;
    }
    else if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0) {
        ExibirCampo($('#validaTpStatusVisitaOS'));
        el.prop('disabled', false); 
        return;
    }

    el.prop('disabled', false); 
    ConfirmarSimNao('Aviso', 'Confirma ação na visita?', 'btnAcaoConfirmada()');
});

function btnAcaoConfirmada() {
    var URL = URLAPI + "VisitaAPI/Alterar";
    var ID_VISITA = $("#ID_VISITA").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS option:selected").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
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
    else if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0) {
        ExibirCampo($('#validaTpStatusVisitaOS'));
        return;
    }

    var visitaEntity = {
        ID_VISITA: ID_VISITA,
        DT_DATA_VISITA: new Date($.now()),
        tpStatusVisitaOS: {
            ST_TP_STATUS_VISITA_OS: ST_TP_STATUS_VISITA_OS,
        },
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        DS_OBSERVACAO: $("#DS_OBSERVACAO").val(),
        DS_NOME_RESPONSAVEL: $("#DS_NOME_RESPONSAVEL").val(),
        nidUsuarioAtualizacao: nidUsuario
    };


    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(visitaEntity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != null && res.MENSAGEM != '')
                Alerta("Aviso", res.MENSAGEM);
            //else 
            //    Alerta("Aviso", MensagemGravacaoSucesso);
            carregarVisita(ID_VISITA);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}



$("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS").change(function () {
    OcultarCampo($('#validaTpStatusVisitaOS'));
});

$('#btnHistorico').click(function () {
    carregarHistorico();
});

function carregarHistorico() {
    var ID_VISITA = $("#ID_VISITA").val();

    if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        ID_VISITA = -1;
    }

    var URL = URLObterListaHistoricoVisita + "?ID_VISITA=" + ID_VISITA;

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
                $('#gridmvcHistoricoVisita').html(res.Html);
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

$('#btnNovaOS').click(function () {
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOSAtual_ST_TP_STATUS_VISITA_OS").val();
    var ExisteOSAberta = $("#ExisteOSAberta").val();

    if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0) {
        ST_TP_STATUS_VISITA_OS = "0";
    }

    if (parseInt(ST_TP_STATUS_VISITA_OS) != statusAberta) {
        Alerta("Aviso", "Só é permitido abrir <strong>Nova OS</strong> para visita com status <strong>ABERTA</strong>!");
        return;
    }
    else if (ExisteOSAberta == "S") {
        Alerta("Aviso", "Só pode haver <strong>UMA OS ABERTA</strong> em cada visita!");
        return;
    }

    window.location = '../Agenda/EditarOS?idKey=' + $("#idKey").val();
});

function remontarIdKey() {
    var ID_AGENDA = $("#agenda_ID_AGENDA").val();
    var ID_VISITA = $("#ID_VISITA").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_OS = $("#OS_ID_OS").val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    var URL = URLAPI + "VisitaAPI/RemontarIdKey?ID_AGENDA=" + ID_AGENDA + "&ID_VISITA=" + ID_VISITA + "&CD_CLIENTE=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO + "&ID_OS=" + ID_OS + "&tipoOrigemPagina=" + tipoOrigemPagina;

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
            if (res != null) {
                $("#idKey").val(res.idKey);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function AlterarLogStatusVisita(ID_LOG_STATUS_VISITA) {
    var URL = URLAPI + "LogStatusVisitaAPI/Obter?ID_LOG_STATUS_VISITA=" + ID_LOG_STATUS_VISITA;

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
            if (res.logStatusVisita != null) {
                LoadLogStatusVisita(res);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadLogStatusVisita(res) {
    $('#ST_TP_STATUS_VISITA_OS_LOG').val(res.logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
    $('#DS_TP_STATUS_VISITA_OS').val(res.logStatusVisita.tpStatusVisitaOS.DS_TP_STATUS_VISITA_OS);
    $('#ID_LOG_STATUS_VISITA').val(res.logStatusVisita.ID_LOG_STATUS_VISITA);
    $('#DT_DATA_LOG_VISITA').val(res.logStatusVisita.DT_DATA_LOG_VISITA_Formatado);
    $('#DT_DATA_LOG_VISITA_HORA').val(res.logStatusVisita.DT_DATA_LOG_VISITA_HORA_Formatado);

    OcultarCampo($('#validaDT_DATA_LOG_VISITA'));
    OcultarCampo($('#validaDT_DATA_LOG_VISITA_HORA'));
}

$('#btnFecharModalHistoricoVisita').click(function () {
    LimparCamposModalHistoricoVisita();
});

function LimparCamposModalHistoricoVisita() {
    $('#DS_TP_STATUS_VISITA_OS').val('');
    $('#ID_LOG_STATUS_VISITA').val('');
    $('#DT_DATA_LOG_VISITA').val('');
    $('#DT_DATA_LOG_VISITA_HORA').val('');

    OcultarCampo($('#validaDT_DATA_LOG_VISITA'));
    OcultarCampo($('#validaDT_DATA_LOG_VISITA_HORA'));
}

$('#btnGravarLogVisita').click(function () {
    var URL = URLAPI + "LogStatusVisitaAPI/Alterar";
    var ID_VISITA = $("#ID_VISITA").val();
    var ST_TP_STATUS_VISITA_OS_LOG = $('#ST_TP_STATUS_VISITA_OS_LOG').val();
    var DS_TP_STATUS_VISITA_OS = $('#DS_TP_STATUS_VISITA_OS').val();
    var ID_LOG_STATUS_VISITA = $('#ID_LOG_STATUS_VISITA').val();
    var DT_DATA_LOG_VISITA = $('#DT_DATA_LOG_VISITA').val();
    var DT_DATA_LOG_VISITA_HORA = $('#DT_DATA_LOG_VISITA_HORA').val();

    if (ID_LOG_STATUS_VISITA == "" || ID_LOG_STATUS_VISITA == "0" || ID_LOG_STATUS_VISITA == 0) {
        Alerta("Aviso", "Nenhum Histórico selecionado para alteração!");
        return false;
    }
    else if (DT_DATA_LOG_VISITA == "") {
        ExibirCampo($('#validaDT_DATA_LOG_VISITA'));
        return false;
    }
    else if (DT_DATA_LOG_VISITA_HORA == "") {
        ExibirCampo($('#validaDT_DATA_LOG_VISITA_HORA'));
        return false;
    }

    var logStatusVisita = {
        visita: {
            ID_VISITA: ID_VISITA
        },
        tpStatusVisitaOS: {
            ST_TP_STATUS_VISITA_OS: ST_TP_STATUS_VISITA_OS_LOG
        },
        ID_LOG_STATUS_VISITA: ID_LOG_STATUS_VISITA,
        DT_DATA_LOG_VISITA_Formatado: DT_DATA_LOG_VISITA,
        DT_DATA_LOG_VISITA_HORA_Formatado: DT_DATA_LOG_VISITA_HORA,
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(logStatusVisita),
        cache: false,
        async: false,
        beforeSend: function () {
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
                carregarVisita(ID_VISITA);
                LimparCamposModalHistoricoVisita();
                Alerta("Aviso", MensagemGravacaoSucesso);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

});

$("#DT_DATA_LOG_VISITA").blur(function () {
    OcultarCampo($('#validaDT_DATA_LOG_VISITA'));
});

$("#DT_DATA_LOG_VISITA").keypress(function () {
    OcultarCampo($('#validaDT_DATA_LOG_VISITA'));
});

$("#DT_DATA_LOG_VISITA_HORA").blur(function () {
    OcultarCampo($('#validaDT_DATA_LOG_VISITA_HORA'));
});

$("#DT_DATA_LOG_VISITA_HORA").keypress(function () {
    OcultarCampo($('#validaDT_DATA_LOG_VISITA_HORA'));
});
