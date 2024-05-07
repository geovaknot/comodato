jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    OcultarCampo($('#validaTecnico'));

    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }

    carregarComboTecnico();
    carregarDados();
    carregarGridMVC();
});

function carregarComboTecnico() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLAPI + "TecnicoAPI/ObterListaByEscala?CD_CLIENTE=" + CD_CLIENTE;
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
            if (res.tecnicos != null) {
                LoadTecnicos(res.tecnicos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadTecnicos(tecnicos) {
    LimparCombo($("#tecnico_CD_TECNICO"));

    for (i = 0; i < tecnicos.length; i++) {
        //$("#tecnico_CD_TECNICO").append("<option value='" + tecnicos[i].tecnico.CD_TECNICO + "'>" + tecnicos[i].tecnico.NM_TECNICO + "</option>");
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
    }
}

function carregarDados() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }

    carregarCliente(CD_CLIENTE);
    carregarQtdeEquipamentos(CD_CLIENTE);
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

    $("#cliente_QT_PERIODO").val(cliente.QT_PERIODO);

    $("#cliente_regiao_DS_REGIAO").val(cliente.regiao.DS_REGIAO);
}

function carregarQtdeEquipamentos(CD_CLIENTE) {
    var URL = URLAPI + "ClienteAPI/ObterQtdeEquipamentos?CD_CLIENTE=" + CD_CLIENTE;
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
            if (res.nvlQtdeEquipamentos != null) {
                $("#nvlQtdeEquipamentos").val(res.nvlQtdeEquipamentos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
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
            //if (res.listaTecnicoXClienteEscalas != null) {
            //    LoadTecnicoXClienteEscalas(res.listaTecnicoXClienteEscalas);
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
    var URL = URLAPI + "TecnicoXClienteAPI/Incluir";
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }
    //var token = sessionStorage.getItem("token");
    var tecnicoClienteEntity = {
        cliente : {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico : {
            CD_TECNICO: CD_TECNICO,
        },
        CD_ORDEM : 0,
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tecnicoClienteEntity),
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
    OcultarCampo($('#validaTecnico'));
});

function atualizarPagina() {
    OcultarCampo($('#validaTecnico'));
    carregarComboTecnico();
    carregarGridMVC();
}

function Subir(CD_ORDEM) {
    var URL = URLAPI + "TecnicoXClienteAPI/Reordenar";
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }
    //var token = sessionStorage.getItem("token");
    var tecnicoClienteReordernar = {
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico: {
            CD_TECNICO: 0,
        },
        CD_ORDEM: CD_ORDEM,
        TP_ACAO : 'S',
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tecnicoClienteReordernar),
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

function Descer(CD_ORDEM) {
    var URL = URLAPI + "TecnicoXClienteAPI/Reordenar";
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }
    //var token = sessionStorage.getItem("token");
    var tecnicoClienteReordernar = {
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico: {
            CD_TECNICO: 0,
        },
        CD_ORDEM: CD_ORDEM,
        TP_ACAO: 'D',
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tecnicoClienteReordernar),
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

function InativarConfirmar(ID_ESCALA, CD_ORDEM) {
    ConfirmarSimNao('Aviso', 'Confirma a exclusão do registro?', 'Inativar(' + ID_ESCALA + ', ' + CD_ORDEM + ')')
}

function Inativar(ID_ESCALA, CD_ORDEM) {
    var URL = URLAPI + "TecnicoXClienteAPI/Inativar";
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }
    //var token = sessionStorage.getItem("token");
    var tecnicoClienteEntity = {
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico: {
            CD_TECNICO: 0,
        },
        CD_ORDEM: CD_ORDEM,
        ID_ESCALA: ID_ESCALA,
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tecnicoClienteEntity),
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

function ExcluirConfirmar(CD_ORDEM) {
    ConfirmarSimNao('Aviso', 'Confirma a exclusão do registro?', 'Excluir(' + CD_ORDEM + ')');
}

function Excluir(CD_ORDEM) {
    var URL = URLAPI + "TecnicoXClienteAPI/Excluir";
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }
    //var token = sessionStorage.getItem("token");
    var tecnicoClienteEntity = {
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        tecnico: {
            CD_TECNICO: 0,
        },
        CD_ORDEM: CD_ORDEM,
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tecnicoClienteEntity),
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
