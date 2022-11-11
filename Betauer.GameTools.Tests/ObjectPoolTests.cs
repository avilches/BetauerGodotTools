using Betauer.Application;
using Betauer.Core.Memory;
using Betauer.Pool;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests {
    public class R : Recyclable {
        public static int Created = 0;
        public string Name;

        public R() {
            Created++;
        }
    }

    public class GR : GodotObjectRecyclable {
        public static int Created = 0;

        public GR() {
            Created++;
        }
    }

    [TestFixture]
    public class DefaultObjectPoolTests {
        [Test(Description = "DefaultObjectPool")]
        public void DefaultObjectPoolTest() {
            R.Created = 0;
            GR.Created = 0;

            var objectPool = new ObjectPool();
            DefaultObjectPool.SetPool(objectPool);
            Assert.That(DefaultObjectPool.Registry, Is.EqualTo(objectPool));
            Assert.That(DefaultObjectPool.Registry.Pools.Count, Is.EqualTo(0));

            DefaultObjectPool.Enabled = true;
            R r = DefaultObjectPool.Get<R>();
            GR gr = DefaultObjectPool.Get<GR>();
            Assert.That(R.Created, Is.EqualTo(1));
            Assert.That(GR.Created, Is.EqualTo(1));
            Assert.That(DefaultObjectPool.Registry.Pools.Count, Is.EqualTo(2));
        }

        [Test(Description = "DefaultObjectPool disabled")]
        public void DefaultObjectPoolDisabledTest() {
            R.Created = 0;
            GR.Created = 0;

            var objectPool = new ObjectPool();
            DefaultObjectPool.SetPool(objectPool);
            Assert.That(DefaultObjectPool.Registry, Is.EqualTo(objectPool));
            Assert.That(DefaultObjectPool.Registry.Pools.Count, Is.EqualTo(0));

            DefaultObjectPool.Enabled = false;
            R r = DefaultObjectPool.Get<R>();
            GR gr = DefaultObjectPool.Get<GR>();
            Assert.That(R.Created, Is.EqualTo(1));
            Assert.That(GR.Created, Is.EqualTo(1));
            Assert.That(DefaultObjectPool.Registry.Pools.Count, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class ObjectPoolTests {
        [Test(Description = "Object pool factory")]
        public void Factory() {
            R.Created = 0;
            int created = 0;
            var name = "FACTORY";
            IObjectPool<R> objectPoolWithFactory = new ObjectPool<R>(() => {
                created++;
                var result = new R {
                    Name = name
                };
                return result;
            });
            IObjectPool<R> objectPool = new ObjectPool<R>();

            R r1f = objectPoolWithFactory.Get();
            R r2f = objectPoolWithFactory.Get();
            Assert.That(r1f.Name, Is.EqualTo(name));
            Assert.That(r2f.Name, Is.EqualTo(name));
            Assert.That(R.Created, Is.EqualTo(2));
            Assert.That(created, Is.EqualTo(2));

            R r1 = objectPool.Get();
            R r2 = objectPool.Get();
            Assert.That(r1.Name, Is.Null);
            Assert.That(r2.Name, Is.Null);
            Assert.That(R.Created, Is.EqualTo(4));
            Assert.That(created, Is.EqualTo(2));
        }

        [Test(Description = "Recyclable tokens are different between calls")]
        public void RecyclableTokens() {
            R.Created = 0;
            IObjectPool<R> objectPool = new ObjectPool<R>();

            R r1 = objectPool.Get();
            var token1 = r1.GetToken();
            Assert.That(token1, Is.Not.Null);
            Assert.That(R.Created, Is.EqualTo(1));
            r1.ReturnToPool();

            R r2 = objectPool.Get();
            string token2 = r2.GetToken();
            Assert.That(token2, Is.Not.Null);

            Assert.That(R.Created, Is.EqualTo(1));
            Assert.That(r1, Is.EqualTo(r2));
            Assert.That(token1, Is.Not.EqualTo(token2));
        }

        [Test(Description = "GodotObjectRecyclable tokens are different between calls")]
        public void GodotObjectRecyclableToken() {
            GR.Created = 0;
            IObjectPool<GR> objectPool = new ObjectPool<GR>();

            GR r1 = objectPool.Get();
            var token1 = r1.GetToken();
            Assert.That(token1, Is.Not.Null);
            Assert.That(GR.Created, Is.EqualTo(1));
            r1.ReturnToPool();

            GR r2 = objectPool.Get();
            var token2 = r2.GetToken();
            Assert.That(token2, Is.Not.Null);

            Assert.That(GR.Created, Is.EqualTo(1));
            Assert.That(r1, Is.EqualTo(r2));
            Assert.That(token1, Is.Not.EqualTo(token2));
        }

        [Test(Description = "Object Pool recycle objects with LIFO")]
        public void ObjectPoolLIFO() {
            GR.Created = 0;
            IObjectPool<GR> objectPool = new ObjectPool<GR>();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));

            GR r1 = objectPool.Get();
            GR r2 = objectPool.Get();
            GR r3 = objectPool.Get();
            Assert.That(GR.Created, Is.EqualTo(3));
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(3));

            r1.ReturnToPool();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(2));

            r3.ReturnToPool();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(2));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(1));

            GR r1r = objectPool.Get();
            Assert.That(r1, Is.EqualTo(r1r));
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(2));

            GR r3r = objectPool.Get();
            Assert.That(r3, Is.EqualTo(r3r));
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(3));

            GR r4 = objectPool.Get();
            Assert.That(GR.Created, Is.EqualTo(4));
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(4));
        }

        [Test(Description = "Duplicate return are ignored")]
        public void AllowDuplicateReturns() {
            GR.Created = 0;
            IObjectPool<GR> objectPool = new ObjectPool<GR>();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));

            GR r1 = objectPool.Get();
            Assert.That(GR.Created, Is.EqualTo(1));
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(1));

            r1.ReturnToPool();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));

            r1.ReturnToPool();
            r1.ReturnToPool();
            r1.ReturnToPool();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));
        }

        [Test(Description = "Return all")]
        public void ReturnAll() {
            GR.Created = 0;
            IObjectPool<GR> objectPool = new ObjectPool<GR>();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));

            GR r1 = objectPool.Get();
            GR r2 = objectPool.Get();
            GR r3 = objectPool.Get();
            Assert.That(GR.Created, Is.EqualTo(3));
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(3));

            objectPool.ReturnAll();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(3));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));
        }

        [Test(Description = "Never return a freed object")]
        public void ControlFreeObjects() {
            GR.Created = 0;
            IObjectPool<GR> objectPool = new ObjectPool<GR>();
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));

            GR r1 = objectPool.Get();
            Assert.That(GR.Created, Is.EqualTo(1));

            objectPool.ReturnAll();
            r1.Free();
            Assert.That(Object.IsInstanceValid(r1), Is.False);
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));

            GR r1r = objectPool.Get();
            Assert.That(GR.Created, Is.EqualTo(2));
            Assert.That(Object.IsInstanceValid(r1r), Is.True);
            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(1));
        }

        [Test(Description = "ObjectPool by type")]
        public void ObjectPool() {
            R.Created = 0;
            GR.Created = 0;

            ObjectPool objectPool = new ObjectPool();
            Assert.That(objectPool.Pools.Count, Is.EqualTo(0));

            R r = objectPool.Get<R>();
            GR gr = objectPool.Get<GR>();
            Assert.That(R.Created, Is.EqualTo(1));
            Assert.That(GR.Created, Is.EqualTo(1));

            Assert.That(objectPool.Pools.Count, Is.EqualTo(2));

            Assert.That(objectPool.Pool<R>().GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.Pool<R>().GetInUse().Count, Is.EqualTo(1));
            Assert.That(objectPool.Pool<GR>().GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.Pool<GR>().GetInUse().Count, Is.EqualTo(1));

            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(2));
            Assert.That(objectPool.GetInUse().Contains(r));
            Assert.That(objectPool.GetInUse().Contains(gr));

            // Smart return by type
            objectPool.Return(r);

            Assert.That(objectPool.Pool<R>().GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.Pool<R>().GetInUse().Count, Is.EqualTo(0));
            Assert.That(objectPool.Pool<GR>().GetAvailable().Count, Is.EqualTo(0));
            Assert.That(objectPool.Pool<GR>().GetInUse().Count, Is.EqualTo(1));

            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.GetAvailable().Contains(r));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(1));
            Assert.That(objectPool.GetInUse().Contains(gr));

            objectPool.Return(gr);

            Assert.That(objectPool.Pool<R>().GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.Pool<R>().GetInUse().Count, Is.EqualTo(0));
            Assert.That(objectPool.Pool<GR>().GetAvailable().Count, Is.EqualTo(1));
            Assert.That(objectPool.Pool<GR>().GetInUse().Count, Is.EqualTo(0));

            Assert.That(objectPool.GetAvailable().Count, Is.EqualTo(2));
            Assert.That(objectPool.GetAvailable().Contains(r));
            Assert.That(objectPool.GetAvailable().Contains(gr));
            Assert.That(objectPool.GetInUse().Count, Is.EqualTo(0));
        }
    }
}