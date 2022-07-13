namespace ProductScrapper.Tests.TestData;

using ProductScrapper.Contracts;

public static class TestInputData
{
    public static IReadOnlyList<ScrappedProduct> GetMrTakoScrappedProducts()
    {
        var scrappedProducts = new[]
        {
            new ScrappedProduct {Name = "Булочка «Баоцзы» 1шт", Price = 19},
            new ScrappedProduct {Name = "Даньхуатан", Price = 210}, new ScrappedProduct {Name = "Мисоширу", Price = 210},
            new ScrappedProduct {Name = "Окрошка азиатская", Price = 240},
            new ScrappedProduct {Name = "Рамен с курицей \"Терияки\"", Price = 200},
            new ScrappedProduct {Name = "Рамен со свининой", Price = 210},
            new ScrappedProduct {Name = "Рамен сырный", Price = 280},
            new ScrappedProduct {Name = "Сливочный рамен с курицей и шампиньонами", Price = 220},
            new ScrappedProduct {Name = "Том ям кай NEW", Price = 270},
            new ScrappedProduct {Name = "Том ям с креветками и кальмаром NEW", Price = 350}
        };

        return scrappedProducts;
    }

    public static string GetMrTakoWebSiteData()
    {
        var pathToTestHtml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData/Супы.html");
        var html = File.ReadAllText(pathToTestHtml);

        return html;
    }
}