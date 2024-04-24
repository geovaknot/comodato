$().ready(function () {
    $('select').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('select').val(null).trigger('change');
    //$('#ddlAcao').val(null).trigger('change');
});

$('#btnExecutarAcao').click(function () {
    
    if (null != $('#ddlAcao').val()) {
        var codigoAcao = $('#ddlAcao option:selected').val();
        var textoMensagem = $('#ddlAcao option:selected').text().replace('enviar', 'receber').replace('Enviar', 'Receber').replace('Marcar', '');;
        var mensagem = 'Todos os clientes serão marcados para "' + textoMensagem + '". Confirma alteração?';

        ConfirmarSimNao('Atenção!', mensagem, 'callbackAcao(' + codigoAcao + ')');
    }
});

function callbackAcao(codigoAcao) {
    var url = URLAPI + 'ClienteAPI/AlterarEnvioPesquisa?fl_pesq_satisf=' + codigoAcao + '&nidUsuario=' + nidUsuario;
    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: url,
        //processData: true,
        async: false,
        cache: false,
        contentType: "application/json",
        dataType: "json",

        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (response) {
            $("#loader").css("display", "none");
            Alerta("Sucesso", response);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function MensagemSucesso() {
    AlertaRedirect("Aviso", MENSAGEMSUCESSO, "window.location = '../AnaliseSatisfacao';");
}