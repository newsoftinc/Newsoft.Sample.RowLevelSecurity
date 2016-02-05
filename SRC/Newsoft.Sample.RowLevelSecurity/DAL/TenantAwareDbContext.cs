using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

using EntityFramework.DynamicFilters;

namespace Newsoft.Sample.RowLevelSecurity.DAL
{
    public class TenantAwareDbContext : DbContext
    {
        public TenantAwareDbContext()
        {
            Init();
        }

        public TenantAwareDbContext(string connectionString) : base(connectionString)
        {
            Init();
        }

        public TenantAwareDbContext(string connectionString, DbCompiledModel model) : base(connectionString, model)
        {
            Init();
        }
        protected internal virtual void Init()
        {
            this.InitializeDynamicFilters();
        }

        Guid? _currentTenantId = null; 

        public Guid? TenantId
        {
            get { return _currentTenantId; }
        }

        public void SetTenantId(Guid? tenantId)
        {
            _currentTenantId = tenantId;
            this.SetFilterScopedParameterValue("SecuredByTenant", "securedByTenantId", _currentTenantId);
            this.SetFilterGlobalParameterValue("SecuredByTenant", "securedByTenantId", _currentTenantId);

            var test =
            this.GetFilterParameterValue("SecuredByTenant", "securedByTenantId");
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Filter("SecuredByTenant",
                (ISecuredByTenant securedByTenant, Guid? securedByTenantId) => securedByTenant.SecuredByTenantId == securedByTenantId,
                () => Guid.Empty);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var createdEntries = GetCreatedEntries();

            if (createdEntries.Any())
            {
                foreach (var createdEntry in createdEntries)
                {
                    var iSecuredByTenantEntry = createdEntry.Entity as ISecuredByTenant;
                    if (iSecuredByTenantEntry != null)
                        iSecuredByTenantEntry.SecuredByTenantId = _currentTenantId;
                }
            }

            return base.SaveChanges();
        }

        private IEnumerable<DbEntityEntry> GetCreatedEntries()
        {
            var createdEntries = ChangeTracker.Entries().Where(V =>
                         EntityState.Added.HasFlag(V.State)
                    );
            return createdEntries;
        }
    }
}
