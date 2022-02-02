using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests.DI {
    [TestFixture]
    public class ScannerBasicTests : Node {
        public interface INotTagged {
        }

        [Service]
        public class MyServiceWithNotScanned {
            [Inject] internal INotTagged singleton22;
        }

        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);
            di.Scanner.Scan<INotTagged>();
            di.Scanner.Scan<MyServiceWithNotScanned>();
            Assert.That(!di.Exist<INotTagged>());
            Assert.That(di.Exist<MyServiceWithNotScanned>());
            try {
                di.Resolve<MyServiceWithNotScanned>();
                Assert.That(false, "It should fail!");
            } catch (InjectFieldException e) {
            }
        }

        [Service(Lifestyle = Lifestyle.Transient)]
        public class EmptyTransient {
            public static int Created = 0;

            public EmptyTransient() {
                Created++;
            }
        }

        [Service]
        public class SingletonWith2Transients {
            public static int Created = 0;

            public SingletonWith2Transients() {
                Created++;
            }

            [Inject] internal EmptyTransient et1;
            [Inject] internal EmptyTransient et2;
        }

        [Service]
        public class MySingleton {
            public static int Created = 0;

            public MySingleton() {
                Created++;
            }

            [Inject] internal SingletonWith2Transients singleton1;
            [Inject] internal SingletonWith2Transients singleton2;
            [Inject] internal EmptyTransient et;
        }

        [Test(Description = "Inject singletons in singleton")]
        public void SingletonInSingleton() {
            var di = new Container(this);
            EmptyTransient.Created = 0;
            SingletonWith2Transients.Created = 0;
            MySingleton.Created = 0;

            di.Scanner.Scan<EmptyTransient>();
            di.Scanner.Scan<MySingleton>();
            di.Scanner.Scan<SingletonWith2Transients>();
            var s1 = di.Resolve<SingletonWith2Transients>();
            var s2 = di.Resolve<SingletonWith2Transients>();
            var ms1 = di.Resolve<MySingleton>();
            var ms2 = di.Resolve<MySingleton>();

            Assert.That(EmptyTransient.Created, Is.EqualTo(3));
            Assert.That(SingletonWith2Transients.Created, Is.EqualTo(1));
            Assert.That(MySingleton.Created, Is.EqualTo(1));

            // Singleton are all the same instance
            Assert.That(s1, Is.EqualTo(s2));
            Assert.That(ms1.singleton1, Is.EqualTo(s1));
            Assert.That(ms1.singleton2, Is.EqualTo(s1));

            Assert.That(ms2.singleton1, Is.EqualTo(s1));
            Assert.That(ms2.singleton2, Is.EqualTo(s1));

            // Transient are all different: 1 in MyService singleton and 2 in SingletonWith2Transients
            Assert.That(ms1.et, Is.Not.EqualTo(ms1.singleton1.et1));
            Assert.That(ms1.et, Is.Not.EqualTo(ms1.singleton1.et2));
        }

        [Service(Lifestyle = Lifestyle.Transient)]
        public class TransientService {
            public static int Created = 0;

            public TransientService() {
                Created++;
            }

            [Inject] internal EmptyTransient et;
            [Inject] internal SingletonWith2Transients SingletonWith2Transients;
        }

        [Test(Description = "Inject transients in transient")]
        public void SingletonInTransient() {
            var di = new Container(this);
            EmptyTransient.Created = 0;
            SingletonWith2Transients.Created = 0;
            TransientService.Created = 0;
            EmptyTransient.Created = 0;

            di.Scanner.Scan<EmptyTransient>();
            di.Scanner.Scan<TransientService>();
            di.Scanner.Scan<SingletonWith2Transients>();
            di.Scanner.Scan<EmptyTransient>();
            var s1 = di.Resolve<SingletonWith2Transients>();

            var ts1 = di.Resolve<TransientService>();
            Assert.That(TransientService.Created, Is.EqualTo(1));
            Assert.That(EmptyTransient.Created, Is.EqualTo(3));

            var ts2 = di.Resolve<TransientService>();
            Assert.That(TransientService.Created, Is.EqualTo(2));
            Assert.That(EmptyTransient.Created, Is.EqualTo(4));

            Assert.That(SingletonWith2Transients.Created, Is.EqualTo(1));

            Assert.That(ts1, Is.Not.EqualTo(ts2));
            Assert.That(ts1.SingletonWith2Transients, Is.EqualTo(s1));
            Assert.That(ts2.SingletonWith2Transients, Is.EqualTo(s1));
        }
    }
}