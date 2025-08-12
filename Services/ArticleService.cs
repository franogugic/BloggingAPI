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
        await _articles.Find(doc => doc.Id == id).FirstOrDefaultAsync();

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
    
    public async Task<Article> LikeAsync(string id)
    {
        //Builders. update je helper iz monga koji radi update, a inc da uvecava
        var liked = Builders<Article?>.Update.Inc(doc => doc.Like, 1);
        //sadrzi dodatke postavke za operacije
        //returndocument.after vrati dokument nakon updatea
        var options = new FindOneAndUpdateOptions<Article>
        {
            ReturnDocument = ReturnDocument.After
        };
        //pronadje document koji zadovoljava uvijet za ovaj filter doc = doc.id
        //primjeni inc na taj dokument i onda options vrati After
        return await _articles.FindOneAndUpdateAsync(doc => doc.Id == id, liked, options);
    }

    public async Task<List<Article>> SortByAsync(List<SortCriteria> sortCriteria)
    {
        //napravimo helper Builders koji nam pomaze definirati pravila sortiranje 
        var sortBuilder = Builders<Article>.Sort;
        //zatim napravimo listu SortDefinitiona koja s sastoji od fielda i directiona
        var sortDefinition = new List<SortDefinition<Article>>();

        //prolazimo svaku kategoriju iz sortcriteria
        foreach (var criterion in sortCriteria)
        {
            //i ako je direction asc onda dodajemo mongu razumljiv izraz koji sznaci da se sortira uzlazno prema fieldu
            if (criterion.Direction.ToLower() == "asc")
            {
                sortDefinition.Add(sortBuilder.Ascending(criterion.Field));
            }
            else
            {
                sortDefinition.Add(sortBuilder.Descending(criterion.Field));
            }
        }
        // zatim preko buildera kombiniramo vise kriterija koje smo spremii u sortdefinition i poslje samo sortiramo 
            var combinedSort = sortBuilder.Combine(sortDefinition);
    
        return await _articles.Find(_ => true)
            .Sort(combinedSort)
            .ToListAsync();
    }
    
    //ovo je ulaz
    //[
    //   { "field": "title", "direction": "asc" },
    //   { "field": "publishedAt", "direction": "desc" }
    // ]
    //  znaci primamo nesto ovako i onda to pretvaramo u sortdefinition u srt {title: 1, publishedat: -1} sto je sortiranje razumljivo monug
}