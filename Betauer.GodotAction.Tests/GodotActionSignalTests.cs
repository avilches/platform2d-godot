using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GodotAction.Tests {
    [TestFixture]
    public class GodotActionSignalTests : Node {
        [Test(Description = "0p and 1p signals")]
        [Only]
        public async Task BasicTest() {
            var b1 = new CheckButton();
            var b2 = new CheckButton();
            AddChild(b1);
            AddChild(b2);
            await this.AwaitIdleFrame();
            var executed1 = 0;
            var executed2 = 0;
            var toggled1 = new List<bool>();
            var toggled2 = new List<bool>();

            CheckButtonAction cb1 = b1.OnPressed(() => {
                executed1++;
            });
            CheckButtonAction cb2 = b2.OnPressed(() => {
                executed2++;
            });

            cb1.OnToggled((tog) => {
                toggled1.Add(tog);
            });
            cb2.OnToggled((tog) => {
                toggled2.Add(tog);
            });
            
            b1.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(0));
            Assert.That(toggled1, Is.EqualTo(new bool[]{}));
            Assert.That(toggled2, Is.EqualTo(new bool[]{}));

            b2.EmitSignal("pressed");
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1, Is.EqualTo(new bool[]{}));
            Assert.That(toggled2, Is.EqualTo(new bool[]{}));

            b1.Pressed = true;
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true}));
            Assert.That(toggled2.ToArray(), Is.EqualTo(new bool[]{}));
            
            b2.Pressed = true;
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true}));
            Assert.That(toggled2.ToArray(), Is.EqualTo(new []{true}));

            b1.Pressed = false;
            b2.Pressed = false;
            Assert.That(executed1, Is.EqualTo(1));
            Assert.That(executed2, Is.EqualTo(1));
            Assert.That(toggled1.ToArray(), Is.EqualTo(new []{true, false}));
            Assert.That(toggled2.ToArray(), Is.EqualTo(new []{true, false}));
            
        }

        [Test]
        [Only]
        public async Task OneShot() {
            var b1 = new CheckButton();
            var b2 = new CheckButton();
            AddChild(b1);
            AddChild(b2);
            await this.AwaitIdleFrame();
            var executedNormal = 0;
            var executedOneShot = 0;

            CheckButtonAction cb1 = b1.OnPressed(() => { executedNormal++; });
            CheckButtonAction cb2 = b2.OnPressed(() => { executedOneShot++; }, true);
            b1.EmitSignal("pressed");
            b2.EmitSignal("pressed");
            Assert.That(executedNormal, Is.EqualTo(1));
            Assert.That(executedOneShot, Is.EqualTo(1));

            b1.EmitSignal("pressed");
            b2.EmitSignal("pressed");
            Assert.That(executedNormal, Is.EqualTo(2));
            Assert.That(executedOneShot, Is.EqualTo(1));
        }
    }
}