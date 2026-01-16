shader_type spatial;
render_mode unshaded, cull_disabled, blend_mix, depth_test_disabled;

uniform float radius = 5.5;
uniform float cell_size = 1.0;
uniform float line_width = 0.05;
uniform float edge_fade = 1.0;

uniform vec4 grid_color : source_color = vec4(1.0, 1.0, 1.0, 0.35);

varying vec3 v_local_pos;

void vertex()
{
    v_local_pos = VERTEX;
}

void fragment()
{
    // Convert local mesh space -> world-sized space
    vec2 world_local_xz = v_local_pos.xz * (radius * 2.0);

    // Distance from preview center
    float dist = length(world_local_xz);

    // Circular fade
    float circle = smoothstep(
        radius,
        radius - edge_fade,
        dist
    );

    // ---- GRID (CELL-CENTERED) ----
    vec2 grid_uv = (world_local_xz + (cell_size * 0.5)) / cell_size + 0.5;
    vec2 grid_frac = abs(fract(grid_uv) - 0.5);

    float line_x = smoothstep(0.0, line_width, grid_frac.x);
    float line_y = smoothstep(0.0, line_width, grid_frac.y);

    float grid_line = 1.0 - min(line_x, line_y);

    // Lines only
    ALBEDO = grid_color.rgb;
    ALPHA  = grid_color.a * grid_line * circle;
}