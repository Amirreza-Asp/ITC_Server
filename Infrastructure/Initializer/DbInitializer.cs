using Application.Services.Interfaces;
using Domain;
using Domain.Entities.Account;
using Domain.Entities.Business;
using Domain.Entities.Static;
using Domain.SubEntities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly ISSOService _ssoService;

        public DbInitializer(ApplicationDbContext context, IWebHostEnvironment hostEnv, ISSOService ssoService)
        {
            _context = context;
            _hostEnv = hostEnv;
            _ssoService = ssoService;
        }

        public async Task Execute()
        {
            //await _context.Database.EnsureDeletedAsync();


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


            if (!_context.ProgramYears.Any())
                _context.ProgramYears.AddRange(ProgramYears);

            if (!_context.IndicatorCategories.Any())
                _context.IndicatorCategories.AddRange(IndicatorCategories);

            if (!_context.IndicatorTypes.Any())
                _context.IndicatorTypes.AddRange(IndicatorTypes);


            if (!_context.Roles.Any())
                _context.Roles.AddRange(Roles);

            if (!_context.Company.Any())
            {
                _context.Company.Add(AdminComapny);
                foreach (var user in Users)
                {
                    user.Id = Guid.NewGuid();
                    var rnd = new Random();
                    user.NationalId = rnd.NextInt64(1111111111, 9999999999).ToString();
                    var act = new Act { Id = Guid.NewGuid(), CompanyId = AdminComapny.Id, RoleId = Roles[new Random().Next(Roles.Count)].Id, UserId = user.Id };
                    _context.Act.Add(act);
                    _context.Users.Add(user);
                }

            }


            var comps = await _ssoService.GetCompaniesAsync();

            if (!_context.Company.Any())
            {
                foreach (var comp in comps)
                {
                    comp.ParentId = AdminComapny.Id;
                    if (comp.Id == Guid.Parse("aa12b4d2-652c-407a-a569-9edcd1e2c467"))
                    {
                        comp.Childs = new List<Company>
                        {
                            new Company
                            {
                                Id = Guid.Parse("FD88FE22-5AB8-48E9-B649-9A9F5FE93180"),
                                ParentId = comp.Id,
                                Title = "واحد علم و فناوری",
                                Childs = new List<Company>
                                {
                                    new Company
                                    {
                                        Id = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676"),
                                        ParentId =  Guid.Parse("FD88FE22-5AB8-48E9-B649-9A9F5FE93180"),
                                        Title = "واحد آپا",
                                        Childs = new List<Company>
                                        {
                                            new Company{Id = Guid.NewGuid() , Title ="واحد آپا مرکزی", ParentId = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676")},
                                            new Company{Id = Guid.NewGuid() , Title ="واحد آپا دانشکده کامپیوتر", ParentId = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676")},
                                            new Company{Id = Guid.NewGuid() , Title ="واحد آپا دانشکده کشاورزی", ParentId = Guid.Parse("03070627-8EDB-410C-AB95-7F29E5FA2676")},

                                        }
                                    },
                                    new Company
                                    {
                                        Id = Guid.NewGuid(),
                                        ParentId =  Guid.Parse("FD88FE22-5AB8-48E9-B649-9A9F5FE93180"),
                                        Title = "مرکز هوش مصنوعی",
                                    },
                                }
                            },
                            new Company
                            {
                                Id = Guid.NewGuid(),
                                ParentId = comp.Id,
                                Title = "واحد شبکه"
                            },
                            new Company
                            {
                                Id = Guid.NewGuid(),
                                ParentId = comp.Id,
                                Title = "واحد سلامت"
                            },
                        };
                    }
                    _context.Company.Add(comp);


                    for (int i = 0; i < new Random().Next(1, 6); i++)
                    {
                        var indicator = CustomeIndicator;
                        indicator.Id = Guid.NewGuid();
                        var companyIndicator = new CompanyIndicator { CompanyId = comp.Id, IndicatorId = indicator.Id };

                        for (int j = 0; j < new Random().Next(0, 7); j++)
                        {
                            var progressIndicator = IndicatorProgress;
                            progressIndicator.Id = Guid.NewGuid();
                            progressIndicator.IndicatorId = indicator.Id;
                            progressIndicator.Value = new Random().NextInt64(indicator.InitValue, indicator.GoalValue);

                            _context.IndicatorProgresses.Add(progressIndicator);
                        }

                        _context.Indicators.Add(indicator);
                        _context.CompanyIndicators.Add(companyIndicator);
                    }

                    List<Guid> peopleId = new List<Guid>();

                    foreach (var person in People)
                    {
                        person.CompanyId = comp.Id;
                        person.Id = Guid.NewGuid();
                        peopleId.Add(person.Id);
                        _context.People.Add(person);
                    }

                    foreach (var heq in HardwareEquipments)
                    {
                        heq.CompanyId = comp.Id;
                        heq.Id = Guid.NewGuid();
                        _context.HardwareEquipment.Add(heq);
                    }

                    foreach (var system in Systems)
                    {
                        system.CompanyId = comp.Id;
                        system.Id = Guid.NewGuid();
                        _context.Systems.Add(system);
                    }

                    foreach (var bigGoal in BigGoals)
                    {
                        bigGoal.Id = Guid.NewGuid();
                        bigGoal.CompanyId = comp.Id;
                        var random = new Random();
                        bigGoal.ProgramYearId = ProgramYears[random.Next(ProgramYears.Count)].Id;

                        var rnd = new Random();

                        for (int i = 0; i < rnd.Next(1, 6); i++)
                        {
                            var customeInd = CustomeIndicator;
                            customeInd.Id = Guid.NewGuid();
                            var bid = new BigGoalIndicator { BigGoalId = bigGoal.Id, IndicatorId = customeInd.Id };

                            for (int j = 0; j < rnd.Next(0, 7); j++)
                            {
                                var progressIndicator = IndicatorProgress;
                                progressIndicator.Id = Guid.NewGuid();
                                progressIndicator.IndicatorId = customeInd.Id;
                                progressIndicator.Value = rnd.NextInt64(customeInd.InitValue, customeInd.GoalValue);

                                _context.IndicatorProgresses.Add(progressIndicator);
                            }

                            _context.Indicators.Add(customeInd);
                            _context.BigGoalIndicators.Add(bid);
                        }

                        _context.BigGoals.Add(bigGoal);

                        foreach (var opo in OperationalObjectives)
                        {
                            opo.Id = Guid.NewGuid();
                            opo.BigGoalId = bigGoal.Id;
                            _context.OperationalObjectives.Add(opo);


                            for (int i = 0; i < rnd.Next(1, 6); i++)
                            {
                                var ind = CustomeIndicator;
                                ind.Id = Guid.NewGuid();

                                var opoInd = new OperationalObjectiveIndicator
                                { IndicatorId = ind.Id, OperationalObjectiveId = opo.Id };

                                for (int j = 0; j < rnd.Next(0, 7); j++)
                                {
                                    var progressIndicator = IndicatorProgress;
                                    progressIndicator.Id = Guid.NewGuid();
                                    progressIndicator.IndicatorId = ind.Id;
                                    progressIndicator.Value = rnd.NextInt64(ind.InitValue, ind.GoalValue);

                                    _context.IndicatorProgresses.Add(progressIndicator);
                                }

                                _context.Indicators.Add(ind);
                                _context.OperationalObjectiveIndicators.Add(opoInd);
                            }

                            foreach (var project in Projects)
                            {
                                project.Id = Guid.NewGuid();
                                project.OperationalObjectiveId = opo.Id;
                                project.LeaderId = peopleId[rnd.Next(peopleId.Count - 1)];

                                project.Financials = People.Take(rnd.Next(0, People.Count - 1)).Select(b => new Financial { Title = b.Name }).ToList();
                                project.Financials.AddRange(HardwareEquipments.Take(rnd.Next(0, HardwareEquipments.Count - 1)).Select(b => new Financial { Title = b.Title }).ToList());
                                project.Financials.AddRange(Systems.Take(rnd.Next(0, Systems.Count - 1)).Select(b => new Financial { Title = b.Title }).ToList());

                                for (int i = 0; i < rnd.Next(1, 6); i++)
                                {
                                    var ind = CustomeIndicator;
                                    ind.Id = Guid.NewGuid();

                                    var projectInd = new ProjectIndicator { IndicatorId = ind.Id, ProjectId = project.Id };

                                    for (int j = 0; j < rnd.Next(0, 7); j++)
                                    {
                                        var progressIndicator = IndicatorProgress;
                                        progressIndicator.Id = Guid.NewGuid();
                                        progressIndicator.IndicatorId = ind.Id;
                                        progressIndicator.Value = rnd.NextInt64(ind.InitValue, ind.GoalValue);

                                        _context.IndicatorProgresses.Add(progressIndicator);
                                    }

                                    _context.Indicators.Add(ind);
                                    _context.ProjectIndicators.Add(projectInd);
                                }

                                _context.Projects.Add(project);
                            }

                            foreach (var practicalAction in PracticalActions)
                            {
                                practicalAction.Id = Guid.NewGuid();
                                practicalAction.OperationalObjectiveId = opo.Id;
                                practicalAction.LeaderId = peopleId[rnd.Next(peopleId.Count - 1)];

                                practicalAction.Financials = People.Take(rnd.Next(0, People.Count - 1)).Select(b => new Financial { Title = b.Name }).ToList();
                                practicalAction.Financials.AddRange(HardwareEquipments.Take(rnd.Next(0, HardwareEquipments.Count - 1)).Select(b => new Financial { Title = b.Title }).ToList());
                                practicalAction.Financials.AddRange(Systems.Take(rnd.Next(0, Systems.Count - 1)).Select(b => new Financial { Title = b.Title }).ToList());


                                for (int i = 0; i < rnd.Next(1, 6); i++)
                                {
                                    var ind = CustomeIndicator;
                                    ind.Id = Guid.NewGuid();

                                    var prInd = new PracticalActionIndicator { IndicatorId = ind.Id, PracticalActionId = practicalAction.Id };

                                    for (int j = 0; j < rnd.Next(0, 7); j++)
                                    {
                                        var progressIndicator = IndicatorProgress;
                                        progressIndicator.Id = Guid.NewGuid();
                                        progressIndicator.IndicatorId = ind.Id;
                                        progressIndicator.Value = rnd.NextInt64(ind.InitValue, ind.GoalValue);

                                        _context.IndicatorProgresses.Add(progressIndicator);
                                    }

                                    _context.Indicators.Add(ind);
                                    _context.PracticalActionIndicators.Add(prInd);
                                }

                                _context.PracticalActions.Add(practicalAction);
                            }
                        }
                    }

                    for (int h = 0; h < Users.Count; h++)
                    {
                        var user = Users[h];
                        user.Id = Guid.NewGuid();
                        var rnd = new Random();
                        user.NationalId = rnd.NextInt64(1111111111, 9999999999).ToString();

                        var act =
                            new Act
                            {
                                Id = Guid.NewGuid(),
                                CompanyId = comp.Id,
                                UserId = user.Id,
                                RoleId = h == 0 ? SD.AgentId : Roles[new Random().Next(1, Roles.Count - 1)].Id
                            };

                        _context.Act.Add(act);
                        _context.Users.Add(user);
                    }

                    foreach (var ujr in UserJoinRequests)
                    {
                        ujr.CompanyId = comp.Id;
                        ujr.Id = Guid.NewGuid();
                        _context.UsersJoinRequests.Add(ujr);
                    }
                }
            }

            if (!_context.Users.Any(b => b.Id == Guid.Parse("06797131-356F-4E99-A413-8E08104E4CB0")))
            {
                var userTest = new User
                {
                    Id = Guid.Parse("06797131-356F-4E99-A413-8E08104E4CB0"),
                    NationalId = "3360408330",
                    Name = "فرهاد",
                    Family = "رحمانی"
                };

                userTest.Act.Add(new Act { UserId = userTest.Id, CompanyId = Guid.Parse("aa12b4d2-652c-407a-a569-9edcd1e2c467"), Id = Guid.NewGuid(), RoleId = SD.AgentId });
                userTest.Act.Add(new Act { UserId = userTest.Id, CompanyId = AdminComapny.Id, Id = Guid.NewGuid(), RoleId = SD.AgentId });

                _context.Add(userTest);
            }

            if (!_context.Permissions.Any())
                _context.Permissions.Add(Permissions);

            _context.SaveChanges();

            if (!_context.RolePermissions.Any())
            {
                var permissions = _context.Permissions.Where(b => b.Discriminator == nameof(PermissionItem)).ToList();
                var agentPermissions = permissions.Select(b => new RolePermission { Id = Guid.NewGuid(), PermissionId = b.Id, RoleId = SD.AgentId });
                _context.RolePermissions.AddRange(agentPermissions.DistinctBy(b => b.Id));
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
                    Id = Guid.Parse("440FF1EF-B4DD-4C6B-B943-A20EA411E9D8") ,
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
                    Id = Guid.Parse("440FF1EF-A4DD-4C6B-B943-A20EA411E9D8"),
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
                    Id = Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "مجازی سازی دانشگاه" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                    ProgramYearId = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30E"),
                },
                new BigGoal{
                    Id =  Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "هوشمند سازی کلاس ها" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                    ProgramYearId = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30F"),
                },
                new BigGoal{
                    Id =  Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DC"),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "افزایش سنوات دانشجویان" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                    ProgramYearId = Guid.NewGuid(),
                },
            };

        private List<OperationalObjective> OperationalObjectives =>
            new List<OperationalObjective>
            {
                new OperationalObjective{
                    Id = Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 1000000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "افزایش تعداد سرور ها",
                },
                new OperationalObjective{
                    Id = Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 157000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "ایجاد سامانه اموزشی",
                },
                new OperationalObjective{
                    Id = Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 157000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "حذف سامانه های قدیمی",
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
                    StartedAt = DateTime.Now.AddDays(new Random().Next(-365 , 0)),
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
                    StartedAt = DateTime.Now.AddYears(new Random().Next(-500 , 0)),
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
            };

        private List<HardwareEquipment> HardwareEquipments =>
            new List<HardwareEquipment>
            {
                new HardwareEquipment
                {
                    Id =  Guid.Parse("440FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سرور",
                    BrandName = "hp",
                    Count = 3,
                    SupportType = "وارانتی پیشنهادی",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.Parse("540FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "مانیتور",
                    BrandName = "apple",
                    Count = 3,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.Parse("640FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "کیبورد",
                    BrandName = "dell",
                    Count = 20,
                    SupportType = "وارانتی متناسب با یک هدف خاص",
                    Description = lorem
                },
                new HardwareEquipment
                {
                    Id = Guid.Parse("740FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
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
                    Id = Guid.Parse("441FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سیستم 1",
                    Database = "SQL Server",
                    Development = lorem,
                    BuildInCompany = "ویتکو",
                    Framework = lorem,
                    OS = "ویندوز",
                    ProgrammingLanguage = "C#",
                    Description= lorem,
                    SupportType = lorem
                },
                new Domain.Entities.Business.System
                {
                    Id = Guid.Parse("442FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سیستم 2",
                    Database = "Mongo",
                    Development = lorem,
                    BuildInCompany = "ویتکو",
                    Framework = lorem,
                    OS = "لینوکس",
                    ProgrammingLanguage = "JS",
                    Description= lorem,
                    SupportType = lorem
                },
                new Domain.Entities.Business.System
                {
                    Id = Guid.Parse("443FF1EF-C4DD-4C6B-B943-A20EA411E9D8"),
                    Title = "سیستم 3",
                    Database = "Elastic Search",
                    Development = lorem,
                    BuildInCompany = "ویتکو",
                    Framework = lorem,
                    OS = "ویندوز",
                    ProgrammingLanguage = "Python",
                    Description= lorem,
                    SupportType = lorem
                },
            };

        private IndicatorProgress IndicatorProgress =>
            new IndicatorProgress
            {
                Id = Guid.NewGuid(),
                ProgressTime = DateTime.Now.AddDays(new Random().Next(-300, 300)),
            };

        #endregion

        #region Account
        private List<Company> Companies()
        {
            var companies = _ssoService.GetCompaniesAsync().GetAwaiter().GetResult();
            return companies;
        }

        private Company AdminComapny => new Company
        {
            Id = Guid.Parse("7992DB78-0F89-4CDB-B13D-0E2E500DEB06"),
            Title = "ادمین سیستم"
        };

        private List<Role> Roles =>
            new List<Role>
            {
                  new Role
                {
                    Id = SD.AgentId,
                    Title = "نماینده سازمان",
                    Description = "نماینده مشخص شده از یک سازمان"
                },
                new Role
                {
                    Id = Guid.Parse("3D768081-EAF8-4BC2-A14F-601C7AAB9338"),
                    Title = "کارشناش شبکه",
                    Description = "کارشناش شبکه"
                },

                new Role
                {
                    Id = Guid.Parse("0D4618FF-7576-47AC-85B8-ABD285939492"),
                    Title = "اپراتور",
                    Description = "اپراتور"
                },
                new Role
                {
                    Id = Guid.Parse("18CD3FC8-8314-4F6C-AE0F-1EA923FA647E"),
                    Title = "کارشنای مرکز داده",
                    Description = "کارشنای مرکز داده"
                },
            };

        private Permission Permissions =>
            new PermissionContainer(
                SD.TopPermissionId,
                "سطوح دسترسی",
                PermissionType.General,
                new List<Permission>
                {
                    new PermissionContainer(
                        "مسئولیت" ,
                        PermissionType.General,
                        new List<Permission>
                        {
                            //********* system **********
                            new PermissionContainer("دانشگاه ها" , PermissionType.System ,
                                new List<Permission> {
                                    new PermissionItem("اهداف کلان" , PermissionType.System , PermissionsSD.System_ShowBigGoals),
                                    new PermissionItem("اهداف عملیاتی" , PermissionType.System , PermissionsSD.System_ShowOperationalObjectives),
                                    new PermissionItem("پروژه ها" , PermissionType.System , PermissionsSD.System_ShowProjects),
                                    new PermissionItem("اقدامات کاربردی" , PermissionType.System , PermissionsSD.System_ShowPracticalActions),
                                    new PermissionItem("افراد" , PermissionType.System , PermissionsSD.System_ShowManpowers),
                                    new PermissionItem("تجهیزات سخت افزاری" , PermissionType.System , PermissionsSD.System_ShowHardwareEquipments),
                                    new PermissionItem("سامانه ها" , PermissionType.System , PermissionsSD.System_ShowSystems),
                                    new PermissionItem("کاربران" , PermissionType.System , PermissionsSD.System_ShowUsers),
                                }),
                            new PermissionContainer("سال برنامه" , PermissionType.System ,
                                new List<Permission> {
                                    new PermissionItem("افزودن سال برنامه", PermissionType.System , PermissionsSD.System_AddProgramYear),
                                    new PermissionItem("ویرایش سال برنامه", PermissionType.System , PermissionsSD.System_EditProgramYear),
                                    new PermissionItem("حذف سال برنامه", PermissionType.System , PermissionsSD.System_RemoveProgramYear),
                                }),
                            new PermissionContainer("طبقه بندی شاخص" , PermissionType.System ,
                                new List<Permission> {
                                    new PermissionItem("افزودن طبقه بندی شاخص", PermissionType.System , PermissionsSD.System_AddIndicatorCategory),
                                    new PermissionItem("ویرایش طبقه بندی شاخص", PermissionType.System , PermissionsSD.System_EditIndicatorCategory),
                                    new PermissionItem("حذف طبقه بندی شاخص ", PermissionType.System , PermissionsSD.System_RemoveIndicatorCategory),
                                }),
                            new PermissionContainer("واحد های شاخص" , PermissionType.System ,
                                new List<Permission> {
                                    new PermissionItem("افزودن واحد شاخص", PermissionType.System , PermissionsSD.System_AddIndicatorType),
                                    new PermissionItem("ویرایش واحد شاخص", PermissionType.System , PermissionsSD.System_EditIndicatorType),
                                    new PermissionItem("حذف واحد شاخص ", PermissionType.System , PermissionsSD.System_RemoveIndicatorType),
                                }),
                            //********* company *********
                            new PermissionContainer("اهداف کلان" , PermissionType.Company ,
                                new List<Permission>
                            {
                                new PermissionItem("افزودن هدف کلان" , PermissionType.Company , PermissionsSD.Company_AddBigGoal),
                                new PermissionItem("ویرایش هدف کلان", PermissionType.Company , PermissionsSD.Company_EditBigGoal),
                                new PermissionItem("حذف هدف کلان" , PermissionType.Company,  PermissionsSD.Company_RemoveBigGoal),
                                new PermissionItem("مدیریت شاخص های هدف کلان" , PermissionType.Company,  PermissionsSD.Company_ManageBigGoalIndicator)
                            }),
                            new PermissionContainer("اهداف عملیاتی" , PermissionType.Company ,
                                new List<Permission>
                            {
                                new PermissionItem("افزودن هدف عملیاتی"  , PermissionType.Company,  PermissionsSD.Company_AddOperationalObjective),
                                new PermissionItem("ویرایش هدف عملیاتی" , PermissionType.Company ,  PermissionsSD.Company_EditOperationalObjective),
                                new PermissionItem("حذف هدف عملیاتی" , PermissionType.Company ,  PermissionsSD.Company_RemoveOperationalObjective),
                                new PermissionItem("مدیریت شاخص‌ های هدف عملیاتی" , PermissionType.Company ,  PermissionsSD.Company_ManageOperationalObjectiveIndicator),
                            }),
                            new PermissionContainer("منابع" , PermissionType.Company ,
                                new List<Permission>{
                                new PermissionContainer("افراد" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن فرد" , PermissionType.Company ,  PermissionsSD.Company_AddPerson),
                                        new PermissionItem("ویرایش فرد" , PermissionType.Company , PermissionsSD.Company_EditPerson),
                                        new PermissionItem("حذف فرد" , PermissionType.Company ,  PermissionsSD.Company_RemovePerson),
                                    }),
                                new PermissionContainer("تجهیزات سخت افزاری" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن تجهیز سخت افزاری" , PermissionType.Company ,  PermissionsSD.Company_AddHardwareEquipment),
                                        new PermissionItem("ویرایش تجهیز سخت افزاری" , PermissionType.Company ,  PermissionsSD.Company_EditHardwareEquipment),
                                        new PermissionItem("حذف تجهیز سخت افزاری" , PermissionType.Company ,  PermissionsSD.Company_RemoveHardwareEquipment),
                                    }),
                                new PermissionContainer("سامانه ها" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن سامانه" , PermissionType.Company ,  PermissionsSD.Company_AddSystem),
                                        new PermissionItem("ویرایش سامانه" , PermissionType.Company ,  PermissionsSD.Company_EditSystem),
                                        new PermissionItem("حذف سامانه" , PermissionType.Company ,  PermissionsSD.Company_RemoveSystem),
                                    }),
                            }),
                            new PermissionContainer("گذار" , PermissionType.Company ,
                                new List<Permission>
                            {
                                new PermissionContainer("پروژه ها" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن پروژه" , PermissionType.Company ,  PermissionsSD.Company_AddProject),
                                        new PermissionItem("ویرایش پروژه" , PermissionType.Company ,  PermissionsSD.Company_EditProject),
                                        new PermissionItem("حذف پروژه" , PermissionType.Company ,  PermissionsSD.Company_RemoveProject),
                                        new PermissionItem("مدیریت شاخص های پروژه" , PermissionType.Company ,  PermissionsSD.Company_ManageProjectIndicator)
                                    }),
                                new PermissionContainer("اقدامات کاربری" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن اقدام" , PermissionType.Company ,  PermissionsSD.Company_AddPracticalAction),
                                        new PermissionItem("ویرایش اقدام" , PermissionType.Company ,  PermissionsSD.Company_EditPracticalAction),
                                        new PermissionItem("حذف اقدام" , PermissionType.Company ,  PermissionsSD.Company_RemovePracticalAction),
                                        new PermissionItem("مدیریت شاخص های اقدام" , PermissionType.Company ,  PermissionsSD.Company_ManagePracticalActionIndicator)
                                    }),
                            }),
                        }),
                    //********* general **********
                    new PermissionContainer(
                        "حساب" ,
                        PermissionType.General ,
                        new List<Permission>
                            {
                                new PermissionContainer("نقش ها", PermissionType.General , new List<Permission>
                                {
                                    new PermissionItem("افزودن نقش" , PermissionType.General ,PermissionsSD.General_AddRole),
                                    new PermissionItem("ویرایش نقش", PermissionType.General , PermissionsSD.General_EditRole),
                                    new PermissionItem("حذف نقش", PermissionType.General , PermissionsSD.General_RemoveRole),
                                }),
                                new PermissionContainer("کاربران", PermissionType.General , new List<Permission>
                                {
                                    new PermissionItem("مشاهده کاربران" , PermissionType.General ,PermissionsSD.General_UsersList),
                                    new PermissionItem("مدیریت درخواست ورود کاربران", PermissionType.General , PermissionsSD.General_UsersRequests),
                                    new PermissionItem("حذف کاربر", PermissionType.General , PermissionsSD.General_RemoveUser),
                                    new PermissionItem("تعیین دسترسی", PermissionType.General , PermissionsSD.General_ManageUserRole),
                                })
                        })
                }
            );

        private List<User> Users =>
            new List<User> {
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = SD.DefaultNationalId,
                    Name = "امیررضا",
                    Family = "محمدی",
                    IsActive = true,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = "3360408330",
                    Name = "نادر",
                    Family = "یاری",
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = "3360408331",
                    Name = "رضا",
                    Family = "اسدی",
                    IsActive = true
                }
            };

        private List<UserJoinRequest> UserJoinRequests =>
            new List<UserJoinRequest>
            {
                new UserJoinRequest
                {
                    NationalId = "3360405050",
                    FullName = "نادر سلیمانی",
                    PhoneNumber = "09211572323",
                },
                new UserJoinRequest
                {
                    NationalId = "3360404050",
                    FullName = "محمد باقری",
                    PhoneNumber = "09371552693",
                },
            };

        #endregion

        #region Static

        private List<ProgramYear> ProgramYears =>
            new List<ProgramYear>
            {
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30E"),Year = "1401-1402"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30F"),Year = "1400-1401"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30A"),Year = "1399-1400"},
            };

        private List<IndicatorCategory> IndicatorCategories =>
            new List<IndicatorCategory>
            {
                new IndicatorCategory
                {
                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA98CB"),
                    Title = "فناوری اطلاعات",
                    Childs = new List<IndicatorCategory>
                    {
                        new IndicatorCategory
                        {
                            Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA98CA"),
                            Title = "سرور"
                        },
                        new IndicatorCategory
                        {
                            Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA98CE"),
                            Title = "شبکه و ارتباطات",
                            Childs = new List<IndicatorCategory>
                            {
                                new IndicatorCategory
                                {
                                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA97CB"),
                                    Title = "طبقه بندی 1"
                                },
                                new IndicatorCategory
                                {
                                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA96CB"),
                                    Title = "طبقه بندی 2"
                                }
                            }
                        },
                        new IndicatorCategory
                        {
                            Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA95CB"),
                            Title = "اطلاع رسانی",
                            Childs = new List<IndicatorCategory>
                            {
                                new IndicatorCategory
                                {
                                    Id =  Guid.Parse("D1089483-938C-401B-A003-BEFF50EA94CB"),
                                    Title = "طبقه بندی 1"
                                },
                                new IndicatorCategory
                                {
                                    Id =  Guid.Parse("D1089483-938C-401B-A003-BEFF50EA93CB"),
                                    Title = "طبقه بندی 2"
                                },
                                new IndicatorCategory
                                {
                                    Id =  Guid.Parse("D1089483-938C-401B-A003-BEFF50EA92CB"),
                                    Title = "طبقه بندی 3"
                                }
                            }
                        }
                    }
                },
                new IndicatorCategory
                {
                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA91CB"),
                    Title = "توسعه و پیشرفت",
                    Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA81CB"),
                             Title = "طبقه بندی 1"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA71CB"),
                             Title = "طبقه بندی 2"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA61CB"),
                             Title = "طبقه بندی 3"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA51CB"),
                             Title = "طبقه بندی 3"
                         }
                    }
                },
                new IndicatorCategory
                {
                    Id = Guid.Parse("D1089483-938C-401B-A003-BEFF50EA41CB"),
                    Title = "بهداشت و سلامت",
                    Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("C1089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 1",
                             Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("B1089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 1"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("A1089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 2"
                         },
                    }
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("91089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 2",
                             Childs = new List<IndicatorCategory>
                    {
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("81089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 1"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("71089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 2"
                         },
                    }
                         },
                         new IndicatorCategory
                         {
                             Id =Guid.Parse("61089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 3"
                         },
                         new IndicatorCategory
                         {
                             Id = Guid.Parse("51089483-938C-401B-A003-BEFF50EA41CB"),
                             Title = "طبقه بندی 3"
                         }
                    }
                },
            };

        private List<IndicatorType> IndicatorTypes =>
            new List<IndicatorType>
            {
                new IndicatorType
                {
                    Id = Guid.Parse("406FA9EC-1FB9-48DF-B6D1-90CEBB3F9F62"),
                    Title = "تعداد"
                },
                new IndicatorType
                {
                    Id =  Guid.Parse("406FA9EC-1FB9-48DF-B6D1-90CEBB3F9F61"),
                    Title = "تومان"
                },
                new IndicatorType
                {
                    Id =  Guid.Parse("406FA9EC-1FB9-48DF-B6D1-90CEBB3F9F60"),
                    Title = "ریال"
                },
            };
        #endregion

        private Indicator CustomeIndicator =>
                new Indicator
                {
                    Id = Guid.NewGuid(),
                    FromDate = DateTime.Now.AddDays(new Random().Next(-700, 0)),
                    ToDate = DateTime.Now.AddDays(new Random().Next(0, 700)),
                    CategoryId = IndicatorCategories[new Random().Next(0, IndicatorCategories.Count - 1)].Id,
                    GoalValue = new Random().Next(500, 10000),
                    InitValue = new Random().Next(0, 499),
                    Period = (IndicatorPeriod)(new Random().Next(0, 6) * 10),
                    TypeId = IndicatorTypes[new Random().Next(0, IndicatorTypes.Count - 1)].Id,
                };

        private String lorem = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است. چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد. کتابهای زیادی در شصت و سه درصد گذشته، حال و آینده شناخت فراوان جامعه و متخصصان را می طلبد تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی و فرهنگ پیشرو در زبان فارسی ایجاد کرد. در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها و شرایط سخت تایپ به پایان رسد وزمان مورد نیاز شامل حروفچینی دستاوردهای اصلی و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.";
    }
}
