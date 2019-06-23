precision highp float;
uniform vec3 resolution;
uniform float time;
uniform float delta;
uniform int frame;
uniform vec4 mouse;

uniform sampler2D dither;

void main()
{
    vec2 pos = vec2(mouse.x, resolution.y - mouse.y);

    mat2 rot45 = mat2(0.707, -0.707, 0.707, 0.707);

    vec2 center = resolution.xy * 0.5;
    vec2 center_to_mouse = pos.xy - center;

    vec2 pos1 = center + center_to_mouse * rot45;
    vec2 pos2 = center + vec2(center_to_mouse.y, -center_to_mouse.x) * rot45;
    vec2 pos3 = center - center_to_mouse * rot45;
    vec2 pos4 = center + vec2(-center_to_mouse.y, center_to_mouse.x) * rot45;

    float scale = 3.75;

    vec3 col1 = vec3(1.0, 0.5, 0.25) * scale / sqrt(distance(gl_FragCoord.xy, pos1));
    vec3 col2 = vec3(0.375, 1.0, 0.25) * scale / sqrt(distance(gl_FragCoord.xy, pos2));
    vec3 col3 = vec3(0.25, 0.5, 1.0) * scale / sqrt(distance(gl_FragCoord.xy, pos3));
    vec3 col4 = vec3(0.825, 0.25, 1.0) * scale / sqrt(distance(gl_FragCoord.xy, pos4));

    vec3 col = col1 + col2 + col3 + col4;

    vec3 tonemapped = 1.0 - exp(-col * (1.0 + 0.125 * sin(time)));

    gl_FragColor = vec4(tonemapped * tonemapped, 1.0);

	gl_FragColor.xyz += 2.0 * vec3(texture2D(dither, gl_FragCoord.xy / 8.0) / 32.0 - (1.0 / 128.0));
}