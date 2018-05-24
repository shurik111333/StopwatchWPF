using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StopWatch.Models.Tests
{
    [TestClass()]
    public class StopwatchModelTests
    {
        private StopwatchModel _model;

        [TestInitialize]
        public void Init()
        {
            _model = new StopwatchModel();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _model = null;
        }

        [TestMethod()]
        public void StopwatchModelTest()
        {
            Assert.AreEqual(0, _model.Elapsed.Ticks);
            Assert.IsFalse(_model.Laps.Any(), "Laps is not empty");

            CheckStopped(_model);
        }

        [TestMethod()]
        public void StartTest()
        {
            _model.Start();
            Thread.Sleep(10);
            Assert.IsTrue(0 < _model.Elapsed.Ticks, 
                "Stopwatch doesn't start. Elapsed.Ticks = {0}", _model.Elapsed.Ticks);

            CheckRunning(_model);

            var stamp = _model.Elapsed.Ticks;
            _model.Start();
            Thread.Sleep(10);
            var actual = _model.Elapsed.Ticks;
            Assert.IsTrue(stamp < actual, 
                "Stamp = {0}, Elapsed = {1}", stamp, actual);
        }

        [TestMethod()]
        public void StopTest()
        {
            _model.Start();
            Thread.Sleep(100);
            _model.Stop();

            CheckPaused(_model);

            var expected = _model.Elapsed.Ticks;
            Assert.IsTrue(100 <= _model.Elapsed.TotalMilliseconds, 
                "Stopwatch shows less than 100 milliseconds");
            Thread.Sleep(100);
            Assert.AreEqual(expected, _model.Elapsed.Ticks, "Time has changed after stopping");

            _model.Stop();
            Thread.Sleep(10);
            Assert.AreEqual(expected, _model.Elapsed.Ticks, 
                "Time has changed after second stopping in a row");
        }

        /// <summary>
        /// Checks that stopwatch can continue after stopping.
        /// </summary>
        [TestMethod]
        public void StartAfterStopTest()
        {
            _model.Start();
            Thread.Sleep(100);
            _model.Stop();

            var stamp = _model.Elapsed.Ticks;
            _model.Start();
            Thread.Sleep(10);

            CheckRunning(_model);

            Assert.IsTrue(stamp < _model.Elapsed.Ticks, 
                "Time doesn't continue after resuming");
        }

        [TestMethod()]
        public void LapCreatesTest()
        {
            _model.Lap();
            Assert.AreEqual(1, _model.Laps.Count, "Zero time was not added");
            Assert.IsTrue(_model.Laps.Contains(TimeSpan.Zero), 
                "Laps doesn't contains zero time");

            _model.Start();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(10);
                _model.Lap();
                int expectedNumber = i + 2;
                int actual = _model.Laps.Count;
                Assert.AreEqual(expectedNumber, actual, 
                    "i = {0}, number of laps: expected = {1}, actual = {2}", i, expectedNumber, actual);
            }
        }

        [TestMethod]
        public void LapCreatesCorrectStampsTest()
        {
            _model.Start();
            var expectedStamps = new List<TimeSpan>();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(10);
                _model.Stop();
                expectedStamps.Add(_model.Elapsed);
                _model.Lap();
                _model.Start();
            }
            Assert.IsTrue(expectedStamps.SequenceEqual(_model.Laps),
                "Expected laps = [{0}], Actual = [{1}]", 
                string.Join(", ", expectedStamps), string.Join(", ", _model.Laps));
        }

        [TestMethod()]
        public void ResetTest()
        {
            // Checks that stopwatch resets after stop
            _model.Start();
            Thread.Sleep(100);
            _model.Stop();
            CheckPaused(_model);
            _model.Reset();
            CheckStopped(_model);

            // Checks that sopwatch resets while running
            _model.Start();
            Thread.Sleep(100);
            _model.Reset();
            CheckStopped(_model);

            // Checks that laps are cleared after reset
            _model.Start();
            Thread.Sleep(10);
            _model.Lap();
            Thread.Sleep(10);
            _model.Lap();
            _model.Reset();
            CheckStopped(_model);
            Assert.IsFalse(_model.Laps.Any(), "Laps were not cleared after reset");
        }

        private void CheckRunning(StopwatchModel model)
        {
            Assert.IsTrue(model.IsRunning, "Stopwatch isn't running");
            Assert.IsFalse(model.IsStopped, "Stopwatch is stopped");
            Assert.IsFalse(model.IsPaused, "Stopwatch is paused");
        }

        private void CheckStopped(StopwatchModel model)
        {
            Assert.IsFalse(model.IsRunning, "Stopwatch is running");
            Assert.IsTrue(model.IsStopped, "Stopwatch isn't stopped");
            Assert.IsFalse(model.IsPaused, "Stopwatch is paused");
        }

        private void CheckPaused(StopwatchModel model)
        {
            Assert.IsFalse(model.IsRunning, "Stopwatch is running");
            Assert.IsFalse(model.IsStopped, "Stopwatch is stopped");
            Assert.IsTrue(model.IsPaused, "Stopwatch isn't paused");
        }
    }
}