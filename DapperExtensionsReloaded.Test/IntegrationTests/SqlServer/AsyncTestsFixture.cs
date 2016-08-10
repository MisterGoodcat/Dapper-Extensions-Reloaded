﻿using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Test.Data;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    [TestFixture]
    public class AsyncTestsFixture : SqlServerBaseFixture
    {
        [TestFixture]
        public class InsertMethod : SqlServerBaseFixture
        {
            [Test]
            public void AddsEntityToDatabase_ReturnsKey()
            {
                var p = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                int id = Connection.Insert(p);
                Assert.AreEqual(1, id);
                Assert.AreEqual(1, p.Id);
            }

            [Test]
            public void AddsEntityToDatabase_ReturnsCompositeKey()
            {
                var m = new Multikey { Key2 = "key", Value = "foo" };
                var key = Connection.Insert(m);
                Assert.AreEqual(1, key.Key1);
                Assert.AreEqual("key", key.Key2);
            }

            [Test]
            public void AddsEntityToDatabase_ReturnsGeneratedPrimaryKey()
            {
                var a1 = new Animal { Name = "Foo" };
                Connection.Insert(a1);

                var a2 = Connection.GetAsync<Animal>(a1.Id).GetAwaiter().GetResult();
                Assert.AreNotEqual(Guid.Empty, a2.Id);
                Assert.AreEqual(a1.Id, a2.Id);
            }

            [Test]
            public void AddsMultipleEntitiesToDatabase()
            {
                var a1 = new Animal { Name = "Foo" };
                var a2 = new Animal { Name = "Bar" };
                var a3 = new Animal { Name = "Baz" };

                Connection.Insert<Animal>(new[] { a1, a2, a3 });

                var animals = Connection.GetListAsync<Animal>().GetAwaiter().GetResult().ToList();
                Assert.AreEqual(3, animals.Count);
            }
        }

        [TestFixture]
        public class GetMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingKey_ReturnsEntity()
            {
                var p1 = new FourLeggedFurryAnimal
                {
                    Active = true,
                    HowItsCalled = "Foo",
                    DateCreated = DateTime.UtcNow
                };
                int id = Connection.Insert(p1);

                var p2 = Connection.GetAsync<FourLeggedFurryAnimal>(id).GetAwaiter().GetResult();
                Assert.AreEqual(id, p2.Id);
                Assert.AreEqual("Foo", p2.HowItsCalled);
            }

            [Test]
            public void UsingCompositeKey_ReturnsEntity()
            {
                var m1 = new Multikey { Key2 = "key", Value = "bar" };
                var key = Connection.Insert(m1);

                var m2 = Connection.GetAsync<Multikey>(new { key.Key1, key.Key2 }).GetAwaiter().GetResult();
                Assert.AreEqual(1, m2.Key1);
                Assert.AreEqual("key", m2.Key2);
                Assert.AreEqual("bar", m2.Value);
            }
        }

        [TestFixture]
        public class DeleteMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingKey_DeletesFromDatabase()
            {
                var p1 = new FourLeggedFurryAnimal
                {
                    Active = true,
                    HowItsCalled = "Foo",
                    DateCreated = DateTime.UtcNow
                };
                int id = Connection.Insert(p1);

                var p2 = Connection.GetAsync<FourLeggedFurryAnimal>(id).GetAwaiter().GetResult();
                Connection.Delete(p2);
                Assert.IsNull(Connection.GetAsync<FourLeggedFurryAnimal>(id).GetAwaiter().GetResult());
            }

            [Test]
            public void UsingCompositeKey_DeletesFromDatabase()
            {
                var m1 = new Multikey { Key2 = "key", Value = "bar" };
                var key = Connection.Insert(m1);

                var m2 = Connection.GetAsync<Multikey>(new { key.Key1, key.Key2 }).GetAwaiter().GetResult();
                Connection.Delete(m2);
                Assert.IsNull(Connection.GetAsync<Multikey>(new { key.Key1, key.Key2 }).GetAwaiter().GetResult());
            }

            [Test]
            public void UsingPredicate_DeletesRows()
            {
                var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
                Connection.Insert(p1);
                Connection.Insert(p2);
                Connection.Insert(p3);

                var list = Connection.GetListAsync<FourLeggedFurryAnimal>().GetAwaiter().GetResult();
                Assert.AreEqual(3, list.Count());

                IPredicate pred = Predicates.Field<FourLeggedFurryAnimal>(p => p.HowItsCalled, Operator.Eq, "Foo2");
                var result = Connection.Delete<FourLeggedFurryAnimal>(pred);
                Assert.IsTrue(result);

                list = Connection.GetListAsync<FourLeggedFurryAnimal > ().GetAwaiter().GetResult();
                Assert.AreEqual(2, list.Count());
            }

            [Test]
            public void UsingObject_DeletesRows()
            {
                var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
                Connection.Insert(p1);
                Connection.Insert(p2);
                Connection.Insert(p3);

                var list = Connection.GetListAsync<FourLeggedFurryAnimal>().GetAwaiter().GetResult();
                Assert.AreEqual(3, list.Count());

                var result = Connection.Delete<FourLeggedFurryAnimal>(new { HowItsCalled = "Foo2" });
                Assert.IsTrue(result);

                list = Connection.GetListAsync<FourLeggedFurryAnimal>().GetAwaiter().GetResult();
                Assert.AreEqual(2, list.Count());
            }
        }

        [TestFixture]
        public class UpdateMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingKey_UpdatesEntity()
            {
                var p1 = new FourLeggedFurryAnimal
                {
                    Active = true,
                    HowItsCalled = "Foo",
                    DateCreated = DateTime.UtcNow
                };
                int id = Connection.Insert(p1);

                var p2 = Connection.GetAsync<FourLeggedFurryAnimal>(id).GetAwaiter().GetResult();
                p2.HowItsCalled = "Baz";
                p2.Active = false;

                Connection.Update(p2);

                var p3 = Connection.GetAsync<FourLeggedFurryAnimal>(id).GetAwaiter().GetResult();
                Assert.AreEqual("Baz", p3.HowItsCalled);
                Assert.AreEqual(false, p3.Active);
            }

            [Test]
            public void UsingCompositeKey_UpdatesEntity()
            {
                var m1 = new Multikey { Key2 = "key", Value = "bar" };
                var key = Connection.Insert(m1);

                var m2 = Connection.GetAsync<Multikey>(new { key.Key1, key.Key2 }).GetAwaiter().GetResult();
                m2.Key2 = "key";
                m2.Value = "barz";
                Connection.Update(m2);

                var m3 = Connection.GetAsync<Multikey>(new { Key1 = 1, Key2 = "key" }).GetAwaiter().GetResult();
                Assert.AreEqual(1, m3.Key1);
                Assert.AreEqual("key", m3.Key2);
                Assert.AreEqual("barz", m3.Value);
            }
        }

        [TestFixture]
        public class GetListMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsAll()
            {
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

                var list = Connection.GetListAsync<FourLeggedFurryAnimal>().GetAwaiter().GetResult();
                Assert.AreEqual(4, list.Count());
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

                var predicate = Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
                var list = Connection.GetListAsync<FourLeggedFurryAnimal>(predicate, null).GetAwaiter().GetResult();
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.HowItsCalled == "a" || p.HowItsCalled == "c"));
            }

            [Test]
            public void UsingObject_ReturnsMatching()
            {
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

                var predicate = new { Active = true, HowItsCalled = "c" };
                var list = Connection.GetListAsync<FourLeggedFurryAnimal>(predicate, null).GetAwaiter().GetResult();
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.HowItsCalled == "c"));
            }
        }

        [TestFixture]
        public class GetPageMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingDefaultPageValues_ReturnsFirstPage()
            {
                var id1 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "01", DateCreated = DateTime.UtcNow });
                var id2 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "02", DateCreated = DateTime.UtcNow });
                var id3 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "03", DateCreated = DateTime.UtcNow });
                var id4 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "04", DateCreated = DateTime.UtcNow });
                var id5 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "05", DateCreated = DateTime.UtcNow });
                var id6 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "06", DateCreated = DateTime.UtcNow });
                var id7 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "07", DateCreated = DateTime.UtcNow });
                var id8 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "08", DateCreated = DateTime.UtcNow });
                var id9 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "09", DateCreated = DateTime.UtcNow });
                var id10 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "10", DateCreated = DateTime.UtcNow });
                var id11 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "11", DateCreated = DateTime.UtcNow });

                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Connection.GetPageAsync<FourLeggedFurryAnimal>(null, sort).GetAwaiter().GetResult();
                Assert.AreEqual(10, list.Count());
                Assert.AreEqual(id1, list.First().Id);
                Assert.AreEqual(id10, list.Last().Id);
            }

            [Test]
            public void UsingNullPredicate_ReturnsMatching()
            {
                var id1 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Connection.GetPageAsync<FourLeggedFurryAnimal>(null, sort, 0, 2).GetAwaiter().GetResult();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(id2, list.First().Id);
                Assert.AreEqual(id4, list.Skip(1).First().Id);
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                var id1 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                var predicate = Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Connection.GetPageAsync<FourLeggedFurryAnimal>(predicate, sort, 0, 3).GetAwaiter().GetResult();
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.HowItsCalled == "Sigma" || p.HowItsCalled == "Theta"));
            }

            [Test]
            public void NotFirstPage_Returns_NextResults()
            {
                var id1 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Connection.GetPageAsync<FourLeggedFurryAnimal>(null, sort, 1, 2).GetAwaiter().GetResult();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(id1, list.First().Id);
                Assert.AreEqual(id3, list.Skip(1).First().Id);
            }

            [Test]
            public void UsingObject_ReturnsMatching()
            {
                var id1 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                var predicate = new { Active = true };
                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Connection.GetPageAsync<FourLeggedFurryAnimal>(predicate, sort, 0, 3).GetAwaiter().GetResult();
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.HowItsCalled == "Sigma" || p.HowItsCalled == "Theta"));
            }
        }

        [TestFixture]
        public class CountMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNullPredicate_Returns_Count()
            {
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

                var count = Connection.Count<FourLeggedFurryAnimal>(null);
                Assert.AreEqual(4, count);
            }

            [Test]
            public void UsingPredicate_Returns_Count()
            {
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

                var predicate = Predicates.Field<FourLeggedFurryAnimal>(f => f.DateCreated, Operator.Lt, DateTime.UtcNow.AddDays(-5));
                var count = Connection.Count<FourLeggedFurryAnimal>(predicate);
                Assert.AreEqual(2, count);
            }

            [Test]
            public void UsingObject_Returns_Count()
            {
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
                Connection.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

                var predicate = new { HowItsCalled = new[] { "b", "d" } };
                var count = Connection.Count<FourLeggedFurryAnimal>(predicate);
                Assert.AreEqual(2, count);
            }
        }
    }
}