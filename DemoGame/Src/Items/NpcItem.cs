using Veronenger.Character.Enemy;

namespace Veronenger.Items;

public class NpcItem : Item  {
    public NpcStatus Status { get; private set; }
    
    public readonly NpcConfig Config;
    public INpcNode Node { get; private set; }

    public NpcItem(int id, string name, string alias, INpcNode npcNode, NpcConfig config) : base(id, name, alias) {
        Config = config;
        Node = npcNode;
        Status = new NpcStatus(config.InitialMaxHealth, config.InitialHealth);
    }
}