using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Veronenger.RTS.Assets; 

public partial class Trees : Node {

	public enum Id {
		Stump,
		MiniStump,
		Trunk
	}

	private readonly Dictionary<Id, StaticBody2D> _data = new();
	public void Configure() {
		GetChildren().OfType<StaticBody2D>().ForEach(sb => {
			var name = sb.Name.ToString();
			if (!Enum.TryParse(name, out Id id)) throw new Exception($"Name without id: {name}"); 
			if (_data.ContainsKey(id)) throw new Exception($"Duplicate id: {name}");
			_data[id] = sb;
		});
		Enum.GetValues<Id>().ForEach(id => {
			if (!_data.ContainsKey(id)) throw new Exception($"Missing id: {id}");
		});
	}
	
	public StaticBody2D Get(Id id) => _data[id];
	public StaticBody2D Duplicate(Id id) => (StaticBody2D)Get(id).Duplicate();
	
}
