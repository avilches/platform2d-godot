using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Tools {
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OnReadyAttribute : Attribute {
        public string Path;

        public OnReadyAttribute(string path) {
            Path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute {
    }

    public class DiRepository {
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private Logger _logger = LoggerFactory.GetLogger(typeof(DiRepository));

        public T AddSingleton<T>(T instance) {
            _singletons.Add(instance.GetType(), instance);
            return instance;
        }

        public T GetSingleton<T>(Type type) {
            return (T)_singletons[type];
        }

        public void Scan(Node node) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            int types = 0, added = 0;
            foreach (var assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    types++;
                    if (Attribute.GetCustomAttribute(type, typeof(SingletonAttribute),
                        false) is SingletonAttribute sa) {
                        var instance = CreateSingletonInstance(type);
                        AddSingleton(instance);
                        added++;
                        _logger.Info("Added Singleton " + type.Name + " (" + type.FullName + ", Assembly: " +
                                     assembly.FullName + ")");
                    }
                }
            }
            _logger.Info("Scanned "+types+" types. Added "+added);

            bool error = false;
            foreach (var instance in _singletons.Values) {
                error |= InjectFields(instance);
                if (instance is Node nodeInstance) {
                    nodeInstance.Name = nodeInstance.GetType().Name;
                    _logger.Info("Adding singleton node "+nodeInstance.GetType().Name+" as Bootstrap child");
                    node.AddChild(nodeInstance);
                }
            }
            if (error) {
                throw new Exception("Scan error. Check the console output");
            }
        }

        private object CreateSingletonInstance(Type type) {
            try {
                var emptyConstructor = type.GetConstructors().Single(info => info.GetParameters().Length == 0);
                var instance = emptyConstructor.Invoke(null);
                return instance;
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AutoWire(object instance) {
            var error = InjectFields(instance);
            if (error) {
                throw new Exception("AutoWire error in " + instance.GetType().FullName + ": Check the console output");
            }
        }

        private bool InjectFields(object target) {
            bool error = false;
            var publicFields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var privateFields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in privateFields.Concat(publicFields)) {
                if (!(Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                var found = _singletons.TryGetValue(property.FieldType, out object instance);
                if (!found) {
                    _logger.Error("Injectable property [" + property.FieldType.Name + " " + property.Name +
                                  "] not found while injecting fields in "+target.GetType().Name);
                    error = true;
                }
                property.SetValue(target, instance);
            }
            return error;
        }

        public void LoadOnReadyNodes(Node target) {
            var publicFields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var privateFields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in privateFields.Concat(publicFields)) {
                if (!(Attribute.GetCustomAttribute(property, typeof(OnReadyAttribute), false) is OnReadyAttribute
                    onReady))
                    continue;
                var instance = target.GetNode(onReady.Path);
                var fieldInfo = "[OnReady(\"" + onReady.Path + "\")] " + property.FieldType.Name + " " +
                                property.Name;
                if (instance == null) {
                    throw new Exception("OnReady path is null in field " + fieldInfo + ", class " +
                                        target.GetType().Name);
                } else if (instance.GetType() != property.FieldType) {
                    throw new Exception("OnReady path returned a wrong type (" + instance.GetType().Name +
                                        ") in field " + fieldInfo + ", class " +
                                        target.GetType().Name);
                }
                property.SetValue(target, instance);
            }
        }
    }

    public class Di {
        public Di() => DiBootstrap.DefaultRepository.AutoWire(this);
    }

    public class DiNode : Node {
        public DiNode() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public class DiNode2D : Node2D {
        public DiNode2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public class DiKinematicBody2D : KinematicBody2D {
        public DiKinematicBody2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public class DiCamera2D : Camera2D {
        public DiCamera2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }

    public class DiArea2D : Area2D {
        public DiArea2D() => DiBootstrap.DefaultRepository.AutoWire(this);

        public sealed override void _Ready() {
            DiBootstrap.DefaultRepository.LoadOnReadyNodes(this);
            Ready();
        }

        public virtual void Ready() {
        }
    }


    /**
     * DiBootstrap. Singleton + Node + Special Di (scan all + autowire ifself)
     */
    public abstract class DiBootstrap : Node /* needed because 1) it's an autoload 2) all Node singletons scanned will be added as child */ {
        public static DiRepository DefaultRepository;
        public static DiBootstrap Instance;

        public DiBootstrap() {
            if (Instance != null) {
                throw new Exception("DiBootstrap can't be instantiated more than once: " + GetType().Name);
            }
            Instance = this;
            DefaultRepository = CreateDiRepository();
            DefaultRepository.Scan(this);
            DefaultRepository.AutoWire(this);
        }

        public virtual DiRepository CreateDiRepository() {
            return new DiRepository();
        }
    }

}