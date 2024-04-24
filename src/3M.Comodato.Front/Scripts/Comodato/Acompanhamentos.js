jQuery(document).ready(function () {
    //$('.js-example-basic-single').select2({ minimumInputLength: 1 });
    $('.js-example-basic-single').select2({
        placeholder: "Selecione...",
        templateSelection: function (data) { return data.id; }
    });

    carregarComboCliente();
});

$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null);
    carregarComboCliente();

});

$('#btnImprimir').click(function () {

    //var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

	if (CD_CLIENTE == undefined)
		CD_CLIENTE = '';

    var URL = URLCriptografarChave + "?Conteudo=" + CD_CLIENTE;

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
            //if (res.idKey != null && res.idKey != '') {
                window.open(URLSite + '/RelatorioAcompanhamentos.aspx?idKey=' + res.idKey, '_blank');
            //}
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

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
        //$("#cliente_CD_CLIENTE").append("<option value='" + clientes[i].CD_CLIENTE + "'>" + clientes[i].NM_CLIENTE + "</option>");
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}