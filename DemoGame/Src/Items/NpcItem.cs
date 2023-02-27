using Veronenger.Character.Enemy;

namespace Veronenger.Items;

public class NpcItem : Item  {
    public NpcStatus Status { get; private set; }
    
    public readonly NpcConfig Config;
    public INpcItemNode ItemNode { get; private set; }

    public NpcItem(int id, string name, string alias, INpcItemNode npcItemNode, NpcConfig config) : base(id, name, alias) {
        Config = config;
        ItemNode = npcItemNode;
        Status = new NpcStatus(config.InitialMaxHealth, config.InitialHealth);
    }
}