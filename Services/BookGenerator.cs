using Bogus;
using Task5.Models;

namespace Task5.Services
{
    public class BookGenerator
    {
        private readonly string _language;
        private readonly int _seed;
        private readonly double _avgLikes;
        private readonly double _avgReviews;

        public BookGenerator(string language, int seed, double avgLikes, double avgReviews)
        {
            _language = language;
            _seed = seed;
            _avgLikes = avgLikes;
            _avgReviews = avgReviews;
        }

        public List<Book> GenerateBooks(int page, int pageSize)
        {
            var books = new List<Book>();
            
            // Create a consistent random generator for this page
            var rng = new Random(_seed);
            
            // Create a Faker with consistent seed
            var faker = new Faker(_language);
            faker.Random = new Randomizer(_seed);

            // Calculate starting index for this page (zero-based, to match controller)
            int startIndex = (page - 1) * pageSize;

            for (int i = 0; i < pageSize; i++)
            {
                int currentIndex = startIndex + i;
                
                // Create a unique random generator for this book using its absolute index
                var bookRng = new Random(_seed + currentIndex);
                faker.Random = new Randomizer(_seed + currentIndex);

                // Generate random number of likes around the average
                int likes = Math.Max(0, (int)Math.Round(_avgLikes + (bookRng.NextDouble() - 0.5) * 2));
                
                // Generate random number of reviews around the average
                int reviewCount = Math.Max(0, (int)Math.Round(_avgReviews + (bookRng.NextDouble() - 0.5) * 2));

                var book = new Book
                {
                    Index = currentIndex + 1, // Human-readable index (1-based)
                    ISBN = GenerateISBN(bookRng),
                    Title = GenerateTitle(faker),
                    Authors = GenerateAuthors(faker),
                    Publisher = faker.Company.CompanyName(),
                    PublishYear = 2000 + bookRng.Next(24),
                    CoverImageUrl = GenerateCoverUrl(bookRng),
                    Likes = likes,
                    Reviews = new List<Review>()
                };

                // Generate reviews with unique faker for each
                for (int j = 0; j < reviewCount; j++)
                {
                    var reviewRng = new Random(_seed + currentIndex * 1000 + j);
                    faker.Random = new Randomizer(reviewRng.Next());
                    
                    book.Reviews.Add(new Review
                    {
                        ReviewerName = faker.Name.FullName(),
                        ReviewText = faker.Lorem.Paragraph(),
                        Rating = reviewRng.Next(1, 6),
                        Company = faker.Company.CompanyName()
                    });
                }

                books.Add(book);
            }

            return books;
        }

        private string GenerateISBN(Random rng)
        {
            var prefix = "978";
            var group = (rng.Next(5) + 1).ToString();
            var publisher = rng.Next(99999).ToString("D5");
            var title = rng.Next(999).ToString("D3");
            var checkDigit = "0";
            return $"{prefix}-{group}-{publisher}-{title}-{checkDigit}";
        }

        private string GenerateTitle(Faker faker)
        {
            var materials = new[] { "Wooden", "Metal", "Glass", "Plastic", "Cotton", "Leather", "Rubber", "Granite" };
            var types = new[] { "Chair", "Table", "Desk", "Cabinet", "Shelf", "Bench", "Stool", "Couch" };
            var adjectives = new[] { "Elegant", "Modern", "Rustic", "Vintage", "Luxurious", "Minimalist", "Classic", "Contemporary" };

            var material = faker.PickRandom(materials);
            var type = faker.PickRandom(types);
            var adjective = faker.PickRandom(adjectives);

            return $"{adjective} {material} {type}";
        }

        private string GenerateAuthors(Faker faker)
        {
            if (faker.Random.Bool(0.4f))  // 40% chance of having two authors
            {
                return $"{faker.Name.FullName()} & {faker.Name.FullName()}";
            }
            return faker.Name.FullName();
        }

        private string GenerateCoverUrl(Random rng)
        {
            return $"https://picsum.photos/seed/{rng.Next(10000)}/400/600";
        }
    }
}