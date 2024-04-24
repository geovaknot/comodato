function carregarComboTecnico() {
    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
    var tecnicoEntity = {};

    tecnicoEntity = {
        usuario: {
            nidUsuario: nidUsuario
        }
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(tecnicoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            ExibirCampo($('#loader'));
        },
        complete: function () {
            OcultarCampo($('#loader'));
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tecnicos != null) {
                LoadTecnicos(res.tecnicos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function LoadTecnicos(tecnicosJO) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    }
       
    if (UsuarioTecnico.toLowerCase() === 'true') {
        $("#tecnico_CD_TECNICO").prop("disabled", true);
        $('#tecnico_CD_TECNICO').val(tecnicos[0].CD_TECNICO).trigger('change');
    }
    else {
        $("#tecnico_CD_TECNICO").prop("disabled", false);
    }
}

