shader_type canvas_item;

uniform vec4 color:source_color;
uniform float multiplier = 0.355f;
uniform float softness = 0.75f;

void fragment() {
	float value = distance(UV, vec2(0.5));
	COLOR = vec4(color.rgb, smoothstep(multiplier, softness, value));
}
