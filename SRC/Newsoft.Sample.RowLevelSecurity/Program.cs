using Newsoft.Sample.RowLevelSecurity.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsoft.Sample.RowLevelSecurity
{
    class Program
    {

        private static Guid tenant1Id = new Guid("F06FE619-21CC-E511-8113-005056BE3E20");
        private static Guid tenant2Id = new Guid("F16FE619-21CC-E511-8113-005056BE3E20");

        static void Main(string[] args)
        {
            //Restore northwind.bak and change your connection settings in the app.config file.

            //Database include two tenants (dbo.Tenant)
            //F06FE619-21CC-E511-8113-005056BE3E20 Tenant 1
            //F16FE619-21CC-E511-8113-005056BE3E20 Tenant 2

            //Database already has some employees assigned to tenant 1 and tenant 2
            var context = new NorthwindContext();

            SelectEmployeeForTenants(context);

            Console.WriteLine("Creating employee for tenant 1");
            CreateTenantEmployee(context,tenant1Id);

            SelectEmployeeForTenants(context);


            Console.ReadLine();

        }

        private static void CreateTenantEmployee(NorthwindContext context, Guid tenant)
        {
            context.SetTenantId(tenant);

            //Adding employee to tenant
            var chuck = new Employee();
            chuck.FirstName = "Chuck";
            chuck.LastName = "Norris";
            chuck.Address = "Everywhere";
            chuck.EmployeeId = new Random().Next();
            context.Employees.Add(chuck);
            context.SaveChanges();


        }

        private static void SelectEmployeeForTenants(NorthwindContext context)
        {
            context.SetTenantId(tenant1Id);

            var tenant1Employees = context.Employees.ToList();
            Console.WriteLine($"Tenant 1 has {tenant1Employees.Count} employees");

            context.SetTenantId(tenant2Id);

            var tenant2Employees = context.Employees.ToList();
            Console.WriteLine($"Tenant 2 has {tenant2Employees.Count} employees");
        }
    }
}
