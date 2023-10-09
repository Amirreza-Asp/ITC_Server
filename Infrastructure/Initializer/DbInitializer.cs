using Application.Services.Interfaces;
using Domain;
using Domain.Entities.Account;
using Domain.Entities.Business;
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

                            var rnd = new Random();

                            foreach (var project in Projects)
                            {
                                project.Id = Guid.NewGuid();
                                project.OperationalObjectiveId = opo.Id;
                                project.LeaderId = peopleId[rnd.Next(peopleId.Count - 1)];
                                _context.Projects.Add(project);
                            }

                            foreach (var practicalAction in PracticalActions)
                            {
                                practicalAction.Id = Guid.NewGuid();
                                practicalAction.OperationalObjectiveId = opo.Id;
                                practicalAction.LeaderId = peopleId[rnd.Next(peopleId.Count - 1)];
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
                    Progress = 94
                },
                new BigGoal{
                    Id =  Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "هوشمند سازی کلاس ها" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                    ProgramYearId = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30F"),
                    Progress = 73
                },
                new BigGoal{
                    Id =  Guid.Parse("4C6B36FE-4C0F-4D81-8F72-E9DBF80FC9DC"),
                    Deadline = DateTime.Now.AddYears(1) ,
                    StartedAt= DateTime.Now ,
                    Title = "افزایش سنوات دانشجویان" ,
                    Description = "مجازی سازی دانشگاه ها برای ایجاد برقراری ارتباط از راه دور و تدریس در هر شرایطی بدون نیاز به حضور در دانشگاه",
                    ProgramYearId = Guid.NewGuid(),
                    Progress = 73
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
                    Progress = 57
                },
                new OperationalObjective{
                    Id = Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 157000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "ایجاد سامانه اموزشی",
                    Progress = 78
                },
                new OperationalObjective{
                    Id = Guid.NewGuid(),
                    Deadline = DateTime.Now.AddYears(1),
                    Budget = 157000000,
                    Description = lorem,
                    GuaranteedFulfillmentAt = DateTime.Now.AddMonths(10),
                    Title = "حذف سامانه های قدیمی",
                    Progress = 93
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
                    },
                    Progress = 73
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
                    Progress = 55,
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
                    Progress = 65,
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
                    Progress = 93,
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

        private List<ProgramYear> ProgramYears =>
            new List<ProgramYear>
            {
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30E"),Year = "1401-1402"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30F"),Year = "1400-1401"},
                new ProgramYear{Id = Guid.Parse("C98775E6-C22E-4B4B-8D4F-A9A2F5ECC30A"),Year = "1399-1400"},
            };
        #endregion

        #region Account

        private List<Company> Companies()
        {
            var path = _hostEnv.WebRootPath + "/files/universitiesJson.txt";
            var jsonData = File.ReadAllText(path);

            var data = JsonSerializer.Deserialize<List<Company>>(jsonData, new JsonSerializerOptions
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


        private Guid UserRoleId = Guid.NewGuid();
        private Guid AgentRoleId = Guid.NewGuid();
        private String lorem = "لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده از طراحان گرافیک است. چاپگرها و متون بلکه روزنامه و مجله در ستون و سطرآنچنان که لازم است و برای شرایط فعلی تکنولوژی مورد نیاز و کاربردهای متنوع با هدف بهبود ابزارهای کاربردی می باشد. کتابهای زیادی در شصت و سه درصد گذشته، حال و آینده شناخت فراوان جامعه و متخصصان را می طلبد تا با نرم افزارها شناخت بیشتری را برای طراحان رایانه ای علی الخصوص طراحان خلاقی و فرهنگ پیشرو در زبان فارسی ایجاد کرد. در این صورت می توان امید داشت که تمام و دشواری موجود در ارائه راهکارها و شرایط سخت تایپ به پایان رسد وزمان مورد نیاز شامل حروفچینی دستاوردهای اصلی و جوابگوی سوالات پیوسته اهل دنیای موجود طراحی اساسا مورد استفاده قرار گیرد.";
    }
}
