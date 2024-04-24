$().ready(function () {
    
    //Necessário para 'focus'
    $('.js-select-basic-m1').select2({ dropdownParent: $('#MovimentacaoAcao1') });
    $('.js-select-basic-m2').select2({ dropdownParent: $('#MovimentacaoAcao2') });
    $('.js-select-basic-m3').select2({ dropdownParent: $('#MovimentacaoAcao3'), matcher: matchCustom });
    //
    $('.js-select-basic-single').select2();
    $('#ddlEstoque').select2();
    OcultarCampo($('#validaComboEstoque'));
    OcultarCampo($('#validaComboAcao'));

    $('#txtQuantidade', '#MovimentacaoAcao2').ForceNumericOnly();
    $('#txtQuantidade', '#MovimentacaoAcao3').ForceNumericOnly();

    $('#PeriodoDe').mask('00/00/0000');
    $('#PeriodoAte').mask('00/00/0000');


    if ($('#ddlEstoque > option').length == 2) {
        $("#ddlEstoque").prop("selectedIndex", 1).trigger('change');
    }

    $('#txtQuantidade', '#MovimentacaoAcao2').keypress(function (e) {
        if (e.keyCode == 13)
            $('#btnSalvar', '#MovimentacaoAcao2').click();
    });

    $('#txtQuantidade', '#MovimentacaoAcao3').keypress(function (e) {
        if (e.keyCode == 13)
            $('#btnSalvar', '#MovimentacaoAcao3').click();
    });
});

function matchCustom(params, data) {
    if ($.trim(params.term) === '') {
        return data;
    }

    if (typeof data.text === 'undefined') {
        return null;
    }

    var term = params.term;
    if (term.length > 11) {
        term = term.substring(0, 11);
    }

    if (data.text.indexOf(term) > -1) {
        return data;
    }
    return null;
}

$('#ddlEstoque').on('select2:select', function (e) {
    OcultarCampo($('#validaComboEstoque'));
});

//1-Mov. Completo de Estoque
$('#ddlEstoqueDe', '#MovimentacaoAcao1').on('select2:select', function (e) {
    $("#ddlEstoque").val(e.params.data.id).trigger('change');
});

//2-Mov. Peças entre Estoque
$('#ddlEstoqueDe', '#MovimentacaoAcao2').on('select2:select', function (e) {
    popularComboPecas(e.params.data.id, $('#ddlPeca', '#MovimentacaoAcao2'), false);
    atualizarQuantidadeEstoque2($('#ddlPeca', '#MovimentacaoAcao2').val());
    $("#ddlEstoque").val(e.params.data.id).trigger('change');
});

$('#ddlEstoquePara', '#MovimentacaoAcao2').on('select2:select', function (e) {
    atualizarQuantidadeEstoque2($('#ddlPeca', '#MovimentacaoAcao2').val());
});

$('#ddlPeca', '#MovimentacaoAcao2').on('select2:select', function (e) {
    atualizarQuantidadeEstoque2(e.params.data.id);
});

function atualizarQuantidadeEstoque2(ccdPeca) {
    var nidEstoqueDe = $('#ddlEstoqueDe', '#MovimentacaoAcao2').val();
    var nidEstoquePara = $('#ddlEstoquePara', '#MovimentacaoAcao2').val();
    var txtQuantidade = $('#txtQuantidadePara', '#MovimentacaoAcao2');

    txtQuantidade.val('');
    if (ccdPeca == '') {
        return;
    }

    if (nidEstoqueDe != '') {
        exibirQuantidadeEmEstoque($('#txtQuantidadeDe', '#MovimentacaoAcao2'), nidEstoqueDe, ccdPeca);
    }

    if (nidEstoquePara != '') {
        exibirQuantidadeEmEstoque(txtQuantidade, nidEstoquePara, ccdPeca);
    }
}

//3-Ajuste de Entrada de Estoque
//4-Ajuste de Saída de Estoque
$('#ddlEstoque', '#MovimentacaoAcao3').on('select2:select', function (e) {
    popularComboPecas(e.params.data.id, $('#ddlPeca', '#MovimentacaoAcao3'), true);
    atualizarQuantidadeEstoque3($('#ddlPeca', '#MovimentacaoAcao3').val());
    $("#ddlEstoque").val(e.params.data.id).trigger('change');
});


$('#ddlPeca', '#MovimentacaoAcao3').on('select2:select', function (e) {
    atualizarQuantidadeEstoque3(e.params.data.id);
    $('#txtQuantidade', '#MovimentacaoAcao3').focus();
});



function atualizarQuantidadeEstoque3(ccdPeca) {
    //if (ccdPeca.length > 11) {
    //    ccdPeca = ccdPeca.substring(0, 11);
    //}
    var nidEstoque = $('#ddlEstoque', '#MovimentacaoAcao3').val();
    var txtQuantidade = $('#txtQuantidadeDe', '#MovimentacaoAcao3');

    txtQuantidade.val('');

    if (ccdPeca == '') {
        return;
    }

    if (nidEstoque != '') {
        exibirQuantidadeEmEstoque(txtQuantidade, nidEstoque, ccdPeca);
    }
}



$('#ddlAcao').on('select2:select', function (e) {
    OcultarCampo($('#validaComboAcao'));

    var modalTarget = '#MovimentacaoAcao';
    if (e.params.data.id == '3' || e.params.data.id == '4') {
        modalTarget += '3';
    }
    else {
        modalTarget += e.params.data.id;
    }

    $('#hTitulo', modalTarget).text($('#ddlAcao').children(':selected').text());
    $('#btnAcao').attr('data-target', modalTarget);
});

$('#btnAcao').click(function () {
    if ($('#ddlAcao').val() == '') {
        ExibirCampo($('#validaComboAcao'));
    }

    limparCamposModal();
    popularCombosMovimentacaoModal();
});

function selecionarEstoqueInicial() {
    selecionarValorPadrao();

    var estoqueSelecionado = $('#ddlEstoque').val();

    $('#ddlEstoqueDe', '#MovimentacaoAcao1').prop("disabled", false);
    $('#ddlEstoqueDe', '#MovimentacaoAcao2').prop("disabled", false);
    $('#ddlEstoque', '#MovimentacaoAcao3').prop("disabled", false);

    if (estoqueSelecionado != '') {
        var codigoAcao = $('#ddlAcao').val();

        if (codigoAcao == '1') {
            $('#ddlEstoqueDe', '#MovimentacaoAcao1').prop("disabled", true);
            $('#ddlEstoqueDe', '#MovimentacaoAcao1').val(estoqueSelecionado).trigger('change');

        } else if (codigoAcao == '2') {
            $('#ddlEstoqueDe', '#MovimentacaoAcao2').prop("disabled", true);
            $('#ddlEstoqueDe', '#MovimentacaoAcao2').val(estoqueSelecionado).trigger('change');
            popularComboPecas(estoqueSelecionado, $('#ddlPeca', '#MovimentacaoAcao2'), false);
        }
        else if (codigoAcao == '3' || codigoAcao == '4') {
            $('#ddlEstoque', '#MovimentacaoAcao3').prop("disabled", true);
            $('#ddlEstoque', '#MovimentacaoAcao3').val(estoqueSelecionado).trigger('change');
            popularComboPecas(estoqueSelecionado, $('#ddlPeca', '#MovimentacaoAcao3'), true);
        }
    }
}

$('#btnImprimir').click(function () {
    var nidEstoque = $("#ddlEstoque").val();
    if (nidEstoque == "" || nidEstoque == "0" || nidEstoque == 0) {
        ExibirCampo($('#validaComboEstoque'));
        return;
    }
    window.open(actionImprimir + '?idKey=' + nidEstoque, '_blank');
});

$("#btnConsultarEstoque").click(function () {
    popularGridEstoque();
});

function popularGridEstoque() {
    var nidEstoque = $("#ddlEstoque").val();
    if (nidEstoque == "" || nidEstoque == "0" || nidEstoque == 0) {
        ExibirCampo($('#validaComboEstoque'));
        return;
    }

    var url = actionConsultarEstoque + "?codigoEstoque=" + nidEstoque;
    atribuirParametrosPaginacao("gridConsulta", url, '{"codigoEstoque":"' + nidEstoque + '"}');

    $.ajax({
        url: url,
        processData: true,
        cache: false,
        dataType: "json",
        contentType: "application/json; charset=utf=8"

    }).done(function (e) {
        $("#loader").css("display", "none");
        if (e.Status == "Success") {
            $('#gridConsulta').html(e.Html);
            $(".grid-mvc").gridmvc();
        }
    });
}

function limparCamposModal() {
    LimparCombo($('#ddlEstoqueDe', '#MovimentacaoAcao1'));
    LimparCombo($('#ddlEstoquePara', '#MovimentacaoAcao1'));
    LimparCombo($('#ddlEstoqueDe', '#MovimentacaoAcao2'));
    LimparCombo($('#ddlEstoquePara', '#MovimentacaoAcao2'));
    LimparCombo($('#ddlEstoque', '#MovimentacaoAcao3'));
    LimparCombo($('#ddlPeca', '#MovimentacaoAcao2'));
    LimparCombo($('#ddlPeca', '#MovimentacaoAcao3'));

    $('#txtQuantidadeDe', '#MovimentacaoAcao2').val('');
    $('#txtQuantidadePara', '#MovimentacaoAcao2').val('');
    $('#txtQuantidade', '#MovimentacaoAcao2').val('');

    $('#txtQuantidadeDe', '#MovimentacaoAcao3').val('');
    $('#txtQuantidade', '#MovimentacaoAcao3').val('');

    OcultarCampo($('#validaComboQuantidade', '#MovimentacaoAcao2'));
    OcultarCampo($('#validaComboQuantidade', '#MovimentacaoAcao3'));
}

function selecionarValorPadrao() {
    $('#ddlEstoqueDe', '#MovimentacaoAcao1').val('').trigger('change');
    $('#ddlEstoquePara', '#MovimentacaoAcao1').val('').trigger('change');
    $('#ddlEstoqueDe', '#MovimentacaoAcao2').val('').trigger('change');
    $('#ddlEstoquePara', '#MovimentacaoAcao2').val('').trigger('change');
    $('#ddlEstoque', '#MovimentacaoAcao3').val('').trigger('change');

    $('#ddlPeca', '#MovimentacaoAcao2').val('').trigger('change');
    $('#ddlPeca', '#MovimentacaoAcao3').val('').trigger('change');
}

function popularCombosMovimentacaoModal() {
    var estoque = new Object();
    estoque.ID_USU_RESPONSAVEL = nidUsuario;
    estoque.FL_ATIVO = "S";
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
            limparCamposModal();
            if (data.ESTOQUE != null) {
                var listaEstoque = JSON.parse(data.ESTOQUE);

                for (i = 0; i < listaEstoque.length; i++) {
                    MontarCombo($('#ddlEstoqueDe', '#MovimentacaoAcao1'), listaEstoque[i].ID_ESTOQUE, listaEstoque[i].CD_ESTOQUE);
                    MontarCombo($('#ddlEstoquePara', '#MovimentacaoAcao1'), listaEstoque[i].ID_ESTOQUE, listaEstoque[i].CD_ESTOQUE);
                    MontarCombo($('#ddlEstoqueDe', '#MovimentacaoAcao2'), listaEstoque[i].ID_ESTOQUE, listaEstoque[i].CD_ESTOQUE);
                    MontarCombo($('#ddlEstoquePara', '#MovimentacaoAcao2'), listaEstoque[i].ID_ESTOQUE, listaEstoque[i].CD_ESTOQUE);
                    MontarCombo($('#ddlEstoque', '#MovimentacaoAcao3'), listaEstoque[i].ID_ESTOQUE, listaEstoque[i].CD_ESTOQUE);
                }
            }

            selecionarEstoqueInicial();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function popularComboPecas(nidEstoque, comboPecasEstoque, abrirPopupSelect2) {

    LimparCombo(comboPecasEstoque);
    if (nidEstoque == '') {
        comboPecasEstoque.val('').trigger('change');
        return;
    }

    var url = URLAPI + 'EstoquePecaAPI/ObterListaPecasPorEstoque?nidEstoque=' + nidEstoque + '&nidUsuario=' + nidUsuario;
    if ($('#ddlAcao').val() == '3') {
        url = URLAPI + 'PecaAPI/ObterListaAtivos';
    }

    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: url,
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
            if (abrirPopupSelect2 == true) {

                setTimeout(function () {
                    comboPecasEstoque.select2('open');//.focus(function () { $(this).select2('focus'); });
                }, 1150);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });


}

function exibirQuantidadeEmEstoque(controle, nidEstoque, ccdPeca) {
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + 'EstoquePecaAPI/ObterQuantidadeEmEstoque?nidEstoque=' + nidEstoque + '&ccdPeca=' + ccdPeca,
        dataType: 'json',
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
            if (null != data.QTD_PECAS) {
                $(controle).val(JSON.parse(data.QTD_PECAS));
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function criarParametroMovimentacao(tipoMovimentacao, idEstoqueOrigem, idEstoqueDestino, codigoPeca, quantidade) {
    var estoqueMovi = new Object();

    estoqueMovi.TP_MOVIMENTACAO = new Object();
    estoqueMovi.TP_MOVIMENTACAO.CD_TP_MOVIMENTACAO = tipoMovimentacao;

    if (idEstoqueOrigem != "") {
        estoqueMovi.ESTOQUE_ORIGEM = new Object();
        estoqueMovi.ESTOQUE_ORIGEM.ID_ESTOQUE = idEstoqueOrigem;
    }

    if (idEstoqueDestino != "") {
        estoqueMovi.ESTOQUE_DESTINO = new Object();
        estoqueMovi.ESTOQUE_DESTINO.ID_ESTOQUE = idEstoqueDestino;
    }

    if (codigoPeca != "") {
        estoqueMovi.Peca = new Object();
        estoqueMovi.Peca.CD_PECA = codigoPeca;
    }

    if (quantidade != "") {
        estoqueMovi.QT_PECA = quantidade;
    }

    estoqueMovi.USU_MOVI = new Object();
    estoqueMovi.USU_MOVI.nidUsuario = nidUsuario;

    estoqueMovi.nidUsuarioAtualizacao = nidUsuario;

    return estoqueMovi;
}

//Movimentação Total
function fnSalvarTransferenciaEstoque() {
    var tipoMovimentacao = $('#ddlAcao').val();
    var estoqueOrigem = $('#ddlEstoqueDe', '#MovimentacaoAcao1').val();
    var estoqueDestino = $('#ddlEstoquePara', '#MovimentacaoAcao1').val();
    var atualizarGrid = false;

    if (estoqueOrigem == estoqueDestino) {
        Alerta('Estoque Inválido', 'O estoque de destino deve ser diferente do estoque de destino.');
        return false;
    }

    if (estoqueOrigem == $('#ddlEstoque').val()) {
        atualizarGrid = true;
    }

    var estoqueMovimentacao =
        criarParametroMovimentacao(tipoMovimentacao, estoqueOrigem, estoqueDestino, "", "");

    this.fnSalvar(estoqueMovimentacao, $('#btnFechar', '#MovimentacaoAcao1'), atualizarGrid);
    $('#btnFechar', '#MovimentacaoAcao1').click();
}

//Movimentação Entre Estoques
function fnSalvarMovimentacaoEstoque() {
    var tipoMovimentacao = $('#ddlAcao').val();
    var estoqueOrigem = $('#ddlEstoqueDe', '#MovimentacaoAcao2').val();
    var estoqueDestino = $('#ddlEstoquePara', '#MovimentacaoAcao2').val();
    var codigoPeca = $('#ddlPeca', '#MovimentacaoAcao2').val();
    var quantidade = $('#txtQuantidade', '#MovimentacaoAcao2').val();
    var quantidadeDe = $('#txtQuantidadeDe', '#MovimentacaoAcao2').val();
    var atualizarGrid = false;

    if (estoqueOrigem == estoqueDestino) {
        Alerta('Estoque Inválido', 'O estoque de destino deve ser diferente do estoque de destino.');
        return false;
    }

    if (quantidade == "" || quantidade == "0" || quantidade == 0) {
        ExibirCampo($('#validaComboQuantidade', '#MovimentacaoAcao2'));
        return false;
    }

    if (quantidadeDe == '') {
        quantidadeDe = 0;
    }

    if (parseInt(quantidade) > parseInt(quantidadeDe)) {
        Alerta("ERRO", "Não há estoque origem suficiente");
        return false;
    }

    if (estoqueOrigem == $('#ddlEstoque').val()) {
        atualizarGrid = true;
    }

    //if (codigoPeca.length > 11) {
    //    codigoPeca = codigoPeca.substring(0, 11);
    //}

    var estoqueMovimentacao =
        criarParametroMovimentacao(tipoMovimentacao, estoqueOrigem, estoqueDestino, codigoPeca, quantidade);

    this.fnSalvar(estoqueMovimentacao, $('#btnFechar', '#MovimentacaoAcao2'), atualizarGrid);

    //Limpa alguns campos após salvar
    LimparCombo($('#ddlPeca', '#MovimentacaoAcao2'));

    $('#txtQuantidadeDe', '#MovimentacaoAcao2').val('');
    $('#txtQuantidadePara', '#MovimentacaoAcao2').val('');
    $('#txtQuantidade', '#MovimentacaoAcao2').val('');

}

//Ajuste de Estoque
function fnSalvarMovimentacaoTecnico() {
    var tipoMovimentacao = $('#ddlAcao').val();
    var estoque = $('#ddlEstoque', '#MovimentacaoAcao3').val();
    var codigoPeca = $('#ddlPeca', '#MovimentacaoAcao3').val();
    var quantidade = $('#txtQuantidade', '#MovimentacaoAcao3').val();
    var quantidadeDe = $('#txtQuantidadeDe', '#MovimentacaoAcao3').val();
    var atualizarGrid = false;
    var estoqueDe = '';
    var estoquePara = '';

    if (quantidade == "" || quantidade == "0" || quantidade == 0) {
        ExibirCampo($('#validaComboQuantidade', '#MovimentacaoAcao3'));
        return false;
    }

    if (tipoMovimentacao == '3') { //Ajuste de Entrada de Estoque
        estoquePara = estoque;
    }
    else if (tipoMovimentacao == '4') { //Ajuste de Saida de Estoque
        estoqueDe = estoque;

        if (quantidadeDe == '') {
            quantidadeDe = '0';
        }

        if (parseInt(quantidade) > parseInt(quantidadeDe)) {
            Alerta("ERRO", "Não há estoque origem suficiente");
            return false;
        }
    }
    if (estoque == $('#ddlEstoque').val()) {
        atualizarGrid = true;
    }

    //if (codigoPeca.length > 11) {
    //    codigoPeca = codigoPeca.substring(0, 11);
    //}

    var estoqueMovimentacao =
        criarParametroMovimentacao(tipoMovimentacao, estoqueDe, estoquePara, codigoPeca, quantidade);

    this.fnSalvar(estoqueMovimentacao, $('#btnFechar', '#MovimentacaoAcao3'), atualizarGrid);

    //Limpa alguns campos após salvar
    LimparCombo($('#ddlPeca', '#MovimentacaoAcao3'));

    $('#txtQuantidadeDe', '#MovimentacaoAcao3').val('');
    $('#txtQuantidade', '#MovimentacaoAcao3').val('');

}

function fnSalvar(estoqueMovi, seletorFechar, atualizarGrid) {
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "MovimentacaoPecaAPI/MovimentarEstoque",
        data: JSON.stringify(estoqueMovi),
        cache: false,
        dataType: 'json',
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
            //Alerta("Aviso", MensagemGravacaoSucesso);
            //seletorFechar.click();

            if (atualizarGrid == true) {
                popularGridEstoque();
            }

            selecionarEstoqueInicial();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}