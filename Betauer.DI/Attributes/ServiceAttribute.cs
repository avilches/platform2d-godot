using System;
using System.Reflection;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Attributes;

public abstract class BaseServiceAttribute : BaseProviderAttribute {
}

public abstract class ServiceAttribute : BaseServiceAttribute {
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;
    public Lifetime Lifetime { get; }

    protected ServiceAttribute(Lifetime lifetime) {
        Lifetime = lifetime;
    }
}

public class SingletonAttribute : ServiceAttribute {
    public bool Lazy { get; set; } = false;
    public SingletonAttribute() : base(Lifetime.Singleton) {
    }

    public SingletonAttribute(string name) : base(Lifetime.Singleton) {
        Name = name;
    }
}

public class SingletonAttribute<T> : SingletonAttribute { }

public class TransientAttribute : ServiceAttribute {
    public TransientAttribute() : base(Lifetime.Transient) {
    }

    public TransientAttribute(string name) : base(Lifetime.Transient) {
        Name = name;
    }
}

public class TransientAttribute<T> : TransientAttribute { }

[AttributeUsage(AttributeTargets.Field)]
public abstract class ServiceTemplateAttribute : BaseServiceAttribute {
    public abstract ProviderTemplate CreateProviderTemplate(FieldInfo fieldInfo);
}
