jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    carregarComboPeriodo()
});

function carregarComboPeriodo() {
    var URL = URLAPI + "PlanoZeroAPI/Periodos"

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
            if (res.periodos != null) {
                LoadPeriodos(res.periodos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadPeriodos(periodos) {

    for (i = 0; i < periodos.length; i++) {
        MontarCombo($("#Periodos"), periodos[i], periodos[i]);
    }
    
}

function validarCampo(id){
    return ($(id).val() != null && $(id).val() != undefined && $(id).val() != '')
}

$('#btnLimpar').click(function () {
    $('#Periodos').val(null).trigger('change');
    $('#selectModeloRelatorio').val(null).trigger('change');
    var texto_erro = document.getElementById('text-erro')
    
    texto_erro.setAttribute("hidden", "")
});

$('#btnImprimir').click(function () {
    var periodoValido = validarCampo('#Periodos')
    var modeloValido = validarCampo('#selectModeloRelatorio')
    var texto_erro = document.getElementById('text-erro')
    
    if(periodoValido && modeloValido){
        texto_erro.setAttribute("hidden", "")
    } else {
        texto_erro.removeAttribute("hidden")
    }
});