using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using App.Domain.Core.Enums;
using App.Domain.Core.Helpers;
using App.Domain.Core.Models;
using App.Domain.Core.Models.Rule;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.Domain.Core.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class FilterTests
    {
        public enum FilterTestEnum
        {
            EnumOne = 0,
            EnumTwo = 1
        }

        private class TestEntity : EntityBase
        {
            public int IdInt { get; set; }
            public string IdString { get; set; }
            public FilterTestEnum EnumValue { get; set; }
            public FilterTestEnum? MaybeEnumValue { get; set; }
            public string Name { get; set; }
            public long? Group { get; set; }
            public DateTime Sometime { get; set; }
            public DateTime? MaybeSometime { get; set; }
            public TestEntity Child { get; set; }
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterStringsTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = "4", Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "1", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // case insensitive string property
            // test constructor without params
            var filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.Name, "ANNA", filterMatch: FilterMatch.Like)).ToList();
            Assert.AreEqual(3, filtered.Count);

            // test constructor with params
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdString, "2")).ToList();
            Assert.AreEqual(1, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.Child.IdString, "2",
                    filterMatch: FilterMatch.Like)).ToList();
            // we have 2 and 12 matching           
            Assert.AreEqual(2, filtered.Count);

            // filter chain with default AND
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.Child.IdString, "2")
                                                       .And(e => e.Name, "manna",
                    filterMatch: FilterMatch.Like)).ToList();
            // we have one 2 AND Manna
            Assert.AreEqual(1, filtered.Count);

            // filter chain with OR
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.Child.IdString, "2",
                            filterMatch: FilterMatch.Like)
                    .Or(e => e.Name, "anna",
                            filterMatch: FilterMatch.Like)).ToList();

            // we have four anna + two 2
            Assert.AreEqual(4, filtered.Count);

            // filter with OR and exact
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.Child.IdString, "2").Or(e => e.Name, "anna",
                                                        FilterMatch.Equal, isCaseSensitive: true))
                                                       .ToList();

            // we have zero 'anna' + one '2'
            Assert.AreEqual(1, filtered.Count);

            // test with Not
            filtered = source.AsQueryable()
                .FilterByRules(
                new EntityFilter<TestEntity>(e => e.Child.IdString, "2",
                                filterMatch: FilterMatch.NotEqual)
                                                       ).ToList();

            Assert.AreEqual(3, filtered.Count);
        }


        private class ValHolder
        {
            public int Val { get; set; }
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterNumeralsTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = "4", Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "1", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // recommended approach - try with direct operators
            var filtered = source.AsQueryable()
                 .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt == 3).And(e => e.IdInt == 1)).ToList();
            Assert.AreEqual(0, filtered.Count);

            filtered = source.AsQueryable()
                 .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt > 1).And(e => e.IdInt < 3)).ToList();
            Assert.AreEqual(1, filtered.Count);

            filtered = source.AsQueryable()
                 .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt >= 1).And(e => e.IdInt <= 3)).ToList();
            Assert.AreEqual(3, filtered.Count);

            filtered = source.AsQueryable()
                 .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt != 1)).ToList();
            Assert.AreEqual(3, filtered.Count);


            // previous in-depth tests - using direct access; not recommended but useful

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 4)).ToList();
            Assert.AreEqual(1, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.Greater)).ToList();
            Assert.AreEqual(2, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.GreaterOrEqual)).ToList();
            Assert.AreEqual(3, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.Less)).ToList();
            Assert.AreEqual(1, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.LessOrEqual)).ToList();
            Assert.AreEqual(2, filtered.Count);


            // combine
            // AND
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.Less).And(e => e.IdInt, 1, filterMatch: FilterMatch.Greater)).ToList();
            Assert.AreEqual(0, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 3, filterMatch: FilterMatch.Less).And(e => e.IdInt, 1, filterMatch: FilterMatch.Greater)).ToList();
            Assert.AreEqual(1, filtered.Count);

            // OR
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 3, filterMatch: FilterMatch.Less).Or(e => e.IdInt, 1, filterMatch: FilterMatch.Greater)).ToList();
            Assert.AreEqual(4, filtered.Count);

            // something illogical
            filtered = source.AsQueryable()
                 .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, 3).And(e => e.IdInt, 1)).ToList();
            Assert.AreEqual(0, filtered.Count);

            // can we solve arrays?
            ValHolder[] vals = { new ValHolder { Val = 1 }, new ValHolder { Val = 2 } };

            filtered = source.AsQueryable()
                     .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt == vals[0].Val)).ToList();
            Assert.AreEqual(1, filtered.Count);

            Assert.AreEqual(1, filtered.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterNullTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = null, Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "21", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }},     
                
                // special Name = null for coalesce needing test
                new TestEntity {IdInt = 1, IdString = null, Name = null, Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // null test
            var filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.IdString, null)).ToList();

            Assert.AreEqual(2, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdString == null)).ToList();

            Assert.AreEqual(2, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.IdString != null)).ToList();

            Assert.AreEqual(3, filtered.Count);

            // test for Coalesce needed case - without coalesce will throw NullRef
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.Name == "Hanna")).ToList();
            Assert.AreEqual(1, filtered.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterValidationTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = null, Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "21", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // bad filter start
            try
            {
                new EntityFilter<TestEntity>().Add(e => e.Child.IdInt, "2",
                    filterCombination: FilterCombination.And);

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // but with silent creation should pass
            var f = new EntityFilter<TestEntity>().Add(e => e.Child.IdInt, "2",
                filterCombination: FilterCombination.And, allowSilentCreation: true);

            // and a proper first rule should be created silently
            Assert.AreEqual(FilterCombination.None, ((FilterRule) f.Filters[0]).FilterCombination);

            // bad filter append
            try
            {
                new EntityFilter<TestEntity>().Add(e => e.Child.IdInt, "2").
                    Add(e => e.Child.IdInt, "2");

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // bad casing on non strings
            try
            {
                new EntityFilter<TestEntity>().Add(e => e.Child.IdInt, 22,
                    filterMatch: FilterMatch.Equal, isCaseSensitive: true);

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // bad operator on non-strings
            try
            {
                new EntityFilter<TestEntity>().Add(e => e.Child.IdInt, 22,
                    filterMatch: FilterMatch.Like);

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // bad data type
            try
            {
                new EntityFilter<TestEntity>().Add(e => e.Child.IdInt, new TestEntity());

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            try
            {
                new EntityFilter<TestEntity>().Add(e => e.Child.IdInt, "2",
                    filterCombination: FilterCombination.And);

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // negative tests, failed from expression builder
            // bad use of LIKE

            try
            {
                new EntityFilter<TestEntity>(e => e.Child.IdInt, 2,
                    filterMatch: FilterMatch.Like);

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // bad use of casing
            try
            {
                new EntityFilter<TestEntity>(e => e.Child.IdInt, 2,
                    filterMatch: FilterMatch.Equal, isCaseSensitive: true);

                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // bad op on strings
            try
            {
                new EntityFilter<TestEntity>(e => e.Name, "anna",
                                                FilterMatch.Greater);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }

            // bad op on null
            try
            {
                new EntityFilter<TestEntity>(e => e.Name, null,
                                                FilterMatch.Greater);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }


            // bad compare to null
            try
            {
                var filtered = source.AsQueryable()
                         .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt, null)).ToList();

                new EntityFilter<TestEntity>(e => e.IdInt, null);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // cannot compare long to null value
            }

        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterAccessorTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Anna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = "4", Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = "Vahunna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "1", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // case insensitive string property
            var ef = new EntityFilter<TestEntity>().And(e => e.Name, "ANNA");
            var filtered = source.AsQueryable().FilterByRules(ef).ToList();
            Assert.AreEqual(1, filtered.Count);
            // got exact match

            // do we have the filter? we might have a problem to find it if it's deep in some CompositeFilterRule
            var nameFilter = ef.Filters.FirstOrDefault(e => e is FilterRule rule && rule.Name == "Name" && rule.FilterMatch == FilterMatch.Equal);
            Assert.IsNotNull(nameFilter);

            // as it's stored by ref, we can try to modify it
            ((FilterRule) nameFilter).FilterMatch = FilterMatch.Like;
            // now will run as LIKE
            filtered = source.AsQueryable().FilterByRules(ef).ToList();
            Assert.AreEqual(3, filtered.Count);

            // now try to find if it's deep
            ef = new EntityFilter<TestEntity>().And(new EntityFilter<TestEntity>(e => e.Name, "ANNA"));
            filtered = source.AsQueryable().FilterByRules(ef).ToList();
            Assert.AreEqual(1, filtered.Count);
            // got exact match

            nameFilter = ef.Filters.FirstOrDefault(e => e is FilterRule rule && rule.Name == "Name" && rule.FilterMatch == FilterMatch.Equal);
            Assert.IsNull(nameFilter);

            // now the same with the new method


            // case insensitive string property
            var efNew = new EntityFilter<TestEntity>().And(e => e.Name, "ANNA");
            var filteredNew = source.AsQueryable().FilterByRules(efNew).ToList();
            Assert.AreEqual(1, filteredNew.Count);
            // got exact match

            // do we have the filter?
            var nameFilterNew = efNew.GetAllFlattenedFilterRules().FirstOrDefault(e => e.Name == "Name" && e.FilterMatch == FilterMatch.Equal);
            Assert.IsNotNull(nameFilterNew);

            // as it's stored by ref, we can try to modify it
            nameFilterNew.FilterMatch = FilterMatch.Like;
            // now will run as LIKE
            filteredNew = source.AsQueryable().FilterByRules(efNew).ToList();
            Assert.AreEqual(3, filteredNew.Count);

            // now try to find if it's deep
            efNew = new EntityFilter<TestEntity>().And(new EntityFilter<TestEntity>(e => e.Name, "ANNA"));
            filteredNew = source.AsQueryable().FilterByRules(efNew).ToList();
            Assert.AreEqual(1, filteredNew.Count);
            // got exact match

            nameFilterNew = efNew.GetAllFlattenedFilterRules().FirstOrDefault(e => e.Name == "Name" && e.FilterMatch == FilterMatch.Equal);
            Assert.IsNotNull(nameFilterNew);

            // as it's stored by ref, we can try to modify it
            nameFilterNew.FilterMatch = FilterMatch.Like;
            // now will run as LIKE
            filteredNew = source.AsQueryable().FilterByRules(efNew).ToList();
            Assert.AreEqual(3, filteredNew.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterGroupsTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = null, Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "21", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // group  Id > 2 && (Id == 3 || Id == 1)

            // if grouped wrong, && will take higher precedence and we'll have 2 items (ids 3 and 1)
            // if grouped right, || will take higher precedence and we'll have 1 item (id 3)

            // test without explicit group
            // Id == 1 || Id == 3 && Id > 2
            var filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.IdInt, 1)
                            .Or(e => e.IdInt, 3)
                            .And(e => e.IdInt, 2, filterMatch: FilterMatch.Greater)
                      ).ToList();
            // NOTICE:
            // PredicateBuilder and lambda helpers have regrouped the expression,
            // so it now looks like this: (Id == 1 || Id == 3) && Id > 2
            Assert.AreEqual(1, filtered.Count);

            // test without explicit group
            // Id > 2 && Id == 3 || Id == 1
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.Greater)
                            .And(e => e.IdInt, 3).Or(e => e.IdInt, 1)
                      ).ToList();

            // NOTICE:
            // PredicateBuilder and lambda helpers have regrouped the expression,
            // so it now looks like this: (Id > 2 && Id == 3) || Id == 1
            Assert.AreEqual(2, filtered.Count);

            // test with explicit group  Id > 2 && (Id == 3 || Id == 1)
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.Greater)
                            .And(new EntityFilter<TestEntity>(e => e.IdInt, 3).Or(e => e.IdInt, 1))
                      ).ToList();

            Assert.AreEqual(1, filtered.Count);

            // test with explicit group and additional expression
            // Id > 2 && (Id == 3 || Id == 1) || IdString == null
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.Greater)
                            .And(new EntityFilter<TestEntity>(e => e.IdInt, 3).Or(e => e.IdInt, 1))
                            .Or(e => e.IdString, null)
                      ).ToList();

            Assert.AreEqual(2, filtered.Count);

            // all in one string
            // Id > 2 && Id == 3 || Id == 1 || IdString == null
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.IdInt, 2, filterMatch: FilterMatch.Greater)
                            .And(e => e.IdInt, 3).Or(e => e.IdInt, 1)
                            .Or(e => e.IdString, null)
                      ).ToList();

            // PredicateBuilder and lambda helpers have regrouped the expression,
            // so it now looks like this:  ((Id > 2 && Id == 3) || Id == 1) || IdString == null
            Assert.AreEqual(3, filtered.Count);


            // shorthand method grouping
            // Id > 2 && (Id == 3 || Id == 1) || IdString == null
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.IdInt > 2)
                            .And(new EntityFilter<TestEntity>(e => e.IdInt == 3).Or(e => e.IdInt == 1))
                            .Or(e => e.IdString == null)
                      ).ToList();

            Assert.AreEqual(2, filtered.Count);

            // shorthand method grouping
            // Id > 2 && (Id == 3 || Id == 1)
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.IdInt > 2)
                            .And(new EntityFilter<TestEntity>(e => e.IdInt == 3).Or(e => e.IdInt == 1))
                      ).ToList();

            Assert.AreEqual(1, filtered.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterWithConvertTest()
        {
            TestEntity[] source = {
                new TestEntity {Id = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {Id = 4, IdString = null, Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {Id = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {Id = 1, IdString = "21", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // deep paths with direct expression
            var filteredx = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.Child.IdInt > 12)).ToList();

            Assert.AreEqual(1, filteredx.Count);


            // Id > 2 && Id == 3 || Id == 1 || IdString == null

            int constant2 = 2;
            var filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.Id, constant2, filterMatch: FilterMatch.Greater)
                            .And(e => e.Id, 3).Or(e => e.Id, 1)
                            .Or(e => e.IdString, null)
                      ).ToList();

            Assert.AreEqual(3, filtered.Count);

            int constant1 = 1;
            // test with converting both sides - member and value
            // using some crazy conversions - extreme Linq test
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.Id > constant2)
                            .And(e => e.Id == 3).Or(e => e.Id == constant1)
                            .Or(e => e.IdString, null)
                      ).ToList();

            Assert.AreEqual(3, filtered.Count);

            // deep paths with direct expression
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.Child.IdInt > 12)).ToList();

            Assert.AreEqual(1, filtered.Count);


        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterIdCompareTest()
        {
            TestEntity[] source = {
                new TestEntity {Id = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {Id = 0, IdString = null, Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {Id = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {Id = 1, IdString = "21", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            TestEntity tcompare = new TestEntity();
            tcompare.Id = 0;

            var filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.Id != tcompare.Id)).ToList();

            Assert.AreEqual(3, filtered.Count);

            tcompare.Id = 3;
            filtered = source.AsQueryable()
              .FilterByRules(
                      new EntityFilter<TestEntity>(e => e.Id != tcompare.Id)).ToList();

            Assert.AreEqual(3, filtered.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterIfNotNullOperationsTest()
        {
            var f = new EntityFilter<TestEntity>()
                .AndIfNotNull(e => e.Child.IdString, "2")
                .OrIfNotNull(e => e.Child.IdString, "")
                .AndIfNotNull(e => e.Child.IdString, null);

            // only two should be created
            Assert.AreEqual(2, f.Filters.Count);

            // and a proper first rule should be created silently
            Assert.AreEqual(FilterCombination.None, ((FilterRule) f.Filters[0]).FilterCombination);
            Assert.AreEqual(FilterCombination.Or, ((FilterRule) f.Filters[1]).FilterCombination);


            f = new EntityFilter<TestEntity>()
                .AndIfNotNull(e => e.Child.IdString == "2")
                .OrIfNotNull(e => e.Child.IdString == "")
                .AndIfNotNull(e => e.Child.IdString == null);

            // only two should be created
            Assert.AreEqual(2, f.Filters.Count);

            // and a proper first rule should be created silently
            Assert.AreEqual(FilterCombination.None, ((FilterRule) f.Filters[0]).FilterCombination);
            Assert.AreEqual(FilterCombination.Or, ((FilterRule) f.Filters[1]).FilterCombination);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterOnNullablesWithNullablesTest()
        {
            TestEntity[] source = {
                new TestEntity { MaybeSometime = null, IdInt = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity { MaybeSometime = new DateTime(2013, 1, 22), IdInt = 4, IdString = null, Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity { MaybeSometime = new DateTime(2013, 1, 12), IdInt = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity { MaybeSometime = null, IdInt = 1, IdString = "21", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            DateTime? start = new DateTime(2013, 1, 1);
            DateTime? end = new DateTime(2013, 2, 1);

            var f = new EntityFilter<TestEntity>()
                .AndIfNotNull(e => e.MaybeSometime >= start)
                .AndIfNotNull(e => e.MaybeSometime <= end);

            var filtered = source.AsQueryable()
                .FilterByRules(f).ToList();

            Assert.AreEqual(2, filtered.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterStringsStartsEndsTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnnah", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = "4", Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = null, Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "1", Name = "Mannah", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // case insensitive string property
            var filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.Name, "A",
                filterMatch: FilterMatch.StartsWith)).ToList();

            Assert.AreEqual(1, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.Name, "ah",
                filterMatch: FilterMatch.EndsWith)).ToList();

            Assert.AreEqual(2, filtered.Count);

            // case sensitive string property
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.Name, "a",
                filterMatch: FilterMatch.StartsWith, isCaseSensitive: true)).ToList();

            Assert.AreEqual(0, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.Name, "AH",
                filterMatch: FilterMatch.EndsWith, isCaseSensitive: true)).ToList();

            Assert.AreEqual(0, filtered.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterLikeCoalesceTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnnah", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = "4", Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = null, Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "1", Name = "Mannah", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // case sensitive string property
            var filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.Name, "anna",
                filterMatch: FilterMatch.Like, isCaseSensitive: true)).ToList();
            // add isCaseSensitive to force ToLower and cause smart coalescing

            Assert.AreEqual(2, filtered.Count);
        }

        private class SearchModel
        {
            public FilterTestEnum? MaybeEnum { get; set; }
        }


        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterEnumTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, EnumValue = FilterTestEnum.EnumOne, IdString = "2", Name = "Ahnnah", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, EnumValue = FilterTestEnum.EnumTwo, IdString = "4", Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, EnumValue = FilterTestEnum.EnumTwo, MaybeEnumValue = FilterTestEnum.EnumTwo, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, EnumValue = FilterTestEnum.EnumOne, IdString = "1", Name = "Mannah", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            var filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.EnumValue, FilterTestEnum.EnumTwo))
                .ToList();

            Assert.AreEqual(2, filtered.Count);

            // the following previously caused comparison error for int and enum,
            // neded additional tweaks in the filter LINQ parser
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.EnumValue == FilterTestEnum.EnumTwo))
                .ToList();

            Assert.AreEqual(2, filtered.Count);

            // the same with nullable
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>().Add(e => e.MaybeEnumValue, FilterTestEnum.EnumTwo))
                .ToList();

            Assert.AreEqual(1, filtered.Count);

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.MaybeEnumValue == FilterTestEnum.EnumTwo))
                .ToList();

            Assert.AreEqual(1, filtered.Count);

            // with dynamic filter
            SearchModel sm = new SearchModel();
            sm.MaybeEnum = null;

            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.MaybeEnumValue == sm.MaybeEnum))
                .ToList();

            Assert.AreEqual(3, filtered.Count);

            sm.MaybeEnum = FilterTestEnum.EnumTwo;
            filtered = source.AsQueryable()
                .FilterByRules(new EntityFilter<TestEntity>(e => e.MaybeEnumValue == sm.MaybeEnum))
                .ToList();

            Assert.AreEqual(1, filtered.Count);
        }

        [TestCategory("App Domain Core Layer"), TestMethod]
        public void EntityFilterArrayAccessTest()
        {
            TestEntity[] source = {
                new TestEntity {IdInt = 2, IdString = "2", Name = "Ahnna", Child = new TestEntity { IdString = "12", IdInt = 12 }},
                new TestEntity {IdInt = 4, IdString = "4", Name = "Hanna", Child = new TestEntity { IdString = "3", IdInt = 3 }},
                new TestEntity {IdInt = 3, IdString = "3", Name = "Vanna", Child = new TestEntity { IdString = "15", IdInt = 15 }},
                new TestEntity {IdInt = 1, IdString = "1", Name = "Manna", Child = new TestEntity { IdString = "2", IdInt = 2 }}
            };

            // can we solve arrays?
            int[] valsArray = { 1, 2 };
            ValHolder[] vals = { new ValHolder { Val = 1 }, new ValHolder { Val = 2 } };

            var filtered = source.AsQueryable()
                     .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt == vals[0].Val)).ToList();
            Assert.AreEqual(1, filtered.Count);

            filtered = source.AsQueryable()
                     .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt == valsArray[0])).ToList();
            Assert.AreEqual(1, filtered.Count);

            // tricky case
            filtered = source.AsQueryable()
                     .FilterByRules(new EntityFilter<TestEntity>(e => e.IdInt == valsArray[vals[0].Val])).ToList();
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual(2, filtered[0].IdInt);
        }
    }
}