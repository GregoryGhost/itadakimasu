using FuzzySharp;
using IronOcr;

namespace Itadakimasu;

public class ProductDetector
{
    static ProductDetector()
    {
        Installation.LicenseKey =
            "IRONOCR.NAKIWA7371.4183-3E711816CA-KY6EFFEZN3X43-WD2PBLS77TUS-CM3IMWVA32NT-2GBNBTGP3HAM-ZHDJW3GQUAM2-NAVZMQ-TKALOTQYACWGUA-DEPLOYMENT.TRIAL-VPJEJ4.TRIAL.EXPIRES.19.JUL.2022";
    }

    public static FoundProductByImage? FindProductByImage(string imagePath, IReadOnlyCollection<Product> possibleResults)
    {
        var description = DetectProductDescription(imagePath);
        if (string.IsNullOrEmpty(description))
        {
            return null;
        }

        var products = possibleResults.Select(x => x.Name).ToArray();
        var foundStr = FuzzySearch(description, products);
        var checkedStr = description.Contains(foundStr);
        if (!checkedStr)
        {
            return null;
        }

        var foundProduct = possibleResults.First(x => x.Name == foundStr);
        var result = new FoundProductByImage
        {
            ProductName = foundProduct.Name,
            Price = foundProduct.Price
        };

        return result;
    }

    private static string DetectProductDescription(string imagePath)
    {
        var ocr = new IronTesseract
        {
            Language = OcrLanguage.Russian,
            Configuration =
            {
                TesseractVersion = TesseractVersion.Tesseract5
            }
        };
        using var input = new OcrInput();
        input.AddImage(imagePath);
        var result = ocr.Read(input);

        return result.Text;
    }

    private static string FuzzySearch(string inputStr, string[] possibleResults)
    {
        return Process.ExtractOne(inputStr, possibleResults).Value;
    }
}