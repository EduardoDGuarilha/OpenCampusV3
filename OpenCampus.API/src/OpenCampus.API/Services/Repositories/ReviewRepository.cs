using OpenCampus.API.Data;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Services.Repositories;

public class ReviewRepository : EfRepository<Review>
{
    public ReviewRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}
