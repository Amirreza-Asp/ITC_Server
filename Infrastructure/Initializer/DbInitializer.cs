using Application.Services;
using Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;

        public DbInitializer(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute()
        {
            await _context.Database.EnsureDeletedAsync();

            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }


            if (!_context.People.Any())
            {
                _context.People.AddRange(People);
            }

            if (!_context.BigGoals.Any())
            {
                _context.BigGoals.AddRange(BigGoals);
            }

            if (!_context.OperationalObjectives.Any())
            {
                _context.OperationalObjectives.AddRange(OperationalObjectives);
            }

            if (!_context.Projects.Any())
            {
                _context.Projects.AddRange(Projects);
            }

            if (!_context.PracticalActions.Any())
            {
                _context.PracticalActions.AddRange(PracticalActions);
            }

            if (!_context.HardwareEquipment.Any())
                _context.HardwareEquipment.AddRange(HardwareEquipments);

            if (!_context.Systems.Any())
                _context.Systems.AddRange(Systems);

            await _context.SaveChangesAsync();
        }

        private List<Person> People =>
            new List<Person>
            {
                new Person{
                    Id = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8") ,
                    Name = "امیررضا" ,
                    Family = "محمدی" ,
                    Education = "لیسانس" ,
                    JobTitle = "برنامه نویس" ,
                    Expertises = new List<Expertise>
                        {
                            new Expertise{Title = "فرانت"},
                            new Expertise{Title = "سرور"},
                            new Expertise{Title = "مهندسی نرم افزار"},
                        }
                },
                new Person{
                    Id = Guid.NewGuid() ,
                    Name = "پریسا" ,
                    Family = "مرادی" ,
                    Education = "لیسانس" ,
                    JobTitle = "برنامه نویس" ,
                    Expertises = new List<Expertise>
                        {
                            new Expertise{Title = "فرانت"},
                        }
                },
                new Person{
                    Id = Guid.NewGuid() ,
                    Name = "پیمان" ,
                    Family = "حیدری" ,
                    Education = "لیسانس" ,
                    JobTitle = "برنامه نویس" ,
                    Expertises = new List<Expertise>
                        {
                            new Expertise{Title = "طراحی رابط کاربری"},
                        }
                }
            };

        private List<BigGoal> BigGoals =>
            new List<BigGoal>
            {
                new BigGoal{
                    Id = Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DA"),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "مجازی سازی دانشگاه" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه"
                },
                new BigGoal{
                    Id =  Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DB"),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "هوشمند سازی کلاس ها" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه"
                },
            };

        private List<OperationalObjective> OperationalObjectives =>
            new List<OperationalObjective>
            {
                new OperationalObjective{
                    Id = Guid.Parse("BFA181D0-DF10-4493-8620-5FC61A3DB9F3"),
                    Deadline = DateTime.Now.AddYears(1),
                    BigGoalId = Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DA"),
                    Budget = 1000000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "افزایش تعداد سرور ها"
                },
                new OperationalObjective{
                    Id = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                    Deadline = DateTime.Now.AddYears(1),
                    BigGoalId = Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DA"),
                    Budget = 157000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "ایجاد سامانه اموزشی"
                },
            };

        private List<Project> Projects =>
            new List<Project>
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    Contractor = "مهدی نیازی" ,
                    GuaranteedFulfillmentAt= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "بهینه سازی سرور های موجود",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Contractor = "محمد قاری" ,
                    GuaranteedFulfillmentAt= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "ایجاد پلتفرم ازمون",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Contractor = "مهدی نیازی" ,
                    GuaranteedFulfillmentAt= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "بهینه سازی سرور های موجود",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-5FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Contractor = "محمد قاری" ,
                    GuaranteedFulfillmentAt= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "ایجاد پلتفرم ازمون",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-5FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                }
            };

        private List<PracticalAction> PracticalActions =>
            new List<PracticalAction>
            {
                new PracticalAction
                {
                    Id = Guid.NewGuid(),
                    Contractor = "نادر نادری" ,
                    Deadline = DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "خرید پروژکتور",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                },
                new PracticalAction
                {
                    Id = Guid.NewGuid(),
                    Contractor = "رها ازادی" ,
                    Deadline = DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "خرید کابل های ازمایشگاه ها",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                },
                new PracticalAction
                {
                    Id = Guid.NewGuid(),
                    Contractor = "نادر نادری" ,
                    Deadline = DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "خرید پروژکتور",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-5FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                },
                new PracticalAction
                {
                    Id = Guid.NewGuid(),
                    Contractor = "رها ازادی" ,
                    Deadline = DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "خرید کابل های ازمایشگاه ها",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-5FC61A3DB9F3"),
                    Financials = new List<Domain.SubEntities.Financial>
                    {
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 1",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 2",
                        },
                        new Domain.SubEntities.Financial
                        {
                            Title = "دارایی 3",
                        },
                    }
                },
            };

        private List<HardwareEquipment> HardwareEquipments =>
            new List<HardwareEquipment>
            {
                new HardwareEquipment
                {
                    Id = Guid.NewGuid(),
                    Title = "سرور",
                    BrandName = "hp",
                    Count = 3,
                    SupportType = "وارانتی پیشنهادی",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.NewGuid(),
                    Title = "مانیتور",
                    BrandName = "apple",
                    Count = 3,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.NewGuid(),
                    Title = "سرور",
                    BrandName = "microsoft",
                    Count = 10,
                    SupportType = "وارانتی پیشنهادی",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.NewGuid(),
                    Title = "کیبورد",
                    BrandName = "dell",
                    Count = 20,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.NewGuid(),
                    Title = "ماوس",
                    BrandName = "dell",
                    Count = 20,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
            };

        private List<Domain.Entities.Business.System> Systems =>
            new List<Domain.Entities.Business.System>
            {
                new Domain.Entities.Business.System
                {
                    Id = Guid.NewGuid(),
                    Title = "سیستم 1",
                    Database = "SQL Server",
                    Development = lorem,
                    Company = "ویتکو",
                    Framework = lorem,
                    OS = "ویندوز",
                    ProgrammingLanguage = "C#",
                    Description= lorem,
                    SupportType = lorem
                },
                new Domain.Entities.Business.System
                {
                    Id = Guid.NewGuid(),
                    Title = "سیستم 2",
                    Database = "Mongo",
                    Development = lorem,
                    Company = "ویتکو",
                    Framework = lorem,
                    OS = "لینوکس",
                    ProgrammingLanguage = "JS",
                    Description= lorem,
                    SupportType = lorem
                },
                new Domain.Entities.Business.System
                {
                    Id = Guid.NewGuid(),
                    Title = "سیستم 3",
                    Database = "Elastic Search",
                    Development = lorem,
                    Company = "ویتکو",
                    Framework = lorem,
                    OS = "ویندوز",
                    ProgrammingLanguage = "Python",
                    Description= lorem,
                    SupportType = lorem
                },
            };






        private String lorem = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است. چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد. کتابهای زیادی در شصت و سه درصد گذشته، حال و آینده شناخت فراوان جامعه و متخصصان را می طلبد تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی و فرهنگ پیشرو در زبان فارسی ایجاد کرد. در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها و شرایط سخت تایپ به پایان رسد وزمان مورد نیاز شامل حروفچینی دستاوردهای اصلی و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.";
    }
}
