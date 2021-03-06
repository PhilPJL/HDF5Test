using HDF.PInvoke;
using HDF5Api;
using PulseData.TvlAlt;
using System.Runtime.InteropServices;

namespace HDF5Test.H5TypeHelpers
{
    /// <summary>
    /// A type converter for <see cref="IntervalRecord"/>.
    /// </summary>
    public sealed class IntervalRecordAdapter : H5TypeAdapter<IntervalRecord, IntervalRecordAdapter.SIntervalRecord>
    {
        private IntervalRecordAdapter() { }

        protected override SIntervalRecord Convert(IntervalRecord source)
        {
            return new SIntervalRecord
            {
                Id = source.Id,
                Timestamp = source.Timestamp.ToOADate(),
                AverageThickness = source.AverageThickness ?? double.NaN,
                MinimumThickness = source.MinimumThickness ?? double.NaN,
                MaximumThickness = source.MaximumThickness ?? double.NaN,
            };
        }

        public override H5Type GetH5Type()
        {
            return H5Type
                .CreateCompoundType<SIntervalRecord>()
                .Insert<SIntervalRecord>(nameof(SIntervalRecord.Id), H5T.NATIVE_INT64)
                .Insert<SIntervalRecord>(nameof(SIntervalRecord.Timestamp), H5T.NATIVE_DOUBLE)
                .Insert<SIntervalRecord>(nameof(SIntervalRecord.AverageThickness), H5T.NATIVE_DOUBLE)
                .Insert<SIntervalRecord>(nameof(SIntervalRecord.MinimumThickness), H5T.NATIVE_DOUBLE)
                .Insert<SIntervalRecord>(nameof(SIntervalRecord.MaximumThickness), H5T.NATIVE_DOUBLE);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIntervalRecord
        {
            public long Id;
            public double Timestamp;
            public double AverageThickness;
            public double MinimumThickness;
            public double MaximumThickness;
        }

        public static IH5TypeAdapter<IntervalRecord> Default { get; } = new IntervalRecordAdapter();
    }
}
