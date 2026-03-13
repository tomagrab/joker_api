using joker_api.Models.Entities;
using joker_api.Data.Context;

namespace joker_api.Data.Seed;


public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        // Guard statement to check if DB has been seeded 🤰
        if (context.Jokes.Any())
        {
            return; // DB has been seeded
        }

        var jokes = new JokeEntity[]
        {
            new() {
                Id = Guid.NewGuid().ToString(),
                Joke = "I have nothing to tell you and I'm only going to say it once!"
            }
        };

        context.Jokes.AddRange(jokes);
        context.SaveChanges();
    }
}