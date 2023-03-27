using System;
using Veronenger.Worlds;

namespace Veronenger.Config;

public abstract class PickableConfig {
    public Action<PickableItemNode>? Initialize;
}