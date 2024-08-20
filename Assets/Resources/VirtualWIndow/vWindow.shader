Shader "Custom/vWindow"
{
    Properties
    {
       [IntRange] _vWindowID("vWindow ID", Range(0,255)) = 0
    }

    SubShader{
        Tags{
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass{
            Blend Zero One
            ZWrite Off

            Stencil{
                Ref[_vWindowID]
                Comp Always
                Pass Replace
            }
        }
    }
}
