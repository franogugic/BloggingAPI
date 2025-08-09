using Microsoft.AspNetCore.Mvc;
using PersonalBloggingPlatformAPI.Models;
using PersonalBloggingPlatformAPI.Services;

namespace PersonalBloggingPlatformAPI.Controllers;

// ZADACA CONTOLLERA JE RUKOVANJE HTTP ZAHTJEVIMA
//  - prima HTTP zahtjeve
//  - poziva servicse da obave posao
//  - vraca odgovre


// oznacava da je ova klasa api kontoler sto pruza
//   - automatskopa validacija modela, akko model nije validiran vraca 400 odma
//   - bokje ponasanje za HTTP odgovre i zahtjeve
[ApiController]

// definira rutu tj. URL putanje
// pocetak rute je /api/ a controller se automatski zamjenjuje imenom
// klase bez tijeci Controller
[Route("api/[controller]")]

//Nasa klasa nesljedjuje klasu ContollerBase osnovne funkcionalnosti su
//meotde kao Ok(), NotFound() itd...
public class ArticleController : ControllerBase
{
    //ovo je polje koje cemo koristiti za pozivanje metoda servisa
    // tj. preko njega cemo pozivati metode servisa
    private readonly ArticleService _articlesServices;
    
    public ArticleController(ArticleService articlesServices)
    {
        // postavimo articlesServies u _art...    
        _articlesServices = articlesServices;
    }

    // Oznacava da se metoda poziva kada korisnik posalje get zahtjev
    [HttpGet]
    
    //metoda koja poziva GetAsync metodu iz servisa
    //Task jer je async, ovo ActionResult List Article jer vraca HTTP odgovor
    // tipa 200 OK, 401, 403, 404, 500...
    public async Task<ActionResult<List<Article>>> Get()
    {
        //u article se sprema retultat metode iz servisa
        var articles = await _articlesServices.GetAsync();
        Console.WriteLine($"Fetched {articles.Count} articles from database.");

        foreach (var article in articles)
        {
            Console.WriteLine(article.Title);
        }

        
        //vraca HTTP odoovor sa json bodyem koji sadzi sve articles
        return Ok(articles);
    }

    [HttpPost]
    //Post metoda poziva se automatski
    public async Task<ActionResult<Article>> Post(Article article)
    {
        var created = await _articlesServices.InsertAsync(article);
        //ovo koristimo da vratimo statusni kod, url di se moze dohvatit i sam bodz u jsonu
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    //Prima string i article
    public async Task<ActionResult<Article>> Put(string id, Article article)
    {
        //provjera da nam id urla i bodza budu isti
        if (id != article.Id)
        {
            return BadRequest("ID u URL-u i u tijelu zahtjeva se ne podudaraju.");
        }

        var updated = await _articlesServices.UpdateAsync(article);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
    
    // samo vracamo statusni kod ne moramo vracat nista drugo
    [HttpDelete("{id}")]
    //Ovde koristimo Interfacr actionresult jer je to interface a samo actionresult je tio
    //a ovde ne vracamo cijel article
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _articlesServices.DeleteAsync(id);

        if (!deleted)   
            return NotFound();

        return NoContent(); // 204 - uspješno obrisano, bez tijela odgovora
    }


}