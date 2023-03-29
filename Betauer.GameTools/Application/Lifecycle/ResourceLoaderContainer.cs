using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.Signal;
using Betauer.DI.Attributes;
using Godot;

namespace Betauer.Application.Lifecycle;

public class ResourceLoaderContainer {
    public List<ResourceFactory> ResourceFactories { get; } = new();

    public Func<Task>? Awaiter { get; private set; }
    public event Action<ResourceProgress>? OnLoadResourceProgress;

    [Inject] public SceneTree SceneTree { get; set; }

    // Use ResourceFactory.SetResourceLoaderContainer() instead
    internal void Add(ResourceFactory resourceFactory) {
        if (ResourceFactories.Contains(resourceFactory)) return; // avoid duplicates
        ResourceFactories.Add(resourceFactory);
    }

    // Use ResourceFactory.SetResourceLoaderContainer() instead
    internal void Remove(ResourceFactory resourceFactory) {
        ResourceFactories.Remove(resourceFactory);
    }
                  
    public Task<TimeSpan> LoadResources(Action<ResourceProgress>? progressAction = null) {
        return LoadResources(new [] { ResourceFactory.DefaultTag }, progressAction);
    }

    public Task<TimeSpan> LoadResources(string tag, Action<ResourceProgress>? progressAction = null) {
        return LoadResources(new [] { tag }, progressAction);
    }

    public async Task<TimeSpan> LoadResources(string[] tags, Action<ResourceProgress>? progressAction = null) {
        var x = Stopwatch.StartNew();
        Func<Task> awaiter = Awaiter ?? (async () => {
            if (SceneTree != null) await SceneTree.AwaitProcessFrame();
            else await Task.Delay(10);
        });
        var resources = GetResourceFactories(tags)
            .Where(sf => !sf.IsLoaded())
            .Select(sf => new ResourceLoad(sf.Path, sf.Load))
            .ToList();
        
        await LoadTools.LoadThreaded(resources, awaiter, (rp) => {
            OnLoadResourceProgress?.Invoke(rp);
            progressAction?.Invoke(rp);
        });
        return x.Elapsed;
    }

    public void UnloadResources() {
        GetResourceFactories(ResourceFactory.DefaultTag).ForEach(sf => sf.Unload());
    }

    public void UnloadResources(string tag) {
        GetResourceFactories(tag).ForEach(sf => sf.Unload());
    }

    public void UnloadResources(string[] tags) {
        GetResourceFactories(tags).ForEach(sf => sf.Unload());
    }

    public IEnumerable<ResourceFactory> GetResourceFactories(string tag) {
        return ResourceFactories.Where(sf => sf.Tag == tag);
    }

    public IEnumerable<ResourceFactory> GetResourceFactories(string[] tags) {
        var set = new HashSet<string>(tags);
        return ResourceFactories.Where(sf => set.Contains(sf.Tag));
    }
}