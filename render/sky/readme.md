Creating custom skybox in hdrp is not so well documented look here to get a first idea

https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@6.9/manual/Creating-a-Custom-Sky.html

Have a look in the unity repository to see how they implemeted it

https://github.com/Unity-Technologies/ScriptableRenderPipeline/tree/master/com.unity.render-pipelines.high-definition/Runtime/Sky

Good basic example is their gradient sky

https://github.com/Unity-Technologies/ScriptableRenderPipeline/tree/master/com.unity.render-pipelines.high-definition/Runtime/Sky/GradientSky

For the sky settings you need volume parameters to set them up

https://github.com/Unity-Technologies/ScriptableRenderPipeline/blob/bdfb084610194c7e87ba59720c2b7a3e70cd4027/com.unity.render-pipelines.core/Runtime/Volume/VolumeParameter.cs

