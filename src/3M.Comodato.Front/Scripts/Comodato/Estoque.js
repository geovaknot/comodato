$().ready(function () {
    $('.js-example-basic-single').select2();

    //$('.js-select-basic-single').val(null).trigger('change');

    if ($('#cdsTipoEstoque').val() == 'CLI') {
        ExibirCampo($('#rowCliente'));
    }
    else {
        OcultarCampo($('#rowCliente'));
    }
});

$('select#nidUsuarioResponsavel').change(function () {
    var codigoUsuario = $('select#nidUsuarioResponsavel').val();
    $('input[type="hidden"]#nidUsuarioResponsavel').val(codigoUsuario);//.trigger('change');
    OcultarCampo($('#validaCliente'));

    if ($('#cdsTipoEstoque').val() == 'CLI' && (codigoUsuario != '' && codigoUsuario != 0)) {
        LimparCombo($("select#CD_CLIENTE"));
        popularComboCliente(codigoUsuario);
    }
});

$('#cdsTipoEstoque').change(function (e) {
    LimparCombo($("select#CD_CLIENTE"));
    OcultarCampo($('#validaCliente'));
    OcultarCampo($('#rowCliente'));

    $('input[type="hidden"]#CD_CLIENTE').val('');
    //$('select#CD_CLIENTE').prop("disabled", true);

    switch ($('#cdsTipoEstoque').val()) {
        case '3M1':
        case '3M2':
            popularComboResponsaveis('1,5');
            break;
        case 'TEC':
            popularComboResponsaveis('2,3,6');
            break;
        case 'CLI':
            //$('select#CD_CLIENTE').prop("disabled", false);
            popularComboResponsaveis('8');
            ExibirCampo($('#rowCliente'));
            break;
        default:
            LimparCombo($("select#nidUsuarioResponsavel"));
            $('input[type="hidden"]#nidUsuarioResponsavel').val('');
    }
});

$('select#CD_CLIENTE').change(function () {
    $('input[type="hidden"]#CD_CLIENTE').val($('select#CD_CLIENTE').val());
});

function popularComboResponsaveis(data) {
    var URL = URLAPI + 'UsuarioAPI/ObterListaPorPerfil?perfis=' + data;
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
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.usuarios != null) {
                preencherComboResponsavel(data.usuarios);
                $('select#nidUsuarioResponsavel').val('').trigger('change');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function preencherComboResponsavel(responsavelJO) {
    LimparCombo($("select#nidUsuarioResponsavel"));
    var usuarios = JSON.parse(responsavelJO);
    for (i = 0; i < usuarios.length; i++) {
        MontarCombo($("select#nidUsuarioResponsavel"), usuarios[i].nidUsuario, usuarios[i].cnmNome + ' (' + usuarios[i].cdsLogin + ')');
    }
}

function popularComboCliente(idUsuario) {
    var URL = URLAPI + "ClienteAPI/";

    //if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaPerfilCliente?nidUsuario=" + idUsuario + "&SomenteAtivos=true";
    //}
    //else {
    //    URL = URL + "ObterListaPorUsuarioPerfil?nidUsuario=" + idUsuario + "&SomenteAtivos=false";
    //}
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
    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

