precision highp float;
uniform vec3 resolution;
uniform float time;
uniform float delta;
uniform int frame;
uniform vec4 mouse;

uniform sampler2D dither;

float intersect(vec3 origin, vec3 direction, vec4 sphere)
{
    vec3 dist = origin - sphere.xyz;
    float B = dot(dist, direction);
    float C = dot(dist, dist) - sphere.w * sphere.w;
    float D = B * B - C;
    return D > 0. ? -B - sqrt(D) : -1.;
}

void main()
{
    vec3 col = vec3(1, 0, 0);

	vec3 origin = vec3(0, 0, 0);
	vec3 forward = vec3(0, 0, 1);
	vec3 up = vec3(0, 1, 0);
	vec3 right = vec3(1, 0, 0);

	float zoom = 5. * mouse.x / resolution.x;

	vec2 samplePosition = 2. * gl_FragCoord.xy / resolution.xy - 1.;
	samplePosition.y *= resolution.y / resolution.x;

	vec3 direction = normalize(samplePosition.x * right + samplePosition.y * up + forward * zoom);

	vec4 sphere = vec4(0, 0, 5, 1);

	float hit = intersect(origin, direction, sphere);
	if(hit > 0.) {
		vec3 hitpoint = origin + hit * direction;
		vec3 normal = normalize(hitpoint - sphere.xyz);

		vec2 pos = vec2(mouse.x, mouse.y);

		pos = 2. * pos - 1.;
		pos.y *= resolution.y / resolution.x;

		mat3 rot45 = mat3(
			0.707, -0.707, 0,
			0.707, 0.707, 0,
			0, 0, 1);

		vec2 center = resolution.xy * 0.5;
		vec2 center_to_mouse = pos.xy - center;
		
		vec3 col1 = vec3(1.0, 0.5, 0.25);
		vec3 col2 = vec3(0.375, 1.0, 0.25);
		vec3 col3 = vec3(0.25, 0.5, 1.0);
		vec3 col4 = vec3(0.825, 0.25, 1.0);
		
		col = col1 * clamp(dot(normal, normalize(vec3(pos.xy, -100) * rot45)), 0., 1.);
		col += col2 * clamp(dot(normal, normalize(vec3(pos.y, -pos.x, -100) * rot45)), 0., 1.);
		col += col3 * clamp(dot(normal, normalize(vec3(-pos.xy, -100) * rot45)), 0., 1.);
		col += col4 * clamp(dot(normal, normalize(vec3(-pos.y, pos.x, -100) * rot45)), 0., 1.);
	} else {
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

		col = col1 + col2 + col3 + col4;
	}

    vec3 tonemapped = 1.0 - exp(-col * (1.0 + 0.125 * sin(time)));

    gl_FragColor = vec4(tonemapped * tonemapped, 1.0);

	gl_FragColor.xyz += 2.0 * vec3(texture2D(dither, gl_FragCoord.xy / 8.0) / 32.0 - (1.0 / 128.0));
}