$().ready(function () {
    $('.js-select-basic-single').select2();

    popularEstoque();
    popularComboPecas();
    //carregarGridMVC();
    
    ConfigurarBotoesAcao();
});

function popularEstoque() {
    var estoque = new Object();
    estoque.ID_USU_RESPONSAVEL = nidUsuario;
    estoque.FL_ATIVO = "S";
    estoque.Web = 1;
    if (UsuarioTecnico.toLowerCase() === 'true') {
        estoque.Tecnico = 1;
    }
    LimparCombo($('#ddlEstoque'));
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "EstoqueAPI/ObterListaEstoque",
        data: JSON.stringify(estoque),
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.ESTOQUE != null) {
                var listaEstoque = JSON.parse(data.ESTOQUE);
                for (i = 0; i < listaEstoque.length; i++) {
                    MontarCombo($('#ddlEstoque'), listaEstoque[i].ID_ESTOQUE, listaEstoque[i].CD_ESTOQUE + ' ' + listaEstoque[i].DS_ESTOQUE);
                }

                //if (UsuarioTecnico.toLowerCase() === 'true') {
                //    $("#ddlEstoque").prop("disabled", true);
                //    $("#ddlEstoque").val(listaEstoque[0].ID_ESTOQUE).trigger('change');
                //}
                //else {
                //    $("#ddlEstoque").prop("disabled", false);
                //    $('#ddlEstoque').val('').trigger('change');
                //}
                
            }

            
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function popularComboPecas() {
    var comboPecasEstoque = $('#ddlPeca');

    LimparCombo(comboPecasEstoque);
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + 'PecaAPI/ObterLista',
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");

            if (data.PECA != null) {
                var listaEstoque = JSON.parse(data.PECA);
                for (i = 0; i < listaEstoque.length; i++) {
                    MontarCombo(comboPecasEstoque, listaEstoque[i].CD_PECA, listaEstoque[i].DS_PECA);
                }
            }
            comboPecasEstoque.val('').trigger('change');
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

$('#btnConsultar').click(function () {
    carregarGridMVC();
});

function carregarGridMVC() {
    var nidEstoque = $('#ddlEstoque').val();
    var ccdPeca = $('#ddlPeca').val();
    var flagAtivo = $('#ddlAtivo').val();

    var estoquePeca = new Object();
    estoquePeca.Estoque = new Object();
    estoquePeca.Peca = new Object();

    if (nidEstoque != "" && nidEstoque != "0" && nidEstoque != 0) {
        estoquePeca.Estoque.nidEstoque = nidEstoque;
    }
    if (ccdPeca != "" && ccdPeca != "0" && ccdPeca != 0) {
        estoquePeca.Peca.CD_PECA = ccdPeca;
    }
    estoquePeca.Peca.FL_ATIVO_PECA = flagAtivo;

    atribuirParametrosPaginacao("gridEstoquePecas", actionConsultar, JSON.stringify(estoquePeca));

    $.ajax({
        type: 'POST',
        url: actionConsultar,
        data: JSON.stringify(estoquePeca),
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.Status == 'Success') {
                $('#gridEstoquePecas').html(data.Html);
                $('.grid-mvc').gridmvc();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}