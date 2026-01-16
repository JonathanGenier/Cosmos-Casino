shader_type spatial;
render_mode unshaded, cull_disabled, blend_mix, depth_test_disabled;

uniform vec4 color : source_color = vec4(0.2, 0.8, 1.0, 0.35);

void fragment()
{
    ALBEDO = color.rgb;
    ALPHA = color.a;
}
