using System;
using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Veronenger.RTS.Assets.Trees; 

public partial class Trees : Node {

	public enum Id {
		Stump,
		MiniStump,
		Trunk,
		
		SmallTree1,
		SmallTree2,
		SmallTree5,
		SmallTree6,
		SmallTree9,
		SmallTree10,
		
		BigTree1,
		BigTree2,
	}

	private readonly Dictionary<Id, StaticBody2D> _enums = new();
	private readonly Dictionary<string, List<StaticBody2D>> _sets = new();

	public void Configure() {
		Parse(this);
		Enum.GetValues<Id>().ForEach(id => {
			if (!_enums.ContainsKey(id)) throw new Exception($"Missing id: {id}");
		});
	}

	public void Parse(Node parent) {
		parent.GetChildren().ForEach(child => {
			if (child is StaticBody2D staticBody2D) {
				AddToSet(parent, staticBody2D);
				AddToEnum(staticBody2D);
			} else {
				Parse(child);
			}
		});
	}

	private void AddToSet(Node node, StaticBody2D staticBody2D) {
		var name = node.Name.ToString()!;
		name = name.Contains('-') ? name.Split('-')[0] : name;
		if (!_sets.TryGetValue(name, out List<StaticBody2D> set)) {
			set = _sets[name] = new List<StaticBody2D>();
		}
		set.Add(staticBody2D);
	}

	private void AddToEnum(StaticBody2D sb) {
		var name = sb.Name.ToString();
		if (Enum.TryParse(name, out Id id)) {
			if (_enums.ContainsKey(id)) throw new Exception($"Duplicate id: {name}");
			_enums[id] = sb;
		}
	}

	public StaticBody2D Get(Id id) => _enums[id];
	public List<StaticBody2D> List(string name) => _sets[name];
	public StaticBody2D Duplicate(Id id) => (StaticBody2D)Get(id).Duplicate();
	
}
