using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Application.Lifecycle;

public static class LoadTools {
    public static async Task<IEnumerable<Resource>> Load(IEnumerable<string> resourcePathsToLoadEnum, Func<Task> awaiter,
        Action<float>? progressAction = null) {
        progressAction?.Invoke(0f);
        var resourcePaths = resourcePathsToLoadEnum.ToArray();
        var count = 0f;
        return resourcePaths.Select(path => {
            count++;
            var resource = ResourceLoader.Load(path);
            if (resource == null) throw new ResourceLoaderException($"Resource {path} not found");
            progressAction?.Invoke(count / resourcePaths.Length);
            return resource;
        });
    }

    public static async Task<Dictionary<string, Resource>> LoadThreaded(List<string> resourcesPaths,
        Func<Task> awaiter, Action<ResourceProgress>? progressAction = null) {
        var resources = resourcesPaths.Select(path => new ResourceLoad(path)).ToList();
        await LoadThreaded(resources, awaiter, progressAction);
        return resources.Select(r => r.Resource!).ToDictionary(r => r.ResourcePath);
    }

    public static async Task LoadThreaded(List<ResourceLoad> resources, Func<Task> awaiter, Action<ResourceProgress>? progressAction = null) {
        if (awaiter == null) throw new ArgumentNullException(nameof(awaiter));
        var resourceProgress = new ResourceProgress(progressAction);
        resourceProgress.Update(0f, 0f, null);
        resources.ForEach(resource => {
            var error = ResourceLoader.LoadThreadedRequest(resource.Path);
            if (error != Error.Ok) throw new ResourceLoaderException($"Error requesting load {resource.Path}: {error}");
            resourceProgress.Update(0f, 0f, resource.Path);
        });
        float TotalProgress() => resources.Sum(r => r.Progress) / resources.Count;

        var pending = true;
        while (pending) {
            foreach (var resource in resources) {
                if (resource.Resource == null) {
                    var (status, progress) = ThreadLoadStatus(resource);
                    if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
                        resource.Progress = 1f;
                        resource.Resource = ResourceLoader.LoadThreadedGet(resource.Path);
                        resource.OnLoad(resource.Resource);
                        resourceProgress.Update(TotalProgress(), 1f, resource.Path);
                    } else if (status == ResourceLoader.ThreadLoadStatus.InProgress) {
                        resource.Progress = progress;
                        resourceProgress.Update(TotalProgress(), progress, resource.Path);
                    } else {
                        throw new ResourceLoaderException($"Error getting load status {resource.Path}: {status}");
                    }
                }
            }
            pending = resources.Any(r => r.Resource == null);
            if (pending) {
                resourceProgress.Update(TotalProgress(), 0, null);
                await awaiter();
            }
        }
        resourceProgress.Update(1f, 0f, null);
    }

    private static (ResourceLoader.ThreadLoadStatus, float) ThreadLoadStatus(ResourceLoad resource) {
        var progressArray = new Godot.Collections.Array();
        var status = ResourceLoader.LoadThreadedGetStatus(resource.Path, progressArray);
        return (status, (float)progressArray[0]);
    }
}

public class ResourceLoad {
    internal Resource? Resource;
    internal readonly string Path;
    internal float Progress = 0;
    private readonly Action<Resource>? _onLoad;

    public ResourceLoad(string path) {
        Path = path;
    }
    
    public ResourceLoad(string path, Action<Resource> onLoad) {
        Path = path;
        _onLoad = onLoad;
    }

    public void OnLoad(Resource resource) {
        _onLoad?.Invoke(resource);
    }
}