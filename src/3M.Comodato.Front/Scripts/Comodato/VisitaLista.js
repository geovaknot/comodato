jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    OcultarCampo($('#validaTecnico'));
    OcultarCampo($('#btnOrdenarVisita'));
    OcultarCampo($('#btnNovaVisita'));

    carregarComboTecnicoVisita();
    carregarComboCliente();
    carregarComboMotivo();
    carregarComboTpStatusVisita();
    carregarComboRegiao();
    ConfigurarCacheNavegador();
    carregarGridMVC();
});

function carregarComboTecnicoVisita() {
    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
    var tecnicoEntity = {};

    tecnicoEntity = {
        usuario: {
            nidUsuario: nidUsuario
        }
    };

    ////var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(tecnicoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            ExibirCampo($('#loader'));
        },
        complete: function () {
            OcultarCampo($('#loader'));
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tecnicos != null) {
                LoadTecnicos(res.tecnicos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function LoadTecnicos(tecnicosJO) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    }

    if (UsuarioTecnico.toLowerCase() === 'true') {
        $("#tecnico_CD_TECNICO").prop("disabled", true);
        $('#tecnico_CD_TECNICO').val(tecnicos[0].CD_TECNICO).trigger('change');
    }
    else {
        $("#tecnico_CD_TECNICO").prop("disabled", false);
    }
}

function ConfigurarCacheNavegador() {

    if (localStorage['Visita_tecnico_CD_TECNICO'] != undefined && localStorage['Visita_tecnico_CD_TECNICO'] != "") {
        $('#tecnico_CD_TECNICO').val(localStorage['Visita_tecnico_CD_TECNICO']).trigger('change');
    }

    if (localStorage['Visita_TpStatusVisita_ST_STATUS_VISITA'] != undefined && localStorage['Visita_TpStatusVisita_ST_STATUS_VISITA'] != "") {
        $('#TpStatusVisita_ST_STATUS_VISITA').val(localStorage['Visita_TpStatusVisita_ST_STATUS_VISITA']).trigger('change');
    }

    if (localStorage['Visita_regiao_CD_REGIAO'] != undefined && localStorage['Visita_regiao_CD_REGIAO'] != "") {
        $('#regiao_CD_REGIAO').val(localStorage['Visita_regiao_CD_REGIAO']).trigger('change');
    }

    if (localStorage['Visita_TpMotivoVisita_CD_MOTIVO_VISITA'] != undefined && localStorage['Visita_TpMotivoVisita_CD_MOTIVO_VISITA'] != "") {
        $('#TpMotivoVisita_CD_MOTIVO_VISITA').val(localStorage['Visita_TpMotivoVisita_CD_MOTIVO_VISITA']).trigger('change');
    }

    if (localStorage['Visita_cliente_CD_CLIENTE'] != undefined && localStorage['Visita_cliente_CD_CLIENTE'] != "") {
        $('#cliente_CD_CLIENTE').val(localStorage['Visita_cliente_CD_CLIENTE']).trigger('change');
    }
}

$('#btnLimpar').click(function () {

    OcultarCampo($('#validaTecnico'));

    if (UsuarioTecnico.toLowerCase() === 'false') {
        $('#tecnico_CD_TECNICO').val(null).trigger('change');
        localStorage.removeItem('Visita_tecnico_CD_TECNICO');
    }

    $('#TpStatusVisita_ST_STATUS_VISITA').val(null).trigger('change');
    localStorage.removeItem('Visita_TpStatusVisita_ST_STATUS_VISITA');

    $('#regiao_CD_REGIAO').val(null).trigger('change');
    localStorage.removeItem('Visita_regiao_CD_REGIAO');

    $("#TpMotivoVisita_CD_MOTIVO_VISITA").val(null).trigger('change');
    localStorage.removeItem('Visita_TpMotivoVisita_CD_MOTIVO_VISITA');

    $("#cliente_CD_CLIENTE").val(null).trigger('change');
    localStorage.removeItem('Visita_cliente_CD_CLIENTE');

    $('#OS_ID_OS').val('');
    carregarGridMVC();
});

$('#btnFiltrar').click(function () {
    ObterListavisita();
});

$('#btnOrdenarVisita').click(function () {
    ObterListavisita();
});

function ObterListavisita() {

    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if ((CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO) == 0 && (UsuarioTecnico.toLowerCase() === 'true')) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    carregarGridMVC();

    localStorage['Visita_tecnico_CD_TECNICO'] = $("#tecnico_CD_TECNICO option:selected").val();
    localStorage['Visita_TpStatusVisita_ST_STATUS_VISITA'] = $("#TpStatusVisita_ST_STATUS_VISITA option:selected").val();
    localStorage['Visita_regiao_CD_REGIAO'] = $("#regiao_CD_REGIAO option:selected").val();
    localStorage['Visita_TpMotivoVisita_CD_MOTIVO_VISITA'] = $("#TpMotivoVisita_CD_MOTIVO_VISITA option:selected").val();
    localStorage['Visita_cliente_CD_CLIENTE'] = $("#cliente_CD_CLIENTE option:selected").val();
}

$("#tecnico_CD_TECNICO").change(function () {
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    OcultarCampo($('#validaTecnico'));

    OcultarCampo($('#gridmvc'));
    OcultarCampo($('#btnOrdenarVisita'));
    OcultarCampo($('#btnNovaVisita'));
    if ((UsuarioTecnico.toLowerCase() !== 'true') && (CD_TECNICO != "" || CD_TECNICO != "0" || CD_TECNICO != 0)) {
        OcultarCampo($('#btnNovaVisita'));
    }
    $('#btnNovaVisita').attr("href", "VisitaPadrao/Incluir?CD_TECNICO=" + $("#tecnico_CD_TECNICO").val() + "");
});

function carregarComboTpStatusVisita() {
    var URL = URLAPI + "TpStatusVisitaPadraoAPI/ObterLista?statusVisitaPadraoEntity.ST_STATUS_VISITA=0";
    ////var token = sessionStorage.getItem("token");
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
            if (res.STATUS_VISITA != null) {
                LoadTpStatusVisitaOS(res.STATUS_VISITA);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadTpStatusVisitaOS(tiposStatusVisita) {
    LimparCombo($("#TpStatusVisita_ST_STATUS_VISITA"));

    for (i = 0; i < tiposStatusVisita.length; i++) {
        MontarCombo($("#TpStatusVisita_ST_STATUS_VISITA"), tiposStatusVisita[i].ID_STATUS_VISITA, tiposStatusVisita[i].DS_STATUS_VISITA);
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
    var ST_STATUS_VISITA = $("#TpStatusVisita_ST_STATUS_VISITA option:selected").val();
    var CD_MOTIVO_VISITA = $("#TpMotivoVisita_CD_MOTIVO_VISITA option:selected").val();
    var CD_REGIAO = $("#regiao_CD_REGIAO option:selected").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();

    if ((CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) && (UsuarioTecnico.toLowerCase() === 'true'))
        return;

    var DT_INICIAL = $('#DT_INICIAL').val();
    var DT_FINAL = $('#DT_FINAL').val();

    var periodoValido = checarDatas(DT_INICIAL, DT_FINAL);

    var URL = URLObterLista + "?CD_TECNICO=" + CD_TECNICO + "&CD_REGIAO=" + CD_REGIAO + "&ST_STATUS_VISITA=" + ST_STATUS_VISITA +
        "&CD_MOTIVO_VISITA=" + CD_MOTIVO_VISITA + "&CD_CLIENTE=" + CD_CLIENTE + "&orderby=ID_VISITA&ordertype=" + ordenacaoListaVisita + "&DT_INICIAL=" + DT_INICIAL + "&DT_FINAL=" + DT_FINAL;

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
                    ExibirCampo($('#btnOrdenarVisita'));
                    //ExibirCampo($('#btnNovaVisita'));
                    if (CD_TECNICO != "" && CD_TECNICO != "0" && CD_TECNICO != 0) {
                        ExibirCampo($('#btnNovaVisita'));
                    }
                    else {
                        OcultarCampo($('#btnNovaVisita'));
                    }
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", res.responseText);
            }
        });

        ConfigurarOrdenacaoListaVisita();
    }
}

function ConfigurarOrdenacaoListaVisita() {

    const ordenacaoAscendente = "asc";
    const ordenacaoDescendente = "desc";

    ordenacaoListaVisita = ordenacaoListaVisita == ordenacaoDescendente ? ordenacaoAscendente : ordenacaoDescendente;
} 

function carregarComboMotivo() {
    var URL = URLAPI + "TpMotivoVisitaPadraoAPI/ObterLista?motivoVisitaPadraoEntity.CD_MOTIVO_VISITA=0";
    ////var token = sessionStorage.getItem("token");

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
                LoadMotivos(res.MOTIVO_VISITA);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadMotivos(motivos) {
    LimparCombo($("#TpMotivoVisita_CD_MOTIVO_VISITA"));

    for (i = 0; i < motivos.length; i++) {
        MontarCombo($("#TpMotivoVisita_CD_MOTIVO_VISITA"), motivos[i].CD_MOTIVO_VISITA, motivos[i].DS_MOTIVO_VISITA);
    }
}

$('#btnNovaVisita').click(function (event) {
   
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
                    Alerta("Aviso", mensagem + "<br><br> Finalize para adicionar uma nova Visita!");
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

function ValidarOrdem() {
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
                    Alerta("Aviso", mensagem + "<br><br> Finalize para adicionar uma nova Visita!");
                }

            }

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}
