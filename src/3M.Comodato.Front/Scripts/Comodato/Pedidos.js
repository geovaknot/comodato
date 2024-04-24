$('#DT_DATAS-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_INICIAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_FINAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$("#cliente_CD_CLIENTE").change(function () {
    carregarComboTecnico();
});

jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();
    $('#cliente_CD_CLIENTE').select2({ minimumInputLength: 3 });

    $('#DT_INICIAL').mask('00/00/0000');
    $('#DT_FINAL').mask('00/00/0000');
    $('#DT_DEV_FINAL').val(Date.now.toString("dd/MM/yyyy"));

    carregarComboCliente();
    carregarComboTecnico();
    carregarComboGrupo();
});

$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null).trigger('change');
    $('#tecnico_CD_TECNICO').val(null).trigger('change');
    $('#grupo_CD_GRUPO').val(null).trigger('change');
    $('#selectModeloRelatorio').val(null).trigger('change');

    $('#DT_INICIAL').val('');
    $('#DT_FINAL').val('');
});

$('#btnImprimir').click(function () {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var CD_GRUPO = $("#grupo_CD_GRUPO option:selected").val();
    var DT_INICIAL = $('#DT_INICIAL').val();
    var DT_FINAL = $('#DT_FINAL').val();
    var ModeloRelatorio = '';
    if (null != $("#selectModeloRelatorio").val() && $("#selectModeloRelatorio").val() != '')
        ModeloRelatorio = $("#selectModeloRelatorio").val();

    var check = checarDatas(DT_INICIAL, DT_FINAL);

    if (CD_CLIENTE == undefined)
        CD_CLIENTE = '';

    if (CD_TECNICO == undefined)
        CD_TECNICO = '';

    if (CD_TECNICO == null || CD_TECNICO == '')
        CD_TECNICO = ' ';


    if (CD_CLIENTE == null || CD_CLIENTE == '')
        CD_CLIENTE = ' ';


    if (CD_GRUPO == undefined)
        CD_GRUPO = '';

    if (CD_GRUPO == null || CD_GRUPO == '')
        CD_GRUPO = ' ';


    if (check) {
        //window.open(URLImprimir + '?idKey=' + DT_INICIAL + "|" + DT_FINAL + '|' + CD_CLIENTE + "|" + CD_TECNICO + "|" + CD_GRUPO + "|", '_blank');

        var URL = URLCriptografarChave + "?Conteudo=" + DT_INICIAL + "|" + DT_FINAL + '|' + CD_CLIENTE + "|" + CD_TECNICO + "|" + CD_GRUPO + "|" + ModeloRelatorio;
        console.log("Valor URL", URL);
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
                if (res.idKey != null && res.idKey != '') {
                    window.open(URLSite + '/RelatorioPedidos.aspx?idKey=' + res.idKey, '_blank');
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                console.log("Teste Erro", res);
                Alerta("ERRO", res.responseText);
            }

        });

    }
});
function carregarComboCliente() {
    var URL = URLAPI + "ClienteAPI/ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;

    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
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
            if (res.clientes != null) {
                LoadClientes(res.clientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadClientes(clientes) {
    LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + ' (' + clientes[i].CD_CLIENTE + ') ' + clientes[i].EN_CIDADE + ' - ' + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

function checarDatas(DT_INICIAL, DT_FINAL) {
    var data_1 = new Date(formatarData(DT_INICIAL));
    var data_2 = new Date(formatarData(DT_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        Alerta("Aviso", "A data inicial não pode ser maior que a data final!");
        return false;
    } else {
        return true;
    }
}

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}

function carregarComboTecnico() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var URL = URLAPI;
    //var token = sessionStorage.getItem("token");
    if (undefined == CD_CLIENTE || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        URL = URLAPI + "TecnicoAPI/ObterListaAtivos";

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
                if (res.tecnicos != null) {
                    LoadTecnicos(res.tecnicos);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });
    }
    else {
        URL = URLAPI + "TecnicoXClienteAPI/ObterLista"; //?CD_CLIENTE=" + CD_CLIENTE;

        var tecnicoClienteEntity = {
            cliente: {
                CD_CLIENTE: CD_CLIENTE,
            }
        };

        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            async: false,
            contentType: "application/json",
            //headers: { "Authorization": "Basic " + localStorage.token },
            data: JSON.stringify(tecnicoClienteEntity),
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                if (res.tecnicos != null) {
                    LoadTecnicosCliente(res.tecnicos);
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

function LoadTecnicos(tecnicosJO) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    }
}

function LoadTecnicosCliente(tecnicos) {
    LimparCombo($("#tecnico_CD_TECNICO"));

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
    }
}

function carregarComboGrupo() {
    var URL = URLAPI + "GrupoAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
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
            if (res.grupos != null) {
                LoadGrupos(res.grupos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadGrupos(grupos) {
    LimparCombo($("#grupo_CD_GRUPO"));

    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#grupo_CD_GRUPO"), grupos[i].CD_GRUPO, grupos[i].DS_GRUPO);
    }
}