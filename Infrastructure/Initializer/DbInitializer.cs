using Application.Services.Interfaces;
using Domain;
using Domain.Entities.Account;
using Domain.Entities.Business;
using Domain.Entities.Static;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public DbInitializer(ApplicationDbContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
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

            if (!_context.Company.Any())
                _context.Company.AddRange(Companies());

            if (!_context.ProgramYears.Any())
                _context.ProgramYears.AddRange(ProgramYears);



            if (!_context.IndicatorCategories.Any())
                _context.IndicatorCategories.AddRange(IndicatorCategories);

            if (!_context.IndicatorTypes.Any())
                _context.IndicatorTypes.AddRange(IndicatorTypes);

            if (!_context.BigGoals.Any())
            {
                foreach (var comp in Companies())
                {
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

                            _context.Indicators.Add(customeInd);
                            _context.BigGoalIndicators.Add(bid);
                        }

                        _context.BigGoals.Add(bigGoal);

                        List<Guid> peopleId = new List<Guid>();

                        foreach (var person in People)
                        {
                            person.CompanyId = comp.Id;
                            person.Id = Guid.NewGuid();
                            peopleId.Add(person.Id);
                            _context.People.Add(person);
                        }


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

                                _context.Indicators.Add(ind);
                                _context.OperationalObjectiveIndicators.Add(opoInd);
                            }

                            foreach (var project in Projects)
                            {
                                project.Id = Guid.NewGuid();
                                project.OperationalObjectiveId = opo.Id;
                                project.LeaderId = peopleId[rnd.Next(peopleId.Count - 1)];

                                for (int i = 0; i < rnd.Next(1, 6); i++)
                                {
                                    var ind = CustomeIndicator;
                                    ind.Id = Guid.NewGuid();

                                    var projectInd = new ProjectIndicator { IndicatorId = ind.Id, ProjectId = project.Id };

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

                                for (int i = 0; i < rnd.Next(1, 6); i++)
                                {
                                    var ind = CustomeIndicator;
                                    ind.Id = Guid.NewGuid();

                                    var prInd = new PracticalActionIndicator { IndicatorId = ind.Id, PracticalActionId = practicalAction.Id };

                                    _context.Indicators.Add(ind);
                                    _context.PracticalActionIndicators.Add(prInd);
                                }

                                _context.PracticalActions.Add(practicalAction);
                            }
                        }


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

                    foreach (var user in Users)
                    {
                        user.CompanyId = comp.Id;
                        user.Id = Guid.NewGuid();
                        var rnd = new Random();
                        user.NationalId = rnd.NextInt64(1111111111, 9999999999).ToString();
                        _context.Users.Add(user);
                    }

                    foreach (var role in Roles)
                    {
                        role.CompanyId = comp.Id;
                        role.Id = Guid.NewGuid();
                        _context.Add(role);
                    }

                    foreach (var ujr in UserJoinRequests)
                    {
                        ujr.CompanyId = comp.Id;
                        ujr.Id = Guid.NewGuid();
                        _context.UsersJoinRequests.Add(ujr);
                    }
                }
            }

            if (!_context.Roles.Any())
                _context.AddRange(Roles);

            if (!_context.Permissions.Any())
                _context.Permissions.Add(Permissions);

            if (!_context.Users.Any())
                _context.Users.AddRange(Users);

            _context.SaveChanges();

            if (!_context.RolePermissions.Any())
            {
                var permissions = _context.Permissions.Where(b => b.Discriminator == nameof(PermissionItem)).ToList();
                var adminPermissions = permissions.Select(b => new RolePermission { Id = Guid.NewGuid(), PermissionId = b.Id, RoleId = SD.AdminRoleId });
                _context.RolePermissions.AddRange(adminPermissions.DistinctBy(b => b.Id));
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
                    BuildInCompany = "ویتکو",
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
                    BuildInCompany = "ویتکو",
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
                    BuildInCompany = "ویتکو",
                    Framework = lorem,
                    OS = "ویندوز",
                    ProgrammingLanguage = "Python",
                    Description= lorem,
                    SupportType = lorem
                },
            };

        #endregion

        #region Account

        private List<Company> Companies()
        {
            //var path = _hostEnv.WebRootPath + "/files/smallUniversitiesJson.txt";
            //var jsonData = File.ReadAllText(path);

            var data = JsonSerializer.Deserialize<List<Company>>(comps, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data;
        }

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
                            //********* company *********
                            new PermissionContainer("اهداف کلان" , PermissionType.Company ,
                                new List<Permission>
                            {
                                new PermissionItem("افزودن هدف کلان" , PermissionType.Company , PermissionsSD.Company_AddBigGoal),
                                new PermissionItem("ویرایش هدف کلان", PermissionType.Company , PermissionsSD.Company_EditBigGoal),
                                new PermissionItem("حذف هدف کلان" , PermissionType.Company,  PermissionsSD.Company_RemoveBigGoal)
                            }),
                            new PermissionContainer("اهداف عملیاتی" , PermissionType.Company ,
                                new List<Permission>
                            {
                                new PermissionItem("افزودن هدف عملیاتی"  , PermissionType.Company,  PermissionsSD.Company_AddOperationalObjective),
                                new PermissionItem("ویرایش هدف عملیاتی" , PermissionType.Company ,  PermissionsSD.Company_EditOperationalObjective),
                                new PermissionItem("حذف هدف عملیاتی" , PermissionType.Company ,  PermissionsSD.Company_RemoveOperationalObjective),
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
                                        new PermissionItem("حذف پروژه" , PermissionType.Company ,  PermissionsSD.Company_RemoveProject)
                                    }),
                                new PermissionContainer("اقدامات کاربری" , PermissionType.Company ,
                                    new List<Permission>
                                    {
                                        new PermissionItem("افزودن اقدام کاربردی" , PermissionType.Company ,  PermissionsSD.Company_AddPracticalAction),
                                        new PermissionItem("ویرایش اقدام کاربردی" , PermissionType.Company ,  PermissionsSD.Company_EditPracticalAction),
                                        new PermissionItem("حذف اقدام کاربردی" , PermissionType.Company ,  PermissionsSD.Company_RemovePracticalAction)
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
                    RoleId = AgentRoleId,
                    IsAdmin = true,
                    IsActive = true,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = "3360408330",
                    RoleId = UserRoleId,
                    IsActive = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    NationalId = "3360408331",
                    RoleId = UserRoleId,
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

        String comps = " [{\r\n        \"id\": \"159c1e74-f30f-4387-bbae-98c9f1669256\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی آی سودا\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/159c1e74-f30f-4387-bbae-98c9f1669256\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"کرج\",\r\n        \"provinceName\": \"البرز\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"fd7ec6a2-f226-472d-a286-98d69e67a757\",\r\n        \"nameUniversity\": \"آموزشکده فنی وحرفه ای پسران ایلام\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/fd7ec6a2-f226-472d-a286-98d69e67a757\",\r\n        \"portalUrl\": \"p-ilam.tvu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"ایلام\",\r\n        \"provinceName\": \"ایلام\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"6024a4a2-8a5a-4f42-9a83-98e5e944a2e0\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی جمعیت هلال احمر استان کرمان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/6024a4a2-8a5a-4f42-9a83-98e5e944a2e0\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"کرمان\",\r\n        \"provinceName\": \"کرمان\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:46\"\r\n    },\r\n    {\r\n        \"id\": \"278990d5-05ec-49e4-98f1-990e87a282db\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی خانه کارگر جمهوری اسلامی ایران تشکیلات استان خوزستان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/278990d5-05ec-49e4-98f1-990e87a282db\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"اهواز\",\r\n        \"provinceName\": \"خوزستان\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"30f7cfc8-05b4-4ef1-80ea-994308686899\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی میاندوآب\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/30f7cfc8-05b4-4ef1-80ea-994308686899\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"میاندوآب\",\r\n        \"provinceName\": \"آذربایجان غربی\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"d6cf5cca-15e9-4ca8-8c0d-9948cf29134a\",\r\n        \"nameUniversity\": \"دانشگاه آزاد اسلامی آموزشکده فنی و حرفه ای سما رودبار\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/d6cf5cca-15e9-4ca8-8c0d-9948cf29134a\",\r\n        \"portalUrl\": \"www.roudbar-samacollege.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"رودبار\",\r\n        \"provinceName\": \"گیلان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"52a9026f-4cd6-4904-84b0-994f0638226d\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی فدراسیون پهلوانی و زورخانه‌ای جمهوری اسلامی ایران\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/52a9026f-4cd6-4904-84b0-994f0638226d\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"d0832416-a034-4680-8f43-9958e3481183\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی فرهنگ و هنر واحد 20 تهران\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/d0832416-a034-4680-8f43-9958e3481183\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"43717eac-d491-4fd6-b55a-997a85ff4b79\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی گروه صنعتی گلرنگ\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/43717eac-d491-4fd6-b55a-997a85ff4b79\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"1e939b8c-ac00-4f34-bafc-99e35e9c808d\",\r\n        \"nameUniversity\": \"دانشگاه آزاد اسلامی واحدرودسر \",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/1e939b8c-ac00-4f34-bafc-99e35e9c808d\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"رودسر\",\r\n        \"provinceName\": \"گیلان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"773fca5c-b795-4f7c-a6ff-99e9b9ccb6a8\",\r\n        \"nameUniversity\": \"پارک علم و فناوری استان گلستان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/773fca5c-b795-4f7c-a6ff-99e9b9ccb6a8\",\r\n        \"portalUrl\": \"https://www.gstpark.ir/\",\r\n        \"singleWindowUrl\": \"https://www.gstpark.ir/\",\r\n        \"nationalSerial\": null,\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"گرگان\",\r\n        \"provinceName\": \"گلستان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"43aa7e52-deb8-4837-9d96-99eff691c9e4\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی  هواپیمایی ساها\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/43aa7e52-deb8-4837-9d96-99eff691c9e4\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"64d5359f-63f6-4c0c-a244-9a0bc2608c58\",\r\n        \"nameUniversity\": \"موسسه آموزش عالی غیردولتی  غیرانتفاعی جهاد دانشگاهی - رشت\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/64d5359f-63f6-4c0c-a244-9a0bc2608c58\",\r\n        \"portalUrl\": \"https://jdrasht.ac.ir\",\r\n        \"singleWindowUrl\": \"https://jdrasht.ac.ir\",\r\n        \"nationalSerial\": null,\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"رشت\",\r\n        \"provinceName\": \"گیلان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"7f1216c8-ee9b-4167-b0b7-9a104e109541\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی جهاد دانشگاهی تهران 3\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/7f1216c8-ee9b-4167-b0b7-9a104e109541\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"شهریار\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"9fc88848-d54c-4468-a863-9a13bf14ddda\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور واحد شوط\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/9fc88848-d54c-4468-a863-9a13bf14ddda\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"شوط\",\r\n        \"provinceName\": \"آذربایجان غربی\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"21b2e3b8-6000-42fa-9938-9a5382cc4dca\",\r\n        \"nameUniversity\": \"سازمان پژوهش های علمی و صنعتی\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/21b2e3b8-6000-42fa-9938-9a5382cc4dca\",\r\n        \"portalUrl\": \"https://ka.irost.org\",\r\n        \"singleWindowUrl\": \"https://ka.irost.org\",\r\n        \"nationalSerial\": null,\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"سازمان‌های تابعه وزارت عتف\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1402/01/23 14:36\"\r\n    },\r\n    {\r\n        \"id\": \"65b80a28-ded1-456d-8784-9a58cf60ba3d\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز دزفول\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/65b80a28-ded1-456d-8784-9a58cf60ba3d\",\r\n        \"portalUrl\": \"dez.khz.pnu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"دزفول\",\r\n        \"provinceName\": \"خوزستان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"93900aa1-bee5-4efd-b6e4-9a8b5e92e263\",\r\n        \"nameUniversity\": \"آموزشکده فنی و حرفه ای پسران شماره دو یزد\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/93900aa1-bee5-4efd-b6e4-9a8b5e92e263\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"یزد\",\r\n        \"provinceName\": \"یزد\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"0d32f638-828a-4bd9-b4c9-9aa16e8005ac\",\r\n        \"nameUniversity\": \"دانشگاه فرهنگیان پردیس شهید مدرس سنندج\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/0d32f638-828a-4bd9-b4c9-9aa16e8005ac\",\r\n        \"portalUrl\": \"http://kurdestan.cfu.ac.ir/\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"سنندج\",\r\n        \"provinceName\": \"کردستان\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"364be879-b014-45bc-a177-9ab45e5060e4\",\r\n        \"nameUniversity\": \"دانشگاه آزاد اسلامی واحد میاندوآب\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/364be879-b014-45bc-a177-9ab45e5060e4\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"میاندوآب\",\r\n        \"provinceName\": \"آذربایجان غربی\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"c1386e8e-9fd9-46a6-a229-9b031cc8b369\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی جهاد دانشگاهی تهران 1\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/c1386e8e-9fd9-46a6-a229-9b031cc8b369\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"6894109f-34e4-4b3b-b67e-9b2192fdf92c\",\r\n        \"nameUniversity\": \"موسسه آموزش عالی غیر دولتی-غیر انتفاعی الوند\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/6894109f-34e4-4b3b-b67e-9b2192fdf92c\",\r\n        \"portalUrl\": \"www.alvand.ac.ir\",\r\n        \"singleWindowUrl\": \"www.alvand.ac.ir\",\r\n        \"nationalSerial\": null,\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"همدان\",\r\n        \"provinceName\": \"همدان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"5cd31667-dbfb-4ddf-8adb-9b744f9d02d6\",\r\n        \"nameUniversity\": \"آموزشکده فنی و حرفه ای پسران شیروان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/5cd31667-dbfb-4ddf-8adb-9b744f9d02d6\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"شیروان\",\r\n        \"provinceName\": \"خراسان شمالی\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"278850e2-8a95-490f-a29c-9bbfcbb84efd\",\r\n        \"nameUniversity\": \"دانشگاه فرهنگیان پردیس شهید مطهری خوی\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/278850e2-8a95-490f-a29c-9bbfcbb84efd\",\r\n        \"portalUrl\": \"http://motahari.te.cfu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"خوی\",\r\n        \"provinceName\": \"آذربایجان غربی\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"bcae6c4d-8ae3-430a-bc66-9bd9fc591880\",\r\n        \"nameUniversity\": \"دانشگاه فرهنگیان پردیس حکیم ابوالقاسم  فردوسی\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/bcae6c4d-8ae3-430a-bc66-9bd9fc591880\",\r\n        \"portalUrl\": \"Hakim.cfu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": \"14002452080\",\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"کرج\",\r\n        \"provinceName\": \"البرز\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"16f2b448-72e2-417a-a73f-9bdb8c230650\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی نیشابور 1\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/16f2b448-72e2-417a-a73f-9bdb8c230650\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"نیشابور\",\r\n        \"provinceName\": \"خراسان رضوی\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"19b14670-5014-4247-a2ee-9c13ad06ee50\",\r\n        \"nameUniversity\": \"دانشگاه آزاد اسلامی واحد نطنز\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/19b14670-5014-4247-a2ee-9c13ad06ee50\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"نطنز\",\r\n        \"provinceName\": \"اصفهان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"fccc1945-4b1b-447e-ac14-9c2591df4ee0\",\r\n        \"nameUniversity\": \"دانشگاه جامع انقلاب اسلامی\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/fccc1945-4b1b-447e-ac14-9c2591df4ee0\",\r\n        \"portalUrl\": \"https://edu.cuir.ac.ir/index.html\",\r\n        \"singleWindowUrl\": \"https://edu.cuir.ac.ir/index.html\",\r\n        \"nationalSerial\": \"۱۴۰۰۳۵۳۷۸۸۶\",\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:46\"\r\n    },\r\n    {\r\n        \"id\": \"0ea36a9d-c420-4716-b4ba-9c2fbd47319d\",\r\n        \"nameUniversity\": \"دانشگاه صنعتی شیراز\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/0ea36a9d-c420-4716-b4ba-9c2fbd47319d\",\r\n        \"portalUrl\": \"www.sutech.ac.ir\",\r\n        \"singleWindowUrl\": \"www.sutech.ac.ir\",\r\n        \"nationalSerial\": null,\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"شیراز\",\r\n        \"provinceName\": \"فارس\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"217a8f4e-1475-4845-969d-9c4e900e20ec\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز تفت\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/217a8f4e-1475-4845-969d-9c4e900e20ec\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تفت\",\r\n        \"provinceName\": \"یزد\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"91eff97a-4bf1-471d-bf03-9c88f5b6a196\",\r\n        \"nameUniversity\": \"موسسه آموزش عالی غیردولتی ـ غیرانتفاعی مهر کرمان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/91eff97a-4bf1-471d-bf03-9c88f5b6a196\",\r\n        \"portalUrl\": \"www.mehrkerman.ac.ir\",\r\n        \"singleWindowUrl\": \"www.mehrkerman.ac.ir\",\r\n        \"nationalSerial\": \"10630064187\",\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"کرمان\",\r\n        \"provinceName\": \"کرمان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"c75f85a0-1709-4bb2-9e74-9c9a3c4ecf85\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز منجیل\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/c75f85a0-1709-4bb2-9e74-9c9a3c4ecf85\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"رودبار\",\r\n        \"provinceName\": \"گیلان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"f50cc288-b534-4bcf-bd34-9ca1f7ffb052\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی شهرداری هفتگل\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/f50cc288-b534-4bcf-bd34-9ca1f7ffb052\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"هفتگل\",\r\n        \"provinceName\": \"خوزستان\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"e66eb345-80a0-4ca1-a7ea-9ca9408e3e5e\",\r\n        \"nameUniversity\": \"موسسه آموزش عالی غیردولتی - غیرانتفاعی علوم و فناوری سپاهان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/e66eb345-80a0-4ca1-a7ea-9ca9408e3e5e\",\r\n        \"portalUrl\": \"www.seahan.ac.ir\",\r\n        \"singleWindowUrl\": \"www.seahan.ac.ir\",\r\n        \"nationalSerial\": \"10260616617\",\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"اصفهان\",\r\n        \"provinceName\": \"اصفهان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"9baf7cb4-6447-40cc-923f-9cb323575c05\",\r\n        \"nameUniversity\": \"دانشگاه آزاد اسلامی مرکز تکاب\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/9baf7cb4-6447-40cc-923f-9cb323575c05\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تکاب\",\r\n        \"provinceName\": \"آذربایجان غربی\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"edf7306a-0be5-4c9f-84d6-9cd3f6e1a061\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی فارسان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/edf7306a-0be5-4c9f-84d6-9cd3f6e1a061\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"فارسان\",\r\n        \"provinceName\": \"چهارمحال و بختیاری\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"d4297893-bc6a-4e25-b3ac-9cdabc339bc1\",\r\n        \"nameUniversity\": \"آموزشکده فنی و حرفه ای دختران خوانسار\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/d4297893-bc6a-4e25-b3ac-9cdabc339bc1\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"خوانسار\",\r\n        \"provinceName\": \"اصفهان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"252a72f5-2688-48dc-8ea5-9ceeb8fd04d7\",\r\n        \"nameUniversity\": \"دانشگاه جیرفت\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/252a72f5-2688-48dc-8ea5-9ceeb8fd04d7\",\r\n        \"portalUrl\": \"www.Ujiroft.ac.ir\",\r\n        \"singleWindowUrl\": \"www.Ujiroft.ac.ir\",\r\n        \"nationalSerial\": \"14003183763\",\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"جیرفت\",\r\n        \"provinceName\": \"کرمان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"f3911a82-c4d4-40fd-97ed-9cf2b7b86240\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز آبادان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/f3911a82-c4d4-40fd-97ed-9cf2b7b86240\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"آبادان\",\r\n        \"provinceName\": \"خوزستان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"a6ba70f0-468f-448d-b877-9cf351b6bb8c\",\r\n        \"nameUniversity\": \"دانشگاه آزاد اسلامی آموزشکده فنی و حرفه ای سما دماوند\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/a6ba70f0-468f-448d-b877-9cf351b6bb8c\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"دماوند\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"6ec6e80b-c533-4e82-8f54-9cfb140c1754\",\r\n        \"nameUniversity\": \"پژوهشکده مواد و زیست مواد\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/6ec6e80b-c533-4e82-8f54-9cfb140c1754\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"e9933d43-1196-4b8e-a799-9d0ac11caf83\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی فرهنگ و هنر واحد 30 تهران\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/e9933d43-1196-4b8e-a799-9d0ac11caf83\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"c1a30439-614c-453b-8bb9-9d1eede7df5f\",\r\n        \"nameUniversity\": \"دانشگاه آزاد اسلامی واحد اشکذر\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/c1a30439-614c-453b-8bb9-9d1eede7df5f\",\r\n        \"portalUrl\": \"www.iauashkezar.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"اشکذر\",\r\n        \"provinceName\": \"یزد\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"a688c10c-b42f-4b0a-adf5-9d2bd0b67c03\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور واحد آبسرد\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/a688c10c-b42f-4b0a-adf5-9d2bd0b67c03\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"آبسرد\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"e47920dd-b84a-40a1-9b3a-9d393f361ae1\",\r\n        \"nameUniversity\": \"آموزشکده کشاورزی پسران مراغه\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/e47920dd-b84a-40a1-9b3a-9d393f361ae1\",\r\n        \"portalUrl\": \"k-maragheh.tvu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"مراغه\",\r\n        \"provinceName\": \"آذربایجان شرقی\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"e5f8912e-0138-4d0f-b2c8-9d3d2c59f844\",\r\n        \"nameUniversity\": \"آموزشکده فنی وحرفه ای دختران کازرون\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/e5f8912e-0138-4d0f-b2c8-9d3d2c59f844\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"کازرون\",\r\n        \"provinceName\": \"فارس\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"a3200677-5f02-4ffa-a0e6-9d57758e44b4\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز خرم آباد\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/a3200677-5f02-4ffa-a0e6-9d57758e44b4\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"خرم آباد\",\r\n        \"provinceName\": \"لرستان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"9013b14f-b10c-441b-99cb-9d6d1e9d7cfb\",\r\n        \"nameUniversity\": \"موسسه آموزش عالی غیر دولتی-غیر انتفاعی ادیبان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/9013b14f-b10c-441b-99cb-9d6d1e9d7cfb\",\r\n        \"portalUrl\": \"www.adiban.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": \"10480009193\",\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"گرمسار\",\r\n        \"provinceName\": \"سمنان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"c6311e39-7186-4e35-900b-9d8e4853c593\",\r\n        \"nameUniversity\": \"آموزشکده فنی و حرفه ای دختران یزد\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/c6311e39-7186-4e35-900b-9d8e4853c593\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"یزد\",\r\n        \"provinceName\": \"یزد\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"0e09ab0f-2225-402b-9cc5-9daa1bf4eb63\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور واحد باغ بهادران\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/0e09ab0f-2225-402b-9cc5-9daa1bf4eb63\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"باغ بهادران\",\r\n        \"provinceName\": \"اصفهان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"b3d79927-4b96-4dd9-a0a4-9dccf2d5cafb\",\r\n        \"nameUniversity\": \"موسسه آموزش عالی غیردولتی  غیرانتفاعی پیشتازان - شیراز\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/b3d79927-4b96-4dd9-a0a4-9dccf2d5cafb\",\r\n        \"portalUrl\": \"pishtazan.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": \"14004544826\",\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"شیراز\",\r\n        \"provinceName\": \"فارس\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"0ad3de07-6770-442f-922f-9df926070ca9\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی جمعیت هلال احمر استان فارس\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/0ad3de07-6770-442f-922f-9df926070ca9\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"شیراز\",\r\n        \"provinceName\": \"فارس\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"fcd24fc2-4f3f-4823-8cf4-9dfbbc3a349f\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز سبزوار\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/fcd24fc2-4f3f-4823-8cf4-9dfbbc3a349f\",\r\n        \"portalUrl\": \"www.pnusab.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": \"14002921527\",\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"سبزوار\",\r\n        \"provinceName\": \"خراسان رضوی\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"f27b463c-0d71-4500-9f0c-9dfc21d25be4\",\r\n        \"nameUniversity\": \"دانشگاه فرهنگیان پردیس امام سجاد(ع) بیرجند\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/f27b463c-0d71-4500-9f0c-9dfc21d25be4\",\r\n        \"portalUrl\": \"asb.cfu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"بیرجند\",\r\n        \"provinceName\": \"خراسان جنوبی\",\r\n        \"universityType\": \"موسسات وابسته به دستگاه های اجرایی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"f1e931ec-fc34-4a39-a6c6-9e24b5b9957e\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی دهدشت\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/f1e931ec-fc34-4a39-a6c6-9e24b5b9957e\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"دهدشت\",\r\n        \"provinceName\": \"کهگیلویه و بویراحمد\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:46\"\r\n    },\r\n    {\r\n        \"id\": \"40f6ae08-5c26-4161-8273-9e25a4e72117\",\r\n        \"nameUniversity\": \"آموزشکده فنی و حرفه ای پسران سبزوار\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/40f6ae08-5c26-4161-8273-9e25a4e72117\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"سبزوار\",\r\n        \"provinceName\": \"خراسان رضوی\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"f486f5cc-5af5-451a-b68f-9e401d57e818\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور واحد آستانه اشرفیه\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/f486f5cc-5af5-451a-b68f-9e401d57e818\",\r\n        \"portalUrl\": \"astane.gilan.pnu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"آستانه اشرفیه\",\r\n        \"provinceName\": \"گیلان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"0cfab680-3612-442c-80c1-9e6377c9a078\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز برازجان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/0cfab680-3612-442c-80c1-9e6377c9a078\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"برازجان\",\r\n        \"provinceName\": \"بوشهر\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"c8c9d6b5-e179-483b-8e76-9e73468bc74c\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی شرکت تولیدی شیمیایی کلران\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/c8c9d6b5-e179-483b-8e76-9e73468bc74c\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"سمنان\",\r\n        \"provinceName\": \"سمنان\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"21cc252b-46e7-4566-8d1e-9e7eeb1f2319\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی فرماندهی انتظامی استان فارس\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/21cc252b-46e7-4566-8d1e-9e7eeb1f2319\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"شیراز\",\r\n        \"provinceName\": \"فارس\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:46\"\r\n    },\r\n    {\r\n        \"id\": \"dd599734-46fa-40d8-9bcd-9e9418963e98\",\r\n        \"nameUniversity\": \"دانشگاه پیام نور مرکز فارسان\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/dd599734-46fa-40d8-9bcd-9e9418963e98\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"فارسان\",\r\n        \"provinceName\": \"چهارمحال و بختیاری\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"ad7eebd7-c1e8-48cc-843f-9ea3be9e951a\",\r\n        \"nameUniversity\": \"مرکز آموزش علمی - کاربردی شرکت ماشین‌سازی اراک\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/ad7eebd7-c1e8-48cc-843f-9ea3be9e951a\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"اراک\",\r\n        \"provinceName\": \"مرکزی\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:46\"\r\n    },\r\n    {\r\n        \"id\": \"8bc980bf-dd0c-4a56-b0ac-9ea714b69f46\",\r\n        \"nameUniversity\": \"آموزشکده فنی و حرفه ای پسران شماره دو زاهدان(شهیدعارفی)\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/8bc980bf-dd0c-4a56-b0ac-9ea714b69f46\",\r\n        \"portalUrl\": \"p2-zahedan.tvu.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"زاهدان\",\r\n        \"provinceName\": \"سیستان و بلوچستان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"901187da-eb42-443b-a602-9eb9bfbb7206\",\r\n        \"nameUniversity\": \"مركز آموزش علمی - كاربردی فرماندهی انتظامی استان اصفهان(انصارالمهدی)\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/901187da-eb42-443b-a602-9eb9bfbb7206\",\r\n        \"portalUrl\": null,\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": null,\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"اصفهان\",\r\n        \"provinceName\": \"اصفهان\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"441a54a0-5594-4648-bcbc-9ed3a179cdf3\",\r\n        \"nameUniversity\": \"دانشگاه غیردولتی - غیرانتفاعی سوره\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/441a54a0-5594-4648-bcbc-9ed3a179cdf3\",\r\n        \"portalUrl\": \"www.soore.ac.ir\",\r\n        \"singleWindowUrl\": null,\r\n        \"nationalSerial\": \"10100338200\",\r\n        \"latitude\": null,\r\n        \"longitude\": null,\r\n        \"cityName\": \"تهران\",\r\n        \"provinceName\": \"تهران\",\r\n        \"universityType\": \"موسسات غیر دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    },\r\n    {\r\n        \"id\": \"aa12b4d2-652c-407a-a569-9edcd1e2c467\",\r\n        \"nameUniversity\": \"دانشگاه رازی - کرمانشاه\",\r\n        \"logoUniversity\": \"https://usw.msrt.ir/api/v1/Download/ImagesUniversitiesLogo/aa12b4d2-652c-407a-a569-9edcd1e2c467\",\r\n        \"portalUrl\": \"www.razi.ac.ir\",\r\n        \"singleWindowUrl\": \"www.razi.ac.ir\",\r\n        \"nationalSerial\": \"14003093474\",\r\n        \"latitude\": \"null\",\r\n        \"longitude\": \"null\",\r\n        \"cityName\": \"کرمانشاه\",\r\n        \"provinceName\": \"کرمانشاه\",\r\n        \"universityType\": \"موسسات دولتی\",\r\n        \"status\": \"فعال\",\r\n        \"createAt\": \"1401/12/23 17:45\"\r\n    }]";

        private Guid UserRoleId = Guid.NewGuid();
        private Guid AgentRoleId = Guid.NewGuid();
        private String lorem = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است. چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد. کتابهای زیادی در شصت و سه درصد گذشته، حال و آینده شناخت فراوان جامعه و متخصصان را می طلبد تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی و فرهنگ پیشرو در زبان فارسی ایجاد کرد. در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها و شرایط سخت تایپ به پایان رسد وزمان مورد نیاز شامل حروفچینی دستاوردهای اصلی و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.";
    }
}
