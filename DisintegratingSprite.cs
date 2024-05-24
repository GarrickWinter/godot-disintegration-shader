using Godot;
using System;

[Tool]
public partial class DisintegratingSprite : Sprite3D
{
	[Export]
	public float disintegration_time = 1f;
	[Export]
	public bool disintegrating = true;
	[Export]
	public bool one_shot = false;

	private double lifespan = 0.0;
	private Vector2 texture_size;

	//Called when the sprite enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Engine.IsEditorHint())
		{
			GD.Print(string.Format("_Ready() in editor"));
		}

		//Properly position the sprite
		texture_size = Texture.GetSize();
		Centered = false;
		Offset = new(-texture_size.X / 2f, 0f);
		//We need to set the correct amount of particles here
		GpuParticles3D particles = GetNode<GpuParticles3D>("Particles");
		particles.Amount = (int)(texture_size.X * texture_size.Y);
		particles.Lifetime = disintegration_time;

		//We also need to set the bounds and offset of the emission box
		float emission_x = texture_size.X * PixelSize;
		float emission_y = texture_size.Y * PixelSize;
		ShaderMaterial particleShader = (ShaderMaterial)particles.ProcessMaterial;
		particleShader.SetShaderParameter("sprite_texture", Texture);
		particleShader.SetShaderParameter("emission_box_extents", new Vector3(emission_x, emission_y, 0f));
		particleShader.SetShaderParameter("emission_shape_offset", new Vector3(emission_x / -2f, 0f, 0f));
	}

	//Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ShaderMaterial spriteShader = (ShaderMaterial)MaterialOverride;
		GpuParticles3D particles = GetNode<GpuParticles3D>("Particles");
		if (disintegrating)
		{
			lifespan += delta;
			double ratio = lifespan / disintegration_time;
			//If we went over disintegration_time, ratio will go from very high to very low, causing the sprite to flicker to almost full visibility
			//To make sure that doesn't happen, if we exceeded the time, just set the cutoff to exactly 1f
			if (ratio >= 1.0)
			{
				lifespan = 0.0;
				spriteShader.SetShaderParameter("cutoff", 1f);
				if (one_shot)
				{
					disintegrating = false;
				}
			}
			else
			{
				spriteShader.SetShaderParameter("cutoff", ratio);
			}
		}
		
		if (disintegrating != particles.Emitting)
		{
			lifespan = 0.0;
			particles.Emitting = disintegrating;
			if (disintegrating == true)
			{
				particles.Lifetime = disintegration_time;
			}
		}

	}
}
