namespace Veronenger.Items;

public class WeaponRangeItem : WeaponItem {
    public readonly WeaponConfig.Range Config;
    public float DamageFactor = 1f;
    public int EnemiesPerHit = 2;

    public override float Damage => Config.Damage * DamageFactor;

    internal WeaponRangeItem(int id, string name, string alias, WeaponConfig.Range config) : base(id, name, alias) {
        Config = config;
    }
}