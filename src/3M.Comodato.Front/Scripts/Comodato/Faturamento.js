function EditarFaturamento(idFaturamento) {
    var ID = idFaturamento;

    var URL = URLAPI + "AtivoClienteAPI/Obter?ID=" + ID;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: { ID: ID },
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.faturamentoEntity != null) {
                $('#cdMaterial').val(res.faturamentoEntity.CD_Material);
                $('#deptoVenda').val(res.faturamentoEntity.DepartamentoVenda);
                $('#aluguel').val(res.faturamentoEntity.AluguelApos3anos);
                $('#dtUltimoFat').val(res.faturamentoEntity.DT_UltimoFaturamento);
                $('#modalFat').modal('show');
                
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}