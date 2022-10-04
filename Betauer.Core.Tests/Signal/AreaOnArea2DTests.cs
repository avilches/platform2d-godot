using System.Threading.Tasks;
using Betauer.Signal;
using Betauer.Bus.Signal;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Tests.Signal {
    [TestFixture]
    public class AreaOnArea2DTests : BaseNodeTest {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
            LoggerFactory.SetConsoleOutput(ConsoleOutput.ConsoleWriteLine);
        }

        [Test]
        public void AreaOnArea2DFilterMulticastTest() {
            var body1Calls = 0;
            var body2Calls = 0;
            var noFilter = 0;
            var noFilterNoOwner = 0;
            Area2D area1 = new Area2D();
            Area2D area2 = new Area2D();
            Area2D body1 = new Area2D();
            Area2D body2 = new Area2D();
            AreaOnArea2DEntered.Multicast topic = new AreaOnArea2DEntered.Multicast("T");
            var o = new Object();
            topic.Subscribe((_, b) => {
                Assert.That(b, Is.EqualTo(body1));
                body1Calls++;
            }).WithFilter(body1);
            topic.Subscribe((_,_) => body2Calls++).WithFilter(body2);
            topic.Subscribe((_,_) => noFilterNoOwner++);
            topic.Subscribe((_,_) => noFilter++).RemoveIfInvalid(o);
            Assert.That(topic.Consumers.Count, Is.EqualTo(4));

            // When events are published, the origin doesn't matter
            topic.Publish(area1, body1);
            topic.Publish(area2, body2);
            topic.Publish(area1, body2);

            // Then
            Assert.That(body1Calls, Is.EqualTo(1));
            Assert.That(body2Calls, Is.EqualTo(2));
            Assert.That(noFilter, Is.EqualTo(3));
            Assert.That(noFilterNoOwner, Is.EqualTo(3));

            // When body is disposed
            body1.Dispose();
            // When new events are published
            topic.Publish(area1, body2);
            // Then the "Body1" listener with the disposed filter disappear
            Assert.That(topic.Consumers.Count, Is.EqualTo(3));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(3)); // 2 before + 1 now
            Assert.That(noFilter, Is.EqualTo(4)); // 3 before + 1 now
            Assert.That(noFilterNoOwner, Is.EqualTo(4)); // 3 before + 1 now
            
            o.Free();
            topic.Publish(area1, body1);

            Assert.That(topic.Consumers.Count, Is.EqualTo(2));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(3)); // 3 before
            Assert.That(noFilter, Is.EqualTo(4)); // 4 as before
            Assert.That(noFilterNoOwner, Is.EqualTo(5)); // 4 as before + 1
        }

        [Test]
        public void AreaOnArea2DFilterUnicastTest() {
            var calls = 0;
            Area2D area1 = new Area2D();
            Area2D area2 = new Area2D();
            Area2D body1 = new Area2D();
            Area2D body2 = new Area2D();
            AreaOnArea2DEntered.Unicast topic = new AreaOnArea2DEntered.Unicast("T");
            
            topic.Subscribe((_, b) => {
                Assert.That(b, Is.EqualTo(body1));
                calls++;
            }).WithFilter(body1);
            topic.Publish(area1, body1);
            topic.Publish(area2, body1);
            topic.Publish(area2, body2);
            Assert.That(calls, Is.EqualTo(2));

            calls = 0;
            topic.Subscribe((_,_) => calls++).WithFilter(body1);
            topic.Publish(area1, body1);
            topic.Publish(area2, body1);
            topic.Publish(area2, body2);
            Assert.That(calls, Is.EqualTo(2));

            calls = 0;
            topic.Subscribe((_,_) => calls++);
            topic.Publish(area1, body1);
            topic.Publish(area2, body1);
            topic.Publish(area2, body2);
            Assert.That(calls, Is.EqualTo(3));
        }

        [Test]
        public async Task AreaOnArea2DCollisionTests() {
            var multiEnterCalls = 0;
            var multiExitCalls = 0;
            var uniEnterCalls = 0;
            var uniExitCalls = 0;
            var body = CreateArea2D("player body", 0, 0);
            var area2D = CreateArea2D("area", 2000, 2000);
            await this.AwaitPhysicsFrame();

            AreaOnArea2DEntered.Multicast multiEnter = new AreaOnArea2DEntered.Multicast("T");
            AreaOnArea2DExited.Multicast multiExit = new AreaOnArea2DExited.Multicast("T");
            AreaOnArea2DEntered.Unicast uniEnter = new AreaOnArea2DEntered.Unicast("T");
            AreaOnArea2DExited.Unicast uniExit = new AreaOnArea2DExited.Unicast("T");
            AreaOnArea2D.Status status = new AreaOnArea2D.Status();
            AreaOnArea2D.Collection collection = new AreaOnArea2D.Collection();
            uniEnter.Connect(area2D);
            uniExit.Connect(area2D);
            multiEnter.Connect(area2D);
            multiExit.Connect(area2D);
            status.Connect(area2D);
            collection.Connect(area2D);

            uniEnter.Subscribe((origin,_) => uniEnterCalls++).WithFilter(body);
            uniExit.Subscribe((origin,_) => uniExitCalls++).WithFilter(body);
            multiEnter.Subscribe((origin,_) => multiEnterCalls++).WithFilter(body);
            multiExit.Subscribe((origin,_) => multiExitCalls++).WithFilter(body);
            await this.AwaitPhysicsFrame();

            // They are not colliding
            Assert.That(uniEnterCalls, Is.EqualTo(0));
            Assert.That(uniExitCalls, Is.EqualTo(0));
            Assert.That(multiEnterCalls, Is.EqualTo(0));
            Assert.That(multiExitCalls, Is.EqualTo(0));
            Assert.That(status.Status, Is.False);
            Assert.That(collection.Size(), Is.EqualTo(0));

            await ForceCollision(area2D, body);

            Assert.That(uniEnterCalls, Is.EqualTo(1));
            Assert.That(uniExitCalls, Is.EqualTo(0));
            Assert.That(multiEnterCalls, Is.EqualTo(1));
            Assert.That(multiExitCalls, Is.EqualTo(0));
            Assert.That(status.Status, Is.True);
            Assert.That(collection.Size(), Is.EqualTo(1));
            Assert.That(collection.Contains(body), Is.True);

            await ForceNotCollision(area2D, body);
            
            Assert.That(uniEnterCalls, Is.EqualTo(1));
            Assert.That(uniExitCalls, Is.EqualTo(1));
            Assert.That(multiEnterCalls, Is.EqualTo(1));
            Assert.That(multiExitCalls, Is.EqualTo(1));
            Assert.That(status.Status, Is.False);
            Assert.That(collection.Size(), Is.EqualTo(0));
        }
    }
}