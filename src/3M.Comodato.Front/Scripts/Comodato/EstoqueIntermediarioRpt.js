
$(document).ready(function () {
    $('#ddlEstoque').select2({
        placeholder: "Selecione...",
        allowClear: true
    });
    $('#ddlPeca').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    popularComboEstoque();
    popularComboPecas();
});

$('#estoqueAtivo').change(function () {
    popularComboEstoque();
});

$('#pecaAtivo').click(function () {
    popularComboPecas();
});

//function popularComboEstoque() {
//    var ativo;
//    if ($('#estoqueAtivo').is(":checked")) {
//        ativo = "S";
//    }
//    else {
//        ativo = "N";
//    }

//    $.ajax({
//        type: 'GET',
//        url: URLAPI + "EstoqueIntermediarioAPI/ObterLista?ativo=" + ativo + "&ID_Usuario=" + nidUsuario,
//        //data: JSON.stringify(estoqueEntity),
//        dataType: "json",
//        cache: false,
//        contentType: "application/json",
//        success: function (res) {
//            $("#loader").css("display", "none");
//            if (res.estoques != null) {
//                preencherComboEstoque(res.estoques);

//                if (UsuarioTecnico.toLowerCase() === 'true') {
//                    $("#ddlEstoque").prop("disabled", true);
//                    $("#ddlEstoque").val(res.estoques[0].CD_ESTOQUE).trigger('change');
//                }
//                else {
//                    $("#ddlEstoque").prop("disabled", false);
//                    $('#ddlEstoque').val('').trigger('change');
//                }
//            }
//        },
//        error: function (res) {
//            $("#loader").css("display", "none");
//            Alerta("ERRO", res.responseText);
//        }
//    });
//}
function popularComboEstoque() {
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

                
            }


        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function preencherComboEstoque(estoques) {
    LimparCombo($("#ddlEstoque"));

    for (i = 0; i < estoques.length; i++) {
        MontarCombo($("#ddlEstoque"), estoques[i].CD_ESTOQUE, estoques[i].DS_ESTOQUE);
    }
}

function popularComboPecas() {
    var filtroPeca = new Object();
    if ($('#pecaAtivo').is(":checked")) {
        filtroPeca.FL_ATIVO_PECA = "S";
    }
    else {
        filtroPeca.FL_ATIVO_PECA = "N";
    }
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "PecaApi/ObterLista",
        dataType: 'json',
        data: JSON.stringify(filtroPeca),
        cache: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            var listaPecas = JSON.parse(data.PECA);

            LimparCombo($("#ddlPeca"));
            for (i = 0; i < listaPecas.length; i++) {
                var codigoPeca = listaPecas[i].CD_PECA;
                var descricaoPeca = listaPecas[i].DS_PECA;

                MontarCombo($("#ddlPeca"), codigoPeca, descricaoPeca);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

$('#btnImprimir').click(function () {
    var tipoRelatorio = $("input[type='radio']:checked").val();
    var codigoEstoque = $("#ddlEstoque").val();
    var codigoPeca = $("#ddlPeca").val();

    var estoqueAtivo = "N";
    if ($('#estoqueAtivo').is(":checked")) {
        estoqueAtivo = "S";
    }

    var pecaAtiva = "N";
    if ($('#pecaAtivo').is(":checked")) {
        pecaAtiva = "S";
    }
    //var parametros = tipoRelatorio + "|" + codigoTecnico + "|" + codigoPeca + '|' + tecnicoAtivo + "|" + pecaAtiva;
    //window.open(actionRelatorio + '?idKey=' + parametros, '_blank');

    var URL = URLCriptografarChave + "?Conteudo=" + tipoRelatorio + "|" + codigoEstoque + "|" + codigoPeca + '|' + estoqueAtivo + "|" + pecaAtiva;

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
                window.open(URLSite + '/RelatorioEstoqueIntermediario.aspx?idKey=' + res.idKey, '_blank');
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
    $("#estoqueAtivo").prop('checked', true);
    $("#pecaAtivo").prop('checked', true);
    if (UsuarioTecnico.toLowerCase() !== 'true') {
        $('#ddlEstoque').val(null).trigger('change');
    }
    $('#ddlPeca').val(null).trigger('change');
});