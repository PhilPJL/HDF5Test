﻿using HDF.PInvoke;
using HDF5Api;
using HDF5Test.H5TypeHelpers;
using PulseData;
using System;
using System.Diagnostics;
using System.Linq;

namespace HDF5Test
{
    internal static class CreateFileTestRealData2
    {
        // Working demo of creating chunked dataset with compound type.
        internal unsafe static void CreateFile()
        {
            Console.WriteLine($"H5 version={H5Global.GetLibraryVersion()}");
            Console.WriteLine();

            // Create file
            using var file = H5File.Create(@"test-realdata.h5", H5F.ACC_TRUNC);

            int maxRows = 500;
            int chunkSize = 50;
            long measurementId = 12019;
            using var group = file.CreateGroup($"Measurement:{measurementId}");
            bool logTimePerChunk = true;
            uint compressionLevel = 1;

            using var altContext = new TvlAltContext();
            using var systemContext = new TvlSystemContext();

            // TODO: async queryable/cancellable
            // TODO: overlap?
            using (new DisposableStopWatch("Overall time", () => 0))
            {
                using var rawRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "RawRecords", RawRecordAdapter.Default, compressionLevel);
                using (var sw = new DisposableStopWatch("RawRecord", () => rawRecordWriter.CurrentPosition))
                {
                    altContext
                        .RawRecords
                        .Where(r => r.MeasurementId == measurementId)
                        .Take(maxRows)
                        .Buffer(chunkSize)
                        .ForEach(rg =>
                        {
                            rawRecordWriter.WriteChunk(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }

                using var intervalRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "IntervalRecords", IntervalRecordAdapter.Default, compressionLevel);
                using (var sw = new DisposableStopWatch("IntervalRecord", () => intervalRecordWriter.CurrentPosition))
                {
                    altContext
                        .IntervalRecords
                        .Where(r => r.RawRecords.Any(rr => rr.MeasurementId == measurementId))
                        .Take(maxRows)
                        .Buffer(chunkSize)
                        .ForEach(rg =>
                        {
                            intervalRecordWriter.WriteChunk(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }

                using var waveformWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "Waveforms", WaveformAdapter.Default, compressionLevel);
                using (var sw = new DisposableStopWatch("Waveform", () => waveformWriter.CurrentPosition))
                {
                    altContext
                        .Waveforms
                        .Where(w => w.RawRecord.MeasurementId == measurementId)
                        .Take(maxRows)
                        .Buffer(chunkSize)
                        .ForEach(rg =>
                        {
                            waveformWriter.WriteChunk(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }

                using var profileRecordWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "Profiles", ProfileAdapter.Default, compressionLevel);
                using (var sw = new DisposableStopWatch("Profile", () => profileRecordWriter.CurrentPosition))
                {
                    altContext
                        .Profiles
                        .Where(p => p.RawRecord.MeasurementId == measurementId)
                        .Take(maxRows)
                        .Buffer(chunkSize)
                        .ForEach(rg =>
                        {
                            profileRecordWriter.WriteChunk(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }

                using var measurementConfigurationWriter = H5DataSetWriter.CreateOneDimensionalDataSetWriter(group, "MeasurementConfigurations", MeasurementConfigurationAdapter.Default, compressionLevel);
                using (var sw = new DisposableStopWatch("MeasurementConfiguration", () => measurementConfigurationWriter.CurrentPosition))
                {
                    systemContext
                        .MeasurementConfigurations
                        .Where(mc => mc.Measurements.Any(m => m.Id == measurementId))
                        .Take(maxRows)
                        .Buffer(20)
                        .ForEach(rg =>
                        {
                            measurementConfigurationWriter.WriteChunk(rg);
                            sw.ShowRowsWritten(logTimePerChunk);
                        });
                }
            }
        }
    }

    class DisposableStopWatch : Disposable
    {
        readonly Stopwatch stopwatch = new();

        public DisposableStopWatch(string name, Func<int> getPosition)
        {
            stopwatch.Start();
            Name = name;
            GetPosition = getPosition;
        }

        private string Name { get; }
        private Func<int> GetPosition { get; }

        public void ShowRowsWritten(bool show)
        {
            if (show)
            {
                Console.WriteLine($"{Name} - time elapsed: {stopwatch.Elapsed}. Rows written = {GetPosition()}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            stopwatch.Stop();
            Console.WriteLine($"{Name} - Complete. Time elapsed: {stopwatch.Elapsed}. Rows written = {GetPosition()}");
            Console.WriteLine();
        }
    }
}

