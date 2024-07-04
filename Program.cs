using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<OrganizadorContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("CriarTarefa", (OrganizadorContext context, Tarefa tableTarefa) =>
{
    context.Tarefas.Add(tableTarefa);
    context.SaveChanges();

    return tableTarefa;
});

app.MapGet("ObterTarefa/{id}", (int id, OrganizadorContext context) => 
{
    var tarefaId = context.Tarefas.Find(id);

    if (tarefaId == null)
        Console.WriteLine("Tarefa não encontrada!");

    return tarefaId;
});

app.MapPut("AtualizarTarefa/{id}", (int id, OrganizadorContext context, Tarefa tableTarefa) =>
{
    var tarefaId = context.Tarefas.Find(id);

    if (tarefaId == null)
        Console.WriteLine("Tarefa não encontrada!");

    tarefaId.Titulo = tableTarefa.Titulo;
    tarefaId.Descricao = tableTarefa.Descricao;
    tarefaId.Data = tableTarefa.Data;
    tarefaId.Status = tableTarefa.Status;

    context.Tarefas.Update(tarefaId);
    context.SaveChanges();

    return tarefaId;
});

app.MapDelete("DeletarTarefa/{id}", (int id, OrganizadorContext context) =>
{
    var tarefaId = context.Tarefas.Find(id);

    if (tarefaId == null)
        Console.WriteLine("Tarefa não encontrada!");

    context.Remove(tarefaId);
    context.SaveChanges();

    return $"Tarefa cujo id é {id} foi deletada com êxito!";
});

app.MapGet("ObterTodas", (OrganizadorContext context) =>
{
    var tarefas = context.Tarefas;
    return tarefas;
});

app.MapGet("ObterPeloTitulo/{titulo}", (string titulo, OrganizadorContext context) =>
{
    var tarefaTitle = context.Tarefas.Where(tarefas => tarefas.Titulo.Contains(titulo));

    return tarefaTitle;
});

app.MapGet("ObterPorData/{data}", (DateTime data, OrganizadorContext context) =>
{
    var tarefaDate = context.Tarefas.Where(x => x.Data.Date == data.Date);

    return tarefaDate;
});

app.MapGet("ObterPorStatus/{status}", (EnumStatusTarefa status, OrganizadorContext context) =>
{
    var tarefaStatus = context.Tarefas.Where(tarefa => tarefa.Status == status);

    return tarefaStatus;
});

app.Run();