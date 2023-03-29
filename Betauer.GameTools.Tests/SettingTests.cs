using System;
using System.Collections.Generic;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.Application.Settings;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.Input;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.GameTools.Tests; 

[TestRunner.Test]
public partial class SettingTests : Node {

    const string SettingsFile = "./test-settings.ini";
    const string SettingsFile1 = "./test-settings-1.ini";
    const string SettingsFile2 = "./test-settings-2.ini";
        
    [SetUpClass]
    [TestRunner.TearDownClass]
    public void Clear() {
        System.IO.File.Delete(SettingsFile);
        System.IO.File.Delete(SettingsFile1);
        System.IO.File.Delete(SettingsFile2);
    }

    [TestRunner.Test]
    public void MemoryTest() {
        var imm = Setting<string>.Memory("I");
            
        Assert.That(imm.Value, Is.EqualTo("I"));
        imm.Value = "X";
        Assert.That(imm.Value, Is.EqualTo("X"));
    }

    [TestRunner.Test]
    public void RegularAutoSaveTest() {
        var sc = new SettingsContainer(SettingsFile);
        var saved = Setting<string>.Persistent("IGNORED", "Section", "AutoSave", "Default");
            
        // Read without container
        Assert.Throws<NullReferenceException>(() => { var x = saved.Value; });

        // Write without container
        Assert.Throws<NullReferenceException>(() => saved.Value = "FAIL");

        saved.SetSettingsContainer(sc);
        Assert.That(saved.Value, Is.EqualTo("Default"));
        Assert.That(saved.SettingsContainer, Is.EqualTo(sc));

        const string changed = "XXXX";
        saved.Value = changed;
        Assert.That(saved.Value, Is.EqualTo(changed));

        var cf = new ConfigFile();
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<string>(saved.Section, saved.Name, "WRONG"), Is.EqualTo(changed));
    } 

    [TestRunner.Test]
    public void RegularNoAutoSaveTest() {
        var sc = new SettingsContainer(SettingsFile);
        var saved = Setting<string>.Persistent("IGNORED", "Section", "NoAutoSave", "Default", false);
            
        // Read without container
        Assert.Throws<NullReferenceException>(() => { var x = saved.Value; });

        // Write without container
        Assert.Throws<NullReferenceException>(() => saved.Value = "FAIL");

        saved.SetSettingsContainer(sc);
        Assert.That(saved.Value, Is.EqualTo("Default"));
        Assert.That(saved.SettingsContainer, Is.EqualTo(sc));

        const string changed = "XXXX";
        saved.Value = changed;
        Assert.That(saved.Value, Is.EqualTo(changed));

        var cf = new ConfigFile();
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<string>(saved.Section, saved.Name, "NOT SAVED"), Is.EqualTo("NOT SAVED"));

        saved.SettingsContainer.Save();
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<string>(saved.Section, saved.Name, "WRONG"), Is.EqualTo(changed));
    } 

    [TestRunner.Test]
    public void DisabledTest() {
        var sc = new SettingsContainer(SettingsFile);
        var savedDisabled = Setting<string>.Persistent("IGNORED", "Section", "SavedDisabled", "Default", true, false);
            
        // Read without container
        Assert.That(savedDisabled.Value, Is.EqualTo("Default"));

        // Write without container
        savedDisabled.Value = "New1";
        Assert.That(savedDisabled.Value, Is.EqualTo("New1"));

        savedDisabled.SetSettingsContainer(sc);
        Assert.That(savedDisabled.Value, Is.EqualTo("New1"));
        Assert.That(savedDisabled.SettingsContainer, Is.EqualTo(sc));

        savedDisabled.Value = "New2";
        Assert.That(savedDisabled.Value, Is.EqualTo("New2"));

        var cf = new ConfigFile();
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<string>(savedDisabled.Section, savedDisabled.Name, "NOT FOUND"), Is.EqualTo("NOT FOUND"));
    } 

    [Configuration]
    internal class ErrorConfigWithNoContainer {
        [Singleton] 
        public ISetting<string> P1() => Setting<string>.Persistent("Section", "Name", "Default");
    }

    [TestRunner.Test(Description = "Error if container not found by type")]
    public void ErrorConfigWithNoContainerTest() {
        var di = new Container.Builder();
        di.Scan<ErrorConfigWithNoContainer>();
        Assert.Throws<ServiceNotFoundException>(() => di.Build());
    }

    [Configuration]
    internal class ErrorConfigWithContainerNotFoundByName {
        [Singleton] 
        public ISetting<string> P1 => Setting<string>.Persistent("NOT FOUND", "Section", "Name", "Default");
    }

    [TestRunner.Test(Description = "Error if container not found by name")]
    public void ErrorConfigWithContainerNotFoundByNameTest() {
        var di = new Container.Builder();
        di.Scan<ErrorConfigWithContainerNotFoundByName>();
        Assert.Throws<ServiceNotFoundException>(() => di.Build());
    }

    [Configuration]
    internal class ConfigWithSettingContainer {
        [Singleton]
        public SettingsContainer SettingsContainer => new SettingsContainer(SettingsFile);
            
        [Singleton] 
        public SaveSetting<bool> BoolSetting => Setting<bool>.Persistent("Section", "PixelPerfect", true);

        [Singleton] 
        public SaveSetting<string> StringSetting => Setting<string>.Persistent("Section", "Name", "Default");
            
        [Singleton] 
        public SaveSetting<Resolution> Resolution => Setting<Resolution>.Persistent("Video", "Screen", Resolutions.WXGA);

        [Singleton] 
        public SaveSetting<string> NoAutoSave => Setting<string>.Persistent("Video", "NoAutoSave", "DEFAULT", false);

        [Singleton] 
        public SaveSetting<string> NoEnabled => Setting<string>.Persistent("Disabled", "PropertyDisabled", "DEFAULT", true, false);
    }

    [Singleton]
    internal class Service1 {
        [Inject] public SettingsContainer SettingsContainerByType { get; set; }
        [Inject] public SaveSetting<bool> BoolSetting { get; set; }
        [Inject] public SaveSetting<string> StringSetting { get; set; }
        [Inject] public SaveSetting<Resolution> Resolution { get; set; }
        [Inject] public SaveSetting<string> NoAutoSave { get; set; }
        [Inject] public SaveSetting<string> NoEnabled { get; set; }
    }

    [TestRunner.Test]
    public void ConfigWithSettingContainerTest() {
        var di = new Container.Builder();
        di.Scan<ConfigWithSettingContainer>();
        di.Scan<Service1>();
        var c = di.Build();

        var b = c.Resolve<Service1>();

        // Check the SettingContainer
        Assert.That(b.BoolSetting.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
        Assert.That(b.StringSetting.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
        Assert.That(b.Resolution.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
        Assert.That(b.NoAutoSave.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));
        Assert.That(b.NoEnabled.SettingsContainer, Is.EqualTo(b.SettingsContainerByType));

        // Read with no settings save, the default values are used
        Assert.That(b.BoolSetting.Value, Is.True);
        Assert.That(b.StringSetting.Value, Is.EqualTo("Default"));
        Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.WXGA));
        Assert.That(b.NoAutoSave.Value, Is.EqualTo("DEFAULT"));
        Assert.That(b.NoEnabled.Value, Is.EqualTo("DEFAULT"));
        Assert.That(b.SettingsContainerByType.Dirty, Is.False);
            
        // When force saved, default values are stored, except the no enabled
        b.BoolSetting.SettingsContainer.Save();
        Assert.That(b.SettingsContainerByType.Dirty, Is.False);
        var cf = new ConfigFile();
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<bool>(b.BoolSetting.Section, b.BoolSetting.Name), Is.True);
        Assert.That(cf.GetValue<string>(b.StringSetting.Section, b.StringSetting.Name, "XXX"), Is.EqualTo("Default"));
        Assert.That(cf.GetValue<Vector2I>(b.Resolution.Section, b.Resolution.Name), Is.EqualTo(Resolutions.WXGA.Size));
        Assert.That(cf.GetValue<string>(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("DEFAULT"));
        Assert.That(cf.GetValue<string>(b.NoEnabled.Section, b.NoEnabled.Name, "XXX"), Is.EqualTo("XXX")); // not written to file

        b.NoEnabled.Value = "NEW VALUE";
        Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE"));
        Assert.That(b.SettingsContainerByType.Dirty, Is.False);
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<string>(b.NoEnabled.Section, b.NoEnabled.Name, "XXX"), Is.EqualTo("XXX")); // not present in file
            
        // When changed, only the auto-saved are stored
        b.BoolSetting.Value = false;
        Assert.That(b.SettingsContainerByType.Dirty, Is.False);
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<bool>(b.BoolSetting.Section, b.BoolSetting.Name, true), Is.False);

        b.StringSetting.Value = "CHANGED";
        Assert.That(b.SettingsContainerByType.Dirty, Is.False);
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<string>(b.StringSetting.Section, b.StringSetting.Name, "XXX"), Is.EqualTo("CHANGED"));
            
        b.Resolution.Value = Resolutions.FULLHD;
        Assert.That(b.SettingsContainerByType.Dirty, Is.False);
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<Vector2I>(b.Resolution.Section, b.Resolution.Name), Is.EqualTo(Resolutions.FULLHD.Size));

        // No autosave, dirty and the value is still the old value
        b.NoAutoSave.Value = "CHANGED";
        Assert.That(b.SettingsContainerByType.Dirty, Is.True);
            
        Assert.That(cf.GetValue<string>(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("DEFAULT"));
        b.NoAutoSave.SettingsContainer.Save();
        cf.Load(SettingsFile);
        Assert.That(cf.GetValue<string>(b.NoAutoSave.Section, b.NoAutoSave.Name, "XXX"), Is.EqualTo("CHANGED"));
            
        // Change the data from the disk
        cf.Clear();
        cf.SetTypedValue(b.NoAutoSave.Section, b.NoAutoSave.Name, "FROM DISK");
        cf.SetTypedValue(b.NoEnabled.Section, b.NoEnabled.Name, "FROM DISK");
        cf.Save(SettingsFile);
        Assert.That(b.NoAutoSave.Value, Is.EqualTo("CHANGED"));
        Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE"));
        b.NoAutoSave.SettingsContainer.Load();
        Assert.That(b.NoAutoSave.Value, Is.EqualTo("FROM DISK"));
        Assert.That(b.NoEnabled.Value, Is.EqualTo("NEW VALUE")); // still the same, no matter the load
    }
        
    [TestRunner.Test]
    public void ConfigWithSettingContainerLoadTest() {
        var cf = new ConfigFile();
        cf.SetTypedValue("Section", "PixelPerfect", false);
        cf.SetTypedValue("Section", "Name", "CHANGED");
        cf.SetTypedValue("Video", "Screen", Resolutions.FULLHD_DIV1_875.Size);
        cf.Save(SettingsFile);
        cf.Clear();
        cf.Dispose();
            
        var di = new Container.Builder();
        di.Scan<ConfigWithSettingContainer>();
        di.Scan<Service1>();
        var c = di.Build();
        var b = c.Resolve<Service1>();
            
        // Stored values are read
        Assert.That(b.BoolSetting.Value, Is.False);
        Assert.That(b.StringSetting.Value, Is.EqualTo("CHANGED"));
        Assert.That(b.Resolution.Value, Is.EqualTo(Resolutions.FULLHD_DIV1_875));
    }
        
        
    [Configuration]
    internal class ConfigWithMultipleContainer {
        [Singleton(SettingsFile1)]
        public SettingsContainer SettingsContainer1 => new SettingsContainer(SettingsFile1);

        [Singleton(SettingsFile2)]
        public SettingsContainer SettingsContainer2 => new SettingsContainer(SettingsFile2);
            
        [Singleton("P1")] 
        public ISetting<bool> PixelPerfect => Setting<bool>.Persistent(SettingsFile1, "Section", "PixelPerfect", true);

        [Singleton] 
        public ISetting<string> P2 => Setting<string>.Persistent(SettingsFile2, "Section", "Name", "Default");
    }

    [Singleton]
    internal class Basic2 {
        [Inject] public ISetting<bool> P1 { get; set; }
        [Inject] public ISetting<string> P2 { get; set; }
        [Inject(SettingsFile1)] public SettingsContainer SettingsContainer1 { get; set; }
        [Inject(SettingsFile2)] public SettingsContainer SettingsContainer2 { get; set; }
    }

    [TestRunner.Test]
    public void ConfigWithMultipleContainerTest() {
        var di = new Container.Builder();
        di.Scan<ConfigWithMultipleContainer>();
        di.Scan<Basic2>();
        var c = di.Build();
        var b = c.Resolve<Basic2>();
            
        Assert.That(b.P1.SettingsContainer, Is.EqualTo(b.SettingsContainer1));
        Assert.That(b.P2.SettingsContainer, Is.EqualTo(b.SettingsContainer2));
        Assert.That(b.SettingsContainer1.FilePath, Is.EqualTo(SettingsFile1));
        Assert.That(b.SettingsContainer2.FilePath, Is.EqualTo(SettingsFile2));
    }
}