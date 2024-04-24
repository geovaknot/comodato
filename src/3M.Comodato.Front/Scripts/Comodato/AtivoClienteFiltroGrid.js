﻿$().ready(function () {

    $('.js-select-basic-single').select2();

    carregarComboClientes();

    if (can_load_grid != "False")
        carregarGridMVC();

});


function carregarComboClientes() {
    var URL = URLAPI + "ClienteAPI/";

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaComboPerfilCliente?nidUsuario=" + nidUsuario;
    }
    else {
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    }
    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
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
            if (res.clientes != null) {
                preencherComboClientes(res.clientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function preencherComboClientes(clientes) {
    if (nidPerfil == perfilCliente)
        $("#cliente_CD_CLIENTE").empty();
    else
        LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE, cd_cliente == clientes[i].CD_CLIENTE);
    }
}



$('#btnFiltrar').click(function () {
    carregarGridMVC();
});

function carregarGridMVC() {
    var CD_ATIVO_FIXO = $("#CD_ATIVO_FIXO").val();
    var NR_NOTAFISCAL = $("#NR_NOTAFISCAL").val();
    //var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == undefined)
        CD_CLIENTE = 0;

    if (NR_NOTAFISCAL == "" || NR_NOTAFISCAL == undefined)
        NR_NOTAFISCAL = 0;


    var URL = URLObterListaAtivoCliente + "?CD_ATIVO_FIXO=" + CD_ATIVO_FIXO + "&CD_CLIENTE=" + CD_CLIENTE + "&NR_NOTAFISCAL=" + NR_NOTAFISCAL;
    //atribuirParametrosPaginacao("gridmvc", URLObterListaVisita, '{"CD_CLIENTE" : "' + CD_CLIENTE + '", "CD_TECNICO":"' + CD_TECNICO + '", "DT_INICIO":"' + DT_INICIO + '", "DT_FIM":"' + DT_FIM + '" }');

    $.ajax({
        url: URL,
        processData: true,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loaderGRIDCliente").css("display", "block");
        },
        complete: function () {
            $("#loaderGRIDCliente").css("display", "none");
        },
        success: function (res) {
            $("#loaderGRIDCliente").css("display", "none");
            if (res.Status == "Success") {
                $('#gridMVCAtivoCliente').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}



$('#btnLimpar').click(function () {

    $("#CD_ATIVO_FIXO").val(null);
    $("#NR_NOTAFISCAL").val(null);
    $('#cliente_CD_CLIENTE').val(null).trigger('change');
    $('#gridMVCAtivoCliente').html(null);

       
    $.ajax({
        url: "AtivoCliente/limpaFiltro",
        dataType: 'json',
        type: "GET",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

        }
    });
});
