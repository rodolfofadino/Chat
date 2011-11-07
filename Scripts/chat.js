$(document).ready(function () {
    // Post /Chat/New
    $('#btnEnviar').bind('click', function () {
        var msgVal = $('#txtMensagem').val();
        $('#txtMensagem').val('');
        $.post("/Chat/New", { nome: $('#txtNome').val(), msg: msgVal }, function (data, s) {
            if (data.d) {
                //mensagem adicionada
            }
            else {
                //erro ao adicionar
            }
           
        });
    });

    //Envia a mensagem com enter
    $('#txtMensagem').keydown(function (e) {
        if (e.keyCode == 13) {
            $('#btnEnviar').click();
        }
    });

    setTimeout(function () {
        getMensagens();
    }, 100)
});

function getMensagens() {
    $.post("/Chat", null, function (data, s) {
        if (data.mensagens) {
            $('#msgTmpl').tmpl(data.mensagens).appendTo('#chatList');
        }
        setTimeout(function () {
            getMensagens();
        }, 500)
    });
}