using System;
using System.Linq;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

public static partial class Scene {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SingletonAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string Name { get; init; }
        public string Path { get; init; }
        public string? Tag { get; init; }
        public string? Flags { get; init; }

        public SingletonAttribute(string name) {
            Name = name;
        }

        public SingletonAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public void Apply(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(SingletonAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
            }
            Container.Builder.CustomFactoryProviders providers = null;
            providers = builder.RegisterFactory<T, SceneFactory<T>>(
                Lifetime.Singleton,
                () => {
                    var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
                    var isAutoload = providers.Provider.Metadata.TryGetValue("Autoload", out var flag) && flag is true;
                    sceneFactory.PreInject(loaderConfiguration.Name, isAutoload ? providers.Provider : null);
                    return sceneFactory;
                },
                Name,
                Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true));

        }
    }
}