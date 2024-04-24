$().ready(function () {

    $('.js-select-basic-single').select2();

    carregarComboClientes();

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
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}



$('#btnFiltrar').click(function () {
    carregarGridMVC();
});

function carregarGridMVC() {
    var CD_ATIVO_FIXO = $("#CD_ATIVO_FIXO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == undefined)
        CD_CLIENTE = 0;


    var URL = URLObterListaAtivo + "?CD_ATIVO_FIXO=" + CD_ATIVO_FIXO + "&CD_CLIENTE=" + CD_CLIENTE;
    //atribuirParametrosPaginacao("gridmvc", URLObterListaVisita, '{"CD_CLIENTE" : "' + CD_CLIENTE + '", "CD_TECNICO":"' + CD_TECNICO + '", "DT_INICIO":"' + DT_INICIO + '", "DT_FIM":"' + DT_FIM + '" }');

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
            if (res.Status == "Success") {
                $('#gridMVCAtivo').html(res.Html);
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
    $('#cliente_CD_CLIENTE').val(null).trigger('change');
    $('#gridMVCAtivo').html(null);


});
