using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Test.Data;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    [TestFixture]
    public class AttributeMapperFixture : SqlServerBaseFixture
    {
        [TestFixture]
        public class InsertMethod : SqlServerBaseFixture
        {
            [Test]
            public void AddsEntityToDatabase_ReturnsKey()
            {
                var p = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                int id = Db.Insert(p);
                Assert.AreEqual(1, id);
                Assert.AreEqual(1, p.Id);
            }

            [Test]
            public void AddsEntityToDatabase_ReturnsCompositeKey()
            {
                var m = new Multikey { Key2 = "key", Value = "foo" };
                var key = Db.Insert(m);
                Assert.AreEqual(1, key.Key1);
                Assert.AreEqual("key", key.Key2);
            }

            [Test]
            public void AddsEntityToDatabase_ReturnsGeneratedPrimaryKey()
            {
                var a1 = new Animal { Name = "Foo" };
                Db.Insert(a1);

                var a2 = Db.Get<Animal>(a1.Id);
                Assert.AreNotEqual(Guid.Empty, a2.Id);
                Assert.AreEqual(a1.Id, a2.Id);
            }

            [Test]
            public void AddsMultipleEntitiesToDatabase()
            {
                var a1 = new Animal { Name = "Foo" };
                var a2 = new Animal { Name = "Bar" };
                var a3 = new Animal { Name = "Baz" };

                Db.Insert<Animal>(new[] { a1, a2, a3 });

                var animals = Db.GetList<Animal>().ToList();
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
                int id = Db.Insert(p1);

                var p2 = Db.Get<FourLeggedFurryAnimal>(id);
                Assert.AreEqual(id, p2.Id);
                Assert.AreEqual("Foo", p2.HowItsCalled);
            }

            [Test]
            public void UsingCompositeKey_ReturnsEntity()
            {
                var m1 = new Multikey { Key2 = "key", Value = "bar" };
                var key = Db.Insert(m1);

                var m2 = Db.Get<Multikey>(new { key.Key1, key.Key2 });
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
                int id = Db.Insert(p1);

                var p2 = Db.Get<FourLeggedFurryAnimal>(id);
                Db.Delete(p2);
                Assert.IsNull(Db.Get<FourLeggedFurryAnimal>(id));
            }

            [Test]
            public void UsingCompositeKey_DeletesFromDatabase()
            {
                var m1 = new Multikey { Key2 = "key", Value = "bar" };
                var key = Db.Insert(m1);

                var m2 = Db.Get<Multikey>(new { key.Key1, key.Key2 });
                Db.Delete(m2);
                Assert.IsNull(Db.Get<Multikey>(new { key.Key1, key.Key2 }));
            }

            [Test]
            public void UsingPredicate_DeletesRows()
            {
                var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
                Db.Insert(p1);
                Db.Insert(p2);
                Db.Insert(p3);

                var list = Db.GetList<FourLeggedFurryAnimal>();
                Assert.AreEqual(3, list.Count());

                IPredicate pred = Predicates.Field<FourLeggedFurryAnimal>(p => p.HowItsCalled, Operator.Eq, "Foo2");
                var result = Db.Delete<FourLeggedFurryAnimal>(pred);
                Assert.IsTrue(result);

                list = Db.GetList<FourLeggedFurryAnimal>();
                Assert.AreEqual(2, list.Count());
            }

            [Test]
            public void UsingObject_DeletesRows()
            {
                var p1 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p2 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo", DateCreated = DateTime.UtcNow };
                var p3 = new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Foo2", DateCreated = DateTime.UtcNow };
                Db.Insert(p1);
                Db.Insert(p2);
                Db.Insert(p3);

                var list = Db.GetList<FourLeggedFurryAnimal>();
                Assert.AreEqual(3, list.Count());

                var result = Db.Delete<FourLeggedFurryAnimal>(new { HowItsCalled = "Foo2" });
                Assert.IsTrue(result);

                list = Db.GetList<FourLeggedFurryAnimal>();
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
                int id = Db.Insert(p1);

                var p2 = Db.Get<FourLeggedFurryAnimal>(id);
                p2.HowItsCalled = "Baz";
                p2.Active = false;

                Db.Update(p2);

                var p3 = Db.Get<FourLeggedFurryAnimal>(id);
                Assert.AreEqual("Baz", p3.HowItsCalled);
                Assert.AreEqual(false, p3.Active);
            }

            [Test]
            public void UsingCompositeKey_UpdatesEntity()
            {
                var m1 = new Multikey { Key2 = "key", Value = "bar" };
                var key = Db.Insert(m1);

                var m2 = Db.Get<Multikey>(new { key.Key1, key.Key2 });
                m2.Key2 = "key";
                m2.Value = "barz";
                Db.Update(m2);

                var m3 = Db.Get<Multikey>(new { Key1 = 1, Key2 = "key" });
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
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

                var list = Db.GetList<FourLeggedFurryAnimal>();
                Assert.AreEqual(4, list.Count());
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

                var predicate = Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
                var list = Db.GetList<FourLeggedFurryAnimal>(predicate, null);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.HowItsCalled == "a" || p.HowItsCalled == "c"));
            }

            [Test]
            public void UsingObject_ReturnsMatching()
            {
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow });

                var predicate = new { Active = true, HowItsCalled = "c" };
                var list = Db.GetList<FourLeggedFurryAnimal>(predicate, null);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.HowItsCalled == "c"));
            }
        }

        [TestFixture]
        public class GetPageMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsMatching()
            {
                var id1 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Db.GetPage<FourLeggedFurryAnimal>(null, sort, 0, 2);
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(id2, list.First().Id);
                Assert.AreEqual(id4, list.Skip(1).First().Id);
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                var id1 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                var predicate = Predicates.Field<FourLeggedFurryAnimal>(f => f.Active, Operator.Eq, true);
                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Db.GetPage<FourLeggedFurryAnimal>(predicate, sort, 0, 3);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.HowItsCalled == "Sigma" || p.HowItsCalled == "Theta"));
            }

            [Test]
            public void NotFirstPage_Returns_NextResults()
            {
                var id1 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Db.GetPage<FourLeggedFurryAnimal>(null, sort, 1, 2);
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(id1, list.First().Id);
                Assert.AreEqual(id3, list.Skip(1).First().Id);
            }

            [Test]
            public void UsingObject_ReturnsMatching()
            {
                var id1 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Sigma",  DateCreated = DateTime.UtcNow });
                var id2 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Delta", DateCreated = DateTime.UtcNow });
                var id3 = Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "Theta",  DateCreated = DateTime.UtcNow });
                var id4 = Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "Iota", DateCreated = DateTime.UtcNow });

                var predicate = new { Active = true };
                IList<ISort> sort = new List<ISort>
                {
                    Predicates.Sort<FourLeggedFurryAnimal>(p => p.HowItsCalled)
                };

                var list = Db.GetPage<FourLeggedFurryAnimal>(predicate, sort, 0, 3);
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
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

                var count = Db.Count<FourLeggedFurryAnimal>(null);
                Assert.AreEqual(4, count);
            }

            [Test]
            public void UsingPredicate_Returns_Count()
            {
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

                var predicate = Predicates.Field<FourLeggedFurryAnimal>(f => f.DateCreated, Operator.Lt, DateTime.UtcNow.AddDays(-5));
                var count = Db.Count<FourLeggedFurryAnimal>(predicate);
                Assert.AreEqual(2, count);
            }

            [Test]
            public void UsingObject_Returns_Count()
            {
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

                var predicate = new { HowItsCalled = new[] { "b", "d" } };
                var count = Db.Count<FourLeggedFurryAnimal>(predicate);
                Assert.AreEqual(2, count);
            }
        }

        [TestFixture]
        public class GetMultipleMethod : SqlServerBaseFixture
        {
            [Test]
            public void ReturnsItems()
            {
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "a", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "b", DateCreated = DateTime.UtcNow.AddDays(-10) });
                Db.Insert(new FourLeggedFurryAnimal { Active = true, HowItsCalled = "c", DateCreated = DateTime.UtcNow.AddDays(-3) });
                Db.Insert(new FourLeggedFurryAnimal { Active = false, HowItsCalled = "d", DateCreated = DateTime.UtcNow.AddDays(-1) });

                Db.Insert(new Animal { Name = "Foo" });
                Db.Insert(new Animal { Name = "Bar" });
                Db.Insert(new Animal { Name = "Baz" });

                var predicate = new GetMultiplePredicate();
                predicate.Add<FourLeggedFurryAnimal>(null);
                predicate.Add<Animal>(Predicates.Field<Animal>(a => a.Name, Operator.Like, "Ba%"));
                predicate.Add<FourLeggedFurryAnimal>(Predicates.Field<FourLeggedFurryAnimal>(a => a.HowItsCalled, Operator.Eq, "c"));

                var result = Db.GetMultiple(predicate);
                var cats = result.Read<FourLeggedFurryAnimal>().ToList();
                var animals = result.Read<Animal>().ToList();
                var cats2 = result.Read<FourLeggedFurryAnimal>().ToList();

                Assert.AreEqual(4, cats.Count);
                Assert.AreEqual(2, animals.Count);
                Assert.AreEqual(1, cats2.Count);
            }
        }
    }
}
