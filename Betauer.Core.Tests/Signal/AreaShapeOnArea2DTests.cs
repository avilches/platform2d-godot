using System.Threading.Tasks;
using Betauer.Signal;
using Betauer.Signal.Bus;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Object = Godot.Object;

namespace Betauer.Tests.Signal {
    [TestFixture]
    public class AreaShapeOnArea2DTests : BaseNodeTest {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
            LoggerFactory.SetConsoleOutput(ConsoleOutput.ConsoleWriteLine);
        }

        [Test]
        public async Task AreaShapeOnArea2DFilterMulticastTest() {
            var body1Calls = 0;
            var body2Calls = 0;
            var noFilter = 0;
            var noFilterNoOwner = 0;
            Area2D area1 = new Area2D();
            Area2D area2 = new Area2D();
            Area2D body1 = new Area2D();
            Area2D body2 = new Area2D();
            AreaShapeOnArea2DEntered.Multicast topic = new AreaShapeOnArea2DEntered.Multicast("T");
            var o = new Object();
            topic.OnEventFilter(body1, (area2D1, tuple) => {
                Assert.That(tuple.Item2, Is.EqualTo(body1));
                body1Calls++;
            });
            topic.OnEventFilter(body2, (a) => body2Calls++);
            topic.OnEvent((a, b) => noFilterNoOwner++);
            topic.OnEvent(o, (a, b) => {
                noFilter++;
            });
            Assert.That(topic.EventHandlers.Count, Is.EqualTo(4));

            // When events are published, the origin doesn't matter
            topic.Emit(area1, (new RID(new Object()), body1, 1, 1));
            topic.Emit(area2, (new RID(new Object()), body2, 1, 1));
            topic.Emit(area1, (new RID(new Object()), body2, 1, 1));

            // Then
            Assert.That(body1Calls, Is.EqualTo(1));
            Assert.That(body2Calls, Is.EqualTo(2));
            Assert.That(noFilter, Is.EqualTo(3));
            Assert.That(noFilterNoOwner, Is.EqualTo(3));

            // When body is disposed
            body1.Dispose();
            // When new events are published
            topic.Emit(area1, (new RID(new Object()), body2, 1, 1));
            // Then the "Body1" listener with the disposed filter disappear
            Assert.That(topic.EventHandlers.Count, Is.EqualTo(3));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(3)); // 2 before + 1 now
            Assert.That(noFilter, Is.EqualTo(4)); // 3 before + 1 now
            Assert.That(noFilterNoOwner, Is.EqualTo(4)); // 3 before + 1 now
            
            o.Free();
            topic.Emit(area1, (new RID(new Object()), body1, 1, 1));

            Assert.That(topic.EventHandlers.Count, Is.EqualTo(2));
            // And data is ok
            Assert.That(body1Calls, Is.EqualTo(1)); // 1 as before
            Assert.That(body2Calls, Is.EqualTo(3)); // 3 before
            Assert.That(noFilter, Is.EqualTo(4)); // 4 as before
            Assert.That(noFilterNoOwner, Is.EqualTo(5)); // 4 as before + 1
        }

        [Test]
        public async Task AreaShapeOnArea2DFilterUnicastTest() {
            var calls = 0;
            Area2D area1 = new Area2D();
            Area2D area2 = new Area2D();
            Area2D body1 = new Area2D();
            Area2D body2 = new Area2D();
            AreaShapeOnArea2DEntered.Unicast topic = new AreaShapeOnArea2DEntered.Unicast("T");
            
            topic.OnEventFilter(body1, (a, tuple) => {
                Assert.That(tuple.Item2, Is.EqualTo(body1));
                calls++;
            });
            topic.Emit(area1, (new RID(new Object()), body1, 1, 1));
            topic.Emit(area2, (new RID(new Object()), body1, 1, 1));
            topic.Emit(area2, (new RID(new Object()), body2, 1, 1));
            Assert.That(calls, Is.EqualTo(2));

            calls = 0;
            topic.OnEventFilter(body1, (a) => calls++);
            topic.Emit(area1, (new RID(new Object()), body1, 1, 1));
            topic.Emit(area2, (new RID(new Object()), body1, 1, 1));
            topic.Emit(area2, (new RID(new Object()), body2, 1, 1));
            Assert.That(calls, Is.EqualTo(2));

            calls = 0;
            topic.OnEvent((a, b) => calls++);
            topic.Emit(area1, (new RID(new Object()), body1, 1, 1));
            topic.Emit(area2, (new RID(new Object()), body1, 1, 1));
            topic.Emit(area2, (new RID(new Object()), body2, 1, 1));
            Assert.That(calls, Is.EqualTo(3));
        }

        [Test]
        public async Task AreaShapeOnArea2DCollisionTests() {
            var multiEnterCalls = 0;
            var multiExitCalls = 0;
            var uniEnterCalls = 0;
            var uniExitCalls = 0;
            var body = CreateArea2D("player body", 0, 0);
            var area2D = CreateArea2D("area", 2000, 2000);
            
            await this.AwaitPhysicsFrame();

            AreaShapeOnArea2DEntered.Multicast multiEnter = new AreaShapeOnArea2DEntered.Multicast("T");
            AreaShapeOnArea2DExited.Multicast multiExit = new AreaShapeOnArea2DExited.Multicast("T");
            AreaShapeOnArea2DEntered.Unicast uniEnter = new AreaShapeOnArea2DEntered.Unicast("T");
            AreaShapeOnArea2DExited.Unicast uniExit = new AreaShapeOnArea2DExited.Unicast("T");
            AreaShapeOnArea2D.Status status = new AreaShapeOnArea2D.Status();
            AreaShapeOnArea2D.Collection collection = new AreaShapeOnArea2D.Collection();
            uniEnter.Connect(area2D);
            uniExit.Connect(area2D);
            multiEnter.Connect(area2D);
            multiExit.Connect(area2D);
            status.Connect(area2D);
            collection.Connect(area2D);

            uniEnter.OnEventFilter(body, (origin) => uniEnterCalls++);
            uniExit.OnEventFilter(body, (origin) => uniExitCalls++);
            multiEnter.OnEventFilter(body, (origin) => multiEnterCalls++);
            multiExit.OnEventFilter(body, (origin) => multiExitCalls++);
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