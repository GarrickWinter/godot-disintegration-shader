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
	private double persistent_ratio = 0.0;
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
		QuadMesh drawPass = particles.DrawPass1 as QuadMesh;
		drawPass.Size = new Vector2(PixelSize, PixelSize);

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
			//Particles are emitted at a constant rate every (lifetime/amount) seconds
			//Meaning (amount/lifetime) particles are emitted per second
			//For example, 1s particles in a system of 10 will emit 1 particle every 0.1 seconds, for 10 particles per second
			double particles_per_second = particles.Amount / particles.Lifetime;
			//That rate is per second, so we need to modify by delta to get the per-frame value
			double particles_this_frame = particles_per_second * delta;
			//Finally, compare this to the total number of particles/pixels expected
			double particle_ratio = particles_this_frame / (texture_size.X * texture_size.Y);
			//Add that to the persistent ratio
			persistent_ratio += particle_ratio;
			//If we went over disintegration_time, ratio will go from very high to very low, causing the sprite to flicker to almost full visibility
			//To make sure that doesn't happen, if we exceeded the time, just set the cutoff to exactly 1f
			if (persistent_ratio >= 1.0)
			{
				persistent_ratio = 0.0;
				spriteShader.SetShaderParameter("cutoff", 1f);
				if (one_shot)
				{
					disintegrating = false;
				}
			}
			else
			{
				spriteShader.SetShaderParameter("cutoff", persistent_ratio);
			}
		}
		
		if (disintegrating != particles.Emitting)
		{
			persistent_ratio = 0.0;
			particles.Emitting = disintegrating;
			if (disintegrating == true)
			{
				particles.Lifetime = disintegration_time;
			}
		}
	}
}
