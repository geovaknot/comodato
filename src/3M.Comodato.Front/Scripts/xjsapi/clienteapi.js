var ClienteAPI = {};

ClienteAPI.carregarKatCliente = function carregarKatCliente(clienteId) {
    return $.ajax({
        type: 'GET',
        url: URLSITE + 'Cliente/ObterKatPorCliente',
        data: { "clienteId": clienteId },
        dataType: "json",
        cache: false,
        //async: false,
        contentType: "json"
    });
};

ClienteAPI.ObterListaPerfilClienteAsync = function (nidUsuario, camposNecessarios)  {
   return $.ajax({
        type: 'GET',
        url: URLAPI + 'ClienteAPI/ObterListaPerfilCliente',
        dataType: "json",
        cache: false,
        //async: false,
        contentType: "json",
        data: {
            "nidUsuario": nidUsuario,
            "camposNecessarios": camposNecessarios
        }
    });
};

ClienteAPI.ObterListaPorUsuarioPerfilAsync = function (nidUsuario, camposNecessarios)  {
    return $.ajax({
        type: 'GET',
        url: URLAPI + 'ClienteAPI/ObterListaPorUsuarioPerfil',
        dataType: "json",
        cache: false,
        //async: false,
        contentType: "json",
        data: {
            "nidUsuario": nidUsuario,
            "camposNecessarios": camposNecessarios
        }
    });
};

//ClienteAPI.ObterListaPorUsuarioPerfil = async (nidUsuario, camposNecessarios) => {
//    return new Promise((resolve, reject) => {
//        $.ajax({
//            type: 'GET',
//            url: URLAPI + 'ClienteAPI/ObterListaPorUsuarioPerfil',
//            dataType: "json",
//            cache: false,
//            async: true,
//            contentType: "application/json",
//            data: {
//                "nidUsuario": nidUsuario,
//                "camposNecessarios": camposNecessarios
//            },
//            success: function (res) {
//                return resolve({
//                    'sucesso': true,
//                    'dados': res
//                })
//            },
//            error: function (res) {
//                return reject({
//                    'sucesso': false,
//                    'mensagem': res.responseText
//                })
//            }
//        });
//    })
//}
