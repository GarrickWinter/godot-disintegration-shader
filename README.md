# Particle Shader to Disintegrate a Sprite3D in Godot 4.2
This is not a full guide, but is intended as an example and starting point for a way to achieve a disintegration effect (also sometimes called "dissolving") in Godot 4.2.

The effect achieved in this resource:

![An image of the Godot robot disintegrating into an upwards spray of particles.](https://i.imgur.com/dnGPswU.gif)

This approach works with a Sprite3D and a childed GPUParticles3D node. Both nodes have custom shaders; the Sprite3D has a simple spatial shader for managing the gradual disappearance of the sprite, while the GPUParticles3D has a more elaborate particle shader for creating particles based on the sprite's texture.

# Analyzing a Functional Particle Shader 
I started this with limited shader experience and zero particle shader experience. While there are some resources for achieving this effect in Godot 3.x, the way particle shaders are written changed in a few important ways in Godot 4. Unfortunately, I was unable to find any fully functioning examples of particle shaders for Godot 4 online, let alone any tutorials or guides to writing them.

In the end, I set up a GPUParticles3D with a ParticleProcessMaterial that emitted a spray of particles upwards. I then used the "Convert to ShaderMaterial" option, which generated a fully written and functioning particle shader I could dissect.

I proceeded to read through most of the shader line-by-line to figure out what it was doing. I commented the shader I found extensively, and removed a few variables that were unused; I've included that baseline particle shader here as "default_analyzed.tres", so that folks can take a look.

# Editor Previewing
When previewing the DisintegratingSprite in the editor, there are three script properties on the Sprite3D that help with editor previewing of the effect:

- **Disintegration Time**: This is how many seconds is takes for the sprite to disintegrate. The sprite uses this to change the lifetime of the particles, which in turn changes the speed of particle spawning, so changing the GPUParticles3D's Lifetime independently can break the disintegration effect. 
- **Disintegrating**: If true, the effect is playing. You can tick it on and off to effectively restart the effect.
- **One Shot**: If true, the sprite disintegrates once and then stays gone; otherwise, it repeats the disintegration cycle, which can be convenient but is also messy because the particles overlap and get mixed together.

# Credit

Thanks to **Astridson**, whose [much more refined shaders and shader work](https://github.com/Astridson/godot-disintegration-effect-examples) started me on the path to figuring this out for the newer particle shader system.

Thanks to **omggomb** on the Godot forums, [whose post here](https://forum.godotengine.org/t/where-can-i-find-a-copy-of-the-default-spatial-shader/19078) helped me understand how to extract a fully written-out shader from a default material, which was necessary for working through this problem.
