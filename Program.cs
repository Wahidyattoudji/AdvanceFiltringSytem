using AdvanceFiltringSytem.QueryService;
using SampleData;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapGet("/products", async (QueryRequest request) => {

    // var result = await  ;

});


app.Run();
