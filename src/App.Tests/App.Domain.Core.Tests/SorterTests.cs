using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using App.Domain.Core.Helpers;
using App.Domain.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.Domain.Core.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class SorterTests
    {
        private class TestEntity
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public long? Group { get; set; }
            public DateTime Sometime { get; set; }
            public TestEntity Child { get; set; }
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntitySortTest()
        {
            DateTime now = DateTime.Now;

            TestEntity[] source = {
                new TestEntity {Id = 2, Sometime = now.AddDays(-3), Name = "Anna", Child = new TestEntity { Id = 12 }, Group = 1},
                new TestEntity {Id = 4, Sometime = now.AddDays(-2), Name = "Hanna", Child = new TestEntity { Id = 3 }, Group = 2},
                new TestEntity {Id = 3, Sometime = now.AddDays(1), Name = "Vanna", Child = new TestEntity { Id = 15 }, Group = 2},
                new TestEntity {Id = 1, Sometime = now.AddDays(2), Name = "Manna", Child = new TestEntity { Id = 2 }, Group = 1}
            };

            // string property
            var sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Name, false)).ToList();
            Assert.AreEqual("Vanna", sorted[0].Name);
            Assert.AreEqual("Anna", sorted[3].Name);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Name)).ToList();
            Assert.AreEqual("Anna", sorted[0].Name);
            Assert.AreEqual("Vanna", sorted[3].Name);

            // long property
            sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Id, false)).ToList();
            Assert.AreEqual(4, sorted[0].Id);
            Assert.AreEqual(1, sorted[3].Id);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Id)).ToList();
            Assert.AreEqual(1, sorted[0].Id);
            Assert.AreEqual(4, sorted[3].Id);

            // date time      
            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Sometime, false)).ToList();
            Assert.AreEqual(1, sorted[0].Id);
            Assert.AreEqual(2, sorted[3].Id);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Sometime)).ToList();
            Assert.AreEqual(2, sorted[0].Id);
            Assert.AreEqual(1, sorted[3].Id);

            // inner property
            sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Child.Id, false)).ToList();
            Assert.AreEqual(15, sorted[0].Child.Id);
            Assert.AreEqual(2, sorted[3].Child.Id);

            sorted = source.AsQueryable()
                            .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Child.Id)).ToList();
            Assert.AreEqual(2, sorted[0].Child.Id);
            Assert.AreEqual(15, sorted[3].Child.Id);

            // test thenby
            sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Group, false).Add(e => e.Name, false)).ToList();
            Assert.AreEqual(3, sorted[0].Id);
            Assert.AreEqual(4, sorted[1].Id);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add(e => e.Group).Add(e => e.Name)).ToList();
            Assert.AreEqual(2, sorted[0].Id);
            Assert.AreEqual(1, sorted[1].Id);

            // test by strings
            sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add("Group", false).Add("Name", false)).ToList();
            Assert.AreEqual(3, sorted[0].Id);
            Assert.AreEqual(4, sorted[1].Id);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add("Group").Add("Name")).ToList();
            Assert.AreEqual(2, sorted[0].Id);
            Assert.AreEqual(1, sorted[1].Id);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntitySortStringsTest()
        {
            DateTime now = DateTime.Now;

            TestEntity[] source = {
                new TestEntity {Id = 2, Sometime = now.AddDays(-3), Name = "Anna", Child = new TestEntity { Id = 12 }, Group = 1},
                new TestEntity {Id = 4, Sometime = now.AddDays(-2), Name = "Hanna", Child = new TestEntity { Id = 3 }, Group = 2},
                new TestEntity {Id = 3, Sometime = now.AddDays(1), Name = "Vanna", Child = new TestEntity { Id = 15 }, Group = 2},
                new TestEntity {Id = 1, Sometime = now.AddDays(2), Name = "Manna", Child = new TestEntity { Id = 2 }, Group = 1}
            };

            // string property
            var sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add("Name", false)).ToList();
            Assert.AreEqual("Vanna", sorted[0].Name);
            Assert.AreEqual("Anna", sorted[3].Name);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add("Name")).ToList();
            Assert.AreEqual("Anna", sorted[0].Name);
            Assert.AreEqual("Vanna", sorted[3].Name);

            // long property
            sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add("Id", false)).ToList();
            Assert.AreEqual(4, sorted[0].Id);
            Assert.AreEqual(1, sorted[3].Id);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add("Id")).ToList();
            Assert.AreEqual(1, sorted[0].Id);
            Assert.AreEqual(4, sorted[3].Id);

            // date time      
            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add("Sometime", false)).ToList();
            Assert.AreEqual(1, sorted[0].Id);
            Assert.AreEqual(2, sorted[3].Id);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add("Sometime")).ToList();
            Assert.AreEqual(2, sorted[0].Id);
            Assert.AreEqual(1, sorted[3].Id);

            // inner property
            sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add("Child.Id", false)).ToList();
            Assert.AreEqual(15, sorted[0].Child.Id);
            Assert.AreEqual(2, sorted[3].Child.Id);

            sorted = source.AsQueryable()
                            .OrderByRules(new EntitySort<TestEntity>().Add("Child.Id")).ToList();
            Assert.AreEqual(2, sorted[0].Child.Id);
            Assert.AreEqual(15, sorted[3].Child.Id);

            // test thenby
            sorted = source.AsQueryable()
                .OrderByRules(new EntitySort<TestEntity>().Add("Group", false).Add("Name", false)).ToList();
            Assert.AreEqual(3, sorted[0].Id);
            Assert.AreEqual(4, sorted[1].Id);

            sorted = source.AsQueryable()
                 .OrderByRules(new EntitySort<TestEntity>().Add("Group").Add("Name")).ToList();
            Assert.AreEqual(2, sorted[0].Id);
            Assert.AreEqual(1, sorted[1].Id);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntitySortExtractRulesTest()
        {
            var sorter = new EntitySort<TestEntity>(e => e.Name);

            var rules = sorter.GetAllSortRules();
            Assert.IsNotNull(rules.FirstOrDefault(r => r.Name == "Name" && r.IsAscending));

            sorter.Add(e => e.Child.Id, false);
            rules = sorter.GetAllSortRules();
            Assert.IsNotNull(rules.FirstOrDefault(r => r.Name == "Name" && r.IsAscending));
            Assert.IsNotNull(rules.FirstOrDefault(r => r.Name == "Child.Id" && r.IsAscending == false));

            // as it's stored by ref, we can try to modify it
            rules.FirstOrDefault(r => r.Name == "Name" && r.IsAscending).IsAscending = false;
            Assert.IsNotNull(sorter.GetAllSortRules().FirstOrDefault(r => r.Name == "Name" && r.IsAscending == false));
        }
    }
}

