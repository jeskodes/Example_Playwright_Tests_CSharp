using Bogus;
using TestFramework.Shared.Models;

namespace TestFramework.Core.Tests.Data.Reviews;

internal class ReviewRepository : RepositoryBase
{
    // Deletes all reviews for a specific review source (used for cleanup)
    public void DeleteReviewsBySourceId(int reviewSourceId)
    {
        var reviews = DbContext.Reviews.Where(r => r.ReviewSourceId == reviewSourceId);
        DbContext.Reviews.RemoveRange(reviews);
        DbContext.SaveChanges();
    }

    // Creates 20 fake reviews using Bogus library for testing
    public List<Review> CreateReviews(int reviewSourceId)
    {
        // Bogus generates realistic fake data for testing
        var reviews = new Faker<Review>()
            .RuleFor(r => r.PhotoUrl, f => f.Internet.Avatar())          // Random avatar image URL
            .RuleFor(r => r.DisplayName, f => f.Person.FullName)         // Random person name
            .RuleFor(r => r.Stars, f => f.Random.Byte(0, 5).OrNull(f, .8f))  // Star rating 0-5, 20% chance of null
            .RuleFor(r => r.Comment, f => f.Lorem.Text())                // Random lorem ipsum text
            .RuleFor(r => r.ReviewSourceId, reviewSourceId)              // Links review to the source
            .RuleFor(r => r.DateCreatedUtc, f => f.Date.Past())          // Random date in the past
            .Generate(20);  // Creates 20 fake reviews

        // Save all the fake reviews to the database
        DbContext.Reviews.AddRange(reviews);
        DbContext.SaveChanges();
        return reviews;
    }
}
