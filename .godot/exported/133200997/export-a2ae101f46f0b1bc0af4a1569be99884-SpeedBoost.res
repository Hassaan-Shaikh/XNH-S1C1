RSRC                    VisualShader            ��������                                            E      resource_local_to_scene    resource_name    output_port_for_preview    default_input_values    expanded_output_ports    parameter_name 
   qualifier    default_value_enabled    default_value    script    input_name    op_type 	   operator 	   function    code    graph_offset    mode    modes/blend    flags/skip_vertex_transform    flags/unshaded    flags/light_only    nodes/vertex/0/position    nodes/vertex/connections    nodes/fragment/0/position    nodes/fragment/2/node    nodes/fragment/2/position    nodes/fragment/3/node    nodes/fragment/3/position    nodes/fragment/4/node    nodes/fragment/4/position    nodes/fragment/5/node    nodes/fragment/5/position    nodes/fragment/6/node    nodes/fragment/6/position    nodes/fragment/7/node    nodes/fragment/7/position    nodes/fragment/8/node    nodes/fragment/8/position    nodes/fragment/9/node    nodes/fragment/9/position    nodes/fragment/10/node    nodes/fragment/10/position    nodes/fragment/11/node    nodes/fragment/11/position    nodes/fragment/12/node    nodes/fragment/12/position    nodes/fragment/13/node    nodes/fragment/13/position    nodes/fragment/14/node    nodes/fragment/14/position    nodes/fragment/15/node    nodes/fragment/15/position    nodes/fragment/connections    nodes/light/0/position    nodes/light/connections    nodes/start/0/position    nodes/start/connections    nodes/process/0/position    nodes/process/connections    nodes/collide/0/position    nodes/collide/connections    nodes/start_custom/0/position    nodes/start_custom/connections     nodes/process_custom/0/position !   nodes/process_custom/connections    nodes/sky/0/position    nodes/sky/connections    nodes/fog/0/position    nodes/fog/connections        -   local://VisualShaderNodeColorParameter_i2ei4 o
      $   local://VisualShaderNodeInput_2yr2j �
      '   local://VisualShaderNodeVectorOp_xthy6       ,   local://VisualShaderNodeVec2Parameter_vxth5 �      %   local://VisualShaderNodeUVFunc_56ls2 �      ,   local://VisualShaderNodeVec2Parameter_ettgp       ,   local://VisualShaderNodeVec2Parameter_h5824 o      ,   local://VisualShaderNodeVec2Parameter_26xl0 �      $   local://VisualShaderNodeInput_3aqgd 6      ,   local://VisualShaderNodeVec2Parameter_k7k40 ~      ,   local://VisualShaderNodeVec2Parameter_kmqgc �      %   local://VisualShaderNodeUVFunc_puds3 /      '   local://VisualShaderNodeVectorOp_43hch b      '   local://VisualShaderNodeVectorOp_miygd �         local://VisualShader_rjwqa X         VisualShaderNodeColorParameter             Color                �� ?    ��k?  �?	         VisualShaderNodeInput              
      
   screen_uv 	         VisualShaderNodeVectorOp                              
                 
                              	         VisualShaderNodeVec2Parameter             Screen          	         VisualShaderNodeUVFunc              	         VisualShaderNodeVec2Parameter             ScreenScale          	         VisualShaderNodeVec2Parameter             ScreenOffset             
      �    	         VisualShaderNodeVec2Parameter             ScreenOffset2             
     ��    	         VisualShaderNodeInput              
      
   screen_uv 	         VisualShaderNodeVec2Parameter             ScreenScale2             
     @?    	         VisualShaderNodeVec2Parameter             Screen2          	         VisualShaderNodeUVFunc              	         VisualShaderNodeVectorOp                              
                 
                     	         VisualShaderNodeVectorOp                              
                 
                              	         VisualShader "           shader_type canvas_item;
render_mode blend_mix;

uniform vec4 Color : source_color = vec4(0.627451, 0.000000, 0.921569, 1.000000);
uniform vec2 Screen = vec2(0.000000, 0.000000);
uniform vec2 ScreenScale = vec2(0.000000, 0.000000);
uniform vec2 ScreenOffset = vec2(-0.500000, 0.000000);
uniform vec2 Screen2 = vec2(0.000000, 0.000000);
uniform vec2 ScreenScale2 = vec2(0.750000, 0.000000);
uniform vec2 ScreenOffset2 = vec2(-1.000000, 0.000000);



void fragment() {
// ColorParameter:2
	vec4 n_out2p0 = Color;


// Vector2Parameter:5
	vec2 n_out5p0 = Screen;


// Input:3
	vec2 n_out3p0 = SCREEN_UV;


// Vector2Parameter:7
	vec2 n_out7p0 = ScreenScale;


// Vector2Parameter:8
	vec2 n_out8p0 = ScreenOffset;


// UVFunc:6
	vec2 n_out6p0 = n_out8p0 * n_out7p0 + n_out3p0;


// VectorOp:4
	vec2 n_out4p0 = n_out5p0 - n_out6p0;


// Vector2Parameter:12
	vec2 n_out12p0 = Screen2;


// Input:10
	vec2 n_out10p0 = SCREEN_UV;


// Vector2Parameter:11
	vec2 n_out11p0 = ScreenScale2;


// Vector2Parameter:9
	vec2 n_out9p0 = ScreenOffset2;


// UVFunc:13
	vec2 n_out13p0 = n_out9p0 * n_out11p0 + n_out10p0;


// VectorOp:14
	vec2 n_out14p0 = n_out12p0 + n_out13p0;


// VectorOp:15
	vec2 n_out15p0 = max(n_out4p0, n_out14p0);


// Output:0
	COLOR.rgb = vec3(n_out2p0.xyz);
	COLOR.a = n_out15p0.x;


}
                       
     D  �B                
     �A  H�               
     f�  HC               
      �  �B               
     ��  �B             !   
     ��  �C"            #   
     p�  �C$            %   
     k�  %D&            '   
     �  �D(            )   
     �  %D*         	   +   
     �  fD,         
   -   
     4�  �C.            /   
     ��  >D0            1   
     �B  �C2            3   
     �C  HC4       8                                                                                                              
              	                                                                     	      RSRC