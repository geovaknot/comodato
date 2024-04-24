jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();
    carregarGridMVC();
});

$("#perfil_nidPerfil").change(function () {
    carregarGridMVC();
});

function carregarGridMVC() {
    var nidPerfil = $("#perfil_nidPerfil option:selected").val();

    if (nidPerfil == "")
        nidPerfil = "0";

    //var URL = "@Url.Action("ObterListaPerfilXFuncaoJson", "PerfilXFuncao")" + "?nidPerfil=" + nidPerfil;
    var URL = URLObterLista + "?nidPerfil=" + nidPerfil;

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
                $('#gridmvc').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function ExcluirConfirmar(nidPerfilFuncao) {
    ConfirmarSimNao('Aviso', 'Confirma a exclusão do registro?', 'Excluir(' + nidPerfilFuncao + ')');
}

function Excluir(nidPerfilFuncao) {
    //var URL = "@Url.Action("ExcluirPerfilXFuncaoJson", "PerfilXFuncao")" + "?nidPerfilFuncao=" + nidPerfilFuncao;
    var URL = URLExcluir + "?nidPerfilFuncao=" + nidPerfilFuncao;

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
                carregarGridMVC();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}
