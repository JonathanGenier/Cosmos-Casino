shader_type spatial;

render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_toon;

uniform sampler2D flat_texture  : source_color;
uniform sampler2D slope_texture : source_color;

uniform float roughness = 1.0;
uniform float metallic  = 1.0;

// Grid
uniform float line_thickness  = 0.01;
uniform float line_visibility = 0.8;

// Blend width in tile UV2 space
uniform float blend_radius = 0.25;

void fragment()
{
	ROUGHNESS = roughness;
	METALLIC  = metallic;
	SPECULAR  = 0.0;

	// --------------------------------------------------
	// Grid overlay (UV)
	// --------------------------------------------------
	float lines = 1.0;
	float upper = 1.0 - line_thickness;

	if (
		UV.x < line_thickness || UV.x > upper ||
		UV.y < line_thickness || UV.y > upper
	) {
		lines = 1.0 - line_visibility;
	}

	// --------------------------------------------------
	// Textures
	// --------------------------------------------------
	vec3 flat_albedo  = texture(flat_texture,  UV).rgb;
	vec3 slope_albedo = texture(slope_texture, UV).rgb;

	// --------------------------------------------------
	// Tile mask from C#
	// COLOR.r = isSlope
	// COLOR.g = cardinal mask (0–1 → 4 bits)
	// COLOR.b = diagonal mask (0–1 → 4 bits)
	// --------------------------------------------------
	float is_slope = COLOR.r;

	vec3 final_albedo = flat_albedo;

	// --------------------------------------------------
	// SLOPE TILES → SOLID
	// --------------------------------------------------
	if (is_slope > 0.5)
	{
		final_albedo = slope_albedo;
	}
	else
	{
		// Decode neighbor masks
		int cardinal = int(COLOR.g * 15.0 + 0.5);
		int diagonal = int(COLOR.b * 15.0 + 0.5);

		bool n  = (cardinal & 1) != 0;
		bool e  = (cardinal & 2) != 0;
		bool s  = (cardinal & 4) != 0;
		bool w  = (cardinal & 8) != 0;

		bool ne = (diagonal & 1) != 0;
		bool se = (diagonal & 2) != 0;
		bool sw = (diagonal & 4) != 0;
		bool nw = (diagonal & 8) != 0;

		// Tile-local coordinates aligned to world directions
		vec2 p = vec2(UV2.x, 1.0 - UV2.y);

		float blend = 0.0;

		// Cardinal edges
		if (n) blend = max(blend, smoothstep(blend_radius, 0.0, 1.0 - p.y)); // North
		if (s) blend = max(blend, smoothstep(blend_radius, 0.0, p.y));       // South
		if (w) blend = max(blend, smoothstep(blend_radius, 0.0, p.x));       // West
		if (e) blend = max(blend, smoothstep(blend_radius, 0.0, 1.0 - p.x)); // East

		// Diagonal corners
		if (ne) blend = max(blend, smoothstep(blend_radius, 0.0, length(vec2(1.0 - p.x, 1.0 - p.y))));
		if (se) blend = max(blend, smoothstep(blend_radius, 0.0, length(vec2(1.0 - p.x, p.y))));
		if (sw) blend = max(blend, smoothstep(blend_radius, 0.0, length(vec2(p.x, p.y))));
		if (nw) blend = max(blend, smoothstep(blend_radius, 0.0, length(vec2(p.x, 1.0 - p.y))));

		final_albedo = mix(flat_albedo, slope_albedo, clamp(blend, 0.0, 1.0));
	}

	ALBEDO = final_albedo * lines;
}