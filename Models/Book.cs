namespace Task5.Models
{
    public class Review
    {
        public string ReviewerName { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Company { get; set; } = string.Empty;
    }

    public class Book
    {
        public int Index { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublishYear { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
        public int Likes { get; set; }
        public List<Review> Reviews { get; set; } = new();
    }
}