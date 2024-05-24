shader_type spatial;
render_mode unshaded;

uniform sampler2D tex:source_color,filter_linear_mipmap,repeat_disable;
uniform float cutoff:hint_range(0.0, 1.0, 0.01) = 0.0;

void vertex()
{
	
}

void fragment()
{
	vec4 tex_color = texture(tex, UV);
	ALBEDO = tex_color.rgb;
	//Calculate the cutoff
	float isCutOff = cutoff > UV.y ? 1.0 : 0.0;
	float alpha = (tex_color.a * (1.0 - isCutOff));
	ALPHA = min(alpha, tex_color.a);
}