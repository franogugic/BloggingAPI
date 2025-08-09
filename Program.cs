using PersonalBloggingPlatformAPI.Models;
using PersonalBloggingPlatformAPI.Services;
using Microsoft.OpenApi.Models;

// konfiguracija servera obavezno u svakoj app
// kreira se builder objekt koji omogucava registraciju servisa
// ucitavanje konfiguracije namjestanje middlewareeova i ostalo
var builder = WebApplication.CreateBuilder(args);

// ucitavmo postave mongodb iz appsettings.json
//ova linija koda uzima sekciju mongodbsettings iz appsettignsa
//i poveyuje je s klasom mongodbsettings
// to nma je bitno jer se articleservices spaja na bazu koristeci t epsotavke
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

//registracija article serbicesa
//pravi istancu articleservicesa a svi koji ga koriste ce dobit kopiju
builder.Services.AddSingleton<ArticleService>();

//dodavanje kontrollera, swaggera, priprema http za swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// za odobravanje CORS/a
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


//buildanje apl
var app = builder.Build();

//ako pokrecemo apl lokalno onda ukljucimo svagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseCors("AllowAll");


//ovo govi apl da pazu na url rute , mogucava autorizaciju i pravi rutu za svak kontoler
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// pokrece server
app.Run();