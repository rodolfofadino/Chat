using System;

public class Mensagem
{
    public long Id { get; set; }
    public DateTime Data { get; set; }
    public string Conteudo { get; set; }
    public Usuario Usuario { get; set; }
}