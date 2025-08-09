using MongoDB.Bson;
using PersonalBloggingPlatformAPI.Models;

namespace PersonalBloggingPlatformAPI.Services;
using MongoDB.Driver;

// SERVICES SE KROISTI ZA RAD S BAZOM
//  - sadrzi logiku pristupa bazi
//  - Komunicira sa MongoDB
//  - ne zanima ga HTTP nego samo rad s podacima

public class ArticleService
{
    //_articles tipa IMongoCollection u koju cemo spremit article iz nase kolekcije
    private readonly IMongoCollection<Article> _articles;
    
    //Kada se napravi instanca klase articleService onda se pravi novi klijent
    // dohvaca se baza pa kolekcija koja se sprema u _articles
    public ArticleService(IConfiguration config)
    {
        var client = new MongoClient(config["MONGODB_URI"]);
        var database = client.GetDatabase("PersonalBloggingAPIDatabase");
        _articles = database.GetCollection<Article>("Articles");
    }
    
    // asikrona metoda koja vraca rezultat tipa lista artikala
    public async Task<List<Article>> GetAsync() =>
        //iz _articles tj kolekcije iz monga find _ => oznacava
        // SELECT * FROM articles, i prebacuje to u listu
        await _articles.Find(_ => true).ToListAsync();
    
    //metoda ista kao predhodna samo prima jos i id, a filtrira
    //prema prosljedjenom idu
    public async Task<Article?> GetAsync(string id) =>
        await _articles.Find(x => x.Id == id).FirstOrDefaultAsync();

    //tipa Article koja prima jedan articel i sprema ga u bazu i vraca
    public async Task<Article> InsertAsync(Article article)
    {
        await _articles.InsertOneAsync(article);
        return article;
    }

    public async Task<Article> UpdateAsync(Article article)
    {
        // ako je id od x jedanak id articlea,i saljemo citav article
        //ovaj doc znaci da program kaze mongu da mu nadje document sa idem isti kao article id
        var result = await _articles.ReplaceOneAsync(doc => doc.Id == article.Id, article);
        if (result.MatchedCount == 0)
        {
            return null; // nije pronađen dokument
        }

        return article;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _articles.DeleteOneAsync(doc => doc.Id == id);
        return result.DeletedCount == 1;
    }   
}