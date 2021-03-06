using EntityFramework.Firebird;
using EntityFramework.Functions;
using PulseData.TvlAlt;
using System.Data.Common;
using System.Data.Entity;

namespace PulseData
{
    public class TvlAltContext : DbContext
    {
        private static DbConnection GetConnection(string nameOrConnectionString)
        {
            return new FbConnectionFactory().CreateConnection(nameOrConnectionString);
        }

        public TvlAltContext(string nameOrConnectionString = "TvlAlt") : base(GetConnection(nameOrConnectionString), true)
        {
            Database.SetInitializer<TvlAltContext>(null);

#if DEBUG
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new FunctionConvention(typeof(FirebirdFunctions)));
        }

        public DbSet<Waveform> Waveforms { get; set; }
        public DbSet<RawRecord> RawRecords { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<IntervalRecord> IntervalRecords { get; set; }
    }
}
