using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests {
    public interface IInterface1 {
    }

    public interface IInterface2 : IInterface2_2 {
    }

    public interface IInterface2_2 {
    }

    public class ClassWith1Interface : IInterface1 {
    }

    public class ClassWith2Interfaces : IInterface1, IInterface2 {
    }

    [TestFixture]
    public class RegisterTests : Node {
        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);

            // Not found types fail
            try {
                di.Resolve<IInterface1>();
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            try {
                di.Resolve(typeof(IInterface1));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        /*
         * Singleton instances
         */
        [Test(Description = "Register singleton instances")]
        public void RegisterSingletonInstanceServiceAllTypes() {
            var di = new Container(this);
            IService s = null;

            // Class with no interfaces
            var node = new Node();
            // Ensure IDisposable is ignored. Assert that Node implements IDisposable
            Assert.That(typeof(Node).GetInterfaces(), Contains.Item(typeof(IDisposable)));
            s = di.Instance(node).Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(Node)));
            Assert.That(s.Resolve(di), Is.EqualTo(node));
            Assert.That(di.Resolve<Node>(), Is.EqualTo(node));

            // Instances of the same Type can be overriden
            var instance2 = new Node();
            s = di.Instance(instance2).Build();
            Assert.That(di.Resolve<Node>(), Is.EqualTo(instance2));
            Assert.That(di.Resolve(typeof(Node)), Is.EqualTo(instance2));

            // Class with type and all nested interfaces by default
            di = new Container(this);
            var classWith2Interfaces = new ClassWith2Interfaces();
            s = di.Instance(classWith2Interfaces).Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(4));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(s.Resolve(di), Is.EqualTo(classWith2Interfaces));
            Assert.That(di.Resolve<ClassWith2Interfaces>(), Is.EqualTo(classWith2Interfaces));
            Assert.That(di.Resolve<IInterface1>(), Is.EqualTo(classWith2Interfaces));
            Assert.That(di.Resolve<IInterface2>(), Is.EqualTo(classWith2Interfaces));
            Assert.That(di.Resolve<IInterface2_2>(), Is.EqualTo(classWith2Interfaces));

            // Class with only one interfaces only. Try to resolve using the class will not work
            di = new Container(this);
            var mySingleton = new ClassWith2Interfaces();
            s = di.Instance(mySingleton).As<IInterface2>().Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.Resolve(di), Is.EqualTo(mySingleton));
            Assert.That(di.Resolve<IInterface2>(), Is.EqualTo(mySingleton));
            try {
                Assert.That(di.Resolve(typeof(ClassWith2Interfaces)), Is.EqualTo(mySingleton));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
            try {
                Assert.That(di.Resolve(typeof(IInterface1)), Is.EqualTo(mySingleton));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // by interfaces only. Try to resolve using the class will not work
            di = new Container(this);
            mySingleton = new ClassWith2Interfaces();
            s = di.Instance(mySingleton).AsAll<IInterface2>().Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(s.Resolve(di), Is.EqualTo(mySingleton));
            Assert.That(di.Resolve(typeof(IInterface2)), Is.EqualTo(mySingleton));
            try {
                Assert.That(di.Resolve(typeof(ClassWith2Interfaces)), Is.EqualTo(mySingleton));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
            try {
                Assert.That(di.Resolve(typeof(IInterface1)), Is.EqualTo(mySingleton));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // Wrong type

            try {
                s = di.Instance(mySingleton).AsAll<IDisposable>().Build();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
            }
        }

        /*
         * Factories
         */
        [Test(Description = "Register a typed factory will use Type and all interfaces too")]
        public void RegisterTypedTypedFactoryWithInterfaces() {
            var di = new Container(this);
            var s = di.Register(Lifestyle.Transient, () => new ClassWith1Interface());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(ClassWith1Interface)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve(typeof(ClassWith1Interface)).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve<ClassWith1Interface>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));

            di = new Container(this);
            s = di.Register(Lifestyle.Transient, () => new ClassWith2Interfaces());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(4));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(di.Resolve(typeof(ClassWith2Interfaces)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2_2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2_2)));
        }

        [Test(Description = "Register a interfaced factory the interfaces of the interfaces only")]
        public void RegisterTypedTypedFactoryWithInterfaces2() {
            var di = new Container(this);
            var s = di.Register(Lifestyle.Transient, () => (IInterface1)new ClassWith1Interface());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));

            try {
                di.Resolve(typeof(ClassWith1Interface));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        [Test(Description = "When register Factory with an interface as Type, it only uses the interfaces")]
        public void RegisterSingleTypedFactory() {
            var di = new Container(this);
            var s = di.RegisterAllTypes(Lifestyle.Transient, typeof(IInterface2), () => new ClassWith2Interfaces());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2_2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));

            // Access by class name shouldn't work
            try {
                di.Resolve(typeof(ClassWith2Interfaces));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // Access by other interface class name shouldn't work
            try {
                di.Resolve(typeof(IInterface1));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        [Test(Description = "When register Factory with an array of types, only the types are registered")]
        public void RegisterTypedTypedFactory() {
            var di = new Container(this);
            var s = di.Register(Lifestyle.Transient, new Type[]{typeof(IInterface2)}, () => new ClassWith2Interfaces());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));

            // Access by class name shouldn't work
            try {
                di.Resolve(typeof(ClassWith2Interfaces));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // Access by other interface class name shouldn't work
            try {
                di.Resolve(typeof(IInterface1));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
            // Access by other interface class name shouldn't work
            try {
                di.Resolve(typeof(IInterface2_2));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        [Test(Description = "Registered a Factory with specific interface type is only registered by its interface")]
        public void RegisterTypedFactoryWithInterface() {
            var di = new Container(this);
            IService s = null;
            // factory typed with specific interface
            di = new Container(this);
            s = di.Register<IInterface2>(Lifestyle.Transient, () => new ClassWith2Interfaces());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2_2)));
            try {
                // The instance type can't be resolved, it's only possible if the interface type is used
                di.Resolve(typeof(IInterface1));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        /*
         * Auto factories
         */

        [Test(Description = "Auto factories with interfaces are not allowed")]
        public void RegisterAutoFactoryWithInterfacesIsNotAllowed() {
            var di = new Container(this);
            try {
                di.Register<IInterface2_2>(Lifestyle.Singleton);
                Assert.That(false, "It should fail!");
            } catch (InvalidOperationException) {
            }

            try {
                di.Register(Lifestyle.Singleton, typeof(IInterface2_2));
                Assert.That(false, "It should fail!");
            } catch (InvalidOperationException) {
            }
        }

        [Test(Description = "Auto factories with Type include all the interfaces too")]
        public void RegisterAutoFactory() {
            var di = new Container(this);
            IService s = null;

            // auto factory
            s = di.Register<Node>(Lifestyle.Singleton);
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(Node)));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(1));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(Node)));
            Assert.That(di.Resolve(typeof(Node)).GetType(), Is.EqualTo(typeof(Node)));
            Assert.That(di.Resolve<Node>().GetType(), Is.EqualTo(typeof(Node)));
            Assert.That(di.Resolve(typeof(Node)).GetType(), Is.EqualTo(typeof(Node)));
            Assert.That(di.Resolve<Node>().GetType(), Is.EqualTo(typeof(Node)));

            // auto factory
            s = di.Register<ClassWith2Interfaces>(Lifestyle.Transient);
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes().Length, Is.EqualTo(4));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(di.Resolve(typeof(ClassWith2Interfaces)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2_2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
        }

        /*
         * Factories
         */

        [Test(Description = "Factories lifestyle")]
        public void RegisterFactory() {
            var di = new Container(this);
            IService s = null;

            // by typed factory
            s = di.Register(Lifestyle.Transient, () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(Control)));

            s = di.Register(Lifestyle.Singleton, () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(Control)));

            // by object typed factory + type
            s = di.RegisterAllTypes(Lifestyle.Transient, typeof(CanvasItem), () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(CanvasItem)));

            s = di.RegisterAllTypes(Lifestyle.Singleton, typeof(CanvasItem), () => new Control());
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetServiceTypes(), Contains.Item(typeof(CanvasItem)));
        }

        /*
         * OnInstanceCreated
         */
        [Test(Description = "Singleton instance OnInstanceCreated. No need to resolve to execute the hook")]
        public void RegisterSingletonInstanceOnInstanceCreated() {
            var di = new Container(this);
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y");
            var instance = new Node();
            var s = di.Instance(instance).Build();
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
            di.Instance(instance).Build();
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

        /**
         * Factories
         */
        [Test(Description = "Register a Singleton factory is executed only the first time")]
        public void RegisterSingletonFactory() {
            var di = new Container(this);
            var n = 0;
            var s = di.Register(Lifestyle.Singleton, () => ++n);

            // Ensures that factory is called only the first time
            Assert.That(n, Is.EqualTo(0));
            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
        }

        [Test(Description = "Register a Singleton type creates a factory and is executed only the first time")]
        public void RegisterSingletonType() {
            var di = new Container(this);
            var s = di.Register<Node>(Lifestyle.Singleton);

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
            di.RegisterAllTypes(Lifestyle.Transient, typeof(string), () => 1);
            di.RegisterAllTypes(Lifestyle.Singleton, typeof(int), () => "");

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
         * Special
         */

        [Test(Description = "Register a lambda as instance can be called as method")]
        public void RegisterFactoryAsInstance() {
            var di = new Container(this);
            var n = 0;
            di.Instance<Func<int>>(() => ++n).Build();

            Func<int> resolve = di.Resolve<Func<int>>();

            Assert.That(resolve(), Is.EqualTo(1));
            Assert.That(resolve(), Is.EqualTo(2));
        }
    }
}