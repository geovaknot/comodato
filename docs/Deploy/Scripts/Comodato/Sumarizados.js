$('#DT_DATAS-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_INICIAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_FINAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

jQuery(document).ready(function () {
    //$('.js-example-basic-single').select2({ minimumInputLength: 1 });
    $('.js-example-basic-single').select2();
    $('.js-example-basic-multiple').select2();

    $('#DT_INICIAL').mask('00/00/0000');
    $('#DT_FINAL').mask('00/00/0000');
    //$('#DT_DEV_FINAL').val(Date.now.toString("dd/MM/yyyy"));
});

$('#btnLimpar').click(function () {
    $('#DT_INICIAL').val('');
    $('#DT_FINAL').val('');
});

$('#btnImprimir').click(function () {

    var DT_INICIAL = $('#DT_INICIAL').val();
    var DT_FINAL = $('#DT_FINAL').val();

    var check = checarDatas(DT_INICIAL, DT_FINAL);

    if (check) {
        //window.open(URLSite + '/RelatorioSumarizados.aspx?idKey=' + DT_INICIAL + "|" + DT_FINAL + "|", '_blank');

        var URL = URLCriptografarChave + "?Conteudo=" + DT_INICIAL + "|" + DT_FINAL;

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
                    window.open(URLSite + '/RelatorioSumarizados.aspx?idKey=' + res.idKey, '_blank');
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });
    }
});

function checarDatas(DT_DEV_INICIAL, DT_DEV_FINAL) {
    var data_1 = new Date(formatarData(DT_DEV_INICIAL));
    var data_2 = new Date(formatarData(DT_DEV_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        alert("A data inicial não pode ser maior que a data final!");
        return false;
    //} else if (data_1.getFullYear() < data.getFullYear() - 5) {
    //    alert("A data inicial excedeu o limite de 5 anos!");
    //    return false;
    //} else if (data_2.getFullYear() < data.getFullYear() - 5) {
    //    alert("A data final excedeu o limite de 5 anos!");
    //    return false;
    } else if (data_1 > data) {
        alert("A data inicial não pode ser posterior a hoje!");
        return false;
    } else if (data_2 > data) {
        alert("A data final não pode ser posterior a hoje!");
        return false;
    } else {
        return true;
    }
}

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}
