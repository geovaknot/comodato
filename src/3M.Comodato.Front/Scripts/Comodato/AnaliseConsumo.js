$(document).ready(function () {
    $('#ddlAgrupamento').select2();

    $('#ddlGrupo').select2({
        placeholder: "Selecione...",
        allowClear: true
    });
    $('#ddlCliente').select2({
        //minimumInputLength: 3,
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlExecutivo').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlVendedor').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlLinhaProduto').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    popularComboGrupo();
    popularComboClientes();
    popularComboLinhaProduto();
});

$('#btnImprimir').click(function () {
    var tipoRelatorio = $("input[type='radio']:checked").val();
    var visaoSelecionada = $('#ddlAgrupamento option:selected').text();

    var codigoVisao = '';
    if (null != $('#ddlAgrupamento').val()) {
        codigoVisao = $('#ddlAgrupamento').val()
    }

    var codigoGrupo = '';
    if (null != $('#ddlGrupo').val()) {
        codigoGrupo = $('#ddlGrupo').val()
    }

    var codigoCliente = '';
    if (null != $('#ddlCliente').val()) {
        codigoCliente = $('#ddlCliente').val()
    }

    var codigoExecutivo = '';
    if (null != $('#ddlExecutivo').val()) {
        codigoExecutivo = $('#ddlExecutivo').val()
    }

    var codigoVendedor = '';
    if (null != $('#ddlVendedor').val()) {
        codigoVendedor = $('#ddlVendedor').val()
    }

    var codigoLinhaProduto = '';
    if (null != $('#ddlLinhaProduto').val()) {
        codigoLinhaProduto = $('#ddlLinhaProduto').val()
    }

    var URL = URLCriptografarChave + "?Conteudo=" + tipoRelatorio + "|" + codigoVisao + "|" + visaoSelecionada + "|" + codigoGrupo + "|" + codigoCliente + '|' + codigoExecutivo + "|" + codigoVendedor + "|" + codigoLinhaProduto;

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
                window.open(URLSite + '/RelatorioConsumo.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

});

$('#btnLimpar').click(function () {
    $('#ddlGrupo').val(null).trigger('change');
    $('#ddlCliente').val(null).trigger('change');
    $('#ddlExecutivo').val(null).trigger('change');
    $('#ddlVendedor').val(null).trigger('change');
    $('#ddlLinhaProduto').val(null).trigger('change');
});

function popularComboGrupo() {
    var URL = URLAPI + "GrupoAPI/ObterLista";
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
            if (res.grupos != null) {
                preencherComboGrupos(res.grupos);
            }
            $('#ddlGrupo').val('').trigger('change');
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function preencherComboGrupos(grupos) {
    LimparCombo($("#ddlGrupo"));
    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#ddlGrupo"), grupos[i].CD_GRUPO, grupos[i].DS_GRUPO);
    }
}

function popularComboClientes() {
    var URL = URLAPI + "ClienteAPI/";

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaPerfilCliente?nidUsuario=" + nidUsuario;
    }
    else {
        //BEGIN - IM8004534 - Melhoria - Ubirajara Lisboa - 12/02/2021
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
        //END - IM8004534 - Melhoria - Ubirajara Lisboa - 12/02/2021
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
        $("#ddlCliente").empty();
    else
        LimparCombo($("#ddlCliente"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#ddlCliente"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

function popularComboLinhaProduto() {
    var URL = URLAPI + "LinhaProdutoAPI/ObterLista";
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
            if (res.produtos != null) {
                preencherComboLinhaProduto(res.produtos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function preencherComboLinhaProduto(produtos) {
    LimparCombo($("#ddlLinhaProduto"));
    for (i = 0; i < produtos.length; i++) {
        MontarCombo($("#ddlLinhaProduto"), produtos[i].CD_LINHA_PRODUTO, produtos[i].DS_LINHA_PRODUTO);
    }
}
