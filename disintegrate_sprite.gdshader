shader_type spatial;
render_mode unshaded;

uniform sampler2D tex:source_color,filter_linear_mipmap,repeat_disable;
uniform float cutoff:hint_range(0.0, 1.0, 0.01) = 0.0;

float calculate_fragment_ratio(vec2 uv, ivec2 texture_size)
{
	float row_size = float(texture_size.x);
	//The fill represents how many rows of the image are before the ratio-point
	//including the last row that's only partially before the ratio-point
	float pixel_fill = (uv.y * float(texture_size.y)) * row_size;
	//The trim represents how much of the final row is after the ratio-point
	float pixel_trim = (1.0 - uv.x) * row_size;
	float result = (pixel_fill - pixel_trim) / float(texture_size.x * texture_size.y);
	return result;
}

void vertex()
{
	
}

void fragment()
{
	vec4 tex_color = texture(tex, UV);
	ALBEDO = tex_color.rgb;
	//Calculate the cutoff
	float uv_ratio = calculate_fragment_ratio(UV, textureSize(tex, 0));
	float isCutOff = cutoff > uv_ratio ? 1.0 : 0.0;
	float alpha = (tex_color.a * (1.0 - isCutOff));
	ALPHA = min(alpha, tex_color.a);
}