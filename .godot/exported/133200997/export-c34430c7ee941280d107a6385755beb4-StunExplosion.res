RSRC                    VisualShader            ��������                                            b      resource_local_to_scene    resource_name    output_port_for_preview    default_input_values    expanded_output_ports    parameter_name 
   qualifier    default_value_enabled    default_value    script    texture_type    color_default    texture_filter    texture_repeat    texture_source    width    height    invert    in_3d_space    generate_mipmaps 	   seamless    seamless_blend_skirt    as_normal_map    bump_strength 
   normalize    color_ramp    noise    source    texture    input_name    op_type 	   operator    hint    min    max    step    code    graph_offset    mode    modes/blend    modes/depth_draw    modes/cull    modes/diffuse    modes/specular    flags/depth_prepass_alpha    flags/depth_test_disabled    flags/sss_mode_skin    flags/unshaded    flags/wireframe    flags/skip_vertex_transform    flags/world_vertex_coords    flags/ensure_correct_normals    flags/shadows_disabled    flags/ambient_light_disabled    flags/shadow_to_opacity    flags/vertex_lighting    flags/particle_trails    flags/alpha_to_coverage     flags/alpha_to_coverage_and_one    flags/debug_shadow_splits    nodes/vertex/0/position    nodes/vertex/connections    nodes/fragment/0/position    nodes/fragment/2/node    nodes/fragment/2/position    nodes/fragment/3/node    nodes/fragment/3/position    nodes/fragment/4/node    nodes/fragment/4/position    nodes/fragment/5/node    nodes/fragment/5/position    nodes/fragment/8/node    nodes/fragment/8/position    nodes/fragment/11/node    nodes/fragment/11/position    nodes/fragment/24/node    nodes/fragment/24/position    nodes/fragment/25/node    nodes/fragment/25/position    nodes/fragment/26/node    nodes/fragment/26/position    nodes/fragment/connections    nodes/light/0/position    nodes/light/connections    nodes/start/0/position    nodes/start/connections    nodes/process/0/position    nodes/process/connections    nodes/collide/0/position    nodes/collide/connections    nodes/start_custom/0/position    nodes/start_custom/connections     nodes/process_custom/0/position !   nodes/process_custom/connections    nodes/sky/0/position    nodes/sky/connections    nodes/fog/0/position    nodes/fog/connections        -   local://VisualShaderNodeColorParameter_dp4gx       1   local://VisualShaderNodeTexture2DParameter_k8b2y �         local://NoiseTexture2D_udo4v Q      &   local://VisualShaderNodeTexture_3jphs p      $   local://VisualShaderNodeInput_st7g0 �      '   local://VisualShaderNodeVectorOp_nggvm �      ,   local://VisualShaderNodeVectorCompose_qohc4 ^      +   local://VisualShaderNodeIntParameter_jwtct �      +   local://VisualShaderNodeIntParameter_ww5mp ,      '   local://VisualShaderNodeVectorOp_ec8vm �         local://VisualShader_a3o68 	         VisualShaderNodeColorParameter             ExplosionColor                  �?��	?      �?	      #   VisualShaderNodeTexture2DParameter             NoiseTexture                   	         NoiseTexture2D    	         VisualShaderNodeTexture                         	         VisualShaderNodeInput             uv 	         VisualShaderNodeVectorOp                    
                 
                              	         VisualShaderNodeVectorCompose                         �?           �?                       	         VisualShaderNodeIntParameter             ScaleX                   	         VisualShaderNodeIntParameter             ScaleY                   	         VisualShaderNodeVectorOp                                                                                  	         VisualShader    $      �  shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 ExplosionColor : source_color = vec4(1.000000, 0.537255, 0.000000, 1.000000);
uniform int ScaleX = 1;
uniform int ScaleY = 1;
uniform sampler2D NoiseTexture : hint_default_transparent, filter_nearest;



void fragment() {
// ColorParameter:2
	vec4 n_out2p0 = ExplosionColor;


// Input:5
	vec2 n_out5p0 = UV;


// IntParameter:24
	int n_out24p0 = ScaleX;


// IntParameter:25
	int n_out25p0 = ScaleY;


// VectorCompose:11
	vec2 n_out11p0 = vec2(float(n_out24p0), float(n_out25p0));


// VectorOp:8
	vec2 n_out8p0 = n_out5p0 * n_out11p0;


	vec4 n_out4p0;
// Texture2D:4
	n_out4p0 = texture(NoiseTexture, n_out8p0);


// VectorOp:26
	vec4 n_out26p0 = n_out2p0 * n_out4p0;


// Output:0
	ALBEDO = vec3(n_out26p0.xyz);
	ALPHA = n_out26p0.x;
	EMISSION = vec3(n_out26p0.xyz);


}
 <   
     �C   C>   
     HD  �D?             @   
     �  DA            B   
     �   DC            D   
     �C ��DE            F   
     �  kDG            H   
     �B ��DI            J   
      �  �DK            L   
     � ��DM            N   
     �  �DO         	   P   
     D  �DQ       ,                                                                                                                                                          	      RSRC