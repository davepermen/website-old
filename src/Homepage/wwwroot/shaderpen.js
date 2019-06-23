var ShaderPen = function () {
    function createCanvas() {
        const canvas = document.createElement('canvas');
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        canvas.style.position = 'fixed';
        canvas.style.zIndex = -1;
        canvas.style.left = 0;
        canvas.style.top = 0;
        document.body.append(canvas);

        return canvas;
    }

    function createWebGLContext(canvas) {
        return canvas.getContext('webgl');
    }

    function createVertexShader(gl) {
        const vertexShader = gl.createShader(gl.VERTEX_SHADER);
        gl.shaderSource(vertexShader, `
          attribute vec2 position;
          void main() {
          gl_Position = vec4(position, 0.0, 1.0);
          }
        `);
        gl.compileShader(vertexShader);

        return vertexShader;
    }

    function createVertices(gl) {
        const vertices = new Float32Array([
            -1, 1, 1, 1, 1, -1,
            -1, 1, 1, -1, -1, -1,
        ]);
        const buffer = this.buffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
        gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW);

        return vertices;
    }

    function createFragmentShader(gl, shader) {
        const fragmentShader = gl.createShader(gl.FRAGMENT_SHADER);
        gl.shaderSource(fragmentShader, shader);
        gl.compileShader(fragmentShader);

        return fragmentShader;
    }

    function bindVertexParameters(gl, program) {
        var position = gl.getAttribLocation(program, 'position');
        gl.enableVertexAttribArray(position);
        gl.vertexAttribPointer(position, 2, gl.FLOAT, false, 0, 0);
    }

    function createProgram(gl, vertexShader, fragmentShader) {
        const program = gl.createProgram();
        gl.attachShader(program, vertexShader);
        gl.attachShader(program, fragmentShader);
        gl.linkProgram(program);
        gl.useProgram(program);

        bindVertexParameters(gl, program);

        return program;
    }

    var uniforms = [];

    function createUniform(gl, program, name, type, value) {
        var uniform = {
            location: gl.getUniformLocation(program, name),
            type,
            method: 'uniform' + (Array.isArray(value) ? value.length + 'fv' : '1' + type[0]),
            value,
            apply() {
                gl[this.method](this.location, this.value)
            }
        };
        uniforms.push(uniform);
        return uniform;
    }

    function updateUniforms() {
        uniforms.forEach(uniform => uniform.apply());
    }

    function createTextureFromArray(gl, width, height, array) {
        var texture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, texture);

        gl.texImage2D(gl.TEXTURE_2D, 0, gl.LUMINANCE, width, height, 0, gl.LUMINANCE, gl.UNSIGNED_BYTE, array);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.REPEAT);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.REPEAT);

        return texture;
    }

    function createUniformSampler(gl, program, name, textureNumber, texture) {
        var uniform = {
            location: gl.getUniformLocation(program, name),
            textureNumber,
            texture,
            apply() {
                gl.activeTexture(gl.TEXTURE0 + this.textureNumber);
                gl.bindTexture(gl.TEXTURE_2D, this.texture);
                gl.uniform1i(this.location, this.textureNumber);
            }
        }
        uniforms.push(uniform);
        return uniform;
    }

    function checkErrors(gl, vertexShader, fragmentShader, program) {
        if (!gl.getShaderParameter(vertexShader, gl.COMPILE_STATUS)) {
            console.error(gl.getShaderInfoLog(vertexShader));
        }

        if (!gl.getShaderParameter(fragmentShader, gl.COMPILE_STATUS)) {
            console.error(gl.getShaderInfoLog(fragmentShader));
        }

        if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
            console.error(gl.getProgramInfoLog(program));
        }
    }

    return function (shader) {

        const canvas = createCanvas();

        const gl = createWebGLContext(canvas);

        const vertexShader = createVertexShader(gl);
        const vertices = createVertices(gl);
        const fragmentShader = createFragmentShader(gl, shader);

        const program = createProgram(gl, vertexShader, fragmentShader);

        const resolution = createUniform(gl, program, 'resolution', 'vec3', [window.innerWidth, window.innerHeight, 0]);
        const time = createUniform(gl, program, 'time', 'float', 0);
        const delta = createUniform(gl, program, 'delta', 'float', 0);
        const frame = createUniform(gl, program, 'frame', 'int', 0);
        const mouse = createUniform(gl, program, 'mouse', 'vec4', [0, 0, 0, 0]);
        const ditherTexture = createTextureFromArray(gl, 8, 8, new Uint8Array([0, 32, 8, 40, 2, 34, 10, 42,   /* 8x8 Bayer ordered dithering  */
            48, 16, 56, 24, 50, 18, 58, 26,  /* pattern.  Each input pixel   */
            12, 44, 4, 36, 14, 46, 6, 38,  /* is scaled to the 0..63 range */
            60, 28, 52, 20, 62, 30, 54, 22,  /* before looking in this table */
            3, 35, 11, 43, 1, 33, 9, 41,   /* to determine the action.     */
            51, 19, 59, 27, 49, 17, 57, 25,
            15, 47, 7, 39, 13, 45, 5, 37,
            63, 31, 55, 23, 61, 29, 53, 21]));
        const dither = createUniformSampler(gl, program, 'dither', 0, ditherTexture);
        dither.apply();

        checkErrors(gl, vertexShader, fragmentShader, program);

        const render = timestamp => {
            let deltaTime = this.lastTime ? ((timestamp - this.lastTime) / 1000) : 0;
            this.lastTime = timestamp;

            time.value += deltaTime;
            delta.value = deltaTime;
            frame.value++;

            gl.clearColor(0, 0, 0, 0);
            gl.clear(gl.COLOR_BUFFER_BIT);

            updateUniforms();

            gl.drawArrays(gl.TRIANGLES, 0, vertices.length / 2);
            requestAnimationFrame(render);
        }

        window.addEventListener('mousemove', e => {
            mouse.value[0] = e.clientX;
            mouse.value[1] = e.clientY;
        });

        window.addEventListener('resize', e => {
            canvas.width = resolution.value[0] = window.innerWidth;
            canvas.height = resolution.value[1] = window.innerHeight;
            gl.viewport(0, 0, canvas.width, canvas.height);
        });

        render();
    }
}();