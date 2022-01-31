using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests {

    public interface ISingleton1 {
    }

    public interface ISingleton2 {
    }

    [Service]
    public class Singleton : ISingleton1, ISingleton2{

    }

    [TestFixture]
    public class DiRepositoryTests : Node {
        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);
            di.Scanner.Scan(typeof(Singleton));

        }
    }

    public interface IInterfaceAlone {
    }

    public interface IInterface {
    }

    public class MyClass1 : IInterface {
    }

    public class MyClass2 : IInterface {
    }

    [TestFixture]
    public class RegisterTests : Node {
        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);

            // Not found types fail
            try {
                di.Resolve<IInterface>();
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            try {
                di.Resolve(typeof(IInterface));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        /*
         * OnInstanceCreated
         */
        [Test(Description = "GetServiceType returns the correct Type")]
        public void RegisterService() {
            var di = new Container(this);
            IService s = null;

            // by class
            var node = new Node();
            s = di.RegisterSingleton(node);
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(Node)));

            // by interface
            var mySingleton = new MyClass1();
            s = di.RegisterSingleton(typeof(IInterface), mySingleton);
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(IInterface)));

            // by class (no interface)
            s = di.RegisterSingleton(typeof(MyClass1), mySingleton);
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(MyClass1)));

            // auto factory typed
            s = di.Register<Node>(Lifestyle.Singleton);
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(Node)));

            // auto factory typed
            s = di.Register<Node>(Lifestyle.Transient);
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(Node)));

            // by typed factory
            s = di.Register(Lifestyle.Transient, () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(Control)));

            s = di.Register(Lifestyle.Singleton, () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(Control)));

            // by object typed factory + type
            s = di.Register(Lifestyle.Transient, typeof(CanvasItem), () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(CanvasItem)));

            s = di.Register(Lifestyle.Singleton, typeof(CanvasItem), () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceType(), Is.EqualTo(typeof(CanvasItem)));
        }

        [Test(Description = "Singleton instance OnInstanceCreated. No need to resolve to execute the hook")]
        public void RegisterSingletonInstanceOnInstanceCreated() {
            var di = new Container(this);
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y");
            var instance = new Node();
            var s = di.RegisterSingleton(instance);
            Assert.That(instance.GetMeta("x"), Is.EqualTo("y"));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated")]
        public void RegisterSingletonFactoryOnInstanceCreated() {
            var di = new Container(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            var instance = new Node();
            di.Register(Lifestyle.Singleton, () => instance);
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated")]
        public void RegisterTransientFactoryOnInstanceCreated() {
            var di = new Container(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            di.Register(Lifestyle.Transient, () => new Node());
            di.Register(Lifestyle.Transient, () => new Control());
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(di.Resolve<Control>().GetMeta("x"), Is.EqualTo("y2"));
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y3"));
        }

        /*
         * Node Singleton are added to the owner when created
         */
        [Test(Description = "Register a Singleton Node adds it as child when added. No need to resolve")]
        public void RegisterNodeInstance() {
            var di = new Container(this);
            // Register instance
            var instance = new Node();
            di.RegisterSingleton(instance);
            Assert.That(GetChildren().Contains(instance));
        }

        [Test(Description = "Register a Singleton Factory Node adds it as child when resolved. No need to resolve")]
        public void RegisterNodeSingletonFactory() {
            var di = new Container(this);
            // Register instance
            var instance = new Node();
            di.Register(Lifestyle.Singleton, () => instance);
            Assert.That(!GetChildren().Contains(instance));

            di.Resolve<Node>();
            Assert.That(GetChildren().Contains(instance));
        }

        /*
         * Access to singleton instance by type
         */

        [Test(Description = "Register a Singleton instance is only accessible by its Type")]
        public void RegisterSingletonInstance() {
            var di = new Container(this);

            // Register instance
            var instance = new Node();
            di.RegisterSingleton(instance);
            Assert.That(di.Resolve<Node>(), Is.EqualTo(instance));
            Assert.That(di.Resolve(typeof(Node)), Is.EqualTo(instance));

            // Instances of the same Type can be overriden
            var instance2 = new Node();
            di.RegisterSingleton(instance2);
            Assert.That(di.Resolve<Node>(), Is.EqualTo(instance2));
            Assert.That(di.Resolve(typeof(Node)), Is.EqualTo(instance2));
        }

        [Test(Description =
            "Register a Singleton instance with an interface as Type is only accessible by interface - Generic")]
        public void RegisterSingletonInterfaceUsingGeneric() {
            var di = new Container(this);

            var mySingleton = new MyClass1();
            di.RegisterSingleton<IInterface>(mySingleton);
            Assert.That(di.Resolve<IInterface>(), Is.EqualTo(mySingleton));
            Assert.That(di.Resolve(typeof(IInterface)), Is.EqualTo(mySingleton));
            try {
                // The instance type can't be resolved, it's only possible if the interface type is used
                di.Resolve(typeof(MyClass1));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // Interface can be overriden by other instance of other type
            var mySingleton2 = new MyClass2();
            di.RegisterSingleton<IInterface>(mySingleton2);
            Assert.That(di.Resolve<IInterface>(), Is.EqualTo(mySingleton2));
            Assert.That(di.Resolve(typeof(IInterface)), Is.EqualTo(mySingleton2));
            try {
                di.Resolve(typeof(MyClass2));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        [Test(Description =
            "Register a Singleton instance with an interface as Type is only accessible by interface - Type")]
        public void RegisterSingletonInterfaceUsingType() {
            var di = new Container(this);

            var mySingleton = new MyClass1();
            // The instance should implement the interface
            di.RegisterSingleton(typeof(IInterface), mySingleton);
            Assert.That(di.Resolve<IInterface>(), Is.EqualTo(mySingleton));
            Assert.That(di.Resolve(typeof(IInterface)), Is.EqualTo(mySingleton));
            try {
                // The instance type can't be resolved, it's only possible if the interface type is used
                di.Resolve(typeof(MyClass1));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // Interface can be overriden by other instance of other type
            var mySingleton2 = new MyClass2();
            di.RegisterSingleton(typeof(IInterface), mySingleton2);
            Assert.That(di.Resolve<IInterface>(), Is.EqualTo(mySingleton2));
            Assert.That(di.Resolve(typeof(IInterface)), Is.EqualTo(mySingleton2));
            try {
                di.Resolve(typeof(MyClass2));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            try {
                // The instance type doesn't match the interface, so it fails
                di.RegisterSingleton(typeof(IInterfaceAlone), mySingleton);
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
            }
        }

        /**
         * Factories
         */
        [Test(Description = "Register a Singleton factory is executed only the first time")]
        public void RegisterSingletonFactory() {
            var di = new Container(this);
            var n = 0;
            di.Register(Lifestyle.Singleton, () => ++n);

            // Ensures that factory is called only the first time
            Assert.That(n, Is.EqualTo(0));
            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
        }

        [Test(Description = "Register a Singleton type creates a factory and is executed only the first time")]
        public void RegisterSingletonType() {
            var di = new Container(this);
            di.Register<Node>(Lifestyle.Singleton);

            // Ensures that factory is called only the first time
            Node node1 = di.Resolve<Node>();
            Node node2 = di.Resolve<Node>();
            Assert.That(node1, Is.Not.Null);
            Assert.That(node1.GetHashCode(), Is.EqualTo(node2.GetHashCode()));
        }

        [Test(Description = "Register Transient factory is executed every time")]
        public void RegisterTransientFactory() {
            var di = new Container(this);
            var n = 0;
            di.Register(Lifestyle.Transient, () => ++n);

            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
            Assert.That(di.Resolve<int>(), Is.EqualTo(2));
        }


        [Test(Description = "Register Transient type creates a factory and is executed every time")]
        public void RegisterTransientType() {
            var di = new Container(this);
            di.Register<Node>(Lifestyle.Transient);

            // Ensures that factory is called only the first time
            Node node1 = di.Resolve<Node>();
            Node node2 = di.Resolve<Node>();
            Assert.That(node1, Is.Not.Null);
            Assert.That(node2, Is.Not.Null);
            Assert.That(node1.GetHashCode(), Is.Not.EqualTo(node2.GetHashCode()));
        }

        [Test(Description = "Register a factory (Singleton or Transient) with a wrong type fails")]
        public void RegisterTypedFactoryWrongType() {
            var di = new Container(this);
            di.Register(Lifestyle.Transient, typeof(string), () => 1);
            di.Register(Lifestyle.Singleton, typeof(int), () => "");

            try {
                di.Resolve<string>();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
            try {
                di.Resolve<int>();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
        }

        /*
         * Types
         */

        [Test(Description = "Register a factory with a compatible type")]
        public void RegisterTypedTypedFactory() {
            var di = new Container(this);
            di.Register(Lifestyle.Transient, typeof(IInterface), () => new MyClass1());

            Assert.That(di.Resolve<IInterface>().GetType(), Is.EqualTo(typeof(MyClass1)));
        }

        /*
         * Special
         */

        [Test(Description = "Register a lambda as instance can be called as method")]
        public void RegisterFactoryAsInstance() {
            var di = new Container(this);
            var n = 0;
            di.RegisterSingleton<Func<int>>(() => ++n);

            Func<int> resolve = di.Resolve<Func<int>>();

            Assert.That(resolve(), Is.EqualTo(1));
            Assert.That(resolve(), Is.EqualTo(2));
        }
    }
}