using Application.Services.Interfaces;
using Domain;
using Domain.Entities.Account;
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

            if (!_context.Company.Any())
                _context.Company.AddRange(Companies);


            if (!_context.ProgramYears.Any())
                _context.ProgramYears.AddRange(ProgramYears);

            if (!_context.People.Any())
                _context.People.AddRange(People);

            if (!_context.BigGoals.Any())
                _context.BigGoals.AddRange(BigGoals);

            if (!_context.OperationalObjectives.Any())
                _context.OperationalObjectives.AddRange(OperationalObjectives);

            if (!_context.Projects.Any())
                _context.Projects.AddRange(Projects);

            if (!_context.PracticalActions.Any())
                _context.PracticalActions.AddRange(PracticalActions);

            if (!_context.HardwareEquipment.Any())
                _context.HardwareEquipment.AddRange(HardwareEquipments);

            if (!_context.Systems.Any())
                _context.Systems.AddRange(Systems);

            if (!_context.Roles.Any())
                _context.AddRange(Roles);

            if (!_context.Permissions.Any())
                _context.Permissions.Add(Permissions);

            if (!_context.Users.Any())
                _context.Users.AddRange(UserRoles);

            _context.SaveChanges();

            if (!_context.RolePermissions.Any())
            {
                var permissions = _context.Permissions.Where(b => b.Discriminator == nameof(PermissionItem)).ToList();
                var adminPermissions = permissions.Select(b => new RolePermission { Id = Guid.NewGuid(), Permission = b, RoleId = SD.AdminRoleId });
                _context.RolePermissions.AddRange(adminPermissions);
                await _context.SaveChangesAsync();
            }
        }

        #region Business
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
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                    ProgramYearId = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30E"),
                    Progress = 94,
                    CompanyId = Guid.Parse("72C9B920-B076-488E-8171-8B2DD4F92A1D")
                },
                new BigGoal{
                    Id =  Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DB"),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "هوشمند سازی کلاس ها" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                    ProgramYearId = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30F"),
                    Progress = 73,
                    CompanyId = Guid.Parse("72C9B920-B076-488E-8171-8B2DD4F92A1D")
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
                new OperationalObjective{
                    Id = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DC9F3"),
                    Deadline = DateTime.Now.AddYears(1),
                    BigGoalId = Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DA"),
                    Budget = 157000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "حذف سامانه های قدیمی"
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
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Contractor = "محمد قاری" ,
                    GuaranteedFulfillmentAt= DateTime.Now.AddYears(1),
                    LeaderId = Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    StartedAt = DateTime.Now,
                    Title = "ایجاد پلتفرم ازمون",
                    OperationalObjectiveId = Guid.Parse("BFA181D0-DF10-4493-8620-8FC61A3DC9F3"),
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

        private List<ProgramYear> ProgramYears =>
            new List<ProgramYear>
            {
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30E"),Year = "1401-1402"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30F"),Year = "1400-1401"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30A"),Year = "1399-1400"},
            };
        #endregion

        #region Account
        private List<Company> Companies =>
            new List<Company>()
            {
                new Company{
                    Id = Guid.Parse("72C9B920-B076-488E-8171-8B2DD4F92A1D"),
                    Title = "رازی"
                },
                new Company{
                    Id = Guid.Parse("72C9B920-B076-488E-8171-8B2DD4F92A1C"),
                    Title = "بوعلی"
                },
                new Company{
                    Id = Guid.Parse("72C9B920-B076-488E-8171-8B2DD4F92A1B"),
                    Title = "شریف"
                },
            };
        private List<Role> Roles =>
            new List<Role>
            {
                new Role
                {
                    Id = SD.AdminRoleId,
                    Title = "ادمین",
                    Description = "ادمین کل سیستم"
                },
                new Role
                {
                    Id = AgentRoleId,
                    Title = "نماینده سازمان",
                    Description = "نماینده مشخص شده از یک سازمان"
                },
                new Role
                {
                    Id = UserRoleId,
                    Title = "کاربر",
                    Description = "حداقل دسترسی"
                }
            };

        private Permission Permissions =>
            new PermissionContainer(
                SD.TopPermissionId,
                "سطوح دسترسی",
                new List<Permission>
                {
                    new PermissionContainer("مسئولیت" , new List<Permission>
                    {
                        new PermissionContainer("اهداف کلان" ,
                            new List<Permission>
                            {
                                new PermissionItem("افزودن هدف کلان"  , SD.Permission_AddBigGoal),
                                new PermissionItem("ویرایش هدف کلان" , SD.Permission_EditBigGoal),
                                new PermissionItem("حذف هدف کلان" , SD.Permission_RemoveBigGoal)
                            }),
                        new PermissionContainer("اهداف عملیاتی" ,
                            new List<Permission>
                            {
                                new PermissionItem("افزودن هدف عملیاتی" , SD.Permission_AddOperationalObjective),
                                new PermissionItem("ویرایش هدف عملیاتی" , SD.Permission_EditOperationalObjective),
                                new PermissionItem("حذف هدف عملیاتی" , SD.Permission_RemoveOperationalObjective),
                            }),
                        new PermissionContainer("منابع" ,
                            new List<Permission>{
                                new PermissionContainer("افراد" ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن فرد" , SD.Permission_AddPerson),
                                        new PermissionItem("حذف فرد" , SD.Permission_RemovePerson),
                                    }),
                                new PermissionContainer("تجهیزات سخت افزاری" ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن تجهیز سخت افزاری" , SD.Permission_AddHardwareEquipment),
                                        new PermissionItem("حذف تجهیز سخت افزاری" , SD.Permission_RemoveHardwareEquipment),
                                    }),
                                new PermissionContainer("سامانه ها" ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن سامانه" , SD.Permission_AddSystem),
                                        new PermissionItem("حذف سامانه" , SD.Permission_RemoveSystem),
                                    }),
                            }),
                        new PermissionContainer("گذار" ,
                            new List<Permission>
                            {
                                new PermissionContainer("پروژه ها" ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن پروژه" , SD.Permission_AddProject),
                                        new PermissionItem("حذف پروژه" , SD.Permission_RemoveProject)
                                    }),
                                new PermissionContainer("اقدامات کاربری" ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن اقدام کاربردی" , SD.Permission_AddPracticalAction),
                                        new PermissionItem("حذف اقدام کاربردی" , SD.Permission_RemovePracticalAction)
                                    }),
                            }),
                    }),
                    new PermissionContainer("حساب" , new List<Permission>
                    {
                        new PermissionContainer("نقش ها" , new List<Permission>
                        {
                            new PermissionItem("افزودن نقش" , SD.Permission_AddRole),
                            new PermissionItem("ویرایش نقش" , SD.Permission_EditRole),
                            new PermissionItem("حذف نقش" , SD.Permission_RemoveRole),
                        })
                    })
                }
            );

        private User UserRoles =>
            new User
            {
                Id = Guid.NewGuid(),
                NationalId = SD.DefaultNationalId,
                RoleId = SD.AdminRoleId,
                IsActive = true,
                CompanyId = Guid.Parse("72C9B920-B076-488E-8171-8B2DD4F92A1D")
            };

        #endregion


        private Guid UserRoleId = Guid.NewGuid();
        private Guid AgentRoleId = Guid.NewGuid();
        private String lorem = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است. چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد. کتابهای زیادی در شصت و سه درصد گذشته، حال و آینده شناخت فراوان جامعه و متخصصان را می طلبد تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی و فرهنگ پیشرو در زبان فارسی ایجاد کرد. در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها و شرایط سخت تایپ به پایان رسد وزمان مورد نیاز شامل حروفچینی دستاوردهای اصلی و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.";
    }
}
