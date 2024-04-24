jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    OcultarCampo($('#validaUsuario'));

    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }

    carregarComboUsuario();
    carregarDados();
    carregarGridMVC();
});

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

function carregarDados() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }

    carregarCliente(CD_CLIENTE);
}

function carregarCliente(CD_CLIENTE) {
    var URL = URLAPI + "ClienteAPI/Obter?CD_CLIENTE=" + CD_CLIENTE;
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
            if (res.cliente != null) {
                LoadCliente(res.cliente);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadCliente(cliente) {
    var NM_CLIENTE = cliente.NM_CLIENTE + " (" + cliente.CD_CLIENTE + ") " + cliente.EN_CIDADE + " - " + cliente.EN_ESTADO;
    $("#cliente_NM_CLIENTE").val(NM_CLIENTE);

    var EN_ENDERECO = cliente.EN_ENDERECO + " " + cliente.EN_BAIRRO + " " + cliente.EN_CIDADE + " " + cliente.EN_ESTADO + " " + cliente.EN_CEP
    $("#cliente_EN_ENDERECO").val(EN_ENDERECO);
}

function carregarGridMVC() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var URL = URLObterLista + "?CD_CLIENTE=" + CD_CLIENTE;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
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
            //if (res.listaUsuarioXClienteEscalas != null) {
            //    LoadUsuarioXClienteEscalas(res.listaUsuarioXClienteEscalas);
            //}
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

$('#btnAdicionar').click(function () {
    var URL = URLAPI + "UsuarioClienteAPI/Incluir";
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var nidUsuarioSelecionado = $("#usuario_nidUsuario option:selected").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (nidUsuarioSelecionado == "" || nidUsuarioSelecionado == "0" || nidUsuarioSelecionado == 0) {
        ExibirCampo($('#validaUsuario'));
        return;
    }

    var usuarioClienteEntity = {
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        usuario: {
            nidUsuario: nidUsuarioSelecionado,
        },
        CD_ORDEM: 0,
        nidUsuarioAtualizacao: nidUsuario
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(usuarioClienteEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

});

$("#tecnico_CD_TECNICO").change(function () {
    OcultarCampo($('#validaUsuario'));
});

function atualizarPagina() {
    OcultarCampo($('#validaUsuario'));
    carregarComboUsuario();
    carregarGridMVC();
}

function ExcluirConfirmar(nidUsuarioCliente) {
    ConfirmarSimNao('Aviso', 'Confirma a exclusão do registro?', 'Excluir(' + nidUsuarioCliente + ')');
}

function Excluir(nidUsuarioCliente) {
    var URL = URLAPI + "UsuarioClienteAPI/Excluir";
    //var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    //if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
    //    AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
    //    return;
    //}

    var usuarioClienteEntity = {
        cliente: {
            CD_CLIENTE: null,
        },
        usuario: {
            nidUsuario: null,
        },
        nidUsuarioCliente: nidUsuarioCliente,
        nidUsuarioAtualizacao: nidUsuario
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(usuarioClienteEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}
