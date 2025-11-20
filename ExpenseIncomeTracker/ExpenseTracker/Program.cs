using Dapper;
using DataAccess.Data;
using IncomeExpenseTracker;

var builder = WebApplication.CreateBuilder(args);
SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());

builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});
builder.Services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

var dataBaseInitializer = app.Services.GetService<DataBaseInitializer>();
await dataBaseInitializer.InitializeAsync();


app.Run();