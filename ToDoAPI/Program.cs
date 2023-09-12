using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using ToDoAPI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDB>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/todoitems", async (ToDoDB db) =>                      /// Obter todos os itens de tarefas pendentes
    await db.ToDos.ToListAsync());                            
 
app.MapGet("/todoitems/complete", async (ToDoDB db) =>          /// Obter um item por ID
    await db.ToDos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, ToDoDB db) =>    /// Obter um item por ID
    await db.ToDos.FindAsync(id)
        is ToDo todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapPost("/todoitems", async (ToDo todo, ToDoDB db) =>
{
    db.ToDos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});
app.MapPut("/todoitems/{id}", async (int id, ToDo inputTodo, ToDoDB db) =>
{
    var todo = await db.ToDos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, ToDoDB db) =>
{
    if (await db.ToDos.FindAsync(id) is ToDo todo)
    {
        db.ToDos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});
app.Run();