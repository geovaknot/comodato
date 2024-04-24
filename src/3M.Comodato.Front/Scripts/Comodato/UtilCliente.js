function carregarComboCliente() {
    var URL = URLAPI + "ClienteAPI/";

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaComboPerfilCliente?nidUsuario=" + nidUsuario;
    }
    else {
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    }

    ////var token = sessionStorage.getItem("token");

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
                LoadClientes(res.clientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadClientes(clientes) {
    if (nidPerfil == perfilCliente) {
        if (clientes.length == 0)
            LimparCombo($("#cliente_CD_CLIENTE"));
        else
            $("#cliente_CD_CLIENTE").empty();
    }
    else
        LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }

    totalClientes = clientes.length;
}

function LoadTecnicosCliente(tecnicos) {
    LimparCombo($("#tecnico_CD_TECNICO"));

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
    }
}