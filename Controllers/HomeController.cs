using Microsoft.AspNetCore.Mvc;
using Task5.Models;
using Task5.Services;
using CsvHelper;
using System.Globalization;

namespace Task5.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/books")]
        public IActionResult GetBooks(
            string language = "en",
            int seed = 0,
            double avgLikes = 0,
            double avgReviews = 0,
            int page = 1,
            int booksPerPage = 20,
            int totalBooks = 100)
        {
            // Validate inputs
            if (booksPerPage <= 0) booksPerPage = 20;
            if (totalBooks <= 0) totalBooks = 100;

            // Calculate the starting index for this page
            int startIndex = (page - 1) * booksPerPage;
            if (startIndex >= totalBooks)
            {
                return Json(new List<Book>());
            }

            // Calculate how many books to generate for this page
            int remainingBooks = totalBooks - startIndex;
            int booksToGenerate = Math.Min(booksPerPage, remainingBooks);

            var generator = new BookGenerator(language, seed, avgLikes, avgReviews);
            var books = generator.GenerateBooks(page, booksToGenerate);

            return Json(books);
        }

        [HttpGet]
        [Route("api/books/export")]
        public IActionResult ExportToCsv(
            string language = "en",
            int seed = 0,
            double avgLikes = 0,
            double avgReviews = 0,
            int totalBooks = 100)
        {
            if (totalBooks <= 0) totalBooks = 100;

            var generator = new BookGenerator(language, seed, avgLikes, avgReviews);
            var books = new List<Book>();
            int booksPerPage = 20;
            int totalPages = (int)Math.Ceiling((double)totalBooks / booksPerPage);

            for (int p = 1; p <= totalPages; p++)
            {
                int remainingBooks = totalBooks - ((p - 1) * booksPerPage);
                int booksToGenerate = Math.Min(booksPerPage, remainingBooks);
                var pageBooks = generator.GenerateBooks(p, booksToGenerate);
                books.AddRange(pageBooks);
            }

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(books);
            writer.Flush();
            return File(memoryStream.ToArray(), "text/csv", "books.csv");
        }
    }
}


