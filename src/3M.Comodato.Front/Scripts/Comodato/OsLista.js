jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    OcultarCampo($('#validaTecnico'));
    OcultarCampo($('#btnOrdenarOs'));
    OcultarCampo($('#btnNovaOs'));

    carregarComboTecnico();
    carregarComboCliente();
    carregarComboTipoOS();
    carregarComboStatusOS();
    carregarComboRegiao();
    ConfigurarCacheNavegador();
    carregarGridMVC();
});

function ConfigurarCacheNavegador() {

    if (localStorage['Os_tecnico_CD_TECNICO'] != undefined && localStorage['Os_tecnico_CD_TECNICO'] != "") {
        $('#tecnico_CD_TECNICO').val(localStorage['Os_tecnico_CD_TECNICO']).trigger('change');
    }

    if (localStorage['Os_TpStatusOS_ST_STATUS_OS'] != undefined && localStorage['Os_TpStatusOS_ST_STATUS_OS'] != "") {
        $('#TpStatusOS_ST_STATUS_OS').val(localStorage['Os_TpStatusOS_ST_STATUS_OS']).trigger('change');
    }

    if (localStorage['Os_regiao_CD_REGIAO'] != undefined && localStorage['Os_regiao_CD_REGIAO'] != "") {
        $('#regiao_CD_REGIAO').val(localStorage['Os_regiao_CD_REGIAO']).trigger('change');
    }

    if (localStorage['Os_TpOS_CD_TIPO_OS'] != undefined && localStorage['Os_TpOS_CD_TIPO_OS'] != "") {
        $('#TpOS_CD_TIPO_OS').val(localStorage['Os_TpOS_CD_TIPO_OS']).trigger('change');
    }

    if (localStorage['Os_cliente_CD_CLIENTE'] != undefined && localStorage['Os_cliente_CD_CLIENTE'] != "") {
        $('#cliente_CD_CLIENTE').val(localStorage['Os_cliente_CD_CLIENTE']).trigger('change');
    }
}

$('#btnLimpar').click(function () {

    OcultarCampo($('#validaTecnico'));

    if (UsuarioTecnico.toLowerCase() === 'false') {
        $('#tecnico_CD_TECNICO').val(null).trigger('change');
        localStorage.removeItem('Os_tecnico_CD_TECNICO');
    }

    $('#TpStatusOS_ST_STATUS_OS').val(null).trigger('change');
    localStorage.removeItem('Os_TpStatusOS_ST_STATUS_OS');

    $('#regiao_CD_REGIAO').val(null).trigger('change');
    localStorage.removeItem('Visita_regiao_CD_REGIAO');

    $("#TpOS_CD_TIPO_OS").val(null).trigger('change');
    localStorage.removeItem('Os_TpOS_CD_TIPO_OS');

    $("#cliente_CD_CLIENTE").val(null).trigger('change');
    localStorage.removeItem('Visita_cliente_CD_CLIENTE');

    $('#OS_ID_OS').val('');
    carregarGridMVC();
});

$('#btnFiltrar').click(function () {
    ObterListaOs();
});

$('#btnOrdenarOs').click(function () {
    ObterListaOs();
});

function ObterListaOs() {

    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if ((CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) && (UsuarioTecnico.toLowerCase() === 'true')) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    carregarGridMVC();

    localStorage['Os_tecnico_CD_TECNICO'] = $("#tecnico_CD_TECNICO option:selected").val();
    localStorage['Os_TpStatusOS_ST_STATUS_OS'] = $("#TpStatusOS_ST_STATUS_OS option:selected").val();
    localStorage['Os_regiao_CD_REGIAO'] = $("#regiao_CD_REGIAO option:selected").val();
    localStorage['Os_TpOS_CD_TIPO_OS'] = $("#TpOS_CD_TIPO_OS option:selected").val();
    localStorage['Os_cliente_CD_CLIENTE'] = $("#cliente_CD_CLIENTE option:selected").val();
}

$("#tecnico_CD_TECNICO").change(function () {
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    OcultarCampo($('#validaTecnico'));

    OcultarCampo($('#gridmvc'));
    OcultarCampo($('#btnOrdenarOs'));
    OcultarCampo($('#btnNovaOs'));
    if ((UsuarioTecnico.toLowerCase() !== 'true') && (CD_TECNICO != "" || CD_TECNICO != "0" || CD_TECNICO != 0)) {
        OcultarCampo($('#btnNovaOs'));
    }
    $('#btnNovaOs').attr("href", "OsPadrao/Incluir?CD_TECNICO=" + $("#tecnico_CD_TECNICO").val()+"");
});

function carregarComboStatusOS() {
    var URL = URLAPI + "TpStatusOSPadraoAPI/ObterLista?statusOSPadraoEntity.ST_STATUS_OS=0";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.STATUS_OS != null) {
                LoadTpStatusOS(res.STATUS_OS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadTpStatusOS(statusOs) {
    LimparCombo($("#TpStatusOS_ST_STATUS_OS"));

    for (i = 0; i < statusOs.length; i++) {
        MontarCombo($("#TpStatusOS_ST_STATUS_OS"), statusOs[i].ST_STATUS_OS, statusOs[i].DS_STATUS_OS);
    }
}

function checarDatas(DT_INICIAL, DT_FINAL) {
    var data_1 = new Date(formatarData(DT_INICIAL));
    var data_2 = new Date(formatarData(DT_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        Alerta("Aviso", "A data inicial não pode ser maior que a data final!");
        return false;
    }
    //} else if (data_1 > data) {
    //    Alerta("Aviso", "A data inicial não pode ser posterior a hoje!");
    //    return false;

    //} else if (data_2 > data) {
    //    Alerta("Aviso", "A data final não pode ser posterior a hoje!");
    //    return false;
    //}

    return true;
}

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}

$('#DT_DATA-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

function carregarGridMVC() {
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var ST_STATUS_OS = $("#TpStatusOS_ST_STATUS_OS option:selected").val();
    var CD_TIPO_OS = $("#TpOS_CD_TIPO_OS option:selected").val();
    var CD_REGIAO = $("#regiao_CD_REGIAO option:selected").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();

    if ((CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) && (UsuarioTecnico.toLowerCase() === 'true'))
        return;

    var DT_INICIAL = $('#DT_INICIAL').val();
    var DT_FINAL = $('#DT_FINAL').val();

    var periodoValido = checarDatas(DT_INICIAL, DT_FINAL);

    var URL = URLObterLista + "?CD_TECNICO=" + CD_TECNICO + "&CD_REGIAO=" + CD_REGIAO + "&ST_STATUS_OS=" + ST_STATUS_OS + 
        "&CD_CLIENTE=" + CD_CLIENTE + "&DT_INICIAL=" + DT_INICIAL + "&DT_FINAL=" + DT_FINAL;

    if (periodoValido) {
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
                $("#loader").css("display", "none");
                if (res != null) {
                    ExibirCampo($('#gridmvc'));
                    $('#gridmvc').html(res.Html);
                    ExibirCampo($('#btnOrdenarOs'));
                    if (CD_TECNICO != "" && CD_TECNICO != "0" && CD_TECNICO != 0) {
                        ExibirCampo($('#btnNovaOs'));
                    }
                    else {
                        OcultarCampo($('#btnNovaOs'));
                    }

                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", res.responseText);
            }
        });

        ConfigurarOrdenacaoListaOS();
    }
}

function ConfigurarOrdenacaoListaOS() {

    const ordenacaoAscendente = "asc";
    const ordenacaoDescendente = "desc";

    ordenacaoListaOs = ordenacaoListaOs == ordenacaoDescendente ? ordenacaoAscendente : ordenacaoDescendente;
} 

function carregarComboTipoOS() {
    var URL = URLAPI + "TpOSPadraoAPI/ObterLista?tipoOSPadraoEntity.CD_TIPO_OS=0";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
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
                LoadTipoOS(res.TIPO_OS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadTipoOS(tipos) {
    LimparCombo($("#TpOS_CD_TIPO_OS"));

    for (i = 0; i < tipos.length; i++) {
        MontarCombo($("#TpOS_CD_TIPO_OS"), tipos[i].CD_TIPO_OS, tipos[i].DS_TIPO_OS);
    }
}

$('#btnNovaOs').click(function (event) {
   
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    const urlVerificaOS = URLPermiteIncluirOsAberta + "?codTecnico=" + CD_TECNICO;

    $.ajax({
        type: 'GET',
        url: urlVerificaOS,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        processData: true,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            if (res.Iniciar != null || res.Aberta != null) {
                event.preventDefault();
                if (res.Iniciar == "TemOS") {
                    var mensagem = "Existe OS com status <strong>AGUARDANDO INICIO</strong> em atendimento.";
                }
                if (res.Aberta == "TemOS") {
                    let mensagemAberta = "Existe OS com status <strong>ABERTA</strong> em atendimento.";
                    mensagem = (mensagem != "") && (typeof mensagem != "undefined") ? mensagem + "<br><br>" + mensagemAberta : mensagemAberta;
                }

                if (res.Iniciar == "TemVisita") {
                    var mensagem = "Existe Visita com status <strong>INICIAR</strong> em atendimento.";
                }
                if (res.Aberta == "TemVisita") {
                    let mensagemAberta = "Existe Visita com status <strong>ABERTA</strong> em atendimento.";
                    mensagem = (mensagem != "") && (typeof mensagem != "undefined") ? mensagem + "<br><br>" + mensagemAberta : mensagemAberta;
                }

                if ((mensagem != "") && (typeof mensagem != "undefined")) {
                    Alerta("Aviso", mensagem + "<br><br> Finalize para adicionar uma nova OS!");
                }

            }
            else {
                ValidarOrdem();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
    
});

function ValidarOrdem () {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    const url = URLPermiteIncluirOs + "?codTecnico=" + CD_TECNICO;

    $.ajax({
        type: 'GET',
        url: url,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        processData: true,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            if (res.Iniciar != null || res.Aberta != null) {
                event.preventDefault();
                if (res.Iniciar == "TemOS") {
                    var mensagem = "Existe OS com status <strong>AGUARDANDO INICIO</strong> em atendimento.";
                }
                if (res.Aberta == "TemOS") {
                    let mensagemAberta = "Existe OS com status <strong>ABERTA</strong> em atendimento.";
                    mensagem = (mensagem != "") && (typeof mensagem != "undefined") ? mensagem + "<br><br>" + mensagemAberta : mensagemAberta;
                }

                if (res.Iniciar == "TemVisita") {
                    var mensagem = "Existe Visita com status <strong>INICIAR</strong> em atendimento.";
                }
                if (res.Aberta == "TemVisita") {
                    let mensagemAberta = "Existe Visita com status <strong>ABERTA</strong> em atendimento.";
                    mensagem = (mensagem != "") && (typeof mensagem != "undefined") ? mensagem + "<br><br>" + mensagemAberta : mensagemAberta;
                }

                if ((mensagem != "") && (typeof mensagem != "undefined")) {
                    Alerta("Aviso", mensagem + "<br><br> Finalize para adicionar uma nova OS!");
                }

            }

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}
