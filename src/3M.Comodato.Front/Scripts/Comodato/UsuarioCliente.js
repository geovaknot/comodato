jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();
    //$('.js-example-basic-single').select2({ minimumInputLength: 3 });
    //$('#tecnico_CD_TECNICO').select2();

    carregarComboCliente();
    carregarComboUsuario();
    if (localStorage['UsuarioCliente_CD_CLIENTE'] != undefined && localStorage['UsuarioCliente_CD_CLIENTE'] != "") {
        $('#cliente_CD_CLIENTE').val(localStorage['UsuarioCliente_CD_CLIENTE']).trigger('change');
    }

    if (localStorage['UsuarioCliente_nidUsuario'] != undefined && localStorage['UsuarioCliente_nidUsuario'] != "") {
        $('#usuario_nidUsuario').val(localStorage['UsuarioCliente_nidUsuario']).trigger('change');
    }

    carregarGridMVC();
});

//$("#cliente_CD_CLIENTE").change(function () {
//    carregarComboTecnico();
//});

$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null).trigger('change');
    $('#usuario_nidUsuario').val(null).trigger('change');

    localStorage.removeItem('UsuarioCliente_CD_CLIENTE');
    localStorage.removeItem('UsuarioCliente_nidUsuario');

    carregarGridMVC();
});

$('#btnFiltrar').click(function () {
    carregarGridMVC();

    localStorage['UsuarioCliente_CD_CLIENTE'] = $("#cliente_CD_CLIENTE option:selected").val();
    localStorage['UsuarioCliente_nidUsuario'] = $("#usuario_nidUsuario option:selected").val();
});

function carregarGridMVC() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();

    if (CD_CLIENTE == "")
        CD_CLIENTE = 0;

    var nidUsuario = $("#usuario_nidUsuario option:selected").val();

    if (nidUsuario == "")
        nidUsuario = 0;

    var URL = URLObterLista + "?CD_CLIENTE=" + CD_CLIENTE + "&nidUsuario=" + nidUsuario;
    atribuirParametrosPaginacao("gridmvc", URLObterLista, '{"CD_CLIENTE" : "' + CD_CLIENTE + '" }');
    atribuirParametrosPaginacao("gridmvc", URLObterLista, '{"CD_CLIENTE" : "' + CD_CLIENTE + '", "nidUsuario":"' + nidUsuario + '" }');

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

function carregarComboCliente() {
    //var URL = URLAPI + "ClienteAPI/ObterListaPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    var URL = URLAPI + "ClienteAPI/";

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaPerfilCliente?nidUsuario=" + nidUsuario + "&SomenteAtivos=true";
    }
    else {
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario + "&SomenteAtivos=true";
    }
    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'GET',
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
    if (nidPerfil == perfilCliente)
        $("#cliente_CD_CLIENTE").empty();
    else
        LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

function carregarComboUsuario() {
    //var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    //if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
    //    AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
    //    return;
    //}

    var URL = URLAPI + "UsuarioAPI/ObterLista";
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
            if (res.usuarios != null) {
                LoadUsuarios(res.usuarios);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadUsuarios(usuariosJO) {
    LimparCombo($("#usuario_nidUsuario"));

    var usuarios = JSON.parse(usuariosJO);
    for (i = 0; i < usuarios.length; i++) {
        //$("#tecnico_CD_TECNICO").append("<option value='" + usuarios[i].tecnico.CD_TECNICO + "'>" + usuarios[i].tecnico.NM_TECNICO + "</option>");
        var cnmNome = usuarios[i].cnmNome + ' (' + usuarios[i].cdsLogin + ')';
        MontarCombo($("#usuario_nidUsuario"), usuarios[i].nidUsuario, cnmNome);
    }
}
