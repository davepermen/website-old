let fixLinks = () => {
    document.querySelectorAll('a').forEach(element => {
        element.addEventListener('click', event => {
            event.preventDefault()
            let href = element.getAttribute('href')
            page(href)
        })
    })
}

let fixForms = () => {
    document.querySelectorAll('form').forEach(element => {
        element.addEventListener('submit', async event => {
            event.preventDefault()
            let formData = new FormData(element)
            let action = element.getAttribute('action')
            let method = element.getAttribute('method')
            if (method === 'post') {
                await fetch(action, {
                    method,
                    body: formData,
                    redirect: 'manual'
                })
                page(location.pathname)
            } else {
                let queryParameters = [...formData.entries()].map(e => encodeURIComponent(e[0]) + "=" + encodeURIComponent(e[1])).join('&')
                page(action + '?' + queryParameters)
            }
        })
    });
}

let loadPage = async context => {
    let response = await (await fetch(context.path)).text()
    let body = {
        start: response.indexOf('<body>') + '<body>'.length,
        end: response.indexOf('</body>')
    }
    let content = response.substring(body.start, body.end)
    document.querySelector('body').innerHTML = content

    fixLinks()
    fixForms()
}

window.onload = () => {
    page('/', loadPage)
    page('/menu', loadPage)
    page({
        dispatch: false
    })

    fixLinks()
    fixForms()
}