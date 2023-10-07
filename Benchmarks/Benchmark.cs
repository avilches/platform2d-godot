using BenchmarkDotNet.Attributes;
using Betauer.Core;
using Betauer.Core.PoissonDiskSampling;
using Godot;

namespace Benchmarks;

[MemoryDiagnoser(true)]
public class Benchmark {
    // private VariablePoissonSampler2DOriginal original;
    private VariablePoissonSampler2D better;
    private Random r1 = new Random(0);
    private Random r2 = new Random(0);
    private Random r3 = new Random(0);

    private const float minRadius = 6f;
    private const float maxRadius = 20f;
    
    
    public Benchmark() {
        better = new VariablePoissonSampler2D( 512, 512);
        a = 1;
        b = 2;
    }

    //
    // [Benchmark]
    // public List<Vector2> PoissonOriginal() {
    //     return original.Generate((x, y) => Range(r2, minRadius, maxRadius), minRadius, maxRadius);
    // }
    //
    // [Benchmark]
    // public List<Vector2> PoissonBetter() {
    //     return better.Generate((x, y) => Range(r3, minRadius, maxRadius), minRadius, maxRadius, r3);
    // }
    //
    // public static float Range(Random random, float start, float endExcluded) {
    //     if (start > endExcluded) (start, endExcluded) = (endExcluded, start);
    //     var limit = endExcluded - start;
    //     var rn = random.NextDouble() * limit;
    //     return (float)rn + start;
    // }

    public enum MyEnum {
        A,
        B,
        C,
    }

    [Benchmark]
    public void CompareEquals() {
        var x = MyEnum.A.Equals(MyEnum.B);
    }

    [Benchmark]
    public void CompareToInt() {
        var x = MyEnum.A.ToInt() == MyEnum.B.ToInt();
    }

    public int a = 1;
    public int b = 1;

    [Benchmark]
    public void CompareInts() {
        var x = a == b;
    }

}