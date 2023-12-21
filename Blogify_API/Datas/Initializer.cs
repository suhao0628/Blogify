using Blogify_API.Datas;
using Blogify_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Delivery_API.Data
{
    public class Initializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                context.Database.Migrate();

                //var communities = new List<Community> {
                //   new Community
                //   {
                //      Id = Guid.NewGuid(),
                //      CreatedTime = DateTime.Now,
                //      Name = "Масонская ложа",
                //      Description = "Место, помещение, где собираются масоны для проведения своих собраний, чаще называемых работами",
                //      IsClosed = true,
                //      SubscribersCount = 0,
                //   },
                //   new Community
                //   {
                //      Id = Guid.NewGuid(),
                //      CreatedTime = DateTime.Now,
                //      Name = "Следствие вели с Л. Каневским",
                //      Description = "Без длинных предисловий: мужчина умер",
                //      IsClosed = false,
                //      SubscribersCount = 0,
                //   },
                //   new Community
                //   {
                //      Id = Guid.NewGuid(),
                //      CreatedTime = DateTime.Now,
                //      Name = "IT <3",
                //      Description = "Информационные технологии связаны с изучением методов и средств сбора, обработки и передачи данных с целью получения информации нового качества о состоянии объекта, процесса или явления",
                //      IsClosed = false,
                //      SubscribersCount = 0,
                //   },
                //};

                //if (!context.Communities.Any())
                //{
                //    context.AddRange(communities);
                //    context.SaveChanges();
                //}


                var tags = new List<Tag> {
                   new Tag
                   {
                       Name="история",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="еда",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="18+",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="it",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="интернет",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="теория_заговора",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="соцсети",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="косплей",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                   new Tag
                   {
                       Name="преступление",
                      Id = Guid.NewGuid(),
                      CreateTime = DateTime.Now,
                   },
                };

                if (!context.Tags.Any())
                {
                    context.AddRange(tags);
                    context.SaveChanges();
                }

            }
        }
    }
}
