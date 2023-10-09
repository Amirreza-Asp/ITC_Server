﻿using Domain.Dtos.Account.Users;
using Domain.Dtos.Companies;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompaniesUsersQuery : IRequest<List<CompanyUsers>>
    {
        public List<Guid> Companies { get; set; }
    }

    public class CompaniesUsersQueryHandler : IRequestHandler<CompaniesUsersQuery, List<CompanyUsers>>
    {
        private readonly ApplicationDbContext _context;

        public CompaniesUsersQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyUsers>> Handle(CompaniesUsersQuery request, CancellationToken cancellationToken)
        {
            var rnd = new Random();

            var data =
                 await _context.Users
              .Where(b =>
                      request.Companies.Contains(b.CompanyId.Value))
              .Select(op => new CompanyUsers
              {
                  CompanyId = op.CompanyId.Value,
                  CompanyName = op.Company.NameUniversity,
                  Users = new List<UserListDto>
                  {
                            new UserListDto
                            {
                                Id = op.Id,
                                NationalId = op.NationalId,
                                FullName = Names[rnd.Next(6)] + " " + Families[rnd.Next(6)],
                                IsAdmin = op.IsAdmin,
                                PhoneNumber = "0" + rnd.NextInt64(1000000000, 9999999999)
                            }
                  }
              })
              .ToListAsync(cancellationToken);

            var coo = new List<CompanyUsers>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                {
                    coo.Find(b => b.CompanyId == item.CompanyId).Users.Add(item.Users.First());
                }
                else
                {
                    coo.Add(item);
                }
            });


            return coo;
        }

        private List<String> Names =>
          new List<string>
          {
                "امیر",
                "محمد",
                "زهرا",
                "پریسا",
                "پیمان",
                "رضا",
          };

        private List<String> Families =>
            new List<string>
            {
                "قادری",
                "محمدی",
                "مرادی",
                "حیدری",
                "منوچهری",
                "نویدی",
            };
    }
}
