using Betauer.DI;
using Betauer.DI.Attributes;
using Veronenger.Game.Items.Config;

namespace Veronenger.Game.Items;

public abstract class WeaponGameObject : PickableGameObject {
    public override WeaponConfig Config => (WeaponConfig)base.Config;
    
    [Inject] protected Container Container { get; set; }

    public float DamageBase;
    public float DamageFactor = 1f;
    public float Damage => DamageBase * DamageFactor;
}