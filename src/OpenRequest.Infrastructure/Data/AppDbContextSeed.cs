using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRequest.Core.Entities;

namespace OpenRequest.Infrastructure.Data;

public class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger, int retry = 0)
    {
        var retryForAvailability = retry;
        try
        {
            context.Database.Migrate();

            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddRangeAsync(GetPreconfiguredCategories());
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            if (retryForAvailability >= 10) throw;

            retryForAvailability++;
            
            logger.LogError(ex.Message);
            await SeedAsync(context, logger, retryForAvailability);
            throw;
        }
    }

    private static IEnumerable<Category> GetPreconfiguredCategories()
    {
        return new List<Category>
        {
            new("Web Development", "Mollit nulla labore consectetur eiusmod excepteur anim pariatur. Ullamco labore enim adipisicing veniam in ea excepteur. Voluptate cupidatat velit dolor incididunt sit id officia eiusmod laborum cillum.", "https://img.freepik.com/free-vector/engineer-working-with-server-equipment-switchboard-technical-support-computer-infrastructure-cables-by-male-professional-electrician-flat-vector-illustration-telecommunication-concept_74855-21081.jpg?t=st=1651716315~exp=1651716915~hmac=079fcf3410c8cd02a6b01e8186ce79324945228cbb75bf96bb1d9ca7311f1299&w=826"),
            new("Design UI/UX", "Deserunt incididunt cupidatat ea incididunt. Duis aute incididunt dolore aliqua do esse. Ut aliquip et esse Lorem aute culpa anim magna. Fugiat nulla exercitation elit fugiat qui. Dolor in in tempor in culpa consectetur pariatur exercitation cupidatat. Pariatur cillum esse dolore ullamco anim sit culpa. Sunt enim proident nostrud proident id ullamco excepteur ea ut reprehenderit tempor proident. Adipisicing nostrud exercitation velit occaecat reprehenderit mollit. Dolore do proident tempor pariatur esse. Laboris dolor velit occaecat amet sit. Dolor nulla quis eiusmod aliquip reprehenderit id cupidatat ut nostrud velit. Ea occaecat nisi minim aliquip. Elit esse eu elit laboris deserunt laboris sunt veniam amet Lorem exercitation sint. Dolor veniam irure ex irure proident et. Aliquip Lorem nisi est deserunt minim", "https://img.freepik.com/free-vector/software-development-programming-coding-learning-information-technology-courses-it-courses-all-levels-computing-hi-tech-course-concept_335657-191.jpg?t=st=1651718233~exp=1651718833~hmac=de69599ee48960bfe2b7b2bbfdaab0b6c068479fe6453937bd5c6196fb8082be&w=1060"),
            new("Android Development", "Ullamco labore anim adipisicing aliqua deserunt. Dolor dolor occaecat do ut. Cillum anim irure magna sit incididunt cupidatat qui. In Lorem in deserunt tempor.Mollit qui dolore aliqua mollit commodo cillum Lorem. Officia eu consequat eiusmod proident. Consectetur dolore quis amet ullamco nulla nostrud non veniam nisi do consectetur velit. Fugiat eiusmod exercitation laborum excepteur voluptate labore dolor dolore. Lorem dolor culpa ad eu tempor quis enim id laboris adipisicing nisi. Nulla qui magna labore ea sunt consectetur anim labore id reprehenderit eu aliqua ex sit", "https://img.freepik.com/free-vector/background-with-virtual-interface-flowchart-circle-composition-round-chat-email-exchange-icons-text-paragraphs-illustration_1284-37028.jpg?t=st=1651718233~exp=1651718833~hmac=9e23c9e72959f0613fabe56f75656f3452702253dbfab8b7edf1a0b475954769&w=1060"),
            new("iOS Development", "Ullamco labore anim adipisicing aliqua deserunt. Dolor dolor occaecat do ut. Cillum anim irure magna sit incididunt cupidatat qui. In Lorem in deserunt tempor. Mollit qui dolore aliqua mollit commodo cillum Lorem. Officia eu consequat eiusmod proident. Consectetur dolore quis amet ullamco nulla nostrud non veniam nisi do consectetur velit. Fugiat eiusmod exercitation laborum excepteur voluptate labore dolor dolore. Lorem dolor culpa ad eu tempor quis enim id laboris adipisicing nisi. Nulla qui magna labore ea sunt consectetur anim labore id reprehenderit eu aliqua ex sit", "https://img.freepik.com/free-vector/colored-isometric-programmers-composition-with-software-development-description-man-sit-work_1284-29045.jpg?t=st=1651718386~exp=1651718986~hmac=2d5acbaacf54f883f11b11797b7b8b9ec3713770dd58ea5725b0fe701260ee9d&w=740"),
            new("Desktop Development", "Elit ullamco exercitation ut culpa velit aliquip voluptate amet aliquip ut. Velit cupidatat eiusmod duis mollit. Fugiat irure mollit et do excepteur ea", "https://img.freepik.com/free-vector/software-development-programming-coding-learning_335657-3118.jpg?t=st=1651718386~exp=1651718986~hmac=62e6932ee96db4a7a299932bb3abc6faf199fa4ecd26c224740ba3df71f96b7f&w=1060")
        };
    }
}