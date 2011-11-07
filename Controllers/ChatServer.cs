using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class ChatServer
{
    public const int MaxMensagemCount = 100;
    public const int MaxTimetoutSegundos = 60;

    private static object _msgLock = new object();
    private static Subject<Mensagem> _mensagens = new Subject<Mensagem>();

    private static object _historicoLock = new object();
    private static Queue<Mensagem> _historico = new Queue<Mensagem>(MaxMensagemCount + 5);

    static ChatServer()
    {
        _mensagens
            .Subscribe(msg =>
                            {
                                lock (_historicoLock)
                                {
                                    while (_historico.Count > MaxMensagemCount)
                                        _historico.Dequeue();

                                    _historico.Enqueue(msg);
                                }
                            });
    }

    public static void CheckForMensagensAsync(Action<List<Mensagem>> onMensagens)
    {
        var queued = ThreadPool.QueueUserWorkItem(
            new WaitCallback(parm =>
                        {
                            var msgs = new List<Mensagem>();
                            var wait = new AutoResetEvent(false);
                            using (var subscriber = _mensagens.Subscribe(msg =>
                                                                            {
                                                                                msgs.Add(msg);
                                                                                wait.Set();
                                                                            }))
                            {
                                // espera maxima para uma nova mensagem
                                wait.WaitOne(TimeSpan.FromSeconds(MaxTimetoutSegundos));
                            }

                            ((Action<List<Mensagem>>)parm)(msgs);
                        }), onMensagens
        );

        if (!queued)
            onMensagens(new List<Mensagem>());
    }

    private static long currMsgId = 0;
    private static long currUserId = 0;

public static void AddMensagem(string nome, string mensagem)
{
    _mensagens
        .OnNext(new Mensagem
                    {
                        Id = currMsgId++,
                        Conteudo = mensagem,
                        Data = DateTime.Now,
                        Usuario = new Usuario
                                        {
                                            Id = currUserId++,
                                            Nome = nome
                                        }
                    });
}

    public static List<Mensagem> GetHistorico()
    {
        var msgs = new List<Mensagem>();
        lock (_historicoLock)
            msgs = _historico.ToList();

        return msgs;
    }
}