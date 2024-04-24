$(document).ready(function () {

    $('select').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ativoFixo_CD_ATIVO_FIXO').select2({
        placeholder: "Carregando...",
        allowClear: true
    });

    $('#DataInicio').mask('00/00/0000');
    $('#DataFim').mask('00/00/0000');
    $('.js-select-basic-single').val(null).trigger('change');
    //console.log($("#loader"))
    $("#loader").css("display", "block");
    carregarGridMVC();

    PopularAtivos();
    PopularTecnicos();
    PopularStatus();
});

$('#DT_FILTRO-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});


$('#btnConsultar').click(function () {
    carregarGridMVC();
});

function carregarGridMVC() {
    var ID_RELATORIO_RECLAMACAO = $('#ID_RELATORIO_RECLAMACAO').val();
    var dataInicio = $('#DataInicio').val();
    var dataFinal = $('#DataFim').val();
    var tipoReclamacao = $('#CD_TIPO_RECLAMACAO').val();
    var tipoAtendimento = $('#CD_TIPO_ATENDIMENTO').val();
    var codcliente = $('#NM_CLIENTE').val();
    var codTecnico = $('#ddlTecnico').val();
    var codPeca = $('#Peca').val();
    var codAtivo = $('#ativoFixo_CD_ATIVO_FIXO').val();
    var tempoAtendimento = $('#TEMPO_ATENDIMENTO').val();
    var Status = $('#Status').val();

    //var relatorioReclamacaoItemFiltro = new Object();

    //if (dataInicio != 'null' || dataInicio != null)
    //    relatorioReclamacaoItemFiltro.DataInicio = dataInicio

    //if (dataFinal != 'null' || dataFinal != null)
    //    relatorioReclamacaoItemFiltro.DataFim = dataFinal;

    //if (codcliente != 'null' || codcliente != null)
    //    relatorioReclamacaoItemFiltro.Cd_Cliente = codcliente;


    //if (codTecnico != 'null' || codTecnico != null)
    //    relatorioReclamacaoItemFiltro.CD_TECNICO = codTecnico;

    //if (tipoAtendimento != 'null' || tipoAtendimento != null)
    //    relatorioReclamacaoItemFiltro.tipoAtendimento = tipoAtendimento;

    //if (tipoReclamacao != 'null' || tipoReclamacao != null)
    //    relatorioReclamacaoItemFiltro.tipoReclamacao = tipoReclamacao;

    //if (status != 'null' || status != null)
    //    relatorioReclamacaoItemFiltro.status = status;

    //if (codPeca != 'null' || codPeca != null)
    //    relatorioReclamacaoItemFiltro.Peca= codPeca;


    //if (codAtivo != 'null' || codAtivo != null)
    //    relatorioReclamacaoItemFiltro.Ativo =codAtivo;



    var relatorioReclamacaoItemFiltro = {
        DataInicio: dataInicio,
        DataFim: dataFinal,
        Status: Status,
        // TipoReclamacaoRR: tipoReclamacao,
        //TipoAtendimento: tipoAtendimento,
        CD_PECA: codPeca,
        CD_ATIVO: codAtivo,
        CD_TECNICO: codTecnico
    };

    //  var URL = actionConsultar + "?dataInicio=" + dataInicio + "&dataFinal=" + dataFinal + "&codcliente=" + codcliente + "&codTecnico=" + codTecnico + "&tipoAtendimento=" + tipoAtendimento + "&tipoReclamacao=" + tipoReclamacao + "&status=" + status + "&codPeca=" + codPeca + "&codAtivo=" + codAtivo;
    //var URL = actionConsultar + "?Status=" + Status;

    // atribuirParametrosPaginacao("gridmvc", actionConsultar, JSON.stringify(relatorioReclamacaoItemFiltro));
    $.ajax({
        type: 'GET',
        url: actionConsultar,
        // url: actionConsultar,
        data: relatorioReclamacaoItemFiltro,
        dataType: "json",
        cache: false,
        contentType: 'application/json',
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            if (data.Status == 'Success') {
                $('#gridMvc').html(data.Html);
                $('.grid-mvc').gridmvc();
            }
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }

    });
}


function PopularTecnicos() {

    //  LimparCombo($("#TecSolicitante"));
    //  LimparCombo($("#TecSolicitante"));
    LimparCombo($("#ddlTecnico"));

    var URL = URLAPI + "TecnicoAPI/ObterListaTecnicoCampo";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
            /// $('#ativoFixo_TecSolicitante').prop('disabled', true)

        },
        complete: function () {
            $("#loader").css("display", "none");

        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.listaTecnico != null) {
                LoadTecnicos(res.listaTecnico);
            }
            //$("#loader").css("display", "none");
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });


}


function PopularStatus() {

    LimparCombo($("#Status"));

    var URL = URLAPI + "RRStatusAPI/ObterListaStatusRR";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
            /// $('#ativoFixo_TecSolicitante').prop('disabled', true)

        },
        complete: function () {

        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.listaRRStatus != null) {
                LoadStatus(res.listaRRStatus);
            }
            //$("#loader").css("display", "none");
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });


}



function PopularAtivos() {

    LimparCombo($("#ativoFixo_CD_ATIVO_FIXO"));

    var URL = URLAPI + "AtivoAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $('#ativoFixo_CD_ATIVO_FIXO').prop('disabled', true)

        },
        complete: function () {
            $("#ativoFixo_CD_ATIVO_FIXO").select2({
                placeholder: "Selecione"
            }).prop('disabled', false)

        },
        success: function (res) {
            if (res.listaAtivosFixos != null) {
                LoadAtivos(res.listaAtivosFixos);
            }
            //$("#loader").css("display", "none");
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}



function LoadAtivos(listaAtivosFixos) {
    LimparCombo($("#ativoFixo_CD_ATIVO_FIXO"));

    var listaAtivosFixos = JSON.parse(listaAtivosFixos);

    for (i = 0; i < listaAtivosFixos.length; i++) {
        MontarCombo($("#ativoFixo_CD_ATIVO_FIXO"), listaAtivosFixos[i].CD_ATIVO_FIXO, listaAtivosFixos[i].DS_ATIVO_FIXO)
    }
}
function LoadStatus(listaRRStatus) {
    LimparCombo($("#Status"));

    var listaRRStatus = JSON.parse(listaRRStatus);

    for (i = 0; i < listaRRStatus.length; i++) {
        MontarCombo($("#Status"), listaRRStatus[i].ST_STATUS_RR, listaRRStatus[i].DS_STATUS_NOME);
    }
}


function LoadTecnicos(listTecnico) {
    LimparCombo($("#TecSolicitante"));

    var listTecnico = JSON.parse(listTecnico);

    for (i = 0; i < listTecnico.length; i++) {
        MontarCombo($("#ddlTecnico"), listTecnico[i].CD_TECNICO, listTecnico[i].NM_TECNICO);
    }

}

$('#btnImprimir').click(function () {
    var dataInicio = $('#DataCadastroInicio').val();
    var dataFinal = $('#DataCadastroFim').val();
    // var codigoPedido = $('#CodigoPedido').val();
    // var ID_RELATORIO_RECLAMACAO = $('#ID_RELATORIO_RECLAMACAO').val();
    var tipoReclamacao = $('#CD_TIPO_RECLAMACAO').val();
    var tipoAtendimento = $('#CD_TIPO_ATENDIMENTO').val();
    var codcliente = $('#NM_CLIENTE').val();
    var codTecnico = $('#ddlTecnico').val();
    var codPeca = $('#Peca').val();
    var codAtivo = $('#ativoFixo_CD_ATIVO_FIXO').val();
    var tempoAtendimento = $('#TEMPO_ATENDIMENTO').val();
    var status = $('#Status').val();


    if (dataInicio == 'null' || dataInicio == null)
        dataInicio = '';

    if (dataFinal == 'null' || dataFinal == null)
        dataFinal = '';

    if (codcliente == 'null' || codcliente == null)
        codcliente = '';


    if (codTecnico == 'null' || codTecnico == null)
        codTecnico = '';

    if (tipoAtendimento == 'null' || tipoAtendimento == null)
        tipoAtendimento = '';

    if (tipoReclamacao == 'null' || tipoReclamacao == null)
        tipoReclamacao = '';

    if (status == 'null' || status == null)
        status = '';

    if (codPeca == 'null' || codPeca == null)
        codPeca = '';


    if (codAtivo == 'null' || codAtivo == null)
        codAtivo = '';



    var URL = URLCriptografarChave + "?Conteudo=" +
        dataInicio + "|" + dataFinal + "|" + codcliente + "|" + codTecnico + "|" + tipoAtendimento + "|" + tipoReclamacao + "|" + status + "|" + codPeca + "|" + codAtivo;

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
            if (res.idKey != null && res.idKey != '') {
                window.open(URLSite + '/RelatorioReclamacao.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }

    });

});