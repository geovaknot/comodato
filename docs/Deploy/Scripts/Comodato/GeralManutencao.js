jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#DT_INICIO').mask('00/00/0000');
    $('#DT_FIM').mask('00/00/0000');

    //$('#filtroAtual').val('Cliente');

});

$('#DT_FILTRO-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#btnImprimir').click(function () {
    var filtroAtual = $("#filtroAtual").val();
    var DT_INICIO = $("#DT_INICIO").val();
    var DT_FIM = $("#DT_FIM").val();
    var listaSelecionados = '';
    OcultarCampo($("#validaSelecionados"));

    switch (filtroAtual) {
        case "Cliente":
            $("INPUT[id*='ClientesSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = 'Todos';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            break;
        case "Grupo":
            $("INPUT[id*='GruposSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = 'Todos';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            break;
        case "Modelo":
            $("INPUT[id*='ModelosSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = 'Todos';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            break;
        case "Técnico":
            $("INPUT[id*='TecnicosSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = 'Todos';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            filtroAtual = "Tecnico";
            break;
        case "Peça":
            $("INPUT[id*='PecasSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = 'Todos';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            filtroAtual = "Peca";
            break;
        case "Equipamento":
            $("INPUT[id*='AtivosSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = 'Todos';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            break;
    }

    if (listaSelecionados == '') {
        ExibirCampo($("#validaSelecionados"));
        return false;
    }

    var URL = URLCriptografarChave + "?Conteudo=" + listaSelecionados + "|" + DT_INICIO + "|" + DT_FIM + "|" + filtroAtual;

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
                window.open(URLSite + '/RelatorioGeralManutencao.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

    //window.open(URLSite + '/RelatorioGeralManutencao.aspx?idKey=' + listaSelecionados + "|" + DT_INICIO + "|" + DT_FIM + "|" + filtroAtual, '_blank');

});

$('#btnMarcarCliente').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ClientesSelecionados']").prop('checked', true);
    $("INPUT[id*='ClientesSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarCliente').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ClientesSelecionados']").prop('checked', false);
    $("INPUT[id*='ClientesSelecionados']").prop('disabled', false);
});

$('#btnMarcarGrupo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='GruposSelecionados']").prop('checked', true);
    $("INPUT[id*='GruposSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarGrupo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='GruposSelecionados']").prop('checked', false);
    $("INPUT[id*='GruposSelecionados']").prop('disabled', false);
});

$('#btnMarcarModelo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ModelosSelecionados']").prop('checked', true);
    $("INPUT[id*='ModelosSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarModelo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ModelosSelecionados']").prop('checked', false);
    $("INPUT[id*='ModelosSelecionados']").prop('disabled', false);
});

$('#btnMarcarTecnico').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='TecnicosSelecionados']").prop('checked', true);
    $("INPUT[id*='TecnicosSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarTecnico').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='TecnicosSelecionados']").prop('checked', false);
    $("INPUT[id*='TecnicosSelecionados']").prop('disabled', false);
});

$('#btnMarcarPeca').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='PecasSelecionados']").prop('checked', true);
    $("INPUT[id*='PecasSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarPeca').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='PecasSelecionados']").prop('checked', false);
    $("INPUT[id*='PecasSelecionados']").prop('disabled', false);
});

$('#btnMarcarAtivo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='AtivosSelecionados']").prop('checked', true);
    $("INPUT[id*='AtivosSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarAtivo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='AtivosSelecionados']").prop('checked', false);
    $("INPUT[id*='AtivosSelecionados']").prop('disabled', false);
});

$('a[data-toggle="list"]').on('shown.bs.tab', function (e) {
    e.target // newly activated tab
    e.relatedTarget // previous active tab

    $('#filtroAtual').val(e.target.text);
})