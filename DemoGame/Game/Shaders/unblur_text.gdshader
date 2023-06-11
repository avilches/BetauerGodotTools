shader_type canvas_item;

uniform float threshold: hint_range(0.0, 1.0, 0.05);
uniform vec4 color : source_color;

void fragment() {
	vec4 input_color = texture(TEXTURE, UV);
	float value = clamp(sign(input_color.a - threshold) + 1.0, 0.0, 1.0);
	COLOR = vec4(color.rgb, value);
}